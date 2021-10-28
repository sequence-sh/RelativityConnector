using System.Collections.Generic;
using Reductech.EDR.Connectors.Relativity.ManagerInterfaces;
using Reductech.EDR.Connectors.Relativity.Steps;
using Reductech.EDR.Core;
using Reductech.EDR.Core.Internal;
using Reductech.EDR.Core.TestHarness;
using static Reductech.EDR.Core.TestHarness.StaticHelpers;
using Reductech.EDR.Core.Util;
using Relativity.Services.Search;

namespace Reductech.EDR.Connectors.Relativity.Tests.Steps
{

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
                        new RelativityReadKeywordSearch()
                        {
                            Workspace = new OneOfStep<int, StringStream>(Constant(11)),
                            SearchId  = Constant(12)
                        }
                    ),
                    Unit.Default,
                    @"('ArtifactID': 12 'Name': ""My Search"" 'ArtifactTypeID': 0 'Notes': null 'Keywords': null 'QueryHint': null 'RequiresManualRun': False 'Scope': ScopeType.EntireCase 'SearchText': null 'SearchType': ""KeywordSearch"")"
                )
                .WithTestRelativitySettings()
                .WithService(
                    new MockSetup<IKeywordSearchManager1, KeywordSearch>(
                        x => x.ReadSingleAsync(11, 12),
                        new KeywordSearch() { Name = "My Search", ArtifactID = 12, }
                    )
                );
        }
    }
}

}
