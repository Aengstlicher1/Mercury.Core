using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mercury.Core.Models
{
    public struct Thumbnail
    {
        public Thumbnail(string url, Dimensions size)
        {
            Url = url;
            Size = size;
        }
        public Thumbnail() { }

        public string Url { get; set; }
        public Dimensions Size { get; set; }

        public override string ToString()
        {
            return $"{Size} - {Url.Substring(0, 24)}...";
        }
    }

    public struct Dimensions
    {
        public Dimensions (int height, int width)
        {
            Height = height;
            Width = width;
        }

        public int Width { get; set; } = 0;
        public int Height { get; set; } = 0;

        public override string ToString()
        {
            return $"{Width}x{Height}";
        }
    }
}
