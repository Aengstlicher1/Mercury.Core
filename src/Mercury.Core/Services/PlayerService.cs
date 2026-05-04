using Mercury.Core.Json;
using Mercury.Core.Json.Parsers.Generic;
using Mercury.Core.Models;
using Mercury.Core.Utils;
using Mercury.Core.Network;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Mercury.Core.Services
{
    /// <summary>
    /// Resolves playable audio and video stream data for a given YouTube video id
    /// by querying the YouTube Music player endpoint.
    /// </summary>
    /// <remarks>
    /// This service is exposed through the static <c>YoutubeMusic.Player</c> facade.
    /// The resolved <see cref="StreamingData"/> is consumed by the Avalonia UI layer's
    /// <c>PlayerService</c> to feed streams into LibVLCSharp.
    /// </remarks>
    public sealed class PlayerService
    {
        /// <summary>
        /// Asynchronously retrieves all available stream formats for the given
        /// <paramref name="videoId"/> and returns them as a <see cref="StreamingData"/>
        /// object.
        /// </summary>
        /// <remarks>
        /// Both the <c>formats</c> (muxed) and <c>adaptiveFormats</c> (separate
        /// audio/video) arrays from the player response are merged and parsed.
        /// Individual formats that fail to parse are skipped and logged via
        /// <see cref="Debug.WriteLine"/>.
        /// </remarks>
        /// <param name="videoId">The 11-character YouTube video id of the track</param>
        /// <param name="cToken"><see cref="CancellationToken"/> to cancel the request</param>
        /// <returns>
        /// A <see cref="StreamingData"/> containing the track title and all
        /// successfully parsed <see cref="StreamInfo"/> entries
        /// </returns>
        public async Task<StreamingData> GetStreamDataAsync(string videoId, CancellationToken cToken = default)
        {
            Dictionary<string, object?> payload = new()
            {
                { "videoId", videoId }
            };

            var response = await RequestHandler.PostAsync(Endpoints.Player, payload, ClientType.AndroidVR, cToken);

            using IDisposable _ = response.ParseJson(out var json);

            // Parse both muxed and adaptive format arrays from the streaming data.
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

            // Combine both format lists for a single pass.
            var allFormats = formats.Concat(adaptiveFormats);

            Collection<StreamInfo> infos = new();

            foreach (var format in allFormats)
            {
                try
                {
                    infos.Add(StreamInfoParser.Parse(format));
                }
                catch (Exception ex)
                {
                    // Skip malformed or unsupported format entries and log the reason.
                    Debug.WriteLine($"{ex.Message}", "STREAM");
                }
            }

            var data = StreamingDataParser.Parse(json);
            data.Streams = infos.ToArray();

            return data;
        }
    }
}
