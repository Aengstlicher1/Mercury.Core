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

        public static string geoLocation { get; set; } = "US";

        readonly static JsonSerializerOptions jsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };


        public static async Task<string> SendAsync(
            string url,
            HttpMethod method,
            Dictionary<string, object?>? payload = null,
            ClientType clientType = ClientType.None,
            CancellationToken cToken = default
        )
        {
            Uri requestUri = new Uri(url);

            // Preperation
            HttpRequestMessage request = new(method, requestUri);
            Dictionary<string, object?> body = payload ?? [];

            if (clientType.ToClient() is Client client)
            {
                client.Gl = geoLocation;
                body["context"] = new { client };
                request.Headers.Add("User-Agent", client.UserAgent);
            }
            else
            {
                request.Headers.Add("User-Agent", DefaultUserAgent);
            }

            if (body.Count != 0)
            {
                string json = JsonSerializer.Serialize(body, jsonOptions);
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            }

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
    }
}
