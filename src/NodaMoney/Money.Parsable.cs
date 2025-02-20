using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Text.Unicode;

namespace NodaMoney;

/// <summary>Represents Money, an amount defined in a specific Currency.</summary>
public partial struct Money
#if NET7_0_OR_GREATER
     : ISpanParsable<Money>, IUtf8SpanParsable<Money>
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
    /// <param name="provider">An object that supplies culture-specific parsing information about <i>value</i>, like <see cref="CurrencyInfo"/>.</param>
    /// <returns>The equivalent to the money amount contained in <i>value</i>.</returns>
    /// <exception cref="System.ArgumentNullException"><i>value</i> is <b>null</b> or empty.</exception>
    /// <exception cref="System.FormatException"><i>value</i> is not in the correct format or the currency sign matches with multiple known currencies.</exception>
    /// <exception cref="System.OverflowException"><i>value</i> represents a number less than <see cref="decimal.MinValue"/> or greater than <see cref="decimal.MaxValue"/>.</exception>
    public static Money Parse(string s, IFormatProvider? provider) => Parse(s, NumberStyles.Currency, provider);

    /// <summary>Converts the string representation of a money value to its <see cref="Money"/> equivalent.</summary>
    /// <param name="s">The string representation of the number to convert.</param>
    /// <param name="style">A bitwise combination of enumeration values that indicates the permitted format of value. A typical value to specify is <see cref="NumberStyles.Currency"/>.</param>
    /// <param name="provider">An object that supplies culture-specific parsing information about <i>value</i>, like <see cref="CurrencyInfo"/>.</param>
    /// <returns>The equivalent to the money amount contained in <i>value</i>.</returns>
    /// <exception cref="System.ArgumentNullException"><i>value</i> is <b>null</b> or empty.</exception>
    /// <exception cref="System.FormatException"><i>value</i> is not in the correct format or the currency sign matches with multiple known currencies.</exception>
    /// <exception cref="System.OverflowException"><i>value</i> represents a number less than <see cref="decimal.MinValue"/> or greater than <see cref="decimal.MaxValue"/>.</exception>
    public static Money Parse(string s, NumberStyles style, IFormatProvider? provider = null)
    {
        if (s == null)
        {
            throw new ArgumentNullException(nameof(s));
        }

        return Parse(s.AsSpan(), style, provider);
    }

    /// <summary>Converts the string representation of a money value to its <see cref="Money"/> equivalent.</summary>
    /// <param name="s">The string representation of the money value to convert.</param>
    /// <param name="provider">An object that supplies culture-specific parsing information about <i>value</i>, like <see cref="CurrencyInfo"/>.</param>
    /// <returns>The equivalent <see cref="Money"/> object represented by the input string.</returns>
    /// <exception cref="System.ArgumentNullException"><i>s</i> is <b>null</b> or empty.</exception>
    /// <exception cref="System.FormatException"><i>s</i> is not in a valid format or the currency sign matches with multiple known currencies.</exception>
    /// <exception cref="System.OverflowException"><i>s</i> represents a number less than <see cref="decimal.MinValue"/> or greater than <see cref="decimal.MaxValue"/>.</exception>
    public static Money Parse(ReadOnlySpan<char> s, IFormatProvider? provider) => Parse(s, NumberStyles.Currency, provider);

    /// <summary>Converts the specified read-only span of characters representing a money value to its <see cref="Money"/> equivalent.</summary>
    /// <param name="s">The read-only span of characters representing the money value to parse.</param>
    /// <param name="style">A bitwise combination of enumeration values that indicates the style elements that can be present in the string.</param>
    /// <param name="provider">An object that supplies culture-specific parsing information about <i>value</i>, like <see cref="CurrencyInfo"/>.</param>
    /// <returns>The <see cref="Money"/> equivalent of the parsed value.</returns>
    /// <exception cref="System.ArgumentNullException">Thrown when <paramref name="provider"/> is null and no currency information can be inferred from the input.</exception>
    /// <exception cref="System.FormatException">Thrown when <paramref name="s"/> is not in a recognized format.</exception>
    /// <exception cref="System.OverflowException">Thrown when the numeric value represented by <paramref name="s"/> is less than <see cref="decimal.MinValue"/> or greater than <see cref="decimal.MaxValue"/>.</exception>
    public static Money Parse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider)
    {
        ReadOnlySpan<char> currencyChars = ParseSymbol(s);

        CurrencyInfo currencyInfo = (provider is CurrencyInfo ci) ? ParseCurrencyInfo(currencyChars, ci) : ParseCurrencyInfo(currencyChars);
        provider ??= (IFormatProvider?)currencyInfo.GetFormat(typeof(NumberFormatInfo));

        ReadOnlySpan<char> numericInput = RemoveCurrencyChars(s, currencyChars);

#if NET7_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        decimal amount = decimal.Parse(numericInput, style, provider);
#else
        decimal amount = decimal.Parse(numericInput.ToString(), style, provider);
#endif

        return new Money(amount, currencyInfo);
    }

