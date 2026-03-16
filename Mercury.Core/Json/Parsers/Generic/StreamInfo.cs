using Mercury.Core.Models;
using Mercury.Core.Utils;


namespace Mercury.Core.Json.Parsers.Generic
{
    internal static class StreamInfoParser
    {
        public static StreamInfo Parse(JElement format)
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

        private static MuxedStreamInfo ParseMuxed(JElement format)
        {
            return new MuxedStreamInfo()
            {
                ITag = format.Get("itag").AsInt32().Or(0),
                Url = format.Get("url").AsString().Or(string.Empty),
                MimeType = format.Get("mimeType").AsString().Or(string.Empty),
                Bitrate = format.Get("bitrate").AsInt32().Or(0),
                Quality = format.Get("quality").AsString().Or(string.Empty),
                Width = format.Get("width").AsInt32().Or(0),
                Height = format.Get("height").AsInt32().Or(0),
                Fps = format.Get("fps").AsInt32().Or(0),
                QualityLabel = format.Get("qualityLabel").AsString().Or(string.Empty),
                AudioQuality = format.Get("audioQuality").AsString().Or(string.Empty)
            };
        }

        private static VideoStreamInfo ParseVideo(JElement format)
        {
            return new VideoStreamInfo()
            {
                ITag = format.Get("itag").AsInt32().Or(0),
                Url = format.Get("url").AsString().Or(string.Empty),
                MimeType = format.Get("mimeType").AsString().Or(string.Empty),
                Bitrate = format.Get("bitrate").AsInt32().Or(0),
                Quality = format.Get("quality").AsString().Or(string.Empty),
                Width = format.Get("width").AsInt32().Or(0),
                Height = format.Get("height").AsInt32().Or(0),
                Fps = format.Get("fps").AsInt32().Or(0),
                QualityLabel = format.Get("qualityLabel").AsString().Or(string.Empty),
                AverageBitrate = format.Get("averageBitrate").AsInt32().Or(0)
            };
        }

        private static AudioStreamInfo ParseAudio(JElement format)
        {
            return new AudioStreamInfo()
            {
                ITag = format.Get("itag").AsInt32().Or(0),
                Url = format.Get("url").AsString().Or(string.Empty),
                MimeType = format.Get("mimeType").AsString().Or(string.Empty),
                Bitrate = format.Get("bitrate").AsInt32().Or(0),
                Quality = format.Get("quality").AsString().Or(string.Empty),
                AudioQuality = format.Get("audioQuality").AsString().Or(string.Empty),
                AudioSampleRate = format.Get("audioSampleRate").AsInt32().Or(0),
                AverageBitrate = format.Get("averageBitrate").AsInt32().Or(0),
                LoudnessDb = format.Get("loudnessDb").AsDouble().Or(0.0)
            };
        }

        private static StreamInfoType GetInfoType(JElement format)
        {
            if (format.Contains("initRange", out JElement _))
                return StreamInfoType.Muxed;
            else if (format.Contains("audioSampleRate", out JElement _))
                return StreamInfoType.Audio;
            else
                return StreamInfoType.Video;
        }

        internal enum StreamInfoType
        {
            Muxed,
            Video,
            Audio
        }
    }
}
