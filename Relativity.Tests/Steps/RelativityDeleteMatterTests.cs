using System.Collections.Generic;
using Reductech.EDR.Connectors.Relativity.ManagerInterfaces;
using Reductech.EDR.Connectors.Relativity.Steps;
using Reductech.EDR.Core.TestHarness;
using Reductech.EDR.Core.Util;
using Relativity.Environment.V1.Matter;

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
                    "Delete Matter",
                    new RelativityDeleteMatter() { MatterArtifactId = StaticHelpers.Constant(123) },
                    Unit.Default
                )
                .WithTestRelativitySettings()
                .WithService(new MockSetupUnit<IMatterManager1>(x => x.DeleteAsync(123)));
        }
    }
}

}
