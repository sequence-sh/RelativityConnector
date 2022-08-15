using Reductech.Sequence.Connectors.Relativity.ManagerInterfaces;
using Relativity.Environment.V1.Matter.Models;
#pragma warning disable CS1591
namespace Reductech.Sequence.Connectors.Relativity.Steps;

public class
    RelativityRetrieveMatter : RelativityApiRequest<SCLInt, IMatterManager1, MatterResponse, Entity>
{
    /// <inheritdoc />
    public override IStepFactory StepFactory =>
        new SimpleStepFactory<RelativityRetrieveMatter, Entity>();

    /// <inheritdoc />
    public override Result<Entity, IErrorBuilder> ConvertOutput(MatterResponse serviceOutput)
    {
        return serviceOutput.ConvertToEntity();
    }

    /// <inheritdoc />
    public override Task<MatterResponse> SendRequest(
        IStateMonad stateMonad,
        IMatterManager1 service,
        SCLInt requestObject,
        CancellationToken cancellationToken)
    {
        return service.ReadAsync(requestObject);
    }

    /// <inheritdoc />
    public override Task<Result<SCLInt, IError>> TryCreateRequest(
        IStateMonad stateMonad,
        CancellationToken cancellation)
    {
        return MatterArtifactId.Run(stateMonad, cancellation);
    }

    /// <summary>
    /// The artifact id of the matter to retrieve
    /// </summary>
    [StepProperty(1)]
    [Required]
    public IStep<SCLInt> MatterArtifactId { get; set; } = null!;
}
