using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Reductech.EDR.Connectors.Relativity.Errors;
using Reductech.EDR.Core;
using Reductech.EDR.Core.Attributes;
using Reductech.EDR.Core.Internal;
using Reductech.EDR.Core.Internal.Errors;
using Reductech.EDR.Core.Util;
using Relativity.Services.Folder;

namespace Reductech.EDR.Connectors.Relativity.Steps
{
    /// <summary>
    /// Deletes unused folders in a relativity workspace
    /// </summary>
    [SCLExample("RelativityDeleteUnusedFolders WorkspaceArtifactId: 42", ExecuteInTests = false)]
    public sealed class RelativityDeleteUnusedFolders : RelativityApiRequest<int, IFolderManager, FolderResultSet, Unit>
    {
        /// <inheritdoc />
        public override IStepFactory StepFactory { get; } = new SimpleStepFactory<RelativityDeleteUnusedFolders, Unit>();

        /// <inheritdoc />
        public override Result<Unit, IErrorBuilder> ConvertOutput(FolderResultSet serviceOutput)
        {
            if (!serviceOutput.Success)
                return ErrorCode_Relativity.Unsuccessful.ToErrorBuilder(serviceOutput.Message);


            return Unit.Default;
        }

        /// <inheritdoc />
        public override async Task<FolderResultSet> SendRequest(IStateMonad stateMonad, IFolderManager service,
            int requestObject, CancellationToken cancellationToken)
        {
            var result = await service.DeleteUnusedFoldersAsync(requestObject);

            return result;
        }

        /// <inheritdoc />
        public override async Task<Result<int, IError>> TryCreateRequest(IStateMonad stateMonad, CancellationToken cancellation)
        {
            return await WorkspaceArtifactId.Run(stateMonad, cancellation);
            
        }

        /// <summary>
        /// The Id of the workspace to delete unused folders from.
        /// </summary>
        [StepProperty(1)]
        [Required]
        public IStep<int> WorkspaceArtifactId { get; set; } = null!;
    }
}