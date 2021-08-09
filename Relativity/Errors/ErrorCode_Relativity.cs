using System.Diagnostics;
using Reductech.EDR.Core.Internal.Errors;

namespace Reductech.EDR.Connectors.Relativity.Errors
{

/// <summary>
/// Error Code for Nuix
/// </summary>
// ReSharper disable once InconsistentNaming
public sealed record ErrorCode_Relativity : ErrorCodeBase
{
    /// <inheritdoc />
    private ErrorCode_Relativity(string code) : base(code) { }

    /// <summary>
    /// Request Failed with status code {0}. {1} {2}
    /// </summary>
    public static readonly ErrorCode_Relativity RequestFailed = new(nameof(RequestFailed));

    /// <summary>
    /// Failed with message {0}
    /// </summary>
    public static readonly ErrorCode_Relativity Unsuccessful = new(nameof(Unsuccessful));

    /// <summary>
    /// Missing Field: {0}
    /// </summary>
    public static readonly ErrorCode_Relativity MissingField = new(nameof(MissingField));

    /// <inheritdoc />
    public override string GetFormatString()
    {
        var s = ErrorMessages_EN.ResourceManager.GetString(Code);

        Debug.Assert(s != null, nameof(s) + " != null");
        return s;
    }
}

}
