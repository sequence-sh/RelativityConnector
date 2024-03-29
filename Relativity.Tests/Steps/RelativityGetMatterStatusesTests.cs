﻿using Sequence.Connectors.Relativity.ManagerInterfaces;
using Relativity.Shared.V1.Models;

namespace Sequence.Connectors.Relativity.Tests.Steps;

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
                        "('Name': \"Status 1\" 'ArtifactID': 1 'Guids': [])",
                        "('Name': \"Status 2\" 'ArtifactID': 2 'Guids': [])"
                    )
                    .WithTestRelativitySettings()
                    .WithService(
                        new MockSetup<IMatterManager1, List<DisplayableObjectIdentifier>>(
                            x => x.GetEligibleStatusesAsync(),
                            new List<DisplayableObjectIdentifier>
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
