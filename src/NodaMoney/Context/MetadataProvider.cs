namespace NodaMoney.Context;

/// <summary>Provides the ability to associate and lookup metadata for various monetary entities like currencies or amounts.</summary>
/// <remarks>Attach metadata to rounding operations. This could include:
/// - Details like maximum/minimum for the allowed scale.
/// - Reasons for rounding (e.g., legal rules, financial calculations).
/// - Region- or jurisdiction-specific adjustments.
/// </remarks>
public class MetadataProvider
{
    private readonly Dictionary<string, object> _metadata = new();

    /// <summary>Adds metadata with the specified key and value.</summary>
    /// <param name="key">The key to identify the metadata.</param>
    /// <param name="value">The value associated with the specified key.</param>
    public void AddMetadata(string key, object value)
    {
        _metadata[key] = value;
    }

    /// <summary>Retrieves metadata associated with the specified key and casts it to the specified type.</summary>
    /// <param name="key">The key to identify the metadata.</param>
    /// <typeparam name="T">The expected type of the metadata value.</typeparam>
    /// <returns>The metadata value cast to the specified type, or the default value of the type if the key does not exist or cannot be cast.</returns>
    public T? GetMetadata<T>(string key)
    {
        if (_metadata.TryGetValue(key, out var value) && value is T typedValue)
            return typedValue;

        return default;
    }
}
