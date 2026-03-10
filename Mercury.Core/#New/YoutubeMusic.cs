using Mercury.Core.Http;
using Mercury.Core.Utils;
using System.Collections;
using System.Text.Json;

namespace Mercury.Core._New
{
    public static class YoutubeMusic
    {
        public static class Search
        {
            public static async Task<IReadOnlyList<Song>?> SearchAsync(
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

                return null;
            }
        }
    }
}
