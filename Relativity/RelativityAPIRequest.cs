﻿using Sequence.Connectors.Relativity.ManagerInterfaces;
#pragma warning disable CS1591
namespace Sequence.Connectors.Relativity;

public abstract class
    RelativityApiRequest<TRequest, TService, TServiceOutput, TOutput> : CompoundStep<TOutput>
    where TService : IManager
where TOutput : ISCLObject
{
    /// <inheritdoc />
    protected override async ValueTask<Result<TOutput, IError>> Run(
        IStateMonad stateMonad,
        CancellationToken cancellationToken)
    {
        var requestObjectResult = await TryCreateRequest(stateMonad, cancellationToken);

        if (requestObjectResult.IsFailure)
            return requestObjectResult.ConvertFailure<TOutput>();

        var serviceResult = stateMonad.TryGetService<TService>();

        if (serviceResult.IsFailure)
            return serviceResult.MapError(x => x.WithLocation(this)).ConvertFailure<TOutput>();

        using var service = serviceResult.Value;

        var output = await
            service.TrySendRequest(
                    (manager) => SendRequest(
                        stateMonad,
                        manager,
                        requestObjectResult.Value,
                        cancellationToken
                    )
                )
                .Bind(ConvertOutput)
                .MapError(x => x.WithLocation(this));

        return output;
    }

    public abstract Result<TOutput, IErrorBuilder> ConvertOutput(TServiceOutput serviceOutput);

    public abstract Task<TServiceOutput> SendRequest(
        IStateMonad stateMonad,
        TService service,
        TRequest requestObject,
        CancellationToken cancellationToken);

    public abstract ValueTask<Result<TRequest, IError>> TryCreateRequest(
        IStateMonad stateMonad,
        CancellationToken cancellation);
}
