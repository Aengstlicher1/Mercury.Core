using System.Collections.ObjectModel;
using System.Security.Authentication;
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
/// Provides user-specific functionality such as authentication management
/// and access to the authenticated user's YouTube Music library.
/// </summary>
public class UserService
{
    
    public IAuthSource? CurrentAuth { get; private set; } = AnonymousAuthSource.Instance;
    public bool IsAuthenticated => CurrentAuth is not AnonymousAuthSource and not null;


    public void SetAuth(IAuthSource source)
    {
        CurrentAuth = source;
    }
    
    /// <summary>
    /// Asynchronously retrieves the authenticated user's library playlists from
    /// YouTube Music.
    /// </summary>
    /// <remarks>
    /// Browses the <c>FEmusic_library_landing</c> page and parses the grid of
    /// playlist items from the first section. Requires the user to be authenticated.
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
            throw new AuthenticationException("Not authenticated");

        Dictionary<string, object?> payload = new()
        {
            ["browseId"] = "FEmusic_library_landing"
        };

        var response = await RequestHandler.PostAsync(Endpoints.Browse, payload, ClientType.WebMusic, cToken);

        cToken.ThrowIfCancellationRequested();

        using IDisposable _ = response.GetJson(out var json);

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
            .UnlessNull(JArray.Empty);

        Collection<Playlist> playlists = [];

        foreach (var item in items)
        {
            playlists.Add(PlaylistParser.Parse(item.Get("musicTwoRowItemRenderer")));
        }

        return new UserLibrary()
        {
            Title = tabRender.Get("title").AsString().UnlessNull(string.Empty),
            Playlists = playlists
        };
    }

    /// <summary>
    /// Asynchronously retrieves the authenticated user's liked songs playlist.
    /// </summary>
    /// <param name="cToken"><see cref="CancellationToken"/> to cancel the request</param>
    /// <returns>
    /// A <see cref="Playlist"/> representing the liked songs
    /// </returns>
    public async Task<Playlist?> GetLikedSongsAsync(CancellationToken cToken = default)
    {
        // TODO: implement browse endpoint for liked songs
        return null;
    }
}
