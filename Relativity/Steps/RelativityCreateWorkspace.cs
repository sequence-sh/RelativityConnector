using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Reductech.EDR.Core;
using Reductech.EDR.Core.Attributes;
using Reductech.EDR.Core.Internal;
using Reductech.EDR.Core.Internal.Errors;
using Reductech.EDR.Core.Util;
using Relativity.Environment.V1.Workspace;
using Relativity.Environment.V1.Workspace.Models;
using Relativity.Shared.V1.Models;
using Entity = Reductech.EDR.Core.Entity;

namespace Reductech.EDR.Connectors.Relativity.Steps
{
    public sealed class
        RelativityRetrieveWorkspaceStatistics : RelativityApiRequest<int, IWorkspaceManager, WorkspaceSummary, Entity>
    {
        /// <summary>
        /// The id of the workspace to retrieve
        /// </summary>
        [StepProperty(1)]
        [Required]
        public IStep<int> WorkspaceId { get; set; } = null!;


        /// <inheritdoc />
        public override IStepFactory StepFactory =>
            new SimpleStepFactory<RelativityRetrieveWorkspaceStatistics, Entity>();

        /// <inheritdoc />
        public override Result<Entity, IErrorBuilder> ConvertOutput(WorkspaceSummary serviceOutput)
        {
            return TryConvertToEntity(serviceOutput);
        }

        /// <inheritdoc />
        public override async Task<WorkspaceSummary> SendRequest(IWorkspaceManager service, int requestObject,
            CancellationToken cancellationToken)
        {
            return await service.GetWorkspaceSummaryAsync(requestObject);
        }

        /// <inheritdoc />
        public override Task<Result<int, IError>> TryCreateRequest(IStateMonad stateMonad,
            CancellationToken cancellation)
        {
            return WorkspaceId.Run(stateMonad, cancellation);
        }
    }


    public sealed class
        RelativityRetrieveWorkspace : RelativityApiRequest<(int workspaceId, bool includeMetadata, bool includeActions),
            IWorkspaceManager, WorkspaceResponse, Entity>
    {
        /// <summary>
        /// The id of the workspace to retrieve
        /// </summary>
        [StepProperty(1)]
        [Required]
        public IStep<int> WorkspaceId { get; set; } = null!;

        /// <summary>
        /// Whether to include metadata in the result
        /// </summary>
        [StepProperty(2)]
        [DefaultValueExplanation("false")]
        public IStep<bool> IncludeMetadata { get; set; } = new BoolConstant(false);

        /// <summary>
        /// Whether to include actions in the result
        /// </summary>
        [StepProperty(3)]
        [DefaultValueExplanation("false")]
        public IStep<bool> IncludeActions { get; set; } = new BoolConstant(false);

        /// <inheritdoc />
        public override IStepFactory StepFactory => new SimpleStepFactory<RelativityRetrieveWorkspace, Entity>();

        /// <inheritdoc />
        public override Result<Entity, IErrorBuilder> ConvertOutput(WorkspaceResponse serviceOutput)
        {
            var r = TryConvertToEntity(serviceOutput);
            return r;
        }

        /// <inheritdoc />
        public override async Task<WorkspaceResponse> SendRequest(IWorkspaceManager service,
            (int workspaceId, bool includeMetadata, bool includeActions) requestObject,
            CancellationToken cancellationToken)
        {
            return await service.ReadAsync(requestObject.workspaceId, requestObject.includeMetadata,
                requestObject.includeActions);
        }

        /// <inheritdoc />
        public override async Task<Result<(int workspaceId, bool includeMetadata, bool includeActions), IError>>
            TryCreateRequest(IStateMonad stateMonad, CancellationToken cancellation)
        {
            return await stateMonad.RunStepsAsync(WorkspaceId, IncludeMetadata, IncludeActions, cancellation);
        }
    }


