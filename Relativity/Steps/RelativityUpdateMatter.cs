using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Reductech.EDR.Connectors.Relativity.ManagerInterfaces;
using Reductech.EDR.Core;
using Reductech.EDR.Core.Attributes;
using Reductech.EDR.Core.Internal;
using Reductech.EDR.Core.Internal.Errors;
using Reductech.EDR.Core.Util;
using Relativity.Environment.V1.Matter;
using Relativity.Environment.V1.Matter.Models;
using Relativity.Shared.V1.Models;

namespace Reductech.EDR.Connectors.Relativity.Steps
{

public class RelativityUpdateMatter : RelativityApiRequest<(int matterArtifactId, MatterRequest
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
        (int matterArtifactId, MatterRequest matterRequest) requestObject,
        CancellationToken cancellationToken)
    {
        await service.UpdateAsync(requestObject.matterArtifactId, requestObject.matterRequest);
        return Unit.Default;
    }

    /// <inheritdoc />
    public override Task<Result<(int matterArtifactId, MatterRequest matterRequest), IError>>
        TryCreateRequest(IStateMonad stateMonad, CancellationToken cancellation)
    {
        return stateMonad.RunStepsAsync(
                    MatterArtifactId,
                    ClientId.WrapNullable(),
                    StatusId.WrapNullable(),
                    MatterName.WrapNullable(x => x.WrapStringStream()),
                    Number.WrapNullable(x => x.WrapStringStream()),
                    Keywords.WrapNullable(x => x.WrapStringStream()),
                    Notes.WrapNullable(x => x.WrapStringStream()),
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

    [StepProperty(1)][Required] public IStep<int> MatterArtifactId { get; set; } = null!;

    [StepProperty(2)]
    [DefaultValueExplanation("Do not set")]
    public IStep<int>? ClientId { get; set; } = null!;

    [StepProperty(3)]
    [DefaultValueExplanation("Do not set")]
    public IStep<int>? StatusId { get; set; } = null!;

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

}
