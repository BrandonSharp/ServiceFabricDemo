using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;

namespace BookList.BookActor.Interfaces {
    public interface IBookActor : IActor {
        Task<BookInformation> CreateBookAsync(string name, string author, int pageCount);
        Task<BookCheckoutResponse> TryCheckoutBookAsync(string user);
        Task<BookStatus> ReturnBookAsync(string user);
        Task<BookStatus> GetBookStatusAsync();
        Task<BookInformation> GetBookInformationAsync();
    }
}
