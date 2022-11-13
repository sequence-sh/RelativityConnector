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
using Relativity.Services.Search;
using Relativity.Services.Interfaces.Document;
using Relativity.Services.Interfaces.Document.Models;
#pragma warning disable CS1591

namespace Sequence.Connectors.Relativity.Managers
{
[GeneratedCode("CodeGenerator", "1")]
public class TemplateKeywordSearchManager1 : ManagerBase, IKeywordSearchManager1
{
	public TemplateKeywordSearchManager1(RelativitySettings relativitySettings, IFlurlClient flurlClient)
	:base(relativitySettings, flurlClient) { }
	
	/// <inheritdoc />
	protected override IEnumerable<string> Prefixes
	{
		get
		{
			yield return $"Relativity.Services.Search.ISearchModule";
			yield return $"Keyword Search Manager";
			yield break;
		}
	}
	
	
	public Task<Int32> CreateSingleAsync(Int32 workspaceArtifactID, KeywordSearch searchDTO)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"/CreateSingleAsync";
		var jsonObject = new {
			workspaceArtifactID,
			searchDTO,
		};
		return PostJsonAsync<Int32>(route, jsonObject, cancellationToken);
	}
	
	
	public Task DeleteSingleAsync(Int32 workspaceArtifactID, Int32 searchArtifactID)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"/DeleteSingleAsync";
		var jsonObject = new {
			workspaceArtifactID,
			searchArtifactID,
		};
		return PostJsonAsync(route, jsonObject, cancellationToken);
	}
	
	
	public Task<KeywordSearch> ReadSingleAsync(Int32 workspaceArtifactID, Int32 searchArtifactID)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"/ReadSingleAsync";
		var jsonObject = new {
			workspaceArtifactID,
			searchArtifactID,
		};
		return PostJsonAsync<KeywordSearch>(route, jsonObject, cancellationToken);
	}
	
}


}
