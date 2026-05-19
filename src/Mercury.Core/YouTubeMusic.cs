using Mercury.Core.Models;
using Mercury.Core.Services;

namespace Mercury.Core
{
    public static class YoutubeMusic
    {
        /// <summary>
        /// The Search service to get search results from YoutubeMusic
        /// </summary>
        public static readonly SearchService Search = new SearchService();

        /// <summary>
        /// The Player service to get streams from YoutubeMusic
        /// </summary>
        public static readonly PlayerService Player = new PlayerService();

        /// <summary>
        /// The Lyrics service to get lyrics from LRCLIB
        /// </summary>
        public static readonly LyricsService Lyrics = new LyricsService();

        /// <summary>
        /// The Browse service to get single medias from their id,
        /// media infos (e.g. <see cref="PlaylistInfo"/>)
        /// and the ExplorePage
        /// </summary>
        public static readonly BrowseService Browse = new BrowseService();
        
        /// <summary>
        /// The User service to authenticate the User and get user specific data
        /// </summary>
        public static readonly UserService User = new UserService();
    }
}
