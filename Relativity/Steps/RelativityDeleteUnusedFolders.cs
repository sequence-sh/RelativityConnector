using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Reductech.EDR.Core;
using Reductech.EDR.Core.Attributes;
using Reductech.EDR.Core.Internal;
using Reductech.EDR.Core.Internal.Errors;
using Reductech.EDR.Core.Util;
using Relativity.Services.Folder;

namespace Reductech.EDR.Connectors.Relativity.Steps
{
    /// <summary>
    /// Deletes unused folders in a relativity workspace
    /// </summary>
    public sealed class RelativityDeleteUnusedFolders : RelativityApiRequest<int, IFolderManager, Unit, Unit>
    {
        /// <inheritdoc />
        public override IStepFactory StepFactory { get; } = new SimpleStepFactory<RelativityDeleteUnusedFolders, Unit>();

        /// <inheritdoc />
        public override Result<Unit, IErrorBuilder> ConvertOutput(Unit serviceOutput)
        {
            return serviceOutput;
        }

        /// <inheritdoc />
        public override async Task<Unit> SendRequest(IFolderManager service, int requestObject, CancellationToken cancellationToken)
        {
            await service.DeleteUnusedFoldersAsync(requestObject);
            return Unit.Default;
        }

        /// <inheritdoc />
        public override async Task<Result<int, IError>> TryCreateRequest(IStateMonad stateMonad, CancellationToken cancellation)
        {
            var i = await WorkspaceArtifactId.Run(stateMonad, cancellation);
            if (i.IsFailure) return i.ConvertFailure<int>();

            return i.Value;
        }

        /// <summary>
        /// The Id of the workspace to delete unused folders from.
        /// </summary>
        [StepProperty(1)]
        [Required]
        public IStep<int> WorkspaceArtifactId { get; set; } = null!;
    }
}