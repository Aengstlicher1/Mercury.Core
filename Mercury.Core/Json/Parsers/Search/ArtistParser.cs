using Mercury.Core.Json.Parsers.Generic;
using Mercury.Core.Models;
using Mercury.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mercury.Core.Json.Parsers.Search
{
    internal static class ArtistParser
    {
        internal static Artist Parse(JElement renderer)
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

            return new Artist()
            {
                Id = GetId(renderer),
                Title = FlexColumnParser.Parse(flex, 0),
                Audience = FlexColumnParser.Parse(flex, 1, 2),
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
