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
using Relativity.Services.Interfaces.Document;
using Relativity.Services.Interfaces.Document.Models;

namespace Reductech.EDR.Connectors.Relativity.Managers
{
    [GeneratedCode("CodeGenerator", "1")]
    public class TemplateDocumentFileManager : ManagerBase, IDocumentFileManager
    {
        public TemplateDocumentFileManager(RelativitySettings relativitySettings, IFlurlClient flurlClient)
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
	
	
        public Task<IKeplerStream> DownloadNativeFileAsync(Int32 workspaceID, Int32 documentID)
        {
            var cancellationToken = CancellationToken.None;
            var route = $"workspace/{workspaceID}/downloadnativefile/{documentID}/";
            return GetJsonAsync<IKeplerStream>(route, cancellationToken);
        }
	
	
        public Task<IKeplerStream> DownloadFileAsync(Int32 workspaceID, Guid fileGuid)
        {
            var cancellationToken = CancellationToken.None;
            var route = $"workspace/{workspaceID}/downloadfile/{fileGuid}";
            return GetJsonAsync<IKeplerStream>(route, cancellationToken);
        }
	
	
        public Task<List<DocumentFile>> GetFileInfoAsync(Int32 workspaceID, Int32 documentID)
        {
            var cancellationToken = CancellationToken.None;
            var route = $"workspace/{workspaceID}/fileinfo/{documentID}";
            return GetJsonAsync<List<DocumentFile>>(route, cancellationToken);
        }
	
    }


}