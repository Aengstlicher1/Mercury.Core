using Mercury.Core.Json;
using Mercury.Core.Utils;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
namespace Mercury.Core.Utils.Network
{
    internal static class RequestHandler
    {
        readonly static HttpClient httpClient = new HttpClient();

        const string DefaultUserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/142.0.0.0 Safari/537.36";

        internal static string VisitorData { get; set; } = "";

        public static string geoLocation { get; set; } = "US";

        readonly static JsonSerializerOptions jsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        static RequestHandler()
        {
            VisitorData = FetchVisitorDataAsync().GetAwaiter().GetResult();
        }

        public static async Task<string> SendAsync(
            string url,
            HttpMethod method,
            Dictionary<string, object?>? payload = null,
            ClientType clientType = ClientType.None,
            CancellationToken cToken = default
        )
        {
            var client = clientType.ToClient();
            if ( client == null ) throw new ArgumentNullException( "ClientType" );

            Uri requestUri= new Uri(url + client.ApiKey);

            // Preperation
            HttpRequestMessage request = new(method, requestUri);
            Dictionary<string, object?> body = payload ?? [];

            // Client prep
            client.Gl = geoLocation;
            client.VisitorData = VisitorData;
            body["context"] = new Dictionary<string, object> { ["client"] = client };
            request.Headers.Add("User-Agent", client.UserAgent);

            if (client.Headers != null)
                foreach (var header in client.Headers)
                    request.Headers.Add(header.Key, header.Value);

            if (body.Count != 0)
            {
                string json = JsonSerializer.Serialize(body, jsonOptions);
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            }

            #if DEBUG
            var test = await request.Content.ReadAsStringAsync();
            #endif

            // Send
            HttpResponseMessage response = httpClient.SendAsync(request, cToken).ConfigureAwait(false).GetAwaiter().GetResult();

            string content = await response.Content.ReadAsStringAsync(cToken).ConfigureAwait(false);

            // Secure
            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException($"HTTP request failed.", new(content), response.StatusCode);

            return content;
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
            return new JElement(doc.RootElement)
                .Get("responseContext")
                .Get("visitorData")
                .AsString()!;
        }
    }
}
