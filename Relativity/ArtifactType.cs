namespace Reductech.EDR.Connectors.Relativity
{

/// <summary>
/// Relativity Artifact Type
/// </summary>
public enum ArtifactType
{
    //https://platform.relativity.com/RelativityOne/index.htm#../Subsystems/rsapiclasses/Content/html/T_kCura_Relativity_Client_ArtifactType.htm


    User = 2,
    Group = 3,
    View = 4,
    Client = 5,
    Matter = 6,
    Code = 7,
    /// <summary>
    /// Case or Workspace
    /// </summary>
    Case = 8,
    Folder = 9,
    Document = 10,
    
    Field = 14,
    Search = 15,
    Layout = 16,
    Production = 17,
    Error = 18,
    MarkupSet = 22,
    Tab = 23,
    BatchSet = 24,
    ObjectType = 25,
    SearchContainer = 26,
    Batch = 27,
    RelativityScript = 28,
    SearchIndex = 29,
    ResourcePool = 31,
    ResourceServer = 32,
    InstanceSetting = 42,
    Credential = 43,
}

}
