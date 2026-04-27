using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

namespace Mercury.Core.Models
{
    public class Lyrics
    {
        public Collection<LyricLine>? PlainLyrics { get; set; }
        
        // LRC format with timestamps
        public Collection<LyricLine>? SyncedLyrics { get; set; } 
        
        public string? TrackName { get; set; }
        
        public string? ArtistName { get; set; }
        
        public string? AlbumName { get; set; }
        
        public TimeSpan Duration { get; set; }
    }
}
