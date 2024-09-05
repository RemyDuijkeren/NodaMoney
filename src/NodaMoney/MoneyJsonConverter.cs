using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NodaMoney;

/// <inheritdoc />
public class MoneyJsonConverter : JsonConverter<Money>
{
    /// <inheritdoc />
    public override Money Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            //return (Money)null;
        }

        // new serialization format (v2): "EUR 234.25"
        if (reader.TokenType == JsonTokenType.String)
        {
            string value = reader.GetString();
            if (string.IsNullOrWhiteSpace(value))
            {
                //return (Money)null;
            }
            else
            {
                ReadOnlySpan<char> valueAsSpan = value.AsSpan();
                int spaceIndex = valueAsSpan.IndexOf(' ');
                if (spaceIndex == -1)
                {
                    throw new JsonException("Invalid format for Money. Expected format is 'Currency Amount'.");
                }

                ReadOnlySpan<char> currencySpan = valueAsSpan.Slice(0, spaceIndex);
                ReadOnlySpan<char> amountSpan = valueAsSpan.Slice(spaceIndex + 1);

                Currency currency1 = new Currency(currencySpan.ToString());
                decimal amount1 = decimal.Parse(amountSpan.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture);

                return new Money(amount1, currency1);
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
        writer.WriteStringValue($"{money.Currency.Code.ToString(CultureInfo.InvariantCulture)} {money.Amount.ToString(CultureInfo.InvariantCulture)}");
        // if (money == null)
        // {
        //     writer.WriteNullValue();
        // }
        // else
        // {
        //     writer.WriteStringValue($"{money.Currency.Code.ToString(CultureInfo.InvariantCulture)} {money.Amount.ToString(CultureInfo.InvariantCulture)}");
        // }
    }
}
