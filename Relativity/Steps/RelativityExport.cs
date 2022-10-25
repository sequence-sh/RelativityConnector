using Reductech.Sequence.Connectors.Relativity.ManagerInterfaces;
#pragma warning disable CS1591
namespace Reductech.Sequence.Connectors.Relativity.Steps;

[SCLExample(
    "RelativityExport Workspace: 12345 Condition: \"'Extracted Text' ISSET \" FieldNames: [\"Field1\", \"Field2\"] BatchSize: 10",
    ExpectedOutput = "[(Field1: \"Hello\" Field2: \"World\" NativeFile: \"Native File Text\")]",
    ExecuteInTests = false
)]
public sealed class RelativityExport : CompoundStep<Array<Entity>>
{
    /// <inheritdoc />
    protected override async ValueTask<Result<Array<Entity>, IError>> Run(
        IStateMonad stateMonad,
        CancellationToken cancellationToken)
    {
        var settingsResult = stateMonad.Settings.TryGetRelativitySettings();

        if (settingsResult.IsFailure)
            return settingsResult.MapError(x => x.WithLocation(this))
                .ConvertFailure<Array<Entity>>();

        var stepsResult = await stateMonad.RunStepsAsync(
            Workspace.WrapArtifact(Relativity.ArtifactType.Case, stateMonad, this),
            FieldNames.WrapStringStreamArray(),
            Condition.WrapStringStream(),
            BatchSize,
            cancellationToken
        );

        if (stepsResult.IsFailure)
            return stepsResult.ConvertFailure<Array<Entity>>();

        var (workspaceId, fieldNames, condition, batchSize) = stepsResult.Value;

        var documentFileManager = stateMonad.TryGetService<IDocumentFileManager1>();

        if (documentFileManager.IsFailure)
            return documentFileManager.ConvertFailure<Array<Entity>>()
                .MapError(x => x.WithLocation(this));

        var objectManager = stateMonad.TryGetService<IObjectManager1>();

        if (objectManager.IsFailure)
            return objectManager.ConvertFailure<Array<Entity>>()
                .MapError(x => x.WithLocation(this));

        var entitiesResult = await
            RelativityExportHelpers.ExportAsync(
                workspaceId,
                ArtifactType.Document,
                fieldNames,
                condition,
                0,
                batchSize,
                documentFileManager.Value,
                objectManager.Value,
                new ErrorLocation(this),
                CancellationToken.None
            );

        if (entitiesResult.IsFailure)
            return entitiesResult.ConvertFailure<Array<Entity>>();

        return entitiesResult.Value;
    }

    /// <summary>
    /// The Workspace to export from.
    /// You can provide either the Artifact Id or the name
    /// </summary>
    [StepProperty(1)]
    [Required]
    public IStep<SCLOneOf<SCLInt, StringStream>> Workspace { get; set; } = null!;

    /// <summary>
    /// The condition that documents must meet to be exported
    /// </summary>
    [StepProperty]
    [Example("'Extracted Text' ISSET ")]
    [DefaultValueExplanation("No condition")]
    public IStep<StringStream> Condition { get; set; } = new SCLConstant<StringStream>("");

    /// <summary>
    /// Names of fields to export
    /// </summary>
    [StepProperty]
    [Required]
    public IStep<Array<StringStream>> FieldNames { get; set; } = null!;

    /// <summary>
    /// The batch size to use when retrieving entities.
    /// </summary>
    [StepProperty]
    [DefaultValueExplanation("10")]
    public IStep<SCLInt> BatchSize { get; set; } = new SCLConstant<SCLInt>(10.ConvertToSCLObject());

    /// <inheritdoc />
    public override IStepFactory StepFactory =>
        new SimpleStepFactory<RelativityExport, Array<Entity>>();
}
