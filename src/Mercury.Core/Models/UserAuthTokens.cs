namespace Mercury.Core.Models;

public class CookieAuthTokens
{
    // Required
    public string SAPISID { get; init; } = string.Empty;
    public string Secure3PAPISID { get; init; } = string.Empty;
    public string Secure3PSID { get; init; } = string.Empty;

    // Important
    public string SID { get; init; } = string.Empty;
    public string HSID { get; init; } = string.Empty;
    public string SSID { get; init; } = string.Empty;
    public string SIDCC { get; init; } = string.Empty;
    public string LoginInfo { get; init; } = string.Empty;
    public string Secure1PSID { get; init; } = string.Empty;
    public string Secure1PAPISID { get; init; } = string.Empty;
    public string Secure1PSIDCC { get; init; } = string.Empty;
    public string Secure3PSIDCC { get; init; } = string.Empty;
    public string Secure1PSIDTS { get; init; } = string.Empty;
    public string Secure3PSIDTS { get; init; } = string.Empty;

    // Optional
    public string APISID { get; init; } = string.Empty;
    public string VisitorInfo { get; init; } = string.Empty;

    // Raw cookie string for requests
    public string RawCookies { get; init; } = string.Empty;
}
