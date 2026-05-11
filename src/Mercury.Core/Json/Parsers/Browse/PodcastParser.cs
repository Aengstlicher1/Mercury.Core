using Mercury.Core.Json.Parsers.Generic;
using Mercury.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mercury.Core.Json.Parsers.Browse
{
    internal static class PodcastParser
    {
        public static Podcast Parse(JObject renderer, string browseId)
        {
            var thumbnails = ThumbnailParser.Parse(ThumbnailParser.GetThumbRenderer(renderer));

            return new Podcast()
            {
                Id = browseId,
                Title = RunsParser.Parse(RunsParser.GetRuns(renderer.Get("title"))),
                Artist = EntityParser.Parse(RunsParser.GetRuns(renderer.Get("straplineTextOne"))[0]),
                Thumbnails = thumbnails
            };
        }
    }
}
