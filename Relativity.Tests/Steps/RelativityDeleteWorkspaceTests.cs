﻿using Sequence.Connectors.Relativity.ManagerInterfaces;

namespace Sequence.Connectors.Relativity.Tests.Steps;

public partial class RelativityDeleteWorkspaceTests : StepTestBase<RelativityDeleteWorkspace, Unit>
{
    /// <inheritdoc />
    protected override IEnumerable<StepCase> StepCases
    {
        get
        {
            yield return new StepCase(
                        "Delete a Workspace",
                        new RelativityDeleteWorkspace
                        {
                            Workspace = new OneOfStep<SCLInt, StringStream>(Constant(42)),
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
