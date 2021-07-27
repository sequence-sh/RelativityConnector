using System.Collections.Generic;
using System.Linq;
using Moq;
using Reductech.EDR.Connectors.Relativity.Steps;
using Reductech.EDR.Core;
using Reductech.EDR.Core.TestHarness;
using Reductech.EDR.Core.Util;
using Relativity.Services.Folder;
using Relativity.Services.Search;
using static Reductech.EDR.Core.TestHarness.StaticHelpers;

namespace Reductech.EDR.Connectors.Relativity.Tests.Steps
{
    public partial class RelativityReadKeywordSearchTests : StepTestBase<RelativityReadKeywordSearch, Entity>
    {

        /// <inheritdoc />
        protected override IEnumerable<StepCase> StepCases
        {
            get
            {
                yield return new StepCase("Read Search",
                        TestHelpers.LogEntity(new RelativityReadKeywordSearch()
                        {
                            WorkspaceId = Constant(11),
                            SearchId = Constant(12)
                        }), Unit.Default,
                        "(ArtifactTypeID: 0 SearchContainer: (ArtifactID: 0 Name: \"\") Owner: (ArtifactID: 0 Name: \"\") SearchIndex: (ArtifactID: 0 Name: \"\") Includes: \"\" Scope: 0 SearchFolders: \"\" RequiresManualRun: False SearchCriteria: (Conditions: \"\" BooleanOperator: 0) Fields: \"\" Sorts: \"\" QueryHint: \"\" SortByRank: False SearchText: \"\" Keywords: \"\" Notes: \"\" RelativityApplications: \"\" SystemCreatedBy: \"\" SystemCreatedOn: \"\" SystemLastModifiedBy: \"\" SystemLastModifiedOn: \"\" Dashboard: \"\" ArtifactID: 12 Name: \"My Search\" SearchType: \"KeywordSearch\")"

                    )
                    .WithTestRelativitySettings()
                    .WithService(new MockSetup<IKeywordSearchManager, KeywordSearch>(
                        x => x.ReadSingleAsync(11, 12),
                        new KeywordSearch()
                        {
                            Name = "My Search",
                            ArtifactID = 12,
                        }

                    ));
            }
        }

    }

    public partial class RelativityCreateKeywordSearchTests : StepTestBase<RelativityCreateKeywordSearch, int>
    {
        /// <inheritdoc />
        protected override IEnumerable<StepCase> StepCases
        {
            get
            {
                yield return new StepCase("Create Keyword",
                            new RelativityCreateKeywordSearch()
                            {
                                WorkspaceId = Constant(10),
                                SearchName = Constant("My Search"),
                                SearchText = Constant("Search Text"),
                                SortByRank = Constant(true),
                                FieldArtifactIds = Array(22, 33, 44),
                                Notes = Constant("My Notes"),
                                Keywords = Constant("My Keywords"),
                                Scope = Constant(SearchScope.EntireCase),
                                SearchFolderArtifactIds = Array(55, 66)
                            },
                            42
                        ).WithTestRelativitySettings()
                        .WithService(new MockSetup<IKeywordSearchManager, int>(
                            x =>
                                x.CreateSingleAsync(10, It.Is<KeywordSearch>(
                                    ks =>
                                        ks.Name == "My Search" &&
                                        ks.ArtifactTypeID == 10 &&
                                        ks.SearchText == "Search Text" &&
                                        ks.SortByRank == true &&
                                        ks.Fields.Select(f => f.ArtifactID).SequenceEqual(new[] { 22, 33, 44 }) &&
                                        ks.Notes == "My Notes" &&
                                        ks.Keywords == "My Keywords" &&
                                        ks.Scope == ScopeType.EntireCase &&
                                        ks.SearchFolders.Select(f => f.ArtifactID).SequenceEqual(new[] { 55, 66 })
                                ))
                            , 42
                        ))
                    ;
            }
        }
    }


    public partial class RelativityDeleteKeywordSearchTests : StepTestBase<RelativityDeleteKeywordSearch, Unit>
    {
        /// <inheritdoc />
        protected override IEnumerable<StepCase> StepCases
        {
            get
            {
                yield return new StepCase("Delete Keyword Search",
                        new RelativityDeleteKeywordSearch()
                        {
                            WorkspaceId = Constant(123),
                            SearchId = Constant(456),
                        }, Unit.Default
                    ).WithTestRelativitySettings()
                    .WithService(new MockSetupUnit<IKeywordSearchManager>(
                        x => x.DeleteSingleAsync(123, 456)
                    ));
            }
        }
    }


    public partial class RelativityCreateFolderTests : StepTestBase<RelativityCreateFolder, int>
    {
        /// <inheritdoc />
        protected override IEnumerable<StepCase> StepCases
        {
            get
            {
                yield return new StepCase(
                        "Create a folder with a parent folder",
                        new RelativityCreateFolder()
                        {
                            FolderName = Constant("MyNewFolder"),
                            ParentFolderId = Constant(14),
                            WorkspaceArtifactId = Constant(13)
                        },
                        42
                    ).WithTestRelativitySettings()
                    .WithService(new MockSetup<IFolderManager, int>(x => x.CreateSingleAsync(13,
                            It.Is<Folder>(
                                folder => folder.Name == "MyNewFolder" && folder.ParentFolder.ArtifactID == 14)),
                        42));
            }
        }
    }
}