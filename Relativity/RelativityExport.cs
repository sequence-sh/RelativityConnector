using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Flurl.Http;
using Reductech.EDR.Core;
using Reductech.EDR.Core.Attributes;
using Reductech.EDR.Core.Internal;
using Reductech.EDR.Core.Internal.Errors;

namespace Reductech.EDR.Connectors.Relativity
{
    public sealed class RelativityExport : CompoundStep<Array<StringStream>>
    {
        /// <inheritdoc />
        protected override async Task<Result<Array<StringStream>, IError>> Run(IStateMonad stateMonad,
            CancellationToken cancellationToken)
        {
            var settingsResult = SettingsHelpers.TryGetRelativitySettings(stateMonad.Settings);
            if (settingsResult.IsFailure)
                return settingsResult.MapError(x => x.WithLocation(this)).ConvertFailure<Array<StringStream>>();

            var workspaceId = await WorkspaceId.Run(stateMonad, cancellationToken);
            if (workspaceId.IsFailure)
                return workspaceId.ConvertFailure<Array<StringStream>>();

            var fieldNames = await FieldNames.Run(stateMonad, cancellationToken)
                .Bind(async x =>
                {
                    var objects = await x
                        .SelectAwait(async s => await s.GetStringAsync())
                        .GetElementsAsync(cancellationToken);
                    return objects;
                });


            if (fieldNames.IsFailure)
                return fieldNames.ConvertFailure<Array<StringStream>>();

            var condition = await Condition.Run(stateMonad, cancellationToken).Map(x => x.GetStringAsync());
            if (condition.IsFailure)
                return condition.ConvertFailure<Array<StringStream>>();

            var batchSize = await BatchSize.Run(stateMonad, cancellationToken);
            if (batchSize.IsFailure)
                return batchSize.ConvertFailure<Array<StringStream>>();

            var flurlClientResult = stateMonad.ExternalContext.TryGetContext<IFlurlClient>(ConnectorInjection.FlurlClientKey);

            if (flurlClientResult.IsFailure) 
                return flurlClientResult.MapError(x=>x.WithLocation(this)).ConvertFailure<Array<StringStream>>();

            var entitiesResult = await
                RelativityExportHelpers.ExportAsync(settingsResult.Value, workspaceId.Value, ArtifactType.Document,
                    fieldNames.Value, condition.Value, 0,
                    batchSize.Value, 
                    flurlClientResult.Value,
                    new ErrorLocation(this), CancellationToken.None);

            if (entitiesResult.IsFailure)
                return entitiesResult.ConvertFailure<Array<StringStream>>();

            var result = entitiesResult.Value.Select(x => new StringStream(x.Serialize));

            return result;
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
        public override IStepFactory StepFactory => new SimpleStepFactory<RelativityExport, Array<StringStream>>();
    }
}