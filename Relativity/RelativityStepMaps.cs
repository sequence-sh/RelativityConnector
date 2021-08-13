using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using OneOf;
using Reductech.EDR.Connectors.Relativity.Errors;
using Reductech.EDR.Connectors.Relativity.ManagerInterfaces;
using Reductech.EDR.Connectors.Relativity.Steps;
using Reductech.EDR.Core;
using Reductech.EDR.Core.Internal;
using Reductech.EDR.Core.Internal.Errors;
using Reductech.EDR.Core.Util;
using Relativity.Services;
using Relativity.Services.Objects.DataContracts;
using Relativity.Shared.V1.Models;

namespace Reductech.EDR.Connectors.Relativity
{

public static class RelativityStepMaps
{
    public static IRunnableStep<int> WrapArtifact(
        this IRunnableStep<OneOf<int, StringStream>> step,
        ArtifactType artifactType,
        IStateMonad stateMonad,
        IStep parentStep)
    {
        return step
            .WrapStep(StepMaps.OneOf(StepMaps.DoNothing<int>(), StepMaps.String()))
            .WrapStep(new RelativityQueryMap (artifactType, stateMonad, parentStep));
    }
    

    public static IRunnableStep<int> WrapClient(
        this IRunnableStep<OneOf<int, StringStream>> step,
        IStateMonad stateMonad,
        IStep parentStep)
    {
        return step.WrapStep(StepMaps.OneOf(StepMaps.DoNothing<int>(), StepMaps.String()))
                .WrapStep(new ClientMap(stateMonad, parentStep))
            ;
    }

    public static IRunnableStep<int> WrapMatterStatus(
        this IStep<OneOf<int, MatterStatus>> step,
        IStateMonad stateMonad,
        IStep parentStep)
    {
        return step.WrapStep(new MatterStatusMap(stateMonad, parentStep))
            ;
    }

    public static IRunnableStep<ArtifactType> WrapArtifactId(
        this IStep<OneOf<ArtifactType, int>> step,
        IStep parentStep)
    {
        return step.WrapStep(new ArtifactTypeMap(parentStep));
    }

    private abstract class
        RelativityArtifactIdMap<TKey, TService, TResult> : IStepValueMap<OneOf<int, TKey>, int>
        where TService : IManager
    {
        private readonly IStateMonad _stateMonad;
        private readonly IStep _parentStep;

        protected RelativityArtifactIdMap(IStateMonad stateMonad, IStep parentStep)
        {
            _stateMonad = stateMonad;
            _parentStep = parentStep;
        }

        /// <inheritdoc />
        public async Task<Result<int, IError>> Map(
            OneOf<int, TKey> t,
            CancellationToken cancellationToken)
        {
            if (t.TryPickT0(out var i, out var key))
                return i;

            var serviceResult = _stateMonad.TryGetService<TService>();

            if (serviceResult.IsFailure)
                return serviceResult.ConvertFailure<int>()
                    .MapError(x => x.WithLocation(_parentStep));

            using var service = serviceResult.Value;

            var queryResult = await service.TrySendRequest(
                    manager => GetResult(manager, key, cancellationToken)
                )
                .Bind(x => TryGetResult(x, key))
                .MapError(x => x.WithLocation(_parentStep));

            return queryResult;
        }

        protected abstract Task<TResult> GetResult(
            TService service,
            TKey key,
            CancellationToken cancellationToken);

        protected abstract Result<int, IErrorBuilder> TryGetResult(TResult result, TKey key);
    }

    private class ClientMap : RelativityArtifactIdMap<string, IMatterManager1,
        List<DisplayableObjectIdentifier>>
    {
        public ClientMap(IStateMonad stateMonad, IStep parentStep) :
            base(stateMonad, parentStep) { }

        /// <inheritdoc />
        protected override Task<List<DisplayableObjectIdentifier>> GetResult(
            IMatterManager1 service,
            string key,
            CancellationToken cancellationToken) => service.GetEligibleClientsAsync();

        /// <inheritdoc />
        protected override Result<int, IErrorBuilder> TryGetResult(
            List<DisplayableObjectIdentifier> result,
            string key)
        {
            var client = result.FirstOrDefault(
                x => x.Name.Equals(key, StringComparison.OrdinalIgnoreCase)
            );

            if (client is null)
                return ErrorCode_Relativity.ObjectNotFound.ToErrorBuilder("Client", key);

            return client.ArtifactID;
        }
    }
        
    private class MatterStatusMap : RelativityArtifactIdMap<MatterStatus, IMatterManager1,
        List<DisplayableObjectIdentifier>>
    {
        /// <inheritdoc />
        public MatterStatusMap(IStateMonad stateMonad, IStep parentStep) : base(
            stateMonad,
            parentStep
        ) { }

        /// <inheritdoc />
        protected override Task<List<DisplayableObjectIdentifier>> GetResult(
            IMatterManager1 service,
            MatterStatus key,
            CancellationToken cancellationToken) => service.GetEligibleClientsAsync();

        /// <inheritdoc />
        protected override Result<int, IErrorBuilder> TryGetResult(
            List<DisplayableObjectIdentifier> result,
            MatterStatus key)
        {
            var client = result.FirstOrDefault(
                x => x.Name.Equals(key.ToString(), StringComparison.OrdinalIgnoreCase)
            );

            if (client is null)
                return ErrorCode_Relativity.ObjectNotFound.ToErrorBuilder("MatterStatus", key);

            return client.ArtifactID;
        }
    }

    private class ArtifactTypeMap : IStepValueMap<OneOf<ArtifactType, int>, ArtifactType>
    {
        private readonly IStep _parentStep;

        public ArtifactTypeMap(IStep parentStep)
        {
            _parentStep = parentStep;
        }

        /// <inheritdoc />
        public async Task<Result<ArtifactType, IError>> Map(
            OneOf<ArtifactType, int> t,
            CancellationToken cancellationToken)
        {
            await Task.CompletedTask;

            if (t.TryPickT0(out var at, out var i))
                return at;

            if (Enum.IsDefined(typeof(ArtifactType), i))
                return (ArtifactType)i;

            return Result.Failure<ArtifactType, IError>(
                ErrorCode.InvalidCast.ToErrorBuilder(nameof(ArtifactType), i)
                    .WithLocation(_parentStep)
            );
        }
    }


    private class RelativityQueryMap : RelativityArtifactIdMap<string, IObjectManager1, QueryResultSlim>
    {
        private readonly ArtifactType _artifactType;

        /// <inheritdoc />
        public RelativityQueryMap(ArtifactType artifactType, IStateMonad stateMonad, IStep parentStep) : base(stateMonad, parentStep)
        {
            _artifactType = artifactType;
        }

        /// <inheritdoc />
        protected override async Task<QueryResultSlim> GetResult(
            IObjectManager1 service,
            string key,
            CancellationToken cancellationToken)
        {
            var request = new QueryRequest
            {
                Condition =
                    new TextCondition("Name", TextConditionEnum.Like, key)
                        .ToQueryString(),
                ObjectType = new ObjectTypeRef { ArtifactTypeID = (int)_artifactType }
            };

            var result = await service.QuerySlimAsync(
                -1,
                request,
                0,
                1,
                cancellationToken
            );

            return result;
        }

        /// <inheritdoc />
        protected override Result<int, IErrorBuilder> TryGetResult(
            QueryResultSlim result,
            string key)
        {
            if (!result.Objects.Any())
                return ErrorCode_Relativity.ObjectNotFound.ToErrorBuilder(_artifactType.ToString(), key);

            return result.Objects.First().ArtifactID;
        }
    }
}

}
