using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using CSharpFunctionalExtensions;
using Flurl.Http;
using Flurl.Http.Testing;
using Reductech.EDR.Core.Connectors;
using Reductech.EDR.Core.Internal.Errors;

namespace Reductech.EDR.Connectors.Relativity
{
    public sealed class ConnectorInjection : IConnectorInjection
    {
        public const string FlurlClientFactoryKey = "FlurlClientFactory";

        /// <inheritdoc />
        public Result<IReadOnlyCollection<(string Name, object Context)>, IErrorBuilder> TryGetInjectedContexts()
        {
            IFlurlClient flurlClient = new FlurlClient(new HttpClient());

            var list = new List<(string Name, object Context)>()
            {
                (FlurlClientFactoryKey, flurlClient)
            };

            return list;
        }
    }

    public interface IFlurlClientFactory : IDisposable
    {
        IFlurlClient FlurlClient { get; }
    }

    public class DefaultFlurlClientFactory : IFlurlClientFactory
    {
        /// <inheritdoc />
        public IFlurlClient FlurlClient { get; } = new FlurlClient(new HttpClient());

        /// <inheritdoc />
        public void Dispose()
        {
            FlurlClient.Dispose();
        }
    }

    public class TestFlurlClientFactory : IFlurlClientFactory
    {
        public TestFlurlClientFactory()
        {
            HttpTest = new HttpTest();
            FlurlClient = GetFlurlClient(HttpTest);
        }

        public readonly HttpTest HttpTest;

        /// <inheritdoc />
        public void Dispose()
        {
            HttpTest.Dispose();
            FlurlClient.Dispose();
        }


        private static IFlurlClient GetFlurlClient(HttpTest httpTest)
        {
            var type = httpTest.GetType();


            var property = type.GetProperty(nameof(HttpClient), BindingFlags.Instance | BindingFlags.NonPublic);
            var httpClient = (HttpClient)property.GetValue(httpTest);

            return new FlurlClient(httpClient);
        }

        /// <inheritdoc />
        public IFlurlClient FlurlClient { get; private set; }
    }
}