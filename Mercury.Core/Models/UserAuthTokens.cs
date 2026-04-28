using System.Diagnostics.CodeAnalysis;

namespace Mercury.Core.Models;

public class UserAuthTokens
{
    // Required
    public string SAPISID { get; init; } = string.Empty;
    public string Secure3PAPISID { get; init; } = string.Empty;
    public string Secure3PSID { get; init; } = string.Empty;

    // Optional
    public string SID { get; init; } = string.Empty;
    public string SIDCC { get; init; } = string.Empty;
    public string APISID { get; init; } = string.Empty;
    public string Secure1PAPISID { get; init; } = string.Empty;
}