using Sequence.Connectors.Relativity.ManagerInterfaces;
using Relativity.Environment.V1.Matter.Models;
using Relativity.Shared.V1.Models;
using CSharpFunctionalExtensions.ValueTasks;
#pragma warning disable CS1591

namespace Sequence.Connectors.Relativity.Steps;

public class RelativityUpdateMatter : RelativityApiRequest<(SCLInt matterArtifactId, MatterRequest
    matterRequest),
    IMatterManager1, Unit, Unit>
{
    /// <inheritdoc />
    public override IStepFactory StepFactory =>
        new SimpleStepFactory<RelativityUpdateMatter, Unit>();

    /// <inheritdoc />
    public override Result<Unit, IErrorBuilder> ConvertOutput(Unit serviceOutput)
    {
        return serviceOutput;
    }

    /// <inheritdoc />
    public override async Task<Unit> SendRequest(
        IStateMonad stateMonad,
        IMatterManager1 service,
        (SCLInt matterArtifactId, MatterRequest matterRequest) requestObject,
        CancellationToken cancellationToken)
    {
        await service.UpdateAsync(requestObject.matterArtifactId, requestObject.matterRequest);
        return Unit.Default;
    }

    /// <inheritdoc />
    public override ValueTask<Result<(SCLInt matterArtifactId, MatterRequest matterRequest), IError>>
        TryCreateRequest(IStateMonad stateMonad, CancellationToken cancellation)
    {
        return stateMonad.RunStepsAsync(
                    MatterArtifactId,
                    ClientId.WrapNullable(),
                    StatusId.WrapNullable(),
                    MatterName.WrapNullable(StepMaps.String()),
                    Number.WrapNullable(StepMaps.String()),
                    Keywords.WrapNullable(StepMaps.String()),
                    Notes.WrapNullable(StepMaps.String()),
                    cancellation
                )
                .Map(
                    x =>
                    {
                        var (matterArtifactId, clientId, statusId, matterName, number, keywords,
                            notes) = x;

                        var request = new MatterRequest();

                        if (clientId.HasValue)
                            request.Client = new Securable<ObjectIdentifier>(
                                new ObjectIdentifier() { ArtifactID = clientId.Value }
                            );

                        if (statusId.HasValue)
                            request.Status = new Securable<ObjectIdentifier>(
                                new ObjectIdentifier() { ArtifactID = statusId.Value }
                            );

                        if (matterName.HasValue)
                            request.Name = matterName.Value;

                        if (number.HasValue)
                            request.Number = number.Value;

                        if (keywords.HasValue)
                            request.Keywords = keywords.Value;

                        if (notes.HasValue)
                            request.Notes = notes.Value;

                        return (matterArtifactId, request);
                    }
                )
            ;
    }

    [StepProperty(1)][Required] public IStep<SCLInt> MatterArtifactId { get; set; } = null!;

    [StepProperty(2)]
    [DefaultValueExplanation("Do not set")]
    public IStep<SCLInt>? ClientId { get; set; } = null!;

    [StepProperty(3)]
    [DefaultValueExplanation("Do not set")]
    public IStep<SCLInt>? StatusId { get; set; } = null!;

    [StepProperty(4)]
    [DefaultValueExplanation("Do not set")]
    public IStep<StringStream>? MatterName { get; set; } = null!;

    [StepProperty(5)]
    [DefaultValueExplanation("Do not set")]
    public IStep<StringStream>? Number { get; set; } = null!;

    [StepProperty(6)]
    [DefaultValueExplanation("Do not set")]
    public IStep<StringStream>? Keywords { get; set; } = null!;

    [StepProperty(7)]
    [DefaultValueExplanation("Do not set")]
    public IStep<StringStream>? Notes { get; set; } = null!;
}
