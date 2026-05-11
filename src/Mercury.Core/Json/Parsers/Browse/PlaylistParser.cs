using Mercury.Core.Json.Parsers.Generic;
using Mercury.Core.Models;
using Mercury.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mercury.Core.Json.Parsers.Browse
{
    internal static class PlaylistParser
    {
        public static Playlist Parse(JObject renderer, string browseId)
        {
            var thumbnails = ThumbnailParser.Parse(ThumbnailParser.GetThumbRenderer(renderer));

            var runsLength = RunsParser.GetRuns(renderer.Get("secondSubtitle")).Length;
            var entityName = renderer
                .Get("facepile")
                .Get("avatarStackViewModel")
                .Get("text")
                .Get("content")
                .AsString()
                .UnlessNull(string.Empty);
            var entityId = renderer
                .Get("facepile")
                .Get("avatarStackViewModel")
                .Get("rendererContext")
                .Get("commandContext")
                .Get("onTap")
                .Get("innertubeCommand")
                .Get("browseEndpoint")
                .Get("browseId")
                .AsString()
                .UnlessNull(string.Empty);

            return new Playlist()
            {
                Id = browseId,
                Title = RunsParser.Parse(RunsParser.GetRuns(renderer.Get("title"))),
                Views = (runsLength > 3) ? RunsParser.Parse(RunsParser.GetRuns(renderer.Get("secondSubtitle"))) : string.Empty,
                ItemCount = RunsParser.Parse(RunsParser.GetRuns(renderer.Get("secondSubtitle")), runsLength > 3 ? 2 : 0),
                Artist = new Entity(entityName, entityId),
                Thumbnails = thumbnails
            };
        }
    }
}
