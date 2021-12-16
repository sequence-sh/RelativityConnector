using Reductech.EDR.Connectors.Relativity.ManagerInterfaces;
using Relativity.Services.Search;

namespace Reductech.EDR.Connectors.Relativity.Steps;

/// <summary>
/// Reads a search
/// </summary>
public class RelativityReadKeywordSearch : RelativityApiRequest<(SCLInt workspaceId, SCLInt searchId),
    IKeywordSearchManager1, KeywordSearch, Entity>
{
    /// <inheritdoc />
    public override IStepFactory StepFactory { get; } =
        new SimpleStepFactory<RelativityReadKeywordSearch, Entity>();

    /// <inheritdoc />
    public override Result<Entity, IErrorBuilder> ConvertOutput(KeywordSearch serviceOutput)
    {
        return serviceOutput.ConvertToEntity();
    }

    /// <inheritdoc />
    public override Task<KeywordSearch> SendRequest(
        IStateMonad stateMonad,
        IKeywordSearchManager1 service,
        (SCLInt workspaceId, SCLInt searchId) requestObject,
        CancellationToken cancellationToken)
    {
        return service.ReadSingleAsync(requestObject.workspaceId, requestObject.searchId);
    }

    /// <inheritdoc />
    public override Task<Result<(SCLInt workspaceId, SCLInt searchId), IError>> TryCreateRequest(
        IStateMonad stateMonad,
        CancellationToken cancellation)
    {
        return stateMonad.RunStepsAsync(
            Workspace.WrapArtifact(ArtifactType.Case, stateMonad, this),
            SearchId,
            cancellation
        );
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
