using Mercury.Core.Services;

namespace Mercury.Core
{
    public static class YoutubeMusic
    {
        public static readonly SearchService Search = new SearchService();

        public static readonly PlayerService Player = new PlayerService();

        public static readonly LyricsService Lyrics = new LyricsService();

        public static readonly BrowseService Browse = new BrowseService();
        
        public static readonly UserService   User   = new UserService();
    }
}
