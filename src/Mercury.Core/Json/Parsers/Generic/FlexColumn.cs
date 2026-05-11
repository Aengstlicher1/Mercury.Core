using Mercury.Core.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mercury.Core.Json.Parsers.Generic
{
    internal static class FlexColumnParser
    {
        internal static string Parse(JArray flex, int colIndex, int runIndex = 0)
        {
            var runs = flex[colIndex]
                .Get("musicResponsiveListItemFlexColumnRenderer")
                .Get("text")
                .Get("runs")
                .AsArray()
                .UnlessNull(JArray.Empty);
            
            return RunsParser.Parse(runs, runIndex);
        }

        internal static JArray GetFlex(JObject renderer)
            =>  renderer
                .Get("flexColumns")
                .AsArray()
                .UnlessNull(JArray.Empty);
    }
}
