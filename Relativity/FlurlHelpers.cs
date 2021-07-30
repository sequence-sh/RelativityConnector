using System.Linq;
using CSharpFunctionalExtensions;
using Flurl;
using Flurl.Http;
using Reductech.EDR.Core;
using Reductech.EDR.Core.Internal.Errors;

namespace Reductech.EDR.Connectors.Relativity
{
    public static class FlurlHelpers
    {
        public static Result<IFlurlClientFactory, IErrorBuilder> GetFlurlClientFactory(this IStateMonad stateMonad)
        {
            return stateMonad.ExternalContext.TryGetContext<IFlurlClientFactory>(ConnectorInjection
                .FlurlClientFactoryKey);
        }

        public static IFlurlRequest SetupRelativityRequest(this IFlurlClient client,
            RelativitySettings relativitySettings, params string[] urlParts)
        {
            var url = Url.Combine(
                new[]
                    {
                        relativitySettings.Url,
                        "Relativity.REST",
                        "api"
                    }
                    .Concat(urlParts).ToArray());

            var request = url
                .WithBasicAuth(relativitySettings.RelativityUsername, relativitySettings.RelativityPassword)
                //.ConfigureRequest(x => x.JsonSerializer = Serializer)
                .WithHeader("X-CSRF-Header", "-")
                .WithClient(client);

            return request;
        }

        //private static readonly ISerializer Serializer = new NewtonsoftJsonSerializer(
        //    new JsonSerializerSettings
        //    {
        //    }
        //);
    }
}