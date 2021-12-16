using Reductech.EDR.Connectors.Relativity.ManagerInterfaces;

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
                    new RelativityDeleteKeywordSearch
                    {
                        Workspace = new OneOfStep<SCLInt, StringStream>(Constant(123)),
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
