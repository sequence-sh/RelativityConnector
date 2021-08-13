using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using OneOf;
using Reductech.EDR.Connectors.Relativity.ManagerInterfaces;
using Reductech.EDR.Core;
using Reductech.EDR.Core.Internal;
using Reductech.EDR.Core.Internal.Errors;
using Relativity.Services.Objects.DataContracts;
using Reductech.EDR.Core.Attributes;
using Reductech.EDR.Core.Util;
using Entity = Reductech.EDR.Core.Entity;

namespace Reductech.EDR.Connectors.Relativity.Steps
{

[SCLExample(
    "RelativityDeleteDocument Workspace: 11 ObjectArtifactId: 22",
    ExecuteInTests = false,
    ExpectedOutput =
        "(Report: (DeletedItems: [(ObjectTypeName: \"document\" Action: \"delete\" Count: 1 Connection: \"object\")]))"
)]
public sealed class RelativityDeleteDocument : RelativityApiRequest<(int workspaceId, DeleteRequest
    deleteRequest),
    IObjectManager1, DeleteResult, Entity>
{
    /// <inheritdoc />
    public override IStepFactory StepFactory =>
        new SimpleStepFactory<RelativityDeleteDocument, Entity>();

    /// <inheritdoc />
    public override Result<Entity, IErrorBuilder> ConvertOutput(DeleteResult serviceOutput)
    {
        return APIRequestHelpers.TryConvertToEntity(serviceOutput);
    }

    /// <inheritdoc />
    public override Task<DeleteResult> SendRequest(
        IStateMonad stateMonad,
        IObjectManager1 service,
        (int workspaceId, DeleteRequest deleteRequest) requestObject,
        CancellationToken cancellationToken)
    {
        return service.DeleteAsync(
            requestObject.workspaceId,
            requestObject.deleteRequest,
            cancellationToken
        );
    }

    /// <inheritdoc />
    public override Task<Result<(int workspaceId, DeleteRequest deleteRequest), IError>>
        TryCreateRequest(IStateMonad stateMonad, CancellationToken cancellation)
    {
        return stateMonad.RunStepsAsync(Workspace.WrapArtifact(Relativity.ArtifactType.Case,stateMonad, this), ObjectArtifactId, cancellation)
            .Map(
                x => (x.Item1,
                      new DeleteRequest()
                      {
                          Object = new RelativityObjectRef() { ArtifactID = x.Item2 }
                      })
            );
    }

    /// <summary>
    /// The Workspace containing the object to delete.
    /// You can provide either the Artifact Id or the name
    /// </summary>
    [StepProperty(1)]
    [Required]
    public IStep<OneOf<int, StringStream>> Workspace { get; set; } = null!;

    /// <summary>
    /// The id of the object to delete
    /// </summary>
    [StepProperty(2)]
    [Required]
    public IStep<int> ObjectArtifactId { get; set; } = null!;
}

}
