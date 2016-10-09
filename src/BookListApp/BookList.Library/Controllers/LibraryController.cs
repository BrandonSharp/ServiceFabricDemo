using BookList.Library.Models;
using Microsoft.ServiceFabric.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace BookList.Library.Controllers {
    [RoutePrefix("book")]
    public class BookController : ApiController {
        IReliableStateManager stateManager;
        public BookController(IReliableStateManager stateManager) {
            this.stateManager = stateManager;
        }

        [HttpGet]
        [Route("sayhello")]
        public string SayHello() {
            return "hello!";
        }

        [HttpPost]
        [Route("{isbn}")]
        public async Task<IHttpActionResult> CreateBook(string isbn, [FromBody] BookCreationRequest bookInfo) {
            try {
                // TODO: Initialize actor

                // TODO: If actor creation successful, store book isbn & name in a reliable dictionary


            } catch (Exception ex) {
                return this.InternalServerError(ex);
            }

            return this.Ok();
        }
    }
}
