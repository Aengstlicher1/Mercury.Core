using Mercury.Core.Json.Parsers.Browse.Explore;
using Mercury.Core.Json.Parsers.Generic;
using Mercury.Core.Models;
using Mercury.Core.Models.User;
using Mercury.Core.Utils;

namespace Mercury.Core.Json.Parsers.User;

internal static class PlaylistParser
{
    public static Playlist Parse(JObject renderer)
    {
        var thumbnails = ThumbnailParser.Parse(renderer
            .Get("thumbnailRenderer")
            .Get("musicThumbnailRenderer")
        );
        
        bool isAuto = renderer
            .Get("subtitle")
            .Get("runs")
            .AsArray()
            .UnlessNull(JArray.Empty)
            .Length == 1;
        
        return new Playlist()
        {
            Id = IdParser.ParseBrowse(renderer),
            Title = renderer
                .Get("title")
                .Get("runs")
                .GetAt(0)
                .Get("text")
                .AsString()
                .UnlessNull(string.Empty),
            Artist = isAuto 
                ? Entity.YoutubeMusic
                : EntityParser.Parse(renderer
                    .Get("subtitle")
                    .Get("runs")
                    .GetAt(2)),
            Thumbnails = thumbnails
        };
    }
}