using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
#if NET8_0_OR_GREATER
using System.Collections.Frozen;
#endif

namespace NodaMoney;

// TODO: allow duplicates with priority? (e.g. non ISO-4217 currencies). For now we don't allow.

/// <summary>Represent the central thread-safe registry for currencies.</summary>
static class CurrencyRegistry
{
#if NET8_0_OR_GREATER // In .NET 8 or higher, we use FrozenDictionary for optimal immutability and performance
    static readonly object s_lock = new();
    static FrozenDictionary<Currency, CurrencyInfo> s_lookupByCurrency;
    static FrozenDictionary<string, CurrencyInfo> s_lookupByCode;
#else // In .NET Standard 2.0, we use Dictionary with ReaderWriterLockSlim for thread safety
    static readonly ReaderWriterLockSlim s_lockSlim = new();
    static readonly Dictionary<Currency, CurrencyInfo> s_lookupByCurrency;
    static readonly Dictionary<string, CurrencyInfo> s_lookupByCode;
#endif
    static ILookup<string, CurrencyInfo> s_lookupByCodeAndSymbol;

    static CurrencyRegistry()
    {
        CurrencyInfo[] currencies = InitializeCurrencies();
#if NET8_0_OR_GREATER
        s_lookupByCode = currencies.ToFrozenDictionary(ci => ci.Code, ci => ci);
        s_lookupByCurrency = currencies.ToFrozenDictionary(ci => (Currency)ci, ci => ci);
#else
        s_lookupByCode = currencies.ToDictionary(ci => ci.Code, ci => ci);
        s_lookupByCurrency = currencies.ToDictionary(ci => (Currency)ci, ci => ci);
#endif
        s_lookupByCodeAndSymbol = CreateLookupByCodeAndSymbol();
    }

    /// <summary>Gets the <see cref="CurrencyInfo"/> associated with the specified currency code.</summary>
    /// <param name="code">The currency code to retrieve, such as EUR or USD.</param>
    /// <returns>The <see cref="CurrencyInfo"/> associated with the specified currency code.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the specified <paramref name="code"/> is null.</exception>
    /// <exception cref="InvalidCurrencyException">Thrown when no currency is found with the specified <paramref name="code"/>.</exception>
    public static CurrencyInfo Get(string code)
    {
        if (code is null) throw new ArgumentNullException(nameof(code));

        if (TryGet(code, out CurrencyInfo? currencyInfo))
        {
            return currencyInfo;
        }

        throw new InvalidCurrencyException($"{code} is unknown currency code!");
    }

    /// <summary>Tries to get the <see cref="CurrencyInfo"/> with the specified code.</summary>
    /// <param name="code">A currency code, like EUR or USD.</param>
    /// <param name="currencyInfo">When this method returns, contains the currency information that has the specified currency code if the code is found; otherwise, null.</param>
    /// <returns><b>true</b> if the <see cref="CurrencyRegistry"/> contains a currency with the specified code; otherwise, <b>false</b>.</returns>
#if NETSTANDARD2_0
    public static bool TryGet(string code, out CurrencyInfo currencyInfo)
#else
    public static bool TryGet(string code, [MaybeNullWhen(false)] out CurrencyInfo currencyInfo)
#endif
    {
        if (string.IsNullOrEmpty(code))
        {
            currencyInfo = null!;
            return false;
        }

#if NET8_0_OR_GREATER
        return s_lookupByCode.TryGetValue(code, out currencyInfo);
#else
        s_lockSlim.EnterReadLock();
        try
        {
            return s_lookupByCode.TryGetValue(code, out currencyInfo);
        }
        finally
        {
            s_lockSlim.ExitReadLock();
        }
#endif
    }

    /// <summary>Tries the get <see cref="CurrencyInfo"/> of the given <see cref="Currency"/>.</summary>
    /// <param name="currency">A currency, like EUR or USD.</param>
    /// <returns><b>true</b> if <see cref="CurrencyRegistry"/> contains a <see cref="CurrencyInfo"/> with the specified code; otherwise, <b>false</b>.</returns>
    /// <exception cref="System.ArgumentNullException">The value of 'code' cannot be null or empty.</exception>
    /// <exception cref="InvalidCurrencyException"> when <see cref="currency"/> is unknown currency.</exception>
    public static CurrencyInfo Get(Currency currency)
    {
#if NET8_0_OR_GREATER
        if (s_lookupByCurrency.TryGetValue(currency, out var ci))
            return ci;
#else
        s_lockSlim.EnterReadLock();
        try
        {
            if (s_lookupByCurrency.TryGetValue(currency, out var ci))
                return ci;
        }
        finally
        {
            s_lockSlim.ExitReadLock();
        }
#endif
        throw new InvalidCurrencyException($"{currency} is unknown currency code!");
    }

    /// <summary>Get all registered currencies.</summary>
    /// <returns>An <see cref="IReadOnlyList{CurrencyInfo}"/> of all registered currencies.</returns>
    public static IReadOnlyList<CurrencyInfo> GetAllCurrencies()
    {
#if NET8_0_OR_GREATER
        return s_lookupByCode.Values.AsReadOnly();
#else
        s_lockSlim.EnterReadLock();
        try
        {
            return s_lookupByCode.Values.ToList().AsReadOnly();
        }
        finally
        {
            s_lockSlim.ExitReadLock();
        }
#endif
    }

    /// <summary>Get all registered currencies that match the given Currency Code or Symbol.</summary>
    /// <param name="currencyChars">The Currency Code or Symbol to match.</param>
    /// <returns>An <see cref="IReadOnlyList{CurrencyInfo}"/> of all registered currencies that matches.</returns>
    public static IReadOnlyList<CurrencyInfo> GetAllCurrencies(ReadOnlySpan<char> currencyChars)
    {
#if NET8_0_OR_GREATER
        return [.. s_lookupByCodeAndSymbol[currencyChars.ToString()]];
#else
        s_lockSlim.EnterReadLock();
        try
        {
            return [.. s_lookupByCodeAndSymbol[currencyChars.ToString()]];
        }
        finally
        {
            s_lockSlim.ExitReadLock();
        }
#endif
    }

    /// <summary>Attempts to add the <see cref="CurrencyInfo"/> to the register.</summary>
    /// <param name="currency">When this method returns, contains the <see cref="CurrencyInfo"/> that has the specified code and namespace, or the default value of the type if the operation failed.</param>
    /// <returns><b>true</b> if the <see cref="Currency"/> with the specified code is added; otherwise, <b>false</b>.</returns>
    /// <exception cref="System.ArgumentNullException">The value of 'code' or 'namespace' cannot be null or empty.</exception>
    public static bool TryAdd(CurrencyInfo currency)
    {
#if NET8_0_OR_GREATER
        lock (s_lock)
        {
            if (s_lookupByCode.ContainsKey(currency.Code))
                return false;

            var mutableDictionary = s_lookupByCode.ToDictionary();
            mutableDictionary[currency.Code] = currency;

            s_lookupByCode = mutableDictionary.ToFrozenDictionary();
            s_lookupByCurrency = mutableDictionary.ToFrozenDictionary(pair => (Currency)pair.Value, pair => pair.Value);
            s_lookupByCodeAndSymbol = CreateLookupByCodeAndSymbol();

            return true;
        }
#else
        s_lockSlim.EnterWriteLock();
        try
        {
            if (s_lookupByCode.ContainsKey(currency.Code))
                return false;

            s_lookupByCode[currency.Code] = currency;
            s_lookupByCurrency[(Currency)currency] = currency;
            s_lookupByCodeAndSymbol = CreateLookupByCodeAndSymbol();

            return true;
        }
        finally
        {
            s_lockSlim.ExitWriteLock();
        }
#endif
    }

