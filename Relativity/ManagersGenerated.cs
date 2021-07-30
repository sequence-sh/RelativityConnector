
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Flurl.Http;
using Relativity.Environment.V1.Matter;
using Relativity.Environment.V1.Matter.Models;
using Relativity.Environment.V1.Shared.Models;
using Relativity.Shared.V1.Models;
using Relativity.Environment.V1.Workspace.Models;
using Relativity.Environment.V1.Workspace;

namespace Reductech.EDR.Connectors.Relativity
{
[GeneratedCode("CodeGenerator", "1")]
public class TemplateMatterManager : ManagerBase, IMatterManager
{
	public TemplateMatterManager(RelativitySettings relativitySettings, IFlurlClient flurlClient)
	:base(relativitySettings, flurlClient) { }
	public override string RoutePrefix => "workspaces/-1/matters";
	public Task<Int32> CreateAsync(MatterRequest matterRequest)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"";
		return PostJsonAsync<Int32>(route, new{matterRequest}, cancellationToken);
	}
	public Task<MatterResponse> ReadAsync(Int32 matterID)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"{matterID:int}";
		return GetJsonAsync<MatterResponse>(route, cancellationToken);
	}
	public Task<MatterResponse> ReadAsync(Int32 matterID, Boolean includeMetadata, Boolean includeActions)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"{matterID:int}/{includeMetadata:bool}/{includeActions:bool}";
		return GetJsonAsync<MatterResponse>(route, cancellationToken);
	}
	public Task UpdateAsync(Int32 matterID, MatterRequest matterRequest)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"{matterID:int}";
		return PutAsync(route, new{matterRequest}, cancellationToken);
	}
	public Task UpdateAsync(Int32 matterID, MatterRequest matterRequest, DateTime lastModifiedOn)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"{matterID:int}";
		return PutAsync(route, new{matterRequest}, cancellationToken);
	}
	public Task DeleteAsync(Int32 matterID)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"{matterID:int}";
		return DeleteAsync(route, cancellationToken);
	}
	public Task<List<DisplayableObjectIdentifier>> GetEligibleClientsAsync()
	{
		var cancellationToken = CancellationToken.None;
		var route = $"~/workspaces/-1/eligible-clients";
		return GetJsonAsync<List<DisplayableObjectIdentifier>>(route, cancellationToken);
	}
	public Task<List<DisplayableObjectIdentifier>> GetEligibleStatusesAsync()
	{
		var cancellationToken = CancellationToken.None;
		var route = $"eligible-statuses";
		return GetJsonAsync<List<DisplayableObjectIdentifier>>(route, cancellationToken);
	}
}


