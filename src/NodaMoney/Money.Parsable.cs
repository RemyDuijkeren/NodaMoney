using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.Unicode;

namespace NodaMoney;

public partial struct Money
#if NET7_0_OR_GREATER
     : ISpanParsable<Money>, IUtf8SpanParsable<Money>
#endif
{
    private const NumberStyles ParseNumberStyle = NumberStyles.Currency & ~NumberStyles.AllowCurrencySymbol;

    /// <summary>Parse a string to <see cref="Money"/>.</summary>
    /// <param name="s">The string to parse.</param>
    /// <returns>The result of parsing <paramref name="s"/> to a <see cref="Money"/> instance.</returns>
    /// <exception cref="System.ArgumentNullException"><paramref name="s"/> is <b>null</b>.</exception>
    /// <exception cref="System.FormatException"><paramref name="s"/> is not in the correct format.</exception>
    /// <exception cref="System.OverflowException"><paramref name="s"/> is not representable by <see cref="Money"/>.</exception>
    public static Money Parse(string s) => Parse(s, provider: null);

    /// <inheritdoc cref="Parse(string)"/>
    /// <param name="provider">An object that supplies culture-specific parsing information about <paramref name="s"/>, like <see cref="CurrencyInfo"/>.</param>
    [SuppressMessage("ReSharper", "InvalidXmlDocComment")]
    public static Money Parse(string s, IFormatProvider? provider) =>
        s == null ? throw new ArgumentNullException(nameof(s)) : Parse(s.AsSpan(), provider);

    /// <inheritdoc cref="Parse(string, IFormatProvider)"/>
    /// <summary>Parse span of characters into <see cref="Money"/>.</summary>
    public static Money Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
    {
        ReadOnlySpan<char> currencySymbol = ParseCurrencySymbol(s);

        CurrencyInfo currencyInfo = (provider is CurrencyInfo ci)
            ? ParseCurrencyInfo(currencySymbol, ci)
            : ParseCurrencyInfo(currencySymbol);

        ReadOnlySpan<char> numericInput = RemoveCurrencySymbol(s, currencySymbol);
        NumberFormatInfo nfi = GetNumberFormatInfo(provider);

#if NET7_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        decimal amount = decimal.Parse(numericInput, ParseNumberStyle, nfi);
#else
        decimal amount = decimal.Parse(numericInput.ToString(), ParseNumberStyle, nfi);
#endif

        return new Money(amount, currencyInfo);
    }

#if NET7_0_OR_GREATER
    /// <summary>Parse the UTF-8 encoded text to <see cref="Money"/>.</summary>
    /// <param name="utf8Text">The UTF-8 encoded text to parse.</param>
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
        return Parse(charSpan, provider);
    }
