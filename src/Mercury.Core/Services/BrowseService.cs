using System.Collections.ObjectModel;
using Mercury.Core.Json;
using Mercury.Core.Json.Parsers.Browse;
using Mercury.Core.Json.Parsers.Browse.Explore;
using Mercury.Core.Json.Parsers.Browse.Info;
using Mercury.Core.Models;
using Mercury.Core.Models.Explore;
using Mercury.Core.Network;
using Mercury.Core.Utils;
using static Mercury.Core.Models.Enums;
using AlbumParser = Mercury.Core.Json.Parsers.Browse.AlbumParser;

namespace Mercury.Core.Services
{
    /// <summary>
    /// The Browse service to get single medias from their id,
    /// media infos (e.g. <see cref="PlaylistInfo"/>)
    /// and the ExplorePage
    /// </summary>
    public sealed class BrowseService
    {
        /// <summary>
        /// Gets the current ExploreFeed from YoutubeMusic.
        /// This is a UserSpecific action, so user data will be used when available
        /// </summary>
        /// <param name="cToken"><see cref="CancellationToken"/> to cancel the request</param>
        /// <returns>The <see cref="ExploreFeed"/> with its child models</returns>
        public async Task<ExploreFeed> GetExploreFeedAsync(CancellationToken cToken = default)
        {
            Dictionary<string, object?> payload = new()
            {
                {"browseId" , "FEmusic_explore" }
            };
            
            var response = await RequestHandler.PostAsync(Endpoints.Browse, payload, ClientType.WebMusic, cToken);

            cToken.ThrowIfCancellationRequested();
            
            using IDisposable _ = response.ParseJson(out var json);

            var contents = json
                .Get("contents")
                .Get("singleColumnBrowseResultsRenderer")
                .Get("tabs")
                .GetAt(0)
                .Get("tabRenderer")
                .Get("content")
                .Get("sectionListRenderer")
                .Get("contents");
            
            return new ExploreFeed()
            {
                Releases = HandleReleases(contents.GetAt(1).Get("musicCarouselShelfRenderer"), cToken),
                Genres = HandleGenres(contents.GetAt(2).Get("musicCarouselShelfRenderer"), cToken),
                Trending = HandleTrending(contents.GetAt(3).Get("musicCarouselShelfRenderer"), cToken),
                NewMusicVideos = HandleNewMusicVideos(contents.GetAt(4).Get("musicCarouselShelfRenderer"), cToken)
            };
        }

        /// <summary>
        /// Handles the <see cref="NewMusicVideosCategory"/> parsing of the <see cref="ExploreFeed"/>
        /// </summary>
        /// <param name="renderer">The <see cref="JElement"/> of the NewMusicVideos category on the Explore page</param>
        /// <param name="cToken"><see cref="CancellationToken"/> to cancel the request</param>
        /// <returns>A <see cref="NewMusicVideosCategory"/> for the <see cref="ExploreFeed"/></returns>
        private NewMusicVideosCategory HandleNewMusicVideos(JElement renderer, CancellationToken cToken = default)
        {
            Collection<Video> videos = new();
            foreach (var video in renderer.Get("contents").AsArray().Or(JArray.Empty))
            {
                var vid = MusicVideoParser.Parse(video.Get("musicTwoRowItemRenderer"));
                if (vid != null)
                    videos.Add(vid);
            }

            return new NewMusicVideosCategory()
            {
                Name = TitleParser.Parse(renderer),
                Content = videos.ToArray()
            };
        }
        
        /// <summary>
        /// Handles the <see cref="ReleasesCategory"/> parsing of the <see cref="ExploreFeed"/>
        /// </summary>
        /// <param name="renderer">The <see cref="JElement"/> of the Releases category on the Explore page</param>
        /// <param name="cToken"><see cref="CancellationToken"/> to cancel the request</param>
        /// <returns>A <see cref="ReleasesCategory"/> for the <see cref="ExploreFeed"/></returns>
        private ReleasesCategory HandleReleases(JElement renderer, CancellationToken cToken = default)
        {
            Collection<Album> albums = new();
            foreach (var media in renderer.Get("contents").AsArray().Or(JArray.Empty))
            {
                var mRenderer = media.Get("musicTwoRowItemRenderer");
                albums.Add(Mercury.Core.Json.Parsers.Browse.Explore.AlbumParser.Parse(mRenderer));
            }
            
            return new ReleasesCategory()
            {
                Name = TitleParser.Parse(renderer),
                Content = albums.ToArray()
            };
        }
        
