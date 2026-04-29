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

public class UserService
{
    // Auth
    public CookieAuthTokens? CurrentAuthTokens { get; private set; }
    public bool IsAuthenticated => CurrentAuthTokens is not null;

    public void SetTokens(string cookies) => SetTokens(AuthParser.Parse(cookies));
    public void SetTokens(CookieAuthTokens tokens) => CurrentAuthTokens = tokens;
    public void ClearTokens() => CurrentAuthTokens = null;

    internal string GenerateSapiSidHash(string origin = "https://music.youtube.com")
    {
        if (CurrentAuthTokens is null || string.IsNullOrEmpty(CurrentAuthTokens.SAPISID))
            throw new InvalidOperationException("No auth tokens set.");

        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var input = $"{timestamp} {CurrentAuthTokens.SAPISID} {origin}";
        var hash = Convert.ToHexStringLower(
            SHA1.HashData(Encoding.UTF8.GetBytes(input)));

        return $"SAPISIDHASH {timestamp}_{hash} " +
               $"SAPISID1PHASH {timestamp}_{hash} " +
               $"SAPISID3PHASH {timestamp}_{hash}";
    }

    
    // User data
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

        Collection<LibraryPlaylist> playlists = [];
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

    public async Task<Collection<Song>> GetLikedSongsAsync(CancellationToken cToken = default)
    {
        // browse endpoint for liked songs
        return null;
    }
}