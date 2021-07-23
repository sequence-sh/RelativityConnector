using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Reductech.EDR.Core;
using Reductech.EDR.Core.Attributes;
using Reductech.EDR.Core.Internal;
using Reductech.EDR.Core.Internal.Errors;
using Relativity.Services.Folder;
using Entity = Reductech.EDR.Core.Entity;

namespace Reductech.EDR.Connectors.Relativity.Steps
{
    /// <summary>
    /// Retrieve the root folder of a Workspace
    /// </summary>
    public sealed class RelativityRetrieveRootFolder : RelativityApiRequest<int, IFolderManager, Folder, Entity>
    {
        /// <inheritdoc />
        public override IStepFactory StepFactory { get; } =
            new SimpleStepFactory<RelativityRetrieveRootFolder, Entity>();

        /// <inheritdoc />
        public override Result<Entity, IErrorBuilder> ConvertOutput(Folder serviceOutput)
        {
            var r = TryConvertToEntity(serviceOutput);
            return r;
        }

        /// <inheritdoc />
        public override async Task<Folder> SendRequest(IFolderManager service, int requestObject,
            CancellationToken cancellationToken)
        {
            return await service.GetWorkspaceRootAsync(requestObject);
        }

        /// <inheritdoc />
        public override async Task<Result<int, IError>> TryCreateRequest(IStateMonad stateMonad,
            CancellationToken cancellation)
        {
            return await WorkspaceArtifactId.Run(stateMonad, cancellation);
        }

        /// <summary>
        /// The Id of the workspace.
        /// </summary>
        [StepProperty(1)]
        [Required]
        public IStep<int> WorkspaceArtifactId { get; set; } = null!;
    }
}