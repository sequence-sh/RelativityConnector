using Sequence.Connectors.Relativity.ManagerInterfaces;
using Relativity.Services.Folder;

namespace Sequence.Connectors.Relativity.Steps;

/// <summary>
///  Move a folder and its children, including subfolders and documents. 
/// </summary>
[SCLExample(
    "RelativityMoveFolder Workspace: 11 FolderArtifactId: 33 DestinationFolderArtifactId: 22",
    expectedOutput: "(TotalOperations: 1 ProcessState: \"Complete\" OperationsCompleted: 1)",
    ExecuteInTests = false
)]
public sealed class RelativityMoveFolder : RelativityApiRequest<(SCLInt workspaceId, SCLInt folderId, SCLInt
    destinationFolderId), IFolderManager1,
    FolderMoveResultSet, Entity>
{
    /// <inheritdoc />
    public override IStepFactory StepFactory =>
        new SimpleStepFactory<RelativityMoveFolder, Entity>();

    /// <inheritdoc />
    public override Result<Entity, IErrorBuilder> ConvertOutput(FolderMoveResultSet serviceOutput)
    {
        return serviceOutput.ConvertToEntity();
    }

    /// <inheritdoc />
    public override async Task<FolderMoveResultSet> SendRequest(
        IStateMonad stateMonad,
        IFolderManager1 service,
        (SCLInt workspaceId, SCLInt folderId, SCLInt destinationFolderId) requestObject,
        CancellationToken cancellationToken)
    {
        var (workspaceId, folderId, destinationFolderId) = requestObject;

        var r = await service.MoveFolderAsync(
            workspaceId,
            folderId,
            destinationFolderId,
            cancellationToken
        );

        return r;
    }

    /// <inheritdoc />
    public override ValueTask<Result<(SCLInt workspaceId, SCLInt folderId, SCLInt destinationFolderId), IError>>
        TryCreateRequest(IStateMonad stateMonad, CancellationToken cancellation)
    {
        return stateMonad.RunStepsAsync(
            Workspace.WrapArtifact(ArtifactType.Case, stateMonad, this),
            FolderArtifactId,
            DestinationFolderArtifactId,
            cancellation
        );
    }

    /// <summary>
    /// The Workspace containing the folder to move.
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

    /// <summary>
    /// The Id of the destination folder.
    /// </summary>
    [StepProperty(3)]
    [Required]
    public IStep<SCLInt> DestinationFolderArtifactId { get; set; } = null!;
}
