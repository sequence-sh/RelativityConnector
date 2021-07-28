using System;
using Relativity.Services.ServiceProxy;

namespace Reductech.EDR.Connectors.Relativity
{

/// <summary>
/// Creates Service Factories which connect to a relativity server
/// </summary>
public class DefaultServiceFactoryFactory : IServiceFactoryFactory
{
    private DefaultServiceFactoryFactory() { }

    public static IServiceFactoryFactory Instance { get; } = new DefaultServiceFactoryFactory();

    /// <inheritdoc />
    public IServiceFactory CreateServiceFactory(RelativitySettings relativitySettings)
    {
        {
            var endPoint = new Uri($"{relativitySettings.Url}/Relativity.REST/api");

            var serviceFactory = new ServiceFactory(
                new ServiceFactorySettings(
                    endPoint,
                    new UsernamePasswordCredentials(
                        relativitySettings.RelativityUsername,
                        relativitySettings.RelativityPassword
                    )
                )
            );

            return serviceFactory;
        }
    }
}

}
