using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Reductech.EDR.Core;
using Reductech.EDR.Core.Attributes;
using Reductech.EDR.Core.Internal;
using Reductech.EDR.Core.Internal.Errors;
using Relativity.Services.Interfaces.Field;
using Relativity.Services.Interfaces.Shared.Models;
using Entity = Reductech.EDR.Core.Entity;

namespace Reductech.EDR.Connectors.Relativity.Steps
{
    public class RelativityGetAvailableObjectTypes : RelativityApiRequest<int, IFieldManager,
        List<ObjectTypeIdentifier>, Array<Entity>>
    {
        /// <inheritdoc />
        public override IStepFactory StepFactory => new SimpleStepFactory<RelativityGetAvailableObjectTypes, Array<Entity>>();

        /// <inheritdoc />
        public override Result<Array<Entity>, IErrorBuilder> ConvertOutput(List<ObjectTypeIdentifier> serviceOutput)
        {
            return TryConvertToEntityArray(serviceOutput);
        }

        /// <inheritdoc />
        public override Task<List<ObjectTypeIdentifier>> SendRequest(IStateMonad stateMonad, IFieldManager service, int requestObject, CancellationToken cancellationToken)
        {
            return service.GetAvailableObjectTypesAsync(requestObject);
        }

        /// <inheritdoc />
        public override Task<Result<int, IError>> TryCreateRequest(IStateMonad stateMonad, CancellationToken cancellation)
        {
            return WorkspaceArtifactId.Run(stateMonad, cancellation);
        }

        /// <summary>
        /// The artifact Id of the workspace
        /// </summary>
        [StepProperty(1)] 
        [Required] 
        public IStep<int> WorkspaceArtifactId { get; set; } = null!;
    }
}