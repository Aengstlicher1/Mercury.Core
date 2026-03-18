using Mercury.Core.Models;
using static Mercury.Core.Models.Enums;


namespace Mercury.Core.Test
{
    public class Program
    {
        public static async Task Main()
        {
            var medias = await YoutubeMusic.Search.SearchCategoryAsync("Runaway", SearchFilter.Episodes);

            var media = medias!.First();

            var test = await YoutubeMusic.Browse.GetAsync(media.Id, media.Type);
        }
    }
}