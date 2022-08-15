using Relativity.Kepler.Services;
using Relativity.Services.Field.Models;
using Relativity.Services.Interfaces.Field.Models;
using Relativity.Services.Interfaces.Shared;
using Relativity.Services.Interfaces.Shared.Models;
#pragma warning disable CS1591
// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming
#pragma warning disable CS8618

namespace Reductech.Sequence.Connectors.Relativity.ManagerInterfaces;

//I had to create this because ObjectType needs to be a securable object identifier

public class FixedLengthFieldRequest1 : BaseFieldRequest1
{
    /// <summary>Gets or sets the maximum length for the field.</summary>
    public int Length { get; set; }

    /// <summary>
    /// Gets or sets whether the field values are added to the workspace’s SQL text index, making the field searchable via keyword search.
    /// </summary>
    public bool IncludeInTextIndex { get; set; }

    /// <summary>Gets or sets if the field has unicode.</summary>
    public bool HasUnicode { get; set; }

    /// <summary>
    /// Gets or sets whether HTML code is allowed to execute within the field.
    /// </summary>
    public bool AllowHtml { get; set; }

    /// <summary>
    /// Gets or sets whether an object field can display its information on an associated object field.
    /// </summary>
    public bool OpenToAssociations { get; set; }

    /// <summary>Gets or sets whether the field is relational.</summary>
    public bool IsRelational { get; set; }

    /// <summary>
    /// Gets or sets a label for the relational field that users can easily understand.
    /// </summary>
    public string FriendlyName { get; set; }

    /// <summary>
    /// Gets or sets how blank values are handled when importing through the Relativity Desktop Client.
    /// </summary>
    public ImportBehavior ImportBehavior { get; set; }

    /// <summary>
    /// Gets or sets an icon for display in the Related Items pane of the core reviewer interface.
    /// </summary>
    public PaneIcon PaneIcon { get; set; }

    /// <summary>
    /// Gets or sets the pane icon order on the bottom of the Related Items pane.
    /// </summary>
    public int Order { get; set; }

    /// <summary>
    /// Gets or sets the view that appears in the Related Items pane.
    /// </summary>
    public Securable<ObjectIdentifier> RelationalView { get; set; }

    /// <summary>
    /// Gets or Sets whether the ctrl key is used in the keyboard shortcut for the field.
    /// </summary>
    public bool CTRL { get; set; }

    /// <summary>
    /// Gets or Sets whether the ctrl key is used in the keyboard shortcut for the field.
    /// </summary>
    public bool ALT { get; set; }

    /// <summary>
    /// Gets or Sets whether the ctrl key is used in the keyboard shortcut for the field.
    /// </summary>
    public bool SHIFT { get; set; }

    /// <summary>
    /// The key used along with SHIFT, ALT, and/or CTRL keys in the keyboard shortcut for the field.
    /// </summary>
    public string Key { get; set; }

    /// <summary>Gets or sets text wrapping for the field.</summary>
    public bool Wrapping { get; set; }
}

public abstract class BaseFieldRequest1
{
    /// <summary>Gets or sets the name for the field.</summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the identifier for the object type the field is associated with.
    /// </summary>
    public Securable<ObjectTypeIdentifier> ObjectType { get; set; } //NOTE This is different

    /// <summary>
    /// Gets or sets which processing field is mapped to the field.
    /// </summary>
    public string Source { get; set; }

    /// <summary>Gets or sets if the field is required.</summary>
    public bool IsRequired { get; set; }

    /// <summary>
    /// Gets or sets whether coding values should be propagated to the specified fields.
    /// </summary>
    public List<ObjectIdentifier> PropagateTo { get; set; }

    /// <summary>
    /// Gets or sets whether the field appears as a hyperlink within a list view.
    /// </summary>
    public bool IsLinked { get; set; }

    /// <summary>
    /// Gets or sets the type of filter available for the field.
    /// </summary>
    public FilterType FilterType { get; set; }

    /// <summary>
    /// Gets or sets the width (in pixels) of the column in the view.
    /// </summary>
    public int? Width { get; set; }

