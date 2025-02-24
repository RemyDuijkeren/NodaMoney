using System.Diagnostics;
using System.Globalization;

namespace NodaMoney;

// TODO: Add support for the following properties?
// Thread.CurrentThread.CurrentCulture = new CurrentInfo("")
// Thread.CurrentThread.CurrentCurrency = new CurrencyInfo("");
//
// CultureInfo.DefaultThreadCurrentCurrency = CurrencyInfo;
//
// var x = CurrencyInfo.CurrentCurrency;
//
// IsObsolete, IsDeprecated, ReplacedBy, Replaces, Locals (NL, EN),
// Order/Priority/Weight=1,0.8,0.6 = q-factor weighting (value between 0 and 1) = Any value placed in an order of preference expressed using a relative quality value called weight.

// TODO: Should we use internal sealed class CurrencyData to store the data? CultureInfo has a similar structure called
// CultureData. https://github.com/dotnet/runtime/blob/main/src/libraries/System.Private.CoreLib/src/System/Globalization/CultureData.cs

/// <summary>A unit of exchange of value, a currency of <see cref="Money" />.</summary>
/// <remarks>See http://en.wikipedia.org/wiki/Currency and
/// https://en.wikipedia.org/wiki/List_of_circulating_currencies and
/// https://www.six-group.com/en/products-services/financial-information/data-standards.html#scrollTo=isin</remarks>
public record CurrencyInfo : IFormatProvider, ICustomFormatter
{
    /// <summary>Gets the currency sign (¤), a character used to denote the generic currency sign, when no currency sign is available.</summary>
    /// <remarks>See https://en.wikipedia.org/wiki/Currency_sign_(typography). </remarks>
    public const string GenericCurrencySign = "¤";

    const string InvalidCurrencyMessage = "Currency code should only exist out of three capital letters";

    readonly string? _internationalSymbol;

    // [ThreadStatic] static CurrencyInfo? s_currentThreadCurrency;
    //
    // static CurrencyInfo()
    // {
    //     s_currentThreadCurrency = NoCurrency;
    // }

    /// <summary>A unit of exchange of value, a currency of <see cref="Money" />.</summary>
    /// <remarks>See http://en.wikipedia.org/wiki/Currency and
    /// https://en.wikipedia.org/wiki/List_of_circulating_currencies and
    /// https://www.six-group.com/en/products-services/financial-information/data-standards.html#scrollTo=isin</remarks>
    /// <param name="code">The (ISO-4217) three-character code of the currency.</param>
    /// <param name="number">The (ISO-4217) number of the currency.</param>
    /// <param name="minorUnit">The minor unit, as an exponent of base 10, by which the currency unit can be divided in.</param>
    /// <param name="englishName">The english name of the currency</param>
    /// <param name="symbol">The currency symbol.</param>
    internal CurrencyInfo(string code, short number, MinorUnit minorUnit, string englishName = "", string symbol = GenericCurrencySign)
    {
        Code = code ?? throw new ArgumentNullException(nameof(code));
        Number = number;
        MinorUnit = minorUnit;
        EnglishName = englishName ?? string.Empty;
        Symbol = symbol ?? GenericCurrencySign;

        Debug.Assert(Code != null, "Code should not be null");
        Debug.Assert(Code!.Length == 3, InvalidCurrencyMessage);
        Debug.Assert(Code.All(c => c is >= 'A' and <= 'Z'), InvalidCurrencyMessage);
        Debug.Assert(Number >= 0, "Number should be greater or equal to 0");
        Debug.Assert(EnglishName != null, "EnglishName should not be null");
        Debug.Assert(Symbol != null, "Symbol should not be null");
    }

    public static readonly CurrencyInfo NoCurrency = new("XXX", 999, MinorUnit.NotApplicable, "No Currency");

    /// <summary>Gets the Currency that represents the country/region used by the current thread.</summary>
    /// <value>The Currency that represents the country/region used by the current thread.</value>
    public static CurrencyInfo CurrentCurrency
    {
        get
        {
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            // In >= .NET5 when CurrentCulture is Invariant, then RegionInfo.CurrentRegion is retrieved from
            // Windows settings. See also https://github.com/xunit/samples.xunit/pull/18
            var currentCulture = CultureInfo.CurrentCulture;
            if (Equals(currentCulture, CultureInfo.InvariantCulture)) // no region information can be extracted
            {
                return NoCurrency;
            }

            return FromCulture(currentCulture);
#else
            RegionInfo currentRegion = RegionInfo.CurrentRegion;
            return currentRegion.Name == "IV" ? NoCurrency : GetInstance(currentRegion);
#endif
        }
    }

