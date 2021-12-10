using OneOf;
using Reductech.EDR.Connectors.Relativity.ManagerInterfaces;
using Relativity.Services.Folder;

namespace Reductech.EDR.Connectors.Relativity.Steps;

/// <summary>
/// Create a folder in a relativity workspace
/// </summary>
public sealed class
    RelativityCreateFolder : RelativityApiRequest<(Folder folder, int workspaceId), IFolderManager1,
        int, int>
{
    /// <inheritdoc />
    public override IStepFactory StepFactory =>
        new SimpleStepFactory<RelativityCreateFolder, int>();

    /// <inheritdoc />
    public override Result<int, IErrorBuilder> ConvertOutput(int serviceOutput)
    {
        return serviceOutput;
    }

    /// <inheritdoc />
    public override async Task<int> SendRequest(
        IStateMonad stateMonad,
        IFolderManager1 service,
        (Folder folder, int workspaceId) requestObject,
        CancellationToken cancellationToken)
    {
        var newFolderArtifactId = await service.CreateSingleAsync(
            requestObject.workspaceId,
            requestObject.folder
        );

        return newFolderArtifactId;
    }

    /// <inheritdoc />
    public override async Task<Result<(Folder folder, int workspaceId), IError>> TryCreateRequest(
        IStateMonad stateMonad,
        CancellationToken cancellation)
    {
        var results = await stateMonad.RunStepsAsync(
            Workspace.WrapArtifact(Relativity.ArtifactType.Case, stateMonad, this),
            FolderName.WrapStringStream(),
            ParentFolderId.WrapNullable(),
            cancellation
        );

        if (results.IsFailure)
            return results.ConvertFailure<(Folder folder, int workspaceId)>();

        var (workspace, folderName, parentFolderId) = results.Value;

        var folder = new Folder { Name = folderName };

        if (parentFolderId.HasValue)
        {
            folder.ParentFolder = new FolderRef(parentFolderId.Value);
        }

        return (folder, workspace);
    }

    /// <summary>
    /// The Workspace where you want to create the folder.
    /// You can provide either the Artifact Id or the name
    /// </summary>
    [StepProperty(1)]
    [Required]
    public IStep<OneOf<int, StringStream>> Workspace { get; set; } = null!;

    /// <summary>
    /// The name of the Folder DTO that you want to create.
    /// </summary>
    [StepProperty(2)]
    [Alias("Name")]
    [Required]
    public IStep<StringStream> FolderName { get; set; } = null!;

    /// <summary>
    /// The Id of the parent where you want to add the new subfolder.
    /// </summary>
    [StepProperty(3)]
    [DefaultValueExplanation("Create the folder at the root of the workspace.")]
    public IStep<int>? ParentFolderId { get; set; } = null!;
}