        /// <summary>
        /// Handles the <see cref="GenresCategory"/> parsing of the <see cref="ExploreFeed"/>
        /// </summary>
        /// <param name="renderer">The <see cref="JElement"/> of the Genres category on the Explore page</param>
        /// <param name="cToken"><see cref="CancellationToken"/> to cancel the request</param>
        /// <returns>A <see cref="GenresCategory"/> for the <see cref="ExploreFeed"/></returns>
        private GenresCategory HandleGenres(JElement renderer, CancellationToken cToken = default)
        {
            Collection<Genre> genres = new();
            foreach (var genre in renderer.Get("contents").AsArray().Or(JArray.Empty))
            {
                genres.Add(GenreParser.Parse(genre));
            }
            
            return new GenresCategory()
            {
                Name = TitleParser.Parse(renderer),
                Content = genres.ToArray()
            };
        }
        
        /// <summary>
        /// Handles the <see cref="TrendingCategory"/> parsing of the <see cref="ExploreFeed"/>
        /// </summary>
        /// <param name="renderer">The <see cref="JElement"/> of the Trending category on the Explore page</param>
        /// <param name="cToken"><see cref="CancellationToken"/> to cancel the request</param>
        /// <returns>A <see cref="TrendingCategory"/> for the <see cref="ExploreFeed"/></returns>
        private TrendingCategory HandleTrending(JElement renderer, CancellationToken cToken = default)
        {
            Collection<Track> tracks = new();

            foreach (var track in renderer.Get("contents").AsArray().Or(JArray.Empty))
            {
                tracks.Add(PlaylistInfoParser.ParsePlaylistTrack(track.Get("musicResponsiveListItemRenderer")));
            }
            
            return new TrendingCategory()
            {
                Name = TitleParser.Parse(renderer),
                Content = tracks.ToArray()
            };
        }

        /// <summary>
        /// Gets the media model from the id.
        /// </summary>
        /// <param name="id">The id string of the Media</param>
        /// <param name="cToken"><see cref="CancellationToken"/> to cancel the request</param>
        /// <returns>The <see cref="Media"/> model(the actual type of the model, inferred from the <paramref name="id"/>)</returns>
        public async Task<Media?> GetAsync(string id, CancellationToken cToken = default)
        {
            return id switch
            {
                _ when id.StartsWith("MPRE")  => await HandleBrowse(id, MediaCategory.Album, cToken),
                _ when id.StartsWith("MPSP")  => await HandleBrowse(id, MediaCategory.Podcast, cToken),
                _ when id.StartsWith("VL") 
                       || id.StartsWith("PL") => await HandleBrowse(id, MediaCategory.Playlist, cToken),
                _ when id.StartsWith("UC")    => await HandleArtists(id, cToken),
                _ when id.StartsWith("FE")    => await HandleProfiles(id, cToken),
                _ when id.Length == 11        => await HandleTrack(id, cToken),
                _ => null
            };
        }

        /// <summary>
        /// Handles the Next endpoint for getting a Media
        /// </summary>
        /// <param name="videoId">The video id of the Track</param>
        /// <param name="cToken"><see cref="CancellationToken"/> to cancel the request</param>
        /// <returns>A <see cref="Media"/> model</returns>
        /// <exception cref="ArgumentException">When the <paramref name="videoId"/> is empty or null</exception>
        private async Task<Media> HandleTrack(string videoId, CancellationToken cToken = default)
        {
            if (string.IsNullOrWhiteSpace(videoId))
                throw new ArgumentException(nameof(videoId));

            Dictionary<string, object?> payload = new()
            {
                {"videoId" , videoId }
            };

            var response = await RequestHandler.PostAsync(Endpoints.Next, payload, ClientType.WebMusic, cToken);

            cToken.ThrowIfCancellationRequested();

            using IDisposable _ = response.ParseJson(out var json);

            var renderer = json
                .Get("contents")
                .Get("singleColumnMusicWatchNextResultsRenderer")
                .Get("tabbedRenderer")
                .Get("watchNextTabbedResultsRenderer")
                .Get("tabs")
                .GetAt(0)
                .Get("tabRenderer")
                .Get("content")
                .Get("musicQueueRenderer")
                .Get("content")
                .Get("playlistPanelRenderer")
                .Get("contents")
                .GetAt(0)
                .Get("playlistPanelVideoRenderer");

            var musicVideoType = renderer
                .Get("navigationEndpoint")
                .Get("watchEndpoint")
                .Get("watchEndpointMusicSupportedConfigs")
                .Get("watchEndpointMusicConfig")
                .Get("musicVideoType")
                .AsString()
                .Or("");

            return musicVideoType == "MUSIC_VIDEO_TYPE_ATV"
                ? SongParser.Parse(renderer)
                : VideoParser.Parse(renderer);
        }

