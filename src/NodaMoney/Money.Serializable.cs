using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace NodaMoney
{
    /// <summary>Represents Money, an amount defined in a specific Currency.</summary>
    [Serializable]
    public partial struct Money : IXmlSerializable, ISerializable
    {
        /// <summary>This method is reserved and should not be used. When implementing the IXmlSerializable interface, you should
        /// return null (Nothing in Visual Basic) from this method, and instead, if specifying a custom schema is required, apply
        /// the <see cref="T:System.Xml.Serialization.XmlSchemaProviderAttribute" /> to the class.</summary>
        /// <returns>An <see cref="T:System.Xml.Schema.XmlSchema" /> that describes the XML representation of the object that is
        /// produced by the <see cref="M:System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter)" /> method
        /// and consumed by the <see cref="M:System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader)" /> method.
        /// </returns>
        public XmlSchema GetSchema() => null;

        /// <summary>Generates an object from its XML representation.</summary>
        /// <param name="reader">The <see cref="T:System.Xml.XmlReader" /> stream from which the object is deserialized.</param>
        /// <exception cref="System.ArgumentNullException">The value of 'reader' cannot be null.</exception>
        /// <exception cref="System.Runtime.Serialization.SerializationException">The xml should have a content element with name Money!</exception>
        public void ReadXml(XmlReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            if (reader.MoveToContent() != XmlNodeType.Element)
                throw new SerializationException("Couldn't find content element with name Money!");

            Amount = decimal.Parse(reader["Amount"], CultureInfo.InvariantCulture);
            Currency = Currency.FromCode(reader["Currency"]);
        }

        /// <summary>Converts an object into its XML representation.</summary>
        /// <param name="writer">The <see cref="T:System.Xml.XmlWriter" /> stream to which the object is serialized.</param>
        /// <exception cref="System.ArgumentNullException">The value of 'writer' cannot be null.</exception>
        public void WriteXml(XmlWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            writer.WriteAttributeString("Amount", Amount.ToString(CultureInfo.InvariantCulture));
            writer.WriteAttributeString("Currency", Currency.Code);
        }

        /// <summary>Populates a <see cref="SerializationInfo" /> with the data needed to serialize the target object.</summary>
        /// <param name="info">The <see cref="SerializationInfo" /> to populate with data. </param>
        /// <param name="context">The destination (see <see cref="StreamingContext" />) for this serialization. </param>
        /// <exception cref="System.Security.SecurityException">The caller does not have the required permission. </exception>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("amount", Amount);
            //info.AddValue("currency", Currency, typeof(Currency));
            info.AddValue("currency", Currency.Code);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Money"/> struct.Initializes a new instace of <see cref="Money"/> with serialized data.</summary>
        /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data about the <see cref="Money"/>.</param>
        /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the source or destination.</param>
        private Money(SerializationInfo info, StreamingContext context)
        //: this(info.GetDecimal("amount"), (Currency)info.GetValue("currency", typeof(Currency)))
        : this(info.GetDecimal("amount"), info.GetString("currency"))
        {
            //var amount = info.GetDecimal("amount");
            //var currencyCode = info.GetString("currency");
            //Currency = currency;

            //if (Currency.DecimalDigits == CurrencyRegistry.NotApplicable)
            //{
            //    Amount = Math.Round(amount);
            //}
            //else if (Currency.DecimalDigits == CurrencyRegistry.Z07)
            //{
            //    // divided into five subunits rather than by a power of ten. 5 is 10 to the power of log(5) = 0.69897...
            //    Amount = Math.Round(amount / 0.2m, 0, rounding) * 0.2m;
            //}
            //else
            //{
            //    Amount = Math.Round(amount, (int)Currency.DecimalDigits, rounding);
            //}

            //Money(info.GetDecimal("amount"), info.GetString("currency"))

            //        if (reader == null)
            //            throw new ArgumentNullException(nameof(reader));

            //        if (reader.TokenType == JsonToken.Null)
            //            return null;

            //        JObject jsonObject = JObject.Load(reader);
            //        var properties = jsonObject.Properties().ToList();

            //        var amountProperty = properties.SingleOrDefault(pr => string.Compare(pr.Name, "amount", StringComparison.OrdinalIgnoreCase) == 0);
            //        var currencyProperty = properties.SingleOrDefault(pr => string.Compare(pr.Name, "currency", StringComparison.OrdinalIgnoreCase) == 0);

            //        if (amountProperty == null)
            //            throw new ArgumentNullException("Ammount needs to be defined", "amount");

            //        if (currencyProperty == null)
            //            throw new ArgumentNullException("Currency needs to be defined", "currency");

            //        return new Money((decimal)amountProperty.Value, Currency.FromCode((string)currencyProperty.Value));
        }
    }
}
