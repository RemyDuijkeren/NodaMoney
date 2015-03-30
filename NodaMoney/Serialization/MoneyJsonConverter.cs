using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NodaMoney.Serialization
{
    /// <summary>Converts an array to and from JSON, even in the case of a single value.</summary>
    /// <typeparam name="T">The type of the item in the array.</typeparam>
    /// <remarks>For more info, see:<see cref="http://michaelcummings.net/mathoms/using-a-custom-jsonconverter-to-fix-bad-json-results#.UwzXKfldXD4"/></remarks>
    internal class MoneyJsonConverter : JsonConverter
    {
        /// <summary>Writes the JSON representation of the object.</summary>
        /// <param name="writer">The <see cref="T:Newtonsoft.Json.JsonWriter"/> to write to.</param>
        /// <param name="value">The value.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <exception cref="System.NotImplementedException">Method is not implemented</exception>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            Money money = (Money)value;

            writer.WriteStartObject();
            writer.WritePropertyName("Amount");
            serializer.Serialize(writer, money.Amount);
            writer.WritePropertyName("Code");
            serializer.Serialize(writer, money.Currency.Code);
            writer.WriteEndObject();
        }

        /// <summary>Reads the JSON representation of the object.</summary>
        /// <param name="reader">The <see cref="T:Newtonsoft.Json.JsonReader"/> to read from.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="existingValue">The existing value of object being read.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <returns>The object value.</returns>
        /// <exception cref="Newtonsoft.Json.JsonException"><c>JsonToken</c> isn't an StartObject or StartArray</exception>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            //switch (reader.TokenType)
            //{
            //    case JsonToken.PropertyName:
            //        string code = (string)serializer.Deserialize(reader, typeof(string));
            //        return Currency.FromCode(code);

            //    default:
            //        throw new JsonException(string.Format("JsonToken isn't an StartObject or StartArray. Don't know how to handle JsonToken {0}!", reader.TokenType));
            //}

            JObject jsonObject = JObject.Load(reader);
            var properties = jsonObject.Properties().ToList();
            return new Money((decimal)properties[0].Value, Currency.FromCode((string)properties[1].Value));
        }

        /// <summary>Determines whether this instance can convert the specified object type.</summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns><c>true</c> if this instance can convert the specified object type; otherwise,<c>false</c>.</returns>
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(Money));
        }

        /// <summary>Gets a value indicating whether this <see cref="T:Newtonsoft.Json.JsonConverter" /> can read JSON.</summary>
        /// <value><c>true</c> if this <see cref="T:Newtonsoft.Json.JsonConverter" /> can read JSON; otherwise, <c>false</c>. </value>
        public override bool CanRead
        {
            get { return true; }
        }
    }
}
