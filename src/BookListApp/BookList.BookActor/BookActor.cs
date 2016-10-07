using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Actors.Client;
using BookList.BookActor.Interfaces;

namespace BookList.BookActor {
    [StatePersistence(StatePersistence.Persisted)]
    internal class BookActor : Actor, IBookActor {

        public BookActor(ActorService actorService, ActorId actorId)
            : base(actorService, actorId) {
        }

        public async Task<BookInformation> CreateBook(string name, string author, int pageCount) {
            // TODO: Infer ISBN from actor ID, create BookInformation, and store. Return info is storage is successful.

            throw new NotImplementedException();
        }

        public Task<BookCheckoutResponse> CheckoutBook(string user) {
            throw new NotImplementedException();
        }

        public Task<BookStatus> ReturnBook(string user) {
            throw new NotImplementedException();
        }

        public Task<BookStatus> GetBookStatus() {
            throw new NotImplementedException();
        }

        public Task<BookInformation> GetBookInformation() {
            return this.StateManager.GetStateAsync<BookInformation>("info");
        }

        /// <summary>
        /// This method is called whenever an actor is activated.
        /// An actor is activated the first time any of its methods are invoked.
        /// </summary>
        protected override Task OnActivateAsync() {
            ActorEventSource.Current.ActorMessage(this, "Actor activated.");

            return Task.FromResult(true);
        }
    }
}
