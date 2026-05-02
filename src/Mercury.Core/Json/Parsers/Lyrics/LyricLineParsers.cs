using System.Collections.ObjectModel;
using System.Text.Json.Nodes;
using Mercury.Core.Models;

namespace Mercury.Core.Json.Parsers.Lyrics;

public static class LyricLineParsers
{
    public static Collection<LyricLine> ParseSyncedLyrics(string lyricText)
    {
        return new Collection<LyricLine>(
            lyricText.Split('\n', StringSplitOptions.RemoveEmptyEntries)
                .Select(line =>
                {
                    var closeBracket = line.IndexOf(']');
                    var timestamp = line[1..closeBracket]; // format: "01:05.17"
                    var content = line[(closeBracket + 1)..].Trim();
                    
                    var parts = timestamp.Split(':', '.');
                    var time = new TimeSpan(
                        0,
                        0,
                        int.Parse(parts[0]),
                        int.Parse(parts[1]),
                        int.Parse(parts[2]) * 10
                    );

                    return new LyricLine
                    {
                        Content = content,
                        Timing = time
                    };
                })
                .ToList()
        );
    }

    public static Collection<LyricLine> ParsePlainLyrics(string lyricText)
    {
        var lines = lyricText.Split('\n');
        return new Collection<LyricLine>(
            lines
                .Select(line => new LyricLine { Content = line.Trim() })
                .ToList()
        );
    }
}