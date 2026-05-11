using Mercury.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mercury.Core.Json.Parsers.Generic
{
    internal static class IdParser
    {
        internal static string ParseBrowse(JObject renderer)
            => renderer
                .Get("navigationEndpoint")
                .Get("browseEndpoint")
                .Get("browseId")
                .AsString()
                .UnlessNull(string.Empty);

        internal static string ParseWatch(JObject renderer)
            => renderer
                .Get("flexColumns")
                .GetAt(0)
                .Get("musicResponsiveListItemFlexColumnRenderer")
                .Get("text")
                .Get("runs")
                .GetAt(0)
                .Get("navigationEndpoint")
                .Get("watchEndpoint")
                .Get("videoId")
                .AsString()
                .UnlessNull(string.Empty);
    }
}