    /// <summary>The (ISO-4217) three-character code of the currency.</summary>
    public string Code { get; init; }

    /// <summary>The (ISO-4217) number of the currency.</summary>
    public short Number { get; init; }

    /// <summary>The minor unit, as an exponent of base 10, by which the currency unit can be divided in.</summary>
    public MinorUnit MinorUnit { get; init; }

    /// <summary>The english name of the currency</summary>
    public string EnglishName { get; init; } = string.Empty;

    /// <summary>The (local) currency symbol.</summary>
    public string Symbol { get; init; } = GenericCurrencySign;

    /// <summary>The international currency symbol.</summary>
    public string InternationalSymbol
    {
        get => _internationalSymbol ?? Symbol;
        init => _internationalSymbol = value;
    }

    /// <summary>Gets a collection of alternative symbols used to represent the currency.</summary>
    /// <remarks>
    /// This property includes any unconventional or secondary symbols that may be associated with the currency,
    /// in addition to its primary symbol.
    /// </remarks>
    public IReadOnlyList<string> AlternativeSymbols { get; init; } = [];

    /// <summary>Indicates whether the currency is in the ISO 4217 standard.</summary>
    /// <remarks>
    /// ISO 4217 is an international standard for defining codes for the representation of currencies and funds.
    /// This property specifies whether the currency code conforms to this standard.
    /// </remarks>
    public bool IsIso4217 { get; init; } = true;

    /// <summary>Gets the date when the currency is expired on (list 3 Historic).</summary>
    /// <value>The expiry date when the currency is not valid anymore.</value>
    public DateTime? ExpiredOn { get; init; }

    /// <summary>Gets the date when the currency is introduced on (list 1 Active).</summary>
    /// <value>The introduction date when the currency is valid.</value>
    public DateTime? IntroducedOn { get; init; }

    /// <summary>Gets the (ISO-4217) three-digit code number of the currency.</summary>
    public string NumericCode => Number.ToString("D3", CultureInfo.InvariantCulture);

    /// <summary>Gets the number of digits after the decimal separator.</summary>
    /// <remarks>
    /// <para>
    /// For example, the default number of fraction digits for the US Dollar and Euro is 2, while for the Japanese Yen it's 0.
    /// In the case of pseudo-currencies, such as Gold or IMF Special Drawing Rights, 0 is returned.
    /// </para>
    /// <para>
    /// Mauritania does not use a decimal division of units, setting 1 ouguiya (UM) equal to 5 khoums, and Madagascar has 1 ariary =
    /// 5 iraimbilanja. The coins display "1/5" on their face and are referred to as a "fifth". These are not used in practice, but when
    /// written out, a single significant digit is used (E.g. 1.2 UM), so 1 is returned.
    /// </para>
    /// </remarks>
    public int DecimalDigits =>
        MinorUnit switch
        {
            MinorUnit.NotApplicable => 0,
            MinorUnit.OneFifth => 1,
            _ => (int)MinorUnit,
        };

    /// <summary>Gets the smallest amount of the currency unit.</summary>
    public decimal MinimalAmount => MinorUnit == 0 ? 1m : (decimal)(1.0 / Math.Pow(10, MinorUnitAsExponentOfBase10));

    /// <summary>Gets the minor unit, as an exponent of base 10, by which the currency unit can be divided in.</summary>
    /// <para>
    /// The US dollar can be divided into 100 cents (1/100), which is 10^2, so the exponent 2 will be returned.
    /// </para>
    /// <para>
    /// Mauritania does not use a decimal division of units, but has 1 ouguiya (UM) which can be divided int 5 khoums (1/5), which is
    /// 10^log10(5) = 10^0.698970004, so the exponent 0.698970004 will be returned.
    /// </para>
    public double MinorUnitAsExponentOfBase10
    {
        get
        {
            // https://www.iso.org/obp/ui/#iso:std:iso:4217:ed-8:v1:en
            // unit of recorded value (i.e. as recorded by banks) which is a division of the respective unit of currency or fund
            return MinorUnit switch
            {
                MinorUnit.NotApplicable => 0,
                MinorUnit.OneFifth => Math.Log10(5),
                _ => (double)MinorUnit,
            };
        }
    }

