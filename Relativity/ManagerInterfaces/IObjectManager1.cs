using System;
using System.Threading;
using System.Threading.Tasks;
using Relativity.Kepler.Services;
using Relativity.Kepler.Transport;
using Relativity.Services.DataContracts.DTOs;
using Relativity.Services.DataContracts.DTOs.Results;
using Relativity.Services.Objects.DataContracts;

namespace Reductech.EDR.Connectors.Relativity.ManagerInterfaces
{

/// <summary>
/// IObjectManager interface exposes methods on the Relativity Object Manager web service used for querying as well as reading and updating fields on Documents and Relativity Dynamic Objects (RDOs).
/// </summary>
[WebService("Object Manager")]
[ServiceAudience(Audience.Public)]
public interface IObjectManager1 : IManager //Relativity.Services.Objects.IObjectManager
{
    /// <summary>
    /// Creates a set of Relativity Dynamic Objects (RDOs) with the specified fields, and provides a token used for canceling the operation.
    /// </summary>
    /// <param name="workspaceID">Workspace ID of the workspace containing the artifacts to be created.</param>
    /// <param name="massRequest">A request to create multiple Relativity Dynamic Objects (RDOs).</param>
    /// <param name="cancel">A request to cancel the execution of a create job for an object.</param>
    /// <returns>A <see cref="T:Relativity.Services.Objects.DataContracts.MassCreateResult" /> containing a list of created RDOs and a message when an error occurs.</returns>
    /// <remarks>Process halts at first failure or upon cancellation with no rollback.</remarks>
    [HttpPost]
    [Route("~/workspace/{workspaceID:int}/object/create")]
    Task<MassCreateResult> CreateAsync(
        int workspaceID,
        [JsonParameter]
        MassCreateRequest massRequest,
        CancellationToken cancel);


    /// <summary>
    /// Deletes Documents or Relativity Dynamic Objects (RDOs) and their associated files, as well as reports the query execution progress, and provides a token used for canceling the operation.
    /// </summary>
    /// <param name="workspaceID">Workspace ID of the workspace containing the artifacts to be deleted.</param>
    /// <param name="request">A request to delete a Document or Relativity Dynamic Object (RDO).</param>
    /// <param name="cancel">Used to request cancel of the object delete execution.</param>
    /// <returns>Returns the results of a delete operation as a <see cref="T:Relativity.Services.Objects.DataContracts.DeleteResult" /> containing information about the deleted items.</returns>
    [HttpPost]
    [Route("~/workspace/{workspaceID:int}/object/delete")]
    Task<DeleteResult> DeleteAsync(
        int workspaceID,
        [JsonParameter]
        DeleteRequest request,
        CancellationToken cancel);

    /// <summary>
    /// Searches for Workspaces, Documents, RDOs, and system types, reports the query execution progress, and provides a token used for canceling the query.
    /// </summary>
    /// <param name="workspaceID">Workspace ID of the workspace containing the artifacts to query for. Pass -1 for admin artifacts.</param>
    /// <param name="request">A <see cref="T:Relativity.Services.Objects.DataContracts.QueryRequest" /> object containing conditions, sorting order, fields, and other information for the query.</param>
    /// <param name="start">Index of the first artifact in the returned <see cref="T:Relativity.Services.Objects.DataContracts.QueryResult" /> object.</param>
    /// <param name="length">Number of items to return in the query result, starting with index in the start parameter.</param>
    /// <param name="cancel">Used to request the cancellation of the object query.</param>
    /// <param name="progress">Contains a callback that reports query execution progress.</param>
    /// <returns>Returns the results of the executed query.</returns>
    /// <remarks>The QueryAsync() methods support querying for the following system types: Application, Batch, Batch Sets, Choice, Client, Group, Markup Set, User,
    /// Object Type, Tab, Folder, Layout, View, Workspace, Error, Field, Relativity Script, and Saved Search.</remarks>
    [HttpPost]
    [Route("~/workspace/{workspaceID:int}/object/query")]
    Task<QueryResult> QueryAsync(
        int workspaceID,
        [JsonParameter]
        QueryRequest request,
        [JsonParameter]
        int start,
        [JsonParameter]
        int length,
        CancellationToken cancel,
        IProgress<ProgressReport> progress);


    /// <summary>
    /// Query for Workspaces, Documents, RDOs, and system types. This returns a slimmer payload aimed towards display of the query results in a grid.
    /// </summary>
    /// <param name="workspaceID">Workspace ID of the workspace containing the artifacts to query for. Pass -1 for admin artifacts.</param>
    /// <param name="request">A <see cref="T:Relativity.Services.Objects.DataContracts.QueryRequest" /> object containing conditions, sorting order, fields and other information for the query.</param>
    /// <param name="start">Index of the first artifact in the returned <see cref="T:Relativity.Services.Objects.DataContracts.QueryResult" /> object.</param>
    /// <param name="length">Number of items to return in the query result, starting with index in the start parameter.</param>
    /// <param name="cancel">A request to cancel the execution of a create job for an object.</param>
    /// <returns>Returns the results of the executed query.</returns>
    [HttpPost]
    [Route("~/workspace/{workspaceID:int}/object/queryslim")]
    Task<QueryResultSlim> QuerySlimAsync(
        int workspaceID,
        [JsonParameter]
        QueryRequest request,
        [JsonParameter]
        int start,
        [JsonParameter]
        int length,
        CancellationToken cancel);


