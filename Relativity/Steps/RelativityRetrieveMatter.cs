using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Reductech.EDR.Connectors.Relativity.ManagerInterfaces;
using Reductech.EDR.Core;
using Reductech.EDR.Core.Attributes;
using Reductech.EDR.Core.Internal;
using Reductech.EDR.Core.Internal.Errors;
using Relativity.Environment.V1.Matter.Models;
using Entity = Reductech.EDR.Core.Entity;

namespace Reductech.EDR.Connectors.Relativity.Steps
{

public class
    RelativityRetrieveMatter : RelativityApiRequest<int, IMatterManager1, MatterResponse, Entity>
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
        int requestObject,
        CancellationToken cancellationToken)
    {
        return service.ReadAsync(requestObject);
    }

    /// <inheritdoc />
    public override Task<Result<int, IError>> TryCreateRequest(
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
    public IStep<int> MatterArtifactId { get; set; } = null!;
}

}
