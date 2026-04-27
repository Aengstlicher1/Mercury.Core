using Mercury.Core.Models;
using Mercury.Core.Network;
using System.Text.Json;
using Mercury.Core.Json.Parsers.Lyrics;
using Mercury.Core.Utils;


namespace Mercury.Core.Services
{
    public sealed class LyricsService
    {
        private const string BaseUrl = "https://lrclib.net/api";

        public async Task<Lyrics?> GetLyricsAsync(Track track)
        {
            try
            {
                var url = $"{BaseUrl}/get?artist_name={Uri.EscapeDataString(track.Artist)}&track_name={Uri.EscapeDataString(track.Title)}";

                if (track.DurationTimeSpan.TotalSeconds > 0)
                    url += $"&duration={Convert.ToInt32(track.DurationTimeSpan.TotalSeconds)}";

                var response = await RequestHandler.httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                    return null;

                var text = await response.Content.ReadAsStringAsync();
                using IDisposable _ = text.ParseJson(out var json);
                return LyricResultParser.Parse(json);
            }
            catch (HttpRequestException ex)
            {
                return new Lyrics() { PlainLyrics = new(
                [
                    new LyricLine{Content = "Http Error: " + ex.Message} 
                ]
                )};
                
            }
        }
    }
}
