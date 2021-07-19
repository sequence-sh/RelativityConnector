using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using Flurl.Http;
using Flurl.Http.Testing;
using Reductech.EDR.ConnectorManagement.Base;
using Reductech.EDR.Core.Internal;
using Reductech.EDR.Core.TestHarness;

namespace Reductech.EDR.Connectors.Relativity.Tests
{
    public static class TestHelpers
    {
        public static T WithRelativitySettings<TStep, TOutput, T>(this T stepCase, RelativitySettings relativitySettings)
        where T : StepTestBase<TStep,TOutput>.CaseThatExecutes
        where TStep : class, ICompoundStep<TOutput>, new()
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


        public static IFlurlClient GetFlurlClient(this HttpTest httpTest)
        {
            var type = httpTest.GetType();
            

            var property = type.GetProperty(nameof(HttpClient), BindingFlags.Instance | BindingFlags.NonPublic);
            var httpClient = (HttpClient) property.GetValue(httpTest);

            return new FlurlClient(httpClient);

        }
    }
}
