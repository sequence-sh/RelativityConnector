using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Reductech.EDR.Core;
using Reductech.EDR.Core.Attributes;
using Reductech.EDR.Core.Internal;
using Reductech.EDR.Core.Internal.Errors;
using Reductech.EDR.Core.Util;
using Relativity.Services.Search;
using Entity = Reductech.EDR.Core.Entity;

namespace Reductech.EDR.Connectors.Relativity.Steps
{
    /// <summary>
    /// Reads a search
    /// </summary>
    public class RelativityReadKeywordSearch : RelativityApiRequest<(int workspaceId, int searchId),
        IKeywordSearchManager, KeywordSearch, Entity>
    {
        /// <inheritdoc />
        public override IStepFactory StepFactory { get; } =
            new SimpleStepFactory<RelativityReadKeywordSearch, Entity>();

        /// <inheritdoc />
        public override Result<Entity, IErrorBuilder> ConvertOutput(KeywordSearch serviceOutput)
        {
            return TryConvertToEntity(serviceOutput);
        }

        /// <inheritdoc />
        public override Task<KeywordSearch> SendRequest(IStateMonad stateMonad, IKeywordSearchManager service,
            (int workspaceId, int searchId) requestObject,
            CancellationToken cancellationToken)
        {
            return service.ReadSingleAsync(requestObject.workspaceId, requestObject.searchId);
        }

        /// <inheritdoc />
        public override Task<Result<(int workspaceId, int searchId), IError>> TryCreateRequest(IStateMonad stateMonad,
            CancellationToken cancellation)
        {
            return stateMonad.RunStepsAsync(WorkspaceId, SearchId, cancellation);
        }

        /// <summary>
        /// The Id of the workspace
        /// </summary>
        [StepProperty(1)]
        [Required]
        public IStep<int> WorkspaceId { get; set; } = null!;

        /// <summary>
        /// The Id of the search to read
        /// </summary>
        [StepProperty(2)]
        [Required]
        public IStep<int> SearchId { get; set; } = null!;
    }
}