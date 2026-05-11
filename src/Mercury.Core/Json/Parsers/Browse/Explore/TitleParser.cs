using Mercury.Core.Utils;

namespace Mercury.Core.Json.Parsers.Browse.Explore;

internal static class TitleParser
{
    public static string Parse(JObject renderer)
        => renderer
            .Get("header")
            .Get("musicCarouselShelfBasicHeaderRenderer")
            .Get("title")
            .Get("runs")
            .GetAt(0)
            .Get("text")
            .AsString()
            .UnlessNull(string.Empty);
}