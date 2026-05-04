using Mercury.Core.Models;
using Mercury.Core.Network;
using System.Text.Json;
using Mercury.Core.Json.Parsers.Lyrics;
using Mercury.Core.Utils;

namespace Mercury.Core.Services
{
    /// <summary>
    /// Fetches plain and time-synced lyrics for a given track from the
    /// <see href="https://lrclib.net">lrclib.net</see> public API.
    /// </summary>
    /// <remarks>
    /// This service is exposed through the static <c>YoutubeMusic.Lyrics</c> facade.
    /// It performs a single GET request per call and returns <see langword="null"/>
    /// when no matching lyrics entry is found on the remote server.
    /// </remarks>
    public sealed class LyricsService
    {
        /// <summary>
        /// Base URL for all lrclib.net API endpoints.
        /// </summary>
        private const string BaseUrl = "https://lrclib.net/api";

        /// <summary>
        /// Asynchronously retrieves lyrics for the specified <paramref name="track"/>.
        /// </summary>
        /// <param name="track">
        /// The track whose lyrics should be fetched. The <c>Artist</c>, <c>Title</c>,
        /// and optionally <c>DurationTimeSpan</c> properties are used to build the
        /// query string sent to lrclib.net.
        /// </param>
        /// <returns>
        /// A <see cref="Lyrics"/> object containing plain and/or synced lyrics on
        /// success; <see langword="null"/> if the server returns a non-success status
        /// code (e.g. 404 - no match found).
        /// </returns>
        /// <remarks>
        /// When <c>track.DurationTimeSpan.TotalSeconds</c> is greater than zero the
        /// duration is appended to the query, which improves match accuracy on
        /// lrclib.net.
        ///
        /// If the HTTP request itself fails (network error, timeout, etc.) the method
        /// returns a <see cref="Lyrics"/> object whose <c>PlainLyrics</c> list
        /// contains a single <see cref="LyricLine"/> describing the error, so the UI
        /// always has something to display.
        /// </remarks>
        public async Task<Lyrics?> GetLyricsAsync(Track track)
        {
            try
            {
                // Build the lrclib.net /get query string.
                // Artist and track name are URL-encoded to handle special characters.
                var url = $"{BaseUrl}/get?artist_name={track.Artist?.Name}&track_name={track.Title}";

                // Append duration when available - lrclib uses it to pick the best match.
                if (track.DurationTimeSpan.TotalSeconds > 0)
                    url += $"&duration={Convert.ToInt32(track.DurationTimeSpan.TotalSeconds)}";

                var response = await RequestHandler.httpClient.GetAsync(url);

                // A non-success status (most commonly 404) means no lyrics were found.
                if (!response.IsSuccessStatusCode)
                    return null;

                var text = await response.Content.ReadAsStringAsync();

                // Parse the JSON response into a Lyrics model via the dedicated parser.
                using IDisposable _ = text.ParseJson(out var json);
                return LyricResultParser.Parse(json);
            }
            catch (HttpRequestException ex)
            {
                // Surface network errors as a single-line lyrics entry so the UI
                // can display a meaningful message instead of crashing or showing nothing.
                return new Lyrics()
                {
                    PlainLyrics = new(
                    [
                        new LyricLine { Content = "Http Error: " + ex.Message }
                    ])
                };
            }
        }
    }
}
