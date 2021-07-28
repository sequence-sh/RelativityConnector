using System.Collections.Generic;
using System.Linq;
using Moq;
using Reductech.EDR.Connectors.Relativity.Steps;
using Reductech.EDR.Core.TestHarness;
using Relativity.Services.Search;

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
                        new RelativityCreateKeywordSearch()
                        {
                            WorkspaceId = StaticHelpers.Constant(10),
                            SearchName = StaticHelpers.Constant("My Search"),
                            SearchText = StaticHelpers.Constant("Search Text"),
                            SortByRank = StaticHelpers.Constant(true),
                            FieldArtifactIds = StaticHelpers.Array(22, 33, 44),
                            Notes = StaticHelpers.Constant("My Notes"),
                            Keywords = StaticHelpers.Constant("My Keywords"),
                            Scope = StaticHelpers.Constant(SearchScope.EntireCase),
                            SearchFolderArtifactIds = StaticHelpers.Array(55, 66)
                        },
                        42
                    ).WithTestRelativitySettings()
                    .WithService(
                        new MockSetup<IKeywordSearchManager, int>(
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
