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

namespace Reductech.EDR.Connectors.Relativity.Steps
{

/// <summary>
/// Deletes a relativity workspace
/// </summary>
[SCLExample(
    "RelativityDeleteWorkspace 42",
    ExecuteInTests = false,
    Description = "Deletes workspace 42"
)]
public sealed class
    RelativityDeleteWorkspace : RelativityApiRequest<int, IWorkspaceManager1, Unit, Unit>
{
    public override IStepFactory StepFactory { get; } =
        new SimpleStepFactory<RelativityDeleteWorkspace, Unit>();

    /// <inheritdoc />
    public override Result<Unit, IErrorBuilder> ConvertOutput(Unit serviceOutput)
    {
        return serviceOutput;
    }

    /// <inheritdoc />
    public override async Task<Unit> SendRequest(
        IStateMonad stateMonad,
        IWorkspaceManager1 service,
        int requestObject,
        CancellationToken cancellationToken)
    {
        await service.DeleteAsync(requestObject, cancellationToken);

        return Unit.Default;
    }

    /// <inheritdoc />
    public override async Task<Result<int, IError>> TryCreateRequest(
        IStateMonad stateMonad,
        CancellationToken cancellation)
    {
        var workspaceId = await Workspace
            .WrapArtifact(Relativity.ArtifactType.Case, stateMonad, this)
            .Run(stateMonad, cancellation);
        return workspaceId;
    }

    /// <summary>
    /// The Workspace to delete.
    /// You can provide either the Artifact Id or the name
    /// </summary>
    [StepProperty(1)]
    [Required]
    public IStep<OneOf<int, StringStream>> Workspace { get; set; } = null!;
}

}
