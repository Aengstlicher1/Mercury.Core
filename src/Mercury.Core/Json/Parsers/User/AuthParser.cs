using System.Collections.ObjectModel;
using Mercury.Core.Models;

namespace Mercury.Core.Json.Parsers.User;

internal static class AuthParser
{
    private static readonly Collection<string> WantedCookies = 
    [
        "SAPISID",
        "__Secure-3PAPISID",
        "__Secure-3PSID",
        "SID",
        "HSID",
        "SSID",
        "SIDCC",
        "LOGIN_INFO",
        "__Secure-1PSID",
        "__Secure-1PAPISID",
        "__Secure-1PSIDCC",
        "__Secure-3PSIDCC",
        "__Secure-1PSIDTS",
        "__Secure-3PSIDTS",
        "APISID",
        "VISITOR_INFO1_LIVE"
    ];

    /// <summary>
    /// Takes a raw Cookie header string and turns it into a usable format
    /// </summary>
    /// <param name="headerString">The raw cookie header string</param>
    /// <returns>A <see cref="CookieAuthTokens"/> object, which contains all needed and optional cookies.</returns>
    public static CookieAuthTokens Parse(string headerString)
    {
        var cookies = new Dictionary<string, string>();

        foreach (var part in headerString.Replace(" ", "").Split(';'))
        {
            var trimmed = part.Trim();
            if (string.IsNullOrEmpty(trimmed)) continue;

            var eqIndex = trimmed.IndexOf('=');
            if (eqIndex < 0) continue; // Skip malformed entries

            var key = trimmed[..eqIndex].Trim();
            var value = trimmed[(eqIndex + 1)..].Trim();

            cookies[key] = value;
        }

        return new CookieAuthTokens(cookies);
    }
}