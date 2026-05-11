using Mercury.Core.Models;
using Mercury.Core.Utils;

namespace Mercury.Core.Json.Parsers.Generic;

internal static class EntityParser
{
    public static Entity Parse(JObject @object)
    {
        return new Entity()
        {
            Name = @object.Get("text").AsString().UnlessNull(string.Empty),
            Id = @object
                .Get("navigationEndpoint")
                .Get("browseEndpoint")
                .Get("browseId")
                .AsString()
                .UnlessNull(string.Empty)
        };
    }
}