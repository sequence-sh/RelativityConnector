using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Flurl.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using OneOf;
using Reductech.EDR.Connectors.Relativity.Errors;
using Reductech.EDR.Connectors.Relativity.ManagerInterfaces;
using Reductech.EDR.Core;
using Reductech.EDR.Core.Entities;
using Reductech.EDR.Core.Internal.Errors;
using Relativity.Services;
using Relativity.Services.Objects.DataContracts;
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

    public static async Task<Result<T, IErrorBuilder>> TrySendRequest<T>(Func<Task<T>> sendRequest)
    {
        T result;

        try
        {
            result = await sendRequest();
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

    public static async Task<Result<int, IErrorBuilder>> TryGetWorkspaceId(
        OneOf<int, string> workspaceOneOf,
        IStateMonad stateMonad,
        CancellationToken cancellationToken)
    {
        if (workspaceOneOf.TryPickT0(out var workspaceId, out var workspaceName))
        {
            return workspaceId;
        }

        var serviceResult = stateMonad.TryGetService<IObjectManager1>();

        if (serviceResult.IsFailure)
            return serviceResult.ConvertFailure<int>();

        using var service = serviceResult.Value;

        var request = new QueryRequest
        {
            Condition =
                new TextCondition("Name", TextConditionEnum.Like, workspaceName)
                    .ToQueryString(),
            ObjectType = new ObjectTypeRef { ArtifactTypeID = (int)ArtifactType.Case }
        };

        var queryResult = await TrySendRequest(
                () =>
                    service.QuerySlimAsync(
                        -1,
                        request,
                        0,
                        100,
                        cancellationToken
                    )
            )
            .Ensure(
                x => x.Objects.Any(),
                ErrorCode_Relativity.ObjectNotFound.ToErrorBuilder("Workspace", workspaceName)
            );

        if (queryResult.IsFailure)
            return queryResult.ConvertFailure<int>();

        return queryResult.Value.Objects.First().ArtifactID;
    }
}

}
