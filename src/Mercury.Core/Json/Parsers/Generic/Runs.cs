using Mercury.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mercury.Core.Json.Parsers.Generic
{
    internal static class RunsParser
    {
        public static string Parse(JArray runs, int runIndex = 0)
        {
            return runs[runIndex].Get("text").AsString().UnlessNull(string.Empty);
        }

        public static JArray GetRuns(JObject parent)
        {
            return parent.Get("runs").AsArray().UnlessNull(JArray.Empty);
        }
    }
}
