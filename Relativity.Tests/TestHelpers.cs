﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Moq;
using Moq.Language.Flow;
using Reductech.EDR.ConnectorManagement.Base;
using Reductech.EDR.Connectors.Relativity.Steps;
using Reductech.EDR.Connectors.Relativity.TestHelpers;
using Reductech.EDR.Core;
using Reductech.EDR.Core.Internal;
using Reductech.EDR.Core.Steps;
using Reductech.EDR.Core.TestHarness;
using Reductech.EDR.Core.Util;
using Entity = Reductech.EDR.Core.Entity;

namespace Reductech.EDR.Connectors.Relativity.Tests
{

public interface IMockSetup
{
    Type ServiceType { get; }
}

public interface IMockSetup<TService> : IMockSetup
    where TService : class, IDisposable
{
    void Setup(Mock<TService> mock);
}

public class MockSetupUnit<TService> : IMockSetup<TService>
    where TService : class, IDisposable
{
    public MockSetupUnit(Expression<Func<TService, Task>> function)
    {
        Function = function;
    }

    public Expression<Func<TService, Task>> Function { get; }

    public void Setup(Mock<TService> mock)
    {
        mock.Setup(Function).Returns(Task.CompletedTask);
    }

    /// <inheritdoc />
    public Type ServiceType => typeof(TService);
}

public class MockSetup<TService, TResult> : IMockSetup<TService>
    where TService : class, IDisposable
{
    public MockSetup(Expression<Func<TService, Task<TResult>>> function, TResult result)
    {
        Function = function;
        Result   = result;
    }

    public Maybe<Action<ISetup<TService, Task<TResult>>>> AdditionalAction { get; set; } =
        Maybe<Action<ISetup<TService, Task<TResult>>>>.None;

    public Expression<Func<TService, Task<TResult>>> Function { get; }

    public TResult Result { get; }

    public void Setup(Mock<TService> mock)
    {
        var s = mock.Setup(Function);

        if (AdditionalAction.HasValue)
            AdditionalAction.Value(s);

        s.ReturnsAsync(Result);
    }

    /// <inheritdoc />
    public Type ServiceType => typeof(TService);
}

public static class TestHelpers
{
    public static IStep<Unit> LogEntity(IStep<Entity> entityStep)
    {
        return new Log<Entity>() { Value = entityStep };
    }

    public static IStep<Unit> LogAllEntities(IStep<Array<Entity>> arrayStep)
    {
        return new ForEach<Entity>()
        {
            Action = new LambdaFunction<Entity, Unit>(
                null,
                new Log<Entity>() { Value = new GetAutomaticVariable<Entity>() }
            ),
            Array = arrayStep
        };
    }

    public static T WithTestRelativitySettings<T>(this T stepCase)
        where T : ICaseThatExecutes
    {
        var settings = new RelativitySettings()
        {
            RelativityUsername = "UN",
            RelativityPassword = "PW",
            Url                = "http://TestRelativityServer",
            DesktopClientPath  = "C:/DesktopClientPath"
        };

        return stepCase.WithRelativitySettings(settings);
    }

    public static T WithService<T>(this T stepCase, params IMockSetup[] mockSetups)
        where T : ICaseWithSetup
    {
        var repo = new MockRepository(MockBehavior.Strict);

        var services = new List<IDisposable>();

        foreach (var grouping in mockSetups.GroupBy(x => x.ServiceType))
        {
            Mock mock = CreateMock(repo, grouping.First() as dynamic, grouping.ToList());

            services.Add(mock.Object as IDisposable);
        }

        stepCase.WithContext(
            ConnectorInjection.ServiceFactoryFactoryKey,
            new TestServiceFactoryFactory(services.ToArray())
        );

        return stepCase;
    }

    private static Mock CreateMock<TService>(
        MockRepository repo,
        IMockSetup<TService> mockSetup1,
        IEnumerable<IMockSetup> mockSetups)
        where TService : class, IDisposable
    {
        var mock = repo.Create<TService>();

        foreach (var mockSetup in mockSetups.Cast<IMockSetup<TService>>())
        {
            mockSetup.Setup(mock);
        }

        mock.Setup(x => x.Dispose());

        return mock;
    }

    public static T WithRelativitySettings<T>(
        this T stepCase,
        RelativitySettings relativitySettings)
        where T : ICaseThatExecutes
    {
        var r = stepCase.WithStepFactoryStore(
            StepFactoryStore.Create(
                new ConnectorData(
                    new ConnectorSettings()
                    {
                        Enable = true,
                        Id     = RelativityAssembly.GetName().Name!,
                        Settings = new Dictionary<string, object>()
                        {
                            {
                                nameof(RelativitySettings.RelativityUsername),
                                relativitySettings.RelativityUsername
                            },
                            {
                                nameof(RelativitySettings.RelativityPassword),
                                relativitySettings.RelativityPassword
                            },
                            {
                                nameof(RelativitySettings.AuthParameters),
                                relativitySettings.AuthParameters
                            },
                            {
                                nameof(RelativitySettings.DesktopClientPath),
                                relativitySettings.DesktopClientPath
                            },
                            { nameof(RelativitySettings.Url), relativitySettings.Url },
                        }
                    },
                    RelativityAssembly
                )
            )
        );

        return r;
    }

    private static Assembly RelativityAssembly { get; } = typeof(RelativityImport).Assembly;
}

}
