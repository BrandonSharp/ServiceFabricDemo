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
    }
}
