using Mercury.Core.Models;


namespace Mercury.Core.Test
{
    public class Program
    {
        public static async Task Main()
        {
            var medias = await YoutubeMusic.Search.SearchAsync("Runaway");

            var streamingData = await YoutubeMusic.Player.GetStreamAsync(medias!.First().Id);

            foreach (var stream in streamingData.Streams)
            {
                Console.WriteLine(stream.Url);
            }
        }
    }
}