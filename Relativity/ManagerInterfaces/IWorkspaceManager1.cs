using System.Threading;
using System.Threading.Tasks;
using Relativity.Environment.V1.Workspace.Models;
using Relativity.Kepler.Services;

namespace Reductech.EDR.Connectors.Relativity.ManagerInterfaces
{

[WebService("Workspace Manager")]
[ServiceAudience(Audience.Public)]
[RoutePrefix("workspace")]
public interface IWorkspaceManager1 : IManager //Relativity.Environment.V1.Workspace.IWorkspaceManager
{
    [HttpPost]
    [Route("")]
    Task<WorkspaceResponse> CreateAsync(
        [JsonParameter]
        WorkspaceRequest workspaceRequest,
        CancellationToken cancel);

    [HttpGet]
    [Route("default-download-handler-url")]
    Task<string> GetDefaultDownloadHandlerURLAsync();

    [HttpDelete]
    [Route("{workspaceID:int}")]
    Task DeleteAsync(int workspaceID, CancellationToken cancel);

    [HttpGet]
    [Route("{workspaceID:int}/{includeMetadata:bool}/{includeActions:bool}")]
    Task<WorkspaceResponse> ReadAsync(
        int workspaceID,
        bool includeMetadata,
        bool includeActions);

    [HttpGet]
    [Route("{workspaceID:int}/summary")]
    Task<WorkspaceSummary> GetWorkspaceSummaryAsync(int workspaceID);
}

}