#if NET7_0_OR_GREATER
    /// <summary>Parses the UTF-8 encoded text representation of a monetary value to its <see cref="Money"/> equivalent.</summary>
    /// <param name="utf8Text">The UTF-8 encoded text representation of the monetary value.</param>
    /// <param name="provider">An object that provides culture-specific formatting information, or <c>null</c> to use the current culture.</param>
    /// <returns>The equivalent monetary value represented by the provided text.</returns>
    /// <exception cref="System.FormatException">The input is not a valid UTF-8 encoded text or cannot be parsed as a monetary value.</exception>
    static Money IUtf8SpanParsable<Money>.Parse(ReadOnlySpan<byte> utf8Text, IFormatProvider? provider)
    {
        // Attempt to decode UTF8 text directly to a char span without intermediate string allocation
        Span<char> charBuffer = stackalloc char[utf8Text.Length]; // Allocate on the stack

        // Decode UTF-8 to UTF-16 (char) directly
        if (Utf8.ToUtf16(utf8Text, charBuffer, out _, out int charsWritten, replaceInvalidSequences: true) != OperationStatus.Done)
        {
            throw new FormatException("The input is not a valid UTF-8 encoded text.");
        }

        ReadOnlySpan<char> charSpan = charBuffer[..charsWritten];

        // Delegate work to the existing ReadOnlySpan<char>-based Parse
        return Parse(charSpan, NumberStyles.Currency, provider);
    }
