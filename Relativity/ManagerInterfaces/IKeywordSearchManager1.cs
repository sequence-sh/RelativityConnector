using Relativity.Kepler.Services;
using Relativity.Services.Search;

namespace Reductech.Sequence.Connectors.Relativity.ManagerInterfaces;

/// <summary>
/// IdtSearchManager interface enables interaction with Relativity dtSearch.
/// </summary>
[WebService("Keyword Search Manager")]
[ServiceAudience(Audience.Public)]
public interface
    IKeywordSearchManager1 : IManager //Relativity.Services.Search.IKeywordSearchManager
{
    /// <summary>
    /// Asynchronously creates an instance of keyword saved search in a workspace.
    /// </summary>
    [HttpPost]
    [Route("CreateSingleAsync")]
    Task<int> CreateSingleAsync([JsonParameter] int workspaceArtifactID,[JsonParameter] KeywordSearch searchDTO);

    /// <summary>
    /// Asynchronously deletes an instance of keyword saved search.
    /// </summary>
    [HttpPost]
    [Route("DeleteSingleAsync")]
    Task DeleteSingleAsync([JsonParameter]int workspaceArtifactID,[JsonParameter] int searchArtifactID);

    /// <summary>
    /// Asynchronously returns an instance of keyword saved search as a KeywordSearch object.
    /// </summary>
    [HttpPost]
    [Route("ReadSingleAsync")]
    Task<KeywordSearch> ReadSingleAsync(
        [JsonParameter] int workspaceArtifactID,
        [JsonParameter]  int searchArtifactID);
}
