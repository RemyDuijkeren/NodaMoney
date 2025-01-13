using System.ComponentModel;
using System.Globalization;
using System.Runtime.Serialization;

namespace NodaMoney.Serialization;

/// <summary>Provides a way of converting the type <see cref="string"/> to and from the type <see cref="Money"/>.</summary>
/// <remarks>Used by <see cref="Newtonsoft.Json"/> for JSON Strings to do the serialization.</remarks>
public class MoneyTypeConverter : TypeConverter
{
    /// <inheritdoc/>
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType) =>
        sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);

    /// <inheritdoc/>
    public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType) =>
        destinationType == typeof(Money) || base.CanConvertTo(context, destinationType);

    /// <inheritdoc/>
    public override object ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object? value)
    {
        // Newtonsoft.Json will call this method when it is a JSON String, like "EUR 234.25",
        // but if it is a JSON Object it tries to check if it can convert JObject (in Newtonsoft.Json).
        if (value is not string jsonString)
            throw new SerializationException("Invalid format for Money. Expected format is 'Currency Amount', like 'EUR 234.25'.");

        var valueAsSpan = jsonString.AsSpan();
        var spaceIndex = valueAsSpan.IndexOf(' ');
        if (spaceIndex == -1)
        {
            throw new FormatException("Invalid format for Money. Expected format is 'Currency Amount', like 'EUR 234.25', but didn't find a space.");
        }

        ReadOnlySpan<char> currencySpan = valueAsSpan.Slice(0, spaceIndex);
        ReadOnlySpan<char> amountSpan = valueAsSpan.Slice(spaceIndex + 1);

        try
        {
            Currency currency1 = new Currency(currencySpan.ToString());
            decimal amount1 = decimal.Parse(amountSpan.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture);

            return new Money(amount1, currency1);
        }
        catch (Exception ex) when (ex is FormatException or ArgumentException)
        {
            try
            {
                // try reverse: 234.25 EUR
                Currency currency1 = new Currency(amountSpan.ToString());
                decimal amount1 = decimal.Parse(currencySpan.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture);

                return new Money(amount1, currency1);
            }
            catch (Exception reverseException)  when (reverseException is FormatException or ArgumentException)
            {
                // throw with original exception!
                throw new SerializationException("Invalid format for Money. Expected format is 'Currency Amount', like 'EUR 234.25'.", ex);
            }
        }

        // old serialization format (v1): { "Amount": 234.25, "Currency": "EUR" }
        // use the build in converter for this.

        return base.ConvertFrom(context, culture, value);
    }

    /// <inheritdoc/>
    public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
    {
        if (destinationType == typeof(string) && value is Money money)
        {
            return $"{money.Currency.Code.ToString(CultureInfo.InvariantCulture)} {money.Amount.ToString(CultureInfo.InvariantCulture)}";
        }

        return base.ConvertTo(context, culture, value, destinationType);
    }

    // IsValid(ITypeDescriptorContext, Object)
}
