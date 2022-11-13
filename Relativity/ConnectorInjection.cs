using Flurl.Http;
using Sequence.Connectors.Relativity.Managers;
using Sequence.Core.Connectors;
#pragma warning disable CS1591
namespace Sequence.Connectors.Relativity;

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
