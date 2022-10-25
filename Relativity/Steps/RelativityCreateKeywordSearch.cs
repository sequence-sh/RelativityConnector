using System.Linq;
using Reductech.Sequence.Connectors.Relativity.ManagerInterfaces;
using Relativity.Services.Field;
using Relativity.Services.Folder;
using Relativity.Services.Search;
using CSharpFunctionalExtensions.ValueTasks;

namespace Reductech.Sequence.Connectors.Relativity.Steps;

/// <summary>
/// Creates a keyword search. Returns the artifact id of the created search.
/// </summary>
public class RelativityCreateKeywordSearch : RelativityApiRequest<(SCLInt workspaceID, KeywordSearch
    search),
    IKeywordSearchManager1, SCLInt, SCLInt>
{
    /// <inheritdoc />
    public override IStepFactory StepFactory { get; } =
        new SimpleStepFactory<RelativityCreateKeywordSearch, SCLInt>();

    /// <inheritdoc />
    public override Result<SCLInt, IErrorBuilder> ConvertOutput(SCLInt serviceOutput)
    {
        return serviceOutput;
    }

    /// <inheritdoc />
    public override async Task<SCLInt> SendRequest(
        IStateMonad stateMonad,
        IKeywordSearchManager1 service,
        (SCLInt workspaceID, KeywordSearch search) requestObject,
        CancellationToken cancellationToken)
    {
        var r = await service.CreateSingleAsync(requestObject.workspaceID, requestObject.search);
        return r.ConvertToSCLObject();
    }

    /// <inheritdoc />
    public override ValueTask<Result<(SCLInt workspaceID, KeywordSearch search), IError>> TryCreateRequest(
        IStateMonad stateMonad,
        CancellationToken cancellation)
    {
        return stateMonad.RunStepsAsync(
                Workspace.WrapArtifact(ArtifactType.Case, stateMonad, this),
                SearchName.WrapStringStream(),
                SearchText.WrapStringStream(),
                SortByRank,
                Fields.WrapOneOf(StepMaps.Array<SCLInt>(), StepMaps.Array(StepMaps.String())),
                Notes.WrapNullable(StepMaps.String()),
                Keywords.WrapNullable(StepMaps.String()),
                Scope,
                SearchFolderArtifactIds.WrapNullable(StepMaps.Array<SCLInt>()),
                cancellation
            )
            .Map(
                result =>
                {
                    var (workspaceId, name, searchText, sortByRank, fieldArtifactIds, notes,
                        keywords, scope,
                        searchFolderArtifactIds) = result;

                    var keywordSearch = new KeywordSearch()
                    {
                        Name           = name,
                        ArtifactTypeID = 10,
                        SearchText     = searchText,
                        SortByRank     = sortByRank,
                        Fields         = fieldArtifactIds.Match(l=> l.Select(x=> new FieldRef(x)),l=> l.Select(x=> new FieldRef(x)) ).ToList(),
                        Scope          = MapSearchScope(scope),
                    };

                    if (notes.HasValue)
                        keywordSearch.Notes = notes.Value;

                    if (keywords.HasValue)
                        keywordSearch.Keywords = keywords.Value;

                    if (searchFolderArtifactIds.HasValue)
                        keywordSearch.SearchFolders =
                            searchFolderArtifactIds.Value.Select(x => new FolderRef(x)).ToList();

                    return (workspaceId, keywordSearch);
                }
            );
    }

    /// <summary>
    /// The Workspace to search.
    /// You can provide either the Artifact Id or the name
    /// </summary>
    [StepProperty(1)]
    [Required]
    public IStep<SCLOneOf<SCLInt, StringStream>> Workspace { get; set; } = null!;

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
    public IStep<SCLBool> SortByRank { get; set; } = null!;

    /// <summary>
    /// The fields of the search
    /// You can provide either the ArtifactId or the name
    /// </summary>
    [StepProperty(5)]
    [Required] 
    public IStep<SCLOneOf<Array<SCLInt>, Array<StringStream>>> Fields { get; set; } = null!;

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
    public IStep<SCLEnum<SearchScope>> Scope { get; set; } =
        new SCLConstant<SCLEnum<SearchScope>>(new SCLEnum<SearchScope>(SearchScope.EntireCase));

    /// <summary>
    /// Artifact ids of the folders to search
    /// </summary>
    [StepProperty(9)]
    [DefaultValueExplanation("Do not specify any folders - for searching the entire case")]
    public IStep<Array<SCLInt>>? SearchFolderArtifactIds { get; set; } = null!;

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
