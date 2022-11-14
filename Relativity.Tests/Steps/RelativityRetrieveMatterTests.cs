using System.Net.Http;
using Sequence.Connectors.Relativity.ManagerInterfaces;
using Relativity.Environment.V1.Matter.Models;

namespace Sequence.Connectors.Relativity.Tests.Steps;

public partial class RelativityRetrieveMatterTests : StepTestBase<RelativityRetrieveMatter, Entity>
{
    /// <inheritdoc />
    protected override IEnumerable<StepCase> StepCases
    {
        get
        {
            yield return new StepCase(
                    "Retrieve Matter",
                    TestHelpers.LogEntity(
                        new RelativityRetrieveMatter
                        {
                            MatterArtifactId = Constant(1234)
                        }
                    ),
                    Unit.Default,
                    @"('ArtifactID': 1234 'Name': ""My Response"" 'Keywords': ""My Keywords"" 'Notes': Null 'Number': ""My Number"")"

                ).WithTestRelativitySettings()
                .WithService(
                    new MockSetup<IMatterManager1, MatterResponse>(
                        x => x.ReadAsync(1234),
                        new MatterResponse { ArtifactID = 1234, Number = "My Number", Name = "My Response", Keywords = "My Keywords" }
                    )
                );

            yield return new StepCase(
                    "Retrieve Matter with HTTP",
                    TestHelpers.LogEntity(
                        new RelativityRetrieveMatter
                        {
                            MatterArtifactId = Constant(1234)
                        }
                    ),
                    Unit.Default,
                    @"('ArtifactID': 1234 'Name': ""My Response"" 'Keywords': ""My Keywords"" 'Notes': Null 'Number': ""My Number"")"

                ).WithTestRelativitySettings()
                .WithFlurlMocks(
                    x => x.ForCallsTo(
                            "http://TestRelativityServer/Relativity.REST/api/relativity-environment/v1/workspaces/-1/matters/1234"
                        )
                        .WithVerb(HttpMethod.Get)
                        .RespondWithJson(
                            new MatterResponse { ArtifactID = 1234, Number = "My Number", Name = "My Response", Keywords = "My Keywords"}
                        )
                );
        }
    }
}