#endif

    /// <summary>Tries to parse a string into a <see cref="Money"/>.</summary>
    /// <param name="s">The string to parse.</param>
    /// <param name="result">When this method returns, contains the <see cref="Money"/> value that is equivalent to the money
    /// value contained in <paramref name="s"/>, if the conversion succeeded, or is Money value of zero with no currency (XXX) if the
    /// conversion failed. The conversion fails if the <paramref name="s"/> parameter is <b>null</b> or <see cref="string.Empty"/>, is not a number
    /// in a valid format, or represents a number less than <see cref="decimal.MinValue"/> or greater than <see cref="decimal.MaxValue"/>. This parameter is passed
    /// uninitialized; any <i>value</i> originally supplied in the result will be overwritten.</param>
    /// <returns><b>true</b> if <paramref name="s"/> parsed successfully; otherwise, <b>false</b>.</returns>
    /// <remarks>See <see cref="decimal.TryParse(string, out decimal)"/> for more info and remarks.</remarks>
    public static bool TryParse([NotNullWhen(true)] string? s, out Money result) =>
        TryParse(s, provider: null, out result);

    /// <inheritdoc cref="TryParse(string, out Money)"/>
    /// <param name="provider">An object that supplies culture-specific parsing information about <i>value</i>, like <see cref="CurrencyInfo"/>.</param>
    [SuppressMessage("ReSharper", "InvalidXmlDocComment")]
    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out Money result)
    {
        if (s is null)
        {
            result = new Money(0, CurrencyInfo.NoCurrency);
            return false;
        }

        return TryParse(s.AsSpan(), provider, out result);
    }

    /// <inheritdoc cref="TryParse(string?, IFormatProvider?, out Money)"/>
    /// <summary>Tries to parse span of characters into a <see cref="Money"/>.</summary>
    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out Money result)
    {
        try
        {
            if (s.IsEmpty || s.IsWhiteSpace())
            {
                result = new Money(0, CurrencyInfo.NoCurrency);
                return false;
            }

            ReadOnlySpan<char> currencySymbol = ParseCurrencySymbol(s);

            CurrencyInfo currencyInfo = provider is CurrencyInfo ci
                ? ParseCurrencyInfo(currencySymbol, ci)
                : ParseCurrencyInfo(currencySymbol);

            ReadOnlySpan<char> numericInput = RemoveCurrencySymbol(s, currencySymbol);
            NumberFormatInfo nfi = GetNumberFormatInfo(provider);

#if NET7_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            bool isParsed = decimal.TryParse(numericInput, ParseNumberStyle, nfi, out decimal amount);
#else
            bool isParsed = decimal.TryParse(numericInput.ToString(), ParseNumberStyle, nfi, out decimal amount);
#endif
            if (isParsed)
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
    /// <summary>Tries to parse the UTF-8 encoded text into a <see cref="Money"/>.</summary>
    /// <param name="utf8Text">The UTF-8 encoded text to parse.</param>
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
        return TryParse(charSpan, provider, out result);
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
                throw new FormatException($"Currency symbol {currencyChars.ToString()} is an unknown currency symbol or code!");
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

                throw new FormatException($"Currency symbol {currencyChars.ToString()} matches with multiple currencies, but none match with specified {specifiedCurrency.Code}!");
            default:
                throw new IndexOutOfRangeException($"MatchedCurrencies.Count {matchedCurrencies.Count} has to be 0, 1 or > 1!"); // Should never happen
        }
    }

    private static ReadOnlySpan<char> RemoveCurrencySymbol(ReadOnlySpan<char> s, ReadOnlySpan<char> currencyChars)
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
            else if (matchStartIndex == 0) // Match is at the start
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

    private static ReadOnlySpan<char> ParseCurrencySymbol(ReadOnlySpan<char> s)
    {
        if (s.IsEmpty || s.IsWhiteSpace())
        {
            return [];
        }

        static bool IsSymbolChar(char c) => !(char.IsWhiteSpace(c) || c == '+' || c == '-' || char.IsDigit(c));

        int start = 0;
        int end = s.Length - 1;

        // Skip leading whitespace
        while (start <= end && char.IsWhiteSpace(s[start])) start++;
        if (start > end) return [];

        // Track if there's a leading '(' to potentially skip a trailing ')'
        bool hasOpeningParen = s[start] == '(';
        if (hasOpeningParen)
        {
            start++;
            while (start <= end && char.IsWhiteSpace(s[start])) start++;
        }

        // Suffix detection: look from the end for a run of symbol chars after trimming trailing whitespace and optional ')'
        int r = end;
        // Trim trailing whitespace
        while (r >= start && char.IsWhiteSpace(s[r])) r--;
        // If there's a trailing ')', allow it only if we had opening '('
        if (r >= start && s[r] == ')' && hasOpeningParen)
        {
            r--;
            while (r >= start && char.IsWhiteSpace(s[r])) r--;
        }

        // Collect a run of symbol chars at the end (suffix)
        int suffixEnd = r; // inclusive
        while (r >= start && IsSymbolChar(s[r])) r--;
        int suffixStart = r + 1; // first symbol char
        if (suffixStart <= suffixEnd)
        {
            // Verify there is at least one digit or sign before the symbol (to mimic the regex branch constraints)
            bool hasNumberOrSignBefore = false;
            for (int i = start; i < suffixStart; i++)
            {
                char c = s[i];
                if (c == '+' || c == '-' || char.IsDigit(c))
                {
                    hasNumberOrSignBefore = true;
                    break;
                }
            }

            if (hasNumberOrSignBefore)
            {
                return s.Slice(suffixStart, suffixEnd - suffixStart + 1);
            }
        }

        // Prefix detection: look from the start for a run of symbol chars after optional sign and whitespace
        int l = start;
        // Optional leading '-' sign can precede the prefix symbol (e.g., "-$123" or "-€ 123").
        if (l <= end && s[l] == '-')
        {
            l++;
            while (l <= end && char.IsWhiteSpace(s[l])) l++;
        }

        // Collect a run of symbol chars at the start (prefix)
        int prefixStart = l;
        while (l <= end && IsSymbolChar(s[l])) l++;
        int prefixEnd = l - 1; // inclusive

        if (prefixStart <= prefixEnd)
        {
            // Ensure there is at least one digit or sign somewhere after the symbol to match the regex prefix branch
            bool hasNumberOrSignAfter = false;
            for (int i = prefixEnd + 1; i <= end; i++)
            {
                char c = s[i];
                if (c == '+' || c == '-' || char.IsDigit(c))
                {
                    hasNumberOrSignAfter = true;
                    break;
                }
            }

            if (hasNumberOrSignAfter)
            {
                return s.Slice(prefixStart, prefixEnd - prefixStart + 1);
            }
        }

        return [];
    }

    private static NumberFormatInfo GetNumberFormatInfo(IFormatProvider? provider) =>
        provider switch
        {
            CultureInfo ci => ci.NumberFormat,
            NumberFormatInfo nfi => nfi,
            CurrencyInfo => CultureInfo.CurrentCulture.NumberFormat,
            _ => CultureInfo.CurrentCulture.NumberFormat
        };

}
