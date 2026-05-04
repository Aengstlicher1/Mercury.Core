using System.Collections.ObjectModel;
using System.Security.Cryptography;
using System.Text;
using Mercury.Core.Json;
using Mercury.Core.Json.Parsers;
using Mercury.Core.Json.Parsers.User;
using Mercury.Core.Models;
using Mercury.Core.Models.User;
using Mercury.Core.Network;
using Mercury.Core.Utils;

namespace Mercury.Core.Services;

/// <summary>
/// Provides user-specific functionality such as authentication token management
/// and access to the authenticated user's YouTube Music library.
/// </summary>
/// <remarks>
/// Authentication is cookie-based. Call <see cref="SetTokens(string)"/> or
/// <see cref="SetTokens(CookieAuthTokens)"/> before invoking any method that
/// requires authentication. Check <see cref="IsAuthenticated"/> before making
/// authenticated requests.
/// </remarks>
public class UserService
{
    // \\// Authentication \\//

    /// <summary>
    /// Gets the currently active authentication tokens, or <see langword="null"/>
    /// if no tokens have been set.
    /// </summary>
    public CookieAuthTokens? CurrentAuthTokens { get; private set; }

    /// <summary>
    /// Gets a value indicating whether valid authentication tokens are currently set.
    /// </summary>
    public bool IsAuthenticated => CurrentAuthTokens is not null;

    /// <summary>
    /// Parses a raw cookie string and stores the resulting tokens as the active
    /// authentication credentials.
    /// </summary>
    /// <param name="cookies">The raw cookie header string to parse</param>
    public void SetTokens(string cookies) => SetTokens(AuthParser.Parse(cookies));

    /// <summary>
    /// Stores the given <paramref name="tokens"/> as the active authentication
    /// credentials.
    /// </summary>
    /// <param name="tokens">The <see cref="CookieAuthTokens"/> to store</param>
    public void SetTokens(CookieAuthTokens tokens) => CurrentAuthTokens = tokens;

    /// <summary>
    /// Clears the currently stored authentication tokens, effectively signing out.
    /// </summary>
    public void ClearTokens() => CurrentAuthTokens = null;

    /// <summary>
    /// Generates a SAPISIDHASH authorization header value from the current
    /// <see cref="CookieAuthTokens.SAPISID"/> token.
    /// </summary>
    /// <remarks>
    /// The hash is computed as <c>SHA1("{timestamp} {SAPISID} {origin}")</c> and
    /// returned as a space-separated string containing the SAPISIDHASH,
    /// SAPISID1PHASH, and SAPISID3PHASH variants, as required by the YouTube
    /// Music API.
    /// </remarks>
    /// <param name="origin">
    /// The origin URL used in the hash input. Defaults to
    /// <c>https://music.youtube.com</c>.
    /// </param>
    /// <returns>
    /// A formatted authorization header value containing all three SAPISID hash
    /// variants
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// When no auth tokens are set or <see cref="CookieAuthTokens.SAPISID"/> is
    /// null or empty
    /// </exception>
    internal string GenerateSapiSidHash(string origin = "https://music.youtube.com")
    {
        if (CurrentAuthTokens is null || string.IsNullOrEmpty(CurrentAuthTokens.SAPISID))
            throw new InvalidOperationException("No auth tokens set.");

        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var input = $"{timestamp} {CurrentAuthTokens.SAPISID} {origin}";
        var hash = Convert.ToHexString(
            SHA1.HashData(Encoding.UTF8.GetBytes(input))).ToLower();

        return $"SAPISIDHASH {timestamp}_{hash} "
             + $"SAPISID1PHASH {timestamp}_{hash} "
             + $"SAPISID3PHASH {timestamp}_{hash}";
    }

    // \\// User data \\//

    /// <summary>
    /// Asynchronously retrieves the authenticated user's library playlists from
    /// YouTube Music.
    /// </summary>
    /// <remarks>
    /// Browses the <c>FEmusic_library_landing</c> page and parses the grid of
    /// playlist items from the first section. Requires the user to be authenticated
    /// via <see cref="SetTokens(string)"/> or <see cref="SetTokens(CookieAuthTokens)"/>.
    /// </remarks>
    /// <param name="cToken"><see cref="CancellationToken"/> to cancel the request</param>
    /// <returns>
    /// A <see cref="UserLibrary"/> containing the library title and a collection
    /// of the user's <see cref="Playlist"/> items
    /// </returns>
    /// <exception cref="InvalidOperationException">When the user is not authenticated</exception>
    public async Task<UserLibrary> GetLibraryPlaylistsAsync(CancellationToken cToken = default)
    {
        if (!IsAuthenticated)
            throw new InvalidOperationException("No authentication");

        Dictionary<string, object?> payload = new()
        {
            ["browseId"] = "FEmusic_library_landing"
        };

        var response = await RequestHandler.PostAsync(Endpoints.Browse, payload, ClientType.WebMusic, cToken);

        cToken.ThrowIfCancellationRequested();

        using IDisposable _ = response.ParseJson(out var json);

        var tabRender = json
            .Get("contents")
            .Get("singleColumnBrowseResultsRenderer")
            .Get("tabs")
            .GetAt(0)
            .Get("tabRenderer");

        var items = tabRender
            .Get("content")
            .Get("sectionListRenderer")
            .Get("contents")
            .GetAt(0)
            .Get("gridRenderer")
            .Get("items")
            .AsArray()
            .Or(JArray.Empty);

        Collection<Playlist> playlists = [];

        foreach (var item in items)
        {
            playlists.Add(PlaylistParser.Parse(item.Get("musicTwoRowItemRenderer")));
        }

        return new UserLibrary()
        {
            Title = tabRender.Get("title").AsString().Or(string.Empty),
            Playlists = playlists
        };
    }

    /// <summary>
    /// Asynchronously retrieves the authenticated user's liked songs playlist.
    /// </summary>
    /// <param name="cToken"><see cref="CancellationToken"/> to cancel the request</param>
    /// <returns>
    /// A <see cref="Playlist"/> representing the liked songs, or
    /// <see langword="null"/> if not yet implemented
    /// </returns>
    public async Task<Playlist?> GetLikedSongsAsync(CancellationToken cToken = default)
    {
        // TODO: implement browse endpoint for liked songs
        return null;
    }
}
