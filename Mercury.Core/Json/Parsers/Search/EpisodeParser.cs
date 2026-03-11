using Mercury.Core.Json.Parsers.Generic;
using Mercury.Core.Models;
using Mercury.Core.Utils;


namespace Mercury.Core.Json.Parsers.Search
{
    internal static class EpisodeParser
    {
        internal static Episode Parse(JElement renderer)
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

            return new Episode()
            {
                Id = GetId(renderer),
                Title = FlexColumnParser.Parse(flex, 0),
                Date = FlexColumnParser.Parse(flex, 1, 2),
                PodcastName = FlexColumnParser.Parse(flex, 1, 4),
                Thumbnails = thumbnails,
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
