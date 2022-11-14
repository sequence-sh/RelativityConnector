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
using Relativity.Services.Objects;
using Relativity.Services.Objects.DataContracts;
using Relativity.Services.DataContracts.DTOs;
using Relativity.Services.DataContracts.DTOs.Results;
using Relativity.Services.Interfaces.Shared;
#pragma warning disable CS1591

namespace Sequence.Connectors.Relativity.Managers
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
	
	
	public Task<QueryResultSlim> QuerySlimAsync(Int32 workspaceID, QueryRequest request, Int32 start, Int32 length, CancellationToken cancel)
	{
		var cancellationToken = cancel;
		var route = $"/~/workspace/{workspaceID}/object/queryslim";
		var jsonObject = new {
			request,
			start,
			length,
		};
		return PostJsonAsync<QueryResultSlim>(route, jsonObject, cancellationToken);
	}
	
	
	public Task<RelativityObjectSlim[]> RetrieveNextResultsBlockFromExportAsync(Int32 workspaceID, Guid runID, Int32 batchSize)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"/~/workspace/{workspaceID}/object/retrievenextresultsblockfromexport";
		var jsonObject = new {
			runID,
			batchSize,
		};
		return PostJsonAsync<RelativityObjectSlim[]>(route, jsonObject, cancellationToken);
	}
	
	
	public Task<IKeplerStream> StreamLongTextAsync(Int32 workspaceID, RelativityObjectRef exportObject, FieldRef longTextField)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"/~/workspace/{workspaceID}/object/streamlongtext";
		var jsonObject = new {
			exportObject,
			longTextField,
		};
		return PostJsonAsync<IKeplerStream>(route, jsonObject, cancellationToken);
	}
	
	
	public Task<ExportInitializationResults> InitializeExportAsync(Int32 workspaceID, QueryRequest queryRequest, Int32 start)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"/~/workspace/{workspaceID}/object/initializeexport";
		var jsonObject = new {
			queryRequest,
			start,
		};
		return PostJsonAsync<ExportInitializationResults>(route, jsonObject, cancellationToken);
	}
	
	
	public Task<UpdateResult> UpdateAsync(Int32 workspaceID, UpdateRequest request, UpdateOptions operationOptions)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"/~/workspace/{workspaceID}/object/update";
		var jsonObject = new {
			request,
			operationOptions,
		};
		return PostJsonAsync<UpdateResult>(route, jsonObject, cancellationToken);
	}
	
}


}
