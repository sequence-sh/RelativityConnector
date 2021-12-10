using Reductech.EDR.Connectors.Relativity.ManagerInterfaces;

namespace Reductech.EDR.Connectors.Relativity;

public abstract class
    RelativityApiRequest<TRequest, TService, TServiceOutput, TOutput> : CompoundStep<TOutput>
    where TService : IManager
{
    /// <inheritdoc />
    protected override async Task<Result<TOutput, IError>> Run(
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

    public abstract Task<Result<TRequest, IError>> TryCreateRequest(
        IStateMonad stateMonad,
        CancellationToken cancellation);
}
