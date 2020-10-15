using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using CSharpFunctionalExtensions;
using Reductech.EDR.Core;
using Reductech.EDR.Core.Attributes;
using Reductech.EDR.Core.Internal;
using Reductech.EDR.Core.Util;

namespace Reductech.Connectors.Relativity
{
    public sealed class RelativityExportStep : CompoundStep<List<string>>
    {
        /// <inheritdoc />
        public override Result<List<string>, IRunErrors> Run(StateMonad stateMonad)
        {
            var settingsResult = stateMonad.GetSettings<IRelativitySettings>(nameof(RelativityExportStep));
            if (settingsResult.IsFailure)
                return settingsResult.ConvertFailure<List<string>>();

            var workspaceId = WorkspaceId.Run(stateMonad);
            if (workspaceId.IsFailure)
                return workspaceId.ConvertFailure<List<string>>();

            var fieldIds = FieldIds.Run(stateMonad);
            if (fieldIds.IsFailure)
                return fieldIds.ConvertFailure<List<string>>();

            var condition = Condition.Run(stateMonad);
            if (condition.IsFailure)
                return condition.ConvertFailure<List<string>>();

            var batchSize = BatchSize.Run(stateMonad);
            if (batchSize.IsFailure)
                return batchSize.ConvertFailure<List<string>>();


            var entitiesResult =

                RelativityExport.ExportAsync(settingsResult.Value, workspaceId.Value, ArtifactType.Document, fieldIds.Value, condition.Value, 0,
                batchSize.Value, CancellationToken.None).Result;


            if (entitiesResult.IsFailure)
                return entitiesResult.MapFailure(x =>
                        new RunError(x, nameof(RelativityExportStep), null, ErrorCode.ExternalProcessError) as
                            IRunErrors)
                    .ConvertFailure<List<string>>();


            var strings = entitiesResult.Value.Select(x => x.Serialize).ToList();

            return strings;
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
        public IStep<string> Condition { get; set; } = new Constant<string>("");

        /// <summary>
        /// Ids of fields to export
        /// </summary>
        [StepProperty]
        [Required]
        public IStep<List<int>> FieldIds { get; set; } = null!;

        /// <summary>
        /// The batch size to use when retrieving entities.
        /// </summary>
        [StepProperty]
        [DefaultValueExplanation("10")]
        public IStep<int> BatchSize { get; set; } = new Constant<int>(10);

        /// <inheritdoc />
        public override IStepFactory StepFactory => RelativityExportStepFactory.Instance;
    }

    public sealed class RelativityExportStepFactory : SimpleStepFactory<RelativityExportStep, List<string>>
    {
        private RelativityExportStepFactory() { }

        public static SimpleStepFactory<RelativityExportStep, List<string>> Instance { get; } = new RelativityExportStepFactory();
    }

}
