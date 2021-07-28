using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Reductech.EDR.Core;
using Reductech.EDR.Core.Internal;
using Reductech.EDR.Core.Internal.Errors;
using Reductech.EDR.Core.Util;
using Relativity.Shared.V1.Models;
using Entity = Reductech.EDR.Core.Entity;
using IMatterManager = Relativity.Environment.V1.Matter.IMatterManager;

namespace Reductech.EDR.Connectors.Relativity.Steps
{

/// <summary>
/// Retrieve a list of the available clients that you can associate with a matter.
/// </summary>
public class RelativityGetClients : RelativityApiRequest<Unit, IMatterManager,
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
        return TryConvertToEntityArray(serviceOutput);
    }

    /// <inheritdoc />
    public override Task<List<DisplayableObjectIdentifier>> SendRequest(
        IStateMonad stateMonad,
        IMatterManager service,
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

}
