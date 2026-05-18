using System.ComponentModel;
using System.Net.Http.Headers;
using Mercury.Core.Json;
using Mercury.Core.Utils;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
namespace Mercury.Core.Network
{
    internal static class RequestHandler
    {
        internal static string VisitorData { get; set; } = "";

        public readonly static HttpClient httpClient = new HttpClient();
        public static int RequestRetryAmount { get; set; } = 5;
        public static string geoLocation { get; set; } = "US";

        readonly static JsonSerializerOptions jsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        private static async Task<string> SendAsync
        (
            string url,
            HttpMethod method,
            Dictionary<string, object?>? payload = null,
            ClientType clientType = ClientType.None,
            CancellationToken cToken = default
        )
        {
            var client = clientType.GetClient();
            if (client == null) throw new ArgumentNullException("ClientType");

            Dictionary<string, object?> body = payload ?? [];

            HttpResponseMessage response = await BuildAndSendAsync(url, client, body, cToken);
            string content = await response.Content.ReadAsStringAsync(cToken).ConfigureAwait(false);

            // Retry on bot detection
            for (int i = 0; i < RequestRetryAmount && IsBotResponse(response, content); i++)
            {
                VisitorData = await FetchVisitorDataAsync();
                response = await BuildAndSendAsync(url, client, body, cToken);
                content = await response.Content.ReadAsStringAsync(cToken).ConfigureAwait(false);
            }

            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException("HTTP request failed.", new(content), response.StatusCode);

            return content;
        }

        private static Client? GetClient(this ClientType type) =>
            type switch
            {
                ClientType.None => null,
                ClientType.WebMusic => Client.WebMusic.Clone(),
                ClientType.IOSMusic => Client.IOSMusic.Clone(),
                ClientType.Web => Client.Web.Clone(),
                ClientType.Android => Client.Android.Clone(),
                ClientType.AndroidVR => Client.AndroidVR.Clone(),
                _ => throw new InvalidEnumArgumentException($"Invalid client type: {type}.")
            };
        
        private static async Task<HttpResponseMessage> BuildAndSendAsync(
            string url,
            Client client,
            Dictionary<string, object?> body,
            CancellationToken cToken
        )
        {
            if (string.IsNullOrWhiteSpace(VisitorData))
                VisitorData = await FetchVisitorDataAsync(cToken);

            Uri requestUri = YoutubeMusic.User.IsAuthenticated
                ? new Uri(url + "?prettyPrint=false")
                : new Uri(url + client.ApiKey);
            HttpRequestMessage request = new(HttpMethod.Post, requestUri);

            client.Gl = geoLocation;
            client.VisitorData = VisitorData;
            body["context"] = new Dictionary<string, object> { ["client"] = client };

            request.Headers.Add("User-Agent", client.UserAgent);

            if (client.Headers != null)
                foreach (var header in client.Headers)
                    request.Headers.Add(header.Key, header.Value);

            // Handle authenticated requests with additional headers, while keeping the unauthenticated requests working
            if (YoutubeMusic.User.IsAuthenticated)
            {
                request.Headers.TryAddWithoutValidation("Authorization", YoutubeMusic.User.GenerateSapiSidHash());
                request.Headers.Add("Cookie", YoutubeMusic.User.CurrentAuthTokens!.ToCookieHeader());
                request.Headers.Add("Origin", "https://music.youtube.com");
                request.Headers.Add("X-Origin", "https://music.youtube.com");
                request.Headers.Add("Referer", "https://music.youtube.com/");
                request.Headers.Add("X-Goog-AuthUser", "0");
                request.Headers.Add("X-Youtube-Bootstrap-Logged-In", "true");
                request.Headers.Add("X-Youtube-Client-Name", "67");
                request.Headers.Add("X-Youtube-Client-Version", "1.20260426.12.00");
            }
            
            if (body.Count != 0)
            {
                string json = JsonSerializer.Serialize(body, jsonOptions);
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            }

            cToken.ThrowIfCancellationRequested();

            return await httpClient.SendAsync(request, cToken).ConfigureAwait(false);
        }

        private static bool IsBotResponse(HttpResponseMessage response, string content)
        {
            return response.StatusCode == System.Net.HttpStatusCode.Forbidden
                || content.Contains("You need to log in", StringComparison.OrdinalIgnoreCase)
                || content.Contains("confirm you're not a bot", StringComparison.OrdinalIgnoreCase);
        }


        public static async Task<string> GetAsync(
            string url,
            Dictionary<string, object?>? payload = null,
            ClientType clientType = ClientType.None,
            CancellationToken cToken = default
        )
        {
            return await SendAsync( url, HttpMethod.Get, payload, clientType, cToken).ConfigureAwait(false);
        }


        public static async Task<string> PostAsync(
            string url,
            Dictionary<string, object?>? payload = null,
            ClientType clientType = ClientType.None,
            CancellationToken cToken = default
        )
        {
            return await SendAsync( url, HttpMethod.Post, payload, clientType, cToken).ConfigureAwait(false);
        }

        public static async Task<string> FetchVisitorDataAsync(CancellationToken ct = default)
        {
            var payload = new Dictionary<string, object>
            {
                ["context"] = new Dictionary<string, object>
                {
                    ["client"] = new Dictionary<string, object>
                    {
                        ["clientName"] = "WEB",
                        ["clientVersion"] = "2.20250101.00.00",
                        ["hl"] = "en",
                        ["gl"] = "US"
                    }
                }
            };

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Post, "https://www.youtube.com/youtubei/v1/visitor_id")
            {
                Content = content
            };
            request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/142.0.0.0 Safari/537.36");

            var response = await httpClient.SendAsync(request, ct);
            var responseJson = await response.Content.ReadAsStringAsync(ct);

            using var doc = JsonDocument.Parse(responseJson);
            return new JObject(doc.RootElement)
                .Get("responseContext")
                .Get("visitorData")
                .AsString()!;
        }
    }
}
