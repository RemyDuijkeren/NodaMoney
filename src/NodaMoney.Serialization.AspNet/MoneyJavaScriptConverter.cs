using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Web.Script.Serialization;

namespace NodaMoney.Serialization.AspNet
{
    /// <summary>Provides a custom Money converter for the JavaScriptSerializer in ASP.NET.</summary>
    /// <code>
    /// // create a new serializer and tell it about the NodaMoney converter.
    /// serializer = new JavaScriptSerializer();
    /// serializer.RegisterConverters(new JavaScriptConverter[] { new MoneyJavaScriptConverter() });
    /// </code>
    public class MoneyJavaScriptConverter : JavaScriptConverter
    {
        /// <summary>When overridden in a derived class, gets a collection of the supported types.</summary>
        public override IEnumerable<Type> SupportedTypes => new ReadOnlyCollection<Type>(new List<Type>(new[] { typeof(Money) }));

        /// <summary>When overridden in a derived class, converts the provided dictionary into an object of the specified type.</summary>
        /// <param name="dictionary">An <see cref="T:System.Collections.Generic.IDictionary`2" /> instance of property data stored as name/value pairs.</param>
        /// <param name="type">The type of the resulting object.</param>
        /// <param name="serializer">The <see cref="T:System.Web.Script.Serialization.JavaScriptSerializer" /> instance.</param>
        /// <returns>The deserialized object.</returns>
        /// <exception cref="System.ArgumentNullException">dictionary should not be null</exception>
        /// <exception cref="System.ArgumentException">object should be of type Money to deserialize!;type</exception>
        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            if (dictionary == null)
                throw new ArgumentNullException(nameof(dictionary));
            if (type != typeof(Money))
                throw new ArgumentException("object should be of type Money to deserialize!", nameof(type));

            if (!dictionary.ContainsKey("amount"))
                throw new ArgumentNullException("Ammount needs to be defined", "amount");

            if (!dictionary.ContainsKey("currency"))
                throw new ArgumentNullException("Currency needs to be defined", "currency");

            var amount = decimal.Parse(Convert.ToString(dictionary["amount"], CultureInfo.InvariantCulture), CultureInfo.InvariantCulture);

            var code = (string)dictionary["currency"];

            return new Money(amount, Currency.FromCode(code));
        }

        /// <summary>When overridden in a derived class, builds a dictionary of name/value pairs.</summary>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="serializer">The object that is responsible for the serialization.</param>
        /// <returns>An object that contains key/value pairs that represent the object’s data.</returns>
        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            Money money = (Money)obj;

            var dictionary = new Dictionary<string, object>
                                 {
                                     { "amount", money.Amount.ToString(CultureInfo.InvariantCulture) },
                                     { "currency", money.Currency.Code }
                                 };

            return dictionary;
        }
    }
}