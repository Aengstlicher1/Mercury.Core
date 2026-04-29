namespace Mercury.Core.Models.User;

public class LibraryPlaylist : Playlist
{
    public bool IsAutoPlaylist { get; init; } = false;
}