using System.Diagnostics;
using Mercury.Core.Models;
using static Mercury.Core.Models.Enums;


namespace Mercury.Core.Test
{
    public class Program
    {
        public static async Task Main()
        {
            YoutubeMusic.User.SetTokens("");
            var startTime = DateTime.Now;
            
            var test = await YoutubeMusic.User.GetLibraryPlaylistsAsync();
            
            var endTime = DateTime.Now;
            Debug.WriteLine("The test took: " + (endTime - startTime));
        }
    }
}