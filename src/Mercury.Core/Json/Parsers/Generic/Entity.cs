using Mercury.Core.Models;
using Mercury.Core.Utils;

namespace Mercury.Core.Json.Parsers.Generic;

internal static class EntityParser
{
    public static Entity Parse(JElement element)
    {
        return new Entity()
        {
            Name = element.Get("text").AsString().Or(string.Empty),
            Id = element
                .Get("navigationEndpoint")
                .Get("browseEndpoint")
                .Get("browseId")
                .AsString()
                .Or(string.Empty)
        };
    }
}