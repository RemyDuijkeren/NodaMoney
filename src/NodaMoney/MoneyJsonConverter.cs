using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NodaMoney;

/// <inheritdoc />
public class MoneyJsonConverter : JsonConverter<Money>
{
    //public override bool HandleNull => false;

    /// <inheritdoc />
    public override Money Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            throw new JsonException("Unexpected null value for Money.");
            return default;
            //return null;
        }

        // new serialization format (v2): "EUR 234.25" (or "234.25 EUR")
        // TODO: serialize non-ISO-4217 currencies with same code as ISO-4217 currencies, like "XXX;NON-ISO 234.25" or something else
        if (reader.TokenType == JsonTokenType.String)
        {
            string? value = reader.GetString();
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new JsonException("Invalid format for Money. Expected format is 'Currency Amount', like 'EUR 234.25'.");
                //return (Money)null;
            }
            else
            {
                ReadOnlySpan<char> valueAsSpan = value.AsSpan();
                int spaceIndex = valueAsSpan.IndexOf(' ');
                if (spaceIndex == -1)
                {
                    throw new JsonException("Invalid format for Money. Expected format is 'Currency Amount', like 'EUR 234.25'.");
                }

                ReadOnlySpan<char> currencySpan = valueAsSpan.Slice(0, spaceIndex);
                ReadOnlySpan<char> amountSpan = valueAsSpan.Slice(spaceIndex + 1);

                try
                {
                    Currency currency1 = new Currency(currencySpan.ToString());
                    decimal amount1 = decimal.Parse(amountSpan.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture);

                    return new Money(amount1, currency1);
                }
                catch (Exception ex) when (ex is FormatException or ArgumentException)
                {
                    try
                    {
                        // try reverse 234.25 EUR
                        Currency currency1 = new Currency(amountSpan.ToString());
                        decimal amount1 = decimal.Parse(currencySpan.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture);

                        return new Money(amount1, currency1);
                    }
                    catch (Exception reverseException)  when (reverseException is FormatException or ArgumentException)
                    {
                        // throw with original exception!
                        throw new JsonException("Invalid format for Money. Expected format is 'Currency Amount', like 'EUR 234.25'.", ex);
                    }
                }
            }
        }

        // old serialization format (v1): { "Amount": 234.25, "Currency": "EUR" }
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException();
        }

        decimal amount = 0;
        Currency currency = Currency.NoCurrency;
        bool hasAmount = false, hasCurrency = false;

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                if (!hasAmount) throw new JsonException("Missing property 'Amount'!");
                if (!hasCurrency) throw new JsonException("Missing property 'Currency'!");

                return new Money(amount, currency);
            }

            if (reader.TokenType == JsonTokenType.PropertyName)
            {
                string propertyName = reader.GetString();
                reader.Read();
                switch (propertyName)
                {
                    case "Amount":
                    case "amount":
                        if (reader.TokenType == JsonTokenType.Number)
                        {
                            amount = reader.GetDecimal();
                        }
                        else
                        {
                            if (!decimal.TryParse(reader.GetString(), NumberStyles.Any, CultureInfo.InvariantCulture, out amount))
                            {
                                throw new JsonException("Can't parse property 'Amount' to a number!");
                            }
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
                }
            }
        }

        throw new JsonException();
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, Money money, JsonSerializerOptions options)
    {
        if (money == null)
        {
            writer.WriteNullValue();
            return;
        }

        writer.WriteStringValue($"{money.Currency.Code.ToString(CultureInfo.InvariantCulture)} {money.Amount.ToString(CultureInfo.InvariantCulture)}");
    }
}
