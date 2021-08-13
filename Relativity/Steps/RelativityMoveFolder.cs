using System.ComponentModel.DataAnnotations;
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
///  Move a folder and its children, including subfolders and documents. 
/// </summary>
[SCLExample(
    "RelativityMoveFolder Workspace: 11 FolderArtifactId: 33 DestinationFolderArtifactId: 22",
    expectedOutput: "(TotalOperations: 1 ProcessState: \"Complete\" OperationsCompleted: 1)",
    ExecuteInTests = false
)]
public sealed class RelativityMoveFolder : RelativityApiRequest<(int workspaceId, int folderId, int
    destinationFolderId), IFolderManager1,
    FolderMoveResultSet, Entity>
{
    /// <inheritdoc />
    public override IStepFactory StepFactory =>
        new SimpleStepFactory<RelativityMoveFolder, Entity>();

    /// <inheritdoc />
    public override Result<Entity, IErrorBuilder> ConvertOutput(FolderMoveResultSet serviceOutput)
    {
        return APIRequestHelpers.TryConvertToEntity(serviceOutput);
    }

    /// <inheritdoc />
    public override async Task<FolderMoveResultSet> SendRequest(
        IStateMonad stateMonad,
        IFolderManager1 service,
        (int workspaceId, int folderId, int destinationFolderId) requestObject,
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
    public override Task<Result<(int workspaceId, int folderId, int destinationFolderId), IError>>
        TryCreateRequest(IStateMonad stateMonad, CancellationToken cancellation)
    {
        return stateMonad.RunStepsAsync(
            Workspace.WrapArtifact(Relativity.ArtifactType.Case, stateMonad, this),
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
    public IStep<OneOf<int, StringStream>> Workspace { get; set; } = null!;

    /// <summary>
    /// The Id of the folder.
    /// </summary>
    [StepProperty(2)]
    [Required]
    public IStep<int> FolderArtifactId { get; set; } = null!;

    /// <summary>
    /// The Id of the destination folder.
    /// </summary>
    [StepProperty(3)]
    [Required]
    public IStep<int> DestinationFolderArtifactId { get; set; } = null!;
}

}