        /// <summary>
        /// Handles the Browse endpoint for getting a <see cref="Media"/> model
        /// whose type is determined by <paramref name="category"/>.
        /// </summary>
        /// <param name="browseId">The browse id of the media (e.g. an album, playlist, or podcast id)</param>
        /// <param name="category">
        /// The <see cref="MediaCategory"/> used to select the correct parser
        /// (<see cref="MediaCategory.Album"/>, <see cref="MediaCategory.Playlist"/>,
        /// or <see cref="MediaCategory.Podcast"/>)
        /// </param>
        /// <param name="cToken"><see cref="CancellationToken"/> to cancel the request</param>
        /// <returns>The parsed <see cref="Media"/> model for the given <paramref name="browseId"/></returns>
        /// <exception cref="ArgumentNullException">When <paramref name="browseId"/> is null or whitespace</exception>
        private async Task<Media> HandleBrowse(string browseId, MediaCategory category, CancellationToken cToken = default)
        {
            if (string.IsNullOrWhiteSpace(browseId))
                throw new ArgumentNullException("browseId");

            Dictionary<string, object?> payload = new()
            {
                {"browseId" , browseId }
            };

            var response = await RequestHandler.PostAsync(Endpoints.Browse, payload, ClientType.WebMusic, cToken);

            cToken.ThrowIfCancellationRequested();

            using IDisposable _ = response.ParseJson(out var json);

            var renderer = json
                .Get("contents")
                .Get("twoColumnBrowseResultsRenderer")
                .Get("tabs")
                .GetAt(0)
                .Get("tabRenderer")
                .Get("content")
                .Get("sectionListRenderer")
                .Get("contents")
                .GetAt(0)
                .Get("musicResponsiveHeaderRenderer");

            Media result = category switch 
            {
                MediaCategory.Album => AlbumParser.Parse(renderer, browseId),
                MediaCategory.Playlist => PlaylistParser.Parse(renderer, browseId),
                MediaCategory.Podcast => PodcastParser.Parse(renderer, browseId),
                _ => null!
            };

            return result;
        }

        /// <summary>
        /// Handles the Browse endpoint for getting a <see cref="Profile"/> model.
        /// </summary>
        /// <param name="browseId">The browse id of the profile (typically starts with <c>FE</c>)</param>
        /// <param name="cToken"><see cref="CancellationToken"/> to cancel the request</param>
        /// <returns>The parsed <see cref="Profile"/> model for the given <paramref name="browseId"/></returns>
        /// <exception cref="ArgumentNullException">When <paramref name="browseId"/> is null or whitespace</exception>
        private async Task<Profile> HandleProfiles(string browseId, CancellationToken cToken = default)
        {
            if (string.IsNullOrWhiteSpace(browseId))
                throw new ArgumentNullException("browseId");

            Dictionary<string, object?> payload = new()
            {
                {"browseId" , browseId }
            };

            var response = await RequestHandler.PostAsync(Endpoints.Browse, payload, ClientType.WebMusic, cToken);

            cToken.ThrowIfCancellationRequested();

            using IDisposable _ = response.ParseJson(out var json);

            var renderer = json
                .Get("header")
                .Get("musicVisualHeaderRenderer");

            return ProfileParser.Parse(renderer, browseId);
        }

        /// <summary>
        /// Handles the Browse endpoint for getting an <see cref="Artist"/> model.
        /// </summary>
        /// <param name="browseId">The browse id of the artist channel (typically starts with <c>UC</c>)</param>
        /// <param name="cToken"><see cref="CancellationToken"/> to cancel the request</param>
        /// <returns>The parsed <see cref="Artist"/> model for the given <paramref name="browseId"/></returns>
        /// <exception cref="ArgumentNullException">When <paramref name="browseId"/> is null or whitespace</exception>
        private async Task<Artist> HandleArtists(string browseId, CancellationToken cToken = default)
        {
            if (string.IsNullOrWhiteSpace(browseId))
                throw new ArgumentNullException("browseId");

            Dictionary<string, object?> payload = new()
            {
                {"browseId" , browseId }
            };

            var response = await RequestHandler.PostAsync(Endpoints.Browse, payload, ClientType.WebMusic, cToken);

            cToken.ThrowIfCancellationRequested();

            using IDisposable _ = response.ParseJson(out var json);

            var renderer = json
                .Get("header")
                .Get("musicImmersiveHeaderRenderer");

            return ArtistParser.Parse(renderer, browseId);
        }

