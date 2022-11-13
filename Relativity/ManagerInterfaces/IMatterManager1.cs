using Relativity.Environment.V1.Matter.Models;
using Relativity.Kepler.Services;
using Relativity.Shared.V1.Models;
#pragma warning disable CS1591
namespace Sequence.Connectors.Relativity.ManagerInterfaces;

[WebService("Matter Manager")]
[ServiceAudience(Audience.Public)]
[RoutePrefix("workspaces/-1/matters")]
public interface IMatterManager1 : IManager //Relativity.Environment.V1.Matter.IMatterManager
{
    [HttpPost]
    [Route("")]
    Task<int> CreateAsync([JsonParameter]MatterRequest matterRequest);

    [HttpGet]
    [Route("{matterID:int}")]
    Task<MatterResponse> ReadAsync(int matterID);

    [HttpGet]
    [Route("{matterID:int}/{includeMetadata:bool}/{includeActions:bool}")]
    Task<MatterResponse> ReadAsync(
        int matterID,
        bool includeMetadata,
        bool includeActions);

    [HttpPut]
    [Route("{matterID:int}")]
    Task UpdateAsync(int matterID,[JsonParameter] MatterRequest matterRequest);
        

    [HttpDelete]
    [Route("{matterID:int}")]
    Task DeleteAsync(int matterID);

    [HttpGet]
    [Route("~/workspaces/-1/eligible-clients")]
    Task<List<DisplayableObjectIdentifier>> GetEligibleClientsAsync();

    [HttpGet]
    [Route("eligible-statuses")]
    Task<List<DisplayableObjectIdentifier>> GetEligibleStatusesAsync();
}
