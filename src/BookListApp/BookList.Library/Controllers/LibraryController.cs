using BookList.BookActor.Interfaces;
using BookList.Library.Models;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace BookList.Library.Controllers {
    [RoutePrefix("library")]
    public class LibraryController : ApiController {
        IReliableStateManager stateManager;
        readonly string bookActorServiceName = "BookActorService";

        public LibraryController(IReliableStateManager stateManager) {
            this.stateManager = stateManager;
        }

        [HttpPost]
        [Route("book/{isbn}")]
        public async Task<IHttpActionResult> CreateBook(string isbn, [FromBody] BookCreationRequest bookInfo) {
            try {
                var actor = GetActor(isbn);
                var info = await actor.CreateBook(bookInfo.Name, bookInfo.Author, bookInfo.PageCount);

                var bookList = await this.stateManager.GetOrAddAsync<IReliableDictionary<string, string>>("bookList");
                using (var tx = this.stateManager.CreateTransaction()) {
                    await bookList.SetAsync(tx, info.Isbn, info.Name);
                    await tx.CommitAsync();
                }
            } catch (Exception ex) {
                return this.InternalServerError(ex);
            }

            return this.Ok();
        }

        [HttpGet]
        [Route("book-list")]
        public async Task<IHttpActionResult> GetBookList() {
            var bookList = await this.stateManager.GetOrAddAsync<IReliableDictionary<string, string>>("bookList");
            Dictionary<string, string> results = new Dictionary<string, string>();

            using (var tx = stateManager.CreateTransaction()) {
                var enumerator = (await bookList.CreateEnumerableAsync(tx)).GetAsyncEnumerator();
                while(await enumerator.MoveNextAsync(CancellationToken.None)) {
                    results.Add(enumerator.Current.Key, enumerator.Current.Value);
                }
            }
            
            return this.Ok(results);
        }

        private IBookActor GetActor(string isbn) {
            var actorId = new ActorId(isbn);
            ServiceUriBuilder serviceUri = new ServiceUriBuilder(bookActorServiceName);

            var actor = ActorProxy.Create<IBookActor>(actorId, serviceUri.ToUri());
            return actor;
        }
    }
}
