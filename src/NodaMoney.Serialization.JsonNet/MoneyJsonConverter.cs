using System;
using System.ComponentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace NodaMoney.Serialization.JsonNet
{
    public class MoneyJsonConverter : JsonConverter<Money>
    {
        private static TypeConverter currencyConverter = TypeDescriptor.GetConverter(typeof(Currency));
        public override Money ReadJson(JsonReader reader, Type objectType, Money existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            decimal? amount = null;
            string currencyCode = null;
            var amountPropertyName = ResolvePropertyName(serializer, nameof(Money.Amount));
            var currencyPropertyName = ResolvePropertyName(serializer, nameof(Money.Currency));

            while (reader.Read())
            {
                if (reader.TokenType != JsonToken.PropertyName)
                {
                    break;
                }

                var propertyName = (string)reader.Value;

                if (!reader.Read())
                {
                    break;
                }

                if (string.Equals(propertyName, amountPropertyName, StringComparison.OrdinalIgnoreCase))
                {
                    amount = serializer.Deserialize<decimal?>(reader);
                }

                if (string.Equals(propertyName, currencyPropertyName, StringComparison.OrdinalIgnoreCase))
                {
                    currencyCode = serializer.Deserialize<string>(reader);
                }
            }

            if (amount == null)
            {
                throw new JsonSerializationException($"Unable to deserialize to {nameof(Money)}, since the property {amountPropertyName} was null or missing");
            }

            if (string.IsNullOrEmpty(currencyCode))
            {
                throw new JsonSerializationException($"Unable to deserialize to {nameof(Money)}, since the property {currencyPropertyName} was null or missing");
            }

            return new Money(amount.Value, (Currency)currencyConverter.ConvertFromString(currencyCode));
        }

        public override void WriteJson(JsonWriter writer, Money value, JsonSerializer serializer)
        {
            writer.WriteStartObject();

            writer.WritePropertyName(ResolvePropertyName(serializer, nameof(Money.Amount)));
            serializer.Serialize(writer, value.Amount);

            writer.WritePropertyName(ResolvePropertyName(serializer, nameof(Money.Currency)));
            writer.WriteValue(value.Currency.Code);

            writer.WriteEndObject();
        }

        static string ResolvePropertyName(JsonSerializer serializer, string propertyName) =>
                    (serializer.ContractResolver as DefaultContractResolver)?.GetResolvedPropertyName(propertyName) ?? propertyName;
    }
}
