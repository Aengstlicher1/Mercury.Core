namespace Mercury.Core.Models;

public class LyricLine
{
    public string Content { get; init; } = string.Empty;
    
    public TimeSpan Timing { get; init; }
}