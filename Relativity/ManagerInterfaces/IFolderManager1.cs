using Relativity.Kepler.Services;
using Relativity.Services.Folder;

namespace Reductech.Sequence.Connectors.Relativity.ManagerInterfaces;

[WebService("Folder Manager")]
[ServiceAudience(Audience.Public)]
public interface IFolderManager1 : IManager //Relativity.Services.Folder.IFolderManager
{
    /// <summary>Asynchronously creates an instance of Folder.</summary>
    /// <returns>ArtifactID of the created Folder.</returns>
    [HttpPost]
    [Route("CreateSingleAsync")]
    Task<int> CreateSingleAsync( [JsonParameter] int workspaceArtifactID, [JsonParameter]  Folder model);


    /// <summary>Asynchronously updates an instance of Folder.</summary>
    [HttpPost]
    [Route("UpdateSingleAsync")]
    Task UpdateSingleAsync([JsonParameter] int workspaceArtifactID, [JsonParameter]  Folder model);

    /// <summary>
    /// Asynchronously deletes any folders in the workspace which do not contain any documents or sub-folders with documents.
    /// </summary>
    /// <param name="workspaceArtifactID">ArtifactID of the workspace</param>
    /// <returns>FolderResultSet of the folders which were deleted.</returns>
    [HttpPost]
    [Route("DeleteUnusedFoldersAsync")]
    Task<FolderResultSet> DeleteUnusedFoldersAsync([JsonParameter] int workspaceArtifactID);

    /// <summary>Returns all child elements of the specified Folder</summary>
    /// <returns>List of child Folders</returns>
    [HttpPost]
    [Route("GetChildrenAsync")]
    Task<List<Folder>> GetChildrenAsync(
        [JsonParameter] int workspaceArtifactID,[JsonParameter] int parentId);

    /// <summary>
    /// Moves an existing folder and its children, including subfolders and documents.
    /// </summary>
    /// <returns>A FolderMoveResultSet object containing status information about the folder being moved.</returns>
    [HttpPost]
    [Route("MoveFolderAsync")]
    Task<FolderMoveResultSet> MoveFolderAsync(
        [JsonParameter]
        int workspaceArtifactID, [JsonParameter] int artifactId, [JsonParameter] int destinationFolderID,
        CancellationToken cancel);

    /// <summary>Retrieves a workplace root folder</summary>
    /// <param name="workspaceArtifactID">ArtifactID of the workspace</param>
    /// <returns>Folder</returns>
    [HttpPost]
    [Route("GetWorkspaceRootAsync")]
    Task<Folder> GetWorkspaceRootAsync([JsonParameter]int workspaceArtifactID);

    
}
