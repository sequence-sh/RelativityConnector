using System;
using System.Threading;
using System.Threading.Tasks;
using Relativity.Kepler.Services;
using Relativity.Services.DataContracts.DTOs;
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
}

}
