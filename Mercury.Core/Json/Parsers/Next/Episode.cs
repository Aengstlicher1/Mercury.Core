using Mercury.Core.Json.Parsers.Generic;
using Mercury.Core.Models;
using Mercury.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mercury.Core.Json.Parsers.Next
{
    internal static class EpisodeParser
    {
        public static Episode Parse(JElement renderer)
        {
            var thumbnails = ThumbnailParser.Parse(renderer);

            return new Episode()
            {
                Id = renderer.Get("videoId").AsString().Or(string.Empty),
                Title = renderer.Get("title").Get("runs").GetAt(0).Get("text").AsString().Or(string.Empty),
                PodcastName = RunsParser.Parse(RunsParser.GetRuns(renderer.Get("longBylineText"))),
                Duration = RunsParser.Parse(RunsParser.GetRuns(renderer.Get("lengthText"))),
                Thumbnails = thumbnails
            };
        }

        public static string GetBrowseId(JElement renderer)
        {
            
        }
    }
}