    /// <summary>Attempts to remove the <see cref="CurrencyInfo"/> from the registry.</summary>
    /// <param name="currency">The <see cref="CurrencyInfo"/> to be removed.</param>
    /// <returns><b>true</b> if the <see cref="CurrencyInfo"/> with the specified code is removed; otherwise, <b>false</b>.</returns>
    /// <exception cref="System.ArgumentNullException">The value of 'code' or 'namespace' cannot be null or empty.</exception>
    public static bool TryRemove(CurrencyInfo currency)
    {
#if NET8_0_OR_GREATER
        lock (s_lock)
        {
            var mutableDictionary = s_lookupByCode.ToDictionary();
            if (!mutableDictionary.Remove(currency.Code))
                return false;

            s_lookupByCode = mutableDictionary.ToFrozenDictionary();
            s_lookupByCurrency = mutableDictionary.ToFrozenDictionary(pair => (Currency)pair.Value, pair => pair.Value);
            s_lookupByCodeAndSymbol = CreateLookupByCodeAndSymbol();

            return true;
        }
#else
        s_lockSlim.EnterWriteLock();
        try
        {
            if (!s_lookupByCode.Remove(currency.Code))
                return false;

            s_lookupByCurrency.Remove((Currency)currency);
            s_lookupByCodeAndSymbol = CreateLookupByCodeAndSymbol();

            return true;
        }
        finally
        {
            s_lockSlim.ExitWriteLock();
        }
#endif
    }

    static ILookup<string, CurrencyInfo> CreateLookupByCodeAndSymbol() =>
        s_lookupByCode.Values
            .SelectMany(currency =>
                new[]
                    {
                        new { Key = currency.Code, Currency = currency }, new { Key = currency.Symbol, Currency = currency },
                        new { Key = currency.InternationalSymbol, Currency = currency },
                    }
                    .Concat(currency.AlternativeSymbols.Select(symbol => new { Key = symbol, Currency = currency }))
                    .Distinct()) // Needed because Code, Symbol and InternationalSymbol can be the same
            .ToLookup(item => item.Key, item => item.Currency);

