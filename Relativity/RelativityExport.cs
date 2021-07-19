using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Reductech.EDR.Core;
using Reductech.EDR.Core.Attributes;
using Reductech.EDR.Core.Internal;
using Reductech.EDR.Core.Internal.Errors;
using Entity = Reductech.EDR.Core.Entity;

namespace Reductech.EDR.Connectors.Relativity
{
    [SCLExample("RelativityExport WorkspaceId: 12345 Condition: \"'Extracted Text' ISSET \" FieldNames: [\"Field1\", \"Field2\"] BatchSize: 10",
        
        ExpectedOutput = "[(Field1: \"Hello\" Field2: \"World\" NativeFile: \"Native File Text\")]", ExecuteInTests = false
        )]
    public sealed class RelativityExport : CompoundStep<Array<Entity>>
    {
        /// <inheritdoc />
        protected override async Task<Result<Array<Entity>, IError>> Run(IStateMonad stateMonad,
            CancellationToken cancellationToken)
        {
            var settingsResult = SettingsHelpers.TryGetRelativitySettings(stateMonad.Settings);
            if (settingsResult.IsFailure)
                return settingsResult.MapError(x => x.WithLocation(this)).ConvertFailure<Array<Entity>>();

            var workspaceId = await WorkspaceId.Run(stateMonad, cancellationToken);
            if (workspaceId.IsFailure)
                return workspaceId.ConvertFailure<Array<Entity>>();

            var fieldNames = await FieldNames.Run(stateMonad, cancellationToken)
                .Bind(async x =>
                {
                    var objects = await x
                        .SelectAwait(async s => await s.GetStringAsync())
                        .GetElementsAsync(cancellationToken);
                    return objects;
                });


            if (fieldNames.IsFailure)
                return fieldNames.ConvertFailure<Array<Entity>>();

            var condition = await Condition.Run(stateMonad, cancellationToken).Map(x => x.GetStringAsync());
            if (condition.IsFailure)
                return condition.ConvertFailure<Array<Entity>>();

            var batchSize = await BatchSize.Run(stateMonad, cancellationToken);
            if (batchSize.IsFailure)
                return batchSize.ConvertFailure<Array<Entity>>();

            var flurlClientResult = stateMonad.GetFlurlClientFactory().Map(x=>x.FlurlClient);  

            if (flurlClientResult.IsFailure) 
                return flurlClientResult.MapError(x=>x.WithLocation(this)).ConvertFailure<Array<Entity>>();

            var entitiesResult = await
                RelativityExportHelpers.ExportAsync(settingsResult.Value, workspaceId.Value, ArtifactType.Document,
                    fieldNames.Value, condition.Value, 0,
                    batchSize.Value, 
                    flurlClientResult.Value,
                    new ErrorLocation(this), CancellationToken.None);

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
        public override IStepFactory StepFactory => new SimpleStepFactory<RelativityExport, Array<Entity>>();
    }
}