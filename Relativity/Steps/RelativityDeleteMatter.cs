using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Reductech.EDR.Core;
using Reductech.EDR.Core.Attributes;
using Reductech.EDR.Core.Internal;
using Reductech.EDR.Core.Internal.Errors;
using Reductech.EDR.Core.Util;
using Relativity.Environment.V1.Matter;

namespace Reductech.EDR.Connectors.Relativity.Steps
{
    /// <summary>
    /// Delete a matter
    /// </summary>
    [SCLExample("RelativityDeleteMatter 123", ExecuteInTests = false)]
    public class RelativityDeleteMatter : RelativityApiRequest<int, IMatterManager, Unit, Unit>
    {
        /// <inheritdoc />
        public override IStepFactory StepFactory => new SimpleStepFactory<RelativityDeleteMatter, Unit>();

        /// <inheritdoc />
        public override Result<Unit, IErrorBuilder> ConvertOutput(Unit serviceOutput)
        {
            return serviceOutput;
        }

        /// <inheritdoc />
        public override async Task<Unit> SendRequest(IStateMonad stateMonad, IMatterManager service, int requestObject,
            CancellationToken cancellationToken)
        {
            await service.DeleteAsync(requestObject);
            return Unit.Default;
        }

        /// <inheritdoc />
        public override Task<Result<int, IError>> TryCreateRequest(IStateMonad stateMonad, CancellationToken cancellation)
        {
            return MatterArtifactId.Run(stateMonad, cancellation);
        }

        /// <summary>
        /// The artifact id of the matter to delete
        /// </summary>
        [StepProperty(1)] [Required] public IStep<int> MatterArtifactId { get; set; } = null!;
    }
}