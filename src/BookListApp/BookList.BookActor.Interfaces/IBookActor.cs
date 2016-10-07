using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;

namespace BookList.BookActor.Interfaces {
    public interface IBookActor : IActor {
        Task<BookInformation> CreateBook(string name, string author, int pageCount);
        Task<BookCheckoutResponse> TryCheckoutBook(string user);
        Task<BookStatus> ReturnBook(string user);
        Task<BookStatus> GetBookStatus();
        Task<BookInformation> GetBookInformation();
    }
}
