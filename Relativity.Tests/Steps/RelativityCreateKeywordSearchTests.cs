using System.Collections.Generic;
using System.Linq;
using Moq;
using Reductech.EDR.Connectors.Relativity.ManagerInterfaces;
using Reductech.EDR.Connectors.Relativity.Steps;
using Reductech.EDR.Core;
using Reductech.EDR.Core.Internal;
using Reductech.EDR.Core.TestHarness;
using Relativity.Services.Search;
using static Reductech.EDR.Core.TestHarness.StaticHelpers;

namespace Reductech.EDR.Connectors.Relativity.Tests.Steps
{

public partial class
    RelativityCreateKeywordSearchTests : StepTestBase<RelativityCreateKeywordSearch, int>
{
    /// <inheritdoc />
    protected override IEnumerable<StepCase> StepCases
    {
        get
        {
            yield return new StepCase(
                        "Create Keyword",
                        new RelativityCreateKeywordSearch
                        {
                            Workspace  = new OneOfStep<int, StringStream>(Constant(10)),
                            SearchName = Constant("My Search"),
                            SearchText = Constant("Search Text"),
                            SortByRank = Constant(true),
                            FieldArtifactIds =
                                new OneOfStep<Array<int>, Array<StringStream>>(Array(22, 33, 44)),
                            Notes                   = Constant("My Notes"),
                            Keywords                = Constant("My Keywords"),
                            Scope                   = Constant(SearchScope.EntireCase),
                            SearchFolderArtifactIds = Array(55, 66)
                        },
                        42
                    ).WithTestRelativitySettings()
                    .WithService(
                        new MockSetup<IKeywordSearchManager1, int>(
                            x =>
                                x.CreateSingleAsync(
                                    10,
                                    It.Is<KeywordSearch>(
                                        ks =>
                                            ks.Name == "My Search" &&
                                            ks.ArtifactTypeID == 10 &&
                                            ks.SearchText == "Search Text" &&
                                            ks.SortByRank == true &&
                                            ks.Fields.Select(f => f.ArtifactID)
                                                .SequenceEqual(new[] { 22, 33, 44 }) &&
                                            ks.Notes == "My Notes" &&
                                            ks.Keywords == "My Keywords" &&
                                            ks.Scope == ScopeType.EntireCase &&
                                            ks.SearchFolders.Select(f => f.ArtifactID)
                                                .SequenceEqual(new[] { 55, 66 })
                                    )
                                ),
                            42
                        )
                    )
                ;
        }
    }
}

}
