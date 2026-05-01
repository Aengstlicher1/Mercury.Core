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
    public sealed class BrowseService
    {
        //\\ Browse Endpoint for the explore page //\\
        
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
        
        //\\ Browse Endpoint for normal Model //\\

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
                _ when id.Length == 11        => await HandleNext(id, cToken),
                _ => null
            };
        }


        private async Task<Media> HandleNext(string videoId, CancellationToken cToken = default)
        {
            if (string.IsNullOrWhiteSpace(videoId))
                throw new ArgumentNullException(nameof(videoId));

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



        //\\ Browse Endpoint for Info-Model //\\
        public async Task<MediaInfo> GetInfoAsync(Media media, CancellationToken cToken = default)
        {
            return media.Type switch
            {
                MediaCategory.Playlist  => await HandlePlalistInfo((media as Playlist)!, cToken),
                _                       => null!
            };
        }

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
