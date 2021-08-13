using System.Collections.Generic;
using System.Threading;
using Reductech.EDR.Connectors.Relativity.ManagerInterfaces;
using Reductech.EDR.Connectors.Relativity.Steps;
using Reductech.EDR.Core;
using Reductech.EDR.Core.Internal;
using Reductech.EDR.Core.TestHarness;
using static Reductech.EDR.Core.TestHarness.StaticHelpers;
using Reductech.EDR.Core.Util;

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
                            Workspace = new OneOfStep<int, StringStream>(Constant(42)),
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
