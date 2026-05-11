using Mercury.Core.Json.Parsers.Generic;
using Mercury.Core.Models;


namespace Mercury.Core.Json.Parsers.Search.Category
{
    internal static class PodcastParser
    {
        internal static Podcast Parse(JObject renderer)
        {
            var thumbnails = ThumbnailParser.Parse(ThumbnailParser.GetThumbRenderer(renderer));

            var flex = FlexColumnParser.GetFlex(renderer);
            var entity = flex[1]
                .Get("musicResponsiveListItemFlexColumnRenderer")
                .Get("text")
                .Get("runs")
                .GetAt(0);

            return new Podcast()
            {
                Id = IdParser.ParseBrowse(renderer),
                Title = FlexColumnParser.Parse(flex, 0),
                Artist = EntityParser.Parse(entity),
                Thumbnails = thumbnails,
            };
        }
    }
}
