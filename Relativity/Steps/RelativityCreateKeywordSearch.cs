using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Reductech.EDR.Core;
using Reductech.EDR.Core.Attributes;
using Reductech.EDR.Core.Internal;
using Reductech.EDR.Core.Internal.Errors;
using Reductech.EDR.Core.Util;
using Relativity.Services.Field;
using Relativity.Services.Folder;
using Relativity.Services.Search;

namespace Reductech.EDR.Connectors.Relativity.Steps
{
    /// <summary>
    /// Creates a keyword search. Returns the artifact id of the created search.
    /// </summary>
    public class RelativityCreateKeywordSearch : RelativityApiRequest<(int workspaceID, KeywordSearch search),
        IKeywordSearchManager, int, int>
    {
        /// <inheritdoc />
        public override IStepFactory StepFactory { get; } = new SimpleStepFactory<RelativityCreateKeywordSearch, int>();

        /// <inheritdoc />
        public override Result<int, IErrorBuilder> ConvertOutput(int serviceOutput)
        {
            return serviceOutput;
        }

        /// <inheritdoc />
        public override Task<int> SendRequest(IStateMonad stateMonad, IKeywordSearchManager service,
            (int workspaceID, KeywordSearch search) requestObject, CancellationToken cancellationToken)
        {
            return service.CreateSingleAsync(requestObject.workspaceID, requestObject.search);
        }

        /// <inheritdoc />
        public override Task<Result<(int workspaceID, KeywordSearch search), IError>> TryCreateRequest(
            IStateMonad stateMonad, CancellationToken cancellation)
        {
            return stateMonad.RunStepsAsync(
                    WorkspaceId,
                    SearchName.WrapStringStream(),
                    SearchText.WrapStringStream(),
                    SortByRank,
                    FieldArtifactIds.WrapArray(),
                    Notes.WrapNullable(x => x.WrapStringStream()),
                    Keywords.WrapNullable(x => x.WrapStringStream()),
                    Scope,
                    SearchFolderArtifactIds.WrapNullable(x => x.WrapArray()),
                    cancellation
                )
                .Map(result =>
                {
                    var (workspaceId, name, searchText, sortByRank, fieldArtifactIds, notes, keywords, scope,
                        searchFolderArtifactIds) = result;


                    var keywordSearch = new KeywordSearch()
                    {
                        Name = name,
                        ArtifactTypeID = 10,
                        SearchText = searchText,
                        SortByRank = sortByRank,
                        Fields = fieldArtifactIds.Select(x => new FieldRef(x)).ToList(),
                        Scope = MapSearchScope(scope),
                    };

                    if (notes.HasValue) keywordSearch.Notes = notes.Value;
                    if (keywords.HasValue) keywordSearch.Keywords = keywords.Value;
                    if (searchFolderArtifactIds.HasValue)
                        keywordSearch.SearchFolders =
                            searchFolderArtifactIds.Value.Select(x => new FolderRef(x)).ToList();

                    return (workspaceId, keywordSearch);
                });
        }

        [StepProperty(1)] [Required] public IStep<int> WorkspaceId { get; set; } = null!;

        /// <summary>
        /// The name of the new search
        /// </summary>
        [StepProperty(2)]
        [Required]
        public IStep<StringStream> SearchName { get; set; } = null!;

        /// <summary>
        /// Search terms
        /// </summary>
        [StepProperty(3)]
        [Required]
        public IStep<StringStream> SearchText { get; set; } = null!;

        /// <summary>
        /// Indicates that the search results must be sorted by rank.
        /// </summary>
        [StepProperty(4)]
        [Required]
        public IStep<bool> SortByRank { get; set; } = null!;

        [StepProperty(5)] [Required] public IStep<Array<int>> FieldArtifactIds { get; set; } = null!;

        [StepProperty(6)]
        [DefaultValueExplanation("")]
        public IStep<StringStream>? Notes { get; set; } = null!;


        /// <summary>
        /// An optional field where extra group information may be recorded.
        /// </summary>
        [StepProperty(7)]
        [DefaultValueExplanation("")]
        public IStep<StringStream>? Keywords { get; set; } = null;


        /// <summary>
        /// The scope of the search
        /// </summary>
        [StepProperty(8)]
        [DefaultValueExplanation(nameof(SearchScope.EntireCase))]
        public IStep<SearchScope> Scope { get; set; } = new EnumConstant<SearchScope>(SearchScope.EntireCase);

        /// <summary>
        /// Artifact ids of the folders to search
        /// </summary>
        [StepProperty(9)]
        [DefaultValueExplanation("Do not specify any folders - for searching the entire case")]
        public IStep<Array<int>>? SearchFolderArtifactIds { get; set; } = null!;

        private static ScopeType MapSearchScope(SearchScope searchScope)
        {
            return searchScope switch
            {
                SearchScope.EntireCase => ScopeType.EntireCase,
                SearchScope.Folders => ScopeType.Folders,
                SearchScope.Subfolders => ScopeType.Subfolders,
                _ => throw new ArgumentOutOfRangeException(nameof(searchScope), searchScope, null)
            };
        }
    }
}