using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Reductech.EDR.Connectors.Relativity.ManagerInterfaces;
using Reductech.EDR.Core;
using Reductech.EDR.Core.Attributes;
using Reductech.EDR.Core.Internal;
using Reductech.EDR.Core.Internal.Errors;
using Reductech.EDR.Core.Util;
using Relativity.Services.Search;

namespace Reductech.EDR.Connectors.Relativity.Steps
{

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
        return stateMonad.RunStepsAsync(WorkspaceId, SearchId, cancellation);
    }

    /// <summary>
    /// The Id of the workspace
    /// </summary>
    [StepProperty(1)]
    [Required]
    public IStep<int> WorkspaceId { get; set; } = null!;

    /// <summary>
    /// The Id of the search to read
    /// </summary>
    [StepProperty(2)]
    [Required]
    public IStep<int> SearchId { get; set; } = null!;
}

}
