using BookList;
using BookList.BookActor.Interfaces;
using BookList.GatewayApi.Models;
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
    [RoutePrefix("api/books")]
    public class BooksController : ApiController {
        readonly string libraryServiceName = "Library";
        readonly string bookActorServiceName = "BookActorService";

        [HttpPost]
        [Route("{isbn}")]
        public async Task<IHttpActionResult> CreateBook(string isbn, [FromBody] BookCreationRequest bookInfo) {
            // TODO: Forward post request to library service

            return this.CreatedAtRoute(nameof(GetBookInformation), isbn, bookInfo);
        }

        [HttpGet]
        [Route("{isbn}", Name = nameof(GetBookInformation))]
        public async Task<IHttpActionResult> GetBookInformation(string isbn) {
            // TODO: Call actor for book info

            return this.NotFound();
        }

        [HttpGet]
        [Route("sayhello")]
        public async Task<IHttpActionResult> SayHello() {
            try {
                var response = await MakeServiceGetRequest(libraryServiceName, new ServicePartitionKey(0), $"book/sayhello");
                var responseData = await response.Content.ReadAsStringAsync();

                return this.Ok(responseData);
            } catch (AggregateException ex) {
                return InternalServerError(ex.InnerException);
            }
        }

        async Task<HttpResponseMessage> MakeServiceGetRequest(string serviceName, ServicePartitionKey partitionKey, string route) {
            var serviceUri = new ServiceUriBuilder(serviceName).ToUri();

            HttpClient client = new HttpClient();
            var response = await client.SendToServiceAsync(serviceUri,
                partitionKey,
                () => new HttpRequestMessage(HttpMethod.Get, route));

            return response;
        }

        private IBookActor GetActor(string isbn) {
            var actorId = new ActorId(isbn);
            ServiceUriBuilder serviceUri = new ServiceUriBuilder(bookActorServiceName);

            var actor = ActorProxy.Create<IBookActor>(actorId, serviceUri.ToUri());
            return actor;
        }
    }
}
