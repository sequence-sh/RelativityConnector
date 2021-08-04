
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Flurl.Http;
using Relativity.Kepler.Transport;
using Relativity.Environment.V1.Matter;
using Relativity.Environment.V1.Matter.Models;
using Relativity.Environment.V1.Workspace.Models;
using Relativity.Environment.V1.Workspace;
using Relativity.Environment.V1.Shared.Models;
using Relativity.Shared.V1.Models;

namespace Reductech.EDR.Connectors.Relativity.Managers
{
[GeneratedCode("CodeGenerator", "1")]
public class TemplateWorkspaceManager : ManagerBase, IWorkspaceManager
{
	public TemplateWorkspaceManager(RelativitySettings relativitySettings, IFlurlClient flurlClient)
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
		return PostJsonAsync<WorkspaceResponse>(route, new{workspaceRequest}, cancellationToken);
	}
	
	
	public Task<String> GetDefaultDownloadHandlerURLAsync()
	{
		var cancellationToken = CancellationToken.None;
		var route = $"workspace/default-download-handler-url";
		return GetJsonAsync<String>(route, cancellationToken);
	}
	
	
	public Task<WorkspaceResponse> CreateAsync(WorkspaceRequest workspaceRequest)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"workspace/";
		return PostJsonAsync<WorkspaceResponse>(route, new{workspaceRequest}, cancellationToken);
	}
	
	
	public Task<WorkspaceResponse> CreateAsync(WorkspaceRequest workspaceRequest, IProgress<ProgressReport> progress)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"workspace/";
		return PostJsonAsync<WorkspaceResponse>(route, new{workspaceRequest}, cancellationToken);
	}
	
	
	public Task<WorkspaceResponse> CreateAsync(WorkspaceRequest workspaceRequest, CancellationToken cancel, IProgress<ProgressReport> progress)
	{
		var cancellationToken = cancel;
		var route = $"workspace/";
		return PostJsonAsync<WorkspaceResponse>(route, new{workspaceRequest}, cancellationToken);
	}
	
	
	public Task<WorkspaceResponse> ReadAsync(Int32 workspaceID)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"workspace/{workspaceID}";
		return GetJsonAsync<WorkspaceResponse>(route, cancellationToken);
	}
	
	
	public Task<WorkspaceResponse> ReadAsync(Int32 workspaceID, Boolean includeMetadata, Boolean includeActions)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"workspace/{workspaceID}/{includeMetadata}/{includeActions}";
		return GetJsonAsync<WorkspaceResponse>(route, cancellationToken);
	}
	
	
	public Task<WorkspaceResponse> UpdateAsync(Int32 workspaceID, WorkspaceRequest workspaceRequest)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"workspace/{workspaceID}";
		return PutAsync<WorkspaceResponse>(route, new{workspaceRequest}, cancellationToken);
	}
	
	
	public Task<WorkspaceResponse> UpdateAsync(Int32 workspaceID, WorkspaceRequest workspaceRequest, DateTime lastModifiedOn)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"workspace/{workspaceID}";
		return PutAsync<WorkspaceResponse>(route, new{workspaceRequest}, cancellationToken);
	}
	
	
	public Task DeleteAsync(Int32 workspaceID)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"workspace/{workspaceID}";
		return DeleteAsync(route, cancellationToken);
	}
	
	
	public Task DeleteAsync(Int32 workspaceID, IProgress<ProgressReport> progress)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"workspace/{workspaceID}";
		return DeleteAsync(route, cancellationToken);
	}
	
	
	public Task DeleteAsync(Int32 workspaceID, CancellationToken cancel)
	{
		var cancellationToken = cancel;
		var route = $"workspace/{workspaceID}";
		return DeleteAsync(route, cancellationToken);
	}
	
	
	public Task DeleteAsync(Int32 workspaceID, CancellationToken cancel, IProgress<ProgressReport> progress)
	{
		var cancellationToken = cancel;
		var route = $"workspace/{workspaceID}";
		return DeleteAsync(route, cancellationToken);
	}
	
	
	public Task<Meta> GetMetaAsync()
	{
		var cancellationToken = CancellationToken.None;
		var route = $"workspace/meta";
		return GetJsonAsync<Meta>(route, cancellationToken);
	}
	
	
	public Task<QueryResultSlim> QueryWorkspaceByGroupAsync(QueryRequest request, Int32 start, Int32 length, Int32 groupID)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"workspace/query-by-group/{groupID}";
		return PostJsonAsync<QueryResultSlim>(route, new{request}, cancellationToken);
	}
	
	
	public Task<QueryResultSlim> QueryWorkspaceByGroupAsync(QueryRequest request, Int32 start, Int32 length, Int32 groupID, IProgress<ProgressReport> progress)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"workspace/query-by-group/{groupID}";
		return PostJsonAsync<QueryResultSlim>(route, new{request}, cancellationToken);
	}
	
	
	public Task<QueryResultSlim> QueryWorkspaceByGroupAsync(QueryRequest request, Int32 start, Int32 length, Int32 groupID, CancellationToken cancel)
	{
		var cancellationToken = cancel;
		var route = $"workspace/query-by-group/{groupID}";
		return PostJsonAsync<QueryResultSlim>(route, new{request}, cancellationToken);
	}
	
	
	public Task<QueryResultSlim> QueryWorkspaceByGroupAsync(QueryRequest request, Int32 start, Int32 length, Int32 groupID, CancellationToken cancel, IProgress<ProgressReport> progress)
	{
		var cancellationToken = cancel;
		var route = $"workspace/query-by-group/{groupID}";
		return PostJsonAsync<QueryResultSlim>(route, new{request}, cancellationToken);
	}
	
	
	public Task<List<DisplayableObjectIdentifier>> GetEligibleResourcePoolsAsync()
	{
		var cancellationToken = CancellationToken.None;
		var route = $"workspace/eligible-resource-pools";
		return GetJsonAsync<List<DisplayableObjectIdentifier>>(route, cancellationToken);
	}
	
	
	public Task<List<DisplayableObjectIdentifier>> GetEligibleFileRepositoriesAsync(Int32 resourcePoolID)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"workspace/eligible-resource-pools/{resourcePoolID}/eligible-file-repositories";
		return GetJsonAsync<List<DisplayableObjectIdentifier>>(route, cancellationToken);
	}
	
	
	public Task<List<DisplayableObjectIdentifier>> GetEligibleCacheLocationsAsync(Int32 resourcePoolID)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"workspace/eligible-resource-pools/{resourcePoolID}/eligible-cache-locations";
		return GetJsonAsync<List<DisplayableObjectIdentifier>>(route, cancellationToken);
	}
	
	
	public Task<List<DisplayableObjectIdentifier>> GetEligibleSqlServersAsync(Int32 resourcePoolID)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"workspace/eligible-resource-pools/{resourcePoolID}/eligible-sql-servers";
		return GetJsonAsync<List<DisplayableObjectIdentifier>>(route, cancellationToken);
	}
	
	
	public Task<List<DisplayableObjectIdentifier>> GetEligibleAzureCredentialsAsync(Int32 resourcePoolID)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"workspace/eligible-resource-pools/{resourcePoolID}/eligible-azure-credentials";
		return GetJsonAsync<List<DisplayableObjectIdentifier>>(route, cancellationToken);
	}
	
	
	public Task<List<DisplayableObjectIdentifier>> GetEligibleAzureFileSystemCredentialsAsync(Int32 resourcePoolID)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"workspace/eligible-resource-pools/{resourcePoolID}/eligible-azure-file-system-credentials";
		return GetJsonAsync<List<DisplayableObjectIdentifier>>(route, cancellationToken);
	}
	
	
	public Task<QueryResultSlim> QueryEligibleClientsAsync(QueryRequest request, Int32 start, Int32 length)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"workspace/query-eligible-clients";
		return PostJsonAsync<QueryResultSlim>(route, new{request}, cancellationToken);
	}
	
	
	public Task<QueryResultSlim> QueryEligibleClientsAsync(QueryRequest request, Int32 start, Int32 length, IProgress<ProgressReport> progress)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"workspace/query-eligible-clients";
		return PostJsonAsync<QueryResultSlim>(route, new{request}, cancellationToken);
	}
	
	
	public Task<QueryResultSlim> QueryEligibleClientsAsync(QueryRequest request, Int32 start, Int32 length, CancellationToken cancel)
	{
		var cancellationToken = cancel;
		var route = $"workspace/query-eligible-clients";
		return PostJsonAsync<QueryResultSlim>(route, new{request}, cancellationToken);
	}
	
	
	public Task<QueryResultSlim> QueryEligibleClientsAsync(QueryRequest request, Int32 start, Int32 length, CancellationToken cancel, IProgress<ProgressReport> progress)
	{
		var cancellationToken = cancel;
		var route = $"workspace/query-eligible-clients";
		return PostJsonAsync<QueryResultSlim>(route, new{request}, cancellationToken);
	}
	
	
	public Task<QueryResultSlim> QueryEligibleGroupsAsync(QueryRequest request, Int32 start, Int32 length)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"workspace/query-eligible-groups";
		return PostJsonAsync<QueryResultSlim>(route, new{request}, cancellationToken);
	}
	
	
	public Task<QueryResultSlim> QueryEligibleGroupsAsync(QueryRequest request, Int32 start, Int32 length, IProgress<ProgressReport> progress)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"workspace/query-eligible-groups";
		return PostJsonAsync<QueryResultSlim>(route, new{request}, cancellationToken);
	}
	
	
	public Task<QueryResultSlim> QueryEligibleGroupsAsync(QueryRequest request, Int32 start, Int32 length, CancellationToken cancel)
	{
		var cancellationToken = cancel;
		var route = $"workspace/query-eligible-groups";
		return PostJsonAsync<QueryResultSlim>(route, new{request}, cancellationToken);
	}
	
	
	public Task<QueryResultSlim> QueryEligibleGroupsAsync(QueryRequest request, Int32 start, Int32 length, CancellationToken cancel, IProgress<ProgressReport> progress)
	{
		var cancellationToken = cancel;
		var route = $"workspace/query-eligible-groups";
		return PostJsonAsync<QueryResultSlim>(route, new{request}, cancellationToken);
	}
	
	
	public Task<QueryResultSlim> QueryEligibleMattersAsync(QueryRequest request, Int32 start, Int32 length)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"workspace/query-eligible-matters";
		return PostJsonAsync<QueryResultSlim>(route, new{request}, cancellationToken);
	}
	
	
	public Task<QueryResultSlim> QueryEligibleMattersAsync(QueryRequest request, Int32 start, Int32 length, IProgress<ProgressReport> progress)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"workspace/query-eligible-matters";
		return PostJsonAsync<QueryResultSlim>(route, new{request}, cancellationToken);
	}
	
	
	public Task<QueryResultSlim> QueryEligibleMattersAsync(QueryRequest request, Int32 start, Int32 length, CancellationToken cancel)
	{
		var cancellationToken = cancel;
		var route = $"workspace/query-eligible-matters";
		return PostJsonAsync<QueryResultSlim>(route, new{request}, cancellationToken);
	}
	
	
	public Task<QueryResultSlim> QueryEligibleMattersAsync(QueryRequest request, Int32 start, Int32 length, CancellationToken cancel, IProgress<ProgressReport> progress)
	{
		var cancellationToken = cancel;
		var route = $"workspace/query-eligible-matters";
		return PostJsonAsync<QueryResultSlim>(route, new{request}, cancellationToken);
	}
	
	
	public Task<QueryResultSlim> QueryEligibleSavedSearchesAsync(QueryRequest request, Int32 start, Int32 length, Int32 workspaceID)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"workspace/{workspaceID}/query-eligible-saved-searches";
		return PostJsonAsync<QueryResultSlim>(route, new{request}, cancellationToken);
	}
	
	
	public Task<QueryResultSlim> QueryEligibleSavedSearchesAsync(QueryRequest request, Int32 start, Int32 length, Int32 workspaceID, IProgress<ProgressReport> progress)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"workspace/{workspaceID}/query-eligible-saved-searches";
		return PostJsonAsync<QueryResultSlim>(route, new{request}, cancellationToken);
	}
	
	
	public Task<QueryResultSlim> QueryEligibleSavedSearchesAsync(QueryRequest request, Int32 start, Int32 length, Int32 workspaceID, CancellationToken cancel)
	{
		var cancellationToken = cancel;
		var route = $"workspace/{workspaceID}/query-eligible-saved-searches";
		return PostJsonAsync<QueryResultSlim>(route, new{request}, cancellationToken);
	}
	
	
	public Task<QueryResultSlim> QueryEligibleSavedSearchesAsync(QueryRequest request, Int32 start, Int32 length, Int32 workspaceID, CancellationToken cancel, IProgress<ProgressReport> progress)
	{
		var cancellationToken = cancel;
		var route = $"workspace/{workspaceID}/query-eligible-saved-searches";
		return PostJsonAsync<QueryResultSlim>(route, new{request}, cancellationToken);
	}
	
	
	public Task<QueryResultSlim> QueryEligibleTemplatesAsync(QueryRequest request, Int32 start, Int32 length)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"workspace/query-eligible-templates";
		return PostJsonAsync<QueryResultSlim>(route, new{request}, cancellationToken);
	}
	
	
	public Task<QueryResultSlim> QueryEligibleTemplatesAsync(QueryRequest request, Int32 start, Int32 length, IProgress<ProgressReport> progress)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"workspace/query-eligible-templates";
		return PostJsonAsync<QueryResultSlim>(route, new{request}, cancellationToken);
	}
	
	
	public Task<QueryResultSlim> QueryEligibleTemplatesAsync(QueryRequest request, Int32 start, Int32 length, CancellationToken cancel)
	{
		var cancellationToken = cancel;
		var route = $"workspace/query-eligible-templates";
		return PostJsonAsync<QueryResultSlim>(route, new{request}, cancellationToken);
	}
	
	
	public Task<QueryResultSlim> QueryEligibleTemplatesAsync(QueryRequest request, Int32 start, Int32 length, CancellationToken cancel, IProgress<ProgressReport> progress)
	{
		var cancellationToken = cancel;
		var route = $"workspace/query-eligible-templates";
		return PostJsonAsync<QueryResultSlim>(route, new{request}, cancellationToken);
	}
	
	
	public Task<SqlFullTextLanguageOptions> GetEligibleSqlFullTextLanguagesAsync()
	{
		var cancellationToken = CancellationToken.None;
		var route = $"workspace/eligible-sql-full-text-languages";
		return GetJsonAsync<SqlFullTextLanguageOptions>(route, cancellationToken);
	}
	
	
	public Task<List<DisplayableObjectIdentifier>> GetEligibleStatusesAsync()
	{
		var cancellationToken = CancellationToken.None;
		var route = $"workspace/eligible-statuses";
		return GetJsonAsync<List<DisplayableObjectIdentifier>>(route, cancellationToken);
	}
	
	
	public Task RetryFailedCreateEventHandlersAsync(Int32 workspaceID)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"workspace/{workspaceID}/retry-failed-create-event-handlers";
		return PostJsonAsync(route, new{workspaceID}, cancellationToken);
	}
	
	
	public Task<WorkspaceSummary> GetWorkspaceSummaryAsync(Int32 workspaceID)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"workspace/{workspaceID}/summary";
		return GetJsonAsync<WorkspaceSummary>(route, cancellationToken);
	}
	
	
	public Task MoveToColdStorageAsync(Int32 workspaceID)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"workspace/{workspaceID}/move-to-cold-storage";
		return PostJsonAsync(route, new{workspaceID}, cancellationToken);
	}
	
}


}
