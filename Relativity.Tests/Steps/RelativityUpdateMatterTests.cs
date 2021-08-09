using System.Collections.Generic;
using Moq;
using Reductech.EDR.Connectors.Relativity.ManagerInterfaces;
using Reductech.EDR.Connectors.Relativity.Steps;
using Reductech.EDR.Core.TestHarness;
using Reductech.EDR.Core.Util;
using Relativity.Environment.V1.Matter;
using Relativity.Environment.V1.Matter.Models;

namespace Reductech.EDR.Connectors.Relativity.Tests.Steps
{

public partial class RelativityUpdateMatterTests : StepTestBase<RelativityUpdateMatter, Unit>
{
    /// <inheritdoc />
    protected override IEnumerable<StepCase> StepCases
    {
        get
        {
            yield return new StepCase(
                        "Update Matter",
                        new RelativityUpdateMatter()
                        {
                            MatterArtifactId = StaticHelpers.Constant(1234),
                            MatterName       = StaticHelpers.Constant("New Name")
                        },
                        Unit.Default
                    )
                    .WithTestRelativitySettings()
                    .WithService(
                        new MockSetupUnit<IMatterManager1>(
                            x => x.UpdateAsync(
                                1234,
                                It.Is<MatterRequest>(mr => mr.Name == "New Name")
                            )
                        )
                    )
                ;
        }
    }
}

}
