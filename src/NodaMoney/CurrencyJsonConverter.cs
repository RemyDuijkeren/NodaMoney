using System.Text.Json;
using System.Text.Json.Serialization;

namespace NodaMoney;

/// <summary>Converts a Currency type to or from JSON.</summary>
/// <remarks>Used by System.Text.Json to do the (de)serialization.</remarks>
#pragma warning disable CA1704
public class CurrencyJsonConverter : JsonConverter<Currency>
#pragma warning restore CA1704
{
    /// <inheritdoc />
    public override Currency Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            var code = reader.GetString();
            if (string.IsNullOrWhiteSpace(code))
                throw new JsonException("Invalid currency code.");

            return Currency.FromCode(code);
        }

        throw new JsonException("Invalid currency code.");
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, Currency value, JsonSerializerOptions options) =>
        writer.WriteStringValue(value.Code);
}
