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
        public async Task<IHttpActionResult> CreateBook(string isbn, string name, string author, int pageCount) {
            try {
                throw new NotImplementedException();
            } catch (Exception ex) {
                return this.InternalServerError(ex);
            }

            return this.Ok();
        }
    }
}