    /// <summary>
    /// Gets or sets if the field is available to the group by when using Pivot.
    /// </summary>
    public bool AllowGroupBy { get; set; }

    /// <summary>Gets or sets if the field is available to pivot on.</summary>
    public bool AllowPivot { get; set; }

    /// <summary>
    /// Gets or sets sorting document lists based on the field.
    /// </summary>
    public bool AllowSortTally { get; set; }

    /// <summary>
    /// Gets or sets a list of identifiers of associated Relativity Applications for the field.
    /// </summary>
    public List<ObjectIdentifier> RelativityApplications { get; set; }

    /// <summary>Gets or sets any keywords associated with the field.</summary>
    public string Keywords { get; set; }

    /// <summary>
    /// Gets or sets an optional description or other information about the field.
    /// </summary>
    public string Notes { get; set; }
}

[WebService("Field Service")]
[ServiceAudience(Audience.Public)]
[RoutePrefix("workspaces/{workspaceID:int}")]
public interface IFieldManager1 : IManager
{
    /// <summary>
    /// Retrieves extended metadata for a field, including information about additional actions available.
    /// </summary>
    /// <param name="workspaceID">The ArtifactID of the workspace where the object type should be created. Use -1 to indicate the admin workspace.</param>
    /// <param name="fieldID">The Artifact ID of the field.</param>
    /// <param name="includeMetadata">A Boolean value indicating whether to return extended field metadata in the response.</param>
    /// <param name="includeActions">A Boolean value indicating whether to return a list of operations available to the current user of this field.</param>
    /// <returns></returns>
    [HttpGet]
    [Route("fields/fixed-length/{fieldID:int}/{includeMetadata:bool}/{includeActions:bool}")]
    Task<FieldResponse> ReadAsync(
        int workspaceID,
        int fieldID,
        bool includeMetadata,
        bool includeActions);

    /// <summary>Deletes a field.</summary>
    /// <param name="workspaceID">The ArtifactID of the workspace where the object type should be deleted. Use -1 to indicate the admin workspace.</param>
    /// <param name="fieldID">The Artifact ID of the field.</param>
    /// <returns></returns>
    [HttpDelete]
    [Route("fields/fixed-length/{fieldID:int}")]
    Task DeleteAsync(int workspaceID, int fieldID);

    /// <summary>Adds a Fixed Length field to Relativity.</summary>
    /// <param name="workspaceID">The ArtifactID of the workspace where the Fixed Length field should be created. Use -1 to indicate the admin workspace.</param>
    /// <param name="fieldRequest">The data about the Fixed Length field being created.</param>
    /// <returns></returns>
    [HttpPost]
    [Route("fields/fixed-length")]
    Task<int> CreateFixedLengthFieldAsync(int workspaceID, [JsonParameter] FixedLengthFieldRequest1 fieldRequest);

    /// <summary>Modifies the properties of a Fixed Length field.</summary>
    /// <param name="workspaceID">The ArtifactID of the workspace where the Fixed Length field should be created. Use -1 to indicate the admin workspace.</param>
    /// <param name="fieldID">The Artifact ID of the Fixed Length field.</param>
    /// <param name="fieldRequest">The data about the Fixed Length field being updated.</param>
    /// <returns></returns>
    [HttpPut]
    [Route("fields/fixed-length/{fieldID:int}")]
    Task UpdateFixedLengthFieldAsync(
        int workspaceID,
        int fieldID,
        [JsonParameter] FixedLengthFieldRequest1 fieldRequest);

    /// <summary>
    /// Gets a list of parent object types for field creation.
    /// </summary>
    /// <param name="workspaceID">The ArtifactID of the workspace to view all the available parent object types. Use -1 to indicate the admin workspace.</param>
    /// <returns>All parent object types available for field creation.</returns>
    [HttpGet]
    [Route("fields/available-object-types")]
    Task<List<ObjectTypeIdentifier>> GetAvailableObjectTypesAsync(
        int workspaceID);
}
