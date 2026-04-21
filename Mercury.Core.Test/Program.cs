using Mercury.Core.Models;
using static Mercury.Core.Models.Enums;


namespace Mercury.Core.Test
{
    public class Program
    {
        public static async Task Main()
        {
            var medias = await YoutubeMusic.Search.SearchCategoryAsync("test", SearchFilter.CommunityPlaylists);

            var media = medias!.First(m => m.Type == MediaCategory.Playlist);
            
            var test = await YoutubeMusic.Browse.GetInfoAsync(media);
        }
    }
}