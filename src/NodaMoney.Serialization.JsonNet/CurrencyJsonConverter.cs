using System;
using System.ComponentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace NodaMoney.Serialization.JsonNet
{

    public class CurrencyJsonConverter : JsonConverter<Currency>
    {
        private static TypeConverter currencyConverter = TypeDescriptor.GetConverter(typeof(Currency));
        public override Currency ReadJson(JsonReader reader, Type objectType, Currency existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var code = reader.Value.ToString();
            return (Currency)currencyConverter.ConvertFromString(code);
        }

        public override void WriteJson(JsonWriter writer, Currency value, JsonSerializer serializer)
        {
            writer.WriteValue(currencyConverter.ConvertToString(value));
        }
    }
}
