/// <summary>
/// Provides the ability to associate and lookup metadata for various monetary entities like currencies or amounts.
/// </summary>
public class MetadataProvider
{
    private readonly Dictionary<string, object> _metadata = new();

    /// <summary>Adds metadata (key-value pair).</summary>
    public void AddMetadata(string key, object value)
    {
        _metadata[key] = value;
    }

    /// <summary>Gets metadata based on the provided key.</summary>
    public T? GetMetadata<T>(string key)
    {
        if (_metadata.TryGetValue(key, out var value) && value is T typedValue)
            return typedValue;

        return default;
    }
}