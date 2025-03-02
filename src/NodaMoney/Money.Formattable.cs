using System.Globalization;

namespace NodaMoney;

/// <summary>Represents Money, an amount defined in a specific Currency.</summary>
public partial struct Money :
#if NET6_0_OR_GREATER
    ISpanFormattable // also implements IFormattable
#else
    IFormattable
#endif
{
    /// <summary>Converts this <see cref="Money"/> instance to its equivalent <see cref="string"/> representation.</summary>
    /// <returns>A string that represents this <see cref="Money"/> instance.</returns>
    /// <remarks>
    /// Converting will use the <see cref="NumberFormatInfo"/> object for the current culture if this has the same
    /// ISOCurrencySymbol, otherwise the <see cref="NumberFormatInfo"/> from the <see cref="Currency"/> will be used.
    /// </remarks>
    public override string ToString() => $"{nameof(Amount)}: {Amount}, {nameof(Currency)}: {Currency}";

    /// <summary>Converts the <see cref="Money"/> value of this instance to its equivalent <see cref="string"/> representation
    /// using the specified format.</summary>
    /// <param name="format">A numeric format string.</param>
    /// <returns>The string representation of this <see cref="Money"/> instance as specified by the format.</returns>
    public string ToString(string format) => Format(format, null);

    /// <summary>Converts this <see cref="Money"/> instance to its equivalent <see cref="string"/> representation using the
    /// specified culture-specific format information.</summary>
    /// <param name="formatProvider">An <see cref="IFormatProvider"/> that supplies culture-specific formatting information.</param>
    /// <returns>The string representation of this <see cref="Money"/> instance as specified by formatProvider.</returns>
    public string ToString(IFormatProvider formatProvider) => Format(null, formatProvider);

    /// <summary>Converts the <see cref="Money"/> value of this instance to its equivalent <see cref="string"/> representation
    /// using the specified format and culture-specific format information.</summary>
    /// <param name="format">A numeric format string.</param>
    /// <param name="formatProvider">An <see cref="IFormatProvider"/> that supplies culture-specific formatting information.</param>
    /// <returns>The string representation of this <see cref="Money"/> instance as specified by the format and formatProvider.</returns>
    public string ToString(string? format, IFormatProvider? formatProvider) => Format(format, formatProvider);

    private string Format(string? format, IFormatProvider? formatProvider)
    {
        CurrencyInfo currencyInfo = CurrencyInfo.GetInstance(Currency);
        return currencyInfo.Format(format, this, formatProvider);
    }

    /// <inheritdoc />
    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
    {
        // TODO: optimize to use Span

        // Produce the same string as ToString(format.ToString(), provider)
        string formatted = Format(format.ToString(), provider);

        // Attempt to copy the formatted result into the destination buffer
        if (formatted.Length > destination.Length)
        {
            charsWritten = 0; // Insufficient space
            return false;
        }

        formatted.AsSpan().CopyTo(destination);
        charsWritten = formatted.Length;
        return true;

        //return destination.TryWrite(provider, $"{nameof(Amount)}: {Amount}, {nameof(Currency)}: {Currency}", out charsWritten);
    }
}
