using System.Diagnostics;
using System.Text.Json;

namespace Mercury.Core.Json;

internal readonly struct JObject(JsonElement element)
{
    private bool IsNotDefined => element.ValueKind == JsonValueKind.Undefined;
    private bool IsJArray => element.ValueKind == JsonValueKind.Array;

    
    public JObject Get(string key)
    {
        if (!IsNotDefined && element.TryGetProperty(key, out JsonElement val))
            return new JObject(val);
        return new JObject(default);
    }
    
    public JObject GetAt(
        int index)
    {
        if (IsJArray && index >= 0 && index < (IsJArray ? element.GetArrayLength() : -1))
            return new JObject(element[index]);
        return new JObject(default);
    }
    
    public bool Contains(string key, out JObject value)
    {
        if (IsNotDefined)
        {
            value = new JObject(default);
            return false;
        }
        bool result = element.TryGetProperty(key, out JsonElement val);
        value = new JObject(val);
        return result;
    }
    
    public JArray? AsArray()
        => IsJArray ? new JArray(element) : null;


    public string? AsString()
    {
        if (element.ValueKind == JsonValueKind.String && element.GetString() is { } value)
            return value;
        return null;
    }
    
    public int? AsInt()
    {
        if (element.TryGetInt32(out int value))
            return value;
        return null;
    }
    
    public long? AsLong()
    {
        if (element.TryGetInt64(out long value))
            return value;
        return null;
    }
    
    public double? AsDouble()
    {
        if (element.TryGetDouble(out double value))
            return value;
        return null;
    }
}