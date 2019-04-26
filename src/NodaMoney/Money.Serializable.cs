using System;
using System.ComponentModel;
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
        /// <summary>Initializes a new instance of the <see cref="Money"/> struct.Initializes a new instace of <see cref="Money"/> with serialized data.</summary>
        /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data about the <see cref="Money"/>.</param>
        /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the source or destination.</param>
        private Money(SerializationInfo info, StreamingContext context)
            : this()
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));

            decimal amount;
            try
            {
                amount = info.GetDecimal("Amount");
            }
            catch (FormatException ex)
            {
                throw new SerializationException("Member 'Amount' was not in a correct number format.", ex);
            }
            catch (SerializationException)
            {
                try
                {
                    amount = info.GetDecimal("amount");
                }
                catch (FormatException ex)
                {
                    throw new SerializationException("Member 'Amount' was not in a correct number format.", ex);
                }
            }

            string currency;
            try
            {
                currency = info.GetString("Currency");
            }
            catch (SerializationException)
            {
                currency = info.GetString("currency");
            }

            if ((currency == ";" || String.IsNullOrEmpty(currency)) && amount == 0)
            {
                Currency = default;
                Amount = 0;
            }
            else
            {
                Currency = (Currency)TypeDescriptor.GetConverter(typeof(Currency)).ConvertFromString(currency);
                Amount = Round(amount, Currency, MidpointRounding.ToEven);
            }
        }

        /// <summary>This method is reserved and should not be used. When implementing the IXmlSerializable interface, you should
        /// return null (Nothing in Visual Basic) from this method, and instead, if specifying a custom schema is required, apply
        /// the <see cref="XmlSchemaProviderAttribute" /> to the class.</summary>
        /// <returns>An <see cref="XmlSchema" /> that describes the XML representation of the object that is
        /// produced by the <see cref="IXmlSerializable.WriteXml(XmlWriter)" /> method
        /// and consumed by the <see cref="IXmlSerializable.ReadXml(XmlReader)" /> method.
        /// </returns>
        public XmlSchema GetSchema() => null;

        /// <summary>Generates an object from its XML representation.</summary>
        /// <param name="reader">The <see cref="XmlReader" /> stream from which the object is deserialized.</param>
        /// <exception cref="ArgumentNullException">The value of 'reader' cannot be null.</exception>
        /// <exception cref="SerializationException">The xml should have a content element with name Money!</exception>
        public void ReadXml(XmlReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            if (reader.MoveToContent() != XmlNodeType.Element)
                throw new SerializationException("Couldn't find content element with name Money!");

            Amount = decimal.Parse(reader["Amount"], CultureInfo.InvariantCulture);
            Currency = (Currency)TypeDescriptor.GetConverter(typeof(Currency)).ConvertFromString(reader["Currency"]);
        }

        /// <summary>Converts an object into its XML representation.</summary>
        /// <param name="writer">The <see cref="XmlWriter" /> stream to which the object is serialized.</param>
        /// <exception cref="System.ArgumentNullException">The value of 'writer' cannot be null.</exception>
        public void WriteXml(XmlWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            writer.WriteAttributeString("Amount", Amount.ToString(CultureInfo.InvariantCulture));
            writer.WriteAttributeString("Currency", TypeDescriptor.GetConverter(typeof(Currency)).ConvertToString(Currency));
        }

        /// <summary>Populates a <see cref="SerializationInfo" /> with the data needed to serialize the target object.</summary>
        /// <param name="info">The <see cref="SerializationInfo" /> to populate with data. </param>
        /// <param name="context">The destination (see <see cref="StreamingContext" />) for this serialization. </param>
        /// <exception cref="System.Security.SecurityException">The caller does not have the required permission. </exception>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Amount", Amount);
            info.AddValue("Currency", TypeDescriptor.GetConverter(typeof(Currency)).ConvertToString(Currency));
        }
    }
}
