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
using Relativity.Services.Interfaces.Document;
using Relativity.Services.Interfaces.Document.Models;

namespace Reductech.EDR.Connectors.Relativity.Managers
{
[GeneratedCode("CodeGenerator", "1")]
public class TemplateDocumentFileManager1 : ManagerBase, IDocumentFileManager1
{
    public TemplateDocumentFileManager1(RelativitySettings relativitySettings, IFlurlClient flurlClient)
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
        var route             = $"workspace/{workspaceID}/downloadnativefile/{documentID}/";
        return GetJsonAsync<IKeplerStream>(route, cancellationToken);
    }
	
}


}
