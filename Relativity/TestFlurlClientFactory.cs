using System.Net.Http;
using System.Reflection;
using Flurl.Http;
using Flurl.Http.Testing;

namespace Reductech.EDR.Connectors.Relativity
{
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