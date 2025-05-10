using System.Text.Json;
using System.Text.Json.Serialization;

namespace NodaMoney.Serialization;

/// <summary>Converts a Currency type to or from JSON.</summary>
/// <remarks>Used by System.Text.Json to do the (de)serialization.</remarks>
#pragma warning disable CA1704
public class CurrencyJsonConverter : JsonConverter<Currency>
#pragma warning restore CA1704
{
    /// <inheritdoc />
    public override Currency Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
            throw new JsonException("Currency code is not a string! Expected a string like 'EUR' or 'USD'.");

        var code = reader.GetString();
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new JsonException("Currency code is null or empty! Expected a string like 'EUR' or 'USD'.");
        }

        try
        {
            return CurrencyInfo.FromCode(code);
        }
        catch (Exception ex) when (ex is ArgumentException or KeyNotFoundException)
        {
            throw new JsonException($"Currency code '{code}' is not a known currency!", ex);
        }
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, Currency value, JsonSerializerOptions options) =>
        writer.WriteStringValue(value.Code);
}
