using System.Collections.Generic;
using System.Net.Http;
using Moq;
using Reductech.EDR.Connectors.Relativity.ManagerInterfaces;
using Reductech.EDR.Connectors.Relativity.Steps;
using Reductech.EDR.Core.TestHarness;
using static Reductech.EDR.Core.TestHarness.StaticHelpers;
using Relativity.Environment.V1.Matter.Models;

namespace Reductech.EDR.Connectors.Relativity.Tests.Steps
{

public partial class RelativityCreateMatterTests : StepTestBase<RelativityCreateMatter, int>
{
    /// <inheritdoc />
    protected override IEnumerable<StepCase> StepCases
    {
        get
        {
            yield return new StepCase(
                    "Create Matter",
                    new RelativityCreateMatter()
                    {
                        MatterName = Constant("My Matter"),
                        ClientId   = Constant(123),
                        Keywords   = Constant("My Keywords"),
                        Notes      = Constant("My Notes"),
                        Number     = Constant("My Number"),
                        StatusId   = Constant(456)
                    },
                    42
                ).WithTestRelativitySettings()
                .WithService(
                    new MockSetup<IMatterManager1, int>(
                        x => x.CreateAsync(
                            It.Is<MatterRequest>(
                                mr =>
                                    mr.Name == "My Matter" &&
                                    mr.Client.Value.ArtifactID == 123 &&
                                    mr.Status.Value.ArtifactID == 456 &&
                                    mr.Keywords == "My Keywords" &&
                                    mr.Notes == "My Notes"
                            )
                        ),
                        42
                    )
                );

            yield return new StepCase(
                    "Create Matter with API",
                    new RelativityCreateMatter()
                    {
                        MatterName = Constant("My Matter"),
                        ClientId   = Constant(123),
                        Keywords   = Constant("My Keywords"),
                        Notes      = Constant("My Notes"),
                        Number     = Constant("My Number"),
                        StatusId   = Constant(456)
                    },
                    42
                ).WithTestRelativitySettings()
                .WithFlurlMocks(
                    x => x.ForCallsTo(
                            "http://TestRelativityServer/Relativity.REST/api/relativity-environment/v1/workspaces/-1/matters"
                        )
                        .WithVerb(HttpMethod.Post)
                        .RespondWithJson("42")
                );
        }
    }
}

}
