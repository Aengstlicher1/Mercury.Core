using Mercury.Core.Json.Parsers.Generic;
using Mercury.Core.Models;
using Mercury.Core.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mercury.Core.Json.Parsers.Browse.Info
{
    internal static class PlaylistInfoParser
    {
        public static PlaylistInfo Parse(JObject renderer, Playlist original)
        {
            var tracks = renderer
                .Get("contents")
                .AsArray()
                .UnlessNull(JArray.Empty);
            
            Collection<PlaylistTrack> playlistTracks = [];
            foreach (var track in tracks)
            {
                playlistTracks.Add(
                    ParsePlaylistTrack(track.Get("musicResponsiveListItemRenderer"))
                );
            }

            return new PlaylistInfo(original)
            {
                Tracks = playlistTracks.ToArray()
            };
        }

        public static PlaylistTrack ParsePlaylistTrack(JObject renderer)
        {
            var thumbnails = ThumbnailParser.Parse(ThumbnailParser.GetThumbRenderer(renderer));

            var flex = FlexColumnParser.GetFlex(renderer);
            var fix = FixedColumnParser.GetFix(renderer);

            return new PlaylistTrack()
            {
                Id = renderer.Get("playlistItemData").Get("videoId").AsString().UnlessNull(string.Empty),
                Title = FlexColumnParser.Parse(flex, 0),
                Artist = EntityParser.Parse(flex[1].Get("runs").GetAt(0)),
                Duration = FixedColumnParser.Parse(fix, 0),
                Thumbnails = thumbnails
            };
        }
    }
}
