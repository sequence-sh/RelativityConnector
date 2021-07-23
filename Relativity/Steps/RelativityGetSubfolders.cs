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
    /// Gets all children of a folder
    /// </summary>
    [SCLExample("RelativityGetSubfolders WorkspaceArtifactId: 11 FolderArtifactId: 22",
        "[(ParentFolder: (ArtifactID: 22 Name: \"MyFolder\") AccessControlListIsInherited: False SystemCreatedBy: \"\" SystemCreatedOn: 0001-01-01T00:00:00.0000000 SystemLastModifiedBy: \"\" SystemLastModifiedOn: 0001-01-01T00:00:00.0000000 Permissions: (add: False delete: False edit: False secure: False) Children: \"\" Selected: False HasChildren: False ArtifactID: 101 Name: \"SubFolder 1\"),(ParentFolder: (ArtifactID: 22 Name: \"MyFolder\") AccessControlListIsInherited: False SystemCreatedBy: \"\" SystemCreatedOn: 0001-01-01T00:00:00.0000000 SystemLastModifiedBy: \"\" SystemLastModifiedOn: 0001-01-01T00:00:00.0000000 Permissions: (add: False delete: False edit: False secure: False) Children: \"\" Selected: False HasChildren: False ArtifactID: 102 Name: \"SubFolder 2\") ]",
        ExecuteInTests = false
    )]
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