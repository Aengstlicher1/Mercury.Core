using Mercury.Core.Json.Parsers.Generic;
using Mercury.Core.Models;
using Mercury.Core.Utils;

namespace Mercury.Core.Json.Parsers.Browse.Explore;

internal static class AlbumParser
{
    public static Album Parse(JObject renderer)
    {
        var thumbnails = ThumbnailParser.Parse(renderer.Get("thumbnailRenderer").Get("musicThumbnailRenderer"));

        return new Album()
        {
            Id = renderer
                .Get("title")
                .Get("runs")
                .GetAt(0)
                .Get("navigationEndpoint")
                .Get("browseEndpoint")
                .Get("browseId")
                .AsString()
                .UnlessNull(string.Empty),
            
            Title = renderer
                .Get("title")
                .Get("runs")
                .GetAt(0)
                .Get("text")
                .AsString()
                .UnlessNull(string.Empty),
            
            Artist = EntityParser.Parse(renderer
                .Get("subtitle")
                .Get("runs")
                .GetAt(2)),
            Thumbnails = thumbnails
        };
    }
}