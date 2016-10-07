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

        public Task<bool> CheckoutBook() {
            throw new NotImplementedException();
        }

        public Task<BookStatus> GetBookStatus() {
            throw new NotImplementedException();
        }

        public Task<BookInformation> GetBookInformation() {
            return this.StateManager.GetStateAsync<BookInformation>("info");
        }

        public Task<BookStatus> ReturnBook() {
            throw new NotImplementedException();
        }

        /// <summary>
        /// This method is called whenever an actor is activated.
        /// An actor is activated the first time any of its methods are invoked.
        /// </summary>
        protected override Task OnActivateAsync() {
            ActorEventSource.Current.ActorMessage(this, "Actor activated.");

            var bookName = this.GetActorId().GetStringId();
            var pageCount = new Random().Next(100, 700);

            var info = new BookInformation() { Name = bookName, Pages = pageCount };

            return this.StateManager.TryAddStateAsync("info", info);
        }
    }
}
