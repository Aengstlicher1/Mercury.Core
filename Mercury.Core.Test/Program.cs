using Mercury.Core.Models;


namespace Mercury.Core.Test
{
    public class Program
    {
        public static async Task Main()
        {
            var medias = await YoutubeMusic.Search.SearchCategoryAsync("Runaway", Enums.SearchFilter.Songs);

            var stream = await YoutubeMusic.Player.GetStreamAsync(medias!.First().Id);
        }
    }
}