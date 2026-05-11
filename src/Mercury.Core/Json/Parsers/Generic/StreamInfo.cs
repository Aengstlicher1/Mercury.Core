using Mercury.Core.Models;
using Mercury.Core.Utils;


namespace Mercury.Core.Json.Parsers.Generic
{
    internal static class StreamInfoParser
    {
        public static StreamInfo Parse(JObject format)
        {
            var type = GetInfoType(format);

            return type switch
            {
                StreamInfoType.Muxed => ParseMuxed(format),
                StreamInfoType.Video => ParseVideo(format),
                StreamInfoType.Audio => ParseAudio(format),
                _ => throw new InvalidOperationException($"Unknown or null StreamInfoType: {type}")
            };
        }

        private static MuxedStreamInfo ParseMuxed(JObject format)
        {
            return new MuxedStreamInfo()
            {
                ITag = format.Get("itag").AsInt().UnlessNull(0),
                Url = format.Get("url").AsString().UnlessNull(string.Empty),
                MimeType = format.Get("mimeType").AsString().UnlessNull(string.Empty),
                Bitrate = format.Get("bitrate").AsInt().UnlessNull(0),
                Quality = format.Get("quality").AsString().UnlessNull(string.Empty),
                Size = new Dimensions()
                {
                    Height = format.Get("height").AsInt().UnlessNull(0),
                    Width = format.Get("width").AsInt().UnlessNull(0)
                },
                Fps = format.Get("fps").AsInt().UnlessNull(0),
                QualityLabel = format.Get("qualityLabel").AsString().UnlessNull(string.Empty),
                AudioQuality = format.Get("audioQuality").AsString().UnlessNull(string.Empty)
            };
        }

        private static VideoStreamInfo ParseVideo(JObject format)
        {
            return new VideoStreamInfo()
            {
                ITag = format.Get("itag").AsInt().UnlessNull(0),
                Url = format.Get("url").AsString().UnlessNull(string.Empty),
                MimeType = format.Get("mimeType").AsString().UnlessNull(string.Empty),
                Bitrate = format.Get("bitrate").AsInt().UnlessNull(0),
                Quality = format.Get("quality").AsString().UnlessNull(string.Empty),
                Size = new Dimensions()
                {
                    Height = format.Get("height").AsInt().UnlessNull(0),
                    Width = format.Get("width").AsInt().UnlessNull(0)
                },
                Fps = format.Get("fps").AsInt().UnlessNull(0),
                QualityLabel = format.Get("qualityLabel").AsString().UnlessNull(string.Empty),
                AverageBitrate = format.Get("averageBitrate").AsInt().UnlessNull(0)
            };
        }

        private static AudioStreamInfo ParseAudio(JObject format)
        {
            var sampleRateStr = format.Get("audioSampleRate").AsString().UnlessNull(string.Empty);

            return new AudioStreamInfo()
            {
                ITag = format.Get("itag").AsInt().UnlessNull(0),
                Url = format.Get("url").AsString().UnlessNull(string.Empty),
                MimeType = format.Get("mimeType").AsString().UnlessNull(string.Empty),
                Bitrate = format.Get("bitrate").AsInt().UnlessNull(0),
                Quality = format.Get("quality").AsString().UnlessNull(string.Empty),
                AudioQuality = format.Get("audioQuality").AsString().UnlessNull(string.Empty),
                AudioSampleRate = int.Parse(sampleRateStr),
                AverageBitrate = format.Get("averageBitrate").AsInt().UnlessNull(0)
            };
        }

        private static StreamInfoType GetInfoType(JObject format)
        {
            if (format.Contains("xtags", out JObject _))
                return StreamInfoType.Muxed;
            else if (format.Get("mimeType").AsString().UnlessNull(string.Empty).Contains("video"))
                return StreamInfoType.Video;
            else if (format.Get("mimeType").AsString().UnlessNull(string.Empty).Contains("audio"))
                return StreamInfoType.Audio;
            else
                throw new ArgumentException("WTF happened!?");
        }

        internal enum StreamInfoType
        {
            Muxed,
            Video,
            Audio
        }
    }
}
