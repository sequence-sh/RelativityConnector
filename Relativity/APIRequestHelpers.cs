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
using Reductech.EDR.Core.Internal;
using Reductech.EDR.Core.Internal.Errors;
using Reductech.EDR.Core.Util;
using Relativity.Services;
using Relativity.Services.Objects.DataContracts;
using Entity = Reductech.EDR.Core.Entity;

namespace Reductech.EDR.Connectors.Relativity
{

public static class RelativityStepMaps
{
    public static IRunnableStep<int> WrapWorkspace(
        this IStep<OneOf<int, StringStream>> step,
        IStateMonad stateMonad,
        IStep parentStep)
    {
        return step.WrapStep(new WorkspaceMap(stateMonad, parentStep));
    }

    public static IRunnableStep<ArtifactType> WrapArtifactId(this IStep<OneOf<ArtifactType, int>> step, IStep parentStep)
    {
        return step.WrapStep(new ArtifactIdMap(parentStep));
    }

    private class ArtifactIdMap : IStepValueMap<OneOf<ArtifactType, int>, ArtifactType>
    {
        private readonly IStep _parentStep;

        public ArtifactIdMap(IStep parentStep) {
            _parentStep = parentStep;
        }

        /// <inheritdoc />
        public async Task<Result<ArtifactType, IError>> Map(OneOf<ArtifactType, int> t, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            if (t.TryPickT0(out var at, out var i))
                return at;

            if (Enum.IsDefined(typeof(ArtifactType), i))
                return (ArtifactType)i;

            return Result.Failure<ArtifactType, IError>(
                    ErrorCode.InvalidCast.ToErrorBuilder(nameof(ArtifactType), i).WithLocation(_parentStep)
                    );
        }
    }

    private class WorkspaceMap : IStepValueMap<OneOf<int, StringStream>, int>
    {

        private readonly IStateMonad _stateMonad;
        private readonly IStep _parentStep;

        public WorkspaceMap(IStateMonad stateMonad, IStep parentStep)
        {
            _stateMonad = stateMonad;
            _parentStep = parentStep;
        }

        /// <inheritdoc />
        public async Task<Result<int, IError>> Map(
            OneOf<int, StringStream> t,
            CancellationToken cancellationToken)
        {
            if (t.TryPickT0(out var workspaceId, out var workspaceNameStringStream))
                return workspaceId;

            var workspaceName = await workspaceNameStringStream.GetStringAsync();

            var workspaceIdResult = await TryGetWorkspaceId(
                    workspaceName,
                    _stateMonad,
                    cancellationToken
                )
                .MapError(x => x.WithLocation(_parentStep));

            return workspaceIdResult;
        }

        private static async Task<Result<int, IErrorBuilder>> TryGetWorkspaceId(
            string workspaceName,
            IStateMonad stateMonad,
            CancellationToken cancellationToken)
        {
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

            var queryResult = await service.TrySendRequest(
                    manager =>
                        manager.QuerySlimAsync(
                            -1,
                            request,
                            0,
                            1,
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
