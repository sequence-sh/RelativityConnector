using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Reductech.EDR.Core;
using Reductech.EDR.Core.Attributes;
using Reductech.EDR.Core.Internal;
using Reductech.EDR.Core.Internal.Errors;
using Relativity.Services.Folder;
using Entity = Reductech.EDR.Core.Entity;

namespace Reductech.EDR.Connectors.Relativity.Steps
{
    /// <summary>
    ///  Move a folder and its children, including subfolders and documents. 
    /// </summary>
    public sealed class RelativityMoveFolders : RelativityApiRequest<(int workspaceId, int folderId, int
        destinationFolderId), IFolderManager,
        FolderMoveResultSet, Entity>
    {
        /// <inheritdoc />
        public override IStepFactory StepFactory => new SimpleStepFactory<RelativityMoveFolders, Entity>();

        /// <inheritdoc />
        public override Result<Entity, IErrorBuilder> ConvertOutput(FolderMoveResultSet serviceOutput)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public override async Task<FolderMoveResultSet> SendRequest(IFolderManager service,
            (int workspaceId, int folderId, int destinationFolderId) requestObject,
            CancellationToken cancellationToken)
        {
            var (workspaceId, folderId, destinationFolderId) = requestObject;
            var r = await service.MoveFolderAsync(workspaceId, folderId, destinationFolderId, cancellationToken);

            return r;
        }

        /// <inheritdoc />
        public override async Task<Result<(int workspaceId, int folderId, int destinationFolderId), IError>>
            TryCreateRequest(IStateMonad stateMonad, CancellationToken cancellation)
        {
            throw new System.NotImplementedException();
        }
    }


    /// <summary>
    /// Gets all children of a folder
    /// </summary>
    public sealed class
        RelativityGetSubfolders : RelativityApiRequest<(int workspaceId, int folderId), IFolderManager, List<Folder>,
            Array<Entity>>
    {
        /// <inheritdoc />
        public override IStepFactory StepFactory => new SimpleStepFactory<RelativityGetSubfolders, Array<Entity>>();


        /// <summary>
        /// The Id of the workspace.
        /// </summary>
        [StepProperty(1)]
        [Required]
        public IStep<int> WorkspaceArtifactId { get; set; } = null!;

        /// <summary>
        /// The Id of the folder.
        /// </summary>
        [StepProperty(2)]
        [Required]
        public IStep<int> FolderArtifactId { get; set; } = null!;

        /// <inheritdoc />
        public override Result<Array<Entity>, IErrorBuilder> ConvertOutput(List<Folder> serviceOutput)
        {
            var r = TryConvertToEntityArray(serviceOutput);

            return r;
        }

        /// <inheritdoc />
        public override async Task<List<Folder>> SendRequest(IFolderManager service,
            (int workspaceId, int folderId) requestObject,
            CancellationToken cancellationToken)
        {
            var children = await service.GetChildrenAsync(requestObject.workspaceId, requestObject.folderId);
            return children;
        }

        /// <inheritdoc />
        public override async Task<Result<(int workspaceId, int folderId), IError>> TryCreateRequest(
            IStateMonad stateMonad, CancellationToken cancellation)
        {
            var wi = await WorkspaceArtifactId.Run(stateMonad, cancellation);
            if (wi.IsFailure) return wi.ConvertFailure<(int workspaceId, int artifactId)>();


            var ai = await FolderArtifactId.Run(stateMonad, cancellation);
            if (ai.IsFailure) return ai.ConvertFailure<(int workspaceId, int artifactId)>();

            return (wi.Value, ai.Value);
        }
    }
}