using System.Collections.Generic;
using Reductech.EDR.Connectors.Relativity.ManagerInterfaces;
using Reductech.EDR.Connectors.Relativity.Steps;
using Reductech.EDR.Core;
using Reductech.EDR.Core.Internal;
using Reductech.EDR.Core.Steps;
using Reductech.EDR.Core.TestHarness;
using Reductech.EDR.Core.Util;
using Relativity.Environment.V1.Workspace.Models;
using static Reductech.EDR.Core.TestHarness.StaticHelpers;

namespace Reductech.EDR.Connectors.Relativity.Tests.Steps
{

public partial class
    RelativityRetrieveWorkspaceStatisticsTests : StepTestBase<RelativityRetrieveWorkspaceStatistics,
        Entity>
{
    /// <inheritdoc />
    protected override IEnumerable<StepCase> StepCases
    {
        get
        {
            yield return new StepCase(
                        "Retrieve Workspace Statistics",
                        new Log<Entity>()
                        {
                            Value = new RelativityRetrieveWorkspaceStatistics()
                            {
                                Workspace = new OneOfStep<int, StringStream>(Constant(42)),
                            }
                        },
                        Unit.Default,
                        "(DocumentCount: 1234 FileSize: 5678)"
                    ).WithTestRelativitySettings()
                    .WithService(
                        new MockSetup<IWorkspaceManager1, WorkspaceSummary>(
                            x => x.GetWorkspaceSummaryAsync(42),
                            new WorkspaceSummary() { DocumentCount = 1234, FileSize = 5678 }
                        )
                    )
                ;
        }
    }
}

}
