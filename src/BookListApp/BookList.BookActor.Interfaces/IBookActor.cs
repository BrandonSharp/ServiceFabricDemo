using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;

namespace BookList.BookActor.Interfaces {
    public interface IBookActor : IActor {
        Task<bool> CreateBook(string name, string author, int pageCount);
        Task<bool> CheckoutBook();
        Task<BookStatus> ReturnBook();
        Task<BookStatus> GetBookStatus();
        Task<BookInformation> GetBookInformation();
    }
}
