using Reductech.Sequence.Connectors.Relativity.ManagerInterfaces;
using Relativity.Environment.V1.Workspace.Models;
using Relativity.Shared.V1.Models;

namespace Reductech.Sequence.Connectors.Relativity.Steps;

/// <summary>
/// Creates a new Relativity Workspace
/// </summary>
public sealed class
    RelativityCreateWorkspace : RelativityApiRequest<WorkspaceRequest, IWorkspaceManager1,
        WorkspaceResponse, Entity>
{
    /// <inheritdoc />
    public override Result<Entity, IErrorBuilder> ConvertOutput(WorkspaceResponse serviceOutput)
    {
        return serviceOutput.ConvertToEntity();
    }

    /// <inheritdoc />
    public override async Task<WorkspaceResponse> SendRequest(
        IStateMonad stateMonad,
        IWorkspaceManager1 service,
        WorkspaceRequest requestObject,
        CancellationToken cancellationToken)
    {
        string downloadHandlerUrl = await service.GetDefaultDownloadHandlerURLAsync();
        requestObject.DownloadHandlerUrl = downloadHandlerUrl;

        var response = await service.CreateAsync(requestObject, cancellationToken);
        return response;
    }

    /// <inheritdoc />
    public override async Task<Result<WorkspaceRequest, IError>> TryCreateRequest(
        IStateMonad stateMonad,
        CancellationToken cancellation)
    {
        var stepResult = await stateMonad.RunStepsAsync(
            WorkspaceName.WrapStringStream(),
            Matter.WrapArtifact(ArtifactType.Matter, stateMonad, this),
            TemplateId.WrapArtifact(ArtifactType.Case, stateMonad, this),
            StatusId,
            ResourcePoolId.WrapArtifact(ArtifactType.ResourcePool, stateMonad, this),
            SqlServerId,
            DefaultFileRepositoryId,
            DefaultCacheLocationId,
            cancellation
        );

        if (stepResult.IsFailure)
            return stepResult.ConvertFailure<WorkspaceRequest>();

        var (name, matterId, templateId, statusId, resourcePoolId, sqlServerId,
            defaultFileRepositoryId,
            defaultCacheLocationId) = stepResult.Value;

        WorkspaceRequest request = new()
        {
            Name = name,
            Matter =
                new Securable<ObjectIdentifier>(new ObjectIdentifier() { ArtifactID = matterId }),
            Template =
                new Securable<ObjectIdentifier>(new ObjectIdentifier() { ArtifactID = templateId }),
            Status = new ObjectIdentifier() { ArtifactID = statusId },
            ResourcePool =
                new Securable<ObjectIdentifier>(
                    new ObjectIdentifier() { ArtifactID = resourcePoolId }
                ),
            SqlServer =
                new Securable<ObjectIdentifier>(
                    new ObjectIdentifier() { ArtifactID = sqlServerId }
                ),
            DefaultFileRepository =
                new Securable<ObjectIdentifier>(
                    new ObjectIdentifier() { ArtifactID = defaultFileRepositoryId }
                ),
            DefaultCacheLocation = new Securable<ObjectIdentifier>(
                new ObjectIdentifier() { ArtifactID = defaultCacheLocationId }
            ),
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
    /// The matter.
    /// A matter is the case or legal action associated with the workspace.
    /// </summary>
    [StepProperty]
    [Required]
    public IStep<SCLOneOf<SCLInt, StringStream>> Matter { get; set; } = null!;

    /// <summary>
    /// The Template Artifact Id.
    /// The Template is an object representing an existing workspace structure used to create the workspace. 
    /// </summary>
    [StepProperty]
    [Required]
    public IStep<SCLOneOf<SCLInt, StringStream>> TemplateId { get; set; } = null!;

    /// <summary>
    /// The status Artifact Id.
    /// A status is an object representing the status of the workspace used in views to filter on workspaces.
    /// </summary>
    [StepProperty]
    [Required]
    public IStep<SCLInt> StatusId { get; set; } = null!;

    /// <summary>
    /// The Resource Pool Artifact Id
    /// A resource pool represents a securable resource pool object associated with the workspace.
    /// </summary>
    [StepProperty]
    [Required]
    public IStep<SCLOneOf<SCLInt, StringStream>> ResourcePoolId { get; set; } = null!;

    /// <summary>
    /// The Sql Server Artifact Id
    /// SQL Server represents a securable SQL server object associated with the workspace.
    /// </summary>
    [StepProperty]
    [Required]
    public IStep<SCLInt> SqlServerId { get; set; } = null!;

    /// <summary>
    /// The Default File Repository Artifact Id.
    /// The Default File Repository represents a securable file repository server object associated with the workspace.
    /// </summary>
    [StepProperty]
    [Required]
    public IStep<SCLInt> DefaultFileRepositoryId { get; set; } = null!;

    /// <summary>
    /// The Default Cache Location Artifact Id
    /// The Default Cache Location represents a securable cache location server object associated with the workspace.
    /// </summary>
    [StepProperty]
    [Required]
    public IStep<SCLInt> DefaultCacheLocationId { get; set; } = null!;

    /// <inheritdoc />
    public override IStepFactory StepFactory =>
        new SimpleStepFactory<RelativityCreateWorkspace, Entity>();
}
