using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using OneOf;
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
    "RelativityGetSubfolders Workspace: 11 FolderArtifactId: 22",
    "[(Name: MySubFolder, ArtifactID: 12345, HasChildren: true, Selected: false)]",
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

    /// <inheritdoc />
    public override Result<Array<Entity>, IErrorBuilder> ConvertOutput(List<Folder> serviceOutput)
    {
        return serviceOutput.Select(RelativityEntityConversionHelpers.ConvertToEntity).ToSCLArray();
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
        return stateMonad.RunStepsAsync(
            Workspace.WrapArtifact(Relativity.ArtifactType.Case, stateMonad, this),
            FolderArtifactId,
            cancellation
        );
    }

    /// <summary>
    /// The Workspace containing the folder
    /// You can provide either the Artifact Id or the name
    /// </summary>
    [StepProperty(1)]
    [Required]
    public IStep<OneOf<int, StringStream>> Workspace { get; set; } = null!;

    /// <summary>
    /// The Id of the folder.
    /// </summary>
    [StepProperty(2)]
    [Required]
    public IStep<int> FolderArtifactId { get; set; } = null!;
}

}
