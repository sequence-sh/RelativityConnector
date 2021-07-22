using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Flurl.Http;
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
    [SCLExample("RelativityDeleteWorkspace 42", ExecuteInTests = false, Description = "Deletes workspace 42")]
    public sealed class RelativityDeleteWorkspace : RelativityApiRequest<int, Unit>
    {
        /// <summary>
        /// The id of the workspace to delete
        /// </summary>
        [StepProperty(1)]
        [Required]
        public IStep<int> WorkspaceId { get; set; } = null!;

        public override IStepFactory StepFactory { get; } = new SimpleStepFactory<RelativityDeleteWorkspace, Unit>();

        /// <inheritdoc />
        public override Task<IFlurlResponse> SendRequest(IFlurlRequest flurlRequest, int requestObject, CancellationToken cancellationToken)
        {
            return flurlRequest.DeleteAsync(cancellationToken);
        }

        /// <inheritdoc />
        public override string[] CreateURL(RelativitySettings settings, int request)
        {
            return new[]
            {
                "Relativity.Rest",
                "API",
                "relativity-environment",
                $"v{settings.APIVersionNumber}",
                "workspace",
                request.ToString()
            };

        }

        /// <inheritdoc />
        public override Unit TryCreateOutput(string stringResult)
        {
            return Unit.Default;
        }

        /// <inheritdoc />
        public override async Task<Result<int, IError>> TryCreateRequest(IStateMonad stateMonad, CancellationToken cancellation)
        {
            var workspaceId = await WorkspaceId.Run(stateMonad, cancellation);
            return workspaceId;
        }
    }
}