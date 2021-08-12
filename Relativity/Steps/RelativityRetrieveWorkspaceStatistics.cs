using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Reductech.EDR.Connectors.Relativity.ManagerInterfaces;
using Reductech.EDR.Core;
using Reductech.EDR.Core.Attributes;
using Reductech.EDR.Core.Internal;
using Reductech.EDR.Core.Internal.Errors;
using Relativity.Environment.V1.Workspace;
using Relativity.Environment.V1.Workspace.Models;
using Entity = Reductech.EDR.Core.Entity;

namespace Reductech.EDR.Connectors.Relativity.Steps
{

[SCLExample(
    "RelativityRetrieveWorkspaceStatistics WorkspaceId: 42",
    expectedOutput: "(DocumentCount: 1234 FileSize: 5678)",
    ExecuteInTests = false
)]
public sealed class
    RelativityRetrieveWorkspaceStatistics : RelativityApiRequest<int, IWorkspaceManager1,
        WorkspaceSummary, Entity>
{
    /// <summary>
    /// The id of the workspace to retrieve
    /// </summary>
    [StepProperty(1)]
    [Required]
    public IStep<int> WorkspaceId { get; set; } = null!;

    /// <inheritdoc />
    public override IStepFactory StepFactory =>
        new SimpleStepFactory<RelativityRetrieveWorkspaceStatistics, Entity>();

    /// <inheritdoc />
    public override Result<Entity, IErrorBuilder> ConvertOutput(WorkspaceSummary serviceOutput)
    {
        return APIRequestHelpers.TryConvertToEntity(serviceOutput);
    }

    /// <inheritdoc />
    public override async Task<WorkspaceSummary> SendRequest(
        IStateMonad stateMonad,
        IWorkspaceManager1 service,
        int requestObject,
        CancellationToken cancellationToken)
    {
        return await service.GetWorkspaceSummaryAsync(requestObject);
    }

    /// <inheritdoc />
    public override Task<Result<int, IError>> TryCreateRequest(
        IStateMonad stateMonad,
        CancellationToken cancellation)
    {
        return WorkspaceId.Run(stateMonad, cancellation);
    }
}

}