    /// <summary>Gets a value indicating whether the minor unit of the currency is based on the decimal system.</summary>
    /// <value><c>true</c> if minor unit is decimal based; otherwise, <c>false</c>.</value>
    /// <remarks>
    /// This property evaluates if the minor unit represents decimal-base minor units (e.g., USD, where 1 unit = 100 minor units).
    /// Certain currencies might use non-decimal-based minor units (e.g., MRU, where 1 unit = 5 minor units).
    /// </remarks>
    public bool MinorUnitIsDecimalBased =>
        MinorUnit switch
        {
            MinorUnit.NotApplicable => true,
            MinorUnit.OneFifth => false,
            _ => true,
        };

    /// <summary>Gets the number of minor units by which the currency unit can be divided in.</summary>
    /// <para>
    /// The US dollar can be divided into 100 cents (1/100) so the 100 will be returned.
    /// </para>
    /// <para>
    /// Mauritania does not use a decimal division of units, but has 1 ouguiya (UM) which can be divided int 5 khoums (1/5),
    /// so 5 will be returned.
    /// </para>
    public double MinorUnits => Math.Pow(10, MinorUnitAsExponentOfBase10);

    // public string NativeName { get; init; } = EnglishName;
    // public string FractionalSingularUnitName { get; init; } = "cent";
    // public string FractionalPluralUnitName { get; init; } = "cents";
    // public string SingularUnitName { get; init; } = "dollar";
    // public string PluralUnitName { get; init; } = "dollars";

    /// <summary>Gets a value indicating whether currency is historic.</summary>
    /// <value><c>true</c> if this instance is valid; otherwise, <c>false</c>.</value>
    public bool IsHistoric => !IsActiveOn(DateTime.Today);

    /// <summary>Defines an implicit conversion operator from <see cref="CurrencyInfo"/> to <see cref="Currency"/>.</summary>
    /// <param name="currencyInfo">The currency information from which a <see cref="Currency"/> instance will be created.</param>
    /// <returns>A new instance of <see cref="Currency"/> initialized with the provided <see cref="CurrencyInfo"/>.</returns>
    public static implicit operator Currency(CurrencyInfo currencyInfo) => new(currencyInfo.Code.AsSpan(), currencyInfo.IsIso4217);

    /// <summary>Creates a new instance of <see cref="CurrencyInfo"/> with the specified three-character currency code.</summary>
    /// <param name="code">The (ISO-4217) three-character currency code.</param>
    /// <remarks>The returned <see cref="CurrencyInfo"/> can be altered further by using the record <c>with { }</c> syntax.</remarks>
    /// <returns>The created <see cref="CurrencyInfo"/> instance.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the specified code is null or whitespace.</exception>
    /// <exception cref="ArgumentException">Thrown when the specified code is not exactly three characters long or does not consist of three uppercase letters.</exception>
    public static CurrencyInfo Create(string code)
    {
        ValidateCurrencyCode(code);

        return new CurrencyInfo(code, 0, MinorUnit.NotApplicable) { IsIso4217 = false };
    }

    /// <summary>Registers the specified <see cref="CurrencyInfo"/> as a custom currency for the current AppDomain.</summary>
    /// <param name="currencyInfo">The given <see cref="CurrencyInfo"/></param>
    /// <exception cref="ArgumentNullException">Thrown when the specified code is null or whitespace.</exception>
    /// <exception cref="ArgumentException">Thrown when the specified code is not exactly three characters long or does not consist of three uppercase letters.</exception>
    /// <exception cref="InvalidCurrencyException">The custom currency is already registered.</exception>
    public static void Register(CurrencyInfo currencyInfo)
    {
        ValidateCurrencyCode(currencyInfo.Code);
        ValidateCurrencyNumber(currencyInfo.Number, currencyInfo.IsIso4217);

        if (!CurrencyRegistry.TryAdd(currencyInfo))
            throw new InvalidCurrencyException($"The currency {currencyInfo.Code} is already registered.");
    }

