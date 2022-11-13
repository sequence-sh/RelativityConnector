namespace Sequence.Connectors.Relativity.Steps;
#pragma warning disable CS1591
public enum UpdateBehaviour
{
    /// <summary>
    /// adds the values that you pass into the service to the current values for the choice or object field
    /// </summary>
    Merge,
    
    Remove,
    
    /// <summary>
    /// overwrites the current values for the choice or object field with those that you pass into the service.
    /// </summary>
    Replace
}
