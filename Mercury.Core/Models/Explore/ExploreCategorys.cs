using System.Collections.ObjectModel;

namespace Mercury.Core.Models.Explore;

public class ReleasesCategory
{
    public string Name { get; init; }
    public Album[] Content { get; init; }
}

public class GenresCategory
{
    public string Name { get; init; }
    public Genre[] Content { get; init; }
}

public class TrendingCategory
{
    public string Name { get; init; }
    public Track[] Content { get; init; }
}

public class NewMusicVideosCategory
{
    public string Name { get; init; }
    public Video[] Content { get; init; }
}