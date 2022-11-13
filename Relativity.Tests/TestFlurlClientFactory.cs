using System.Net.Http;
using System.Reflection;
using Flurl.Http;
using Flurl.Http.Testing;

namespace Sequence.Connectors.Relativity.Tests;

public class TestFlurlClientFactory
{
    public static IFlurlClient GetFlurlClient(HttpTest httpTest)
    {
        var type = httpTest.GetType();

        var property = type.GetProperty(
            nameof(HttpClient),
            BindingFlags.Instance | BindingFlags.NonPublic
        );

        var httpClient = (HttpClient)property!.GetValue(httpTest)!;

        return new FlurlClient(httpClient);
    }
}
