using Mercury.Core.Models;

namespace Mercury.Core.Json.Parsers;

internal static class AuthParser
{
    public static CookieAuthTokens Parse(string cookies)
    {
        var parts = cookies.Replace(" ", "").Split(';');
        var cookieDict = new Dictionary<string, string>();

        foreach (var part in parts)
        {
            var pairs = part.Split('=', 2);
            if (pairs.Length == 2)
                cookieDict[pairs[0]] = pairs[1];
        }

        return new CookieAuthTokens
        {
            SAPISID = cookieDict.GetValueOrDefault("SAPISID", ""),
            Secure3PAPISID = cookieDict.GetValueOrDefault("__Secure-3PAPISID", ""),
            Secure3PSID = cookieDict.GetValueOrDefault("__Secure-3PSID", ""),
            SID = cookieDict.GetValueOrDefault("SID", ""),
            HSID = cookieDict.GetValueOrDefault("HSID", ""),
            SSID = cookieDict.GetValueOrDefault("SSID", ""),
            SIDCC = cookieDict.GetValueOrDefault("SIDCC", ""),
            LoginInfo = cookieDict.GetValueOrDefault("LOGIN_INFO", ""),
            Secure1PSID = cookieDict.GetValueOrDefault("__Secure-1PSID", ""),
            Secure1PAPISID = cookieDict.GetValueOrDefault("__Secure-1PAPISID", ""),
            Secure1PSIDCC = cookieDict.GetValueOrDefault("__Secure-1PSIDCC", ""),
            Secure3PSIDCC = cookieDict.GetValueOrDefault("__Secure-3PSIDCC", ""),
            Secure1PSIDTS = cookieDict.GetValueOrDefault("__Secure-1PSIDTS", ""),
            Secure3PSIDTS = cookieDict.GetValueOrDefault("__Secure-3PSIDTS", ""),
            APISID = cookieDict.GetValueOrDefault("APISID", ""),
            VisitorInfo = cookieDict.GetValueOrDefault("VISITOR_INFO1_LIVE", ""),
            FullCookies = cookies
        };
    }
}