    /// <summary>Retrieves a results block from a given export set.</summary>
    /// <param name="workspaceID">Workspace ID of the workspace containing the data to export.</param>
    /// <param name="runID">The run ID of the export generated from initialization.</param>
    /// <param name="batchSize">The desired number of results to return from the export set; note that this size may be capped by an internal limit.</param>
    /// <returns>The next block of export results, or null if the export is complete .</returns>
    [HttpPost]
    [Route("~/workspace/{workspaceID:int}/object/retrievenextresultsblockfromexport")]
    Task<RelativityObjectSlim[]> RetrieveNextResultsBlockFromExportAsync(
        int workspaceID,
        [JsonParameter]
        Guid runID,
        [JsonParameter]
        int batchSize);


    /// <summary>
    /// Retrieves the contents of a long text field on a Document or Relativity Dynamic Object (RDO) as a stream.
    /// </summary>
    /// <param name="workspaceID">Workspace ID of the workspace containing the object to be retrieved.</param>
    /// <param name="exportObject">A <see cref="T:Relativity.Services.Objects.DataContracts.RelativityObjectRef" /> of the Document or Relativity Dynamic Object (RDO) that contains the text to be streamed.</param>
    /// <param name="longTextField">A <see cref="T:Relativity.Services.Objects.DataContracts.FieldRef" /> of the long text field that contains the text to be streamed.</param>
    /// <returns>
    /// A <see cref="T:Relativity.Kepler.Transport.IKeplerStream" /> that contains the content of the selected long text field.
    /// This stream is the text encoded in either <see cref="P:System.Text.Encoding.Unicode" /> for unicode-enabled fields or <see cref="P:System.Text.Encoding.ASCII" /> otherwise.
    /// </returns>
    [HttpPost]
    [Route("~/workspace/{workspaceID:int}/object/streamlongtext")]
    Task<IKeplerStream> StreamLongTextAsync(
        int workspaceID,
        [JsonParameter]
        RelativityObjectRef exportObject,
        [JsonParameter]
        FieldRef longTextField);


    /// <summary>
    /// Initializes an export job, populated with the results of a <see cref="T:Relativity.Services.Objects.DataContracts.QueryRequest" />.
    /// </summary>
    /// <param name="workspaceID">Workspace ID of the workspace containing the data to export.</param>
    /// <param name="queryRequest">Query that specifies the data set to export.
    /// Includes ObjectType and Fields to be exported, query information to determine the set of objects to export, and an optional maximum length to export inline.</param>
    /// <param name="start">The record index to begin exporting from.</param>
    /// <returns>An <see cref="T:Relativity.Services.DataContracts.DTOs.Results.ExportInitializationResults" /> containing an export run ID, and a count of records to be exported.</returns>
    /// <remarks>After initialization, results can be retrieved using <see cref="M:Relativity.Services.Objects.IObjectManager.RetrieveNextResultsBlockFromExportAsync(System.Int32,System.Guid,System.Int32)" /> or <see cref="M:Relativity.Services.Objects.IObjectManager.RetrieveResultsBlockFromExportAsync(System.Int32,System.Guid,System.Int32,System.Int32)" />.</remarks>
    [HttpPost]
    [Route("~/workspace/{workspaceID:int}/object/initializeexport")]
    Task<ExportInitializationResults> InitializeExportAsync(
        int workspaceID,
        [JsonParameter]
        QueryRequest queryRequest,
        [JsonParameter]
        int start);


    /// <summary>
    /// Modifies all fields or a specified subset of fields on a Document or Relativity Dynamic Object (RDO), and uses any specified update options.
    /// </summary>
    /// <param name="workspaceID">Workspace ID of the workspace containing the artifact to be updated.</param>
    /// <param name="request">A request to update a Document or Relativity Dynamic Object (RDO).</param>
    /// <param name="operationOptions">An options object containing a calling context property providing information used to populate data for event handlers.</param>
    /// <returns>An <see cref="T:Relativity.Services.Objects.DataContracts.UpdateResult" /> with event handler statuses.</returns>
    [HttpPost]
    [Route("~/workspace/{workspaceID:int}/object/update")]
    Task<UpdateResult> UpdateAsync(
        int workspaceID,
        [JsonParameter]
        UpdateRequest request,
        [JsonParameter]
        UpdateOptions operationOptions);
}

}
