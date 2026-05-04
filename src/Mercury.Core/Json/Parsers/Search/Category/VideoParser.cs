using Mercury.Core.Json.Parsers.Generic;
using Mercury.Core.Models;
using Mercury.Core.Utils;

namespace Mercury.Core.Json.Parsers.Search.Category
{
    internal static class VideoParser
    {
        internal static Video Parse(JElement renderer)
        {
            var thumbnails = ThumbnailParser.Parse(ThumbnailParser.GetThumbRenderer(renderer));

            var flex = FlexColumnParser.GetFlex(renderer);
            var entity = flex[1]
                .Get("musicResponsiveListItemFlexColumnRenderer")
                .Get("text")
                .Get("runs")
                .GetAt(0);


            // Assemble Video
            return new Video()
            {
                Id = IdParser.ParseWatch(renderer),
                Title = FlexColumnParser.Parse(flex, 0),
                Artist = EntityParser.Parse(entity),
                Views = FlexColumnParser.Parse(flex, 1, 2),
                Duration = FlexColumnParser.Parse(flex, 1, 4),
                Thumbnails = thumbnails,
            };
        }
    }
}
