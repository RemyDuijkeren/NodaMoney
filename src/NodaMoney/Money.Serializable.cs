using System.ComponentModel;
using System.Globalization;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace NodaMoney;

/// <summary>Represents Money, an amount defined in a specific Currency.</summary>
[Serializable]
//[TypeConverter(typeof(MoneyTypeConverter))]
[JsonConverter(typeof(MoneyJsonConverter))]
// IXmlSerializable used for XML serialization (ReadXml, WriteXml, GetSchema),
// ISerializable for binary serialization (GetObjectData, ctor(SerializationInfo, StreamingContext))
public partial struct Money : IXmlSerializable, ISerializable
{
#pragma warning disable CA1801 // Parameter context of method.ctor is never used.
    /// <summary>Initializes a new instance of the <see cref="Money"/> struct.Initializes a new instance of <see cref="Money"/> with serialized binary data.</summary>
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

        Currency = (Currency)TypeDescriptor.GetConverter(typeof(Currency)).ConvertFromString(currency);
        Amount = Round(amount, Currency, MidpointRounding.ToEven);
    }
#pragma warning restore CA1801 // Parameter context of method.ctor is never used.

    /// <inheritdoc/>
    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        if (info == null)
            throw new ArgumentNullException(nameof(info));

        info.AddValue("Amount", Amount);
        info.AddValue("Currency", TypeDescriptor.GetConverter(typeof(Currency)).ConvertToString(Currency));
    }

    /// <inheritdoc/>
    public XmlSchema GetSchema() => null;

    /// <inheritdoc/>
    public void ReadXml(XmlReader reader)
    {
        if (reader == null)
            throw new ArgumentNullException(nameof(reader));

        // To decide between V1 and V2 format, we need to check the current if it has an attribute "Amount" or "amount"
        // if it has, we are in V1 format, otherwise we are in V2 format
        if (reader.MoveToAttribute("Amount") || reader.MoveToAttribute("amount"))
        {
            // v1 format: <Money Amount="765.43" Currency="USD" />
            if (!decimal.TryParse(reader.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out var amount))
                throw new InvalidOperationException($"Couldn't parse the Money amount '{reader.Value}' to decimal!");

            string currencyAttribute = reader.GetAttribute("Currency") ??
                                       reader.GetAttribute("currency") ??
                                       throw new InvalidOperationException("Couldn't find attribute 'Currency' or 'currency'!");

            var currency = (Currency)(TypeDescriptor.GetConverter(typeof(Currency)).ConvertFromString(currencyAttribute) ??
                                      throw new InvalidOperationException($"Converting '{currencyAttribute}' to Currency failed!"));

            Unsafe.AsRef(in this) = new Money(amount, currency);
        }
        else
        {
            // v2 format: <Money Currency="USD">765.43</Money>
            string currencyAttribute = reader.GetAttribute("Currency") ??
                                       reader.GetAttribute("currency") ??
                                       throw new InvalidOperationException("Couldn't find attribute 'Currency' or 'currency'!");

            var currency = (Currency)(TypeDescriptor.GetConverter(typeof(Currency)).ConvertFromString(currencyAttribute) ??
                                      throw new InvalidOperationException($"Converting '{currencyAttribute}' to Currency failed!"));

            var content = reader.ReadElementContentAsString();
            if (!decimal.TryParse(content, NumberStyles.Any, CultureInfo.InvariantCulture, out var amount))
                throw new InvalidOperationException($"Couldn't parse the Money amount '{content}' to decimal!");

            Unsafe.AsRef(in this) = new Money(amount, currency);
        }
    }

    /// <inheritdoc/>
    public void WriteXml(XmlWriter writer)
    {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            // v2 format: <Money Currency="USD">765.43</Money>
            writer.WriteAttributeString("Currency", TypeDescriptor.GetConverter(typeof(Currency)).ConvertToString(Currency));
            writer.WriteString(Amount.ToString(CultureInfo.InvariantCulture));

            // v1 format: <Money Amount="765.43" Currency="USD" />
            // writer.WriteAttributeString("Amount", Amount.ToString(CultureInfo.InvariantCulture));
            // writer.WriteAttributeString("Currency", TypeDescriptor.GetConverter(typeof(Currency)).ConvertToString(Currency));
        }
}
