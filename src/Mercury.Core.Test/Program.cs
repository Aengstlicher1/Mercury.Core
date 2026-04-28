using Mercury.Core.Models;
using static Mercury.Core.Models.Enums;


namespace Mercury.Core.Test
{
    public class Program
    {
        public static async Task Main()
        {
            var songs = await YoutubeMusic.Search.SearchCategoryAsync("Runaway", SearchFilter.Songs);
            var test = await YoutubeMusic.Lyrics.GetLyricsAsync((songs![0] as Track)!);
        }
    }
}