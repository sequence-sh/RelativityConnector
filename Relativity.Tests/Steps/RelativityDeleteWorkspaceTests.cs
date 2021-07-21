using System.Collections.Generic;
using System.Net.Http;
using Reductech.EDR.Connectors.Relativity.Steps;
using Reductech.EDR.Core.TestHarness;
using Reductech.EDR.Core.Util;

namespace Reductech.EDR.Connectors.Relativity.Tests.Steps
{
    public partial class RelativityDeleteWorkspaceTests : StepTestBase<RelativityDeleteWorkspace, Unit>
    {
        /// <inheritdoc />
        protected override IEnumerable<StepCase> StepCases
        {
            get
            {
                var flurlClientFactory = new TestFlurlClientFactory();

                flurlClientFactory.HttpTest

                    .ForCallsTo(
                        "http://TestRelativityServer/Relativity.Rest/API/relativity-environment/v1/workspace/42")
                    .WithVerb(HttpMethod.Delete)
                    .RespondWith();

                flurlClientFactory.HttpTest.RespondWith(status: 417);

                yield return new StepCase("Delete a Workspace",
                        new RelativityDeleteWorkspace()
                        {
                            WorkspaceId = StaticHelpers.Constant(42)
                        }, Unit.Default



                    ).WithRelativitySettings<RelativityDeleteWorkspace, Unit, StepCase>(
                        new RelativitySettings()
                        {
                            RelativityUsername = "UN",
                            RelativityPassword = "PW",
                            Url = "http://TestRelativityServer"
                        }
                    )
                    .WithContext(
                        ConnectorInjection.FlurlClientFactoryKey,
                        flurlClientFactory
                    );;
            }
        }

    }
}