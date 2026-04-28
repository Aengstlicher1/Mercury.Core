using System.Drawing;

namespace Mercury.Core.Models.Explore;

public class Genre
{
    public string Title { get; set; } = string.Empty;
    
    public Color Color { get; set; }
    
    internal string BrowseParam { get; set; } = string.Empty;
}