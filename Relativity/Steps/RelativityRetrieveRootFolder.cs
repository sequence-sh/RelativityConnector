using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using OneOf;
using Reductech.EDR.Connectors.Relativity.ManagerInterfaces;
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

}
