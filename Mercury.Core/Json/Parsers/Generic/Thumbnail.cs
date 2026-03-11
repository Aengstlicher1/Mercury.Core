using Mercury.Core.Models;
using Mercury.Core.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mercury.Core.Json.Parsers.Generic
{
    internal static class ThumbnailParser
    {
        internal static Thumbnail[] Parse(JArray thumbs)
        {
            if (thumbs != null && thumbs.Length > 0)
            {
                Collection<Thumbnail> thumbnails = new ();

                for (int i = 0; i < thumbs.Length; i++)
                {
                    thumbnails.Add(
                        new Thumbnail()
                        {
                            Url = thumbs[i].Get("url").AsString().Or(string.Empty),
                            Size = new Dimensions()
                            {
                                Width = thumbs[i].Get("width").AsInt32().Or(0),
                                Height = thumbs[i].Get("height").AsInt32().Or(0)
                            }
                        }
                    );
                }

                return thumbnails.ToArray();
            }
            else
                return new Thumbnail[0];
        }
    }
}
