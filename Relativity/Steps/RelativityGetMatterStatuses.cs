using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Reductech.EDR.Core;
using Reductech.EDR.Core.Attributes;
using Reductech.EDR.Core.Internal;
using Reductech.EDR.Core.Internal.Errors;
using Reductech.EDR.Core.Util;
using Relativity.Environment.V1.Matter;
using Relativity.Shared.V1.Models;
using Entity = Reductech.EDR.Core.Entity;

namespace Reductech.EDR.Connectors.Relativity.Steps
{

[SCLExample(
    "RelativityGetMatterStatuses",
    ExecuteInTests = false,
    ExpectedOutput =
        "[(Name: \"Status 1\" ArtifactID: 1 Guids: \"\"), (Name: \"Status 2\" ArtifactID: 2 Guids: \"\")]"
)]
public class RelativityGetMatterStatuses : RelativityApiRequest<Unit, IMatterManager,
    List<DisplayableObjectIdentifier>, Array<Entity>>
{
    /// <inheritdoc />
    public override IStepFactory StepFactory =>
        new SimpleStepFactory<RelativityGetMatterStatuses, Array<Entity>>();

    /// <inheritdoc />
    public override Result<Array<Entity>, IErrorBuilder> ConvertOutput(
        List<DisplayableObjectIdentifier> serviceOutput)
    {
        return TryConvertToEntityArray(serviceOutput);
    }

    /// <inheritdoc />
    public override Task<List<DisplayableObjectIdentifier>> SendRequest(
        IStateMonad stateMonad,
        IMatterManager service,
        Unit requestObject,
        CancellationToken cancellationToken)
    {
        return service.GetEligibleStatusesAsync();
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

}
