using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Reductech.EDR.Connectors.Relativity.ManagerInterfaces;
using Reductech.EDR.Core;
using Reductech.EDR.Core.Attributes;
using Reductech.EDR.Core.Internal;
using Reductech.EDR.Core.Internal.Errors;
using Reductech.EDR.Core.Util;
using Relativity.Services.Folder;
using Entity = Reductech.EDR.Core.Entity;

namespace Reductech.EDR.Connectors.Relativity.Steps
{

/// <summary>
/// Gets all children of a folder
/// </summary>
[SCLExample(
    "RelativityGetSubfolders WorkspaceArtifactId: 11 FolderArtifactId: 22",
    "[(ParentFolder: (ArtifactID: 22 Name: \"MyFolder\") AccessControlListIsInherited: False SystemCreatedBy: \"\" SystemCreatedOn: 0001-01-01T00:00:00.0000000 SystemLastModifiedBy: \"\" SystemLastModifiedOn: 0001-01-01T00:00:00.0000000 Permissions: (add: False delete: False edit: False secure: False) Children: \"\" Selected: False HasChildren: False ArtifactID: 101 Name: \"SubFolder 1\"),(ParentFolder: (ArtifactID: 22 Name: \"MyFolder\") AccessControlListIsInherited: False SystemCreatedBy: \"\" SystemCreatedOn: 0001-01-01T00:00:00.0000000 SystemLastModifiedBy: \"\" SystemLastModifiedOn: 0001-01-01T00:00:00.0000000 Permissions: (add: False delete: False edit: False secure: False) Children: \"\" Selected: False HasChildren: False ArtifactID: 102 Name: \"SubFolder 2\") ]",
    ExecuteInTests = false
)]
public sealed class
    RelativityGetSubfolders : RelativityApiRequest<(int workspaceId, int folderId), IFolderManager1,
        List<Folder>,
        Array<Entity>>
{
    /// <inheritdoc />
    public override IStepFactory StepFactory =>
        new SimpleStepFactory<RelativityGetSubfolders, Array<Entity>>();

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
    public override async Task<List<Folder>> SendRequest(
        IStateMonad stateMonad,
        IFolderManager1 service,
        (int workspaceId, int folderId) requestObject,
        CancellationToken cancellationToken)
    {
        var children = await service.GetChildrenAsync(
            requestObject.workspaceId,
            requestObject.folderId
        );

        return children;
    }

    /// <inheritdoc />
    public override Task<Result<(int workspaceId, int folderId), IError>> TryCreateRequest(
        IStateMonad stateMonad,
        CancellationToken cancellation)
    {
        return stateMonad.RunStepsAsync(WorkspaceArtifactId, FolderArtifactId, cancellation);
    }
}

}
