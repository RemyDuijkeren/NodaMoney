﻿using System.ComponentModel;
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
        destinationType == typeof(Money) || destinationType == typeof(Money?) || base.CanConvertTo(context, destinationType);

    /// <inheritdoc/>
    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object? value)
    {
        // Newtonsoft.Json will only call this method when it is a JSON String, like "EUR 234.25", but if it is
        // a JSON Object (= "{...}") it will not call this method but tries to convert it internal in Newtonsoft.Json (which fails).
        if (value is null || value is not string jsonString)
            return base.ConvertFrom(context, culture, value!);

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
            CurrencyInfo currencyInfo = CurrencyInfo.FromCode(currencySpan.ToString());
            decimal amount = decimal.Parse(amountSpan.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture);

            return new Money(amount, currencyInfo);
        }
        catch (Exception ex) when (ex is FormatException or ArgumentException or InvalidCurrencyException)
        {
            try
            {
                // try reverse: 234.25 EUR
                CurrencyInfo currencyInfo = CurrencyInfo.FromCode(amountSpan.ToString());
                decimal amount = decimal.Parse(currencySpan.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture);

                return new Money(amount, currencyInfo);
            }
            catch (Exception reverseException)  when (reverseException is FormatException or ArgumentException or InvalidCurrencyException)
            {
                // throw with original exception!
                throw new SerializationException("Invalid format for Money. Expected format is 'Currency Amount', like 'EUR 234.25'.", ex);
            }
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

    // IsValid(ITypeDescriptorContext, Object)
}
