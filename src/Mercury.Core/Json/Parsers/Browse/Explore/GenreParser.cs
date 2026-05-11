using System.Drawing;
using Mercury.Core.Models.Explore;
using Mercury.Core.Utils;

namespace Mercury.Core.Json.Parsers.Browse.Explore;

internal static class GenreParser
{
    public static Genre Parse(JObject renderer)
    {
        var color = GetColor(renderer
            .Get("musicNavigationButtonRenderer")
            .Get("solid")
            .Get("leftStripeColor")
            .AsLong()
            .UnlessNull(0)
        );
        
        return new Genre()
        {
            Title = renderer
                .Get("musicNavigationButtonRenderer")
                .Get("buttonText")
                .Get("runs")
                .GetAt(0)
                .Get("text")
                .AsString()
                .UnlessNull(string.Empty),
            Color = color,
            BrowseParam = renderer
                .Get("musicNavigationButtonRenderer")
                .Get("clickCommand")
                .Get("browseEndpoint")
                .Get("params")
                .AsString()
                .UnlessNull(string.Empty),
        };
    }

    private static Color GetColor(long argb)
        => System.Drawing.Color.FromArgb((int)(uint)argb);
}