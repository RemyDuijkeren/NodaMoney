using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace NodaMoney;

/// <summary>Represents Money, an amount defined in a specific Currency.</summary>
public partial struct Money
#if NET7_0_OR_GREATER
     : IParsable<Money> //, ISpanParsable<Money>, IUtf8SpanParsable<Money>
#endif
{
    /// <summary>Converts the string representation of a money value to its <see cref="Money"/> equivalent.</summary>
    /// <param name="s">The string representation of the number to convert.</param>
    /// <returns>The equivalent to the money amount contained in <i>value</i>.</returns>
    /// <exception cref="System.ArgumentNullException"><i>value</i> is <b>null</b> or empty.</exception>
    /// <exception cref="System.FormatException"><i>value</i> is not in the correct format or the currency sign matches with multiple known currencies.</exception>
    /// <exception cref="System.OverflowException"><i>value</i> represents a number less than <see cref="decimal.MinValue"/> or greater than <see cref="decimal.MaxValue"/>.</exception>
    public static Money Parse(string s) => Parse(s, NumberStyles.Currency, provider: null);

    /// <summary>Converts the string representation of a money value to its <see cref="Money"/> equivalent.</summary>
    /// <param name="s">The string representation of the number to convert.</param>
    /// <param name="provider">An object that supplies culture-specific parsing information about <i>value</i>.</param>
    /// <returns>The equivalent to the money amount contained in <i>value</i>.</returns>
    /// <exception cref="System.ArgumentNullException"><i>value</i> is <b>null</b> or empty.</exception>
    /// <exception cref="System.FormatException"><i>value</i> is not in the correct format or the currency sign matches with multiple known currencies.</exception>
    /// <exception cref="System.OverflowException"><i>value</i> represents a number less than <see cref="decimal.MinValue"/> or greater than <see cref="decimal.MaxValue"/>.</exception>
    public static Money Parse(string s, IFormatProvider? provider) => Parse(s, NumberStyles.Currency, provider);

    /// <summary>Converts the string representation of a money value to its <see cref="Money"/> equivalent.</summary>
    /// <param name="s">The string representation of the number to convert.</param>
    /// <param name="style">A bitwise combination of enumeration values that indicates the permitted format of value. A typical value to specify is <see cref="NumberStyles.Currency"/>.</param>
    /// <param name="provider">An object that supplies culture-specific parsing information about <i>value</i>.</param>
    /// <returns>The equivalent to the money amount contained in <i>value</i>.</returns>
    /// <exception cref="System.ArgumentNullException"><i>value</i> is <b>null</b> or empty.</exception>
    /// <exception cref="System.FormatException"><i>value</i> is not in the correct format or the currency sign matches with multiple known currencies.</exception>
    /// <exception cref="System.OverflowException"><i>value</i> represents a number less than <see cref="decimal.MinValue"/> or greater than <see cref="decimal.MaxValue"/>.</exception>
    public static Money Parse(string s, NumberStyles style, IFormatProvider? provider = null)
    {
            if (string.IsNullOrWhiteSpace(s))
                throw new ArgumentNullException(nameof(s));

            CurrencyInfo currencyInfo = (provider is CurrencyInfo ci) ? ExtractCurrencyInfoFromString(s, ci) : ExtractCurrencyInfoFromString(s);
            if (provider == null)
            {
                provider = (IFormatProvider?)currencyInfo.GetFormat(typeof(NumberFormatInfo)); //GetFormatProvider(currency.Value, null);
            }

            decimal amount = decimal.Parse(s, style, provider);

            return new Money(amount, currencyInfo);
        }

    /// <summary>Converts the string representation of a money value to its <see cref="Money"/> equivalent. A return value indicates whether the conversion succeeded or failed.</summary>
    /// <param name="s">The string representation of the money to convert.</param>
    /// <param name="result">When this method returns, contains the <see cref="Money"/> value that is equivalent to the money
    /// value contained in <i>value</i>, if the conversion succeeded, or is Money value of zero with no currency (XXX) if the
    /// conversion failed. The conversion fails if the <i>value</i> parameter is <b>null</b> or <see cref="string.Empty"/>, is not a number
    /// in a valid format, or represents a number less than <see cref="decimal.MinValue"/> or greater than <see cref="decimal.MaxValue"/>. This parameter is passed
    /// uninitialized; any <i>value</i> originally supplied in result will be overwritten.</param>
    /// <returns><b>true</b> if <i>value</i> was converted successfully; otherwise, <b>false</b>.</returns>
    /// <remarks>See <see cref="decimal.TryParse(string, out decimal)"/> for more info and remarks.</remarks>
    public static bool TryParse([NotNullWhen(true)] string? s, out Money result) =>
        TryParse(s, NumberStyles.Currency, provider: null, currency: null, out result);

    /// <summary>Converts the string representation of a money value to its <see cref="Money"/> equivalent. A return value indicates whether the conversion succeeded or failed.</summary>
    /// <param name="s">The string representation of the money to convert.</param>
    /// <param name="provider">An object that supplies culture-specific parsing information about <i>value</i>.</param>
    /// <param name="result">When this method returns, contains the <see cref="Money"/> value that is equivalent to the money
    /// value contained in <i>value</i>, if the conversion succeeded, or is Money value of zero with no currency (XXX) if the
    /// conversion failed. The conversion fails if the <i>value</i> parameter is <b>null</b> or <see cref="string.Empty"/>, is not a number
    /// in a valid format, or represents a number less than <see cref="decimal.MinValue"/> or greater than <see cref="decimal.MaxValue"/>. This parameter is passed
    /// uninitialized; any <i>value</i> originally supplied in result will be overwritten.</param>
    /// <returns><b>true</b> if <i>value</i> was converted successfully; otherwise, <b>false</b>.</returns>
    /// <remarks>See <see cref="decimal.TryParse(string, NumberStyles, IFormatProvider, out decimal)"/> for more info and remarks.</remarks>
    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out Money result) =>
        TryParse(s, NumberStyles.Currency, provider, currency: null, out result);

    /// <summary>Converts the string representation of a money value to its <see cref="Money"/> equivalent. A return value indicates whether the conversion succeeded or failed.</summary>
    /// <param name="s">The string representation of the money to convert.</param>
    /// <param name="currency">The currency to use for parsing the string representation.</param>
    /// <param name="result">When this method returns, contains the <see cref="Money"/> value that is equivalent to the money
    /// value contained in <i>value</i>, if the conversion succeeded, or is Money value of zero with no currency (XXX) if the
    /// conversion failed. The conversion fails if the <i>value</i> parameter is <b>null</b> or <see cref="string.Empty"/>, is not a number
    /// in a valid format, or represents a number less than <see cref="decimal.MinValue"/> or greater than <see cref="decimal.MaxValue"/>. This parameter is passed
    /// uninitialized; any <i>value</i> originally supplied in result will be overwritten.</param>
    /// <returns><b>true</b> if <i>value</i> was converted successfully; otherwise, <b>false</b>.</returns>
    /// <remarks>See <see cref="decimal.TryParse(string, out decimal)"/> for more info and remarks.</remarks>
    public static bool TryParse([NotNullWhen(true)] string? s, Currency currency, out Money result) =>
        TryParse(s, NumberStyles.Currency, provider: null, currency, out result);

    /// <summary>Converts the string representation of a money value to its <see cref="Money"/> equivalent. A return value indicates whether the conversion succeeded or failed.</summary>
    /// <param name="s">The string representation of the money to convert.</param>
    /// <param name="style">A bitwise combination of enumeration values that indicates the permitted format of value. A typical value to specify is <see cref="NumberStyles.Currency"/>.</param>
    /// <param name="provider">An object that supplies culture-specific parsing information about <i>value</i>.</param>
    /// <param name="currency">The currency to use for parsing the string representation.</param>
    /// <param name="result">When this method returns, contains the <see cref="Money"/> value that is equivalent to the money
    /// value contained in <i>value</i>, if the conversion succeeded, or is Money value of zero with no currency (XXX) if the
    /// conversion failed. The conversion fails if the <i>value</i> parameter is <b>null</b> or <see cref="string.Empty"/>, is not a number
    /// in a valid format, or represents a number less than <see cref="decimal.MinValue"/> or greater than <see cref="decimal.MaxValue"/>. This parameter is passed
    /// uninitialized; any <i>value</i> originally supplied in result will be overwritten.</param>
    /// <returns><b>true</b> if <i>value</i> was converted successfully; otherwise, <b>false</b>.</returns>
    /// <remarks>See <see cref="decimal.TryParse(string, NumberStyles, IFormatProvider, out decimal)"/> for more info and remarks.</remarks>
    public static bool TryParse([NotNullWhen(true)] string? s, NumberStyles style, IFormatProvider? provider, Currency? currency, out Money result)
    {
        if (string.IsNullOrWhiteSpace(s))
        {
            result = new Money(0, CurrencyInfo.NoCurrency);
            return false;
        }

        if (currency == null)
        {
            try
            {
                currency = ExtractCurrencyInfoFromString(s);
            }
            catch (FormatException)
            {
                result = new Money(0, CurrencyInfo.NoCurrency);
                return false;
            }
        }

        bool isParsingSuccessful = decimal.TryParse(s, style, GetFormatProvider(currency.Value, provider), out decimal amount);
        if (isParsingSuccessful)
        {
            result = new Money(amount, currency.Value);
            return true;
        }

        result = new Money(0, CurrencyInfo.NoCurrency);
        return false;
    }

    private static CurrencyInfo ExtractCurrencyInfoFromString(string value, CurrencyInfo? currencyInfo = null)
    {
            // TODO: How to handle alternative symbols, like US$ => AlternativeCurrencySymbols in CurrencyInfo?
            string currencyAsString = new string(value.Cast<char>().Where(IsNotNumericCharacter()).ToArray());

            if (currencyAsString.Length == 0 || CurrencyInfo.CurrentCurrency.Symbol == currencyAsString
                || CurrencyInfo.CurrentCurrency.Code == currencyAsString)
            {
                return currencyInfo ?? CurrencyInfo.CurrentCurrency;
            }

            List<CurrencyInfo> match =
                CurrencyInfo.GetAllCurrencies().Where(c => c.Symbol == currencyAsString || c.Code == currencyAsString).ToList();

            if (match.Count == 0)
            {
                throw new FormatException($"{currencyAsString} is an unknown currency sign or code!");
            }

            if (match.Count > 1)
            {
                if (currencyInfo == null)
                {
                    throw new FormatException(
                        $"Currency sign {currencyAsString} matches with multiple known currencies! Specify currency or culture explicit.");
                }

                if (match.Any(c => c == currencyInfo))
                {
                    return match.First(c => c == currencyInfo);
                }
                else
                {
                    throw new FormatException(
                        $"Currency sign {currencyAsString} matches with multiple known currencies, but none match with specified {currencyInfo.Code} or {currencyInfo.Symbol}!");
                }
            }

            if (match.Count == 1 && (currencyInfo is not null && match[0] != currencyInfo))
            {
                throw new FormatException(
                    $"Currency sign {currencyAsString} matches with {match[0].Code} or {match[0].Symbol}, but doesn't match with specified {currencyInfo.Code} or {currencyInfo.Symbol}!");
            }

            return match[0];
        }

    private static Func<char, bool> IsNotNumericCharacter() => character =>
        !char.IsDigit(character) && !char.IsWhiteSpace(character) &&
        character != '.' && character != ',' && character != '(' && character != ')' && character != '+' && character != '-';

    // public static Money Parse(ReadOnlySpan<char> s, IFormatProvider? provider) => throw new NotImplementedException();
    //
    // public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out Money result) => throw new NotImplementedException();
    //
    // public static Money Parse(ReadOnlySpan<byte> utf8Text, IFormatProvider? provider) => throw new NotImplementedException();
    //
    // public static bool TryParse(ReadOnlySpan<byte> utf8Text, IFormatProvider? provider, out Money result) => throw new NotImplementedException();
}