#endif

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
        TryParse(s, NumberStyles.Currency, provider: null, out result);

    /// <summary>Converts the string representation of a money value to its <see cref="Money"/> equivalent. A return value indicates whether the conversion succeeded or failed.</summary>
    /// <param name="s">The string representation of the money to convert.</param>
    /// <param name="provider">An object that supplies culture-specific parsing information about <i>value</i>, like <see cref="CurrencyInfo"/>.</param>
    /// <param name="result">When this method returns, contains the <see cref="Money"/> value that is equivalent to the money
    /// value contained in <i>value</i>, if the conversion succeeded, or is Money value of zero with no currency (XXX) if the
    /// conversion failed. The conversion fails if the <i>value</i> parameter is <b>null</b> or <see cref="string.Empty"/>, is not a number
    /// in a valid format, or represents a number less than <see cref="decimal.MinValue"/> or greater than <see cref="decimal.MaxValue"/>. This parameter is passed
    /// uninitialized; any <i>value</i> originally supplied in result will be overwritten.</param>
    /// <returns><b>true</b> if <i>value</i> was converted successfully; otherwise, <b>false</b>.</returns>
    /// <remarks>See <see cref="decimal.TryParse(string, NumberStyles, IFormatProvider, out decimal)"/> for more info and remarks.</remarks>
    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out Money result) =>
        TryParse(s, NumberStyles.Currency, provider, out result);

    /// <summary>Converts the string representation of a money value to its <see cref="Money"/> equivalent. A return value indicates whether the conversion succeeded or failed.</summary>
    /// <param name="s">The string representation of the money to convert.</param>
    /// <param name="style">A bitwise combination of enumeration values that indicates the permitted format of value. A typical value to specify is <see cref="NumberStyles.Currency"/>.</param>
    /// <param name="provider">An object that supplies culture-specific parsing information about <i>value</i>, like <see cref="CurrencyInfo"/>.</param>
    /// <param name="result">When this method returns, contains the <see cref="Money"/> value that is equivalent to the money
    /// value contained in <i>value</i>, if the conversion succeeded, or is Money value of zero with no currency (XXX) if the
    /// conversion failed. The conversion fails if the <i>value</i> parameter is <b>null</b> or <see cref="string.Empty"/>, is not a number
    /// in a valid format, or represents a number less than <see cref="decimal.MinValue"/> or greater than <see cref="decimal.MaxValue"/>. This parameter is passed
    /// uninitialized; any <i>value</i> originally supplied in result will be overwritten.</param>
    /// <returns><b>true</b> if <i>value</i> was converted successfully; otherwise, <b>false</b>.</returns>
    /// <remarks>See <see cref="decimal.TryParse(string, NumberStyles, IFormatProvider, out decimal)"/> for more info and remarks.</remarks>
    public static bool TryParse([NotNullWhen(true)] string? s, NumberStyles style, IFormatProvider? provider, out Money result)
    {
        if (s is null)
        {
            result = new Money(0, CurrencyInfo.NoCurrency);
            return false;
        }

        return TryParse(s.AsSpan(), style, provider, out result);
    }

    /// <summary>Tries to parse the specified span of characters into its <see cref="Money"/> equivalent.</summary>
    /// <param name="s">The span of characters containing the money value to convert.</param>
    /// <param name="provider">An object that provides culture-specific formatting information.</param>
    /// <param name="result">When this method returns, contains the <see cref="Money"/> value equivalent to the money amount in <paramref name="s"/>, if the conversion succeeded, or the default value of <see cref="Money"/> if the conversion failed.</param>
    /// <returns><c>true</c> if <paramref name="s"/> was converted successfully; otherwise, <c>false</c>.</returns>
    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out Money result) =>
        TryParse(s, NumberStyles.Currency, provider, out result);

    /// <summary>Attempts to convert the span representation of a money value to its <see cref="Money"/> equivalent.</summary>
    /// <param name="s">A span containing the representation of the money value to convert.</param>
    /// <param name="style">A bitwise combination of <see cref="NumberStyles"/> values that indicates the style elements that can be present in the value parameter.</param>
    /// <param name="provider">An <see cref="IFormatProvider"/> that provides culture-specific formatting information about the value.</param>
    /// <param name="result">When this method returns, contains the <see cref="Money"/> equivalent of the value contained in the input span if the conversion succeeded, or a default <see cref="Money"/> value if the conversion failed. This parameter is passed uninitialized.</param>
    /// <returns><c>true</c> if the value was converted successfully; otherwise, <c>false</c>.</returns>
    public static bool TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider, out Money result)
    {
        try
        {
            ReadOnlySpan<char> currencyChars = ParseSymbol(s);

            CurrencyInfo currencyInfo = (provider is CurrencyInfo ci) ? ParseCurrencyInfo(currencyChars, ci) : ParseCurrencyInfo(currencyChars);
            provider ??= (IFormatProvider?)currencyInfo.GetFormat(typeof(NumberFormatInfo));

            ReadOnlySpan<char> numericInput = RemoveCurrencyChars(s, currencyChars);

#if NET7_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            bool isParsingSuccessful = decimal.TryParse(numericInput, style, provider, out decimal amount);
#else
            bool isParsingSuccessful = decimal.TryParse(numericInput.ToString(), style, provider, out decimal amount);
#endif
            if (isParsingSuccessful)
            {
                result = new Money(amount, currencyInfo);
                return true;
            }

            result = new Money(0, CurrencyInfo.NoCurrency);
            return false;
        }
        catch (FormatException)
        {
            result = new Money(0, CurrencyInfo.NoCurrency);
            return false;
        }
    }

