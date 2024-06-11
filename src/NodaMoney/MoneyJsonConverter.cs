using System.Text.Json;
using System.Text.Json.Serialization;

namespace NodaMoney;

public class MoneyJsonConverter : JsonConverter<Money>
{
    public override Money Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException();
        }

        decimal amount = 0;
        string currencyCode = null;

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                return new Money(amount, Currency.FromCode(currencyCode));
            }

            if (reader.TokenType == JsonTokenType.PropertyName)
            {
                string propertyName = reader.GetString();
                reader.Read();
                switch (propertyName)
                {
                    case "Amount":
                        amount = reader.GetDecimal();
                        break;
                    case "Currency":
                        currencyCode = reader.GetString();
                        break;
                }
            }
        }

        throw new JsonException();
    }

    public override void Write(Utf8JsonWriter writer, Money value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteNumber("Amount", value.Amount);
        writer.WriteString("Currency", value.Currency.Code);
        writer.WriteEndObject();
    }
}
