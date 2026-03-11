using Mercury.Core.Json;
using Mercury.Core.Json.Parsers;
using Mercury.Core.Json.Parsers.Search;
using Mercury.Core.Json.Parsers.Generic;
using Mercury.Core.Models;
using Mercury.Core.Utils;
using Mercury.Core.Utils.Network;
using System.Collections.ObjectModel;


namespace Mercury.Core.Services
{
    public sealed class SearchService
    {
        public async Task<Collection<Media>?> SearchAsync(
            string query,
            bool ignoreSpelling = true,
            CancellationToken cToken = default
        )
        {
            if (string.IsNullOrEmpty(query))
                throw new ArgumentNullException("query");

            Dictionary<string, object?> payload = new ()
            {
                { "query", query }
            };

            var response = await RequestHandler.PostAsync(Endpoints.Search, payload, ClientType.WebMusic, cToken);
            using IDisposable _ = response.ParseJson(out var json);

            var renderers = json
                .Get("contents")
                .Get("tabbedSearchResultsRenderer")
                .Get("tabs").GetAt(0)
                .Get("tabRenderer")
                .Get("content")
                .Get("sectionListRenderer")
                .Get("contents");
                    
            var topResult = renderers
                .GetAt(1)
                .Get("musicCardShelfRenderer");

            var normalResults = renderers
                .GetAt(2).Get("musicShelfRenderer")
                .Get("contents")
                .AsArray()
                .Or(JArray.Empty);

            Collection<Media> results = new Collection<Media>();
            foreach (var result in normalResults)
            {
                var renderer = result.Get("musicResponsiveListItemRenderer");
                var flex = FlexColumnParser.GetFlex(renderer);
                var category = CategoryParser.Parse(flex);

                results.Add
                (
                    category switch
                    {
                        Enums.MediaCategory.Song    => SongParser.Parse(renderer),
                        Enums.MediaCategory.Video   => VideoParser.Parse(renderer),
                        Enums.MediaCategory.Artist   => ArtistParser.Parse(renderer),
                        Enums.MediaCategory.Album   => AlbumParser.Parse(renderer),
                        Enums.MediaCategory.FeaturedPlaylist => PlaylistParser.Parse(renderer), 
                        Enums.MediaCategory.CommunityPlaylist => PlaylistParser.Parse(renderer),
                        Enums.MediaCategory.Episode => EpisodeParser.Parse(renderer),
                        _ => null!
                    }
                );
            }

            return results;
        }
    }
}
