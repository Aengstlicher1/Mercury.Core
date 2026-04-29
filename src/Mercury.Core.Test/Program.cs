using System.Diagnostics;
using Mercury.Core.Models;
using static Mercury.Core.Models.Enums;


namespace Mercury.Core.Test
{
    public class Program
    {
        private const string cookies =
            "APISID=CF6r2CLvAI9Bshho/AG1Zvr8WJG3klV6Ea; " +
            "HSID=Aj93ya0BC9rANdsyf; " +
            "LOGIN_INFO=AFmmF2swRAIgEce_jHD1cbjw4h1WQNpUdklbQZFGRar40x1S-BmLcF4CIA1rzw6K1RtoAYVL2BG03VuOW2jlu-EBIQ8WyCdtvjJG:QUQ3MjNmeEVMN2ZUWmlmcVc2TDhsTzcwN1N6aVAySnRmaXNyMWZCeWFFZlpLV1RsMXVtMHd2SVlfZGw4akViTW9SU1VPd05oS3I2NkMwSFpLSXFlZVEydnY1NnpCLWxnTTdPYlRvUTd1YzNkWjd4eTUxUXJQejY2d3ljNkFlYXQxdjYwZHRYNzRzdklPb2hKYm1KZW9SdW4yUDJtSDdZUDJR; " +
            "PREF=repeat=NONE&autoplay=true&tz=Europe.Berlin&guide_collapsed=true&f5=30000; " +
            "SAPISID=6SOOtHUUNnjx5SC1/AybW0LlowHppFDQZ4; " +
            "SID=g.a0009QiLsr7UUhjhHmxF7gziel8blyvEPCwkIsHYxXRdvW-0jOGXoK2vm7XvmmoUM_b3Seb1lwACgYKAdASARUSFQHGX2MivrBXq0RBNjtQWr2WTFKn_hoVAUF8yKpdeSYx9LY8qrjq0kqd0Xzp0076; " +
            "SIDCC=AKEyXzWtUc6sOrY9MdFqQ9CKVF5SpWnE6hpaGuz4nwOZoYqvVlgSQS4gyZpceh0Iw8X-iSqWsg; " +
            "SOCS=CAISNQgREitib3FfaWRlbnRpdHlmcm9udGVuZHVpc2VydmVyXzIwMjYwMjIzLjA3X3AwGgJkZSACGgYIgPf4zAY; " +
            "SSID=AAHkPrVSX3GGN5dfY; " +
            "VISITOR_INFO1_LIVE=_4r90xBHvKo; " +
            "VISITOR_PRIVACY_METADATA=CgJERRIEEgAgaw%3D%3D; " +
            "__Secure-1PAPISID=6SOOtHUUNnjx5SC1/AybW0LlowHppFDQZ4; " +
            "__Secure-1PSID=g.a0009QiLsr7UUhjhHmxF7gziel8blyvEPCwkIsHYxXRdvW-0jOGXgeIsCQOn_7h58IUxlV3HrQACgYKAVgSARUSFQHGX2Miptzi6wusPAZCj73OsBjhXhoVAUF8yKrPEowWP6-hoACKwWniegJO0076; " +
            "__Secure-1PSIDCC=AKEyXzXi8hhNP8W1rsQGE9d0n9qx_sH2WARPOkDjgt84akb8P1m7F808KON6PtD7fsW7oXA0kQ; " +
            "__Secure-1PSIDTS=sidts-CjQBhkeRd5HzgihuQ03xOrCtgOEZgimjA7sYXp2SNnkWOFjljKKyvZWoJWH9Dko0vgIpMWIuEAA; " +
            "__Secure-3PAPISID=6SOOtHUUNnjx5SC1/AybW0LlowHppFDQZ4; " +
            "__Secure-3PSID=g.a0009QiLsr7UUhjhHmxF7gziel8blyvEPCwkIsHYxXRdvW-0jOGX5DUu8lhJaeoOLbYzsgQ0DQACgYKAXgSARUSFQHGX2Mi6B02Y42Nx9lxgYnxRWe9khoVAUF8yKr61WKDEBkxa1xu4oHVsOXF0076; " +
            "__Secure-3PSIDCC=AKEyXzVFuXbzzT5uJ-ja3etgvSqk_wBX3F7QA9T5C8I8hBnVeQHHIgEerMje3E7sByLOcE7hzYk; " +
            "__Secure-3PSIDTS=sidts-CjQBhkeRd5HzgihuQ03xOrCtgOEZgimjA7sYXp2SNnkWOFjljKKyvZWoJWH9Dko0vgIpMWIuEAA; " +
            "__Secure-BUCKET=CJwF; " +
            "__Secure-ROLLOUT_TOKEN=CIeN6JDAxoGrdxDiufqL-ICTAxjHtM6rjvyTAw%3D%3D; " +
            "__Secure-YNID=18.YT=Q8TKNW2qTh3w91_GG_qYTazKu4L6CmFMmKwDHWf60yZOSIOOBdGPCnTUMmuStp3VQXJYpNMy2tCVY_pJFGHxnJwUStzFAWWX1io5pja6VAmd66mH0RBgtmdBeYJh9m1D76NgyWjYaU9_IEHJZ2-rCxRlCrxkcFEGi67_yvduWHS5Ir1KYno8mH_hOnz_TAWgS0LqnYqhEkcyZPXJ_2QCZJWqWvnyR_UunWIGZw81iBbduuxacWkLktte97cVYyTjihFxqjdoshwmI3e-6Sl4PCkyFbDDI7SMLpteF-vOav0awXXQ9fWPLV9Xbn67KI6y0F4iTNzvN9CCWfFY9K2PPw";
        
        public static async Task Main()
        {
            YoutubeMusic.User.SetTokens(cookies);
            var startTime = DateTime.Now;
            
            var test = await YoutubeMusic.User.GetLibraryPlaylistsAsync();
            
            var endTime = DateTime.Now;
            Debug.WriteLine("The test took: " + (endTime - startTime));
        }
    }
}