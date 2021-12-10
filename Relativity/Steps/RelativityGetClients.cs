using System.Linq;
using Reductech.EDR.Connectors.Relativity.ManagerInterfaces;
using Relativity.Shared.V1.Models;

namespace Reductech.EDR.Connectors.Relativity.Steps;

/// <summary>
/// Retrieve a list of the available clients that you can associate with a matter.
/// </summary>
[SCLExample(
    "RelativityGetClients",
    ExecuteInTests = false,
    ExpectedOutput =
        "[(Name: \"Client 1\" ArtifactID: 1), (Name: \"Client 2\" ArtifactID: 2)]"
)]
public class RelativityGetClients : RelativityApiRequest<Unit, IMatterManager1,
    List<DisplayableObjectIdentifier>,
    Array<Entity>>
{
    /// <inheritdoc />
    public override IStepFactory StepFactory =>
        new SimpleStepFactory<RelativityGetClients, Array<Entity>>();

    /// <inheritdoc />
    public override Result<Array<Entity>, IErrorBuilder> ConvertOutput(
        List<DisplayableObjectIdentifier> serviceOutput)
    {
        return serviceOutput.Select(RelativityEntityConversionHelpers.ConvertToEntity).ToSCLArray();
    }

    /// <inheritdoc />
    public override Task<List<DisplayableObjectIdentifier>> SendRequest(
        IStateMonad stateMonad,
        IMatterManager1 service,
        Unit requestObject,
        CancellationToken cancellationToken)
    {
        return service.GetEligibleClientsAsync();
    }

    /// <inheritdoc />
    public override async Task<Result<Unit, IError>> TryCreateRequest(
        IStateMonad stateMonad,
        CancellationToken cancellation)
    {
        await Task.CompletedTask;
        return Unit.Default;
    }
}
