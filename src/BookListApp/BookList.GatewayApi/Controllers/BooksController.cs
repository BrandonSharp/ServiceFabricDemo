﻿using BookList;
using BookList.BookActor.Interfaces;
using BookList.GatewayApi.Models;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Communication.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace BookList.GatewayApi.Controllers {
    [ServiceRequestActionFilter]
    [RoutePrefix("api/books")]
    public class BooksController : ApiController {
        readonly string libraryServiceName = "Library";
        readonly string bookActorServiceName = "BookActorService";

        private readonly HttpCommunicationClientFactory clientFactory = new HttpCommunicationClientFactory(
                ServicePartitionResolver.GetDefault(),
                "EndpointName",
                TimeSpan.FromSeconds(10),
                TimeSpan.FromSeconds(3));


        [HttpPost]
        [Route("{isbn}")]
        public async Task<IHttpActionResult> CreateBook(string isbn, [FromBody] BookCreationRequest bookInfo) {
            var success = await PostDataToService(libraryServiceName, new ServicePartitionKey(0), $"library/book/{isbn}", bookInfo);

            if (success) {
                return this.CreatedAtRoute(nameof(GetBookInformation), isbn, bookInfo);
            } else {
                return this.BadRequest();
            }
        }

        [HttpGet]
        [Route("{isbn}", Name = nameof(GetBookInformation))]
        public async Task<IHttpActionResult> GetBookInformation(string isbn) {
            var actor = GetActor(isbn);
            return this.Ok(await actor.GetBookInformationAsync());
        }

        [HttpGet]
        [Route("{isbn}/status")]
        public async Task<IHttpActionResult> GetBookStatus(string isbn) {
            var actor = GetActor(isbn);
            return this.Ok(await actor.GetBookStatusAsync());
        }

        [HttpPut]
        [Route("{isbn}/checkout")]
        public async Task<IHttpActionResult> CheckoutBook(string isbn, string user) {
            var actor = GetActor(isbn);
            var results = await actor.TryCheckoutBookAsync(user);
            return this.Ok(results);
        }

        [HttpPut]
        [Route("{isbn}/return")]
        public async Task<IHttpActionResult> ReturnBook(string isbn, string user) {
            var actor = GetActor(isbn);
            var results = await actor.ReturnBookAsync(user);
            return this.Ok(results);
        }

        async Task<HttpResponseMessage> MakeServiceGetRequest(string serviceName, ServicePartitionKey partitionKey, string route) {
            var serviceUri = new ServiceUriBuilder(serviceName).ToUri();

            HttpClient client = new HttpClient();
            var response = await client.SendToServiceAsync(serviceUri,
                partitionKey,
                () => new HttpRequestMessage(HttpMethod.Get, route));

            return response;
        }

        async Task<bool> PostDataToService<T>(string serviceName, ServicePartitionKey partitionKey, string route, T payload) {
            var serviceUri = new ServiceUriBuilder(serviceName).ToUri();

            ServicePartitionClient<HttpCommunicationClient> servicePartitionClient =
                new ServicePartitionClient<HttpCommunicationClient>(
                    this.clientFactory,
                    serviceUri,
                    partitionKey);

            var result = await servicePartitionClient.InvokeWithRetryAsync(
                client => {
                    Uri serviceAddress = new Uri(client.BaseAddress, route);
                    HttpWebRequest request = WebRequest.CreateHttp(serviceAddress);
                    request.Method = "POST";
                    request.ContentType = "application/json";
                    request.Timeout = (int)client.OperationTimeout.TotalMilliseconds;
                    request.ReadWriteTimeout = (int)client.ReadWriteTimeout.TotalMilliseconds;

                    try {
                        using (Stream requestStream = request.GetRequestStream()) {
                            using (BufferedStream buffer = new BufferedStream(requestStream)) {
                                using (StreamWriter writer = new StreamWriter(buffer)) {
                                    JsonSerializer serializer = new JsonSerializer();
                                    serializer.Serialize(writer, payload);
                                    buffer.Flush();
                                }
                            }
                        }

                        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse()) {
                            return Task.FromResult(true);
                        }
                    } catch (WebException ex) {
                        string message;
                        if (ex.Response != null) {
                            var responseStream = ex.Response.GetResponseStream();
                            responseStream.Seek(0, SeekOrigin.Begin);
                            using (StreamReader sr = new StreamReader(responseStream)) {
                                message = sr.ReadToEnd();
                            }
                        } else { message = "<no message>"; }

                        ServiceEventSource.Current.Message($"EXCEPTION: WebException {ex.Status} caught when trying to POST to {serviceAddress}{Environment.NewLine}{message}");
                        return Task.FromResult(false);
                    }

                }, CancellationToken.None);

            return result;
        }

        private IBookActor GetActor(string isbn) {
            var actorId = new ActorId(isbn);
            ServiceUriBuilder serviceUri = new ServiceUriBuilder(bookActorServiceName);

            var actor = ActorProxy.Create<IBookActor>(actorId, serviceUri.ToUri());
            return actor;
        }
    }
}
