using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mercury.Core.Models
{
    public struct ThumbArray : IEnumerable
    {
        public ThumbArray(Thumbnail[] thumbnails) 
        {
            Thumbs = thumbnails;
        }

        public Thumbnail[] Thumbs { get; set; }

        public Thumbnail HighestRes => Thumbs.MaxBy(t => t.Size.Height);
        public Thumbnail LowestRes => Thumbs.MinBy(t => t.Size.Height);

        public IEnumerator GetEnumerator()
            => Thumbs.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public static ThumbArray Empty { get => new ThumbArray(); }
    }

    public struct Thumbnail
    {
        public Thumbnail(string url, Dimensions size)
        {
            Url = url;
            Size = size;
        }
        public Thumbnail() { }

        public string Url { get; set; } = "";
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
