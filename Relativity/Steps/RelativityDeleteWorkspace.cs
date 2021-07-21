using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Flurl;
using Flurl.Http;
using Reductech.EDR.Connectors.Relativity.Errors;
using Reductech.EDR.Core;
using Reductech.EDR.Core.Attributes;
using Reductech.EDR.Core.Internal;
using Reductech.EDR.Core.Internal.Errors;
using Reductech.EDR.Core.Util;

namespace Reductech.EDR.Connectors.Relativity.Steps
{
    /// <summary>
    /// Deletes a relativity workspace
    /// </summary>
    [SCLExample("RelativityDeleteWorkspace 42",ExecuteInTests = false, Description = "Deletes workspace 42")]
    public sealed class RelativityDeleteWorkspace : CompoundStep<Unit>
    {
        protected override async Task<Result<Unit, IError>> Run(IStateMonad stateMonad,
            CancellationToken cancellationToken)
        {
            var workspaceId = await WorkspaceId.Run(stateMonad, cancellationToken);
            if (workspaceId.IsFailure) return workspaceId.ConvertFailure<Unit>();

            var settingsResult = stateMonad.Settings.TryGetRelativitySettings();
            if (settingsResult.IsFailure)
                return settingsResult.MapError(x => x.WithLocation(this)).ConvertFailure<Unit>();

            var flurlClientResult = stateMonad.GetFlurlClientFactory().Map(x => x.FlurlClient);
            if (flurlClientResult.IsFailure)
                return flurlClientResult.MapError(x => x.WithLocation(this)).ConvertFailure<Unit>();


            var url = Url.Combine(settingsResult.Value.Url,
                "Relativity.Rest",
                "API",
                "relativity-environment",
                $"v{settingsResult.Value.APIVersionNumber}",
                "workspace",
                workspaceId.Value.ToString()
            );

            IFlurlResponse response;

            try
            {
                response =
                    await url
                        .WithBasicAuth(settingsResult.Value.RelativityUsername, settingsResult.Value.RelativityPassword)
                        .WithHeader("X-CSRF-Header", "-")
                        .WithClient(flurlClientResult.Value)
                        .DeleteAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                var error = ErrorCode.Unknown.ToErrorBuilder(ex).WithLocation(this);
                return Result.Failure<Unit, IError>(error);
            }

            if (!response.ResponseMessage.IsSuccessStatusCode)
                return Result.Failure<Unit, IError>(
                    ErrorCode_Relativity.RequestFailed
                        .ToErrorBuilder(response.StatusCode, response.ResponseMessage.ReasonPhrase)
                        .WithLocation(this)
                );

            return Unit.Default;
        }


        /// <summary>
        /// The id of the workspace to delete
        /// </summary>
        [StepProperty(1)]
        [Required]
        public IStep<int> WorkspaceId { get; set; } = null!;

        public override IStepFactory StepFactory { get; } = new SimpleStepFactory<RelativityDeleteWorkspace, Unit>();
    }
}