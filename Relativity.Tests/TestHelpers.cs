using System;
using System.Collections.Generic;
using System.Reflection;
using Moq;
using Reductech.EDR.ConnectorManagement.Base;
using Reductech.EDR.Connectors.Relativity.Steps;
using Reductech.EDR.Core.Internal;
using Reductech.EDR.Core.TestHarness;

namespace Reductech.EDR.Connectors.Relativity.Tests
{
    public static class TestHelpers
    {
        public static T WithTestRelativitySettings<T>(this T stepCase)
            where T : ICaseThatExecutes
        {
            var settings = new RelativitySettings()
            {
                RelativityUsername = "UN",
                RelativityPassword = "PW",
                Url = "http://TestRelativityServer",
                DesktopClientPath = "C:/DesktopClientPath"
            };

            return stepCase.WithRelativitySettings(settings);
        }

        public static T WithService<T, TService>(this T stepCase, Action<Mock<TService>> setup)
        where TService : class, IDisposable
        where T : ICaseWithSetup
        {
            var repo = new MockRepository(MockBehavior.Strict);

            var mock = repo.Create<TService>();
            setup(mock);

            stepCase.WithContext(
                ConnectorInjection.ServiceFactoryFactoryKey,
                new TestServiceFactoryFactory(mock.Object)
            );

            return stepCase;
        }

        public static T WithRelativitySettings<T>(this T stepCase, RelativitySettings relativitySettings)
            where T : ICaseThatExecutes
        {
            var r = stepCase.WithStepFactoryStore(
                StepFactoryStore.Create(
                    new ConnectorData(
                        new ConnectorSettings()
                        {
                            Enable = true,
                            Id = RelativityAssembly.GetName().Name!,
                            Settings = new Dictionary<string, object>()
                            {
                                { nameof(RelativitySettings.RelativityUsername), relativitySettings.RelativityUsername },
                                { nameof(RelativitySettings.RelativityPassword), relativitySettings.RelativityPassword },
                                { nameof(RelativitySettings.AuthParameters), relativitySettings.AuthParameters },
                                { nameof(RelativitySettings.DesktopClientPath), relativitySettings.DesktopClientPath },
                                { nameof(RelativitySettings.Url), relativitySettings.Url },
                            }
                        }, RelativityAssembly
                    ))
            );

            return r;
        }


        private static Assembly RelativityAssembly { get; } = typeof(RelativityImport).Assembly;


        
    }
}
