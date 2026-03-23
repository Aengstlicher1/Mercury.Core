using Mercury.Core.Json.Parsers.Generic;
using Mercury.Core.Models;
using Mercury.Core.Utils;

namespace Mercury.Core.Json.Parsers.Search
{
    internal static class SongParser
    {
        public static Song Parse(JElement renderer)
        {
            var id = IdParser.ParseWatch(renderer);

            YoutubeMusic.Browse.GetAsync(id, Enums.MediaCategory.Song).GetAwaiter().GetResult();
            throw new Exception("Something went seriously WRONG!!!");
        }
    }
}
