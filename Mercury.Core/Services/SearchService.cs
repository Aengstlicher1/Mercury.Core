using Mercury.Core.Json;
using Mercury.Core.Json.Parsers;
using Mercury.Core.Json.Parsers.Search;
using Mercury.Core.Json.Parsers.Generic;
using Mercury.Core.Models;
using Mercury.Core.Utils;
using Mercury.Core.Network;
using System.Collections.ObjectModel;
using System.Diagnostics;


namespace Mercury.Core.Services
{
    public sealed class SearchService
    {
        public async Task<Collection<Media>?> SearchCategoryAsync(
            string query,
            Enums.SearchFilter filter,
            bool ignoreSpelling = true,
            CancellationToken cToken = default)
        {

            try 
            {
                if (string.IsNullOrEmpty(query))
                    throw new ArgumentNullException("query");

                Dictionary<string, object?> payload = new()
                {
                    { "query", query },
                    { "params", filter.ToParam() }
                }; 

                var startTime = DateTime.Now;
                var response = await RequestHandler.PostAsync(Endpoints.Search, payload, ClientType.WebMusic, cToken);
                Debug.WriteLine($"Core: \"Sending Search Request\" took { DateTime.Now - startTime}");

                using IDisposable _ = response.ParseJson(out var json);

                var shelfResults = json
                    .Get("contents")
                    .Get("tabbedSearchResultsRenderer")
                    .Get("tabs").GetAt(0)
                    .Get("tabRenderer")
                    .Get("content")
                    .Get("sectionListRenderer")
                    .Get("contents")
                    .GetAt(1)
                    .Get("musicShelfRenderer")
                    .Get("contents")
                    .AsArray()
                    .Or(JArray.Empty);

                Collection<Media> results = new Collection<Media>();
                foreach (var result in shelfResults)
                {
                    cToken.ThrowIfCancellationRequested();

                    var renderer = result.Get("musicResponsiveListItemRenderer");
                    var category = CategoryParser.Parse(renderer);

                    results.Add
                    (
                        filter switch
                        {
                            Enums.SearchFilter.Songs                => Json.Parsers.Search.Category.SongParser.Parse(renderer),
                            Enums.SearchFilter.Videos               => Json.Parsers.Search.Category.VideoParser.Parse(renderer),
                            Enums.SearchFilter.Artists              => ArtistParser.Parse(renderer),
                            Enums.SearchFilter.Albums               => AlbumParser.Parse(renderer),
                            Enums.SearchFilter.FeaturedPlaylists    => Json.Parsers.Search.Category.PlaylistParser.Parse(renderer),
                            Enums.SearchFilter.CommunityPlaylists   => Json.Parsers.Search.Category.PlaylistParser.Parse(renderer),
                            Enums.SearchFilter.Episodes             => Json.Parsers.Search.Category.EpisodeParser.Parse(renderer),
                            Enums.SearchFilter.Profiles             => ProfileParser.Parse(renderer),
                            Enums.SearchFilter.Podcasts             => Json.Parsers.Search.Category.PodcastParser.Parse(renderer),
                            _ => null!
                        }
                    );
                }

                return results;
            }
            catch (TaskCanceledException) { }
            return null;
        }
    }
}