        /// <summary>
        /// Handles both the Next and Browse endpoints to build a fully populated
        /// <see cref="Episode"/> model.
        /// </summary>
        /// <remarks>
        /// Two sequential requests are made: one to the Next endpoint (for playback
        /// metadata) and one to the Browse endpoint using the prefixed id
        /// <c>MPED{id}</c> (for episode details). Both responses are combined by
        /// <see cref="EpisodeParser"/>.
        /// </remarks>
        /// <param name="id">The video id of the podcast episode (11 characters)</param>
        /// <param name="cToken"><see cref="CancellationToken"/> to cancel the request</param>
        /// <returns>The parsed <see cref="Episode"/> model</returns>
        /// <exception cref="ArgumentNullException">When <paramref name="id"/> is null or whitespace</exception>
        private async Task<Episode> HandleEpisode(string id, CancellationToken cToken = default)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentNullException("id");

            Dictionary<string, object?> nextPayload = new()
            {
                {"videoId" , id }
            };
            Dictionary<string, object?> browsePayload = new()
            {
                {"browseId" , "MPED" + id }
            };

            var nextResponse = await RequestHandler.PostAsync(Endpoints.Next, nextPayload, ClientType.WebMusic, cToken);
            var browseResponse = await RequestHandler.PostAsync(Endpoints.Browse, browsePayload, ClientType.WebMusic, cToken);

            cToken.ThrowIfCancellationRequested();

            nextResponse.ParseJson(out var nextJson);
            browseResponse.ParseJson(out var browseJson);

            var nextRenderer = nextJson
                .Get("contents")
                .Get("singleColumnMusicWatchNextResultsRenderer")
                .Get("tabbedRenderer")
                .Get("watchNextTabbedResultsRenderer")
                .Get("tabs")
                .GetAt(0)
                .Get("tabRenderer")
                .Get("content")
                .Get("musicQueueRenderer")
                .Get("content")
                .Get("playlistPanelRenderer")
                .Get("contents")
                .GetAt(0)
                .Get("playlistPanelVideoRenderer");

            var browseRenderer = browseJson
                .Get("contents")
                .Get("twoColumnBrowseResultsRenderer")
                .Get("tabs")
                .GetAt(0)
                .Get("tabRenderer")
                .Get("content")
                .Get("sectionListRenderer")
                .Get("contents")
                .GetAt(0)
                .Get("musicResponsiveHeaderRenderer");

            return EpisodeParser.Parse(nextRenderer, browseRenderer);
        }

        /// <summary>
        /// Gets the detailed info model for a given <see cref="Media"/>.
        /// </summary>
        /// <remarks>
        /// Currently only <see cref="MediaCategory.Playlist"/> is supported,
        /// returning a <see cref="PlaylistInfo"/> with the full track listing.
        /// All other media categories return <see langword="null"/>.
        /// </remarks>
        /// <param name="media">The <see cref="Media"/> whose info should be fetched</param>
        /// <param name="cToken"><see cref="CancellationToken"/> to cancel the request</param>
        /// <returns>
        /// A <see cref="MediaInfo"/> subtype (e.g. <see cref="PlaylistInfo"/>) on success,
        /// or <see langword="null"/> for unsupported media types
        /// </returns>
        public async Task<MediaInfo> GetInfoAsync(Media media, CancellationToken cToken = default)
        {
            return media.Type switch
            {
                MediaCategory.Playlist  => await HandlePlalistInfo((media as Playlist)!, cToken),
                _                       => null!
            };
        }

        /// <summary>
        /// Handles the Browse endpoint for fetching the full track listing of a
        /// <see cref="Playlist"/> as a <see cref="PlaylistInfo"/>.
        /// </summary>
        /// <param name="playlist">
        /// The <see cref="Playlist"/> whose tracks should be loaded.
        /// Its <c>Id</c> is used as the browse id in the request.
        /// </param>
        /// <param name="cToken"><see cref="CancellationToken"/> to cancel the request</param>
        /// <returns>A <see cref="PlaylistInfo"/> containing the full track listing</returns>
        /// <exception cref="ArgumentNullException">When <paramref name="playlist"/> has a null or whitespace <c>Id</c></exception>
        private async Task<PlaylistInfo> HandlePlalistInfo(Playlist playlist, CancellationToken cToken = default)
        {
            if (string.IsNullOrWhiteSpace(playlist.Id))
                throw new ArgumentNullException(nameof(playlist));

            Dictionary<string, object?> payload = new()
            {
                {"browseId" , playlist.Id }
            };

            var response = await RequestHandler.GetAsync(Endpoints.Browse, payload, ClientType.WebMusic, cToken);

            IDisposable _ = response.ParseJson(out var json);

            var renderer = json
                .Get("contents")
                .Get("twoColumnBrowseResultsRenderer")
                .Get("secondaryContents")
                .Get("sectionListRenderer")
                .Get("contents")
                .GetAt(0)
                .Get("musicPlaylistShelfRenderer");

            return PlaylistInfoParser.Parse(renderer, playlist);
        }
    }
}
