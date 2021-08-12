using System;
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
/// Contains helper methods for Relativity settings
/// </summary>
public static class SettingsHelpers
{
    private static readonly string RelativityConnectorKey =
        typeof(RelativityImport).Assembly.GetName().Name!;

    /// <summary>
    /// Try to get a TesseractSettings from a list of Connector Informations
    /// </summary>
    public static Result<RelativitySettings, IErrorBuilder> TryGetRelativitySettings(
        this Entity settings)
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
            return ErrorCode.MissingStepSettings.ToErrorBuilder(RelativityConnectorKey);

        var connectorSettings =
            EntityConversionHelpers.TryCreateFromEntity<RelativitySettings>(nestedEntity.Value);

        return connectorSettings;
    }

    /// <summary>
    /// Get a service from the Relativity proxy
    /// </summary>
    public static Result<TService, IErrorBuilder> TryGetService<TService>(
        this IStateMonad stateMonad)
        where TService : IDisposable
    {
        var settingsResult = stateMonad.Settings.TryGetRelativitySettings();

        if (settingsResult.IsFailure)
            return settingsResult.ConvertFailure<TService>();

        var serviceFactoryFactory =
            stateMonad.ExternalContext.TryGetContext<IServiceFactoryFactory>(
                ConnectorInjection.ServiceFactoryFactoryKey
            );

        if (serviceFactoryFactory.IsFailure)
            return serviceFactoryFactory.ConvertFailure<TService>();

        var serviceFactory = serviceFactoryFactory.Value.CreateServiceFactory(settingsResult.Value);

        TService service;

        try
        {
            service = serviceFactory.CreateProxy<TService>();
        }
        catch (Exception)
        {
            return ErrorCode.MissingContext.ToErrorBuilder(typeof(TService).Name);
        }

        return service;
    }
}

}
