using Reductech.Sequence.Connectors.Relativity.ManagerInterfaces;
using Relativity.Services.Folder;

namespace Reductech.Sequence.Connectors.Relativity.Steps;

/// <summary>
/// Update a Relativity folder
/// </summary>
[SCLExample(
    "RelativityUpdateFolder Workspace: 11 FolderId: 22 FolderName: \"NewName\"",
    ExecuteInTests = false
)]
public sealed class
    RelativityUpdateFolder : RelativityApiRequest<(Folder folder, int workspaceId), IFolderManager1,
        Unit, Unit>
{
    /// <inheritdoc />
    public override IStepFactory StepFactory { get; } =
        new SimpleStepFactory<RelativityUpdateFolder, Unit>();

    /// <inheritdoc />
    public override Result<Unit, IErrorBuilder> ConvertOutput(Unit serviceOutput)
    {
        return serviceOutput;
    }

    /// <inheritdoc />
    public override async Task<Unit> SendRequest(
        IStateMonad stateMonad,
        IFolderManager1 service,
        (Folder folder, int workspaceId) requestObject,
        CancellationToken cancellationToken)
    {
        await service.UpdateSingleAsync(requestObject.workspaceId, requestObject.folder);
        return Unit.Default;
    }

    /// <inheritdoc />
    public override async Task<Result<(Folder folder, int workspaceId), IError>> TryCreateRequest(
        IStateMonad stateMonad,
        CancellationToken cancellation)
    {
        var r = await stateMonad.RunStepsAsync(
            Workspace.WrapArtifact(Relativity.ArtifactType.Case, stateMonad, this),
            FolderId,
            FolderName.WrapStringStream(),
            cancellation
        );

        if (r.IsFailure)
            return r.ConvertFailure<(Folder folder, int workspaceId)>();

        var (workspaceId, folderId, folderName) = r.Value;

        var folder = new Folder { Name = folderName, ArtifactID = folderId };

        return (folder, workspaceId);
    }

    /// <summary>
    /// The Workspace containing the folder.
    /// You can provide either the Artifact Id or the name
    /// </summary>
    [StepProperty(1)]
    [Required]
    public IStep<SCLOneOf<SCLInt, StringStream>> Workspace { get; set; } = null!;

    /// <summary>
    /// The Id of the folder you want to update
    /// </summary>
    [StepProperty(2)]
    [Required]
    public IStep<SCLInt> FolderId { get; set; } = null!;

    /// <summary>
    /// The new name of the folder.
    /// </summary>
    [StepProperty(3)]
    [Alias("Name")]
    [Required]
    public IStep<StringStream> FolderName { get; set; } = null!;
}
