using System.Diagnostics;
using Mercury.Core.Models;
using static Mercury.Core.Models.Enums;


namespace Mercury.Core.Test
{
    public class Program
    {
        public static async Task Main()
        {
            var startTime = DateTime.Now;

            var test = await YoutubeMusic.Search.SearchAsync("kanye");
            var gupp = await YoutubeMusic.Browse.GetAsync(test!.First(x => x.Type is MediaCategory.Playlist).Id);
            
            var endTime = DateTime.Now;
            Debug.WriteLine("The test took: " + (endTime - startTime));
        }
    }
}