using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mercury.Core.Models
{
    public abstract class MediaInfo
    {
        public abstract Media Base { get; }
    }

    public class PlaylistInfo(Playlist original) : MediaInfo
    {
        public IReadOnlyList<PlaylistTrack> Tracks { get; init; } = new List<PlaylistTrack>();
        public int TracksCount => Tracks.Count;
        
        public override Playlist Base { get; } = original;
    }

    public class PlaylistTrack : Track;
}
