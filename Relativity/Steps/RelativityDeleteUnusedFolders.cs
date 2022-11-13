using Sequence.Connectors.Relativity.Errors;
using Sequence.Connectors.Relativity.ManagerInterfaces;
using Relativity.Services.Folder;

namespace Sequence.Connectors.Relativity.Steps;

/// <summary>
/// Deletes unused folders in a relativity workspace
/// </summary>
[SCLExample("RelativityDeleteUnusedFolders Workspace: 42", ExecuteInTests = false)]
public sealed class
    RelativityDeleteUnusedFolders : RelativityApiRequest<SCLInt, IFolderManager1, FolderResultSet, Unit>
{
    /// <inheritdoc />
    public override IStepFactory StepFactory { get; } =
        new SimpleStepFactory<RelativityDeleteUnusedFolders, Unit>();

    /// <inheritdoc />
    public override Result<Unit, IErrorBuilder> ConvertOutput(FolderResultSet serviceOutput)
    {
        if (!serviceOutput.Success)
            return ErrorCode_Relativity.Unsuccessful.ToErrorBuilder(serviceOutput.Message);

        return Unit.Default;
    }

    /// <inheritdoc />
    public override async Task<FolderResultSet> SendRequest(
        IStateMonad stateMonad,
        IFolderManager1 service,
        SCLInt requestObject,
        CancellationToken cancellationToken)
    {
        var result = await service.DeleteUnusedFoldersAsync(requestObject);

        return result;
    }

    /// <inheritdoc />
    public override async ValueTask<Result<SCLInt, IError>> TryCreateRequest(
        IStateMonad stateMonad,
        CancellationToken cancellation)
    {
        return await Workspace.WrapArtifact(ArtifactType.Case,stateMonad, this).Run(stateMonad, cancellation);
    }

    /// <summary>
    /// The Workspace to delete unused folders from
    /// You can provide either the Artifact Id or the name
    /// </summary>
    [StepProperty(1)]
    [Required]
    public IStep<SCLOneOf<SCLInt, StringStream>> Workspace { get; set; } = null!;
}
