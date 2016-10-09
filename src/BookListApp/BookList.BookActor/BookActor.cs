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
            var isbn = this.GetActorId().GetStringId();
            var info = new BookInformation() { Isbn = isbn, Name = name, Author = author, Pages = pageCount };
            var stateAdded = await this.StateManager.TryAddStateAsync("info", info);

            if(stateAdded) {
                return info;
            } else {
                throw new ApplicationException("The book could not be created. (Has it been created before?)");
            }
        }

        public async Task<BookCheckoutResponse> TryCheckoutBook(string user) {
            bool isCheckoutSuccessful = false;

            var status = await GetBookStatus();

            if(status.IsAvailable) {
                status.IsAvailable = false;
                status.UserWithCurrentLoan = user;
                status.WaitlistLength = 0;

                isCheckoutSuccessful = true;
            } else {
                var queue = await GetQueuedUsers();
                queue.Enqueue(user);
                status.WaitlistLength = queue.Count;

                await this.StateManager.SetStateAsync("userQueue", queue);
            }
            await this.StateManager.SetStateAsync("status", status);

            return new BookCheckoutResponse() { IsSuccessful = isCheckoutSuccessful, Status = status };
        }

        public async Task<BookStatus> ReturnBook(string user) {
            var status = await GetBookStatus();
            if(status.UserWithCurrentLoan != user) {
                throw new ApplicationException("That user is not the one holding the current loan.");
            }

            var queue = await GetQueuedUsers();
            if(queue.Any()) {
                var nextUser = queue.Dequeue();
                status.UserWithCurrentLoan = nextUser;
                status.WaitlistLength = queue.Count;
            } else {
                status.IsAvailable = true;
                status.UserWithCurrentLoan = null;
                status.WaitlistLength = 0;
            }
            await this.StateManager.SetStateAsync("status", status);

            return status;
        }

        public Task<BookStatus> GetBookStatus() {
            return this.StateManager.GetOrAddStateAsync<BookStatus>("status", new BookStatus() { IsAvailable = true, WaitlistLength = 0 });
        }

        public Task<BookInformation> GetBookInformation() {
            return this.StateManager.GetStateAsync<BookInformation>("info");
        }

        async Task<Queue<string>> GetQueuedUsers() {
            return await this.StateManager.GetOrAddStateAsync("userQueue", new Queue<string>());
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
