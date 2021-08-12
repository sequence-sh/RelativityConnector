using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using OneOf;
using Reductech.EDR.Connectors.Relativity.Errors;
using Reductech.EDR.Connectors.Relativity.ManagerInterfaces;
using Reductech.EDR.Core;
using Reductech.EDR.Core.Attributes;
using Reductech.EDR.Core.Internal;
using Reductech.EDR.Core.Internal.Errors;
using Reductech.EDR.Core.Util;
using Relativity.Services.Folder;

namespace Reductech.EDR.Connectors.Relativity.Steps
{

/// <summary>
/// Deletes unused folders in a relativity workspace
/// </summary>
[SCLExample("RelativityDeleteUnusedFolders Workspace: 42", ExecuteInTests = false)]
public sealed class
    RelativityDeleteUnusedFolders : RelativityApiRequest<int, IFolderManager1, FolderResultSet, Unit>
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
        int requestObject,
        CancellationToken cancellationToken)
    {
        var result = await service.DeleteUnusedFoldersAsync(requestObject);

        return result;
    }

    /// <inheritdoc />
    public override async Task<Result<int, IError>> TryCreateRequest(
        IStateMonad stateMonad,
        CancellationToken cancellation)
    {
        return await Workspace.WrapWorkspace(stateMonad, TextLocation).Run(stateMonad, cancellation);
    }

    /// <summary>
    /// The Workspace to delete unused folders from
    /// You can provide either the Artifact Id or the name
    /// </summary>
    [StepProperty(1)]
    [Required]
    public IStep<OneOf<int, StringStream>> Workspace { get; set; } = null!;
}

}
