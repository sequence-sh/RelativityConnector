using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Flurl.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Reductech.EDR.Connectors.Relativity.Errors;
using Reductech.EDR.Connectors.Relativity.ManagerInterfaces;
using Reductech.EDR.Core;
using Reductech.EDR.Core.Entities;
using Reductech.EDR.Core.Internal.Errors;
using Entity = Reductech.EDR.Core.Entity;

namespace Reductech.EDR.Connectors.Relativity
{

/// <summary>
/// Contains methods to help with API requests
/// </summary>
public static class APIRequestHelpers
{
    public static Result<Array<Entity>, IErrorBuilder> TryConvertToEntityArray<T>(
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

    public static Result<Entity, IErrorBuilder> TryConvertToEntity<T>(T thing)
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

    public static async Task<Result<TResult, IErrorBuilder>> TrySendRequest<TManager, TResult>(
        this TManager manager,
        Func<TManager, Task<TResult>> sendRequest)
        where TManager : IManager
    {
        TResult result;

        try
        {
            result = await sendRequest(manager);
        }
        catch (FlurlHttpException flurlHttpException)
        {
            IDictionary<string, object?> responseException =
                await flurlHttpException.GetResponseJsonAsync();

            var responseMessage = responseException?["Message"]?.ToString() ?? "";

            return ErrorCode_Relativity.RequestFailed
                .ToErrorBuilder(
                    flurlHttpException.StatusCode ?? 0,
                    flurlHttpException.Message,
                    responseMessage
                );
        }

        catch (Exception ex)
        {
            return ErrorCode.Unknown.ToErrorBuilder(ex);
        }

        return result;
    }
}

}
