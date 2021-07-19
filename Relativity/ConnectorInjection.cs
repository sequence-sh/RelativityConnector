using System.Collections.Generic;
using System.Net.Http;
using CSharpFunctionalExtensions;
using Flurl.Http;
using Reductech.EDR.Core.Connectors;
using Reductech.EDR.Core.Internal.Errors;

namespace Reductech.EDR.Connectors.Relativity
{
    public sealed class ConnectorInjection : IConnectorInjection
    {
        public const string FlurlClientKey = "FlurlClient";

        /// <inheritdoc />
        public Result<IReadOnlyCollection<(string Name, object Context)>, IErrorBuilder> TryGetInjectedContexts()
        {
            IFlurlClient flurlClient = new FlurlClient(new HttpClient());

            var list = new List<(string Name, object Context)>()
            {
                (FlurlClientKey, flurlClient)
            };

            return list;
        }
    }
}