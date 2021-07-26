using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Reductech.EDR.Core;
using Reductech.EDR.Core.Internal;
using Reductech.EDR.Core.Internal.Errors;
using Relativity.Services.Objects;
using Relativity.Services.Objects.DataContracts;
using Reductech.EDR.Core.Attributes;
using Reductech.EDR.Core.Util;
using Entity = Reductech.EDR.Core.Entity;


namespace Reductech.EDR.Connectors.Relativity.Steps
{
    [SCLExample("RelativityDeleteDocument WorkspaceArtifactId: 11 ObjectArtifactId: 22", ExecuteInTests = false,
        ExpectedOutput = "(Report: (DeletedItems: [(ObjectTypeName: \"document\" Action: \"delete\" Count: 1 Connection: \"object\")]))" )]
    public sealed class RelativityDeleteDocument : RelativityApiRequest<(int workspaceId, DeleteRequest deleteRequest),
        IObjectManager, DeleteResult, Entity>
    {
        /// <inheritdoc />
        public override IStepFactory StepFactory => new SimpleStepFactory<RelativityDeleteDocument, Entity>();

        /// <inheritdoc />
        public override Result<Entity, IErrorBuilder> ConvertOutput(DeleteResult serviceOutput)
        {
            return TryConvertToEntity(serviceOutput);
        }

        /// <inheritdoc />
        public override Task<DeleteResult> SendRequest(IStateMonad stateMonad, IObjectManager service,
            (int workspaceId, DeleteRequest deleteRequest) requestObject,
            CancellationToken cancellationToken)
        {
            return service.DeleteAsync(requestObject.workspaceId, requestObject.deleteRequest, cancellationToken);
        }

        /// <inheritdoc />
        public override Task<Result<(int workspaceId, DeleteRequest deleteRequest), IError>> TryCreateRequest(
            IStateMonad stateMonad, CancellationToken cancellation)
        {
            return stateMonad.RunStepsAsync(WorkspaceArtifactId, ObjectArtifactId, cancellation)
                .Map(x => (x.Item1,
                    new DeleteRequest() { Object = new RelativityObjectRef() { ArtifactID = x.Item2 } }));
        }

        /// <summary>
        /// The id of the workspace to delete the object from
        /// </summary>
        [StepProperty(1)]
        [Required]
        public IStep<int> WorkspaceArtifactId { get; set; } = null!;

        /// <summary>
        /// The id of the object to delete
        /// </summary>
        [StepProperty(2)]
        [Required]
        public IStep<int> ObjectArtifactId { get; set; } = null!;
    }
}