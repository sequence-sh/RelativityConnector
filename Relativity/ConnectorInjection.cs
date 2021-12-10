using Flurl.Http;
using Reductech.EDR.Connectors.Relativity.Managers;
using Reductech.EDR.Core.Connectors;

namespace Reductech.EDR.Connectors.Relativity;

public sealed class ConnectorInjection : IConnectorInjection
{

    public const string ServiceFactoryFactoryKey = "RelativityServiceFactoryFactory";

    /// <inheritdoc />
    public Result<IReadOnlyCollection<(string Name, object Context)>, IErrorBuilder>
        TryGetInjectedContexts()
    {
        IFlurlClient flurlClient = new FlurlClient();

        var list = new List<(string Name, object Context)>()
        {
            (ServiceFactoryFactoryKey, new TemplateServiceFactoryFactory(flurlClient))
        };

        return list;
    }
}
