using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using NodaMoney.Serialization;

namespace NodaMoney;

[Serializable]
[TypeConverter(typeof(CurrencyTypeConverter))]  // Used by Newtonsoft.Json to do the serialization.
[JsonConverter(typeof(CurrencyJsonConverter))] // Used by System.Text.Json to do the serialization.
// IXmlSerializable used for XML serialization (ReadXml, WriteXml, GetSchema),
// ISerializable for binary serialization (GetObjectData, ctor(SerializationInfo, StreamingContext))
public readonly partial record struct Currency : IXmlSerializable, ISerializable
{
    /// <inheritdoc cref="IXmlSerializable.GetSchema"/>
    public XmlSchema? GetSchema() => null;

    /// <inheritdoc cref="IXmlSerializable.ReadXml"/>
    public void ReadXml(XmlReader reader)
    {
        if (reader == null)
            throw new ArgumentNullException(nameof(reader));

        var currency = reader.GetAttribute("Currency");
        if (currency is not null)
        {
            // v1 format: <Money Amount="765.43" Currency="USD;CUSTOM" />
            string[] v = currency.Split([';']);
            if (v.Length == 1 || string.IsNullOrWhiteSpace(v[1]) || v[1] == "ISO-4217")
            {
                Unsafe.AsRef(in this) = new Currency(v[0]);
            }
            else // Only if the 2nd part is not empty and not "ISO-4217", it is a custom currency
            {
                Unsafe.AsRef(in this) = new Currency(v[0]) { IsIso4217 = false };
            }
        }
    }

    /// <inheritdoc cref="IXmlSerializable.WriteXml"/>
    public void WriteXml(XmlWriter writer)
    {
        if (writer == null)
            throw new ArgumentNullException(nameof(writer));

        writer.WriteAttributeString("Currency", IsIso4217 ? Code : $"{Code};CUSTOM");
    }

    /// <inheritdoc cref="ISerializable.GetObjectData"/>
    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        if (info == null)
            throw new ArgumentNullException(nameof(info));

        info.AddValue("code", Code);
        info.AddValue("isIso4217", IsIso4217);
    }
}
