using System.Net.Http;
using Flurl.Http;

namespace Reductech.EDR.Connectors.Relativity
{
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
}