using Mercury.Core.Json.Parsers.Generic;
using Mercury.Core.Models;
using Mercury.Core.Utils;

namespace Mercury.Core.Json.Parsers.Search
{
    internal static class SongParser
    {
        public static Song Parse(JObject renderer)
        {
            var thumbnails = ThumbnailParser.Parse(ThumbnailParser.GetThumbRenderer(renderer));

            var flex = FlexColumnParser.GetFlex(renderer);
            var entity = flex[1]
                .Get("musicResponsiveListItemFlexColumnRenderer")
                .Get("text")
                .Get("runs")
                .GetAt(2);

            // Assemble Song
            return new Song()
            {
                Id = IdParser.ParseWatch(renderer),
                Thumbnails = thumbnails,
                Title = FlexColumnParser.Parse(flex, 0),
                Artist = EntityParser.Parse(entity)
            };
        }
    }
}
