using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Reductech.EDR.Core;
using Reductech.EDR.Core.Entities;
using Reductech.EDR.Core.Internal;
using Reductech.EDR.Core.Internal.Errors;
using Entity = Reductech.EDR.Core.Entity;

namespace Reductech.EDR.Connectors.Relativity.Steps
{

public abstract class
    RelativityApiRequest<TRequest, TService, TServiceOutput, TOutput> : CompoundStep<TOutput>
    where TService : IDisposable
{
    protected static Result<Array<Entity>, IErrorBuilder> TryConvertToEntityArray<T>(
        IEnumerable<T> stuff)
    {
        var array = new List<Entity>();

        foreach (var v in stuff)
        {
            var r = TryConvertToEntity(v);

            if (r.IsFailure)
                return r.ConvertFailure<Array<Entity>>();

            array.Add(r.Value);
        }

        return array.ToSCLArray();
    }

    protected static Result<Entity, IErrorBuilder> TryConvertToEntity<T>(T thing)
    {
        var json = JsonConvert.SerializeObject(thing);

        var responseEntity = JsonConvert.DeserializeObject<Entity>(
            json,
            EntityJsonConverter.Instance,
            new VersionConverter()
        );

        if (responseEntity is null)
            return ErrorCode.CouldNotParse.ToErrorBuilder(json, nameof(Entity));

        return responseEntity;
    }

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

        TServiceOutput serviceOutput;

        try
        {
            serviceOutput = await SendRequest(
                stateMonad,
                serviceResult.Value,
                requestObjectResult.Value,
                cancellationToken
            );
        }
        catch (Exception ex)
        {
            return Result.Failure<TOutput, IError>(
                ErrorCode.Unknown.ToErrorBuilder(ex).WithLocation(this)
            );
        }

        var output = ConvertOutput(serviceOutput).MapError(x => x.WithLocation(this));

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

}
