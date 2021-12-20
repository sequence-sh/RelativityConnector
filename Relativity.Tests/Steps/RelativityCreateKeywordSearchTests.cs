using Moq;
using Reductech.Sequence.Connectors.Relativity.ManagerInterfaces;
using Relativity.Services.Search;

namespace Reductech.Sequence.Connectors.Relativity.Tests.Steps;

public partial class
    RelativityCreateKeywordSearchTests : StepTestBase<RelativityCreateKeywordSearch, SCLInt>
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
                            Workspace  = new OneOfStep<SCLInt, StringStream>(Constant(10)),
                            SearchName = Constant("My Search"),
                            SearchText = Constant("Search Text"),
                            SortByRank = Constant(true),
                            Fields =
                                new OneOfStep<Array<SCLInt>, Array<StringStream>>(Array(22, 33, 44)),
                            Notes                   = Constant("My Notes"),
                            Keywords                = Constant("My Keywords"),
                            Scope                   = Constant(SearchScope.EntireCase),
                            SearchFolderArtifactIds = Array(55, 66)
                        },
                        42.ConvertToSCLObject()
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
