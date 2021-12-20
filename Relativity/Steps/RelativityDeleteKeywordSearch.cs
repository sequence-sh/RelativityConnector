using Reductech.Sequence.Connectors.Relativity.ManagerInterfaces;

namespace Reductech.Sequence.Connectors.Relativity.Steps;

public class
    RelativityDeleteKeywordSearch : RelativityApiRequest<(SCLInt workspaceId, SCLInt searchId),
        IKeywordSearchManager1,
        Unit,
        Unit>
{
    /// <inheritdoc />
    public override IStepFactory StepFactory =>
        new SimpleStepFactory<RelativityDeleteKeywordSearch, Unit>();

    /// <inheritdoc />
    public override Result<Unit, IErrorBuilder> ConvertOutput(Unit serviceOutput)
    {
        return serviceOutput;
    }

    /// <inheritdoc />
    public override async Task<Unit> SendRequest(
        IStateMonad stateMonad,
        IKeywordSearchManager1 service,
        (SCLInt workspaceId, SCLInt searchId) requestObject,
        CancellationToken cancellationToken)
    {
        await service.DeleteSingleAsync(requestObject.workspaceId, requestObject.searchId);

        return Unit.Default;
    }

    /// <inheritdoc />
    public override Task<Result<(SCLInt workspaceId, SCLInt searchId), IError>> TryCreateRequest(
        IStateMonad stateMonad,
        CancellationToken cancellation)
    {
        return stateMonad.RunStepsAsync(Workspace.WrapArtifact(ArtifactType.Case,stateMonad, this), SearchId, cancellation);
    }

    /// <summary>
    /// The Workspace containing the search.
    /// You can provide either the Artifact Id or the name
    /// </summary>
    [StepProperty(1)]
    [Required]
    public IStep<SCLOneOf<SCLInt, StringStream>> Workspace { get; set; } = null!;

    /// <summary>
    /// The Id of the search to read
    /// </summary>
    [StepProperty(2)]
    [Required]
    public IStep<SCLInt> SearchId { get; set; } = null!;
}
