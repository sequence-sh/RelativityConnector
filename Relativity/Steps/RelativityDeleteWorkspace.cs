using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Reductech.EDR.Core;
using Reductech.EDR.Core.Attributes;
using Reductech.EDR.Core.Internal;
using Reductech.EDR.Core.Internal.Errors;
using Reductech.EDR.Core.Util;
using Relativity.Environment.V1.Workspace;

namespace Reductech.EDR.Connectors.Relativity.Steps
{
    /// <summary>
    /// Deletes a relativity workspace
    /// </summary>
    [SCLExample("RelativityDeleteWorkspace 42", ExecuteInTests = false, Description = "Deletes workspace 42")]
    public sealed class RelativityDeleteWorkspace : RelativityApiRequest<int, IWorkspaceManager, Unit, Unit>
    {
        /// <summary>
        /// The id of the workspace to delete
        /// </summary>
        [StepProperty(1)]
        [Required]
        public IStep<int> WorkspaceId { get; set; } = null!;

        public override IStepFactory StepFactory { get; } = new SimpleStepFactory<RelativityDeleteWorkspace, Unit>();


        /// <inheritdoc />
        public override Result<Unit, IErrorBuilder> ConvertOutput(Unit serviceOutput)
        {
            return serviceOutput;
        }

        /// <inheritdoc />
        public override async Task<Unit> SendRequest(IStateMonad stateMonad, IWorkspaceManager service,
            int requestObject,
            CancellationToken cancellationToken)
        {
            await service.DeleteAsync(requestObject, cancellationToken);

            return Unit.Default;
        }

        /// <inheritdoc />
        public override async Task<Result<int, IError>> TryCreateRequest(IStateMonad stateMonad,
            CancellationToken cancellation)
        {
            var workspaceId = await WorkspaceId.Run(stateMonad, cancellation);
            return workspaceId;
        }
    }
}