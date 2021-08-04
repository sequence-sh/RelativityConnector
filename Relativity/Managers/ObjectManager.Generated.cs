
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
using Relativity.Services.Objects;
using Relativity.Services.Objects.DataContracts;
using Relativity.Services.DataContracts.DTOs;
using Relativity.Services.DataContracts.DTOs.Results;
using Relativity.Services.Interfaces.Shared;

namespace Reductech.EDR.Connectors.Relativity.Managers
{
[GeneratedCode("CodeGenerator", "1")]
public class TemplateObjectManager : ManagerBase, IObjectManager
{
	public TemplateObjectManager(RelativitySettings relativitySettings, IFlurlClient flurlClient)
	:base(relativitySettings, flurlClient) { }
	
	/// <inheritdoc />
	protected override IEnumerable<string> Prefixes
	{
		get
		{
			yield break;
		}
	}
	
	
	public Task<MassCreateResult> CreateAsync(Int32 workspaceID, MassCreateRequest massRequest, CancellationToken cancel)
	{
		var cancellationToken = cancel;
		var route = $"/~/workspace/{workspaceID}/object/create";
		return PostJsonAsync<MassCreateResult>(route, new{workspaceID}, cancellationToken);
	}
	
	
	public Task<QueryResult> QueryAsync(Int32 workspaceID, QueryRequest request, Int32 start, Int32 length)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"Relativity.Objects/workspace/{workspaceID}/object/query";
		return PostJsonAsync<QueryResult>(route, new{workspaceID}, cancellationToken);
	}
	
	
	public Task<QueryResult> QueryAsync(Int32 workspaceID, QueryRequest request, Int32 start, Int32 length, IProgress<ProgressReport> progress)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"Relativity.Objects/workspace/{workspaceID}/object/query";
		return PostJsonAsync<QueryResult>(route, new{workspaceID}, cancellationToken);
	}
	
	
	public Task<QueryResult> QueryAsync(Int32 workspaceID, QueryRequest request, Int32 start, Int32 length, CancellationToken cancel, IProgress<ProgressReport> progress)
	{
		var cancellationToken = cancel;
		var route = $"Relativity.Objects/workspace/{workspaceID}/object/query";
		return PostJsonAsync<QueryResult>(route, new{workspaceID}, cancellationToken);
	}
	
	
	public Task<UpdateResult> UpdateAsync(Int32 workspaceID, UpdateRequest request)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"/~/workspace/{workspaceID}/object/update";
		return PostJsonAsync<UpdateResult>(route, new{workspaceID}, cancellationToken);
	}
	
	
	public Task<UpdateResult> UpdateAsync(Int32 workspaceID, UpdateRequest request, UpdateOptions operationOptions)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"/~/workspace/{workspaceID}/object/update";
		return PostJsonAsync<UpdateResult>(route, new{workspaceID}, cancellationToken);
	}
	
	
	public Task<ReadResult> ReadAsync(Int32 workspaceID, ReadRequest request)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"/~/workspace/{workspaceID}/object/read";
		return PostJsonAsync<ReadResult>(route, new{workspaceID}, cancellationToken);
	}
	
	
	public Task<ReadResult> ReadAsync(Int32 workspaceID, ReadRequest request, OperationOptions operationOptions)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"/~/workspace/{workspaceID}/object/read";
		return PostJsonAsync<ReadResult>(route, new{workspaceID}, cancellationToken);
	}
	
	
	public Task<ReadResult> ReadAsync(Int32 workspaceID, ReadRequest request, ReadOptions readOptions)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"/~/workspace/{workspaceID}/object/read";
		return PostJsonAsync<ReadResult>(route, new{workspaceID}, cancellationToken);
	}
	
	
	public Task<DeleteResult> DeleteAsync(Int32 workspaceID, DeleteRequest request)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"/~/workspace/{workspaceID}/object/delete";
		return PostJsonAsync<DeleteResult>(route, new{workspaceID}, cancellationToken);
	}
	
	
	public Task<DeleteResult> DeleteAsync(Int32 workspaceID, DeleteRequest request, CancellationToken cancel, IProgress<DeleteProcessStateProgress> progress)
	{
		var cancellationToken = cancel;
		var route = $"/~/workspace/{workspaceID}/object/delete";
		return PostJsonAsync<DeleteResult>(route, new{workspaceID}, cancellationToken);
	}
	
	
	public Task<DeleteResult> DeleteAsync(Int32 workspaceID, DeleteRequest request, CancellationToken cancel)
	{
		var cancellationToken = cancel;
		var route = $"/~/workspace/{workspaceID}/object/delete";
		return PostJsonAsync<DeleteResult>(route, new{workspaceID}, cancellationToken);
	}
	
	
	public Task<DeleteResult> DeleteAsync(Int32 workspaceID, DeleteRequest request, IProgress<DeleteProcessStateProgress> progress)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"/~/workspace/{workspaceID}/object/delete";
		return PostJsonAsync<DeleteResult>(route, new{workspaceID}, cancellationToken);
	}
	
	
	public Task<CreateResult> CreateAsync(Int32 workspaceID, CreateRequest request)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"/~/workspace/{workspaceID}/object/create";
		return PostJsonAsync<CreateResult>(route, new{workspaceID}, cancellationToken);
	}
	
	
	public Task<CreateResult> CreateAsync(Int32 workspaceID, CreateRequest request, OperationOptions operationOptions)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"/~/workspace/{workspaceID}/object/create";
		return PostJsonAsync<CreateResult>(route, new{workspaceID}, cancellationToken);
	}
	
	
	public Task<QueryResultSlim> QuerySlimAsync(Int32 workspaceID, QueryRequest request, Int32 start, Int32 length)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"/~/workspace/{workspaceID}/object/queryslim";
		return PostJsonAsync<QueryResultSlim>(route, new{workspaceID}, cancellationToken);
	}
	
	
	public Task<QueryResultSlim> QuerySlimAsync(Int32 workspaceID, QueryRequest request, Int32 start, Int32 length, CancellationToken cancel, IProgress<ProgressReport> progress)
	{
		var cancellationToken = cancel;
		var route = $"/~/workspace/{workspaceID}/object/queryslim";
		return PostJsonAsync<QueryResultSlim>(route, new{workspaceID}, cancellationToken);
	}
	
	
	public Task<QueryResultSlim> QuerySlimAsync(Int32 workspaceID, QueryRequest request, Int32 start, Int32 length, CancellationToken cancel)
	{
		var cancellationToken = cancel;
		var route = $"/~/workspace/{workspaceID}/object/queryslim";
		return PostJsonAsync<QueryResultSlim>(route, new{workspaceID}, cancellationToken);
	}
	
	
	public Task<QueryResultSlim> QuerySlimAsync(Int32 workspaceID, QueryRequest request, Int32 start, Int32 length, IProgress<ProgressReport> progress)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"/~/workspace/{workspaceID}/object/queryslim";
		return PostJsonAsync<QueryResultSlim>(route, new{workspaceID}, cancellationToken);
	}
	
	
	public Task<IKeplerStream> StreamLongTextAsync(Int32 workspaceID, RelativityObjectRef exportObject, FieldRef longTextField)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"/~/workspace/{workspaceID}/object/streamlongtext";
		return PostJsonAsync<IKeplerStream>(route, new{workspaceID}, cancellationToken);
	}
	
	
	public Task<ExportInitializationResults> InitializeExportAsync(Int32 workspaceID, QueryRequest queryRequest, Int32 start)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"/~/workspace/{workspaceID}/object/initializeexport";
		return PostJsonAsync<ExportInitializationResults>(route, new{workspaceID}, cancellationToken);
	}
	
	
	public Task<ExportInitializationResults> InitializeExportAsync(Int32 workspaceID, QueryRequest queryRequest, Guid requestKey, Int32 start)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"/~/workspace/{workspaceID}/object/initializeexport";
		return PostJsonAsync<ExportInitializationResults>(route, new{workspaceID}, cancellationToken);
	}
	
	
	public Task<RelativityObjectSlim[]> RetrieveNextResultsBlockFromExportAsync(Int32 workspaceID, Guid runID, Int32 batchSize)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"/~/workspace/{workspaceID}/object/retrievenextresultsblockfromexport";
		return PostJsonAsync<RelativityObjectSlim[]>(route, new{workspaceID}, cancellationToken);
	}
	
	
	public Task<RelativityObjectSlim[]> RetrieveResultsBlockFromExportAsync(Int32 workspaceID, Guid runID, Int32 resultsBlockSize, Int32 exportIndexID)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"/~/workspace/{workspaceID}/object/retrieveresultsblockfromexport";
		return PostJsonAsync<RelativityObjectSlim[]>(route, new{workspaceID}, cancellationToken);
	}
	
	
	public Task<MassDeleteResult> DeleteAsync(Int32 workspaceID, MassDeleteByObjectIdentifiersRequest massRequestByObjectIdentifiers)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"/~/workspace/{workspaceID}/object/delete";
		return PostJsonAsync<MassDeleteResult>(route, new{workspaceID}, cancellationToken);
	}
	
	
	public Task<MassDeleteResult> DeleteAsync(Int32 workspaceID, MassDeleteByObjectIdentifiersRequest massRequestByObjectIdentifiers, IProgress<MassOperationsStateProgress> progress)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"/~/workspace/{workspaceID}/object/delete";
		return PostJsonAsync<MassDeleteResult>(route, new{workspaceID}, cancellationToken);
	}
	
	
	public Task<MassDeleteResult> DeleteAsync(Int32 workspaceID, MassDeleteByObjectIdentifiersRequest massRequestByObjectIdentifiers, CancellationToken cancel)
	{
		var cancellationToken = cancel;
		var route = $"/~/workspace/{workspaceID}/object/delete";
		return PostJsonAsync<MassDeleteResult>(route, new{workspaceID}, cancellationToken);
	}
	
	
	public Task<MassDeleteResult> DeleteAsync(Int32 workspaceID, MassDeleteByObjectIdentifiersRequest massRequestByObjectIdentifiers, CancellationToken cancel, IProgress<MassOperationsStateProgress> progress)
	{
		var cancellationToken = cancel;
		var route = $"/~/workspace/{workspaceID}/object/delete";
		return PostJsonAsync<MassDeleteResult>(route, new{workspaceID}, cancellationToken);
	}
	
	
	public Task<MassDeleteResult> DeleteAsync(Int32 workspaceID, MassDeleteByCriteriaRequest massRequestByCriteria)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"/~/workspace/{workspaceID}/object/delete";
		return PostJsonAsync<MassDeleteResult>(route, new{workspaceID}, cancellationToken);
	}
	
	
	public Task<MassDeleteResult> DeleteAsync(Int32 workspaceID, MassDeleteByCriteriaRequest massRequestByCriteria, CancellationToken cancel)
	{
		var cancellationToken = cancel;
		var route = $"/~/workspace/{workspaceID}/object/delete";
		return PostJsonAsync<MassDeleteResult>(route, new{workspaceID}, cancellationToken);
	}
	
	
	public Task<MassDeleteResult> DeleteAsync(Int32 workspaceID, MassDeleteByCriteriaRequest massRequestByCriteria, IProgress<MassOperationsStateProgress> progress)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"/~/workspace/{workspaceID}/object/delete";
		return PostJsonAsync<MassDeleteResult>(route, new{workspaceID}, cancellationToken);
	}
	
	
	public Task<MassDeleteResult> DeleteAsync(Int32 workspaceID, MassDeleteByCriteriaRequest massRequestByCriteria, CancellationToken cancel, IProgress<MassOperationsStateProgress> progress)
	{
		var cancellationToken = cancel;
		var route = $"/~/workspace/{workspaceID}/object/delete";
		return PostJsonAsync<MassDeleteResult>(route, new{workspaceID}, cancellationToken);
	}
	
	
	public Task<MassCreateResult> CreateAsync(Int32 workspaceID, MassCreateRequest massRequest)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"/~/workspace/{workspaceID}/object/create";
		return PostJsonAsync<MassCreateResult>(route, new{workspaceID}, cancellationToken);
	}
	
	
	public Task<MassCreateResult> CreateAsync(Int32 workspaceID, MassCreateRequest massRequest, IProgress<MassOperationsStateProgress> progress)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"/~/workspace/{workspaceID}/object/create";
		return PostJsonAsync<MassCreateResult>(route, new{workspaceID}, cancellationToken);
	}
	
	
	public Task<MassCreateResult> CreateAsync(Int32 workspaceID, MassCreateRequest massRequest, CancellationToken cancel, IProgress<MassOperationsStateProgress> progress)
	{
		var cancellationToken = cancel;
		var route = $"/~/workspace/{workspaceID}/object/create";
		return PostJsonAsync<MassCreateResult>(route, new{workspaceID}, cancellationToken);
	}
	
	
	public Task<MassUpdateResult> UpdateAsync(Int32 workspaceID, MassUpdateByObjectIdentifiersRequest massRequestByObjectIdentifiers)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"/~/workspace/{workspaceID}/object/update";
		throw new NotImplementedException();
	}
	
	
	public Task<MassUpdateResult> UpdateAsync(Int32 workspaceID, MassUpdateByObjectIdentifiersRequest massRequestByObjectIdentifiers, MassUpdateOptions options)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"/~/workspace/{workspaceID}/object/update";
		throw new NotImplementedException();
	}
	
	
	public Task<MassUpdateResult> UpdateAsync(Int32 workspaceID, MassUpdateByObjectIdentifiersRequest massRequestByObjectIdentifiers, CancellationToken cancel)
	{
		var cancellationToken = cancel;
		var route = $"/~/workspace/{workspaceID}/object/update";
		throw new NotImplementedException();
	}
	
	
	public Task<MassUpdateResult> UpdateAsync(Int32 workspaceID, MassUpdateByObjectIdentifiersRequest massRequestByObjectIdentifiers, MassUpdateOptions options, CancellationToken cancel)
	{
		var cancellationToken = cancel;
		var route = $"/~/workspace/{workspaceID}/object/update";
		throw new NotImplementedException();
	}
	
	
	public Task<MassUpdateResult> UpdateAsync(Int32 workspaceID, MassUpdateByObjectIdentifiersRequest massRequestByObjectIdentifiers, IProgress<MassOperationsStateProgress> progress)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"/~/workspace/{workspaceID}/object/update";
		throw new NotImplementedException();
	}
	
	
	public Task<MassUpdateResult> UpdateAsync(Int32 workspaceID, MassUpdateByObjectIdentifiersRequest massRequestByObjectIdentifiers, MassUpdateOptions options, IProgress<MassOperationsStateProgress> progress)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"/~/workspace/{workspaceID}/object/update";
		throw new NotImplementedException();
	}
	
	
	public Task<MassUpdateResult> UpdateAsync(Int32 workspaceID, MassUpdateByObjectIdentifiersRequest massRequestByObjectIdentifiers, CancellationToken cancel, IProgress<MassOperationsStateProgress> progress)
	{
		var cancellationToken = cancel;
		var route = $"/~/workspace/{workspaceID}/object/update";
		throw new NotImplementedException();
	}
	
	
	public Task<MassUpdateResult> UpdateAsync(Int32 workspaceID, MassUpdateByObjectIdentifiersRequest massRequestByObjectIdentifiers, MassUpdateOptions options, CancellationToken cancel, IProgress<MassOperationsStateProgress> progress)
	{
		var cancellationToken = cancel;
		var route = $"/~/workspace/{workspaceID}/object/update";
		throw new NotImplementedException();
	}
	
	
	public Task<MassUpdateResult> UpdateAsync(Int32 workspaceID, MassUpdateByCriteriaRequest massRequestByCriteria)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"/~/workspace/{workspaceID}/object/update";
		throw new NotImplementedException();
	}
	
	
	public Task<MassUpdateResult> UpdateAsync(Int32 workspaceID, MassUpdateByCriteriaRequest massRequestByCriteria, MassUpdateOptions options)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"/~/workspace/{workspaceID}/object/update";
		throw new NotImplementedException();
	}
	
	
	public Task<MassUpdateResult> UpdateAsync(Int32 workspaceID, MassUpdateByCriteriaRequest massRequestByCriteria, CancellationToken cancel)
	{
		var cancellationToken = cancel;
		var route = $"/~/workspace/{workspaceID}/object/update";
		throw new NotImplementedException();
	}
	
	
	public Task<MassUpdateResult> UpdateAsync(Int32 workspaceID, MassUpdateByCriteriaRequest massRequestByCriteria, MassUpdateOptions options, CancellationToken cancel)
	{
		var cancellationToken = cancel;
		var route = $"/~/workspace/{workspaceID}/object/update";
		throw new NotImplementedException();
	}
	
	
	public Task<MassUpdateResult> UpdateAsync(Int32 workspaceID, MassUpdateByCriteriaRequest massRequestByCriteria, IProgress<MassOperationsStateProgress> progress)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"/~/workspace/{workspaceID}/object/update";
		throw new NotImplementedException();
	}
	
	
	public Task<MassUpdateResult> UpdateAsync(Int32 workspaceID, MassUpdateByCriteriaRequest massRequestByCriteria, MassUpdateOptions options, IProgress<MassOperationsStateProgress> progress)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"/~/workspace/{workspaceID}/object/update";
		throw new NotImplementedException();
	}
	
	
	public Task<MassUpdateResult> UpdateAsync(Int32 workspaceID, MassUpdateByCriteriaRequest massRequestByCriteria, CancellationToken cancel, IProgress<MassOperationsStateProgress> progress)
	{
		var cancellationToken = cancel;
		var route = $"/~/workspace/{workspaceID}/object/update";
		throw new NotImplementedException();
	}
	
	
	public Task<MassUpdateResult> UpdateAsync(Int32 workspaceID, MassUpdateByCriteriaRequest massRequestByCriteria, MassUpdateOptions options, CancellationToken cancel, IProgress<MassOperationsStateProgress> progress)
	{
		var cancellationToken = cancel;
		var route = $"/~/workspace/{workspaceID}/object/update";
		throw new NotImplementedException();
	}
	
	
	public Task<MassUpdateResult> UpdateAsync(Int32 workspaceID, MassUpdatePerObjectsRequest massRequestPerObjects)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"/~/workspace/{workspaceID}/object/update";
		throw new NotImplementedException();
	}
	
	
	public Task<MassUpdateResult> UpdateAsync(Int32 workspaceID, MassUpdatePerObjectsRequest massRequestPerObjects, MassUpdateOptions options)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"/~/workspace/{workspaceID}/object/update";
		throw new NotImplementedException();
	}
	
	
	public Task<MassUpdateResult> UpdateAsync(Int32 workspaceID, MassUpdatePerObjectsRequest massRequestPerObjects, CancellationToken cancel)
	{
		var cancellationToken = cancel;
		var route = $"/~/workspace/{workspaceID}/object/update";
		throw new NotImplementedException();
	}
	
	
	public Task<MassUpdateResult> UpdateAsync(Int32 workspaceID, MassUpdatePerObjectsRequest massRequestPerObjects, MassUpdateOptions options, CancellationToken cancel)
	{
		var cancellationToken = cancel;
		var route = $"/~/workspace/{workspaceID}/object/update";
		throw new NotImplementedException();
	}
	
	
	public Task<MassUpdateResult> UpdateAsync(Int32 workspaceID, MassUpdatePerObjectsRequest massRequestPerObjects, IProgress<MassOperationsStateProgress> progress)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"/~/workspace/{workspaceID}/object/update";
		throw new NotImplementedException();
	}
	
	
	public Task<MassUpdateResult> UpdateAsync(Int32 workspaceID, MassUpdatePerObjectsRequest massRequestPerObjects, MassUpdateOptions options, IProgress<MassOperationsStateProgress> progress)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"/~/workspace/{workspaceID}/object/update";
		throw new NotImplementedException();
	}
	
	
	public Task<MassUpdateResult> UpdateAsync(Int32 workspaceID, MassUpdatePerObjectsRequest massRequestPerObjects, CancellationToken cancel, IProgress<MassOperationsStateProgress> progress)
	{
		var cancellationToken = cancel;
		var route = $"/~/workspace/{workspaceID}/object/update";
		throw new NotImplementedException();
	}
	
	
	public Task UpdateLongTextFromStreamAsync(Int32 workspaceID, UpdateLongTextFromStreamRequest updateLongTextFromStreamRequest, IKeplerStream keplerStream)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"/~/workspace/{workspaceID}/object/updatelongtextfromstream";
		throw new NotImplementedException();
	}
	
	
	public Task<MassUpdateResult> UpdateAsync(Int32 workspaceID, MassUpdatePerObjectsRequest massRequestPerObjects, MassUpdateOptions options, CancellationToken cancel, IProgress<MassOperationsStateProgress> progress)
	{
		var cancellationToken = cancel;
		var route = $"/~/workspace/{workspaceID}/object/update";
		throw new NotImplementedException();
	}
	
	
	public Task<List<Dependency>> GetDependencyListAsync(Int32 workspaceID, DependencyListByObjectIdentifiersRequest request)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"/~/workspace/{workspaceID}/object/dependencylist";
		throw new NotImplementedException();
	}
	
}


}
