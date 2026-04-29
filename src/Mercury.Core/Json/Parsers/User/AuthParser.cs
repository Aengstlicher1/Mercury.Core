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
        var cookies = PrepareHeader(headerString);
        var cleanHeader = BuildCleanHeader(cookies);

        return new CookieAuthTokens
        {
            SAPISID          = Get("SAPISID"),
            Secure3PAPISID   = Get("__Secure-3PAPISID"),
            Secure3PSID      = Get("__Secure-3PSID"),
            SID              = Get("SID"),
            HSID             = Get("HSID"),
            SSID             = Get("SSID"),
            SIDCC            = Get("SIDCC"),
            LoginInfo        = Get("LOGIN_INFO"),
            Secure1PSID      = Get("__Secure-1PSID"),
            Secure1PAPISID   = Get("__Secure-1PAPISID"),
            Secure1PSIDCC    = Get("__Secure-1PSIDCC"),
            Secure3PSIDCC    = Get("__Secure-3PSIDCC"),
            Secure1PSIDTS    = Get("__Secure-1PSIDTS"),
            Secure3PSIDTS    = Get("__Secure-3PSIDTS"),
            APISID           = Get("APISID"),
            VisitorInfo      = Get("VISITOR_INFO1_LIVE"),
            RawCookies = cleanHeader
        };

        string Get(string key) => cookies.GetValueOrDefault(key, string.Empty);
    }

    private static string BuildCleanHeader(Dictionary<string, string> cookies) =>
        string.Join("; ", WantedCookies
            .Where(cookies.ContainsKey)
            .Select(key => $"{key}={cookies[key]}"));
    
    private static Dictionary<string, string> PrepareHeader(string header)
    {
        var dict = new Dictionary<string, string>();

        foreach (var part in header.Replace(" ", "").Split(';'))
        {
            var trimmed = part.Trim();
            if (string.IsNullOrEmpty(trimmed)) continue;

            var eqIndex = trimmed.IndexOf('=');
            if (eqIndex < 0) continue; // Skip malformed entries

            var key = trimmed[..eqIndex].Trim();
            var value = trimmed[(eqIndex + 1)..].Trim();

            dict[key] = value;
        }
        
        return dict;
    }
}