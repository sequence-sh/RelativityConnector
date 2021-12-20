using Reductech.Sequence.Connectors.Relativity.ManagerInterfaces;
using Relativity.Services.Search;

namespace Reductech.Sequence.Connectors.Relativity.Tests.Steps;

public partial class
    RelativityReadKeywordSearchTests : StepTestBase<RelativityReadKeywordSearch, Entity>
{
    /// <inheritdoc />
    protected override IEnumerable<StepCase> StepCases
    {
        get
        {
            yield return new StepCase(
                    "Read Search",
                    TestHelpers.LogEntity(
                        new RelativityReadKeywordSearch
                        {
                            Workspace = new OneOfStep<SCLInt, StringStream>(Constant(11)),
                            SearchId  = Constant(12)
                        }
                    ),
                    Unit.Default,
                    @"('ArtifactID': 12 'Name': ""My Search"" 'ArtifactTypeID': 0 'Notes': Null 'Keywords': Null 'QueryHint': Null 'RequiresManualRun': False 'Scope': ""EntireCase"" 'SearchText': Null 'SearchType': ""KeywordSearch"")"
                )
                .WithTestRelativitySettings()
                .WithService(
                    new MockSetup<IKeywordSearchManager1, KeywordSearch>(
                        x => x.ReadSingleAsync(11, 12),
                        new KeywordSearch { Name = "My Search", ArtifactID = 12, }
                    )
                );
        }
    }
}
