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
    /// <summary>
    /// Provides search functionality against the YouTube Music catalogue.
    /// Supports both mixed-type top-level searches and filtered category searches.
    /// </summary>
    public sealed class SearchService
    {
        /// <summary>
        /// Searches YouTube Music for the given <paramref name="query"/> and returns
        /// a mixed collection of all matching media types.
        /// </summary>
        /// <remarks>
        /// The method parses the top result shelf and the normal results shelf from
        /// the YouTube Music search response. Each item is dispatched to the correct
        /// parser based on its detected <see cref="Enums.MediaCategory"/>.
        /// Returns <see langword="null"/> when the request is cancelled via
        /// <paramref name="cToken"/>.
        /// </remarks>
        /// <param name="query">The search query string. Must not be null or empty.</param>
        /// <param name="ignoreSpelling">
        /// When <see langword="true"/> (default), YouTube Music spelling corrections
        /// are ignored and the query is used verbatim.
        /// </param>
        /// <param name="cToken"><see cref="CancellationToken"/> to cancel the request</param>
        /// <returns>
        /// A collection of <see cref="Media"/> results on success,
        /// or <see langword="null"/> if the request was cancelled.
        /// </returns>
        /// <exception cref="ArgumentNullException">When <paramref name="query"/> is null or empty</exception>
        public async Task<Collection<Media>?> SearchAsync(
            string query,
            bool ignoreSpelling = true,
            CancellationToken cToken = default
        )
        {
            try
            {
                if (string.IsNullOrEmpty(query))
                    throw new ArgumentNullException("query");

                Dictionary<string, object?> payload = new()
                {
                    { "query", query }
                };

                var response = await RequestHandler.PostAsync(Endpoints.Search, payload, ClientType.WebMusic, cToken);

                cToken.ThrowIfCancellationRequested();

                using IDisposable _ = response.GetJson(out var json);

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
                    .UnlessNull(JArray.Empty);

                Collection<Media> results = new Collection<Media>();

                foreach (var result in normalResults)
                {
                    cToken.ThrowIfCancellationRequested();

                    var renderer = result.Get("musicResponsiveListItemRenderer");
                    var category = CategoryParser.Parse(renderer);

                    results.Add
                    (
                        category switch
                        {
                            Enums.MediaCategory.Song     => SongParser.Parse(renderer),
                            Enums.MediaCategory.Video    => VideoParser.Parse(renderer),
                            Enums.MediaCategory.Artist   => ArtistParser.Parse(renderer),
                            Enums.MediaCategory.Album    => AlbumParser.Parse(renderer),
                            Enums.MediaCategory.Playlist => PlaylistParser.Parse(renderer),
                            Enums.MediaCategory.Episode  => EpisodeParser.Parse(renderer),
                            Enums.MediaCategory.Profile  => ProfileParser.Parse(renderer),
                            Enums.MediaCategory.Podcast  => PodcastParser.Parse(renderer),
                            _                            => null!
                        }
                    );
                }

                return results;
            }
            catch (TaskCanceledException) { }

            return null!;
        }

        /// <summary>
        /// Searches YouTube Music for the given <paramref name="query"/> restricted
        /// to a specific <paramref name="filter"/> category.
        /// </summary>
        /// <remarks>
        /// When <paramref name="filter"/> is <see cref="Enums.SearchFilter.All"/>,
        /// this method delegates to <see cref="SearchAsync"/> instead.
        /// Otherwise the filter is encoded as a YouTube Music param and a dedicated
        /// category parser is used for each result.
        /// Returns <see langword="null"/> when the request is cancelled via
        /// <paramref name="cToken"/>.
        /// </remarks>
        /// <param name="query">The search query string. Must not be null or empty.</param>
        /// <param name="filter">
        /// The <see cref="Enums.SearchFilter"/> that restricts results to a single
        /// media category (e.g. Songs, Albums, Artists).
        /// </param>
        /// <param name="ignoreSpelling">
        /// When <see langword="true"/> (default), YouTube Music spelling corrections
        /// are ignored and the query is used verbatim.
        /// </param>
        /// <param name="cToken"><see cref="CancellationToken"/> to cancel the request</param>
        /// <returns>
        /// A collection of <see cref="Media"/> results on success,
        /// or <see langword="null"/> if the request was cancelled.
        /// </returns>
        /// <exception cref="ArgumentNullException">When <paramref name="query"/> is null or empty</exception>
        public async Task<Collection<Media>?> SearchCategoryAsync(
            string query,
            Enums.SearchFilter filter,
            bool ignoreSpelling = true,
            CancellationToken cToken = default)
        {
            if (filter is Enums.SearchFilter.All)
                return await SearchAsync(query, cToken: cToken);

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

                using IDisposable _ = response.GetJson(out var json);

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
                    .UnlessNull(JArray.Empty);

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
                            Enums.SearchFilter.Songs               => Json.Parsers.Search.Category.SongParser.Parse(renderer),
                            Enums.SearchFilter.Videos              => Json.Parsers.Search.Category.VideoParser.Parse(renderer),
                            Enums.SearchFilter.Artists             => ArtistParser.Parse(renderer),
                            Enums.SearchFilter.Albums              => AlbumParser.Parse(renderer),
                            Enums.SearchFilter.FeaturedPlaylists   => Json.Parsers.Search.Category.PlaylistParser.Parse(renderer),
                            Enums.SearchFilter.CommunityPlaylists  => Json.Parsers.Search.Category.PlaylistParser.Parse(renderer),
                            Enums.SearchFilter.Episodes            => Json.Parsers.Search.Category.EpisodeParser.Parse(renderer),
                            Enums.SearchFilter.Profiles            => ProfileParser.Parse(renderer),
                            Enums.SearchFilter.Podcasts            => Json.Parsers.Search.Category.PodcastParser.Parse(renderer),
                            _                                      => null!
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
