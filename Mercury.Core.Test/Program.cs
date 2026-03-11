namespace Mercury.Core.Test
{
    public class Program
    {
        public static async Task Main()
        {
            var test = await YoutubeMusic.Search.SearchAsync("Runaway");

            //var client = new YouTubeMusicAPI.YouTubeMusicClient();
            //var test = await client.Search.AllAsync("Runaway");
        }
    }
}