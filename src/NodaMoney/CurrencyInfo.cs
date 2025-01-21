using System.Globalization;

namespace NodaMoney;

// TODO: Add support for the following properties:
// Thread.CurrentThread.CurrentCulture = new CurrentInfo("")
// Thread.CurrentThread.CurrentCurrency = new CurrencyInfo("");
//
// CultureInfo.DefaultThreadCurrentCurrency = CurrencyInfo;
//
// var x = CurrencyInfo.CurrentCurrency;
//
// IsObsolete, IsDeprecated
// ReplacedBy, Replaces, AlternativeSymbol, Locals (NL, EN),
// Order/Priority/Weight=1,0.8,0.6 = q-factor weighting (value between 0 and 1) = Any value placed in an order of preference expressed using a relative quality value called weight.

// TODO: Should we use internal sealed class CurrencyData to store the data? CultureInfo has a similar structure called
// CultureData. https://github.com/dotnet/runtime/blob/main/src/libraries/System.Private.CoreLib/src/System/Globalization/CultureData.cs

/// <summary>A unit of exchange of value, a currency of <see cref="Money" />.</summary>
/// <remarks>See http://en.wikipedia.org/wiki/Currency and
/// https://en.wikipedia.org/wiki/List_of_circulating_currencies and
/// https://www.six-group.com/en/products-services/financial-information/data-standards.html#scrollTo=isin</remarks>
public record CurrencyInfo : IFormatProvider
{
    /// <summary>A unit of exchange of value, a currency of <see cref="Money" />.</summary>
    /// <remarks>See http://en.wikipedia.org/wiki/Currency and
    /// https://en.wikipedia.org/wiki/List_of_circulating_currencies and
    /// https://www.six-group.com/en/products-services/financial-information/data-standards.html#scrollTo=isin</remarks>
    /// <param name="Code">The (ISO-4217) three-character code of the currency.</param>
    /// <param name="Number">The (ISO-4217) number of the currency.</param>
    /// <param name="MinorUnit">The minor unit, as an exponent of base 10, by which the currency unit can be divided in.</param>
    /// <param name="EnglishName">The english name of the currency</param>
    /// <param name="Symbol">The currency symbol.</param>
    public CurrencyInfo(string Code, short Number, MinorUnit MinorUnit, string EnglishName = "", string Symbol = CurrencyInfo.GenericCurrencySign)
    {
        this.Code = Code ?? throw new ArgumentNullException(nameof(Code));
        this.Number = Number;
        this.MinorUnit = MinorUnit;
        this.EnglishName = EnglishName ?? string.Empty;
        this.Symbol = Symbol ?? CurrencyInfo.GenericCurrencySign;
    }

    /// <summary>Gets the currency sign (¤), a character used to denote the generic currency sign, when no currency sign is available.</summary>
    /// <remarks>See https://en.wikipedia.org/wiki/Currency_sign_(typography). </remarks>
    public const string GenericCurrencySign = "¤";

    public static readonly CurrencyInfo NoCurrency = new("XXX", 999, MinorUnit.NotApplicable, "No Currency");

    [ThreadStatic] static CurrencyInfo? s_currentThreadCurrency;

    // static CurrencyInfo()
    // {
    //     s_currentThreadCurrency = NoCurrency;
    // }

    //public Currency CurrencyUnit => new(Code);

    public static implicit operator Currency(CurrencyInfo currency) => new(currency.Code);

    /// <summary>Get the (ISO-4217) three-character code of the currency.</summary>
    //public string Code { get; init; } = Code;

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
    // public string FractionalUnit { get; init; } = "Cent";

    /// <summary>Gets a value indicating whether currency is historic.</summary>
    /// <value><c>true</c> if this instance is valid; otherwise, <c>false</c>.</value>
    public bool IsHistoric => !IsActiveOn(DateTime.Today);

    /// <summary>The (ISO-4217) three-character code of the currency.</summary>
    public string Code { get; init; }

    /// <summary>The (ISO-4217) number of the currency.</summary>
    public short Number { get; init; }

    /// <summary>The minor unit, as an exponent of base 10, by which the currency unit can be divided in.</summary>
    public MinorUnit MinorUnit { get; init; }

    /// <summary>The english name of the currency</summary>
    public string EnglishName { get; init; }

    /// <summary>The currency symbol.</summary>
    public string Symbol { get; init; }

