using Relativity.Services.ServiceProxy;

namespace Reductech.EDR.Connectors.Relativity
{

/// <summary>
/// Produces IServiceFactories
/// </summary>
public interface IServiceFactoryFactory
{
    IServiceFactory CreateServiceFactory(RelativitySettings relativitySettings);
}

}