    /// <summary>Unregisters the specified currency code from the current AppDomain and returns it.</summary>
    /// <param name="code">The name of the currency to unregister.</param>
    /// <returns>An instance of the type <see cref="CurrencyInfo"/>.</returns>
    /// <exception cref="ArgumentException">code specifies a currency that is not found in the register.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="code" /> is <see langword="null" /> or empty.</exception>
    /// <exception cref="InvalidCurrencyException"> when currency is not registered.</exception>
    public static CurrencyInfo Unregister(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentNullException(nameof(code));

        CurrencyInfo currencyInfo = FromCode(code);
        if (CurrencyRegistry.TryRemove(currencyInfo))
            return currencyInfo;

        throw new InvalidCurrencyException($"Can't unregister the currency {code} because it is not registered!");
    }

    /// <summary>Check a value indication whether currency is valid on a given date.</summary>
    /// <param name="date">The date on which the Currency should be valid.</param>
    /// <returns><c>true</c> when the date is within the valid range of this currency; otherwise <c>false</c>.</returns>
    public bool IsActiveOn(DateTime date) =>
        (!IntroducedOn.HasValue || IntroducedOn <= date) &&
        (!ExpiredOn.HasValue || ExpiredOn >= date);

    /// <summary>Get all currencies.</summary>
    /// <returns>An <see cref="IReadOnlyList{CurrencyInfo}"/> of all registered currencies.</returns>
    public static IReadOnlyList<CurrencyInfo> GetAllCurrencies() => CurrencyRegistry.GetAllCurrencies();

    /// <summary>Get all currencies that matches the given Currency Code or Symbol.</summary>
    /// <param name="currencyChars">The Currency Code or Symbol to match.</param>
    /// <returns>An <see cref="IReadOnlyList{CurrencyInfo}"/> of all currencies that matches.</returns>
    public static IReadOnlyList<CurrencyInfo> GetAllCurrencies(ReadOnlySpan<char> currencyChars) => CurrencyRegistry.GetAllCurrencies(currencyChars);

    /// <summary>Create an instance of the <see cref="CurrencyInfo"/> based on a ISO 4217 currency code.</summary>
    /// <param name="code">A ISO 4217 currency code, like EUR or USD.</param>
    /// <returns>An instance of the type <see cref="CurrencyInfo"/>.</returns>
    /// <exception cref="ArgumentNullException">The value of 'code' cannot be null.</exception>
    /// <exception cref="ArgumentException">The 'code' is an unknown ISO 4217 currency code.</exception>
    public static CurrencyInfo FromCode(string code) => CurrencyRegistry.Get(code);

    /// <summary>Retrieves a <see cref="CurrencyInfo"/> instance based on the provided <see cref="IFormatProvider"/>.</summary>
    /// <param name="formatProvider">
    /// The <see cref="IFormatProvider"/> instance used to determine the currency information. This can be a
    /// <see cref="CurrencyInfo"/>, <see cref="CultureInfo"/>, or <see cref="NumberFormatInfo"/>.
    /// </param>
    /// <returns>A <see cref="CurrencyInfo"/> instance based on the provided format provider or the current currency if no suitable provider is found.</returns>
    /// <exception cref="ArgumentNullException">The value of 'formatProvider' cannot be null.</exception>
    /// <exception cref="NotSupportedException">Can't create instance from unknown IFormatProvider.</exception>
    public static CurrencyInfo GetInstance(IFormatProvider formatProvider) =>
        formatProvider switch
        {
            null => throw new ArgumentNullException(nameof(formatProvider)),
            CurrencyInfo currencyInfo => currencyInfo,
            CultureInfo cultureInfo => FromCulture(cultureInfo),
            NumberFormatInfo numberFormatInfo => FromNumberFormatInfo(numberFormatInfo),
            _ => throw new NotSupportedException($"Can't create instance from IFormatProvider {formatProvider.GetType()}!")
        };

    /// <summary>Retrieves a <see cref="CurrencyInfo"/> instance used within the specified <see cref="RegionInfo"/>.</summary>
    /// <param name="region"><see cref="RegionInfo"/> to get a <see cref="Currency"/> for.</param>
    /// <returns>The <see cref="CurrencyInfo"/> instance used within the specified <see cref="RegionInfo"/>.</returns>
    /// <exception cref="ArgumentNullException">The value of 'region' cannot be null.</exception>
    /// <exception cref="ArgumentException">The 'code' is an unknown ISO 4217 currency code.</exception>
    public static CurrencyInfo GetInstance(RegionInfo region) =>
        region != null ? FromCode(region.ISOCurrencySymbol) : throw new ArgumentNullException(nameof(region));