[GeneratedCode("CodeGenerator", "1")]
public class TemplateWorkspaceManager : ManagerBase, IWorkspaceManager
{
	public TemplateWorkspaceManager(RelativitySettings relativitySettings, IFlurlClient flurlClient)
	:base(relativitySettings, flurlClient) { }
	public override string RoutePrefix => "workspace";
	public Task<WorkspaceResponse> CreateAsync(WorkspaceRequest workspaceRequest, CancellationToken cancel)
	{
		var cancellationToken = cancel;
		var route = $"";
		return PostJsonAsync<WorkspaceResponse>(route, new{workspaceRequest}, cancellationToken);
	}
	public Task<String> GetDefaultDownloadHandlerURLAsync()
	{
		var cancellationToken = CancellationToken.None;
		var route = $"default-download-handler-url";
		return GetJsonAsync<String>(route, cancellationToken);
	}
	public Task<WorkspaceResponse> CreateAsync(WorkspaceRequest workspaceRequest)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"";
		return PostJsonAsync<WorkspaceResponse>(route, new{workspaceRequest}, cancellationToken);
	}
	public Task<WorkspaceResponse> CreateAsync(WorkspaceRequest workspaceRequest, IProgress<ProgressReport> progress)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"";
		return PostJsonAsync<WorkspaceResponse>(route, new{workspaceRequest}, cancellationToken);
	}
	public Task<WorkspaceResponse> CreateAsync(WorkspaceRequest workspaceRequest, CancellationToken cancel, IProgress<ProgressReport> progress)
	{
		var cancellationToken = cancel;
		var route = $"";
		return PostJsonAsync<WorkspaceResponse>(route, new{workspaceRequest}, cancellationToken);
	}
	public Task<WorkspaceResponse> ReadAsync(Int32 workspaceID)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"{workspaceID:int}";
		return GetJsonAsync<WorkspaceResponse>(route, cancellationToken);
	}
	public Task<WorkspaceResponse> ReadAsync(Int32 workspaceID, Boolean includeMetadata, Boolean includeActions)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"{workspaceID:int}/{includeMetadata:bool}/{includeActions:bool}";
		return GetJsonAsync<WorkspaceResponse>(route, cancellationToken);
	}
	public Task<WorkspaceResponse> UpdateAsync(Int32 workspaceID, WorkspaceRequest workspaceRequest)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"{workspaceID:int}";
		return PutAsync<WorkspaceResponse>(route, new{workspaceRequest}, cancellationToken);
	}
	public Task<WorkspaceResponse> UpdateAsync(Int32 workspaceID, WorkspaceRequest workspaceRequest, DateTime lastModifiedOn)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"{workspaceID:int}";
		return PutAsync<WorkspaceResponse>(route, new{workspaceRequest}, cancellationToken);
	}
	public Task DeleteAsync(Int32 workspaceID)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"{workspaceID:int}";
		return DeleteAsync(route, cancellationToken);
	}
	public Task DeleteAsync(Int32 workspaceID, IProgress<ProgressReport> progress)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"{workspaceID:int}";
		return DeleteAsync(route, cancellationToken);
	}
	public Task DeleteAsync(Int32 workspaceID, CancellationToken cancel)
	{
		var cancellationToken = cancel;
		var route = $"{workspaceID:int}";
		return DeleteAsync(route, cancellationToken);
	}
	public Task DeleteAsync(Int32 workspaceID, CancellationToken cancel, IProgress<ProgressReport> progress)
	{
		var cancellationToken = cancel;
		var route = $"{workspaceID:int}";
		return DeleteAsync(route, cancellationToken);
	}
	public Task<Meta> GetMetaAsync()
	{
		var cancellationToken = CancellationToken.None;
		var route = $"meta";
		return GetJsonAsync<Meta>(route, cancellationToken);
	}
	public Task<QueryResultSlim> QueryWorkspaceByGroupAsync(QueryRequest request, Int32 start, Int32 length, Int32 groupID)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"query-by-group/{groupID:int}";
		return PostJsonAsync<QueryResultSlim>(route, new{request}, cancellationToken);
	}
	public Task<QueryResultSlim> QueryWorkspaceByGroupAsync(QueryRequest request, Int32 start, Int32 length, Int32 groupID, IProgress<ProgressReport> progress)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"query-by-group/{groupID:int}";
		return PostJsonAsync<QueryResultSlim>(route, new{request}, cancellationToken);
	}
	public Task<QueryResultSlim> QueryWorkspaceByGroupAsync(QueryRequest request, Int32 start, Int32 length, Int32 groupID, CancellationToken cancel)
	{
		var cancellationToken = cancel;
		var route = $"query-by-group/{groupID:int}";
		return PostJsonAsync<QueryResultSlim>(route, new{request}, cancellationToken);
	}
	public Task<QueryResultSlim> QueryWorkspaceByGroupAsync(QueryRequest request, Int32 start, Int32 length, Int32 groupID, CancellationToken cancel, IProgress<ProgressReport> progress)
	{
		var cancellationToken = cancel;
		var route = $"query-by-group/{groupID:int}";
		return PostJsonAsync<QueryResultSlim>(route, new{request}, cancellationToken);
	}
	public Task<List<DisplayableObjectIdentifier>> GetEligibleResourcePoolsAsync()
	{
		var cancellationToken = CancellationToken.None;
		var route = $"eligible-resource-pools";
		return GetJsonAsync<List<DisplayableObjectIdentifier>>(route, cancellationToken);
	}
	public Task<List<DisplayableObjectIdentifier>> GetEligibleFileRepositoriesAsync(Int32 resourcePoolID)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"eligible-resource-pools/{resourcePoolID:int}/eligible-file-repositories";
		return GetJsonAsync<List<DisplayableObjectIdentifier>>(route, cancellationToken);
	}
	public Task<List<DisplayableObjectIdentifier>> GetEligibleCacheLocationsAsync(Int32 resourcePoolID)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"eligible-resource-pools/{resourcePoolID:int}/eligible-cache-locations";
		return GetJsonAsync<List<DisplayableObjectIdentifier>>(route, cancellationToken);
	}
	public Task<List<DisplayableObjectIdentifier>> GetEligibleSqlServersAsync(Int32 resourcePoolID)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"eligible-resource-pools/{resourcePoolID:int}/eligible-sql-servers";
		return GetJsonAsync<List<DisplayableObjectIdentifier>>(route, cancellationToken);
	}
	public Task<List<DisplayableObjectIdentifier>> GetEligibleAzureCredentialsAsync(Int32 resourcePoolID)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"eligible-resource-pools/{resourcePoolID:int}/eligible-azure-credentials";
		return GetJsonAsync<List<DisplayableObjectIdentifier>>(route, cancellationToken);
	}
	public Task<List<DisplayableObjectIdentifier>> GetEligibleAzureFileSystemCredentialsAsync(Int32 resourcePoolID)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"eligible-resource-pools/{resourcePoolID:int}/eligible-azure-file-system-credentials";
		return GetJsonAsync<List<DisplayableObjectIdentifier>>(route, cancellationToken);
	}
	public Task<QueryResultSlim> QueryEligibleClientsAsync(QueryRequest request, Int32 start, Int32 length)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"query-eligible-clients";
		return PostJsonAsync<QueryResultSlim>(route, new{request}, cancellationToken);
	}
	public Task<QueryResultSlim> QueryEligibleClientsAsync(QueryRequest request, Int32 start, Int32 length, IProgress<ProgressReport> progress)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"query-eligible-clients";
		return PostJsonAsync<QueryResultSlim>(route, new{request}, cancellationToken);
	}
	public Task<QueryResultSlim> QueryEligibleClientsAsync(QueryRequest request, Int32 start, Int32 length, CancellationToken cancel)
	{
		var cancellationToken = cancel;
		var route = $"query-eligible-clients";
		return PostJsonAsync<QueryResultSlim>(route, new{request}, cancellationToken);
	}
	public Task<QueryResultSlim> QueryEligibleClientsAsync(QueryRequest request, Int32 start, Int32 length, CancellationToken cancel, IProgress<ProgressReport> progress)
	{
		var cancellationToken = cancel;
		var route = $"query-eligible-clients";
		return PostJsonAsync<QueryResultSlim>(route, new{request}, cancellationToken);
	}
	public Task<QueryResultSlim> QueryEligibleGroupsAsync(QueryRequest request, Int32 start, Int32 length)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"query-eligible-groups";
		return PostJsonAsync<QueryResultSlim>(route, new{request}, cancellationToken);
	}
	public Task<QueryResultSlim> QueryEligibleGroupsAsync(QueryRequest request, Int32 start, Int32 length, IProgress<ProgressReport> progress)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"query-eligible-groups";
		return PostJsonAsync<QueryResultSlim>(route, new{request}, cancellationToken);
	}
	public Task<QueryResultSlim> QueryEligibleGroupsAsync(QueryRequest request, Int32 start, Int32 length, CancellationToken cancel)
	{
		var cancellationToken = cancel;
		var route = $"query-eligible-groups";
		return PostJsonAsync<QueryResultSlim>(route, new{request}, cancellationToken);
	}
	public Task<QueryResultSlim> QueryEligibleGroupsAsync(QueryRequest request, Int32 start, Int32 length, CancellationToken cancel, IProgress<ProgressReport> progress)
	{
		var cancellationToken = cancel;
		var route = $"query-eligible-groups";
		return PostJsonAsync<QueryResultSlim>(route, new{request}, cancellationToken);
	}
	public Task<QueryResultSlim> QueryEligibleMattersAsync(QueryRequest request, Int32 start, Int32 length)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"query-eligible-matters";
		return PostJsonAsync<QueryResultSlim>(route, new{request}, cancellationToken);
	}
	public Task<QueryResultSlim> QueryEligibleMattersAsync(QueryRequest request, Int32 start, Int32 length, IProgress<ProgressReport> progress)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"query-eligible-matters";
		return PostJsonAsync<QueryResultSlim>(route, new{request}, cancellationToken);
	}
	public Task<QueryResultSlim> QueryEligibleMattersAsync(QueryRequest request, Int32 start, Int32 length, CancellationToken cancel)
	{
		var cancellationToken = cancel;
		var route = $"query-eligible-matters";
		return PostJsonAsync<QueryResultSlim>(route, new{request}, cancellationToken);
	}
	public Task<QueryResultSlim> QueryEligibleMattersAsync(QueryRequest request, Int32 start, Int32 length, CancellationToken cancel, IProgress<ProgressReport> progress)
	{
		var cancellationToken = cancel;
		var route = $"query-eligible-matters";
		return PostJsonAsync<QueryResultSlim>(route, new{request}, cancellationToken);
	}
	public Task<QueryResultSlim> QueryEligibleSavedSearchesAsync(QueryRequest request, Int32 start, Int32 length, Int32 workspaceID)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"{workspaceID:int}/query-eligible-saved-searches";
		return PostJsonAsync<QueryResultSlim>(route, new{request}, cancellationToken);
	}
	public Task<QueryResultSlim> QueryEligibleSavedSearchesAsync(QueryRequest request, Int32 start, Int32 length, Int32 workspaceID, IProgress<ProgressReport> progress)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"{workspaceID:int}/query-eligible-saved-searches";
		return PostJsonAsync<QueryResultSlim>(route, new{request}, cancellationToken);
	}
	public Task<QueryResultSlim> QueryEligibleSavedSearchesAsync(QueryRequest request, Int32 start, Int32 length, Int32 workspaceID, CancellationToken cancel)
	{
		var cancellationToken = cancel;
		var route = $"{workspaceID:int}/query-eligible-saved-searches";
		return PostJsonAsync<QueryResultSlim>(route, new{request}, cancellationToken);
	}
	public Task<QueryResultSlim> QueryEligibleSavedSearchesAsync(QueryRequest request, Int32 start, Int32 length, Int32 workspaceID, CancellationToken cancel, IProgress<ProgressReport> progress)
	{
		var cancellationToken = cancel;
		var route = $"{workspaceID:int}/query-eligible-saved-searches";
		return PostJsonAsync<QueryResultSlim>(route, new{request}, cancellationToken);
	}
	public Task<QueryResultSlim> QueryEligibleTemplatesAsync(QueryRequest request, Int32 start, Int32 length)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"query-eligible-templates";
		return PostJsonAsync<QueryResultSlim>(route, new{request}, cancellationToken);
	}
	public Task<QueryResultSlim> QueryEligibleTemplatesAsync(QueryRequest request, Int32 start, Int32 length, IProgress<ProgressReport> progress)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"query-eligible-templates";
		return PostJsonAsync<QueryResultSlim>(route, new{request}, cancellationToken);
	}
	public Task<QueryResultSlim> QueryEligibleTemplatesAsync(QueryRequest request, Int32 start, Int32 length, CancellationToken cancel)
	{
		var cancellationToken = cancel;
		var route = $"query-eligible-templates";
		return PostJsonAsync<QueryResultSlim>(route, new{request}, cancellationToken);
	}
	public Task<QueryResultSlim> QueryEligibleTemplatesAsync(QueryRequest request, Int32 start, Int32 length, CancellationToken cancel, IProgress<ProgressReport> progress)
	{
		var cancellationToken = cancel;
		var route = $"query-eligible-templates";
		return PostJsonAsync<QueryResultSlim>(route, new{request}, cancellationToken);
	}
	public Task<SqlFullTextLanguageOptions> GetEligibleSqlFullTextLanguagesAsync()
	{
		var cancellationToken = CancellationToken.None;
		var route = $"eligible-sql-full-text-languages";
		return GetJsonAsync<SqlFullTextLanguageOptions>(route, cancellationToken);
	}
	public Task<List<DisplayableObjectIdentifier>> GetEligibleStatusesAsync()
	{
		var cancellationToken = CancellationToken.None;
		var route = $"eligible-statuses";
		return GetJsonAsync<List<DisplayableObjectIdentifier>>(route, cancellationToken);
	}
	public Task RetryFailedCreateEventHandlersAsync(Int32 workspaceID)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"{workspaceID:int}/retry-failed-create-event-handlers";
		return PostJsonAsync(route, new{workspaceID}, cancellationToken);
	}
	public Task<WorkspaceSummary> GetWorkspaceSummaryAsync(Int32 workspaceID)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"{workspaceID:int}/summary";
		return GetJsonAsync<WorkspaceSummary>(route, cancellationToken);
	}
	public Task MoveToColdStorageAsync(Int32 workspaceID)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"{workspaceID:int}/move-to-cold-storage";
		return PostJsonAsync(route, new{workspaceID}, cancellationToken);
	}
}


}
