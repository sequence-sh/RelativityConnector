using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Flurl;
using Flurl.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Reductech.EDR.Core;
using Reductech.EDR.Core.Attributes;
using Reductech.EDR.Core.Entities;
using Reductech.EDR.Core.Internal;
using Reductech.EDR.Core.Internal.Errors;
using Relativity.Environment.V1.Workspace.Models;
using Relativity.Shared.V1.Models;
using Entity = Reductech.EDR.Core.Entity;

namespace Reductech.EDR.Connectors.Relativity.Steps
{
    /// <summary>
    /// Creates a new relativity workspace
    /// </summary>
    public sealed class RelativityCreateWorkspace : CompoundStep<Entity>
    {
        /// <inheritdoc />
        protected override async Task<Result<Entity, IError>> Run(IStateMonad stateMonad,
            CancellationToken cancellationToken)
        {
            var workSpaceRequest = await CreateWorkspaceRequest(stateMonad, cancellationToken);

            if (workSpaceRequest.IsFailure) return workSpaceRequest.ConvertFailure<Entity>();

            var settingsResult = stateMonad.Settings.TryGetRelativitySettings();
            if (settingsResult.IsFailure)
                return settingsResult.MapError(x => x.WithLocation(this)).ConvertFailure<Entity>();

            var flurlClientResult = stateMonad.GetFlurlClientFactory().Map(x => x.FlurlClient);
            if (flurlClientResult.IsFailure)
                return flurlClientResult.MapError(x => x.WithLocation(this)).ConvertFailure<Entity>();

            var url = Url.Combine(settingsResult.Value.Url,
                "Relativity.Rest",
                "API",
                "relativity-environment",
                $"v{settingsResult.Value.APIVersionNumber}",
                "workspace"
            );


            WorkspaceResponse response;

            try
            {
                response =
                    await url
                        .WithBasicAuth(settingsResult.Value.RelativityUsername, settingsResult.Value.RelativityPassword)
                        .WithHeader("X-CSRF-Header", "-")
                        .WithClient(flurlClientResult.Value)
                        .PostJsonAsync(workSpaceRequest.Value, cancellationToken)
                        .ReceiveJson<WorkspaceResponse>();
            }
            catch (Exception ex)
            {
                var error = ErrorCode.Unknown.ToErrorBuilder(ex).WithLocation(this);
                return Result.Failure<Entity, IError>(error);
            }

            var responseJson = JsonConvert.SerializeObject(response);

            var responseEntity = JsonConvert.DeserializeObject<Entity>(responseJson,
                EntityJsonConverter.Instance, new VersionConverter());


            return responseEntity;
        }

        private async Task<Result<WorkspaceRequest, IError>> CreateWorkspaceRequest(IStateMonad stateMonad,
            CancellationToken cancellation)
        {
            var name = await WorkspaceName.Run(stateMonad, cancellation).Map(x => x.GetStringAsync());
            if (name.IsFailure) return name.ConvertFailure<WorkspaceRequest>();

            var matterId = await MatterId.Run(stateMonad, cancellation);
            if (matterId.IsFailure) return matterId.ConvertFailure<WorkspaceRequest>();

            var templateId = await TemplateId.Run(stateMonad, cancellation);
            if (templateId.IsFailure) return templateId.ConvertFailure<WorkspaceRequest>();

            var statusId = await StatusId.Run(stateMonad, cancellation);
            if (statusId.IsFailure) return statusId.ConvertFailure<WorkspaceRequest>();

            var resourcePoolId = await ResourcePoolId.Run(stateMonad, cancellation);
            if (resourcePoolId.IsFailure) return resourcePoolId.ConvertFailure<WorkspaceRequest>();

            var sqlServerId = await SqlServerId.Run(stateMonad, cancellation);
            if (sqlServerId.IsFailure) return sqlServerId.ConvertFailure<WorkspaceRequest>();

            var defaultFileRepositoryId = await DefaultFileRepositoryId.Run(stateMonad, cancellation);
            if (defaultFileRepositoryId.IsFailure) return defaultFileRepositoryId.ConvertFailure<WorkspaceRequest>();

            var defaultCacheLocationId = await DefaultCacheLocationId.Run(stateMonad, cancellation);
            if (defaultCacheLocationId.IsFailure) return defaultCacheLocationId.ConvertFailure<WorkspaceRequest>();

            WorkspaceRequest request = new()
            {
                Name = name.Value,
                Matter = new Securable<ObjectIdentifier>(new ObjectIdentifier() { ArtifactID = matterId.Value }),
                Template = new Securable<ObjectIdentifier>(new ObjectIdentifier() { ArtifactID = templateId.Value }),
                Status = new ObjectIdentifier() { ArtifactID = statusId.Value },
                ResourcePool =
                    new Securable<ObjectIdentifier>(new ObjectIdentifier() { ArtifactID = resourcePoolId.Value }),
                SqlServer = new Securable<ObjectIdentifier>(new ObjectIdentifier() { ArtifactID = sqlServerId.Value }),
                DefaultFileRepository = new Securable<ObjectIdentifier>(new ObjectIdentifier()
                    { ArtifactID = defaultFileRepositoryId.Value }),
                DefaultCacheLocation = new Securable<ObjectIdentifier>(new ObjectIdentifier()
                    { ArtifactID = defaultCacheLocationId.Value }),
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