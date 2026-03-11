using Mercury.Core.Json.Parsers.Generic;
using Mercury.Core.Models;
using Mercury.Core.Utils;


namespace Mercury.Core.Json.Parsers.Search
{
    internal static class PlaylistParser
    {
        internal static Playlist Parse(JElement renderer)
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

            return new Playlist()
            {
                Id = GetId(renderer),
                Title = FlexColumnParser.Parse(flex,0),
                Artists = FlexColumnParser.Parse(flex, 1, 2),
                Thumbnails = thumbnails,
                Views = FlexColumnParser.Parse(flex, 1, 4)
            };
        }

        private static string GetId(JElement renderer)
            =>  renderer
                .Get("navigationEndpoint")
                .Get("browseEndpoint")
                .Get("browseId")
                .AsString()
                .Or(string.Empty);
    }
}
