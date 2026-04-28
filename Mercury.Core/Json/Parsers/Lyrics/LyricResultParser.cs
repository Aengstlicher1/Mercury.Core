using System;
using Mercury.Core.Models;
using Mercury.Core.Utils;

namespace Mercury.Core.Json.Parsers.Lyrics;

internal static class LyricResultParser
{
    public static Models.Lyrics Parse(JElement json)
    {
        return new Models.Lyrics()
        {
            TrackName = json.Get("trackName").AsString(),
            ArtistName = json.Get("artistName").AsString(),
            AlbumName = json.Get("albumName").AsString(),
            Duration = TimeSpan.FromSeconds(json.Get("duration").AsDouble().Or(0d)),
            SyncedLyrics = LyricLineParsers.ParseSyncedLyrics(json
                .Get("syncedLyrics")
                .AsString()
                .Or(string.Empty)
            ),
            PlainLyrics = LyricLineParsers.ParsePlainLyrics(json
                .Get("plainLyrics")
                .AsString()
                .Or(string.Empty)
            )
        };
    }
}