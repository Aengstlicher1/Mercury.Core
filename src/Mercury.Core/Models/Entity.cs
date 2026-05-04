namespace Mercury.Core.Models;

public class Entity
{
    public Entity(string name, string id)
    {
        Name = name;
        Id = id;
    }

    public Entity()
    {
        
    }
    
    public string Name { get; set; } = string.Empty;
    
    public string Id { get; set; } = string.Empty;

    public override string ToString()
    {
        return Name;
    }
    

    public static readonly Entity YoutubeMusic
        = new Entity()
        {
            Name = "Youtube Music",
            Id = "UCStaiwu-FAgp_RC_tBiLh9A"
        };
}