using OneOf;
using Reductech.EDR.Connectors.Relativity.ManagerInterfaces;
using Relativity.Services.Folder;

namespace Reductech.EDR.Connectors.Relativity.Steps;

/// <summary>
/// Retrieve the root folder of a Workspace
/// </summary>
[SCLExample(
    scl: "RelativityRetrieveRootFolder Workspace: 42",
    expectedOutput:
    "(Name: MyRootFolder, ArtifactId: 123, HasChildren: true, Selected: false)",
    ExecuteInTests = false
)]
public sealed class
    RelativityRetrieveRootFolder : RelativityApiRequest<int, IFolderManager1, Folder, Entity>
{
    /// <inheritdoc />
    public override IStepFactory StepFactory { get; } =
        new SimpleStepFactory<RelativityRetrieveRootFolder, Entity>();

    /// <inheritdoc />
    public override Result<Entity, IErrorBuilder> ConvertOutput(Folder serviceOutput)
    {
        return serviceOutput.ConvertToEntity();
    }

    /// <inheritdoc />
    public override async Task<Folder> SendRequest(
        IStateMonad stateMonad,
        IFolderManager1 service,
        int requestObject,
        CancellationToken cancellationToken)
    {
        return await service.GetWorkspaceRootAsync(requestObject);
    }

    /// <inheritdoc />
    public override async Task<Result<int, IError>> TryCreateRequest(
        IStateMonad stateMonad,
        CancellationToken cancellation)
    {
        return await Workspace
            .WrapArtifact(ArtifactType.Case, stateMonad, this)
            .Run(stateMonad, cancellation);
    }

    /// <summary>
    /// The Workspace.
    /// You can provide either the Artifact Id or the name
    /// </summary>
    [StepProperty(1)]
    [Required]
    public IStep<OneOf<int, StringStream>> Workspace { get; set; } = null!;
}
