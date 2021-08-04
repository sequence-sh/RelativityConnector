
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
using Relativity.Services.Interfaces.Field;
using Relativity.Services.Interfaces.Shared.Models;
using Relativity.Services.Interfaces.Shared;
using Relativity.Services.Interfaces.Field.Models;

namespace Reductech.EDR.Connectors.Relativity.Managers
{
[GeneratedCode("CodeGenerator", "1")]
public class TemplateFieldManager : ManagerBase, IFieldManager
{
	public TemplateFieldManager(RelativitySettings relativitySettings, IFlurlClient flurlClient)
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
	
	
	public Task<List<ObjectTypeIdentifier>> GetAvailableObjectTypesAsync(Int32 workspaceID)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"workspaces/{workspaceID}/fields/available-object-types";
		return GetJsonAsync<List<ObjectTypeIdentifier>>(route,  cancellationToken);
	}
	
	
	public Task<FieldResponse> ReadAsync(Int32 workspaceID, Int32 fieldID)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"workspace/{workspaceID}/fields/{fieldID}";
		return GetJsonAsync<FieldResponse>(route, cancellationToken);
	}
	
	
	public Task<FieldResponse> ReadAsync(Int32 workspaceID, Int32 fieldID, Boolean includeMetadata, Boolean includeActions)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"workspace/{workspaceID}/fields/{fieldID}/{includeMetadata}/{includeActions}";
		return GetJsonAsync<FieldResponse>(route, cancellationToken);
	}
	
	
	public Task DeleteAsync(Int32 workspaceID, Int32 fieldID)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"workspace/{workspaceID}/fields/{fieldID}";
		return DeleteAsync(route, cancellationToken);
	}
	
	
	public Task<Int32> CreateDateFieldAsync(Int32 workspaceID, DateFieldRequest fieldRequest)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"workspace/{workspaceID}/datefields";
		return PostJsonAsync<Int32>(route, new{workspaceID}, cancellationToken);
	}
	
	
	public Task UpdateDateFieldAsync(Int32 workspaceID, Int32 fieldID, DateFieldRequest fieldRequest)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"workspace/{workspaceID}/datefields/{fieldID}";
		return PutAsync(route, new{fieldRequest}, cancellationToken);
	}
	
	
	public Task UpdateDateFieldAsync(Int32 workspaceID, Int32 fieldID, DateFieldRequest fieldRequest, DateTime lastModifiedOn)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"workspace/{workspaceID}/datefields/{fieldID}";
		return PutAsync(route, new{fieldRequest}, cancellationToken);
	}
	
	
	public Task<Int32> CreateFixedLengthFieldAsync(Int32 workspaceID, FixedLengthFieldRequest fieldRequest)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"workspace/{workspaceID}/fixedlengthfields";
		return PostJsonAsync<Int32>(route, new{workspaceID}, cancellationToken);
	}
	
	
	public Task UpdateFixedLengthFieldAsync(Int32 workspaceID, Int32 fieldID, FixedLengthFieldRequest fieldRequest)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"workspace/{workspaceID}/fixedlengthfields/{fieldID}";
		return PutAsync(route, new{fieldRequest}, cancellationToken);
	}
	
	
	public Task UpdateFixedLengthFieldAsync(Int32 workspaceID, Int32 fieldID, FixedLengthFieldRequest fieldRequest, DateTime lastModifiedOn)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"workspace/{workspaceID}/fixedlengthfields/{fieldID}";
		return PutAsync(route, new{fieldRequest}, cancellationToken);
	}
	
	
	public Task<Int32> CreateUserFieldAsync(Int32 workspaceID, UserFieldRequest fieldRequest)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"workspace/{workspaceID}/userfields";
		return PostJsonAsync<Int32>(route, new{workspaceID}, cancellationToken);
	}
	
	
	public Task UpdateUserFieldAsync(Int32 workspaceID, Int32 fieldID, UserFieldRequest fieldRequest)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"workspace/{workspaceID}/userfields/{fieldID}";
		return PutAsync(route, new{fieldRequest}, cancellationToken);
	}
	
	
	public Task UpdateUserFieldAsync(Int32 workspaceID, Int32 fieldID, UserFieldRequest fieldRequest, DateTime lastModifiedOn)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"workspace/{workspaceID}/userfields/{fieldID}";
		return PutAsync(route, new{fieldRequest}, cancellationToken);
	}
	
	
	public Task<Int32> CreateYesNoFieldAsync(Int32 workspaceID, YesNoFieldRequest fieldRequest)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"workspace/{workspaceID}/yesnofields";
		return PostJsonAsync<Int32>(route, new{workspaceID}, cancellationToken);
	}
	
	
	public Task UpdateYesNoFieldAsync(Int32 workspaceID, Int32 fieldID, YesNoFieldRequest fieldRequest)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"workspace/{workspaceID}/yesnofields/{fieldID}";
		return PutAsync(route, new{fieldRequest}, cancellationToken);
	}
	
	
	public Task UpdateYesNoFieldAsync(Int32 workspaceID, Int32 fieldID, YesNoFieldRequest fieldRequest, DateTime lastModifiedOn)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"workspace/{workspaceID}/yesnofields/{fieldID}";
		return PutAsync(route, new{fieldRequest}, cancellationToken);
	}
	
	
	public Task<Int32> CreateLongTextFieldAsync(Int32 workspaceID, LongTextFieldRequest fieldRequest)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"workspace/{workspaceID}/longtextfields";
		return PostJsonAsync<Int32>(route, new{workspaceID}, cancellationToken);
	}
	
	
	public Task UpdateLongTextFieldAsync(Int32 workspaceID, Int32 fieldID, LongTextFieldRequest fieldRequest)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"workspace/{workspaceID}/longtextfields/{fieldID}";
		return PutAsync(route, new{fieldRequest}, cancellationToken);
	}
	
	
	public Task UpdateLongTextFieldAsync(Int32 workspaceID, Int32 fieldID, LongTextFieldRequest fieldRequest, DateTime lastModifiedOn)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"workspace/{workspaceID}/longtextfields/{fieldID}";
		return PutAsync(route, new{fieldRequest}, cancellationToken);
	}
	
	
	public Task<Int32> CreateFileFieldAsync(Int32 workspaceID, FileFieldRequest fieldRequest)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"workspace/{workspaceID}/filefields";
		return PostJsonAsync<Int32>(route, new{workspaceID}, cancellationToken);
	}
	
	
	public Task UpdateFileFieldAsync(Int32 workspaceID, Int32 fieldID, FileFieldRequest fieldRequest)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"workspace/{workspaceID}/filefields/{fieldID}";
		return PutAsync(route, new{fieldRequest}, cancellationToken);
	}
	
	
	public Task UpdateFileFieldAsync(Int32 workspaceID, Int32 fieldID, FileFieldRequest fieldRequest, DateTime lastModifiedOn)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"workspace/{workspaceID}/filefields/{fieldID}";
		return PutAsync(route, new{fieldRequest}, cancellationToken);
	}
	
	
	public Task<Int32> CreateWholeNumberFieldAsync(Int32 workspaceID, WholeNumberFieldRequest fieldRequest)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"workspace/{workspaceID}/wholenumberfields";
		return PostJsonAsync<Int32>(route, new{workspaceID}, cancellationToken);
	}
	
	
	public Task UpdateWholeNumberFieldAsync(Int32 workspaceID, Int32 fieldID, WholeNumberFieldRequest fieldRequest)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"workspace/{workspaceID}/wholenumberfields/{fieldID}";
		return PutAsync(route, new{fieldRequest}, cancellationToken);
	}
	
	
	public Task UpdateWholeNumberFieldAsync(Int32 workspaceID, Int32 fieldID, WholeNumberFieldRequest fieldRequest, DateTime lastModifiedOn)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"workspace/{workspaceID}/wholenumberfields/{fieldID}";
		return PutAsync(route, new{fieldRequest}, cancellationToken);
	}
	
	
	public Task<Int32> CreateDecimalFieldAsync(Int32 workspaceID, DecimalFieldRequest fieldRequest)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"workspace/{workspaceID}/decimalfields";
		return PostJsonAsync<Int32>(route, new{workspaceID}, cancellationToken);
	}
	
	
	public Task UpdateDecimalFieldAsync(Int32 workspaceID, Int32 fieldID, DecimalFieldRequest fieldRequest)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"workspace/{workspaceID}/decimalfields/{fieldID}";
		return PutAsync(route, new{fieldRequest}, cancellationToken);
	}
	
	
	public Task UpdateDecimalFieldAsync(Int32 workspaceID, Int32 fieldID, DecimalFieldRequest fieldRequest, DateTime lastModifiedOn)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"workspace/{workspaceID}/decimalfields/{fieldID}";
		return PutAsync(route, new{fieldRequest}, cancellationToken);
	}
	
	
	public Task<Int32> CreateCurrencyFieldAsync(Int32 workspaceID, CurrencyFieldRequest fieldRequest)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"workspace/{workspaceID}/currencyfields";
		return PostJsonAsync<Int32>(route, new{workspaceID}, cancellationToken);
	}
	
	
	public Task UpdateCurrencyFieldAsync(Int32 workspaceID, Int32 fieldID, CurrencyFieldRequest fieldRequest)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"workspace/{workspaceID}/currencyfields/{fieldID}";
		return PutAsync(route, new{fieldRequest}, cancellationToken);
	}
	
	
	public Task UpdateCurrencyFieldAsync(Int32 workspaceID, Int32 fieldID, CurrencyFieldRequest fieldRequest, DateTime lastModifiedOn)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"workspace/{workspaceID}/currencyfields/{fieldID}";
		return PutAsync(route, new{fieldRequest}, cancellationToken);
	}
	
	
	public Task<Int32> CreateSingleChoiceFieldAsync(Int32 workspaceID, SingleChoiceFieldRequest fieldRequest)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"workspace/{workspaceID}/singlechoicefields";
		return PostJsonAsync<Int32>(route, new{workspaceID}, cancellationToken);
	}
	
	
	public Task UpdateSingleChoiceFieldAsync(Int32 workspaceID, Int32 fieldID, SingleChoiceFieldRequest fieldRequest)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"workspace/{workspaceID}/singlechoicefields/{fieldID}";
		return PutAsync(route, new{fieldRequest}, cancellationToken);
	}
	
	
	public Task UpdateSingleChoiceFieldAsync(Int32 workspaceID, Int32 fieldID, SingleChoiceFieldRequest fieldRequest, DateTime lastModifiedOn)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"workspace/{workspaceID}/singlechoicefields/{fieldID}";
		return PutAsync(route, new{fieldRequest}, cancellationToken);
	}
	
	
	public Task<Int32> CreateMultipleChoiceFieldAsync(Int32 workspaceID, MultipleChoiceFieldRequest fieldRequest)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"workspace/{workspaceID}/multiplechoicefields";
		return PostJsonAsync<Int32>(route, new{workspaceID}, cancellationToken);
	}
	
	
	public Task UpdateMultipleChoiceFieldAsync(Int32 workspaceID, Int32 fieldID, MultipleChoiceFieldRequest fieldRequest)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"workspace/{workspaceID}/multiplechoicefields/{fieldID}";
		return PutAsync(route, new{fieldRequest}, cancellationToken);
	}
	
	
	public Task UpdateMultipleChoiceFieldAsync(Int32 workspaceID, Int32 fieldID, MultipleChoiceFieldRequest fieldRequest, DateTime lastModifiedOn)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"workspace/{workspaceID}/multiplechoicefields/{fieldID}";
		return PutAsync(route, new{fieldRequest}, cancellationToken);
	}
	
	
	public Task<Int32> CreateSingleObjectFieldAsync(Int32 workspaceID, SingleObjectFieldRequest fieldRequest)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"workspace/{workspaceID}/singleobjectfields";
		return PostJsonAsync<Int32>(route, new{workspaceID}, cancellationToken);
	}
	
	
	public Task UpdateSingleObjectFieldAsync(Int32 workspaceID, Int32 fieldID, SingleObjectFieldRequest fieldRequest)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"workspace/{workspaceID}/singleobjectfields/{fieldID}";
		return PutAsync(route, new{fieldRequest}, cancellationToken);
	}
	
	
	public Task UpdateSingleObjectFieldAsync(Int32 workspaceID, Int32 fieldID, SingleObjectFieldRequest fieldRequest, DateTime lastModifiedOn)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"workspace/{workspaceID}/singleobjectfields/{fieldID}";
		return PutAsync(route, new{fieldRequest}, cancellationToken);
	}
	
	
	public Task<Int32> CreateMultipleObjectFieldAsync(Int32 workspaceID, MultipleObjectFieldRequest fieldRequest)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"workspace/{workspaceID}/multipleobjectfields";
		return PostJsonAsync<Int32>(route, new{workspaceID}, cancellationToken);
	}
	
	
	public Task UpdateMultipleObjectFieldAsync(Int32 workspaceID, Int32 fieldID, MultipleObjectFieldRequest fieldRequest)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"workspace/{workspaceID}/multipleobjectfields/{fieldID}";
		return PutAsync(route, new{fieldRequest}, cancellationToken);
	}
	
	
	public Task UpdateMultipleObjectFieldAsync(Int32 workspaceID, Int32 fieldID, MultipleObjectFieldRequest fieldRequest, DateTime lastModifiedOn)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"workspace/{workspaceID}/multipleobjectfields/{fieldID}";
		return PutAsync(route, new{fieldRequest}, cancellationToken);
	}
	
	
	public Task<List<ObjectTypeIdentifier>> GetAvailableSingleAssociativeObjectTypesAsync(Int32 workspaceID, ObjectTypeIdentifier objectType)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"workspace/{workspaceID}/associativefields/availablesingleassociativeobjecttypes";
		return PostJsonAsync<List<ObjectTypeIdentifier>>(route, new{workspaceID}, cancellationToken);
	}
	
	
	public Task<List<ObjectTypeIdentifier>> GetAvailableMultiAssociativeObjectTypesAsync(Int32 workspaceID, ObjectTypeIdentifier objectType)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"workspace/{workspaceID}/associativefields/availablemultiassociativeobjecttypes";
		return PostJsonAsync<List<ObjectTypeIdentifier>>(route, new{workspaceID}, cancellationToken);
	}
	
	
	public Task<List<DisplayableObjectIdentifier>> GetAvailablePropagateToFieldsAsync(Int32 workspaceID)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"workspace/{workspaceID}/associativefields/availablepropagatetofields";
		return PostJsonAsync<List<DisplayableObjectIdentifier>>(route, new{workspaceID}, cancellationToken);
	}
	
	
	public Task<List<String>> GetValidKeys(Int32 workspaceID)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"workspace/{workspaceID}/associativefields/keys";
		return GetJsonAsync<List<String>>(route, cancellationToken);
	}
	
	
	public Task<List<Securable<DisplayableObjectIdentifier>>> GetAvailableObjectTypeViewsAsync(Int32 workspaceID, ObjectTypeIdentifier objectType)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"workspace/{workspaceID}/associativefields/availableobjecttypeviews";
		return PostJsonAsync<List<Securable<DisplayableObjectIdentifier>>>(route, new{workspaceID}, cancellationToken);
	}
	
	
	public Task<List<KeyboardShortcut>> GetAvailableKeyboardShortcutsAsync(Int32 workspaceID)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"workspace/{workspaceID}/associativefields/availablekeyboardshortcuts";
		return PostJsonAsync<List<KeyboardShortcut>>(route, new{workspaceID}, cancellationToken);
	}
	
	
	public Task<List<RelationalFieldOrder>> GetRelationalOrderAsync(Int32 workspaceID)
	{
		var cancellationToken = CancellationToken.None;
		var route = $"workspace/{workspaceID}/associativefields/availablerelationalorder";
		return GetJsonAsync<List<RelationalFieldOrder>>(route, cancellationToken);
	}
	
}


}
