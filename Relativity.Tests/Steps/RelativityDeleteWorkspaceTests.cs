using System.Collections.Generic;
using System.Threading;
using Reductech.EDR.Connectors.Relativity.ManagerInterfaces;
using Reductech.EDR.Connectors.Relativity.Steps;
using Reductech.EDR.Core.TestHarness;
using Reductech.EDR.Core.Util;
using Relativity.Environment.V1.Workspace;

namespace Reductech.EDR.Connectors.Relativity.Tests.Steps
{

public partial class RelativityDeleteWorkspaceTests : StepTestBase<RelativityDeleteWorkspace, Unit>
{
    /// <inheritdoc />
    protected override IEnumerable<StepCase> StepCases
    {
        get
        {
            yield return new StepCase(
                        "Delete a Workspace",
                        new RelativityDeleteWorkspace()
                        {
                            WorkspaceId = StaticHelpers.Constant(42)
                        },
                        Unit.Default
                    ).WithTestRelativitySettings()
                    .WithService(
                        new MockSetupUnit<IWorkspaceManager1>(
                            x => x.DeleteAsync(42, CancellationToken.None)
                        )
                    )
                ;
        }
    }
}

}
