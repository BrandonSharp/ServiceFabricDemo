using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Communication.Client;
using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BookList {
    public static class HttpClientExtensions {
        private static FabricClient fabricClient = new FabricClient();

        private static HttpCommunicationClientFactory clientFactory = new HttpCommunicationClientFactory(
            ServicePartitionResolver.GetDefault(),
            "endpointName",
            TimeSpan.FromSeconds(2),
            TimeSpan.FromSeconds(2));


        public static Task<HttpResponseMessage> SendToServiceAsync(
            this HttpClient instance, Uri serviceInstanceUri, long partitionKey, Func<HttpRequestMessage> createRequest,
            CancellationToken cancellationToken = default(CancellationToken)) {
            return SendToServiceAsync(instance, serviceInstanceUri, new ServicePartitionKey(partitionKey), createRequest, cancellationToken);
        }

        public static Task<HttpResponseMessage> SendToServiceAsync(
            this HttpClient instance, Uri serviceInstanceUri, ServicePartitionKey partitionKey, Func<HttpRequestMessage> createRequest,
            CancellationToken cancellationToken = default(CancellationToken)) {
            ServicePartitionClient<HttpCommunicationClient> servicePartitionClient = new ServicePartitionClient<HttpCommunicationClient>(
                clientFactory,
                serviceInstanceUri,
                partitionKey);

            return MakeHttpRequest(instance, createRequest, cancellationToken, servicePartitionClient);
        }

        public static Task<HttpResponseMessage> SendToServiceAsync(
            this HttpClient instance, Uri serviceInstanceUri, Func<HttpRequestMessage> createRequest,
            CancellationToken cancellationToken = default(CancellationToken)) {
            ServicePartitionClient<HttpCommunicationClient> servicePartitionClient = new ServicePartitionClient<HttpCommunicationClient>(
                clientFactory,
                serviceInstanceUri);

            return MakeHttpRequest(instance, createRequest, cancellationToken, servicePartitionClient);
        }

        private static Task<HttpResponseMessage> MakeHttpRequest(
            HttpClient instance, Func<HttpRequestMessage> createRequest, CancellationToken cancellationToken,
            ServicePartitionClient<HttpCommunicationClient> servicePartitionClient) {
            return servicePartitionClient.InvokeWithRetryAsync(
                async
                    client => {
                        HttpRequestMessage request = createRequest();

                        Uri newUri = new Uri(client.BaseAddress, request.RequestUri.OriginalString.TrimStart('/'));

                        request.RequestUri = newUri;

                        HttpResponseMessage response = await instance.SendAsync(request, cancellationToken);

                        response.EnsureSuccessStatusCode();

                        return response;
                    });
        }
    }
}
