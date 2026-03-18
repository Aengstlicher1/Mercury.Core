using Mercury.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mercury.Core.Json.Parsers.Generic
{
    internal static class CategoryParser
    {
        internal static Enums.MediaCategory Parse(JArray flex)
        {
            var category = FlexColumnParser.Parse(flex, 1); 

            return category switch
            {
                "Song" => Enums.MediaCategory.Song,
                "Video" => Enums.MediaCategory.Video,
                "Playlist" => Enums.MediaCategory.Playlist,
                "Artist" => Enums.MediaCategory.Artist,
                "EP" or "Album" or "Single" => Enums.MediaCategory.Album,
                "Profile" => Enums.MediaCategory.Profile,
                "Podcast" => Enums.MediaCategory.Podcast,
                "Episode" => Enums.MediaCategory.Episode,
                _ => Enums.MediaCategory.None
            };
        }
    }
}
