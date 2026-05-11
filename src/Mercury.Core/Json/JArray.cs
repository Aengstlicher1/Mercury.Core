using System.Collections;
using System.Diagnostics;
using System.Text.Json;

namespace Mercury.Core.Json;

internal sealed class JArray(JsonElement backingElement = default) : IEnumerable<JObject>
{
    private readonly JsonElement _backingElement = backingElement;
    
    public int Length => _backingElement.ValueKind == JsonValueKind.Array ? _backingElement.GetArrayLength() : -1;
    
    public JObject this[int index]
    {
        get
        {
            if (index >= 0 && index < Length)
                return new JObject(_backingElement[index]);
            return new JObject(default);
        }
    }
    

    public IEnumerator<JObject> GetEnumerator()
    {
        for (int i = 0; i < Length; i++)
        {
            yield return new JObject(_backingElement[i]);
        }
    }
    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();
    
    
    public static JArray Empty { get; } = new();
}