using System.Diagnostics;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using Mercury.Core.Json.Parsers.User;
using Mercury.Core.Network;

namespace Mercury.Core.Models;

public interface IAuthSource
{
    void ModifyHeader(HttpRequestHeaders headers);
}


public class AnonymousAuthSource : IAuthSource
{
    public static AnonymousAuthSource Instance { get; } = new AnonymousAuthSource();
    public void ModifyHeader(HttpRequestHeaders _){}
}

public class OAuthSource(string accessToken, string refreshToken, DateTimeOffset expiresAt)
    : IAuthSource
{
    private readonly CancellationTokenSource _cts = new();
    private Task? _refreshLoop;
    private readonly TimeSpan _margin = TimeSpan.FromMinutes(5);
    
    public event Action<OAuthSource>? Refreshed;
    
    public string AccessToken { get; set; } = accessToken;
    public string RefreshToken { get; set; } = refreshToken;
    public DateTimeOffset ExpiresAt { get; private set; } = expiresAt;


    private async Task LoopAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            // Sleep until shortly before expiry, never longer than 1h.
            var delay = ExpiresAt - DateTimeOffset.UtcNow - _margin;
            if (delay < TimeSpan.Zero) delay = TimeSpan.Zero;
            if (delay > TimeSpan.FromHours(1)) delay = TimeSpan.FromHours(1);

            try { await Task.Delay(delay, ct); }
            catch (OperationCanceledException) { return; }

            try
            {
                var fresh = await OAuthHelper.RefreshAsync(RefreshToken, ct);
                AccessToken = fresh.AccessToken;
                ExpiresAt = DateTimeOffset.UtcNow.AddSeconds(fresh.ExpiresIn);
                // RefreshToken stays the same
                
                Refreshed?.Invoke(this);
            }
            catch (Exception ex)
            {
                // Log the error and wait before re-trying
                Debug.WriteLine($"[OAuth] Refresh failed: {ex.Message}");
                try { await Task.Delay(TimeSpan.FromSeconds(30), ct); }
                catch (OperationCanceledException) { return; }
            }
        }
    }
    
    
    public void ModifyHeader(HttpRequestHeaders headers)
    {
        headers.TryAddWithoutValidation("Authorization", $"Bearer {this.AccessToken}");
    }
}

public class CookieAuthSource(IReadOnlyDictionary<string, string> cookies) : IAuthSource
{
    private static readonly string[] RequiredNames =
        ["SAPISID", "__Secure-3PAPISID", "__Secure-3PSID", "SID"];

    public IReadOnlyDictionary<string, string> Cookies { get; private set; } = cookies;

    public string this[string name] =>
        Cookies.TryGetValue(name, out var v) ? v : string.Empty;

    public string SAPISID => this["SAPISID"];
    public string Secure1PAPISID => this["__Secure-1PAPISID"];
    public string Secure3PAPISID => this["__Secure-3PAPISID"];
    public string Secure3PSID => this["__Secure-3PSID"];
    public string SID => this["SID"];

    public bool IsValid => RequiredNames.All(n => !string.IsNullOrEmpty(this[n]));


    public void SetAuth(string cookies)
    {
        Cookies = AuthParser.Parse(cookies);
    }
    
    public void ModifyHeader(HttpRequestHeaders headers)
    {
        var header = string.Join("; ", Cookies.Select(kv => $"{kv.Key}={kv.Value}"));
        headers.TryAddWithoutValidation("Authorization", GenerateSapiSidHash());
        headers.Add("Cookie", header);
        headers.Add("X-Goog-AuthUser", "0");
    }
    
    private string GenerateSapiSidHash(string origin = "https://music.youtube.com")
    {
        var ts = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        string Hash(string secret) =>
            Convert.ToHexString(SHA1.HashData(
                Encoding.UTF8.GetBytes($"{ts} {secret} {origin}"))).ToLowerInvariant();

        var sapi  = !string.IsNullOrEmpty(SAPISID)        ? Hash(SAPISID)        : null;
        var sapi1 = !string.IsNullOrEmpty(Secure1PAPISID) ? Hash(Secure1PAPISID) : sapi;
        var sapi3 = !string.IsNullOrEmpty(Secure3PAPISID) ? Hash(Secure3PAPISID) : sapi;

        return $"SAPISIDHASH {ts}_{sapi} SAPISID1PHASH {ts}_{sapi1} SAPISID3PHASH {ts}_{sapi3}";
    }
}