    /// <summary>Retrieves a <see cref="CurrencyInfo"/> instance based on the specified <see cref="Currency"/>.</summary>
    /// <param name="currency">The <see cref="Currency"/> for which the <see cref="CurrencyInfo"/> should be retrieved.</param>
    /// <returns>An instance of <see cref="CurrencyInfo"/> corresponding to the provided <see cref="Currency"/>.</returns>
    public static CurrencyInfo GetInstance(Currency currency) => CurrencyRegistry.Get(currency);

    /// <inheritdoc />
    public object? GetFormat(Type? formatType)
    {
        if (formatType == typeof(CurrencyInfo))
            return this;

        if (formatType == typeof(ICustomFormatter))
            return this;

        if (formatType == typeof(NumberFormatInfo))
            return ToNumberFormatInfo(formatProvider: null);

        return null;
    }

    /// <inheritdoc />
    public string Format(string? format, object? arg, IFormatProvider? formatProvider)
    {
        // styles: symbol, code, name, accounting

        // Supported formats: see https://learn.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings
        // G: General format = C but with currency code => ISO code with number, like EUR 23.002,43 , EUR 23,002.43, 23,002.43 EUR
        // C: Currency Symbol format, like € 23.002,43 , € 23,002.43, 23,002.43 €
        // C => TODO: if symbol is GenericCurrencySign, then use code? What if NoCurrency?
        // c => TODO: use C for international version (US$) and c for local version ($) in some locals?
        // R: Round-trip format with currency code
        // N: Number format = decimal
        // F: Fixed point format = decimal
        // L: English name, like 23.002,43 dollar
        // l: Native name, like 23.002,43 dólar
        // ?: Name in currency culture (needs Unicode CLDR https://cldr.unicode.org/)
        // ?: Accounting ($23.002,43) for negative numbers

        // If argument is not a Money, fallback to default formatting
        if (arg is not Money money)
        {
            return arg is IFormattable formattable
                ? formattable.ToString(format, formatProvider)
                : arg?.ToString() ?? string.Empty;
        }

        // TODO: short= $13B, $12.8B or long= $14 billion
        // TODO: CLDR-data: https://github.com/unicode-org/cldr-json/tree/main/cldr-json/cldr-numbers-full
        // For example USD in NL
        // "USD": {
        //     "displayName": "Amerikaanse dollar",
        //     "displayName-count-one": "Amerikaanse dollar",
        //     "displayName-count-other": "Amerikaanse dollar",
        //     "symbol": "US$",
        //     "symbol-alt-narrow": "$"
        // },
        // https://github.com/globalizejs/globalize/blob/master/doc/api/currency/currency-formatter.md

        // TODO: check if Money.Currency and this is equal, and also check IFormatProvider? What to do if not equal?
        // Always use CurrencyDecimalDigits and CurrencySymbol from CurrencyInfo, but override other properties?

        NumberFormatInfo nfi = ToNumberFormatInfo(formatProvider);
        char fmt = ParseFormatSpecifier(format.AsSpan(), out int digits);
        if (fmt == 0)
        {
            // custom format
            return money.Amount.ToString(format, nfi);
        }

        return fmt switch
        {
            // Currency formats
            'C' when digits == -1 => money.Amount.ToString("C", nfi),
            'C' or 'c' => // TODO: use C for international version (US$) and c for local version ($) in some locals
                money.Amount.ToString($"C{digits}", nfi),

            // General format (uses currency code as symbol)
            'G' or 'g' when digits == -1 => money.Amount.ToString("C", ToNumberFormatInfo(formatProvider, true)),
            'G' or 'g' => money.Amount.ToString($"C{digits}", ToNumberFormatInfo(formatProvider, true)),

            // English Name currency (e.g., "1234.56 US dollars") // TODO: future use lower-case for local native name
            'L' or 'l' when digits == -1 =>
                // N will use NumberDecimalDigits instead of CurrencyDecimalDigits.
                $"{money.Amount.ToString($"N{nfi.CurrencyDecimalDigits}", nfi)} {EnglishName}",
            'L' or 'l' => $"{money.Amount.ToString($"N{digits}", nfi)} {EnglishName}",

            // Round-trip format (e.g., "USD 1234.56")
            'R' or 'r' when digits == -1 => $"{Code} {money.Amount.ToString("R", nfi)}",
            'R' or 'r' => $"{Code} {money.Amount.ToString($"R{digits} ", nfi)}",

            _ => money.Amount.ToString(format, nfi)
        };
    }

