using System.Text.Json;

namespace Mercury.Core.Network;

internal static class OAuthHelper
{
    internal static class GoogleOAuthConstants
    {
        public const string ClientId = "861556708454-d6dlm3lh05idd8npek18k6be8ba3oc68.apps.googleusercontent.com";
        public const string ClientSecret = "SboVhoG9s0rNafixCSGGKXAT";
        public const string Scope = "https://www.googleapis.com/auth/youtube";

        public const string DeviceCodeUrl = "https://www.youtube.com/o/oauth2/device/code";
        public const string TokenUrl = "https://www.youtube.com/o/oauth2/token";
    }
    
    public sealed record TokenResponse(string AccessToken, int ExpiresIn, string? RefreshToken);

    public static async Task<TokenResponse> RefreshAsync(string refreshToken, CancellationToken ct)
    {
        var form = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["client_id"]     = GoogleOAuthConstants.ClientId,
            ["client_secret"] = GoogleOAuthConstants.ClientSecret,
            ["refresh_token"] = refreshToken,
            ["grant_type"]    = "refresh_token",
        });

        var resp = await RequestHandler.Client.PostAsync(GoogleOAuthConstants.TokenUrl, form, ct);
        var json = await resp.Content.ReadAsStringAsync(ct);
        if (!resp.IsSuccessStatusCode)
            throw new HttpRequestException($"Refresh failed: {resp.StatusCode} {json}");

        using var doc = JsonDocument.Parse(json);
        return new TokenResponse(
            doc.RootElement.GetProperty("access_token").GetString()!,
            doc.RootElement.GetProperty("expires_in").GetInt32(),
            doc.RootElement.TryGetProperty("refresh_token", out var rt) ? rt.GetString() : null);
    }
}