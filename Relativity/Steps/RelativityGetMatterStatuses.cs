using System.Linq;
using Reductech.Sequence.Connectors.Relativity.ManagerInterfaces;
using Relativity.Shared.V1.Models;
#pragma warning disable CS1591
namespace Reductech.Sequence.Connectors.Relativity.Steps;

[SCLExample(
    "RelativityGetMatterStatuses",
    ExecuteInTests = false,
    ExpectedOutput =
        "[(Name: \"Status 1\" ArtifactID: 1), (Name: \"Status 2\" ArtifactID: 2)]"
)]
public class RelativityGetMatterStatuses : RelativityApiRequest<Unit, IMatterManager1,
    List<DisplayableObjectIdentifier>, Array<Entity>>
{
    /// <inheritdoc />
    public override IStepFactory StepFactory =>
        new SimpleStepFactory<RelativityGetMatterStatuses, Array<Entity>>();

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
        return service.GetEligibleStatusesAsync();
    }

    /// <inheritdoc />
    public override async ValueTask<Result<Unit, IError>> TryCreateRequest(
        IStateMonad stateMonad,
        CancellationToken cancellation)
    {
        await Task.CompletedTask;
        return Unit.Default;
    }
}