    /// <summary>Deconstructs the current <see cref="CurrencyInfo" /> instance into its components.</summary>
    /// <param name="code">The three-character currency code (ISO-4217) of the current instance.</param>
    /// <param name="symbol">The currency symbol of the current instance.</param>
    /// <param name="number">The numeric currency code (ISO-4217) of the current instance.</param>
    public void Deconstruct(out string code, out string symbol, out short number)
    {
        code = Code;
        number = Number;
        symbol = Symbol;
    }

    /// <summary>Creates an instance of the <see cref="CurrencyInfo"/> used within the specified <see cref="CultureInfo"/>.</summary>
    /// <param name="culture"><see cref="CultureInfo"/> to get a <see cref="Currency"/> for.</param>
    /// <returns>The <see cref="CurrencyInfo"/> instance used within the specified <see cref="CultureInfo"/>.</returns>
    /// <exception cref="ArgumentNullException">The value of 'culture' cannot be null.</exception>
    /// <exception cref="ArgumentException">
    /// Culture is a neutral culture, from which no region information can be extracted -or-
    /// The 'code' is an unknown ISO 4217 currency code.
    /// </exception>
    private static CurrencyInfo FromCulture(CultureInfo culture)
    {
        if (culture == null)
            throw new ArgumentNullException(nameof(culture));
        if (Equals(culture, CultureInfo.InvariantCulture))
            throw new ArgumentException("Culture {0} is a invariant culture, from which no region information can be extracted!", culture.Name);

        return GetInstance(new RegionInfo(culture.Name));
    }

    private static CurrencyInfo FromNumberFormatInfo(NumberFormatInfo nfi)
    {
        if (nfi == null)
            throw new ArgumentNullException(nameof(nfi));
        if (string.IsNullOrEmpty(nfi.CurrencySymbol))
            throw new ArgumentException("Currency symbol in NumberFormatInfo is null or empty!", nameof(nfi));
        if (nfi.CurrencySymbol == GenericCurrencySign)
            throw new ArgumentException("Currency symbol in NumberFormatInfo is generic currency sign!", nameof(nfi));
        return Money.ParseCurrencyInfo(nfi.CurrencySymbol.AsSpan()); // throws FormatException if invalid
    }

    /// <summary>
    /// Converts the currency information into a <see cref="NumberFormatInfo"/> object, setting the currency formatting properties based
    /// on the current currency settings and optionally replacing the currency symbol with the currency code.
    /// </summary>
    /// <param name="formatProvider">An optional format provider to influence the number formatting. If null, the current culture's format provider is used.</param>
    /// <param name="useCurrencyCode">A boolean value indicating whether the currency symbol should be replaced with the currency code. Default is false.</param>
    /// <returns>A <see cref="NumberFormatInfo"/> instance configured with currency formatting properties specific to the current currency.</returns>
    private NumberFormatInfo ToNumberFormatInfo(IFormatProvider? formatProvider, bool useCurrencyCode = false)
    {
        NumberFormatInfo numberFormatInfo = formatProvider switch
        {
            CultureInfo ci => (NumberFormatInfo)ci.NumberFormat.Clone(),
            NumberFormatInfo nfi => (NumberFormatInfo)nfi.Clone(),
            _ => (NumberFormatInfo)CultureInfo.CurrentCulture.NumberFormat.Clone()
        };

        numberFormatInfo.CurrencyDecimalDigits = DecimalDigits;
        numberFormatInfo.CurrencySymbol = Symbol;

        // check if we need to replace with currency code
        if (!useCurrencyCode && Symbol != GenericCurrencySign) return numberFormatInfo;

        // Replace currency symbol with the code
        numberFormatInfo.CurrencySymbol = Code;

        // For PositivePattern add space between code and value
        numberFormatInfo.CurrencyPositivePattern = numberFormatInfo.CurrencyPositivePattern switch
        {
            0 => 2, // $n -> $ n
            1 => 3, // n$ -> n $
            _ => numberFormatInfo.CurrencyPositivePattern // No change needed
        };

        // For NegativePattern add space between code and value
        numberFormatInfo.CurrencyNegativePattern = numberFormatInfo.CurrencyNegativePattern switch
        {
            0 => 14, // ($n) -> ($ n)
            1 => 9, // -$n -> -$ n
            2 => 12, // $-n -> $ -n
            3 => 11, // $n- -> $ n-
            4 => 15, // (n$) -> (n $)
            5 => 8, // -n$ -> -n $
            6 => 13, // n-$ -> n- $
            7 => 10, // n$- -> n $-
            _ => numberFormatInfo.CurrencyNegativePattern // No change needed
        };

        return numberFormatInfo;
    }

