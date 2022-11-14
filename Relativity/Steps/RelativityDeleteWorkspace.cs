using Sequence.Connectors.Relativity.ManagerInterfaces;

namespace Sequence.Connectors.Relativity.Steps;

/// <summary>
/// Deletes a relativity workspace
/// </summary>
[SCLExample(
    "RelativityDeleteWorkspace 42",
    ExecuteInTests = false,
    Description = "Deletes workspace 42"
)]
public sealed class
    RelativityDeleteWorkspace : RelativityApiRequest<SCLInt, IWorkspaceManager1, Unit, Unit>
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
        SCLInt requestObject,
        CancellationToken cancellationToken)
    {
        await service.DeleteAsync(requestObject, cancellationToken);

        return Unit.Default;
    }

    /// <inheritdoc />
    public override async ValueTask<Result<SCLInt, IError>> TryCreateRequest(
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
    public IStep<SCLOneOf<SCLInt, StringStream>> Workspace { get; set; } = null!;
}
