﻿using Sequence.Connectors.Relativity.Managers;
using Relativity.Kepler.Services;
using Relativity.Kepler.Transport;
#pragma warning disable CS1591
namespace Sequence.Connectors.Relativity.ManagerInterfaces;

/// <summary>
/// Exposes the methods for working with files associated with documents
/// </summary>
[WebService("Document File Manager")]
[ServiceAudience(Audience.Public)]
[RoutePrefix("workspace/{workspaceID:int}")]
public interface IDocumentFileManager1 : IManager //Relativity.Services.Interfaces.Document.IDocumentFileManager
{
    /// <summary>Download a document's native file.</summary>
    /// <param name="workspaceID">The ArtifactID of the workspace where the document resides.</param>
    /// <param name="documentID">The ArtifactID of the document</param>
    /// <returns>A stream that contains the native file associated with the document</returns>
    [HttpGet]
    // ReSharper disable once StringLiteralTypo
    [Route("downloadnativefile/{documentID:int}/")]
    Task<IKeplerStream> DownloadNativeFileAsync(int workspaceID, int documentID);

    [SkipCodeGeneration]
    Task<string> DownloadDataAsync(int workspaceID, int documentID);

}
