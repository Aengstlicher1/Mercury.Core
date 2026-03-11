using Mercury.Core.Json.Parsers.Generic;
using Mercury.Core.Models;
using Mercury.Core.Utils;

namespace Mercury.Core.Json.Parsers.Search
{
    internal static class SongParser
    {
        public static Song Parse(JElement renderer)
        {
            // Get the Thumbnails
            var thumbs = renderer
                .Get("thumbnail")
                .Get("musicThumbnailRenderer")
                .Get("thumbnail")
                .Get("thumbnails")
                .AsArray()
                .Or(JArray.Empty);

            var thumbnails = ThumbnailParser.Parse(thumbs);

            var flex = FlexColumnParser.GetFlex(renderer);

            // Assemble Song
            return new Song()
            {
                Id = GetId(renderer),
                Thumbnails = thumbnails,
                Title = FlexColumnParser.Parse(flex, 0)!,
                Artists = FlexColumnParser.Parse(flex, 1, 2)!
            };
        }

        private static string GetId(JElement renderer)
            =>  renderer
                .Get("flexColumns")
                .GetAt(0)
                .Get("musicResponsiveListItemFlexColumnRenderer")
                .Get("text")
                .Get("runs")
                .GetAt(0)
                .Get("navigationEndpoint")
                .Get("watchEndpoint")
                .Get("videoId")
                .AsString()
                .Or(string.Empty);
    }
}
