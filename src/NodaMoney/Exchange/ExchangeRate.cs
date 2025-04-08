using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace NodaMoney.Exchange;

/// <summary>A conversion of money of one currency into money of another currency.</summary>
/// <remarks>See http://en.wikipedia.org/wiki/Exchange_rate .</remarks>
public readonly record struct ExchangeRate
#if NET7_0_OR_GREATER
    : ISpanParsable<ExchangeRate>//, IUtf8SpanParsable<ExchangeRate>
#endif
{
    /// <summary>Initializes a new instance of the <see cref="ExchangeRate"/> struct.</summary>
    /// <param name="baseCurrency">The base currency.</param>
    /// <param name="quoteCurrency">The quote currency.</param>
    /// <param name="rate">The rate of the exchange.</param>
    /// <exception cref="ArgumentNullException">The value of 'baseCurrency' or 'quoteCurrency' cannot be null. </exception>
    /// <exception cref="ArgumentOutOfRangeException">Rate must be greater than zero.</exception>
    /// <exception cref="ArgumentException">When the base and quote currency are equal, the only allowed rate is 1.</exception>
    public ExchangeRate(Currency baseCurrency, Currency quoteCurrency, decimal rate)
        : this()
    {
        if (baseCurrency == quoteCurrency && rate != 1M)
            throw new ArgumentOutOfRangeException(nameof(rate), "When the base and quote currency are equal, the only allowed rate is 1!");
        if (rate < 0)
            throw new ArgumentOutOfRangeException(nameof(rate), "Rate must be greater than zero!");

        BaseCurrency = baseCurrency;
        QuoteCurrency = quoteCurrency;
        Value = rate; // value is a ratio
    }

    /// <summary>Initializes a new instance of the <see cref="ExchangeRate"/> struct.</summary>
    /// <param name="baseCurrency">The base currency.</param>
    /// <param name="quoteCurrency">The quote currency.</param>
    /// <param name="rate">The rate of the exchange.</param>
    /// <param name="numberOfDecimals">The number of decimals to round the exchange rate to.</param>
    public ExchangeRate(Currency baseCurrency, Currency quoteCurrency, double rate, int numberOfDecimals = 6)
        : this(baseCurrency, quoteCurrency, Math.Round((decimal)rate, numberOfDecimals))
    {
    }

    /// <summary>Initializes a new instance of the <see cref="ExchangeRate"/> struct.</summary>
    /// <param name="baseCode">The code of the base currency.</param>
    /// <param name="quoteCode">The code of the quote currency.</param>
    /// <param name="rate">The rate of the exchange.</param>
    public ExchangeRate(string baseCode, string quoteCode, decimal rate)
        : this(CurrencyInfo.FromCode(baseCode), CurrencyInfo.FromCode(quoteCode), rate)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="ExchangeRate"/> struct.</summary>
    /// <param name="baseCode">The code of the base currency.</param>
    /// <param name="quoteCode">The code of the quote currency.</param>
    /// <param name="rate">The rate of the exchange.</param>
    /// <param name="numberOfDecimals">The number of decimals to round the exchange rate to.</param>
    public ExchangeRate(string baseCode, string quoteCode, double rate, int numberOfDecimals = 6)
        : this(CurrencyInfo.FromCode(baseCode), CurrencyInfo.FromCode(quoteCode), rate, numberOfDecimals)
    {
    }

    /// <summary>Deconstruct the current instance into its components.</summary>
    /// <param name="baseCurrency">The base currency.</param>
    /// <param name="quoteCurrency">The quote currency.</param>
    /// <param name="rate">The rate of the exchange.</param>
    public void Deconstruct(out Currency baseCurrency, out Currency quoteCurrency, out decimal rate)
    {
        baseCurrency = BaseCurrency;
        quoteCurrency = QuoteCurrency;
        rate = Value;
    }

    /// <summary>Gets the base currency.</summary>
    /// <value>The base currency.</value>
    public Currency BaseCurrency { get; }

    /// <summary>Gets the quote currency.</summary>
    /// <value>The quote currency.</value>
    public Currency QuoteCurrency { get; }

    /// <summary>Gets the value of the exchange rate.</summary>
    /// <value>The value of the exchange rate.</value>
    public decimal Value { get; }

    /// <summary>Converts this <see cref="ExchangeRate"/> instance to its equivalent <see cref="string"/> representation.</summary>
    /// <returns>A string that represents this <see cref="ExchangeRate"/> instance.</returns>
    /// <remarks>See http://en.wikipedia.org/wiki/Currency_Pair for more info about how an ExchangeRate can be presented.</remarks>
    public override string ToString() => ToString(CultureInfo.CurrentCulture);

    /// <inheritdoc cref="ToString()"/>
    /// <param name="provider">The <see cref="IFormatProvider"/> used to format the amount</param>
    public string ToString(IFormatProvider provider) => $"{BaseCurrency.Code}/{QuoteCurrency.Code} {Value.ToString(provider)}";

    /// <summary>Parse a string into a <see cref="ExchangeRate"/>.</summary>
    /// <param name="s">The string to parse.</param>
    /// <returns>The result of parsing <paramref name="s"/> to a <see cref="ExchangeRate"/> instance.</returns>
    /// <exception cref="System.ArgumentNullException"><paramref name="s"/>  is <b>null</b>.</exception>
    /// <exception cref="System.FormatException"><paramref name="s"/> is not in the correct format.</exception>
    /// <exception cref="System.OverflowException"><paramref name="s"/> is not representable by <see cref="ExchangeRate"/>.</exception>
    public static ExchangeRate Parse(string s) => Parse(s, CultureInfo.CurrentCulture);

    /// <inheritdoc cref="Parse(string)"/>
    /// <param name="provider">The <see cref="IFormatProvider"/> used to parse numeric part of <paramref name="s"/></param>
    [SuppressMessage("ReSharper", "InvalidXmlDocComment")]
    public static ExchangeRate Parse(string s, IFormatProvider? provider) =>
        s == null ? throw new ArgumentNullException(nameof(s)) : Parse(s.AsSpan(), provider);

    /// <inheritdoc cref="Parse(string, IFormatProvider)"/>
    /// <summary>Parse span of characters into <see cref="ExchangeRate"/>.</summary>
    public static ExchangeRate Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
    {
        ReadOnlySpan<char> numericInput =
            ParseNumericInput(s, out ReadOnlySpan<char> baseCurrencySpan, out ReadOnlySpan<char> quoteCurrencySpan);

#if NET7_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        decimal rate = decimal.Parse(numericInput, NumberStyles.Currency, provider);
#else
        decimal rate = decimal.Parse(numericInput.ToString(), NumberStyles.Currency, provider);
#endif

        return new ExchangeRate(CurrencyInfo.FromCode(baseCurrencySpan.ToString()), CurrencyInfo.FromCode(quoteCurrencySpan.ToString()), rate);
    }

    /// <summary>Tries to parse a string into a <see cref="ExchangeRate"/>.</summary>
    /// <param name="s">The string to parse.</param>
    /// <param name="result">When this method returns, contains the <see cref="ExchangeRate"/> that is equivalent to the exchange rate contained in
    /// rate, if the conversion succeeded,
    /// or is zero if the conversion failed. The conversion fails if the rate parameter is null, is not an exchange rate in a
    /// valid format, or represents a number less than MinValue
    /// or greater than MaxValue. This parameter is passed uninitialized.</param>
    /// <returns><b>true</b> if <paramref name="s"/> parsed successfully; otherwise, <b>false</b>.</returns>
    public static bool TryParse([NotNullWhen(true)] string? s, out ExchangeRate result) =>
        TryParse(s, CultureInfo.CurrentCulture, out result);

    /// <inheritdoc cref="TryParse(string, out ExchangeRate)"/>
    /// <param name="provider">The <see cref="IFormatProvider"/> used to parse <paramref name="s"/></param>
    [SuppressMessage("ReSharper", "InvalidXmlDocComment")]
    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out ExchangeRate result)
    {
        if (s is null)
        {
            result = default;
            return false;
        }

        return TryParse(s.AsSpan(), provider, out result);
    }

    /// <inheritdoc cref="TryParse(string?, IFormatProvider?, out ExchangeRate)"/>
    /// <summary>Tries to parse span of characters into a <see cref="ExchangeRate"/>.</summary>
    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out ExchangeRate result)
    {
        try
        {
            if (s.IsEmpty || s.IsWhiteSpace())
            {
                result = default;
                return false;
            }

            ReadOnlySpan<char> numericInput =
                ParseNumericInput(s, out ReadOnlySpan<char> baseCurrencySpan, out ReadOnlySpan<char> quoteCurrencySpan);

#if NET7_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            bool isParsed = decimal.TryParse(numericInput, NumberStyles.Currency, provider, out decimal rate);
#else
            bool isParsed = decimal.TryParse(numericInput.ToString(), NumberStyles.Currency, provider, out decimal rate);
#endif
            if (isParsed)
            {
                result = new ExchangeRate(CurrencyInfo.FromCode(baseCurrencySpan.ToString()), CurrencyInfo.FromCode(quoteCurrencySpan.ToString()), rate);
                return true;
            }

            result = default;
            return false;
        }
        catch (Exception ex) when (ex is FormatException or ArgumentOutOfRangeException)
        {
            result = default;
            return false;
        }
    }

    /// <summary>Converts the specified money.</summary>
    /// <param name="money">The money.</param>
    /// <returns>The converted money.</returns>
    /// <exception cref="System.ArgumentException">Money should have the same currency as the base currency or the quote currency.</exception>
    public Money Convert(Money money)
    {
        if (money.Currency != BaseCurrency && money.Currency != QuoteCurrency)
        {
            throw new ArgumentException("Money should have the same currency as the base currency or the quote currency!", nameof(money));
        }

        return money.Currency == BaseCurrency
            ? new Money(money.Amount * Value, QuoteCurrency)
            : new Money(money.Amount / Value, BaseCurrency);
    }

    private static ReadOnlySpan<char> ParseNumericInput(ReadOnlySpan<char> s, out ReadOnlySpan<char> baseCurrencySpan, out ReadOnlySpan<char> quoteCurrencySpan)
    {
        baseCurrencySpan = ReadOnlySpan<char>.Empty;
        quoteCurrencySpan = ReadOnlySpan<char>.Empty;

        // The format is: ABCDEF1, ABC/DEF1, ABCDEF 1 or ABC/DEF 1
        if (s.Length < 7)
        {
            return ReadOnlySpan<char>.Empty;
        }

        s = s.Trim();
        baseCurrencySpan = s.Slice(0, 3);
        int separatorIndex = s[3] == '/' ? 4 : 3;
        quoteCurrencySpan = s.Slice(separatorIndex, 3);

        return s.Slice(separatorIndex + 3);
    }
}
