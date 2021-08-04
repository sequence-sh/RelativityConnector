using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Reductech.EDR.Core;
using Reductech.EDR.Core.Attributes;
using Reductech.EDR.Core.Internal;
using Reductech.EDR.Core.Internal.Errors;
using Reductech.EDR.Core.Util;
using Relativity.Services.Interfaces.Document;
using Relativity.Services.Objects;
using Entity = Reductech.EDR.Core.Entity;

namespace Reductech.EDR.Connectors.Relativity.Steps
{

[SCLExample(
    "RelativityExport WorkspaceId: 12345 Condition: \"'Extracted Text' ISSET \" FieldNames: [\"Field1\", \"Field2\"] BatchSize: 10",
    ExpectedOutput = "[(Field1: \"Hello\" Field2: \"World\" NativeFile: \"Native File Text\")]",
    ExecuteInTests = false
)]
public sealed class RelativityExport : CompoundStep<Array<Entity>>
{
    /// <inheritdoc />
    protected override async Task<Result<Array<Entity>, IError>> Run(
        IStateMonad stateMonad,
        CancellationToken cancellationToken)
    {
        var settingsResult = stateMonad.Settings.TryGetRelativitySettings();

        if (settingsResult.IsFailure)
            return settingsResult.MapError(x => x.WithLocation(this))
                .ConvertFailure<Array<Entity>>();

        var stepsResult = await stateMonad.RunStepsAsync(
            WorkspaceId,
            FieldNames.WrapStringStreamArray(),
            Condition.WrapStringStream(),
            BatchSize,
            cancellationToken
        );

        if (stepsResult.IsFailure)
            return stepsResult.ConvertFailure<Array<Entity>>();

        var (workspaceId, fieldNames, condition, batchSize) = stepsResult.Value;

        var documentFileManager = stateMonad.TryGetService<IDocumentFileManager>();

        if (documentFileManager.IsFailure)
            return documentFileManager.ConvertFailure<Array<Entity>>()
                .MapError(x => x.WithLocation(this));

        var objectManager = stateMonad.TryGetService<IObjectManager>();

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
    /// The id of the workspace to export from
    /// </summary>
    [StepProperty]
    [Required]
    [Example("12345")]
    public IStep<int> WorkspaceId { get; set; } = null!;

    /// <summary>
    /// The condition that documents must meet to be exported
    /// </summary>
    [StepProperty]
    [Example("'Extracted Text' ISSET ")]
    [DefaultValueExplanation("No condition")]
    public IStep<StringStream> Condition { get; set; } = new StringConstant("");

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
    public IStep<int> BatchSize { get; set; } = new IntConstant(10);

    /// <inheritdoc />
    public override IStepFactory StepFactory =>
        new SimpleStepFactory<RelativityExport, Array<Entity>>();
}

}
