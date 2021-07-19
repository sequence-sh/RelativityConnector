using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Flurl;
using Flurl.Http;
using Reductech.EDR.Core.Internal.Errors;

namespace Reductech.EDR.Connectors.Relativity
{
    public static class DocumentFileManager
    {
        public static async Task<Result<string, IError>> DownloadFile(RelativitySettings settings,
            IFlurlClient client,
            ErrorLocation errorLocation,

            int workspaceId, int documentArtifactId, CancellationToken cancellationToken)
        {
            var urlSuffix =
                $"/Relativity.REST/api/Relativity.Document/workspace/{workspaceId}/downloadnativefile/{documentArtifactId}";
            
            

            var url = Url.Combine(settings.Url, urlSuffix);


            string resultString;

            try
            {
                resultString = await url.WithHeader("X-CSRF-Header", "-")
                    .WithBasicAuth(settings.RelativityUsername, settings.RelativityPassword)
                    .WithClient(client)
                    .GetStringAsync(cancellationToken);
            }
            catch (FlurlHttpException e)
            {
                return Result.Failure<string, IError>(ErrorCode.Unknown.ToErrorBuilder(e).WithLocation(errorLocation));
            }

            return resultString;
        }
    }
}