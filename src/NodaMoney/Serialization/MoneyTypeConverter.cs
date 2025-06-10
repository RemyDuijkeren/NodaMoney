using System.ComponentModel;
using System.Globalization;
using System.Runtime.Serialization;
using NodaMoney.Context;

namespace NodaMoney.Serialization;

/// <summary>Provides a way of converting the type <see cref="string"/> to and from the type <see cref="Money"/>.</summary>
/// <remarks>Used by Newtonsoft.Json for JSON Strings to do the serialization.</remarks>
public class MoneyTypeConverter : TypeConverter
{
    const string InvalidFormatMessage = "Invalid format for Money. Expected format is '<Currency> <Amount>', like 'EUR 234.25'.";

    /// <inheritdoc/>
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType) =>
        sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);

    /// <inheritdoc/>
    public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType) =>
        destinationType == typeof(Money) || destinationType == typeof(Money?) || base.CanConvertTo(context, destinationType);

    /// <inheritdoc/>
    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object? value)
    {
        // Newtonsoft.Json will only call this method when it is a JSON String, like "EUR 234.25", but if it is
        // a JSON Object (= "{...}") it will not call this method but tries to convert it internally in Newtonsoft.Json (which fails).
        if (value is not string jsonString)
            return base.ConvertFrom(context, culture, value!);

        ReadOnlySpan<char> valueAsSpan = jsonString.AsSpan();
        int spaceIndex = valueAsSpan.IndexOf(' ');
        if (spaceIndex == -1)
        {
            throw new FormatException(InvalidFormatMessage);
        }

        ReadOnlySpan<char> currencySpan = valueAsSpan.Slice(0, spaceIndex);
        ReadOnlySpan<char> amountSpan = valueAsSpan.Slice(spaceIndex + 1);

        try
        {
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            if (decimal.TryParse(amountSpan, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal amount))
#else
            if (decimal.TryParse(amountSpan.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal amount))
#endif
            {
                CurrencyInfo currencyInfo = CurrencyInfo.FromCode(currencySpan.ToString());
                return new Money(amount, currencyInfo, MoneyContext.NoRounding);
            }

            // try reverse: 234.25 EUR
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            if (decimal.TryParse(currencySpan, NumberStyles.Any, CultureInfo.InvariantCulture, out amount))
#else
            if (decimal.TryParse(currencySpan.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out amount))
#endif
            {
                CurrencyInfo currencyInfo = CurrencyInfo.FromCode(amountSpan.ToString());
                return new Money(amount, currencyInfo, MoneyContext.NoRounding);
            }

            throw new SerializationException(InvalidFormatMessage);
        }
        catch (Exception ex) when (ex is FormatException or ArgumentException or InvalidCurrencyException)
        {
            throw new SerializationException(InvalidFormatMessage, ex);
        }
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
}
