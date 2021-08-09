
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Flurl.Http;
using Reductech.EDR.Connectors.Relativity.ManagerInterfaces;
using Relativity.Kepler.Transport;
using Relativity.Environment.V1.Matter;
using Relativity.Environment.V1.Matter.Models;
using Relativity.Environment.V1.Workspace.Models;
using Relativity.Environment.V1.Workspace;
using Relativity.Services.Objects;
using Relativity.Services.Objects.DataContracts;
using Relativity.Services.DataContracts.DTOs;
using Relativity.Services.DataContracts.DTOs.Results;
using Relativity.Services.Interfaces.Shared;

namespace Reductech.EDR.Connectors.Relativity.Managers
{
[GeneratedCode("CodeGenerator", "1")]
public class TemplateObjectManager1 : ManagerBase, IObjectManager1
{
	public TemplateObjectManager1(RelativitySettings relativitySettings, IFlurlClient flurlClient)
	:base(relativitySettings, flurlClient) { }
	
	/// <inheritdoc />
	protected override IEnumerable<string> Prefixes
	{
		get
		{
			yield return $"Relativity.Objects";
			yield break;
		}
	}
	
	
	public Task<MassCreateResult> CreateAsync(Int32 workspaceID, MassCreateRequest massRequest, CancellationToken cancel)
	{
		var cancellationToken = cancel;
		var route = $"/~/workspace/{workspaceID}/object/create";
		var jsonObject = new {
			massRequest,
		};
		return PostJsonAsync<MassCreateResult>(route, jsonObject, cancellationToken);
	}
	
	
	public Task<DeleteResult> DeleteAsync(Int32 workspaceID, DeleteRequest request, CancellationToken cancel)
	{
		var cancellationToken = cancel;
		var route = $"/~/workspace/{workspaceID}/object/delete";
		var jsonObject = new {
			request,
		};
		return PostJsonAsync<DeleteResult>(route, jsonObject, cancellationToken);
	}
	
	
	public Task<QueryResult> QueryAsync(Int32 workspaceID, QueryRequest request, Int32 start, Int32 length, CancellationToken cancel, IProgress<ProgressReport> progress)
	{
		var cancellationToken = cancel;
		var route = $"/~/workspace/{workspaceID}/object/query";
		var jsonObject = new {
			request,
			start,
			length,
		};
		return PostJsonAsync<QueryResult>(route, jsonObject, cancellationToken);
	}
	
}


}
