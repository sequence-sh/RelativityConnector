using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using CSharpFunctionalExtensions;
using Flurl.Http;
using Flurl.Http.Configuration;
using Reductech.EDR.Connectors.Relativity.Managers;
using Reductech.EDR.Core.Connectors;
using Reductech.EDR.Core.Internal.Errors;

namespace Reductech.EDR.Connectors.Relativity
{
    public sealed class ConnectorInjection : IConnectorInjection
    {
        public const string FlurlClientFactoryKey = "FlurlClientFactory";

        public const string ServiceFactoryFactoryKey = "RelativityServiceFactoryFactory";

        private const bool UseFiddlerProxy = false;

        /// <inheritdoc />
        public Result<IReadOnlyCollection<(string Name, object Context)>, IErrorBuilder> TryGetInjectedContexts()
        {
            IFlurlClient flurlClient;

            if (UseFiddlerProxy)
            {
                var factory = new ProxyHttpClientFactory("http://127.0.0.1:8866");
                flurlClient = new FlurlClient(factory.CreateHttpClient(factory.CreateMessageHandler()));
            }
            else
            {
                flurlClient = new FlurlClient();
            }

            

            var list = new List<(string Name, object Context)>()
            {
                (FlurlClientFactoryKey, flurlClient),
                (ServiceFactoryFactoryKey, new TemplateServiceFactoryFactory(flurlClient))
            };

            return list;
        }

        /// <summary>
        /// Proxy class that lets you see requests in fiddler
        /// </summary>
         private class ProxyHttpClientFactory : DefaultHttpClientFactory {
            private string _address;

            public ProxyHttpClientFactory(string address) {
                _address = address;
            }

            public override HttpMessageHandler CreateMessageHandler() {
                return new HttpClientHandler {
                    Proxy = new WebProxy(_address),
                    UseProxy = true
                };
            }
        } 
    }
}