    /// <summary>Check a value indication whether currency is valid on a given date.</summary>
    /// <param name="date">The date on which the Currency should be valid.</param>
    /// <returns><c>true</c> when the date is within the valid range of this currency; otherwise <c>false</c>.</returns>
    public bool IsActiveOn(DateTime date) =>
        (!IntroducedOn.HasValue || IntroducedOn <= date) &&
        (!ExpiredOn.HasValue || ExpiredOn >= date);

    /// <summary>Gets the Currency that represents the country/region used by the current thread.</summary>
    /// <value>The Currency that represents the country/region used by the current thread.</value>
    public static CurrencyInfo CurrentCurrency
    {
        get
        {
#if NET5_0_OR_GREATER
            // In >= .NET5 when CurrentCulture is Invariant, then RegionInfo.CurrentRegion is retrieved from
            // Windows settings. See also https://github.com/xunit/samples.xunit/pull/18
            var currentCulture = CultureInfo.CurrentCulture;
            if (Equals(currentCulture, CultureInfo.InvariantCulture)) // no region information can be extracted
            {
                return NoCurrency;
            }

            return FromCulture(currentCulture);
#else
            var currentRegion = RegionInfo.CurrentRegion;
            return currentRegion.Name == "IV" ? NoCurrency : FromRegion(currentRegion);
#endif
        }
    }

    /// <summary>Get all currencies.</summary>
    /// <returns>An <see cref="IEnumerable{CurrencyInfo}"/> of all registered currencies.</returns>
    public static IEnumerable<CurrencyInfo> GetAllCurrencies() => CurrencyRegistry.GetAllCurrencies();

    public static CurrencyInfo FromCode(string code) => CurrencyRegistry.Get(code);

    public static CurrencyInfo FromCurrencyUnit(Currency currency) => CurrencyRegistry.Get(currency);

    /// <summary>Creates an instance of the <see cref="CurrencyInfo"/> used within the specified <see cref="RegionInfo"/>.</summary>
    /// <param name="region"><see cref="RegionInfo"/> to get a <see cref="Currency"/> for.</param>
    /// <returns>The <see cref="CurrencyInfo"/> instance used within the specified <see cref="RegionInfo"/>.</returns>
    /// <exception cref="ArgumentNullException">The value of 'region' cannot be null.</exception>
    /// <exception cref="ArgumentException">The 'code' is an unknown ISO 4217 currency code.</exception>
    public static CurrencyInfo FromRegion(RegionInfo region)
    {
        if (region == null)
            throw new ArgumentNullException(nameof(region));

        return FromCode(region.ISOCurrencySymbol);
    }

    /// <summary>Creates an instance of the <see cref="CurrencyInfo"/> used within the specified name of the region or culture.</summary>
    /// <param name="name">
    /// <para>A string that contains a two-letter code defined in ISO 3166 for country/region.</para>
    /// <para>-or-</para>
    /// <para>A string that contains the culture name for a specific culture, custom culture, or Windows-only culture. If the
    /// culture name is not in RFC 4646 format, your application should specify the entire culture name instead of just the
    /// country/region. See also <seealso cref="System.Globalization.RegionInfo(string)"/>.</para>
    /// </param>
    /// <returns>The <see cref="CurrencyInfo"/> instance used within the specified region.</returns>
    /// <exception cref="ArgumentNullException">The value of 'name' cannot be null.</exception>
    public static CurrencyInfo FromRegion(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentNullException(nameof(name));

        return FromRegion(new RegionInfo(name));
    }

    /// <summary>Creates an instance of the <see cref="CurrencyInfo"/> used within the specified <see cref="CultureInfo"/>.</summary>
    /// <param name="culture"><see cref="CultureInfo"/> to get a <see cref="Currency"/> for.</param>
    /// <returns>The <see cref="CurrencyInfo"/> instance used within the specified <see cref="CultureInfo"/>.</returns>
    /// <exception cref="ArgumentNullException">The value of 'culture' cannot be null.</exception>
    /// <exception cref="ArgumentException">
    /// Culture is a neutral culture, from which no region information can be extracted -or-
    /// The 'code' is an unknown ISO 4217 currency code.
    /// </exception>
    public static CurrencyInfo FromCulture(CultureInfo culture)
    {
        if (culture == null)
            throw new ArgumentNullException(nameof(culture));
        if (Equals(culture, CultureInfo.InvariantCulture))
            throw new ArgumentException("Culture {0} is a invariant culture, from which no region information can be extracted!", culture.Name);

        return FromRegion(culture.Name);
    }

