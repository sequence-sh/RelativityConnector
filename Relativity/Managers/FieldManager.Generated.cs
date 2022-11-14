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
using Relativity.Services.Interfaces.Field.Models;
using Relativity.Services.Interfaces.Shared.Models;
#pragma warning disable CS1591

namespace Sequence.Connectors.Relativity.Managers
{
[GeneratedCode("CodeGenerator", "1")]
public class TemplateFieldManager1 : ManagerBase, IFieldManager1
{
	public TemplateFieldManager1(RelativitySettings relativitySettings, IFlurlClient flurlClient)
	:base(relativitySettings, flurlClient) { }
	
	/// <inheritdoc />
	protected override IEnumerable<string> Prefixes
	{
		get
		{
			yield return $"relativity-object-model";
			yield return $"v{RelativitySettings.APIVersionNumber}";
			yield break;
		}
	}
	
	
	public Task<FieldResponse> ReadAsync(Int32 workspaceID, Int32 fieldID, Boolean includeMetadata, Boolean includeActions)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"workspaces/{workspaceID}/fields/fixed-length/{fieldID}/{includeMetadata}/{includeActions}";
		return GetJsonAsync<FieldResponse>(route, cancellationToken);
	}
	
	
	public Task DeleteAsync(Int32 workspaceID, Int32 fieldID)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"workspaces/{workspaceID}/fields/fixed-length/{fieldID}";
		return DeleteAsync(route, cancellationToken);
	}
	
	
	public Task<Int32> CreateFixedLengthFieldAsync(Int32 workspaceID, FixedLengthFieldRequest1 fieldRequest)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"workspaces/{workspaceID}/fields/fixed-length";
		var jsonObject = new {
			fieldRequest,
		};
		return PostJsonAsync<Int32>(route, jsonObject, cancellationToken);
	}
	
	
	public Task UpdateFixedLengthFieldAsync(Int32 workspaceID, Int32 fieldID, FixedLengthFieldRequest1 fieldRequest)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"workspaces/{workspaceID}/fields/fixed-length/{fieldID}";
		var jsonObject = new {
			fieldRequest,
		};
		return PutAsync(route, jsonObject, cancellationToken);
	}
	
	
	public Task<List<ObjectTypeIdentifier>> GetAvailableObjectTypesAsync(Int32 workspaceID)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"workspaces/{workspaceID}/fields/available-object-types";
		var jsonObject = new {
		};
		return GetJsonAsync<List<ObjectTypeIdentifier>>(route, cancellationToken);
	}
	
}


}
