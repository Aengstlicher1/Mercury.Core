using System.Drawing;
using Mercury.Core.Models.Explore;
using Mercury.Core.Utils;

namespace Mercury.Core.Json.Parsers.Browse.Explore;

internal static class GenreParser
{
    public static Genre Parse(JElement renderer)
    {
        return new Genre()
        {
            Title = renderer
                .Get("musicNavigationButtonRenderer")
                .Get("buttonText")
                .Get("runs")
                .GetAt(0)
                .Get("text")
                .AsString()
                .Or(string.Empty),
            Color = GetColor (
                renderer
                    .Get("musicNavigationButtonRenderer")
                    .Get("solid")
                    .Get("leftStripeColor")
                    .AsInt32()
                    .Or(0)
            ),
        };
    }

    private static Color GetColor(int color)
        => System.Drawing.Color.FromArgb(color);
}