    public void Deconstruct(out string Code, out short Number, out MinorUnit MinorUnit, out string EnglishName, out string Symbol)
    {
        Code = this.Code;
        Number = this.Number;
        MinorUnit = this.MinorUnit;
        EnglishName = this.EnglishName;
        Symbol = this.Symbol;
    }

    public static CurrencyInfo GetInstance(IFormatProvider? formatProvider)
    {
        if (formatProvider == null)
            return CurrentCurrency;

        if (formatProvider is CurrencyInfo currencyInfo)
            return currencyInfo;

        if (formatProvider is CultureInfo cultureInfo)
            return FromCulture(cultureInfo);

        if (formatProvider is NumberFormatInfo)
            throw new NotImplementedException();

        return CurrentCurrency;

        // return formatProvider == null ?
        //     CurrentInfo : // Fast path for a null provider
        //     GetProviderNonNull(formatProvider);
        //
        // static NumberFormatInfo GetProviderNonNull(IFormatProvider provider)
        // {
        //     // Fast path for a regular CultureInfo
        //     if (provider is CultureInfo cultureProvider && !cultureProvider._isInherited)
        //     {
        //         return cultureProvider._numInfo ?? cultureProvider.NumberFormat;
        //     }
        //
        //     return
        //         provider as NumberFormatInfo ?? // Fast path for an NFI
        //         provider.GetFormat(typeof(NumberFormatInfo)) as NumberFormatInfo ??
        //         CurrentInfo;
        // }
    }

    /// <inheritdoc />
    public object? GetFormat(Type? formatType)
    {
        if (formatType == typeof(CurrencyInfo))
            return this;

        if (formatType == typeof(NumberFormatInfo))
        {
            return this.ToNumberFormatInfo(formatProvider: null);
        }

        return null;
    }

    public NumberFormatInfo ToNumberFormatInfo(IFormatProvider? formatProvider, bool useISOCurrencySymbol = false)
    {
        NumberFormatInfo numberFormatInfo = (NumberFormatInfo)CultureInfo.CurrentCulture.NumberFormat.Clone();
        if (formatProvider != null)
        {
            if (formatProvider is CultureInfo ci)
                numberFormatInfo = (NumberFormatInfo)ci.NumberFormat.Clone();

            if (formatProvider is NumberFormatInfo nfi)
                numberFormatInfo = (NumberFormatInfo)nfi.Clone();
        }

        numberFormatInfo.CurrencyDecimalDigits = this.DecimalDigits;
        numberFormatInfo.CurrencySymbol = this.Symbol;

        if (!useISOCurrencySymbol) return numberFormatInfo;

        // Replace symbol with the code
        numberFormatInfo.CurrencySymbol = this.Code;

        // For PositivePattern and NegativePattern add space between code and value
        if (numberFormatInfo.CurrencyPositivePattern == 0) // $n
            numberFormatInfo.CurrencyPositivePattern = 2; // $ n
        if (numberFormatInfo.CurrencyPositivePattern == 1) // n$
            numberFormatInfo.CurrencyPositivePattern = 3; // n $

        switch (numberFormatInfo.CurrencyNegativePattern)
        {
            case 0: // ($n)
                numberFormatInfo.CurrencyNegativePattern = 14; // ($ n)
                break;
            case 1: // -$n
                numberFormatInfo.CurrencyNegativePattern = 9; // -$ n
                break;
            case 2: // $-n
                numberFormatInfo.CurrencyNegativePattern = 12; // $ -n
                break;
            case 3: // $n-
                numberFormatInfo.CurrencyNegativePattern = 11; // $ n-
                break;
            case 4: // (n$)
                numberFormatInfo.CurrencyNegativePattern = 15; // (n $)
                break;
            case 5: // -n$
                numberFormatInfo.CurrencyNegativePattern = 8; // -n $
                break;
            case 6: // n-$
                numberFormatInfo.CurrencyNegativePattern = 13; // n- $
                break;
            case 7: // n$-
                numberFormatInfo.CurrencyNegativePattern = 10; // n $-
                break;
        }

        return numberFormatInfo;
    }
}