#if NET7_0_OR_GREATER
    /// <summary>Attempts to parse the specified UTF-8 encoded text into its <see cref="Money"/> equivalent.</summary>
    /// <param name="utf8Text">The UTF-8 encoded text representing the monetary value to parse.</param>
    /// <param name="provider">An object that provides culture-specific formatting information.</param>
    /// <param name="result">When this method returns, contains the parsed <see cref="Money"/> value if the conversion succeeded, or the default value if it failed.</param>
    /// <returns><c>true</c> if the text was successfully parsed; otherwise, <c>false</c>.</returns>
    static bool IUtf8SpanParsable<Money>.TryParse(ReadOnlySpan<byte> utf8Text, IFormatProvider? provider, out Money result)
    {
        // Decode UTF-8 encoded bytes to a char span
        Span<char> charBuffer = stackalloc char[utf8Text.Length]; // Allocate stack buffer

        if (Utf8.ToUtf16(utf8Text, charBuffer, out _, out int charsWritten, replaceInvalidSequences: true) != OperationStatus.Done)
        {
            result = default; // Set default result on failure
            return false;     // Invalid UTF-8 text
        }

        // Get the decoded char span
        ReadOnlySpan<char> charSpan = charBuffer[..charsWritten];

        // Delegate to the TryParse(charSpan, ...) API
        return TryParse(charSpan, NumberStyles.Currency, provider, out result);
    }
