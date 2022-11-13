using System.Linq;
using Flurl;
using Flurl.Http;
#pragma warning disable CS1591
namespace Sequence.Connectors.Relativity;

public static class FlurlHelpers
{
    public static IFlurlRequest SetupRelativityRequest(
        this IFlurlClient client,
        RelativitySettings relativitySettings,
        params string[] urlParts)
    {
        var url = Url.Combine(
            new[] { relativitySettings.Url, "Relativity.REST", "api" }
                .Concat(urlParts)
                .ToArray()
        );

        var request = url
            .WithBasicAuth(
                relativitySettings.RelativityUsername,
                relativitySettings.RelativityPassword
            )
            //.ConfigureRequest(x => x.JsonSerializer = Serializer)
            .WithHeader("X-CSRF-Header", "-")
            .WithClient(client);

        return request;
    }
}
