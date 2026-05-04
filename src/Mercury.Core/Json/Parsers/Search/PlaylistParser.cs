using Mercury.Core.Json.Parsers.Generic;
using Mercury.Core.Models;
using Mercury.Core.Utils;


namespace Mercury.Core.Json.Parsers.Search
{
    internal static class PlaylistParser
    {
        internal static Playlist Parse(JElement renderer)
        {
            var thumbnails = ThumbnailParser.Parse(ThumbnailParser.GetThumbRenderer(renderer));

            var flex = FlexColumnParser.GetFlex(renderer);
            var entity = flex[1]
                .Get("musicResponsiveListItemFlexColumnRenderer")
                .Get("text")
                .Get("runs")
                .GetAt(2);


            return new Playlist()
            {
                Id = IdParser.ParseBrowse(renderer),
                Title = FlexColumnParser.Parse(flex,0),
                Artist = EntityParser.Parse(entity),
                Thumbnails = thumbnails,
                Views = FlexColumnParser.Parse(flex, 1, 4)
            };
        }
    }
}
