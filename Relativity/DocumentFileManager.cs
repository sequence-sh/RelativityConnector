using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Flurl;
using Flurl.Http;

namespace Reductech.Connectors.Relativity
{
    public static class DocumentFileManager
    {
        public static async Task<Result<string, FlurlHttpException>> DownloadFile(IRelativitySettings settings, int workspaceId, int documentArtifactId, CancellationToken cancellationToken)
        {
            var urlSuffix = $"/Relativity.REST/api/Relativity.Document/workspace/{workspaceId}/downloadnativefile/{documentArtifactId}";

            var url = Url.Combine(settings.Url, urlSuffix);


            string resultString;

            try
            {
                resultString = await url.WithHeader("X-CSRF-Header", "-")
                .WithBasicAuth(settings.RelativityUsername, settings.RelativityPassword)
                .GetStringAsync(cancellationToken);
            }
            catch (FlurlHttpException e)
            {
                return Result.Failure<string, FlurlHttpException>(e);
            }

            return resultString;
        }


    }
}
