using System.Linq;
using OneOf;
using Sequence.Connectors.Relativity.Errors;
using Sequence.Connectors.Relativity.ManagerInterfaces;
using Relativity.Services;
using Relativity.Services.Objects.DataContracts;
using Relativity.Shared.V1.Models;
#pragma warning disable CS1591

namespace Sequence.Connectors.Relativity;

public static class RelativityStepMaps
{
    public static IRunnableStep<SCLInt> WrapArtifact(
        this IRunnableStep<SCLOneOf<SCLInt, StringStream>> step,
        ArtifactType artifactType,
        IStateMonad stateMonad,
        IStep parentStep)
    {
        return step
            .WrapStep(StepMaps.OneOf(StepMaps.DoNothing<SCLInt>(), StepMaps.DoNothing<StringStream>()))
            .WrapStep(new RelativityQueryMap(artifactType, stateMonad, parentStep));
    }

    public static IRunnableStep<SCLInt> WrapClient(
        this IRunnableStep<SCLOneOf<SCLInt, StringStream>> step,
        IStateMonad stateMonad,
        IStep parentStep)
    {
        return step.WrapStep(StepMaps.OneOf(StepMaps.DoNothing<SCLInt>(), StepMaps.DoNothing<StringStream>()))
                .WrapStep(new ClientMap(stateMonad, parentStep))
            ;
    }

    public static IRunnableStep<SCLInt> WrapMatterStatus(
        this IStep<SCLOneOf<SCLInt, SCLEnum<MatterStatus>>> step,
        IStateMonad stateMonad,
        IStep parentStep)
    {
        return step.WrapOneOf(
                    StepMaps.DoNothing<SCLInt>(),
                    StepMaps.DoNothing<SCLEnum<MatterStatus>>()
                )
                .WrapStep(new MatterStatusMap(stateMonad, parentStep))
            ;
    }

    public static IRunnableStep<ArtifactType> WrapArtifactId(
        this IStep<SCLOneOf<SCLEnum<ArtifactType>, SCLInt>> step,
        IStep parentStep)
    {
        return step.WrapStep(new ArtifactTypeMap(parentStep));
    }

    private abstract class
        RelativityArtifactIdMap<TKey, TService, TResult> : IStepValueMap<OneOf<SCLInt, TKey>, SCLInt>
        where TService : IManager
        where TKey : ISCLObject
    {
        private readonly IStateMonad _stateMonad;
        private readonly IStep _parentStep;

        protected RelativityArtifactIdMap(IStateMonad stateMonad, IStep parentStep)
        {
            _stateMonad = stateMonad;
            _parentStep = parentStep;
        }

        /// <inheritdoc />
        public async ValueTask<Result<SCLInt, IError>> Map(
            OneOf<SCLInt, TKey> t,
            CancellationToken cancellationToken)
        {
            if (t.TryPickT0(out var i, out var key))
                return i;

            var serviceResult = _stateMonad.TryGetService<TService>();

            if (serviceResult.IsFailure)
                return serviceResult.ConvertFailure<SCLInt>()
                    .MapError(x => x.WithLocation(_parentStep));

            using var service = serviceResult.Value;

            var queryResult = await service.TrySendRequest(
                    manager => GetResult(manager, key, cancellationToken)
                )
                .Bind(x => TryGetResult(x, key))
                .Map(x=>x.ConvertToSCLObject())
                .MapError(x => x.WithLocation(_parentStep));

            return queryResult;
        }

        protected abstract Task<TResult> GetResult(
            TService service,
            TKey key,
            CancellationToken cancellationToken);

        protected abstract Result<int, IErrorBuilder> TryGetResult(TResult result, TKey key);
    }

    private class ClientMap : RelativityArtifactIdMap<StringStream, IMatterManager1,
        List<DisplayableObjectIdentifier>>
    {
        public ClientMap(IStateMonad stateMonad, IStep parentStep) :
            base(stateMonad, parentStep) { }

        /// <inheritdoc />
        protected override Task<List<DisplayableObjectIdentifier>> GetResult(
            IMatterManager1 service,
            StringStream key,
            CancellationToken cancellationToken) => service.GetEligibleClientsAsync();

        /// <inheritdoc />
        protected override Result<int, IErrorBuilder> TryGetResult(
            List<DisplayableObjectIdentifier> result,
            StringStream key)
        {
            var client = result.FirstOrDefault(
                x => x.Name.Equals(key.GetString(), StringComparison.OrdinalIgnoreCase)
            );

            if (client is null)
                return ErrorCode_Relativity.ObjectNotFound.ToErrorBuilder("Client", key);

            return client.ArtifactID;
        }
    }

    private class MatterStatusMap : RelativityArtifactIdMap<SCLEnum<MatterStatus>, IMatterManager1,
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
            SCLEnum<MatterStatus> key,
            CancellationToken cancellationToken) => service.GetEligibleClientsAsync();

        /// <inheritdoc />
        protected override Result<int, IErrorBuilder> TryGetResult(
            List<DisplayableObjectIdentifier> result,
            SCLEnum<MatterStatus> key)
        {
            var client = result.FirstOrDefault(
                x => x.Name.Equals(key.ToString(), StringComparison.OrdinalIgnoreCase)
            );

            if (client is null)
                return ErrorCode_Relativity.ObjectNotFound.ToErrorBuilder("MatterStatus", key);

            return client.ArtifactID;
        }
    }

    private class
        ArtifactTypeMap : IStepValueMap<SCLOneOf<SCLEnum<ArtifactType>, SCLInt>, ArtifactType>
    {
        private readonly IStep _parentStep;

        public ArtifactTypeMap(IStep parentStep)
        {
            _parentStep = parentStep;
        }

        /// <inheritdoc />
        public async ValueTask<Result<ArtifactType, IError>> Map(
            SCLOneOf<SCLEnum<ArtifactType>, SCLInt> t,
            CancellationToken cancellationToken)
        {
            await Task.CompletedTask;

            if (t.OneOf.TryPickT0(out var at, out var i))
                return at.Value;

            if (Enum.IsDefined(typeof(ArtifactType), i.Value))
                return (ArtifactType)i.Value;

            return Result.Failure<ArtifactType, IError>(
                ErrorCode.InvalidCast.ToErrorBuilder(nameof(ArtifactType), i)
                    .WithLocation(_parentStep)
            );
        }
    }

    private class
        RelativityQueryMap : RelativityArtifactIdMap<StringStream, IObjectManager1, QueryResultSlim>
    {
        private readonly ArtifactType _artifactType;

        /// <inheritdoc />
        public RelativityQueryMap(
            ArtifactType artifactType,
            IStateMonad stateMonad,
            IStep parentStep) : base(stateMonad, parentStep)
        {
            _artifactType = artifactType;
        }

        /// <inheritdoc />
        protected override async Task<QueryResultSlim> GetResult(
            IObjectManager1 service,
            StringStream key,
            CancellationToken cancellationToken)
        {
            var keyText = await key.GetStringAsync();

            var request = new QueryRequest
            {
                Condition =
                    new TextCondition("Name", TextConditionEnum.EqualTo, keyText)
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
            StringStream key)
        {
            if (!result.Objects.Any())
                return ErrorCode_Relativity.ObjectNotFound.ToErrorBuilder(
                    _artifactType.ToString(),
                    key
                );

            return result.Objects.First().ArtifactID;
        }
    }
}
