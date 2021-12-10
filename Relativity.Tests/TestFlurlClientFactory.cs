using System.Net.Http;
using System.Reflection;
using Flurl.Http;
using Flurl.Http.Testing;

namespace Reductech.EDR.Connectors.Relativity.TestHelpers;

public class TestFlurlClientFactory
{
    public static IFlurlClient GetFlurlClient(HttpTest httpTest)
    {
        var type = httpTest.GetType();

        var property = type.GetProperty(
            nameof(HttpClient),
            BindingFlags.Instance | BindingFlags.NonPublic
        );

        var httpClient = (HttpClient)property.GetValue(httpTest);

        return new FlurlClient(httpClient);
    }
}