#endif

    /// <summary>Parses the currency information from the given span of characters, optionally using a specified currency.</summary>
    /// <param name="currencyChars">A span of characters containing the input string to parse for currency information.</param>
    /// <param name="specifiedCurrency">An optional currency to use when resolving the currency information.</param>
    /// <returns>The parsed <see cref="CurrencyInfo"/> representing the currency information found in the input.</returns>
    /// <exception cref="FormatException">Thrown when no matching currency symbol or code can be resolved from the input.</exception>
    /// <exception cref="IndexOutOfRangeException"></exception>
    internal static CurrencyInfo ParseCurrencyInfo(ReadOnlySpan<char> currencyChars, CurrencyInfo? specifiedCurrency = null)
    {
        if (currencyChars.IsEmpty)
            return specifiedCurrency ?? CurrencyInfo.CurrentCurrency;

        // try to find a match
        var matchedCurrencies = CurrencyInfo.GetAllCurrencies(currencyChars);
        switch (matchedCurrencies.Count)
        {
            case 0:
                throw new FormatException($"{currencyChars.ToString()} is an unknown currency symbol or code!");
            case 1:
                if (specifiedCurrency is not null && matchedCurrencies[0] != specifiedCurrency)
                    throw new FormatException($"Currency symbol {currencyChars.ToString()} matches with {matchedCurrencies[0].Code}, but doesn't match the specified {specifiedCurrency.Code}!");

                return matchedCurrencies[0];
            case > 1:
                // If specifiedCurrency matches, prioritize it and return immediately
                var matchedCurrency = matchedCurrencies.FirstOrDefault(ci => ci == specifiedCurrency);
                if (matchedCurrency is not null) return matchedCurrency;

                if (specifiedCurrency is null)
                {
                    // If the current currency matches, prioritize it and return immediately
                    matchedCurrency = matchedCurrencies.FirstOrDefault(ci => ci == CurrencyInfo.CurrentCurrency);
                    if (matchedCurrency is not null) return matchedCurrency;

                    throw new FormatException($"Currency symbol {currencyChars.ToString()} matches with multiple currencies! Specify currency or culture explicitly.");
                }

                throw new FormatException($"Currency symbol {currencyChars.ToString()} matches with multiple currencies, but doesn't match specified {specifiedCurrency.Code}!");
            default:
                throw new IndexOutOfRangeException($"MatchedCurrencies.Count {matchedCurrencies.Count} has to be 0, 1 or > 1!"); // Should never happen
        }
    }

    private static ReadOnlySpan<char> RemoveCurrencyChars(ReadOnlySpan<char> s, ReadOnlySpan<char> currencyChars)
    {
        // Find the first occurrence of the matched currency characters in the input
        int matchStartIndex = s.IndexOf(currencyChars, StringComparison.Ordinal);
        if (matchStartIndex >= 0)
        {
            // If a match is found, return a slice excluding the match
            int matchEndIndex = matchStartIndex + currencyChars.Length;

            // Trim spaces before the match
            while (matchStartIndex > 0 && char.IsWhiteSpace(s[matchStartIndex - 1]))
            {
                matchStartIndex--;
            }

            // Trim spaces after the match
            while (matchEndIndex < s.Length && char.IsWhiteSpace(s[matchEndIndex]))
            {
                matchEndIndex++;
            }

            if (matchStartIndex == 0 && matchEndIndex == s.Length) // Match covers the entire input
            {
                return [];
            }
            else if (matchStartIndex == 0) // Match is at the beginning
            {
                return s.Slice(matchEndIndex);
            }
            else if (matchEndIndex >= s.Length) // Match is at the end
            {
                return s.Slice(0, matchStartIndex);
            }
            else // Match is in the middle
            {
                var beforeMatch = s.Slice(0, matchStartIndex);
                var afterMatch = s.Slice(matchEndIndex);

                // Allocate a new buffer only when necessary for combining slices
                char[] buffer = new char[beforeMatch.Length + afterMatch.Length];
                beforeMatch.CopyTo(buffer);
                afterMatch.CopyTo(buffer.AsSpan(beforeMatch.Length));

                return buffer;
            }
        }

        // If no match is found, return the input as is
        return s;
    }

    private static ReadOnlySpan<char> ParseSymbol(ReadOnlySpan<char> s)
    {
        // Return immediately if the input is empty or whitespace
        if (s.IsEmpty || s.IsWhiteSpace())
        {
            return [];
        }

#if NET7_0_OR_GREATER
        var match = s_currencySymbolMatcher().Match(s.ToString());
        if (!match.Success)
        {
            return [];
        }

        var suffixSymbol = match.Groups[1].ValueSpan; // SuffixSymbolGroupIndex
        var prefixSymbol = match.Groups[2].ValueSpan; // PrefixSymbolGroupIndex

        if (!suffixSymbol.IsEmpty)
        {
            return suffixSymbol;
        }
        else if (!prefixSymbol.IsEmpty)
        {
            return prefixSymbol;
        }

#else
        var match = s_currencySymbolMatcher.Match(s.ToString());
        if (!match.Success)
        {
            return [];
        }

        var suffixSymbol = match.Groups[1].Value; // SuffixSymbolGroupIndex
        var prefixSymbol = match.Groups[2].Value; // PrefixSymbolGroupIndex

        if (!string.IsNullOrEmpty(suffixSymbol))
        {
            return suffixSymbol.AsSpan();
        }
        else if (!string.IsNullOrEmpty(prefixSymbol))
        {
            return prefixSymbol.AsSpan();
        }
#endif

        return [];
    }

    // Regex to capture symbol (4 or 5) and amount (6): @"^\(?\s*(([-+\d](.*\d)?)\s*([^-+\d\s]+)|([^-+\d\s]+)\s*([-+\d](.*\d)?))\s*\)?$"
#if NET7_0_OR_GREATER
    [GeneratedRegex(@"^[-(]?\s*(?:[-+\d].*?\s*([^-+\d\s]+)|([^-+\d\s]+).*?[-+\d])\s*\)?$",
        RegexOptions.CultureInvariant | RegexOptions.Singleline)]
    private static partial Regex s_currencySymbolMatcher();
#else
    private static readonly Regex s_currencySymbolMatcher = new(@"^[-(]?\s*(?:[-+\d].*?\s*([^-+\d\s]+)|([^-+\d\s]+).*?[-+\d])\s*\)?$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.Singleline);
#endif
}
