using System.Collections.Generic;
using Reductech.EDR.Connectors.Relativity.ManagerInterfaces;
using Reductech.EDR.Connectors.Relativity.Steps;
using Reductech.EDR.Core;
using Reductech.EDR.Core.TestHarness;
using Reductech.EDR.Core.Util;
using Relativity.Environment.V1.Matter;
using Relativity.Shared.V1.Models;

namespace Reductech.EDR.Connectors.Relativity.Tests.Steps
{

public partial class
    RelativityGetMatterStatusesTests : StepTestBase<RelativityGetMatterStatuses, Array<Entity>>
{
    /// <inheritdoc />
    protected override IEnumerable<StepCase> StepCases
    {
        get
        {
            yield return new StepCase(
                        "Get Matter Statuses",
                        TestHelpers.LogAllEntities(new RelativityGetMatterStatuses()),
                        Unit.Default,
                        "('Name': \"Status 1\" 'ArtifactID': 1 'Guids': null)",
                        "('Name': \"Status 2\" 'ArtifactID': 2 'Guids': null)"
                    )
                    .WithTestRelativitySettings()
                    .WithService(
                        new MockSetup<IMatterManager1, List<DisplayableObjectIdentifier>>(
                            x => x.GetEligibleStatusesAsync(),
                            new List<DisplayableObjectIdentifier>()
                            {
                                new() { ArtifactID = 1, Name = "Status 1" },
                                new() { ArtifactID = 2, Name = "Status 2" }
                            }
                        )
                    )
                ;
        }
    }
}

}
