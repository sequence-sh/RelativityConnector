using System.Linq;
using Reductech.EDR.Connectors.Relativity.ManagerInterfaces;
using Relativity.Services.Folder;

namespace Reductech.EDR.Connectors.Relativity.Steps;

/// <summary>
/// Gets all children of a folder
/// </summary>
[SCLExample(
    "RelativityGetSubfolders Workspace: 11 FolderArtifactId: 22",
    "[(Name: MySubFolder, ArtifactID: 12345, HasChildren: true, Selected: false)]",
    ExecuteInTests = false
)]
public sealed class
    RelativityGetSubfolders : RelativityApiRequest<(SCLInt workspaceId, SCLInt folderId), IFolderManager1,
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
        (SCLInt workspaceId, SCLInt folderId) requestObject,
        CancellationToken cancellationToken)
    {
        var children = await service.GetChildrenAsync(
            requestObject.workspaceId,
            requestObject.folderId
        );

        return children;
    }

    /// <inheritdoc />
    public override Task<Result<(SCLInt workspaceId, SCLInt folderId), IError>> TryCreateRequest(
        IStateMonad stateMonad,
        CancellationToken cancellation)
    {
        return stateMonad.RunStepsAsync(
            Workspace.WrapArtifact(ArtifactType.Case, stateMonad, this),
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
    public IStep<SCLOneOf<SCLInt, StringStream>> Workspace { get; set; } = null!;

    /// <summary>
    /// The Id of the folder.
    /// </summary>
    [StepProperty(2)]
    [Required]
    public IStep<SCLInt> FolderArtifactId { get; set; } = null!;
}
