using Mercury.Core.Models;
using Mercury.Core.Utils;


namespace Mercury.Core.Json.Parsers.Generic
{
    internal static class StreamingDataParser
    {
        public static StreamingData Parse(JObject json)
        {
            var expiresIn = TimeSpan.FromSeconds(int.Parse(json
                .Get("streamingData")
                .Get("expiresInSeconds")
                .AsString()
                .UnlessNull(string.Empty)));

            var details = json
                .Get("videoDetails");

            return new StreamingData()
            {
                ExpiresAt = DateTime.Now + expiresIn,
                Id = details.Get("videoId").AsString().UnlessNull(string.Empty),
                Title = details.Get("title").AsString().UnlessNull(string.Empty),
                Duration = TimeSpan.FromSeconds(int.Parse(details.Get("lengthSeconds").AsString().UnlessNull(string.Empty))),
                ViewCount = int.Parse(details.Get("viewCount").AsString().UnlessNull(string.Empty))
            };
        }
    }
}
