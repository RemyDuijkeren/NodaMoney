using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Newtonsoft.Json;
using NodaMoney.Serialization.JsonNet;

namespace NodaMoney
{
    /// <summary>Represents Money, an amount defined in a specific Currency.</summary>
    [JsonConverter(typeof(MoneyJsonConverter))]
    public partial struct Money : IXmlSerializable
    {
        /// <summary>This method is reserved and should not be used. When implementing the IXmlSerializable interface, you should
        /// return null (Nothing in Visual Basic) from this method, and instead, if specifying a custom schema is required, apply
        /// the <see cref="T:System.Xml.Serialization.XmlSchemaProviderAttribute" /> to the class.</summary>
        /// <returns>An <see cref="T:System.Xml.Schema.XmlSchema" /> that describes the XML representation of the object that is
        /// produced by the <see cref="M:System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter)" /> method
        /// and consumed by the <see cref="M:System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader)" /> method.
        /// </returns>
        public XmlSchema GetSchema()
        {
            return null;
        }

        /// <summary>Generates an object from its XML representation.</summary>
        /// <param name="reader">The <see cref="T:System.Xml.XmlReader" /> stream from which the object is deserialized.</param>
        /// <exception cref="System.ArgumentNullException">The value of 'reader' cannot be null.</exception>
        /// <exception cref="System.Runtime.Serialization.SerializationException">The xml should have a content element with name Money!</exception>
        public void ReadXml(XmlReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

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
                throw new ArgumentNullException("writer");

            writer.WriteAttributeString("Amount", Amount.ToString(CultureInfo.InvariantCulture));
            writer.WriteAttributeString("Currency", Currency.Code);
        }
    }
}
