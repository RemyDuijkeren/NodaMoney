using System.ComponentModel;
using System.Globalization;
using System.Text;

namespace NodaMoney;

/// <summary>Provides a way of converting the type <see cref="string"/> to and from the type <see cref="Money"/>.</summary>
/// <remarks>Used by <see cref="Newtonsoft.Json."/> to do the serialization.</remarks>
public class MoneyTypeConverter : TypeConverter
{
    /// <inheritdoc/>
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType) =>
        sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);

    /// <inheritdoc/>
    public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType) =>
        destinationType == typeof(Money) || base.CanConvertTo(context, destinationType);

    /// <inheritdoc/>
    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object? value)
    {
        if (value is string valueAsString)
        {
            // Use Span<T> to slice the string without creating a new array
            var valueAsSpan = valueAsString.AsSpan();
            var separatorIndex = valueAsSpan.IndexOf(' ');
            if (separatorIndex == -1)
            {
                throw new FormatException("Invalid format. Expected format is 'amount currency' but didn't find a space.");
            }

            var amountPart = valueAsSpan.Slice(0, separatorIndex);
            var currencyPart = valueAsSpan.Slice(separatorIndex + 1);

            var amount = decimal.Parse(amountPart.ToString(), culture);
            var currency = new Currency(currencyPart.ToString());

            return new Money(amount, currency);

            // string[] v = valueAsString.Split([' ']);
            // return new Money(decimal.Parse(v[0], culture), v[1]);
        }

        return base.ConvertFrom(context, culture, value);
    }

    /// <inheritdoc/>
    public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
    {
        if (destinationType == typeof(string) && value is Money money)
        {
            var result = new StringBuilder();
            result.Append(money.Amount.ToString(culture));
            result.Append(' ');
            result.Append(money.Currency.Code.ToString(culture));

            return result.ToString();
        }

        return base.ConvertTo(context, culture, value, destinationType);
    }

    // IsValid(ITypeDescriptorContext, Object)
}
