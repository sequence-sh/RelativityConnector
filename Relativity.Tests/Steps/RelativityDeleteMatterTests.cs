using System.Net.Http;
using Sequence.Connectors.Relativity.ManagerInterfaces;

namespace Sequence.Connectors.Relativity.Tests.Steps;

public partial class RelativityDeleteMatterTests : StepTestBase<RelativityDeleteMatter, Unit>
{
    /// <inheritdoc />
    protected override IEnumerable<StepCase> StepCases
    {
        get
        {
            yield return new StepCase(
                    "Delete Matter using mock service",
                    new RelativityDeleteMatter { MatterArtifactId = Constant(123) },
                    Unit.Default
                )
                .WithTestRelativitySettings()
                .WithService(new MockSetupUnit<IMatterManager1>(x => x.DeleteAsync(123)));

            yield return new StepCase(
                    "Delete Matter using mock http",
                    new RelativityDeleteMatter { MatterArtifactId = Constant(123) },
                    Unit.Default
                )
                .WithTestRelativitySettings()
                .WithFlurlMocks(
                    x => x.ForCallsTo(
                            "http://TestRelativityServer/Relativity.REST/api/relativity-environment/v1/workspaces/-1/matters/123"
                        )
                        .WithVerb(HttpMethod.Delete)
                        .RespondWith()
                );
        }
    }
}
