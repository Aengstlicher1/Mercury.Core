using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Mercury.Core.Json;

namespace Mercury.Core.Utils;

internal static class Syntax
{
    public static T UnlessNull<T>(this T? value, T @default) where T : struct =>
        value ?? @default;
    
    public static T UnlessNull<T>(this T? value,T @default) where T : class =>
        value ?? @default;
    
 
    public static IDisposable GetJson(this string json, out JObject jObject)
    {
        JsonDocument document = JsonDocument.Parse(json);
        jObject = new JObject(document.RootElement);
        return document;
    }
}