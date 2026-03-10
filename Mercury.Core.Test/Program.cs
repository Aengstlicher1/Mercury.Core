using Mercury.Core.Http.Authentication;

namespace Mercury.Core.Test
{
    public class Program
    {
        public static async Task Main()
        {
            var config = new YouTubeMusicConfig()
            {
                GeographicalLocation = "DE",
                Authenticator = new AnonymousAuthenticator(),
            };
            var client = new YouTubeMusicClient(config);

            var page = await client.Search.AllAsync("Runaway");
            var songs = page.Results.ToList();
        }
    }
}