namespace Mercury.Core.Models.Explore;

public class ExploreFeed
{
    public required ReleasesCategory Releases { get; init; }
    
    public required GenresCategory Genres { get; init; }
    
    public required TrendingCategory Trending { get; init; }
    
    public required NewMusicVideosCategory NewMusicVideos { get; init; }
}