using System;
using System.Runtime.Serialization;
using System.Text;
using CSharpFunctionalExtensions;
using Reductech.EDR.ConnectorManagement.Base;
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
        public static Result<RelativitySettings, IErrorBuilder> TryGetRelativitySettings(Entity settings)
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


    [DataContract]
    public sealed class RelativitySettings : IEntityConvertible
    {
        [DataMember] public string RelativityUsername { get; set; } = null!;

        [DataMember] public string RelativityPassword { get; set; } = null!;

        [DataMember] public string Url { get; set; } = null!;

        [DataMember] public string DesktopClientPath { get; set; } = null!;

        public string AuthParameters => GenerateBasicAuthorizationParameter(RelativityUsername, RelativityPassword);

        static string GenerateBasicAuthorizationParameter(string username, string password)
        {
            var unencodedUsernameAndPassword = $"{username}:{password}";
            var unencodedBytes = Encoding.ASCII.GetBytes(unencodedUsernameAndPassword);
            var base64UsernameAndPassword = Convert.ToBase64String(unencodedBytes);

            return $"Basic {base64UsernameAndPassword}";
        }
    }
}