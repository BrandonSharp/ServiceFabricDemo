using BookList;
using BookList.BookActor.Interfaces;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Services.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace BookList.GatewayApi.Controllers {
    [ServiceRequestActionFilter]
    [RoutePrefix("book")]
    public class BookController : ApiController {
        readonly string libraryServiceName = "Library";
        readonly string bookActorServiceName = "BookActorService";

        [HttpGet]
        [Route("sayhello")]
        public async Task<IHttpActionResult> SayHello() {
            try {
                var libraryServiceUri = new ServiceUriBuilder(libraryServiceName).ToUri();
                ServicePartitionKey partitionKey = new ServicePartitionKey(0);

                HttpClient client = new HttpClient();
                var response = await client.SendToServiceAsync(libraryServiceUri,
                    partitionKey,
                    () => new HttpRequestMessage(HttpMethod.Get, $"book/sayhello"));

                var responseData = await response.Content.ReadAsStringAsync();

                return this.Ok(responseData);
            } catch (AggregateException ex) {
                return InternalServerError(ex.InnerException);
            }
        }

        private IBookActor GetActor(string bookName) {
            var actorId = new ActorId(bookName);
            ServiceUriBuilder serviceUri = new ServiceUriBuilder(bookActorServiceName);

            var actor = ActorProxy.Create<IBookActor>(actorId, serviceUri.ToUri());
            return actor;
        }
    }
}
