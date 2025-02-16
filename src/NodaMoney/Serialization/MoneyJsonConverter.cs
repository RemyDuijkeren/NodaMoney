using System.Buffers;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NodaMoney.Serialization;

/// <summary>Converts a Money type to or from JSON.</summary>
/// <remarks>Used by System.Text.Json to do the (de)serialization.</remarks>
#pragma warning disable CA1704
public class MoneyJsonConverter : JsonConverter<Money>
#pragma warning restore CA1704
{
    const string InvalidFormatMessage = "Invalid format for Money. Expected format is '<Currency> <Amount>', like 'EUR 234.25'.";

    /// <inheritdoc />
    public override bool CanConvert(Type typeToConvert) =>
        typeToConvert == typeof(Money) || typeToConvert == typeof(Money?);

    /// <inheritdoc />
    public override Money Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        reader.TokenType switch
        {
            JsonTokenType.Null when typeToConvert == typeof(Money) => throw new JsonException(
                "Null value encountered for 'Money' during JSON deserialization. This value is not allowed. Use Money? instead or make sure the JSON value is not null."),
            JsonTokenType.Null => default, // Will return null for Money?
            JsonTokenType.String => ParseMoneyFromString(ref reader),
            JsonTokenType.StartObject => ParseMoneyFromJsonObject(ref reader),
            _ => throw new JsonException(InvalidFormatMessage)
        };

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, Money value, JsonSerializerOptions options) =>
        writer.WriteStringValue($"{value.Currency.Code.ToString(CultureInfo.InvariantCulture)} {value.Amount.ToString(CultureInfo.InvariantCulture)}");

    /// <summary>Parses a JSON string representation of monetary value into a <see cref="Money"/> object.</summary>
    /// <param name="reader">An instance of <see cref="Utf8JsonReader"/> providing the JSON string to parse.</param>
    /// <returns>A <see cref="Money"/> object representing the parsed currency and amount.</returns>
    /// <exception cref="JsonException">Thrown when the JSON string is null, empty, or in an invalid format not adhering to 'Currency Amount'.</exception>
    /// <remarks>This is the new serialization format from v2 and up, like: "EUR 234.25" (or "234.25 EUR")</remarks>
    static Money ParseMoneyFromString(ref Utf8JsonReader reader)
    {
        // TODO: serialize non-ISO-4217 currencies with same code as ISO-4217 currencies, like "XXX;NON-ISO 234.25" or something else?

        // Get the JSON value as UTF-8 bytes and then decode to a ReadOnlySpan<char>, avoiding intermediate string allocations.
        ReadOnlySpan<byte> valueBytes = reader.HasValueSequence ? reader.ValueSequence.ToArray() : reader.ValueSpan;
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        ReadOnlySpan<char> valueChars = System.Text.Encoding.UTF8.GetString(valueBytes).AsSpan();
#else
        ReadOnlySpan<char> valueChars = System.Text.Encoding.UTF8.GetString(valueBytes.ToArray()).AsSpan();
#endif
        if (valueChars.IsWhiteSpace())
            throw new JsonException(InvalidFormatMessage);

        // Expecting '<Currency> <Amount>', like 'EUR 234.25', so search for space.
        int spaceIndex = valueChars.IndexOf(' ');
        if (spaceIndex == -1)
            throw new JsonException(InvalidFormatMessage);

        // Split the string into currency and amount, like 'EUR' and '234.25'
        ReadOnlySpan<char> currencySpan = valueChars.Slice(0, spaceIndex);
        ReadOnlySpan<char> amountSpan = valueChars.Slice(spaceIndex + 1);

        try
        {
            Currency currency = new(currencySpan);
            decimal amount = decimal.Parse(amountSpan.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture);

            return new Money(amount, currency);
        }
        catch (Exception ex) when (ex is FormatException or ArgumentException)
        {
            // Retry using reverse format, like '234.25 EUR'
            try
            {
                Currency currency = new(amountSpan);
                decimal amount = decimal.Parse(currencySpan.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture);

                return new Money(amount, currency);
            }
            catch (Exception reverseException) when (reverseException is FormatException or ArgumentException)
            {
                // Throw with original exception because using reverse format also failed!
                throw new JsonException(InvalidFormatMessage, ex);
            }
        }
    }

    /// <summary>Parses a JSON object and converts it into a <see cref="Money"/> instance.</summary>
    /// <param name="reader">The <see cref="Utf8JsonReader"/> used to parse the JSON object.</param>
    /// <returns>A <see cref="Money"/> instance created from the JSON object.</returns>
    /// <exception cref="JsonException">Thrown if the JSON is invalid, or if required properties such as 'Amount' or 'Currency' are missing.</exception>
    /// <remarks>This is the old serialization format used in v1, like: { "Amount": 234.25, "Currency": "EUR" }.</remarks>
#pragma warning disable CA1704
    static Money ParseMoneyFromJsonObject(ref Utf8JsonReader reader)
#pragma warning restore CA1704
    {
        decimal amount = 0;
        Currency currency = Currency.NoCurrency;
        bool hasAmount = false, hasCurrency = false;

        while (reader.Read())
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.EndObject when !hasAmount:
                    throw new JsonException("Missing property 'Amount'!");
                case JsonTokenType.EndObject when !hasCurrency:
                    throw new JsonException("Missing property 'Currency'!");
                case JsonTokenType.EndObject:
                    return new Money(amount, currency);
                case JsonTokenType.PropertyName:
                    string? propertyName = reader.GetString();
                    reader.Read();
                    switch (propertyName)
                    {
                        case "Amount":
                        case "amount":
                            if (reader.TokenType == JsonTokenType.Number)
                            {
                                amount = reader.GetDecimal();
                            }
                            else if (!decimal.TryParse(reader.GetString(), NumberStyles.Any, CultureInfo.InvariantCulture, out amount))
                            {
                                throw new JsonException("Can't parse property 'Amount' to a number!");
                            }

                            hasAmount = true;
                            break;
                        case "Currency":
                        case "currency":
                            var valueAsString = reader.GetString();
                            if (valueAsString == null) break;

                            string[] v = valueAsString.Split([';']);
                            if (v.Length == 1 || string.IsNullOrWhiteSpace(v[1]) || v[1] == "ISO-4217")
                            {
                                currency = new Currency(v[0]);
                            }
                            else // ony 2nd part is not empty and not "ISO-4217" is a custom currency
                            {
                                currency = new Currency(v[0]) { IsIso4217 = false };
                            }

                            hasCurrency = true;
                            break;
                        default:
                            throw new JsonException($"Invalid property '{propertyName}' in JSON object!");
                    }

                    break;

                default:
                    throw new JsonException("Invalid JSON format for 'Money'.");
            }
        }

        throw new JsonException("Invalid JSON format for 'Money'. Expected a JSON object (e.g., { \"Amount\": 234.25, \"Currency\": \"EUR\" }).");
    }
}
