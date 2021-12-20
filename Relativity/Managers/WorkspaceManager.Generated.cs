
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Flurl.Http;
using Reductech.Sequence.Connectors.Relativity.ManagerInterfaces;
using Relativity.Kepler.Transport;
using Relativity.Environment.V1.Matter;
using Relativity.Environment.V1.Matter.Models;
using Relativity.Environment.V1.Workspace.Models;
using Relativity.Environment.V1.Workspace;
using Relativity.Environment.V1.Shared.Models;
using Relativity.Shared.V1.Models;

namespace Reductech.Sequence.Connectors.Relativity.Managers
{
[GeneratedCode("CodeGenerator", "1")]
public class TemplateWorkspaceManager1 : ManagerBase, IWorkspaceManager1
{
	public TemplateWorkspaceManager1(RelativitySettings relativitySettings, IFlurlClient flurlClient)
	:base(relativitySettings, flurlClient) { }
	
	/// <inheritdoc />
	protected override IEnumerable<string> Prefixes
	{
		get
		{
			yield return $"relativity-environment";
			yield return $"v{RelativitySettings.APIVersionNumber}";
			yield break;
		}
	}
	
	
	public Task<WorkspaceResponse> CreateAsync(WorkspaceRequest workspaceRequest, CancellationToken cancel)
	{
		var cancellationToken = cancel;
		var route = $"workspace/";
		var jsonObject = new {
			workspaceRequest,
		};
		return PostJsonAsync<WorkspaceResponse>(route, jsonObject, cancellationToken);
	}
	
	
	public Task<String> GetDefaultDownloadHandlerURLAsync()
	{
		var cancellationToken = CancellationToken.None;
		var route = $"workspace/default-download-handler-url";
		return GetJsonAsync<String>(route, cancellationToken);
	}
	
	
	public Task DeleteAsync(Int32 workspaceID, CancellationToken cancel)
	{
		var cancellationToken = cancel;
		var route = $"workspace/{workspaceID}";
		return DeleteAsync(route, cancellationToken);
	}
	
	
	public Task<WorkspaceResponse> ReadAsync(Int32 workspaceID, Boolean includeMetadata, Boolean includeActions)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"workspace/{workspaceID}/{includeMetadata}/{includeActions}";
		return GetJsonAsync<WorkspaceResponse>(route, cancellationToken);
	}
	
	
	public Task<WorkspaceSummary> GetWorkspaceSummaryAsync(Int32 workspaceID)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"workspace/{workspaceID}/summary";
		return GetJsonAsync<WorkspaceSummary>(route, cancellationToken);
	}
	
}


}
