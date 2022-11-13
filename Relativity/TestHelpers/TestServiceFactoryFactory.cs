using System.Linq;
using Relativity.Services.ServiceProxy;
#pragma warning disable CS1591
namespace Sequence.Connectors.Relativity.TestHelpers;

public class TestServiceFactoryFactory : IServiceFactoryFactory
{
    public TestServiceFactoryFactory(params IDisposable[] services)
    {
        MyTestServiceFactory = new TestServiceFactory(services);
    }

    public TestServiceFactory MyTestServiceFactory { get; }

    /// <inheritdoc />
    public IServiceFactory CreateServiceFactory(RelativitySettings relativitySettings)
    {
        return MyTestServiceFactory;
    }

    public class TestServiceFactory : IServiceFactory
    {
        public TestServiceFactory(IReadOnlyList<IDisposable> services)
        {
            Services = services;
        }

        public IReadOnlyList<IDisposable> Services { get; }

        /// <inheritdoc />
        public T CreateProxy<T>() where T : IDisposable
        {
            var result = Services.OfType<T>().FirstOrDefault();

            if (result is null)
                throw new Exception($"Could not create a service of type {typeof(T).Name}");

            return result;
        }
    }
}
