namespace Mercury.Core.Models.Explore;

public class ExploreFeed
{
    public NewReleasesCategory Releases { get; init; }
    
    public GenresCategory Genres { get; init; }
    
    public TrendingCategory Trending { get; init; }
    
    public NewMusicVideosCategory NewMusicVideos { get; init; }
}