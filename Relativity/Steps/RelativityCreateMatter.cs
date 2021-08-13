using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using OneOf;
using Reductech.EDR.Connectors.Relativity.ManagerInterfaces;
using Reductech.EDR.Core;
using Reductech.EDR.Core.Attributes;
using Reductech.EDR.Core.Internal;
using Reductech.EDR.Core.Internal.Errors;
using Reductech.EDR.Core.Util;
using Relativity.Environment.V1.Matter.Models;
using Relativity.Shared.V1.Models;

namespace Reductech.EDR.Connectors.Relativity.Steps
{

public class RelativityCreateMatter : RelativityApiRequest<MatterRequest, IMatterManager1, int, int>
{
    /// <inheritdoc />
    public override IStepFactory StepFactory =>
        new SimpleStepFactory<RelativityCreateMatter, int>();

    /// <inheritdoc />
    public override Result<int, IErrorBuilder> ConvertOutput(int serviceOutput)
    {
        return serviceOutput;
    }

    /// <inheritdoc />
    public override Task<int> SendRequest(
        IStateMonad stateMonad,
        IMatterManager1 service,
        MatterRequest requestObject,
        CancellationToken cancellationToken)
    {
        return service.CreateAsync(requestObject);
    }

    /// <inheritdoc />
    public override Task<Result<MatterRequest, IError>> TryCreateRequest(
        IStateMonad stateMonad,
        CancellationToken cancellation)
    {
        var result = stateMonad.RunStepsAsync(
                Client.WrapClient(stateMonad, this),
                Status.WrapMatterStatus(stateMonad, this),
                MatterName.WrapStringStream(),
                Number.WrapStringStream(),
                Keywords.WrapNullable(StepMaps.String()),
                Notes.WrapNullable(StepMaps.String()),
                cancellation
            )
            .Map(
                x =>
                {
                    var (clientId, statusId, matterName, number, keywords, notes) = x;

                    var request = new MatterRequest()
                    {
                        Name = matterName,
                        Client =
                            new Securable<ObjectIdentifier>(
                                new ObjectIdentifier() { ArtifactID = clientId }
                            ),
                        Status = new Securable<ObjectIdentifier>(
                            new ObjectIdentifier() { ArtifactID = statusId }
                        ),
                        Number = number
                    };

                    if (keywords.HasValue)
                        request.Keywords = keywords.Value;

                    if (notes.HasValue)
                        request.Notes = notes.Value;

                    return request;
                }
            );

        return result;
    }

    [StepProperty(1)][Required] public IStep<OneOf<int, StringStream>> Client { get; set; } = null!;

    [StepProperty(2)][Required] public IStep<OneOf<int, MatterStatus>> Status { get; set; } = null!;

    /// <summary>
    /// The name of the matter to create
    /// </summary>
    [StepProperty(3)][Required] public IStep<StringStream> MatterName { get; set; } = null!;

    /// <summary>
    /// The number field
    /// </summary>
    [StepProperty(4)][Required] public IStep<StringStream> Number { get; set; } = null!;

    /// <summary>
    /// The keywords
    /// </summary>
    [StepProperty(5)][Required] public IStep<StringStream>? Keywords { get; set; } = null!;

    /// <summary>
    /// The notes
    /// </summary>
    [StepProperty(6)][Required] public IStep<StringStream>? Notes { get; set; } = null!;
}

public enum MatterStatus
{
    Active, Inactive
}

}
