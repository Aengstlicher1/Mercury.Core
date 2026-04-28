using System.Collections.ObjectModel;

namespace Mercury.Core.Models.Explore;

public class ReleasesCategory
{
    public string Name { get; init; } = string.Empty;
    public Album[] Content { get; init; } = Array.Empty<Album>();
}

public class GenresCategory
{
    public string Name { get; init; } = string.Empty;
    public Genre[] Content { get; init; } = Array.Empty<Genre>();
}

public class TrendingCategory
{
    public string Name { get; init; } = string.Empty;
    public Track[] Content { get; init; } = Array.Empty<Track>();
}

public class NewMusicVideosCategory
{
    public string Name { get; init; } = string.Empty;
    public Video[] Content { get; init; } = Array.Empty<Video>();
}