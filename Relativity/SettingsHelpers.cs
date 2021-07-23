using CSharpFunctionalExtensions;
using Reductech.EDR.ConnectorManagement.Base;
using Reductech.EDR.Connectors.Relativity.Steps;
using Reductech.EDR.Core;
using Reductech.EDR.Core.Entities;
using Reductech.EDR.Core.Internal;
using Reductech.EDR.Core.Internal.Errors;
using Entity = Reductech.EDR.Core.Entity;

namespace Reductech.EDR.Connectors.Relativity
{
    /// <summary>
    /// Contains helper methods for Tesseract settings
    /// </summary>
    public static class SettingsHelpers
    {
        private static readonly string RelativityConnectorKey = typeof(RelativityImport).Assembly.GetName().Name!;

        /// <summary>
        /// Try to get a TesseractSettings from a list of Connector Informations
        /// </summary>
        public static Result<RelativitySettings, IErrorBuilder> TryGetRelativitySettings(this Entity settings)
        {
            var connectorEntityValue = settings.TryGetValue(
                new EntityPropertyKey(
                    StateMonad.ConnectorsKey,
                    RelativityConnectorKey,
                    nameof(ConnectorSettings.Settings)
                )
            );

            if (connectorEntityValue.HasNoValue ||
                connectorEntityValue.Value is not EntityValue.NestedEntity nestedEntity)
                return ErrorCode.MissingStepSettings.ToErrorBuilder(
                    RelativityConnectorKey
                );


            var connectorSettings = EntityConversionHelpers.TryCreateFromEntity<RelativitySettings>(nestedEntity.Value);

            return connectorSettings;
        }
    }
}