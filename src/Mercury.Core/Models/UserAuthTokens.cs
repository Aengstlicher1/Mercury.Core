namespace Mercury.Core.Models;

public sealed record CookieAuthTokens(IReadOnlyDictionary<string, string> Cookies)
{
    private static readonly string[] RequiredNames =
        ["SAPISID", "__Secure-3PAPISID", "__Secure-3PSID"];

    public string this[string name] =>
        Cookies.TryGetValue(name, out var v) ? v : string.Empty;

    public string SAPISID => this["SAPISID"];
    public string Secure3PAPISID => this["__Secure-3PAPISID"];
    public string Secure3PSID => this["__Secure-3PSID"];
    public string SID => this["SID"];

    public bool IsValid => RequiredNames.All(n => !string.IsNullOrEmpty(this[n]));

    public string ToCookieHeader() =>
        string.Join("; ", Cookies.Select(kv => $"{kv.Key}={kv.Value}"));

    // Value-equality over the dictionary contents (records compare by reference for IDictionary)
    public bool Equals(CookieAuthTokens? other) =>
        other is not null &&
        Cookies.Count == other.Cookies.Count &&
        Cookies.All(kv => other.Cookies.TryGetValue(kv.Key, out var v) && v == kv.Value);

    public override int GetHashCode()
    {
        var hc = new HashCode();
        foreach (var kv in Cookies.OrderBy(k => k.Key, StringComparer.Ordinal))
        {
            hc.Add(kv.Key);
            hc.Add(kv.Value);
        }
        return hc.ToHashCode();
    }
}