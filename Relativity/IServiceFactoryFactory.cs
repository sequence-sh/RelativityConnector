using Relativity.Services.ServiceProxy;

namespace Sequence.Connectors.Relativity;

/// <summary>
/// Produces IServiceFactories
/// </summary>
public interface IServiceFactoryFactory
{
    IServiceFactory CreateServiceFactory(RelativitySettings relativitySettings);
}
