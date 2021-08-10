using System;
using System.Collections.Generic;
using System.Net.Http;
using Flurl.Http.Testing;
using Reductech.EDR.Connectors.Relativity.ManagerInterfaces;
using Reductech.EDR.Connectors.Relativity.Steps;
using Reductech.EDR.Core.TestHarness;
using Reductech.EDR.Core.Util;

namespace Reductech.EDR.Connectors.Relativity.Tests.Steps
{

public partial class RelativityDeleteMatterTests : StepTestBase<RelativityDeleteMatter, Unit>
{
    /// <inheritdoc />
    protected override IEnumerable<StepCase> StepCases
    {
        get
        {
            yield return new StepCase(
                    "Delete Matter using mock service",
                    new RelativityDeleteMatter() { MatterArtifactId = StaticHelpers.Constant(123) },
                    Unit.Default
                )
                .WithTestRelativitySettings()
                .WithService(new MockSetupUnit<IMatterManager1>(x => x.DeleteAsync(123)));

            var httpTest = new HttpTest();

            httpTest.ForCallsTo("http://TestRelativityServer/Relativity.REST/api/relativity-environment/v1/workspaces/-1/matters/123")
                .WithVerb(HttpMethod.Delete)
                .RespondWith();

            yield return new StepCase(
                    "Delete Matter using mock http",
                    new RelativityDeleteMatter() { MatterArtifactId = StaticHelpers.Constant(123) },
                    Unit.Default
                )
                .WithTestRelativitySettings()
                .WithFlurlMocks(httpTest);

        }
    }
}

}
