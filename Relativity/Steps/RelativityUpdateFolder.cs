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
    /// Update a Relativity folder
    /// </summary>
    [SCLExample("RelativityUpdateFolder WorkspaceArtifactId: 11 FolderId: 22 FolderName: \"NewName\"",
        ExecuteInTests = false)]
    public sealed class
        RelativityUpdateFolder : RelativityApiRequest<(Folder folder, int workspaceId), IFolderManager, Unit, Unit>
    {
        /// <inheritdoc />
        public override IStepFactory StepFactory { get; } = new SimpleStepFactory<RelativityUpdateFolder, Unit>();

        /// <inheritdoc />
        public override Result<Unit, IErrorBuilder> ConvertOutput(Unit serviceOutput)
        {
            return serviceOutput;
        }

        /// <inheritdoc />
        public override async Task<Unit> SendRequest(IStateMonad stateMonad, IFolderManager service,
            (Folder folder, int workspaceId) requestObject,
            CancellationToken cancellationToken)
        {
            await service.UpdateSingleAsync(requestObject.workspaceId, requestObject.folder);
            return Unit.Default;
        }

        /// <inheritdoc />
        public override async Task<Result<(Folder folder, int workspaceId), IError>> TryCreateRequest(
            IStateMonad stateMonad, CancellationToken cancellation)
        {
            var r = await stateMonad.RunStepsAsync(WorkspaceArtifactId, FolderId, FolderName.WrapStringStream(),
                cancellation);
            if (r.IsFailure) return r.ConvertFailure<(Folder folder, int workspaceId)>();

            var (workspaceId, folderId, folderName) = r.Value;

            var folder = new Folder
            {
                Name = folderName,
                ArtifactID = folderId
            };

            return (folder, workspaceId);
        }

        /// <summary>
        /// The workspace ID of the folder.
        /// </summary>
        [StepProperty(1)]
        [Required]
        public IStep<int> WorkspaceArtifactId { get; set; } = null!;

        /// <summary>
        /// The Id of the folder you want to update
        /// </summary>
        [StepProperty(2)]
        [Required]
        public IStep<int> FolderId { get; set; } = null!;

        /// <summary>
        /// The new name of the folder.
        /// </summary>
        [StepProperty(3)]
        [Alias("Name")]
        [Required]
        public IStep<StringStream> FolderName { get; set; } = null!;
    }
}