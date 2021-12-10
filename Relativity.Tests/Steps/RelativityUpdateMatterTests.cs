using System.Net.Http;
using Moq;
using Reductech.EDR.Connectors.Relativity.ManagerInterfaces;
using Reductech.EDR.Connectors.Relativity.Steps;
using Relativity.Environment.V1.Matter.Models;

namespace Reductech.EDR.Connectors.Relativity.Tests.Steps;

public partial class RelativityUpdateMatterTests : StepTestBase<RelativityUpdateMatter, Unit>
{
    /// <inheritdoc />
    protected override IEnumerable<StepCase> StepCases
    {
        get
        {
            yield return new StepCase(
                    "Update Matter with mock service",
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
                );

            yield return new StepCase(
                    "Update Matter with mock http",
                    new RelativityUpdateMatter()
                    {
                        MatterArtifactId = StaticHelpers.Constant(1234),
                        MatterName       = StaticHelpers.Constant("New Name")
                    },
                    Unit.Default
                )
                .WithTestRelativitySettings()
                .WithFlurlMocks(
                    x => x.ForCallsTo(
                            "http://TestRelativityServer/Relativity.REST/api/relativity-environment/v1/workspaces/-1/matters/1234"
                        )
                        .WithVerb(HttpMethod.Put)
                        .RespondWith()
                );
        }
    }
}
