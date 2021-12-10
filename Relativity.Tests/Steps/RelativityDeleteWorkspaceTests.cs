using Reductech.EDR.Connectors.Relativity.ManagerInterfaces;

namespace Reductech.EDR.Connectors.Relativity.Tests.Steps;

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
