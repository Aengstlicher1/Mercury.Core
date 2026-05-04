using Mercury.Core.Json.Parsers.Generic;
using Mercury.Core.Models;
using Mercury.Core.Utils;

namespace Mercury.Core.Json.Parsers.Search.Category
{
    internal static class SongParser
    {
        public static Song Parse(JElement renderer)
        {
            var thumbnails = ThumbnailParser.Parse(ThumbnailParser.GetThumbRenderer(renderer));

            var flex = FlexColumnParser.GetFlex(renderer);
            var runs = flex[1]
                .Get("musicResponsiveListItemFlexColumnRenderer")
                .Get("text")
                .Get("runs")
                .AsArray()
                .Or(JArray.Empty);

            // Assemble Song
            return new Song()
            {
                Id = IdParser.ParseWatch(renderer),
                Thumbnails = thumbnails,
                Title = FlexColumnParser.Parse(flex, 0),
                Artist = EntityParser.Parse(runs[0]),
                Album = runs.Length >= 3 ? FlexColumnParser.Parse(flex, 1, 2) : string.Empty,
                Duration = FlexColumnParser.Parse(flex, 1, runs.Length - 1)
            };
        }
    }
}
