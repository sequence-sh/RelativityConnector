using Reductech.EDR.Connectors.Relativity.ManagerInterfaces;
using Reductech.EDR.Connectors.Relativity.Steps;

namespace Reductech.EDR.Connectors.Relativity.Tests.Steps;

public partial class
    RelativityDeleteKeywordSearchTests : StepTestBase<RelativityDeleteKeywordSearch, Unit>
{
    /// <inheritdoc />
    protected override IEnumerable<StepCase> StepCases
    {
        get
        {
            yield return new StepCase(
                    "Delete Keyword Search",
                    new RelativityDeleteKeywordSearch()
                    {
                        Workspace = new OneOfStep<int, StringStream>(Constant(123)),
                        SearchId  = Constant(456),
                    },
                    Unit.Default
                ).WithTestRelativitySettings()
                .WithService(
                    new MockSetupUnit<IKeywordSearchManager1>(x => 
                                                                  x.DeleteSingleAsync(123, 456))
                );
        }
    }
}
