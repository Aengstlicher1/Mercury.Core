using System.Collections.ObjectModel;

namespace Mercury.Core.Models.User;

public class UserLibrary
{
    public string Title {get; init; } = string.Empty;

    public Collection<LibraryPlaylist> Playlists { get; init; } = new();
}