using Mercury.Core.Json.Parsers.Generic;
using Mercury.Core.Models;
using Mercury.Core.Utils;


namespace Mercury.Core.Json.Parsers.Search
{
    internal static class EpisodeParser
    {
        internal static Episode Parse(JElement renderer)
        {
            var thumbnails = ThumbnailParser.Parse(ThumbnailParser.GetThumbRenderer(renderer));

            var flex = FlexColumnParser.GetFlex(renderer);

            return new Episode()
            {
                BrowseId = IdParser.ParseBrowse(renderer
                    .Get("flexColumns")
                    .GetAt(0)
                    .Get("musicResponsiveListItemFlexColumnRenderer")
                    .Get("text")
                    .Get("runs")
                    .GetAt(0)),
                Id = renderer
                    .Get("overlay")
                    .Get("musicItemThumbnailOverlayRenderer")
                    .Get("content")
                    .Get("musicPlayButtonRenderer")
                    .Get("playNavigationEndpoint")
                    .Get("watchEndpoint")
                    .Get("videoId")
                    .AsString()
                    .Or(string.Empty),
                Title = FlexColumnParser.Parse(flex, 0),
                Date = FlexColumnParser.Parse(flex, 1, 2),
                PodcastName = FlexColumnParser.Parse(flex, 1, 4),
                Thumbnails = thumbnails,
            };
        }
    }
}
