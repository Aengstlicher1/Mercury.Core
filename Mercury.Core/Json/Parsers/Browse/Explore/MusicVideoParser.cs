using Mercury.Core.Json.Parsers.Generic;
using Mercury.Core.Models;
using Mercury.Core.Utils;

namespace Mercury.Core.Json.Parsers.Browse.Explore;

internal static class MusicVideoParser
{
    public static Video? Parse(JElement renderer)
    {
        var thumbnails = ThumbnailParser.Parse(renderer.Get("thumbnailRenderer").Get("musicThumbnailRenderer"));
        
        var vid = new Video()
        {
            Id = renderer
                .Get("title")
                .Get("runs")
                .GetAt(0)
                .Get("navigationEndpoint")
                .Get("browseEndpoint")
                .Get("browseId")
                .AsString()
                .Or(string.Empty),
            
            Title = renderer
                .Get("title")
                .Get("runs")
                .GetAt(0)
                .Get("text")
                .AsString()
                .Or(string.Empty),
            
            Artist = renderer
                .Get("subtitle")
                .Get("runs")
                .GetAt(0)
                .Get("text")
                .AsString()
                .Or(string.Empty),
            
            Views = renderer
                .Get("subtitle")
                .Get("runs")
                .GetAt(2)
                .Get("text")
                .AsString()
                .Or(string.Empty),
            Thumbnails = thumbnails,
        };
        return vid.Artist.Any(char.IsDigit) ? null : vid;
    }
}