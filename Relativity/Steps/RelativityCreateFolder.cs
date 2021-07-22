using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Reductech.EDR.Core;
using Reductech.EDR.Core.Attributes;
using Reductech.EDR.Core.Internal;
using Reductech.EDR.Core.Internal.Errors;
using Relativity.Services.Folder;

namespace Reductech.EDR.Connectors.Relativity.Steps
{

    /// <summary>
    /// Create a folder in a relativity workspace
    /// </summary>
    public sealed class RelativityCreateFolder : RelativityApiRequest<(Folder folder, int workspaceId), IFolderManager, int, int>
    {

        /// <inheritdoc />
        public override IStepFactory StepFactory => new SimpleStepFactory<RelativityCreateFolder, int>();
        


        /// <summary>
        /// The Artifact ID of the workspace where you want to create the folder.
        /// </summary>
        [StepProperty(1)]
        [Required]
        public IStep<int> WorkspaceArtifactId { get; set; } = null!;
        
        /// <summary>
        /// The name of the Folder DTO that you want to create.
        /// </summary>
        [StepProperty(2)]
        [Alias("Name")]
        [Required]
        public IStep<StringStream> FolderName { get; set; } = null!;
        
        /// <summary>
        /// The Id of the parent where you want to add the new subfolder.
        /// </summary>
        [StepProperty(3)]
        [DefaultValueExplanation("Create the folder at the root of the workspace.")]
        public IStep<int>? ParentFolderId { get; set; } = null!;


        /// <inheritdoc />
        public override Result<int, IErrorBuilder> ConvertOutput(int serviceOutput)
        {
            return serviceOutput;
        }

        /// <inheritdoc />
        public override async Task<int> SendRequest(IFolderManager service, (Folder folder, int workspaceId) requestObject, CancellationToken cancellationToken)
        {
            var newFolderArtifactId = await service.CreateSingleAsync(requestObject.workspaceId, requestObject.folder);

            return newFolderArtifactId;
        }

        /// <inheritdoc />
        public override async Task<Result<(Folder folder, int workspaceId), IError>> TryCreateRequest(IStateMonad stateMonad, CancellationToken cancellation)
        {
            var workspaceId = await WorkspaceArtifactId.Run(stateMonad, cancellation);
            if (workspaceId.IsFailure) return workspaceId.ConvertFailure<(Folder folder, int workspaceId)>();

            var folderName = await FolderName.Run(stateMonad, cancellation).Map(x=>x.GetStringAsync());
            if (folderName.IsFailure) return folderName.ConvertFailure<(Folder folder, int workspaceId)>();

            int? parentFolderId = null;
            if (ParentFolderId is not null)
            {
                var parentFolderResult = await ParentFolderId.Run(stateMonad, cancellation);
                if (parentFolderResult.IsFailure) return parentFolderResult.ConvertFailure<(Folder folder, int workspaceId)>();
                parentFolderId = parentFolderResult.Value;
            }
            
            var folder = new Folder
            {
                Name = folderName.Value
            };

            if (parentFolderId.HasValue)
            {
                folder.ParentFolder = new FolderRef(parentFolderId.Value);
            }

            return (folder, workspaceId.Value);
        }
    }
}