    // TODO: Move to resource file?
    static CurrencyInfo[] InitializeCurrencies() =>
        [
            // ISO-4217 currencies (list one)
            new ("AED", 784, MinorUnit.Two, "United Arab Emirates dirham", "د.إ"),
            new ("AFN", 971, MinorUnit.Two, "Afghan afghani", "؋"),
            new ("ALL", 008, MinorUnit.Two, "Albanian lek", "L"),
            new ("AMD", 051, MinorUnit.Two, "Armenian dram", "֏") { AlternativeSymbols = ["dram"] },
            new ("AOA", 973, MinorUnit.Two, "Angolan kwanza", "Kz"),
            new ("ARS", 032, MinorUnit.Two, "Argentine peso", "$") { InternationalSymbol = "ARS" },
            new ("AUD", 036, MinorUnit.Two, "Australian dollar", "$") { InternationalSymbol = "A$" },
            new ("AWG", 533, MinorUnit.Two, "Aruban florin", "ƒ"),
            new ("AZN", 944, MinorUnit.Two, "Azerbaijan Manat", "ман"), // AZERBAIJAN
            new ("BAM", 977, MinorUnit.Two, "Bosnia and Herzegovina convertible mark", "KM"),
            new ("BBD", 052, MinorUnit.Two, "Barbados dollar", "$") { InternationalSymbol = "BBD$", AlternativeSymbols = ["Bds$", "BB$", "BDS$"] },
            new ("BDT", 050, MinorUnit.Two, "Bangladeshi taka", "৳") { InternationalSymbol = "Tk" },
            new ("BHD", 048, MinorUnit.Three, "Bahraini dinar", "BD"), // or د.ب. (switched for unit tests to work)
            new ("BIF", 108, MinorUnit.Zero, "Burundian franc", "FBu"),
            new ("BMD", 060, MinorUnit.Two, "Bermudian dollar", "$") { InternationalSymbol = "BD$" },
            new ("BND", 096, MinorUnit.Two, "Brunei dollar", "$") { InternationalSymbol = "BND$", AlternativeSymbols = ["B$"] },
            new ("BOB", 068, MinorUnit.Two, "Boliviano", "Bs."), // or BS or $b
            new ("BOV", 984, MinorUnit.Two, "Bolivian Mvdol (funds code)"), // <==== not found symbol. Is generic currency sign correct?
            new ("BRL", 986, MinorUnit.Two, "Brazilian real", "R$"),
            new ("BSD", 044, MinorUnit.Two, "Bahamian dollar", "$") { InternationalSymbol = "BSD$", AlternativeSymbols = ["B$"] },
            new ("BTN", 064, MinorUnit.Two, "Bhutanese ngultrum", "Nu."),
            new ("BWP", 072, MinorUnit.Two, "Botswana pula", "P"),
            new ("BYN", 933, MinorUnit.Two, "Belarusian ruble", "Br") { IntroducedOn = new DateTime(2006, 06, 01) },
            new ("BZD", 084, MinorUnit.Two, "Belize dollar", "BZ$"),
            new ("CAD", 124, MinorUnit.Two, "Canadian dollar", "$") { InternationalSymbol = "CA$", AlternativeSymbols = ["C$"] },
            new ("CDF", 976, MinorUnit.Two, "Congolese franc", "FC"),
            new ("CHE", 947, MinorUnit.Two, "WIR Euro (complementary currency)", "CHE"),
            new ("CHF", 756, MinorUnit.Two, "Swiss franc", "Fr.") { InternationalSymbol = "CHF", AlternativeSymbols = ["fr.", "SFr."] }, // outdated "SFr."
            new ("CHW", 948, MinorUnit.Two, "WIR Franc (complementary currency)", "CHW"),
            new ("CLF", 990, MinorUnit.Four, "Unidad de Fomento (funds code)", "CLF"),
            new ("CLP", 152, MinorUnit.Zero, "Chilean peso", "$") { InternationalSymbol = "CLP$" },
            new ("CNY", 156, MinorUnit.Two, "Chinese yuan", "¥"),
            new ("COP", 170, MinorUnit.Two, "Colombian peso", "$") { InternationalSymbol = "COP$", AlternativeSymbols = ["Col$"] },
            new ("COU", 970, MinorUnit.Two, "Unidad de Valor Real", CurrencyInfo.GenericCurrencySign), // ???
            new ("CRC", 188, MinorUnit.Two, "Costa Rican colon", "₡"),
            new ("CUP", 192, MinorUnit.Two, "Cuban peso", "$") { InternationalSymbol = "CUP$", AlternativeSymbols = ["₱"] },
            new ("CVE", 132, MinorUnit.Two, "Cape Verde escudo", "$") { InternationalSymbol = "CVE", AlternativeSymbols = ["Esc"] },
            new ("CZK", 203, MinorUnit.Two, "Czech koruna", "Kč"),
            new ("DJF", 262, MinorUnit.Zero, "Djiboutian franc", "Fdj"),
            new ("DKK", 208, MinorUnit.Two, "Danish krone", "kr."),
            new ("DOP", 214, MinorUnit.Two, "Dominican peso", "$") { InternationalSymbol = "RD$" },
            new ("DZD", 012, MinorUnit.Two, "Algerian dinar", "DA"), // (Latin) or د.ج (Arabic)
            new ("EGP", 818, MinorUnit.Two, "Egyptian pound", "LE"), // or E£ or ج.م (Arabic)
            new ("ERN", 232, MinorUnit.Two, "Eritrean nakfa", "ERN"),
            new ("ETB", 230, MinorUnit.Two, "Ethiopian birr", "Br"), // (Latin) or ብር (Ethiopic)
            new ("EUR", 978, MinorUnit.Two, "Euro", "€"),
            new ("FJD", 242, MinorUnit.Two, "Fiji dollar", "$") { InternationalSymbol = "FJ$" },
            new ("FKP", 238, MinorUnit.Two, "Falkland Islands pound", "£"),
            new ("GBP", 826, MinorUnit.Two, "Pound sterling", "£"),
            new ("GEL", 981, MinorUnit.Two, "Georgian lari", "ლ."), // TODO: new symbol since July 18, 2014 => see http://en.wikipedia.org/wiki/Georgian_lari
            new ("GHS", 936, MinorUnit.Two, "Ghanaian cedi", "GH₵") { AlternativeSymbols = ["GH¢"] },
            new ("GIP", 292, MinorUnit.Two, "Gibraltar pound", "£"),
            new ("GMD", 270, MinorUnit.Two, "Gambian dalasi", "D"),
            new ("GNF", 324, MinorUnit.Zero, "Guinean Franc", "FG") { AlternativeSymbols = ["Fr", "GFr"] }, // GUINEA
            new ("GTQ", 320, MinorUnit.Two, "Guatemalan quetzal", "Q"),
            new ("GYD", 328, MinorUnit.Two, "Guyanese dollar", "$") { InternationalSymbol = "G$", AlternativeSymbols = ["GY$"] },
            new ("HKD", 344, MinorUnit.Two, "Hong Kong dollar", "$") { InternationalSymbol = "HK$" },
            new ("HNL", 340, MinorUnit.Two, "Honduran lempira", "L"),
            new ("HTG", 332, MinorUnit.Two, "Haitian gourde", "G"),
            new ("HUF", 348, MinorUnit.Two, "Hungarian forint", "Ft"),
            new ("IDR", 360, MinorUnit.Two, "Indonesian rupiah", "Rp"),
            new ("ILS", 376, MinorUnit.Two, "Israeli new shekel", "₪"),
            new ("INR", 356, MinorUnit.Two, "Indian rupee", "₹"),
            new ("IQD", 368, MinorUnit.Three, "Iraqi dinar", "د.ع"),
            new ("IRR", 364, MinorUnit.Two, "Iranian rial", "ريال"),
            new ("ISK", 352, MinorUnit.Zero, "Icelandic króna", "kr"),
            new ("JMD", 388, MinorUnit.Two, "Jamaican dollar", "$") { InternationalSymbol = "J$" },
            new ("JOD", 400, MinorUnit.Three, "Jordanian dinar", "د.ا.‏"),
            new ("JPY", 392, MinorUnit.Zero, "Japanese yen", "¥"),
            new ("KES", 404, MinorUnit.Two, "Kenyan shilling", "KSh"),
            new ("KGS", 417, MinorUnit.Two, "Kyrgyzstani som", "сом"),
            new ("KHR", 116, MinorUnit.Two, "Cambodian riel", "៛"),
            new ("KMF", 174, MinorUnit.Zero, "Comorian Franc", "CF"), // COMOROS (THE)
            new ("KPW", 408, MinorUnit.Two, "North Korean won", "₩"),
            new ("KRW", 410, MinorUnit.Zero, "South Korean won", "₩"),
            new ("KWD", 414, MinorUnit.Three, "Kuwaiti dinar", "د.ك") { AlternativeSymbols = ["KD"] },
            new ("KYD", 136, MinorUnit.Two, "Cayman Islands dollar", "$"),
            new ("KZT", 398, MinorUnit.Two, "Kazakhstani tenge", "₸"),
            new ("LAK", 418, MinorUnit.Two, "Lao Kip", "₭") { AlternativeSymbols = ["₭N"] }, // LAO PEOPLE’S DEMOCRATIC REPUBLIC(THE), ISO says minor unit=2 but wiki says Historically, one kip was divided into 100 att (ອັດ).
            new ("LBP", 422, MinorUnit.Two, "Lebanese pound", "ل.ل"),
            new ("LKR", 144, MinorUnit.Two, "Sri Lankan rupee", "Rs") { AlternativeSymbols = ["රු", "௹"] },
            new ("LRD", 430, MinorUnit.Two, "Liberian dollar", "$") { InternationalSymbol = "L$", AlternativeSymbols = ["LD$"] },
            new ("LSL", 426, MinorUnit.Two, "Lesotho loti", "L") { AlternativeSymbols = ["M"] }, // M is for plural
            new ("LYD", 434, MinorUnit.Three, "Libyan dinar", "ل.د") { AlternativeSymbols = ["LD"] },
            new ("MAD", 504, MinorUnit.Two, "Moroccan dirham", "د.م."),
            new ("MDL", 498, MinorUnit.Two, "Moldovan leu", "L"),
            new ("MGA", 969, MinorUnit.OneFifth, "Malagasy ariary", "Ar"), // divided into five subunits rather than by a power of ten. 5 is 10 to the power of 0.69897...
            new ("MKD", 807, MinorUnit.Two, "Macedonian denar", "ден"),
            new ("MMK", 104, MinorUnit.Two, "Myanma kyat", "K"),
            new ("MNT", 496, MinorUnit.Two, "Mongolian tugrik", "₮"),
            new ("MOP", 446, MinorUnit.Two, "Macanese pataca", "MOP$"),
            new ("MRU", 929, MinorUnit.OneFifth, "Mauritanian ouguiya", "UM") { IntroducedOn = new DateTime(2018, 01, 01) }, // divided into five subunits rather than by a power of ten. 5 is 10 to the power of 0.69897...
            new ("MUR", 480, MinorUnit.Two, "Mauritian rupee", "Rs"),
            new ("MVR", 462, MinorUnit.Two, "Maldivian rufiyaa", "Rf") { AlternativeSymbols = ["ރ"]},
            new ("MWK", 454, MinorUnit.Two, "Malawi kwacha", "MK"),
            new ("MXN", 484, MinorUnit.Two, "Mexican peso", "$") { InternationalSymbol = "Mex$", AlternativeSymbols = ["MX$"] },
            new ("MXV", 979, MinorUnit.Two, "Mexican Unidad de Inversion (UDI) (funds code)"),  // <==== not found
            new ("MYR", 458, MinorUnit.Two, "Malaysian ringgit", "RM"),
            new ("MZN", 943, MinorUnit.Two, "Mozambican metical", "MT") { AlternativeSymbols = ["MTn"]},
            new ("NAD", 516, MinorUnit.Two, "Namibian dollar", "$") { InternationalSymbol = "N$" },
            new ("NGN", 566, MinorUnit.Two, "Nigerian naira", "₦"),
            new ("NIO", 558, MinorUnit.Two, "Nicaraguan córdoba", "C$"),
            new ("NOK", 578, MinorUnit.Two, "Norwegian krone", "kr"),
            new ("NPR", 524, MinorUnit.Two, "Nepalese rupee", "रु") { AlternativeSymbols = ["₨", "Rs"]},
            new ("NZD", 554, MinorUnit.Two, "New Zealand dollar", "$") { InternationalSymbol = "NZ$" },
            new ("OMR", 512, MinorUnit.Three, "Omani rial", "ر.ع."),
            new ("PAB", 590, MinorUnit.Two, "Panamanian balboa", "B/."),
            new ("PEN", 604, MinorUnit.Two, "Peruvian sol", "S/."),
            new ("PGK", 598, MinorUnit.Two, "Papua New Guinean kina", "K"),
            new ("PHP", 608, MinorUnit.Two, "Philippine Peso", "₱") { AlternativeSymbols = ["P", "PhP"] },
            new ("PKR", 586, MinorUnit.Two, "Pakistani rupee", "Rs"),
            new ("PLN", 985, MinorUnit.Two, "Polish złoty", "zł"),
            new ("PYG", 600, MinorUnit.Zero, "Paraguayan guaraní", "₲"),
            new ("QAR", 634, MinorUnit.Two, "Qatari riyal", "ر.ق") { AlternativeSymbols = ["QR"] },
            new ("RON", 946, MinorUnit.Two, "Romanian new leu", "lei"),
            new ("RSD", 941, MinorUnit.Two, "Serbian dinar", "дин.") { AlternativeSymbols = ["din."] },
            new ("RUB", 643, MinorUnit.Two, "Russian rouble", "₽"),
            new ("RWF", 646, MinorUnit.Zero, "Rwandan franc", "RFw") { AlternativeSymbols = ["RF", "R₣"]},
            new ("SAR", 682, MinorUnit.Two, "Saudi riyal", "ر.س") { AlternativeSymbols = ["SR"] },
            new ("SBD", 090, MinorUnit.Two, "Solomon Islands dollar", "$") { InternationalSymbol = "SI$"},
            new ("SCR", 690, MinorUnit.Two, "Seychelles rupee", "Rs") { AlternativeSymbols = ["Re", "Rs.", "Re."] },
            new ("SDG", 938, MinorUnit.Two, "Sudanese pound", "ج.س."),
            new ("SEK", 752, MinorUnit.Two, "Swedish krona/kronor", "kr"),
            new ("SGD", 702, MinorUnit.Two, "Singapore dollar", "$") { InternationalSymbol = "S$" },
            new ("SHP", 654, MinorUnit.Two, "Saint Helena pound", "£"),
            new ("SOS", 706, MinorUnit.Two, "Somali shilling", "Sh.So."),
            new ("SRD", 968, MinorUnit.Two, "Surinamese dollar", "$") { InternationalSymbol = "Sr$" },
            new ("SSP", 728, MinorUnit.Two, "South Sudanese pound", "£") { InternationalSymbol = "SSP" },
            new ("SVC", 222, MinorUnit.Two, "El Salvador Colon", "₡"),
            new ("SYP", 760, MinorUnit.Two, "Syrian pound", "ل.س") { AlternativeSymbols = ["LS", "£S"] },
            new ("SZL", 748, MinorUnit.Two, "Swazi lilangeni", "L") { AlternativeSymbols = ["E"] }, // E is for plural
            new ("THB", 764, MinorUnit.Two, "Thai baht", "฿"),
            new ("TJS", 972, MinorUnit.Two, "Tajikistani somoni", "смн"),
            new ("TMT", 934, MinorUnit.Two, "Turkmenistani manat", "m"),
            new ("TND", 788, MinorUnit.Three, "Tunisian dinar", "د.ت") { AlternativeSymbols = ["DT"]},
            new ("TOP", 776, MinorUnit.Two, "Tongan paʻanga", "T$"),
            new ("TRY", 949, MinorUnit.Two, "Turkish lira", "₺"),
            new ("TTD", 780, MinorUnit.Two, "Trinidad and Tobago dollar", "$") { InternationalSymbol = "TT$" },
            new ("TWD", 901, MinorUnit.Two, "New Taiwan dollar", "$") { InternationalSymbol = "NT$" },
            new ("TZS", 834, MinorUnit.Two, "Tanzanian shilling", "TSh"), // often written in x/y format, where x is the amount above 1 shilling, while y is the amount in cents
            new ("UAH", 980, MinorUnit.Two, "Ukrainian hryvnia", "₴"),
            new ("UGX", 800, MinorUnit.Zero, "Ugandan shilling", "USh"),
            new ("USD", 840, MinorUnit.Two, "United States dollar", "$") { InternationalSymbol = "US$"},
            new ("USN", 997, MinorUnit.Two, "United States dollar (next day) (funds code)", "$"),
            new ("UYI", 940, MinorUnit.Zero, "Uruguay Peso en Unidades Indexadas (UI) (funds code)"), // List two
            new ("UYU", 858, MinorUnit.Two, "Uruguayan peso", "$") { InternationalSymbol = "$U" },
            new ("UZS", 860, MinorUnit.Two, "Uzbekistan som", "сўм") { AlternativeSymbols = ["soʻm"]},
            new ("VND", 704, MinorUnit.Zero, "Vietnamese dong", "₫"),
            new ("VUV", 548, MinorUnit.Zero, "Vanuatu vatu", "VT"),
            new ("WST", 882, MinorUnit.Two, "Samoan tala", "$") { InternationalSymbol = "WS$", AlternativeSymbols = ["SAT", "ST", "T"] },
            new ("XAF", 950, MinorUnit.Zero, "CFA franc BEAC", "FCFA"),
            new ("XAG", 961, MinorUnit.NotApplicable, "Silver (one troy ounce)"),
            new ("XAU", 959, MinorUnit.NotApplicable, "Gold (one troy ounce)"),
            new ("XBA", 955, MinorUnit.NotApplicable, "European Composite Unit (EURCO) (bond market unit)"),
            new ("XBB", 956, MinorUnit.NotApplicable, "European Monetary Unit (E.M.U.-6) (bond market unit)"),
            new ("XBC", 957, MinorUnit.NotApplicable, "European Unit of Account 9 (E.U.A.-9) (bond market unit)"),
            new ("XBD", 958, MinorUnit.NotApplicable, "European Unit of Account 17 (E.U.A.-17) (bond market unit)"),
            new ("XCD", 951, MinorUnit.Two, "East Caribbean dollar", "$") { InternationalSymbol = "EC$" },
            new ("XDR", 960, MinorUnit.NotApplicable, "Special drawing rights"),
            new ("XOF", 952, MinorUnit.Zero, "CFA franc BCEAO", "CFA"),
            new ("XPD", 964, MinorUnit.NotApplicable, "Palladium (one troy ounce)"),
            new ("XPF", 953, MinorUnit.Zero, "CFP franc", "F"),
            new ("XPT", 962, MinorUnit.NotApplicable, "Platinum (one troy ounce)"),
            new ("XSU", 994, MinorUnit.NotApplicable, "SUCRE"),
            new ("XTS", 963, MinorUnit.NotApplicable, "Code reserved for testing purposes"),
            new ("XUA", 965, MinorUnit.NotApplicable, "ADB Unit of Account"),
            new ("XXX", 999, MinorUnit.NotApplicable, "No currency"),
            new ("YER", 886, MinorUnit.Two, "Yemeni rial", "﷼") { AlternativeSymbols = ["YRI", "YRIs"]}, // YRI is for singular and YRIs for plural
            new ("ZAR", 710, MinorUnit.Two, "South African rand", "R"),
            new ("ZMW", 967, MinorUnit.Two, "Zambian kwacha", "ZK"),
            new ("STN", 930, MinorUnit.Two, "Dobra", "Db") { IntroducedOn = new DateTime(2018, 1, 1) }, // New Currency of São Tomé and Príncipe from 1 Jan 2018 (Amendment 164)
            new ("UYW", 927, MinorUnit.Four, "Unidad Previsional", "Db") { IntroducedOn = new DateTime(2018, 8, 29) },
            new ("VES", 928, MinorUnit.Two, "Venezuelan Bolívar Soberano", "Bs.") { IntroducedOn = new DateTime(2018, 8, 20) }, // or Bs.F. , Amendment 167 talks about delay but from multiple sources on the web the date seems to be 20 aug. // Replaced by VED/926 but stays active for now
            new ("VED", 926, MinorUnit.Two, "Venezuelan Bolívar Soberano", "Bs.") { IntroducedOn = new DateTime(2021, 10, 01) }, // replaces VES/928 (Amendment 170)
            new ("SLE", 925, MinorUnit.Two, "Sierra Leonean leone", "Le") { IntroducedOn = new DateTime(2021, 04, 01) }, // replaces SLL/694
            new ("ZWG", 924, MinorUnit.Two, "Zimbabwe Gold", "ZiG") { IntroducedOn = new DateTime(2024, 06, 25) }, // Amendment 177,  replaces ZWL/932,
            new ("XCG", 532, MinorUnit.Two, "Caribbean Guilder", "ƒ") { IntroducedOn = new DateTime(2025, 03, 31) }, // Amendment 176, replaces ANG/532 => Activate 31 March 2025
            new ("XAD", 396, MinorUnit.Two, "Arab Accounting Dinar") { IntroducedOn = new DateTime(2025, 05, 12) }, // Amendment 179, add new currency for the Finance Department Arab Monetary Fund (AMF)

            // Still Active list one: will move to Historic (list three) in the future
            new ("BGN", 975, MinorUnit.Two, "Bulgarian lev", "лв.") { ExpiredOn = new DateTime(2026, 01, 01) }, // Amendment 180, replaced by EUR/978,

            // Historic ISO-4217 currencies (list three)
            new ("ANG", 532, MinorUnit.Two, "Netherlands Antillean guilder", "ƒ") { ExpiredOn = new DateTime(2025, 07, 01) }, // Amendment 176, replaced by XCG/532
            new ("CUC", 931, MinorUnit.Two, "Cuban convertible peso", "CUC$") { ExpiredOn = new DateTime(2021, 06, 30), IntroducedOn = new DateTime(2009, 03, 01) }, // $ or CUC // Amendment 178, replace by CUP/192,
            new ("ZWL", 932, MinorUnit.Two, "Zimbabwean dollar", "$") { ExpiredOn = new DateTime(2024, 08, 31) }, // Amendment 177, replaces ZWG/924,
            new ("HRK", 191, MinorUnit.Two, "Croatian kuna", "kn") { ExpiredOn = new DateTime(2022, 12, 31) }, // replaced by EUR/978
            new ("SLL", 694, MinorUnit.Two, "Sierra Leonean leone", "Le") { ExpiredOn = new DateTime(2022, 9, 30) }, // replaced by SLE/925, redenominated by removing three (3) zeros from the denominations
            new ("BYR", 974, MinorUnit.Zero, "Belarusian ruble", "Br") { ExpiredOn = new DateTime(2016, 12, 31), IntroducedOn = new DateTime(2000, 01, 01) },
            new ("STD", 678, MinorUnit.Two, "Dobra", "Db") { ExpiredOn = new DateTime(2018, 1, 1) }, // To be replaced Currency of São Tomé and Príncipe from 1 Jan 2018 (Amendment 164),  inflation has rendered the cêntimo obsolete
            new ("VEF", 937, MinorUnit.Two, "Venezuelan bolívar", "Bs.") { ExpiredOn = new DateTime(2018, 8, 20) }, // replaced by VEF, The conversion rate is 1000 (old) Bolívar to 1 (new) Bolívar Soberano (1000:1). The expiration date of the current bolívar will be defined later and communicated by the Central Bank of Venezuela in due time.
            new ("MRO", 478, MinorUnit.OneFifth, "Mauritanian ouguiya", "UM") { ExpiredOn = new DateTime(2018, 1, 1) }, // replaced by MRU
            new ("ESA", 996, MinorUnit.NotApplicable, "Spanish peseta (account A)", "Pta") { ExpiredOn = new DateTime(2002, 3, 1) }, // replaced by ESP (EUR)
            new ("ESB", 995, MinorUnit.NotApplicable, "Spanish peseta (account B)", "Pta") { ExpiredOn = new DateTime(2002, 3, 1) }, // replaced by ESP (EUR)
            new ("LTL", 440, MinorUnit.Two, "Lithuanian litas", "Lt") { ExpiredOn = new DateTime(2014, 12, 31), IntroducedOn = new DateTime(1993, 1, 1) }, // replaced by EUR
            new ("USS", 998, MinorUnit.Two, "United States dollar (same day) (funds code)", "$") { ExpiredOn = new DateTime(2014, 3, 28) }, // replaced by (no successor)
            new ("LVL", 428, MinorUnit.Two, "Latvian lats", "Ls") { ExpiredOn = new DateTime(2013, 12, 31), IntroducedOn = new DateTime(1992, 1, 1) }, // replaced by EUR
            new ("XFU",   0, MinorUnit.NotApplicable, "UIC franc (special settlement currency) International Union of Railways") { ExpiredOn = new DateTime(2013, 11, 7) }, // replaced by EUR
            new ("ZMK", 894, MinorUnit.Two, "Zambian kwacha", "ZK") { ExpiredOn = new DateTime(2013, 1, 1), IntroducedOn = new DateTime(1968, 1, 16) }, // replaced by ZMW
            new ("EEK", 233, MinorUnit.Two, "Estonian kroon", "kr") { ExpiredOn = new DateTime(2010, 12, 31), IntroducedOn = new DateTime(1992, 1, 1) }, // replaced by EUR
            new ("ZWR", 935, MinorUnit.Two, "Zimbabwean dollar A/09", "$") { ExpiredOn = new DateTime(2009, 2, 2), IntroducedOn = new DateTime(2008, 8, 1) }, // replaced by ZWL
            new ("SKK", 703, MinorUnit.Two, "Slovak koruna", "Sk") { ExpiredOn = new DateTime(2008, 12, 31), IntroducedOn = new DateTime(1993, 2, 8) }, // replaced by EUR
            new ("TMM", 795, MinorUnit.Zero, "Turkmenistani manat", "T") { ExpiredOn = new DateTime(2008, 12, 31), IntroducedOn = new DateTime(1993, 11, 1) }, // replaced by TMT
            new ("ZWN", 942, MinorUnit.Two, "Zimbabwean dollar A/08", "$") { ExpiredOn = new DateTime(2008, 7, 31), IntroducedOn = new DateTime(2006, 8, 1) }, // replaced by ZWR
            new ("VEB", 862, MinorUnit.Two, "Venezuelan bolívar", "Bs.") { ExpiredOn = new DateTime(2008, 1, 1) }, // replaced by VEF
            new ("CYP", 196, MinorUnit.Two, "Cypriot pound", "£") { ExpiredOn = new DateTime(2007, 12, 31), IntroducedOn = new DateTime(1879, 1, 1) }, // replaced by EUR
            new ("MTL", 470, MinorUnit.Two, "Maltese lira", "₤") { ExpiredOn = new DateTime(2007, 12, 31), IntroducedOn = new DateTime(1972, 1, 1) }, // replaced by EUR
            new ("GHC", 288, MinorUnit.Zero, "Ghanaian cedi", "GH₵") { ExpiredOn = new DateTime(2007, 7, 1), IntroducedOn = new DateTime(1967, 1, 1) }, // replaced by GHS
            new ("SDD", 736, MinorUnit.NotApplicable, "Sudanese dinar", "£Sd") { ExpiredOn = new DateTime(2007, 1, 10), IntroducedOn = new DateTime(1992, 6, 8) }, // replaced by SDG
            new ("SIT", 705, MinorUnit.Two, "Slovenian tolar") { ExpiredOn = new DateTime(2006, 12, 31), IntroducedOn = new DateTime(1991, 10, 8) }, // replaced by EUR
            new ("ZWD", 716, MinorUnit.Two, "Zimbabwean dollar A/06", "$") { ExpiredOn = new DateTime(2006, 7, 31), IntroducedOn = new DateTime(1980, 4, 18) }, // replaced by ZWN
            new ("MZM", 508, MinorUnit.Zero, "Mozambican metical", "MT") { ExpiredOn = new DateTime(2006, 6, 30), IntroducedOn = new DateTime(1980, 1, 1) }, // replaced by MZN
            new ("AZM", 031, MinorUnit.Zero, "Azerbaijani manat", "₼") { ExpiredOn = new DateTime(2006, 1, 1), IntroducedOn = new DateTime(1992, 8, 15) }, // replaced by AZN
            new ("CSD", 891, MinorUnit.Two, "Serbian dinar") { ExpiredOn = new DateTime(2006, 12, 31), IntroducedOn = new DateTime(2003, 7, 3) }, // replaced by RSD
            new ("MGF", 450, MinorUnit.Two, "Malagasy franc") { ExpiredOn = new DateTime(2005, 1, 1), IntroducedOn = new DateTime(1963, 7, 1) }, // replaced by MGA
            new ("ROL", 642, MinorUnit.NotApplicable, "Romanian leu A/05") { ExpiredOn = new DateTime(2005, 12, 31), IntroducedOn = new DateTime(1952, 1, 28) }, // replaced by RON
            new ("TRL", 792, MinorUnit.Zero, "Turkish lira A/05", "₺") { ExpiredOn = new DateTime(2005, 12, 31) }, // replaced by TRY
            new ("SRG", 740, MinorUnit.NotApplicable, "Suriname guilder", "ƒ") { ExpiredOn = new DateTime(2004, 12, 31) }, // replaced by SRD
            new ("YUM", 891, MinorUnit.Two, "Yugoslav dinar", "дин.") { ExpiredOn =  new DateTime(2003, 7, 2), IntroducedOn = new DateTime(1994, 1, 24) }, // replaced by CSD
            new ("AFA", 004, MinorUnit.NotApplicable, "Afghan afghani", "؋") { ExpiredOn = new DateTime(2003, 12, 31), IntroducedOn = new DateTime(1925, 1, 1) }, // replaced by AFN
            new ("XFO",   0, MinorUnit.NotApplicable, "Gold franc (special settlement currency)") { ExpiredOn = new DateTime(2003, 12, 31), IntroducedOn = new DateTime(1803, 1, 1) }, // replaced by XDR
            new ("GRD", 300, MinorUnit.Two, "Greek drachma", "₯") { ExpiredOn = new DateTime(2000, 12, 31), IntroducedOn = new DateTime(1954, 1, 1) }, // replaced by EUR
            new ("TJR", 762, MinorUnit.NotApplicable, "Tajikistani ruble") { ExpiredOn = new DateTime(2000, 10, 30), IntroducedOn = new DateTime(1995, 5, 10) }, // replaced by TJS
            new ("ECV", 983, MinorUnit.NotApplicable, "Ecuador Unidad de Valor Constante (funds code)") { ExpiredOn = new DateTime(2000, 1, 9), IntroducedOn = new DateTime(1993, 1, 1) }, // replaced by (no successor)
            new ("ECS", 218, MinorUnit.Zero, "Ecuadorian sucre", "S/.") { ExpiredOn = new DateTime(2000, 12, 31), IntroducedOn = new DateTime(1884, 1, 1) }, // replaced by USD
            new ("BYB", 112, MinorUnit.Two, "Belarusian ruble", "Br") { ExpiredOn = new DateTime(1999, 12, 31), IntroducedOn = new DateTime(1992, 1, 1) }, // replaced by BYR
            new ("AOR", 982, MinorUnit.Zero, "Angolan kwanza readjustado", "Kz") { ExpiredOn = new DateTime(1999, 11, 30), IntroducedOn = new DateTime(1995, 7, 1) }, // replaced by AOA
            new ("BGL", 100, MinorUnit.Two, "Bulgarian lev A/99", "лв.") { ExpiredOn = new DateTime(1999, 7, 5), IntroducedOn = new DateTime(1962, 1, 1) }, // replaced by BGN
            new ("ADF",   0, MinorUnit.Two, "Andorran franc (1:1 peg to the French franc)", "Fr") { ExpiredOn = new DateTime(1998, 12, 31), IntroducedOn = new DateTime(1960, 1, 1) }, // replaced by EUR
            new ("ADP", 020, MinorUnit.Zero, "Andorran peseta (1:1 peg to the Spanish peseta)") { ExpiredOn = new DateTime(1998, 12, 31), IntroducedOn = new DateTime(1869, 1, 1) }, // replaced by EUR
            new ("ATS", 040, MinorUnit.Two, "Austrian schilling", "öS") { ExpiredOn = new DateTime(1998, 12, 31), IntroducedOn = new DateTime(1945, 1, 1) }, // replaced by EUR
            new ("BEF", 056, MinorUnit.Two, "Belgian franc (currency union with LUF)", "fr.") { ExpiredOn = new DateTime(1998, 12, 31), IntroducedOn = new DateTime(1832, 1, 1) }, // replaced by EUR
            new ("DEM", 276, MinorUnit.Two, "German mark", "DM") { ExpiredOn = new DateTime(1998, 12, 31), IntroducedOn = new DateTime(1948, 1, 1) }, // replaced by EUR
            new ("ESP", 724, MinorUnit.Zero, "Spanish peseta", "Pta") { ExpiredOn = new DateTime(1998, 12, 31), IntroducedOn = new DateTime(1869, 1, 1) }, // replaced by EUR
            new ("FIM", 246, MinorUnit.Two, "Finnish markka", "mk") { ExpiredOn = new DateTime(1998, 12, 31), IntroducedOn = new DateTime(1860, 1, 1) }, // replaced by EUR
            new ("FRF", 250, MinorUnit.Two, "French franc", "Fr") { ExpiredOn = new DateTime(1998, 12, 31), IntroducedOn = new DateTime(1960, 1, 1) }, // replaced by EUR
            new ("IEP", 372, MinorUnit.Two, "Irish pound (punt in Irish language)", "£") { ExpiredOn = new DateTime(1998, 12, 31), IntroducedOn = new DateTime(1938, 1, 1) }, // replaced by EUR
            new ("ITL", 380, MinorUnit.Zero, "Italian lira", "₤") { ExpiredOn = new DateTime(1998, 12, 31), IntroducedOn = new DateTime(1861, 1, 1) }, // replaced by EUR
            new ("LUF", 442, MinorUnit.Two, "Luxembourg franc (currency union with BEF)", "fr.") { ExpiredOn = new DateTime(1998, 12, 31), IntroducedOn = new DateTime(1944, 1, 1) }, // replaced by EUR
            new ("MCF",   0, MinorUnit.Two, "Monegasque franc (currency union with FRF)", "fr.") { ExpiredOn = new DateTime(1998, 12, 31), IntroducedOn = new DateTime(1960, 1, 1) }, // replaced by EUR
            new ("NLG", 528, MinorUnit.Two, "Dutch guilder", "ƒ") { ExpiredOn = new DateTime(1998, 12, 31), IntroducedOn = new DateTime(1810, 1, 1) }, // replaced by EUR
            new ("PTE", 620, MinorUnit.Zero, "Portuguese escudo", "$") { ExpiredOn = new DateTime(1998, 12, 31), IntroducedOn = new DateTime(1911, 5, 22) }, // replaced by EUR
            new ("SML",   0, MinorUnit.Zero, "San Marinese lira (currency union with ITL and VAL)", "₤") { ExpiredOn = new DateTime(1998, 12, 31), IntroducedOn = new DateTime(1864, 1, 1) }, // replaced by EUR
            new ("VAL",   0, MinorUnit.Zero, "Vatican lira (currency union with ITL and SML)", "₤") { ExpiredOn = new DateTime(1998, 12, 31), IntroducedOn = new DateTime(1929, 1, 1) }, // replaced by EUR
            new ("XEU", 954, MinorUnit.NotApplicable, "European Currency Unit (1 XEU = 1 EUR)", "ECU") { ExpiredOn = new DateTime(1998, 12, 31), IntroducedOn = new DateTime(1979, 3, 13) }, // replaced by EUR
            new ("BAD",   0, MinorUnit.Two, "Bosnia and Herzegovina dinar") { ExpiredOn = new DateTime(1998, 12, 31), IntroducedOn = new DateTime(1992, 7, 1) }, // replaced by BAM
            new ("RUR", 810, MinorUnit.Two, "Russian ruble A/97", "₽") { ExpiredOn = new DateTime(1997, 12, 31), IntroducedOn = new DateTime(1992, 1, 1) }, // replaced by RUB
            new ("GWP", 624, MinorUnit.NotApplicable, "Guinea-Bissau peso") { ExpiredOn = new DateTime(1997, 12, 31), IntroducedOn = new DateTime(1975, 1, 1) }, // replaced by XOF
            new ("ZRN", 180, MinorUnit.Two, "Zaïrean new zaïre", "Ƶ") { ExpiredOn = new DateTime(1997, 12, 31), IntroducedOn = new DateTime(1993, 1, 1) }, // replaced by CDF
            new ("UAK", 804, MinorUnit.NotApplicable, "Ukrainian karbovanets") { ExpiredOn = new DateTime(1996, 9, 1), IntroducedOn = new DateTime(1992, 10, 1) }, // replaced by UAH
            new ("YDD", 720, MinorUnit.NotApplicable, "South Yemeni dinar") { ExpiredOn = new DateTime(1996, 6, 11) }, // replaced by YER
            new ("AON", 024, MinorUnit.Zero, "Angolan new kwanza", "Kz") { ExpiredOn = new DateTime(1995, 6, 30), IntroducedOn = new DateTime(1990, 9, 25) }, // replaced by AOR
            new ("ZAL", 991, MinorUnit.NotApplicable, "South African financial rand (funds code)") { ExpiredOn = new DateTime(1995, 3, 13), IntroducedOn = new DateTime(1985, 9, 1) }, // replaced by (no successor)
            new ("PLZ", 616, MinorUnit.NotApplicable, "Polish zloty A/94", "zł") { ExpiredOn = new DateTime(1994, 12, 31), IntroducedOn = new DateTime(1950, 10, 30) }, // replaced by PLN
            new ("BRR",   0, MinorUnit.Two, "Brazilian cruzeiro real", "CR$") { ExpiredOn = new DateTime(1994, 6, 30), IntroducedOn = new DateTime(1993, 8, 1) }, // replaced by BRL
            new ("HRD",   0, MinorUnit.NotApplicable, "Croatian dinar") { ExpiredOn = new DateTime(1994, 5, 30), IntroducedOn = new DateTime(1991, 12, 23) }, // replaced by HRK
            new ("YUG",   0, MinorUnit.Two, "Yugoslav dinar", "дин.") { ExpiredOn = new DateTime(1994, 1, 23), IntroducedOn = new DateTime(1994, 1, 1) }, // replaced by YUM
            new ("YUO",   0, MinorUnit.Two, "Yugoslav dinar", "дин.") { ExpiredOn = new DateTime(1993, 12, 31), IntroducedOn = new DateTime(1993, 10, 1) }, // replaced by YUG
            new ("YUR",   0, MinorUnit.Two, "Yugoslav dinar", "дин.") { ExpiredOn = new DateTime(1993, 9, 30), IntroducedOn = new DateTime(1992, 7, 1) }, // replaced by YUO
            new ("BRE",   0, MinorUnit.Two, "Brazilian cruzeiro", "₢") { ExpiredOn = new DateTime(1993, 8, 1), IntroducedOn = new DateTime(1990, 3, 15) }, // replaced by BRR
            new ("UYN", 858, MinorUnit.NotApplicable, "Uruguay Peso", "$U") { ExpiredOn = new DateTime(1993, 3, 1), IntroducedOn = new DateTime(1975, 7, 1) }, // replaced by UYU
            new ("CSK", 200, MinorUnit.NotApplicable, "Czechoslovak koruna", "Kčs") { ExpiredOn = new DateTime(1993, 2, 8), IntroducedOn = new DateTime(7040, 1, 1) }, // replaced by CZK and SKK (CZK and EUR)
            new ("MKN", 0, MinorUnit.NotApplicable, "Old Macedonian denar A/93", "ден") { ExpiredOn = new DateTime(1993, 12, 31) }, // replaced by MKD
            new ("MXP", 484, MinorUnit.NotApplicable, "Mexican peso", "$") { ExpiredOn = new DateTime(1993, 12, 31) }, // replaced by MXN
            new ("ZRZ",   0, MinorUnit.Three, "Zaïrean zaïre", "Ƶ") { ExpiredOn = new DateTime(1993, 12, 31), IntroducedOn = new DateTime(1967, 1, 1) }, // replaced by ZRN
            new ("YUN",   0, MinorUnit.Two, "Yugoslav dinar", "дин.") { ExpiredOn = new DateTime(1992, 6, 30), IntroducedOn = new DateTime(1990, 1, 1) }, // replaced by YUR
            new ("SDP", 736, MinorUnit.NotApplicable, "Sudanese old pound", "ج.س.") { ExpiredOn = new DateTime(1992, 6, 8), IntroducedOn = new DateTime(1956, 1, 1) }, // replaced by SDD
            new ("ARA",   0, MinorUnit.Two, "Argentine austral", "₳") { ExpiredOn = new DateTime(1991, 12, 31), IntroducedOn = new DateTime(1985, 6, 15) }, // replaced by ARS
            new ("PEI",   0, MinorUnit.NotApplicable, "Peruvian inti", "I/.") { ExpiredOn = new DateTime(1991, 10, 1), IntroducedOn = new DateTime(1985, 2, 1) }, // replaced by PEN
            new ("SUR", 810, MinorUnit.NotApplicable, "Soviet Union Ruble", "руб") { ExpiredOn = new DateTime(1991, 12, 31), IntroducedOn = new DateTime(1961, 1, 1) }, // replaced by RUR
            new ("AOK", 024, MinorUnit.Zero, "Angolan kwanza", "Kz") { ExpiredOn = new DateTime(1990, 9, 24), IntroducedOn = new DateTime(1977, 1, 8) }, // replaced by AON
            new ("DDM", 278, MinorUnit.NotApplicable, "East German Mark of the GDR (East Germany)") { ExpiredOn = new DateTime(1990, 7, 1), IntroducedOn = new DateTime(1948, 6, 21) }, // replaced by DEM (EUR)
            new ("BRN",   0, MinorUnit.Two, "Brazilian cruzado novo", "NCz$") { ExpiredOn = new DateTime(1990, 3, 15), IntroducedOn = new DateTime(1989, 1, 16) }, // replaced by BRE
            new ("YUD", 891, MinorUnit.Two, "New Yugoslavian Dinar", "дин.") { ExpiredOn = new DateTime(1989, 12, 31), IntroducedOn = new DateTime(1966, 1, 1) }, // replaced by YUN
            new ("BRC",   0, MinorUnit.Two, "Brazilian cruzado", "Cz$") { ExpiredOn = new DateTime(1989, 1, 15), IntroducedOn = new DateTime(1986, 2, 28) }, // replaced by BRN
            new ("BOP", 068, MinorUnit.Two, "Peso boliviano", "b$.") { ExpiredOn = new DateTime(1987, 1, 1), IntroducedOn = new DateTime(1963, 1, 1) }, // replaced by BOB
            new ("UGS", 800, MinorUnit.NotApplicable, "Ugandan shilling A/87", "USh") { ExpiredOn = new DateTime(1987, 12, 31) }, // replaced by UGX
            new ("BRB", 076, MinorUnit.Two, "Brazilian cruzeiro", "₢") { ExpiredOn = new DateTime(1986, 2, 28), IntroducedOn = new DateTime(1970, 1, 1) }, // replaced by BRC
            new ("ILR", 376, MinorUnit.Two, "Israeli shekel") { ExpiredOn = new DateTime(1985, 12, 31), IntroducedOn = new DateTime(1980, 2, 24) }, // replaced by ILS
            new ("ARP",   0, MinorUnit.Two, "Argentine peso argentino", "$a") { ExpiredOn = new DateTime(1985, 6, 14), IntroducedOn = new DateTime(1983, 6, 6) }, // replaced by ARA
            new ("PEH", 604, MinorUnit.NotApplicable, "Peruvian old sol") { ExpiredOn = new DateTime(1985, 2, 1), IntroducedOn = new DateTime(1863, 1, 1) }, // replaced by PEI
            new ("GQE",   0, MinorUnit.NotApplicable, "Equatorial Guinean ekwele") { ExpiredOn = new DateTime(1985, 12, 31), IntroducedOn = new DateTime(1975, 1, 1) }, // replaced by XAF
            new ("GNE", 324, MinorUnit.NotApplicable, "Guinean syli") { ExpiredOn = new DateTime(1985, 12, 31), IntroducedOn = new DateTime(1971, 1, 1) }, // replaced by GNF
            new ("MLF",   0, MinorUnit.NotApplicable, "Mali franc", "MAF") { ExpiredOn = new DateTime(1984, 12, 31) }, // replaced by XOF
            new ("ARL",   0, MinorUnit.Two, "Argentine peso ley", "$L") { ExpiredOn = new DateTime(1983, 5, 5), IntroducedOn = new DateTime(1970, 1, 1) }, // replaced by ARP
            new ("ISJ", 352, MinorUnit.Two, "Icelandic krona", "kr") { ExpiredOn = new DateTime(1981, 12, 31), IntroducedOn = new DateTime(1922, 1, 1) }, // replaced by ISK
            new ("MVQ", 462, MinorUnit.NotApplicable, "Maldivian rupee", "Rf") { ExpiredOn = new DateTime(1981, 12, 31) }, // replaced by MVR
            new ("ILP", 376, MinorUnit.Three, "Israeli lira", "I£") { ExpiredOn = new DateTime(1980, 12, 31), IntroducedOn = new DateTime(1948, 1, 1) }, // ISRAEL Pound,  replaced by ILR
            new ("ZWC", 716, MinorUnit.Two, "Rhodesian dollar", "$") { ExpiredOn = new DateTime(1980, 12, 31), IntroducedOn = new DateTime(1970, 2, 17) }, // replaced by ZWD
            new ("LAJ", 418, MinorUnit.NotApplicable, "Pathet Lao Kip", "₭") { ExpiredOn = new DateTime(1979, 12, 31) }, // replaced by LAK
            new ("TPE",   0, MinorUnit.NotApplicable, "Portuguese Timorese escudo") { ExpiredOn = new DateTime(1976, 12, 31), IntroducedOn = new DateTime(1959, 1, 1) }, // replaced by IDR
            new ("UYP", 858, MinorUnit.NotApplicable, "Uruguay Peso", "$") { ExpiredOn = new DateTime(1975, 7, 1), IntroducedOn = new DateTime(1896, 1, 1) }, // replaced by UYN
            new ("CLE",   0, MinorUnit.NotApplicable, "Chilean escudo", "Eº") { ExpiredOn = new DateTime(1975, 12, 31), IntroducedOn = new DateTime(1960, 1, 1) }, // replaced by CLP
            new ("MAF",   0, MinorUnit.NotApplicable, "Moroccan franc") { ExpiredOn = new DateTime(1976, 12, 31), IntroducedOn = new DateTime(1921, 1, 1) }, // replaced by MAD
            new ("PTP",   0, MinorUnit.NotApplicable, "Portuguese Timorese pataca") { ExpiredOn = new DateTime(1958, 12, 31), IntroducedOn = new DateTime(1894, 1, 1) }, // replaced by TPE
            new ("TNF",   0, MinorUnit.Two, "Tunisian franc", "F") { ExpiredOn = new DateTime(1958, 12, 31), IntroducedOn = new DateTime(1991, 7, 1) }, // replaced by TND
            new ("NFD",   0, MinorUnit.Two, "Newfoundland dollar", "$") { ExpiredOn = new DateTime(1949, 12, 31), IntroducedOn = new DateTime(1865, 1, 1) }, // replaced by CAD

            // Added historic currencies of amendment 164 (research dates and other info)
            new ("VNC", 704, MinorUnit.Two, "Old Dong", "₫") { ExpiredOn = new DateTime(2014, 1, 1) }, // VIETNAM, replaced by VND with same number! Formerly, it was subdivided into 10 hào.
            new ("GNS", 324, MinorUnit.NotApplicable, "Guinean Syli") { ExpiredOn = new DateTime(1970, 12, 31) }, // GUINEA, replaced by GNE?
            new ("UGW", 800, MinorUnit.NotApplicable, "Old Shilling") { ExpiredOn = new DateTime(2017, 9, 22) }, // UGANDA
            new ("RHD", 716, MinorUnit.NotApplicable, "Rhodesian Dollar") { ExpiredOn = new DateTime(2017, 9, 22) }, // SOUTHERN RHODESIA
            new ("ROK", 642, MinorUnit.NotApplicable, "Leu A/52") { ExpiredOn = new DateTime(2017, 9, 22) }, // ROMANIA
            new ("NIC", 558, MinorUnit.NotApplicable, "Cordoba") { ExpiredOn = new DateTime(2017, 9, 22) }, // NICARAGUA
            new ("MZE", 508, MinorUnit.NotApplicable, "Mozambique Escudo") { ExpiredOn = new DateTime(2017, 9, 22) }, // MOZAMBIQUE
            new ("MTP", 470, MinorUnit.NotApplicable, "Maltese Pound") { ExpiredOn = new DateTime(2017, 9, 22) }, // MALTA
            new ("LSM", 426, MinorUnit.NotApplicable, "Loti") { ExpiredOn = new DateTime(2017, 9, 22) }, // LESOTHO
            new ("GWE", 624, MinorUnit.NotApplicable, "Guinea Escudo") { ExpiredOn = new DateTime(2017, 9, 22) }, // GUINEA-BISSAU
            new ("CSJ", 203, MinorUnit.NotApplicable, "Krona A/53") { ExpiredOn = new DateTime(2017, 9, 22) }, // CZECHOSLOVAKIA
            new ("BUK", 104, MinorUnit.NotApplicable, "Kyat") { ExpiredOn = new DateTime(2017, 9, 22) }, // BURMA
            new ("BGK", 100, MinorUnit.NotApplicable, "Lev A / 62") { ExpiredOn = new DateTime(2017, 9, 22) }, // BULGARIA
            new ("BGJ", 100, MinorUnit.NotApplicable, "Lev A / 52") { ExpiredOn = new DateTime(2017, 9, 22) }, // BULGARIA
            new ("ARY", 032, MinorUnit.NotApplicable, "Peso") { ExpiredOn = new DateTime(2017, 9, 22) }, // ARGENTINA

            // Other currencies (Non-ISO-4217)
            new ("BTC", 0, MinorUnit.Eight, "Bitcoin", "₿") { IsIso4217 = false, IntroducedOn = new DateTime(2009, 1, 3) },

            // The smallest unit of Ethereum is called a wei, which is equivalent to 10E−18
            new ("ETH", 0, MinorUnit.Eighteen, "Ethereum", "Ξ") { IsIso4217 = false, IntroducedOn = new DateTime(2015, 7, 30) }
        ];
}
