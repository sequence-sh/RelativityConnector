using OneOf;
using Reductech.EDR.Connectors.Relativity.ManagerInterfaces;

namespace Reductech.EDR.Connectors.Relativity.Steps;

public class
    RelativityDeleteKeywordSearch : RelativityApiRequest<(int workspaceId, int searchId),
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
        (int workspaceId, int searchId) requestObject,
        CancellationToken cancellationToken)
    {
        await service.DeleteSingleAsync(requestObject.workspaceId, requestObject.searchId);

        return Unit.Default;
    }

    /// <inheritdoc />
    public override Task<Result<(int workspaceId, int searchId), IError>> TryCreateRequest(
        IStateMonad stateMonad,
        CancellationToken cancellation)
    {
        return stateMonad.RunStepsAsync(Workspace.WrapArtifact(Relativity.ArtifactType.Case,stateMonad, this), SearchId, cancellation);
    }

    /// <summary>
    /// The Workspace containing the search.
    /// You can provide either the Artifact Id or the name
    /// </summary>
    [StepProperty(1)]
    [Required]
    public IStep<OneOf<int, StringStream>> Workspace { get; set; } = null!;

    /// <summary>
    /// The Id of the search to read
    /// </summary>
    [StepProperty(2)]
    [Required]
    public IStep<int> SearchId { get; set; } = null!;
}
