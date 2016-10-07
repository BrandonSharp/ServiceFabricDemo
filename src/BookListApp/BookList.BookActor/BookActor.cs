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

        public Task<BookCheckoutResponse> TryCheckoutBook(string user) {
            // TODO: Check book status to see if it's available. 
            //      If so, mark it unavailable, checked out to the requesting user, and return response
            //      If not, add the user to the queue, and return response

            throw new NotImplementedException();
        }

        public Task<BookStatus> ReturnBook(string user) {
            // TODO: Check if the user returning is the user the book is on loan to. Throw exception if not.
            // Check the queue, and assign ownership to the next user in the queue, if any.
            // If no queued users, then mark the book as available.

            throw new NotImplementedException();
        }

        public Task<BookStatus> GetBookStatus() {
            // TODO: Return the current status of the book

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
