using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Flurl.Http;
using Sequence.Connectors.Relativity.ManagerInterfaces;
using Relativity.Kepler.Transport;
using Relativity.Environment.V1.Matter;
using Relativity.Environment.V1.Matter.Models;
using Relativity.Environment.V1.Workspace.Models;
using Relativity.Environment.V1.Workspace;
using Relativity.Services.Interfaces.Document;
using Relativity.Services.Interfaces.Document.Models;
using Relativity.Services.Folder;
#pragma warning disable CS1591

namespace Sequence.Connectors.Relativity.Managers
{
[GeneratedCode("CodeGenerator", "1")]
public class TemplateFolderManager1 : ManagerBase, IFolderManager1
{
	public TemplateFolderManager1(RelativitySettings relativitySettings, IFlurlClient flurlClient)
	:base(relativitySettings, flurlClient) { }
	
	/// <inheritdoc />
	protected override IEnumerable<string> Prefixes
	{
		get
		{
			yield return $"Relativity.Services.Folder.IFolderModule";
			yield return $"Folder Manager";
			yield break;
		}
	}
	
	
	public Task<Int32> CreateSingleAsync(Int32 workspaceArtifactID, Folder model)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"/CreateSingleAsync";
		var jsonObject = new {
			workspaceArtifactID,
			model,
		};
		return PostJsonAsync<Int32>(route, jsonObject, cancellationToken);
	}
	
	
	public Task UpdateSingleAsync(Int32 workspaceArtifactID, Folder model)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"/UpdateSingleAsync";
		var jsonObject = new {
			workspaceArtifactID,
			model,
		};
		return PostJsonAsync(route, jsonObject, cancellationToken);
	}
	
	
	public Task<FolderResultSet> DeleteUnusedFoldersAsync(Int32 workspaceArtifactID)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"/DeleteUnusedFoldersAsync";
		var jsonObject = new {
			workspaceArtifactID,
		};
		return PostJsonAsync<FolderResultSet>(route, jsonObject, cancellationToken);
	}
	
	
	public Task<List<Folder>> GetChildrenAsync(Int32 workspaceArtifactID, Int32 parentId)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"/GetChildrenAsync";
		var jsonObject = new {
			workspaceArtifactID,
			parentId,
		};
		return PostJsonAsync<List<Folder>>(route, jsonObject, cancellationToken);
	}
	
	
	public Task<FolderMoveResultSet> MoveFolderAsync(Int32 workspaceArtifactID, Int32 artifactId, Int32 destinationFolderID, CancellationToken cancel)
	{
		var cancellationToken = cancel;
		var route = $"/MoveFolderAsync";
		var jsonObject = new {
			workspaceArtifactID,
			artifactId,
			destinationFolderID,
		};
		return PostJsonAsync<FolderMoveResultSet>(route, jsonObject, cancellationToken);
	}
	
	
	public Task<Folder> GetWorkspaceRootAsync(Int32 workspaceArtifactID)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"/GetWorkspaceRootAsync";
		var jsonObject = new {
			workspaceArtifactID,
		};
		return PostJsonAsync<Folder>(route, jsonObject, cancellationToken);
	}
	
}


}
