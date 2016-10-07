using Microsoft.ServiceFabric.Services.Communication.Client;
using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookList {
    public class HttpCommunicationClient : ICommunicationClient {
        public HttpCommunicationClient(Uri baseAddress, string listenerName, TimeSpan operationTimeout, TimeSpan readWriteTimeout) {
            this.BaseAddress = baseAddress;
            this.ListenerName = listenerName;
            this.OperationTimeout = operationTimeout;
            this.ReadWriteTimeout = readWriteTimeout;
        }


        ///// <summary>
        ///// The service base address.
        ///// </summary>
        public Uri BaseAddress { get; private set; }

        public TimeSpan OperationTimeout { get; set; }

        public TimeSpan ReadWriteTimeout { get; set; }

        public ResolvedServiceEndpoint Endpoint { get; set; }

        public string ListenerName { get; set; }

        public ResolvedServicePartition ResolvedServicePartition { get; set; }
    }
}
