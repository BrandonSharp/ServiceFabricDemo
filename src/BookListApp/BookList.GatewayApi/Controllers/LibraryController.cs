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
    [RoutePrefix("api/library")]
    public class LibraryController : ApiController {
        readonly string libraryServiceName = "Library";

        [HttpGet]
        [Route("book-list")]
        public async Task<IHttpActionResult> GetCurrentBookList() {
            var message = await MakeServiceGetRequest(libraryServiceName, new ServicePartitionKey(0), $"library/book-list");
            
            return this.Ok(await message.Content.ReadAsAsync<Dictionary<string, string>>());
        }

        async Task<HttpResponseMessage> MakeServiceGetRequest(string serviceName, ServicePartitionKey partitionKey, string route) {
            var serviceUri = new ServiceUriBuilder(serviceName).ToUri();

            HttpClient client = new HttpClient();
            var response = await client.SendToServiceAsync(serviceUri,
                partitionKey,
                () => new HttpRequestMessage(HttpMethod.Get, route));

            return response;
        }
    }
}
