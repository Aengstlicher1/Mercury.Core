using Mercury.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Mercury.Core.Models.Enums;

namespace Mercury.Core.Utils
{
    public static class MediaExtensions
    {
        internal static bool IsNextEndpoint(this MediaCategory category)
        {
            return category switch
            {
                MediaCategory.Song => true,
                MediaCategory.Video => true,
                MediaCategory.Episode => true,
                _ => false
            };
        }
    }
}
