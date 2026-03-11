using Mercury.Core.Models;
using YouTubeMusicAPI.Client;

namespace Mercury.Core.Test
{
    public class Program
    {
        public static async Task Main()
        {
            var runaway = await YoutubeMusic.Search.SearchCategoryAsync("Runaway", Enums.SearchFilter.Songs);

            var test = await YoutubeMusic.Player.GetStreamAsync(runaway[0].Id);

            var client = new YouTubeMusicClient();
            client.GetStreamingDataAsync(runaway[0].Id);
        }
    }
}