using Mercury.Core.Json;
using Mercury.Core.Json.Parsers.Next;
using Mercury.Core.Models;
using Mercury.Core.Network;
using Mercury.Core.Utils;
using static Mercury.Core.Models.Enums;

namespace Mercury.Core.Services
{
    public sealed class BrowseService
    {
        public async Task<Media> GetAsync(string id, MediaCategory category, CancellationToken cToken = default)
        {
            if (category.IsNextEndpoint())
                return await HandleNext(id, category, cToken);
            else
                return await HandleBrowse(id, category, cToken);
        }

        private async Task<Media> HandleNext(string videoId, MediaCategory category, CancellationToken cToken = default)
        {
            if (string.IsNullOrWhiteSpace(videoId))
                throw new ArgumentNullException("videoId");

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

            Media result = category switch
            {
                MediaCategory.Song      => SongParser.Parse(renderer),
                MediaCategory.Video     => VideoParser.Parse(renderer),
                MediaCategory.Episode   => await HandleBrowse(EpisodeParser.GetBrowseId(renderer), category),
                _ => null!
            };

            return result;
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

            return null;
        }
}
}