    private static char ParseFormatSpecifier(ReadOnlySpan<char> format, out int digits)
    {
        char c = default;
        if (format.Length > 0)
        {
            // If the format begins with a symbol, see if it's a standard format
            // with or without a specified number of digits.
            c = format[0];
#if NET7_0_OR_GREATER
            if (char.IsAsciiLetter(c))
#else
            // char.IsAsciiLetter(c) => (uint)((c | 0x20) - 'a') <= 'z' - 'a'
            if ((uint)((c | 0x20) - 'a') <= 'z' - 'a')
#endif
            {
                // Fast path for sole symbol, e.g. "D"
                if (format.Length == 1)
                {
                    digits = -1;
                    return c;
                }

                if (format.Length == 2)
                {
                    // Fast path for symbol and single digit, e.g. "X4"
                    int d = format[1] - '0';
                    if ((uint)d < 10)
                    {
                        digits = d;
                        return c;
                    }
                }
                else if (format.Length == 3)
                {
                    // Fast path for symbol and double-digit, e.g. "F12"
                    int d1 = format[1] - '0', d2 = format[2] - '0';
                    if ((uint)d1 < 10 && (uint)d2 < 10)
                    {
                        digits = (d1 * 10) + d2;
                        return c;
                    }
                }

                // Fallback for symbol and any length digits.  The digits value must be >= 0 && <= 999_999_999,
                // but it can begin with any number of 0s, and thus we may need to check more than 9
                // digits.  Further, for compat, we need to stop when we hit a null char.
                int n = 0;
                int i = 1;
                while ((uint)i < (uint)format.Length &&
#if NET7_0_OR_GREATER
                        char.IsAsciiDigit(format[i])
#else
                        // char.IsAsciiDigit(format[i])) => char.IsBetween(format[i], '0', '9')) => (uint)(c - minInclusive) <= (uint)(maxInclusive - minInclusive)
                        (uint)(format[i] - '0') <= (uint)('9' - '0')
#endif
                      )
                {
                    // Check if we are about to overflow past our limit of 9 digits
                    if (n >= 100_000_000)
                    {
                        throw new FormatException("format is invalid!");
                    }

                    n = (n * 10) + format[i++] - '0';
                }

                // If we're at the end of the digits rather than having stopped because we hit something
                // other than a digit or overflowed, return the standard format info.
                if ((uint)i >= (uint)format.Length || format[i] == '\0')
                {
                    digits = n;
                    return c;
                }
            }
        }

        // Default empty format to be "C"; custom format is signified with '\0'.
        digits = -1;
        return format.Length == 0 || c == '\0'
            ? // For compat, treat '\0' as the end of the specifier, even if the specifier extends beyond it.
            'C'
            : '\0';
    }

    private static void ValidateCurrencyCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentNullException(nameof(code));

        if (code.Length != 3)
            throw new ArgumentException(InvalidCurrencyMessage, nameof(code));

        // Only capital letters
        foreach (var c in code)
        {
            if (c is < 'A' or > 'Z')
                throw new ArgumentException(InvalidCurrencyMessage, nameof(code));
        }
    }

    private static void ValidateCurrencyNumber(short number, bool isIso4217)
    {
        if (number < 0)
        {
            throw new ArgumentException($"The number code '{number}' must be greater than 0!");
        }

        switch (isIso4217)
        {
            case true when (number is < 1 or > 999):
                throw new ArgumentException($"For ISO 4217 the number code '{number}' must be between 1 and 999!");
            case false when (number is >= 1 and <= 999):
                throw new ArgumentException($"For non ISO 4217 the number code '{number}' must be 0 or greater than 999!");
        }
    }
}
