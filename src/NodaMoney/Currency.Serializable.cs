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

        // v1 format: <Money Amount="765.43" Currency="USD;CUSTOM" />
        // v2 format: <Money Currency="EUR">765.43</Money>
        var currency = reader.GetAttribute("Currency");
        if (currency is not null)
        {
            ReadOnlySpan<char> currencySpan = currency.AsSpan();
            int separatorIndex = currencySpan.IndexOf(';');
            if (separatorIndex == -1)
            {
                // v2 fast path: No semicolon, use the string directly
                Unsafe.AsRef(in this) = new Currency(currencySpan);
            }
            else
            {
                // v1 fallback: Slice the span to get the part before the semicolon
                Unsafe.AsRef(in this) = new Currency(currencySpan.Slice(0, separatorIndex));
            }
        }
    }

    /// <inheritdoc cref="IXmlSerializable.WriteXml"/>
    public void WriteXml(XmlWriter writer)
    {
        if (writer == null)
            throw new ArgumentNullException(nameof(writer));

        writer.WriteAttributeString("Currency", Code);
    }

    /// <inheritdoc cref="ISerializable.GetObjectData"/>
    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        if (info == null)
            throw new ArgumentNullException(nameof(info));

        info.AddValue("code", Code);
    }
}