    /// <summary>
    /// Creates a new Relativity Workspace
    /// </summary>
    public sealed class
        RelativityCreateWorkspace : RelativityApiRequest<WorkspaceRequest, IWorkspaceManager, WorkspaceResponse, Entity>
    {
        /// <inheritdoc />
        public override Result<Entity, IErrorBuilder> ConvertOutput(WorkspaceResponse serviceOutput)
        {
            var r = TryConvertToEntity(serviceOutput);
            return r;
        }

        /// <inheritdoc />
        public override async Task<WorkspaceResponse> SendRequest(IWorkspaceManager service,
            WorkspaceRequest requestObject, CancellationToken cancellationToken)
        {
            string downloadHandlerUrl = await service.GetDefaultDownloadHandlerURLAsync();
            requestObject.DownloadHandlerUrl = downloadHandlerUrl;

            var response = await service.CreateAsync(requestObject, cancellationToken);
            return response;
        }

        /// <inheritdoc />
        public override async Task<Result<WorkspaceRequest, IError>> TryCreateRequest(IStateMonad stateMonad,
            CancellationToken cancellation)
        {
            var stepResult = await stateMonad.RunStepsAsync(WorkspaceName.WrapStringStream(), MatterId, TemplateId,
                StatusId, ResourcePoolId, SqlServerId,
                DefaultFileRepositoryId, DefaultCacheLocationId, cancellation);
            if (stepResult.IsFailure) return stepResult.ConvertFailure<WorkspaceRequest>();

            var (name, matterId, templateId, statusId, resourcePoolId, sqlServerId, defaultFileRepositoryId,
                defaultCacheLocationId) = stepResult.Value;


            WorkspaceRequest request = new()
            {
                Name = name,
                Matter = new Securable<ObjectIdentifier>(new ObjectIdentifier() { ArtifactID = matterId }),
                Template = new Securable<ObjectIdentifier>(new ObjectIdentifier() { ArtifactID = templateId }),
                Status = new ObjectIdentifier() { ArtifactID = statusId },
                ResourcePool =
                    new Securable<ObjectIdentifier>(new ObjectIdentifier() { ArtifactID = resourcePoolId }),
                SqlServer = new Securable<ObjectIdentifier>(new ObjectIdentifier() { ArtifactID = sqlServerId }),
                DefaultFileRepository = new Securable<ObjectIdentifier>(new ObjectIdentifier()
                    { ArtifactID = defaultFileRepositoryId }),
                DefaultCacheLocation = new Securable<ObjectIdentifier>(new ObjectIdentifier()
                    { ArtifactID = defaultCacheLocationId }),
            };

            return request;
        }

        /// <summary>
        /// The user-friendly name of the workspace
        /// </summary>
        [StepProperty(1)]
        [Alias("Name")]
        [Required]
        public IStep<StringStream> WorkspaceName { get; set; } = null!;

        /// <summary>
        /// The matter Artifact Id.
        /// A matter is the case or legal action associated with the workspace.
        /// </summary>
        [StepProperty]
        [Required]
        public IStep<int> MatterId { get; set; } = null!;

        /// <summary>
        /// The Template Artifact Id.
        /// The Template is an object representing an existing workspace structure used to create the workspace. 
        /// </summary>
        [StepProperty]
        [Required]
        public IStep<int> TemplateId { get; set; } = null!;

        /// <summary>
        /// The status Artifact Id.
        /// A status is an object representing the status of the workspace used in views to filter on workspaces.
        /// </summary>
        [StepProperty]
        [Required]
        public IStep<int> StatusId { get; set; } = null!;

        /// <summary>
        /// The Resource Pool Artifact Id
        /// A resource pool represents a securable resource pool object associated with the workspace.
        /// </summary>
        [StepProperty]
        [Required]
        public IStep<int> ResourcePoolId { get; set; } = null!;

        /// <summary>
        /// The Sql Server Artifact Id
        /// SQL Server represents a securable SQL server object associated with the workspace.
        /// </summary>
        [StepProperty]
        [Required]
        public IStep<int> SqlServerId { get; set; } = null!;

        /// <summary>
        /// The Default File Repository Artifact Id.
        /// The Default File Repository represents a securable file repository server object associated with the workspace.
        /// </summary>
        [StepProperty]
        [Required]
        public IStep<int> DefaultFileRepositoryId { get; set; } = null!;

        /// <summary>
        /// The Default Cache Location Artifact Id
        /// The Default Cache Location represents a securable cache location server object associated with the workspace.
        /// </summary>
        [StepProperty]
        [Required]
        public IStep<int> DefaultCacheLocationId { get; set; } = null!;


        /// <inheritdoc />
        public override IStepFactory StepFactory => new SimpleStepFactory<RelativityCreateWorkspace, Entity>();
    }
}