using Mercury.Core.Json;
using Mercury.Core.Json.Parsers.Generic;
using Mercury.Core.Models;
using Mercury.Core.Utils;
using Mercury.Core.Utils.Network;
using System.Collections.ObjectModel;
using System.Data;

namespace Mercury.Core.Services
{
    public class PlayerService
    {
        public async Task<StreamingData?> GetStreamAsync(string videoId, CancellationToken cToken = default)
        {
            Dictionary<string, object?> payload = new()
            {
                { "videoId", videoId }
            };

            var response = await RequestHandler.PostAsync(Endpoints.Player, payload, ClientType.AndroidVR, cToken);

            using IDisposable _ = response.ParseJson(out var json);

            JArray formats = json
                .Get("streamingData")
                .Get("formats")
                .AsArray()
                .Or(JArray.Empty);

            JArray adaptiveFormats = json
                .Get("streamingData")
                .Get("adaptiveFormats")
                .AsArray()
                .Or(JArray.Empty);

            var allFormats = formats.Concat(adaptiveFormats);

            Collection<StreamInfo> infos = new ();

            foreach (var format in allFormats)
            {
                infos.Add(StreamInfoParser.Parse(format));
            }

            var data = StreamingDataParser.Parse(json);
            data.Streams = infos.ToArray();

            return data;
        } 
    }
}
