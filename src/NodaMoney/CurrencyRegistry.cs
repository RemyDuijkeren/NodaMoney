namespace NodaMoney;

/// <summary>Represent the central thread-safe registry for currencies.</summary>
static class CurrencyRegistry
{
    static CurrencyInfo[] s_currencies;
    static readonly Dictionary<Currency, CurrencyInfo> s_lookupCurrencies;
    static readonly object s_changeLock = new();

    static CurrencyRegistry()
    {
        s_currencies = InitializeCurrencies();

        // TODO: allow duplicates? (e.g. non ISO-4217 currencies)
        s_lookupCurrencies = new Dictionary<Currency, CurrencyInfo>(s_currencies.Length);
        foreach (var ci in s_currencies)
        {
            s_lookupCurrencies[ci.CurrencyUnit] = ci;
        }

        // TODO: Parallel foreach? ReadOnlySpan<T>
        // var xa = Currencies.AsMemory();
        // TODO: Use ReadOnlySpan<T> or ReadOnlyMemory<T>  to split up namespaces? 0..999 ISO4127, 1000..9999 ISO4127-HISTORIC
        // To much useless gaps, but for first 0..999 performance boost, because of no key lookup?
    }

    /// <summary>Tries the get <see cref="CurrencyInfo"/> of the given code and namespace.</summary>
    /// <param name="code">A currency code, like EUR or USD.</param>
    /// <returns><b>true</b> if <see cref="CurrencyRegistry"/> contains a <see cref="CurrencyInfo"/> with the specified code; otherwise, <b>false</b>.</returns>
    /// <exception cref="System.ArgumentNullException">The value of 'code' cannot be null or empty.</exception>
    public static CurrencyInfo Get(string code) => Get(new Currency(code));

    /// <summary>Tries the get <see cref="CurrencyInfo"/> of the given <see cref="Currency"/>.</summary>
    /// <param name="currency">A currency, like EUR or USD.</param>
    /// <returns><b>true</b> if <see cref="CurrencyRegistry"/> contains a <see cref="CurrencyInfo"/> with the specified code; otherwise, <b>false</b>.</returns>
    /// <exception cref="System.ArgumentNullException">The value of 'code' cannot be null or empty.</exception>
    public static CurrencyInfo Get(Currency currency) => s_lookupCurrencies.TryGetValue(currency, out var ci)
        ? ci
        : throw new InvalidCurrencyException($"{currency} is unknown currency code!");

    /// <summary>Attempts to add the <see cref="CurrencyInfo"/> to the register.</summary>
    /// <param name="currency">When this method returns, contains the <see cref="CurrencyInfo"/> that has the specified code and namespace, or the default value of the type if the operation failed.</param>
    /// <returns><b>true</b> if the <see cref="Currency"/> with the specified code is added; otherwise, <b>false</b>.</returns>
    /// <exception cref="System.ArgumentNullException">The value of 'code' or 'namespace' cannot be null or empty.</exception>
    public static bool TryAdd(CurrencyInfo currency)
    {
        lock (s_changeLock)
        {
            if (!s_lookupCurrencies.ContainsKey(currency.CurrencyUnit))
            {
                return false;
            }

            s_lookupCurrencies[currency.CurrencyUnit] = currency;

            Array.Resize(ref s_currencies, s_currencies.Length + 1);
            int index = s_currencies.Length - 1;
            s_currencies[index] = currency;

            return true;
        }
    }

    /// <summary>Attempts to remove the <see cref="CurrencyInfo"/> from the registry.</summary>
    /// <param name="currency">The <see cref="CurrencyInfo"/> to be removed.</param>
    /// <returns><b>true</b> if the <see cref="CurrencyInfo"/> with the specified code is removed; otherwise, <b>false</b>.</returns>
    /// <exception cref="System.ArgumentNullException">The value of 'code' or 'namespace' cannot be null or empty.</exception>
    public static bool TryRemove(CurrencyInfo currency)
    {
        lock (s_changeLock)
        {
            if (!s_lookupCurrencies.Remove(currency.CurrencyUnit))
            {
                return false;
            }

            int index = Array.IndexOf(s_currencies, currency);
            if (index == -1)
            {
                return true;
            }

            int lastIndex = s_currencies.Length - 1;
            if (index != lastIndex)
            {
                // Move the last element to the vacated spot.
                s_currencies[index] = s_currencies[lastIndex];
            }

            Array.Resize(ref s_currencies, s_currencies.Length - 1);

            return true;
        }
    }

    /// <summary>Get all registered currencies.</summary>
    /// <returns>An <see cref="IEnumerable{Currency}"/> of all registered currencies.</returns>
    public static IEnumerable<CurrencyInfo> GetAllCurrencies() => s_currencies.AsEnumerable();

    // TODO: Move to resource file?
    static CurrencyInfo[] InitializeCurrencies() =>
        [
            // ISO-4217 currencies (list one)
            new ("AED", 784, MinorUnit.Two, "United Arab Emirates dirham", "د.إ"),
            new ("AFN", 971, MinorUnit.Two, "Afghan afghani", "؋"),
            new ("ALL", 008, MinorUnit.Two, "Albanian lek", "L"),
            new ("AMD", 051, MinorUnit.Two, "Armenian dram", "֏"),
            new ("AOA", 973, MinorUnit.Two, "Angolan kwanza", "Kz"),
            new ("ARS", 032, MinorUnit.Two, "Argentine peso", "$"),
            new ("AUD", 036, MinorUnit.Two, "Australian dollar", "$"),
            new ("AWG", 533, MinorUnit.Two, "Aruban florin", "ƒ"),
            new ("AZN", 944, MinorUnit.Two, "Azerbaijan Manat", "ман"), // AZERBAIJAN
            new ("BAM", 977, MinorUnit.Two, "Bosnia and Herzegovina convertible mark", "KM"),
            new ("BBD", 052, MinorUnit.Two, "Barbados dollar", "$"),
            new ("BDT", 050, MinorUnit.Two, "Bangladeshi taka", "৳"), // or Tk
            new ("BGN", 975, MinorUnit.Two, "Bulgarian lev", "лв."),
            new ("BHD", 048, MinorUnit.Three, "Bahraini dinar", "BD"), // or د.ب. (switched for unit tests to work)
            new ("BIF", 108, MinorUnit.Zero, "Burundian franc", "FBu"),
            new ("BMD", 060, MinorUnit.Two, "Bermudian dollar", "$"),
            new ("BND", 096, MinorUnit.Two, "Brunei dollar", "$"), // or B$
            new ("BOB", 068, MinorUnit.Two, "Boliviano", "Bs."), // or BS or $b
            new ("BOV", 984, MinorUnit.Two, "Bolivian Mvdol (funds code)"), // <==== not found symbol. Is generic currency sign correct?
            new ("BRL", 986, MinorUnit.Two, "Brazilian real", "R$"),
            new ("BSD", 044, MinorUnit.Two, "Bahamian dollar", "$"),
            new ("BTN", 064, MinorUnit.Two, "Bhutanese ngultrum", "Nu."),
            new ("BWP", 072, MinorUnit.Two, "Botswana pula", "P"),
            new ("BYN", 933, MinorUnit.Two, "Belarusian ruble", "Br") { IntroducedOn = new DateTime(2006, 06, 01) },
            new ("BZD", 084, MinorUnit.Two, "Belize dollar", "BZ$"),
            new ("CAD", 124, MinorUnit.Two, "Canadian dollar", "$"),
            new ("CDF", 976, MinorUnit.Two, "Congolese franc", "FC"),
            new ("CHE", 947, MinorUnit.Two, "WIR Euro (complementary currency)", "CHE"),
            new ("CHF", 756, MinorUnit.Two, "Swiss franc", "fr."), // or CHF
            new ("CHW", 948, MinorUnit.Two, "WIR Franc (complementary currency)", "CHW"),
            new ("CLF", 990, MinorUnit.Four, "Unidad de Fomento (funds code)", "CLF"),
            new ("CLP", 152, MinorUnit.Zero, "Chilean peso", "$"),
            new ("CNY", 156, MinorUnit.Two, "Chinese yuan", "¥"),
            new ("COP", 170, MinorUnit.Two, "Colombian peso", "$"),
            new ("COU", 970, MinorUnit.Two, "Unidad de Valor Real", CurrencyInfo.GenericCurrencySign), // ???
            new ("CRC", 188, MinorUnit.Two, "Costa Rican colon", "₡"),
            new ("CUC", 931, MinorUnit.Two, "Cuban convertible peso", "CUC$"), // $ or CUC
            new ("CUP", 192, MinorUnit.Two, "Cuban peso", "$"), // or ₱ (obsolete?)
            new ("CVE", 132, MinorUnit.Two, "Cape Verde escudo", "$"),
            new ("CZK", 203, MinorUnit.Two, "Czech koruna", "Kč"),
            new ("DJF", 262, MinorUnit.Zero, "Djiboutian franc", "Fdj"),
            new ("DKK", 208, MinorUnit.Two, "Danish krone", "kr."),
            new ("DOP", 214, MinorUnit.Two, "Dominican peso", "RD$"), // or $
            new ("DZD", 012, MinorUnit.Two, "Algerian dinar", "DA"), // (Latin) or د.ج (Arabic)
            new ("EGP", 818, MinorUnit.Two, "Egyptian pound", "LE"), // or E£ or ج.م (Arabic)
            new ("ERN", 232, MinorUnit.Two, "Eritrean nakfa", "ERN"),
            new ("ETB", 230, MinorUnit.Two, "Ethiopian birr", "Br"), // (Latin) or ብር (Ethiopic)
            new ("EUR", 978, MinorUnit.Two, "Euro", "€"),
            new ("FJD", 242, MinorUnit.Two, "Fiji dollar", "$"), // or FJ$
            new ("FKP", 238, MinorUnit.Two, "Falkland Islands pound", "£"),
            new ("GBP", 826, MinorUnit.Two, "Pound sterling", "£"),
            new ("GEL", 981, MinorUnit.Two, "Georgian lari", "ლ."), // TODO: new symbol since July 18, 2014 => see http://en.wikipedia.org/wiki/Georgian_lari
            new ("GHS", 936, MinorUnit.Two, "Ghanaian cedi", "GH¢"), // or GH₵
            new ("GIP", 292, MinorUnit.Two, "Gibraltar pound", "£"),
            new ("GMD", 270, MinorUnit.Two, "Gambian dalasi", "D"),
            new ("GNF", 324, MinorUnit.Zero, "Guinean Franc", "FG"), // (possibly also Fr or GFr)  GUINEA
            new ("GTQ", 320, MinorUnit.Two, "Guatemalan quetzal", "Q"),
            new ("GYD", 328, MinorUnit.Two, "Guyanese dollar", "$"), // or G$
            new ("HKD", 344, MinorUnit.Two, "Hong Kong dollar", "HK$"), // or $
            new ("HNL", 340, MinorUnit.Two, "Honduran lempira", "L"),
            new ("HTG", 332, MinorUnit.Two, "Haitian gourde", "G"),
            new ("HUF", 348, MinorUnit.Two, "Hungarian forint", "Ft"),
            new ("IDR", 360, MinorUnit.Two, "Indonesian rupiah", "Rp"),
            new ("ILS", 376, MinorUnit.Two, "Israeli new shekel", "₪"),
            new ("INR", 356, MinorUnit.Two, "Indian rupee", "₹"),
            new ("IQD", 368, MinorUnit.Two, "Iraqi dinar", "د.ع"),
            new ("IRR", 364, MinorUnit.Two, "Iranian rial", "ريال"),
            new ("ISK", 352, MinorUnit.Zero, "Icelandic króna", "kr"),
            new ("JMD", 388, MinorUnit.Two, "Jamaican dollar", "J$"), // or $
            new ("JOD", 400, MinorUnit.Three, "Jordanian dinar", "د.ا.‏"),
            new ("JPY", 392, MinorUnit.Zero, "Japanese yen", "¥"),
            new ("KES", 404, MinorUnit.Two, "Kenyan shilling", "KSh"),
            new ("KGS", 417, MinorUnit.Two, "Kyrgyzstani som", "сом"),
            new ("KHR", 116, MinorUnit.Two, "Cambodian riel", "៛"),
            new ("KMF", 174, MinorUnit.Zero, "Comorian Franc", "CF"), // COMOROS (THE)
            new ("KPW", 408, MinorUnit.Two, "North Korean won", "₩"),
            new ("KRW", 410, MinorUnit.Zero, "South Korean won", "₩"),
            new ("KWD", 414, MinorUnit.Three, "Kuwaiti dinar", "د.ك"), // or K.D.
            new ("KYD", 136, MinorUnit.Two, "Cayman Islands dollar", "$"),
            new ("KZT", 398, MinorUnit.Two, "Kazakhstani tenge", "₸"),
            new ("LAK", 418, MinorUnit.Two, "Lao Kip", "₭"), // or ₭N,  LAO PEOPLE’S DEMOCRATIC REPUBLIC(THE), ISO says minor unit=2 but wiki says Historically, one kip was divided into 100 att (ອັດ).
            new ("LBP", 422, MinorUnit.Two, "Lebanese pound", "ل.ل"),
            new ("LKR", 144, MinorUnit.Two, "Sri Lankan rupee", "Rs"), // or රු
            new ("LRD", 430, MinorUnit.Two, "Liberian dollar", "$"), // or L$, LD$
            new ("LSL", 426, MinorUnit.Two, "Lesotho loti", "L"), // L or M (pl.)
            new ("LYD", 434, MinorUnit.Three, "Libyan dinar", "ل.د"), // or LD
            new ("MAD", 504, MinorUnit.Two, "Moroccan dirham", "د.م."),
            new ("MDL", 498, MinorUnit.Two, "Moldovan leu", "L"),
            new ("MGA", 969, MinorUnit.OneFifth, "Malagasy ariary", "Ar"),  // divided into five subunits rather than by a power of ten. 5 is 10 to the power of 0.69897...
            new ("MKD", 807, MinorUnit.Two, "Macedonian denar", "ден"),
            new ("MMK", 104, MinorUnit.Two, "Myanma kyat", "K"),
            new ("MNT", 496, MinorUnit.Two, "Mongolian tugrik", "₮"),
            new ("MOP", 446, MinorUnit.Two, "Macanese pataca", "MOP$"),
            new ("MRU", 929, MinorUnit.OneFifth, "Mauritanian ouguiya", "UM") { IntroducedOn = new DateTime(2018, 01, 01) }, // divided into five subunits rather than by a power of ten. 5 is 10 to the power of 0.69897...
            new ("MUR", 480, MinorUnit.Two, "Mauritian rupee", "Rs"),
            new ("MVR", 462, MinorUnit.Two, "Maldivian rufiyaa", "Rf"), // or , MRf, MVR, .ރ or /-
            new ("MWK", 454, MinorUnit.Two, "Malawi kwacha", "MK"),
            new ("MXN", 484, MinorUnit.Two, "Mexican peso", "$"),
            new ("MXV", 979, MinorUnit.Two, "Mexican Unidad de Inversion (UDI) (funds code)"),  // <==== not found
            new ("MYR", 458, MinorUnit.Two, "Malaysian ringgit", "RM"),
            new ("MZN", 943, MinorUnit.Two, "Mozambican metical", "MTn"), // or MTN
            new ("NAD", 516, MinorUnit.Two, "Namibian dollar", "N$"), // or $
            new ("NGN", 566, MinorUnit.Two, "Nigerian naira", "₦"),
            new ("NIO", 558, MinorUnit.Two, "Nicaraguan córdoba", "C$"),
            new ("NOK", 578, MinorUnit.Two, "Norwegian krone", "kr"),
            new ("NPR", 524, MinorUnit.Two, "Nepalese rupee", "Rs"), // or ₨ or रू
            new ("NZD", 554, MinorUnit.Two, "New Zealand dollar", "$"),
            new ("OMR", 512, MinorUnit.Three, "Omani rial", "ر.ع."),
            new ("PAB", 590, MinorUnit.Two, "Panamanian balboa", "B/."),
            new ("PEN", 604, MinorUnit.Two, "Peruvian sol", "S/."),
            new ("PGK", 598, MinorUnit.Two, "Papua New Guinean kina", "K"),
            new ("PHP", 608, MinorUnit.Two, "Philippine Peso", "₱"), // or P or PHP or PhP
            new ("PKR", 586, MinorUnit.Two, "Pakistani rupee", "Rs"),
            new ("PLN", 985, MinorUnit.Two, "Polish złoty", "zł"),
            new ("PYG", 600, MinorUnit.Zero, "Paraguayan guaraní", "₲"),
            new ("QAR", 634, MinorUnit.Two, "Qatari riyal", "ر.ق"), // or QR
            new ("RON", 946, MinorUnit.Two, "Romanian new leu", "lei"),
            new ("RSD", 941, MinorUnit.Two, "Serbian dinar", "РСД"), // or RSD (or дин or d./д)
            new ("RUB", 643, MinorUnit.Two, "Russian rouble", "₽"), // or R or руб (both onofficial)
            new ("RWF", 646, MinorUnit.Zero, "Rwandan franc", "RFw"), // or RF, R₣
            new ("SAR", 682, MinorUnit.Two, "Saudi riyal", "ر.س"), // or SR (Latin) or ﷼‎ (Unicode)
            new ("SBD", 090, MinorUnit.Two, "Solomon Islands dollar", "SI$"),
            new ("SCR", 690, MinorUnit.Two, "Seychelles rupee", "SR"), // or SRe
            new ("SDG", 938, MinorUnit.Two, "Sudanese pound", "ج.س."),
            new ("SEK", 752, MinorUnit.Two, "Swedish krona/kronor", "kr"),
            new ("SGD", 702, MinorUnit.Two, "Singapore dollar", "S$"), // or $
            new ("SHP", 654, MinorUnit.Two, "Saint Helena pound", "£"),
            new ("SOS", 706, MinorUnit.Two, "Somali shilling", "S"), // or Sh.So.
            new ("SRD", 968, MinorUnit.Two, "Surinamese dollar", "$"),
            new ("SSP", 728, MinorUnit.Two, "South Sudanese pound", "£"), // not sure about symbol...
            new ("SVC", 222, MinorUnit.Two, "El Salvador Colon", "₡"),
            new ("SYP", 760, MinorUnit.Two, "Syrian pound", "ܠ.ܣ.‏"), // or LS or £S (or £)
            new ("SZL", 748, MinorUnit.Two, "Swazi lilangeni", "L"), // or E (plural)
            new ("THB", 764, MinorUnit.Two, "Thai baht", "฿"),
            new ("TJS", 972, MinorUnit.Two, "Tajikistani somoni", "смн"),
            new ("TMT", 934, MinorUnit.Two, "Turkmenistani manat", "m"), // or T?
            new ("TND", 788, MinorUnit.Three, "Tunisian dinar", "د.ت"), // or DT (Latin)
            new ("TOP", 776, MinorUnit.Two, "Tongan paʻanga", "T$"), // (sometimes PT)
            new ("TRY", 949, MinorUnit.Two, "Turkish lira", "₺"),
            new ("TTD", 780, MinorUnit.Two, "Trinidad and Tobago dollar", "$"), // or TT$
            new ("TWD", 901, MinorUnit.Two, "New Taiwan dollar", "NT$"), // or $
            new ("TZS", 834, MinorUnit.Two, "Tanzanian shilling", "x/y"), // or TSh
            new ("UAH", 980, MinorUnit.Two, "Ukrainian hryvnia", "₴"),
            new ("UGX", 800, MinorUnit.Zero, "Ugandan shilling", "USh"),
            new ("USD", 840, MinorUnit.Two, "United States dollar", "$"), // or US$
            new ("USN", 997, MinorUnit.Two, "United States dollar (next day) (funds code)", "$"),
            new ("UYI", 940, MinorUnit.Zero, "Uruguay Peso en Unidades Indexadas (UI) (funds code)"), // List two
            new ("UYU", 858, MinorUnit.Two, "Uruguayan peso", "$"), // or $U
            new ("UZS", 860, MinorUnit.Two, "Uzbekistan som", "лв"), // or сўм ?
            new ("VND", 704, MinorUnit.Zero, "Vietnamese dong", "₫"),
            new ("VUV", 548, MinorUnit.Zero, "Vanuatu vatu", "VT"),
            new ("WST", 882, MinorUnit.Two, "Samoan tala", "WS$"), // sometimes SAT, ST or T
            new ("XAF", 950, MinorUnit.Zero, "CFA franc BEAC", "FCFA"),
            new ("XAG", 961, MinorUnit.NotApplicable, "Silver (one troy ounce)"),
            new ("XAU", 959, MinorUnit.NotApplicable, "Gold (one troy ounce)"),
            new ("XBA", 955, MinorUnit.NotApplicable, "European Composite Unit (EURCO) (bond market unit)"),
            new ("XBB", 956, MinorUnit.NotApplicable, "European Monetary Unit (E.M.U.-6) (bond market unit)"),
            new ("XBC", 957, MinorUnit.NotApplicable, "European Unit of Account 9 (E.U.A.-9) (bond market unit)"),
            new ("XBD", 958, MinorUnit.NotApplicable, "European Unit of Account 17 (E.U.A.-17) (bond market unit)"),
            new ("XCD", 951, MinorUnit.Two, "East Caribbean dollar", "$"), // or EC$
            new ("XDR", 960, MinorUnit.NotApplicable, "Special drawing rights"),
            new ("XOF", 952, MinorUnit.Zero, "CFA franc BCEAO", "CFA"),
            new ("XPD", 964, MinorUnit.NotApplicable, "Palladium (one troy ounce)"),
            new ("XPF", 953, MinorUnit.Zero, "CFP franc", "F"),
            new ("XPT", 962, MinorUnit.NotApplicable, "Platinum (one troy ounce)"),
            new ("XSU", 994, MinorUnit.NotApplicable, "SUCRE"),
            new ("XTS", 963, MinorUnit.NotApplicable, "Code reserved for testing purposes"),
            new ("XUA", 965, MinorUnit.NotApplicable, "ADB Unit of Account"),
            new ("XXX", 999, MinorUnit.NotApplicable, "No currency"),
            new ("YER", 886, MinorUnit.Two, "Yemeni rial", "﷼"), // or ر.ي.‏‏ ?
            new ("ZAR", 710, MinorUnit.Two, "South African rand", "R"),
            new ("ZMW", 967, MinorUnit.Two, "Zambian kwacha", "ZK"), // or ZMW
            new ("ZWL", 932, MinorUnit.Two, "Zimbabwean dollar", "$"),
            new ("STN", 930, MinorUnit.Two, "Dobra", "Db") { IntroducedOn = new DateTime(2018, 1, 1) }, // New Currency of São Tomé and Príncipe from 1 Jan 2018 (Amendment 164)
            new ("UYW", 927, MinorUnit.Four, "Unidad Previsional", "Db") { IntroducedOn = new DateTime(2018, 8, 29) },
            new ("VES", 928, MinorUnit.Two, "Venezuelan Bolívar Soberano", "Bs.") { IntroducedOn = new DateTime(2018, 8, 20) }, // or Bs.F. , Amendment 167 talks about delay but from multiple sources on the web the date seems to be 20 aug. // Replaced by VED/926 but stays active for now
            new ("VED", 926, MinorUnit.Two, "Venezuelan Bolívar Soberano", "Bs.") { IntroducedOn = new DateTime(2021, 10, 01) }, // replaces VES/928 (Amendment 170)
            new ("SLE", 925, MinorUnit.Two, "Sierra Leonean leone", "Le") { IntroducedOn = new DateTime(2021, 04, 01) }, // replaces SLL/694
            new ("ANG", 532, MinorUnit.Two, "Netherlands Antillean guilder", "ƒ") { ExpiredOn = new DateTime(2025, 03, 31) }, // Amendment 176, replaced by XCG/532

            // Still Active (list one), will move to Historic (list three) in the future
            new ("XCG", 532, MinorUnit.Two, "Caribbean Guilder", "ƒ") { IntroducedOn = new DateTime(2025, 03, 31) }, // Amendment 176, replaces ANG/532 => Activate 31 March 2025

            // Historic ISO-4217 currencies (list three)
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

            // The smallest unit of Ethereum is called a wei, which is equivalent to 10E−18 => TODO: How to store this? 13 is the max currently
            new ("ETH", 0, MinorUnit.Thirteen, "Ethereum", "Ξ") { IsIso4217 = false, IntroducedOn = new DateTime(2015, 7, 30) }
        ];

    // private static IDictionary<string, Currency> InitializeIsoCurrencies()
    // {
    //     // TODO: Move to resource file.
    //     return new Dictionary<string, Currency>
    //     {
    //         // ISO-4217 currencies (list one)
    //         ["ISO-4217::AED"] = new Currency("AED", 784, 2, "United Arab Emirates dirham", "د.إ"),
    //         ["ISO-4217::AFN"] = new Currency("AFN", 971, 2, "Afghan afghani", "؋"),
    //         ["ISO-4217::ALL"] = new Currency("ALL", 008, 2, "Albanian lek", "L"),
    //         ["ISO-4217::AMD"] = new Currency("AMD", 051, 2, "Armenian dram", "֏"),
    //         ["ISO-4217::AOA"] = new Currency("AOA", 973, 2, "Angolan kwanza", "Kz"),
    //         ["ISO-4217::ARS"] = new Currency("ARS", 032, 2, "Argentine peso", "$"),
    //         ["ISO-4217::AUD"] = new Currency("AUD", 036, 2, "Australian dollar", "$"),
    //         ["ISO-4217::AWG"] = new Currency("AWG", 533, 2, "Aruban florin", "ƒ"),
    //         ["ISO-4217::AZN"] = new Currency("AZN", 944, 2, "Azerbaijan Manat", "ман"), // AZERBAIJAN
    //         ["ISO-4217::BAM"] = new Currency("BAM", 977, 2, "Bosnia and Herzegovina convertible mark", "KM"),
    //         ["ISO-4217::BBD"] = new Currency("BBD", 052, 2, "Barbados dollar", "$"),
    //         ["ISO-4217::BDT"] = new Currency("BDT", 050, 2, "Bangladeshi taka", "৳"), // or Tk
    //         ["ISO-4217::BGN"] = new Currency("BGN", 975, 2, "Bulgarian lev", "лв."),
    //         ["ISO-4217::BHD"] = new Currency("BHD", 048, 3, "Bahraini dinar", "BD"), // or د.ب. (switched for unit tests to work)
    //         ["ISO-4217::BIF"] = new Currency("BIF", 108, 0, "Burundian franc", "FBu"),
    //         ["ISO-4217::BMD"] = new Currency("BMD", 060, 2, "Bermudian dollar", "$"),
    //         ["ISO-4217::BND"] = new Currency("BND", 096, 2, "Brunei dollar", "$"), // or B$
    //         ["ISO-4217::BOB"] = new Currency("BOB", 068, 2, "Boliviano", "Bs."), // or BS or $b
    //         ["ISO-4217::BOV"] = new Currency("BOV", 984, 2, "Bolivian Mvdol (funds code)", Currency.GenericCurrencySign), // <==== not found
    //         ["ISO-4217::BRL"] = new Currency("BRL", 986, 2, "Brazilian real", "R$"),
    //         ["ISO-4217::BSD"] = new Currency("BSD", 044, 2, "Bahamian dollar", "$"),
    //         ["ISO-4217::BTN"] = new Currency("BTN", 064, 2, "Bhutanese ngultrum", "Nu."),
    //         ["ISO-4217::BWP"] = new Currency("BWP", 072, 2, "Botswana pula", "P"),
    //         ["ISO-4217::BYN"] = new Currency("BYN", 933, 2, "Belarusian ruble", "Br", validFrom: new DateTime(2006, 06, 01)),
    //         ["ISO-4217::BZD"] = new Currency("BZD", 084, 2, "Belize dollar", "BZ$"),
    //         ["ISO-4217::CAD"] = new Currency("CAD", 124, 2, "Canadian dollar", "$"),
    //         ["ISO-4217::CDF"] = new Currency("CDF", 976, 2, "Congolese franc", "FC"),
    //         ["ISO-4217::CHE"] = new Currency("CHE", 947, 2, "WIR Euro (complementary currency)", "CHE"),
    //         ["ISO-4217::CHF"] = new Currency("CHF", 756, 2, "Swiss franc", "fr."), // or CHF
    //         ["ISO-4217::CHW"] = new Currency("CHW", 948, 2, "WIR Franc (complementary currency)", "CHW"),
    //         ["ISO-4217::CLF"] = new Currency("CLF", 990, 4, "Unidad de Fomento (funds code)", "CLF"),
    //         ["ISO-4217::CLP"] = new Currency("CLP", 152, 0, "Chilean peso", "$"),
    //         ["ISO-4217::CNY"] = new Currency("CNY", 156, 2, "Chinese yuan", "¥"),
    //         ["ISO-4217::COP"] = new Currency("COP", 170, 2, "Colombian peso", "$"),
    //         ["ISO-4217::COU"] = new Currency("COU", 970, 2, "Unidad de Valor Real", Currency.GenericCurrencySign), // ???
    //         ["ISO-4217::CRC"] = new Currency("CRC", 188, 2, "Costa Rican colon", "₡"),
    //         ["ISO-4217::CUC"] = new Currency("CUC", 931, 2, "Cuban convertible peso", "CUC$"), // $ or CUC
    //         ["ISO-4217::CUP"] = new Currency("CUP", 192, 2, "Cuban peso", "$"), // or ₱ (obsolete?)
    //         ["ISO-4217::CVE"] = new Currency("CVE", 132, 2, "Cape Verde escudo", "$"),
    //         ["ISO-4217::CZK"] = new Currency("CZK", 203, 2, "Czech koruna", "Kč"),
    //         ["ISO-4217::DJF"] = new Currency("DJF", 262, 0, "Djiboutian franc", "Fdj"),
    //         ["ISO-4217::DKK"] = new Currency("DKK", 208, 2, "Danish krone", "kr."),
    //         ["ISO-4217::DOP"] = new Currency("DOP", 214, 2, "Dominican peso", "RD$"), // or $
    //         ["ISO-4217::DZD"] = new Currency("DZD", 012, 2, "Algerian dinar", "DA"), // (Latin) or د.ج (Arabic)
    //         ["ISO-4217::EGP"] = new Currency("EGP", 818, 2, "Egyptian pound", "LE"), // or E£ or ج.م (Arabic)
    //         ["ISO-4217::ERN"] = new Currency("ERN", 232, 2, "Eritrean nakfa", "ERN"),
    //         ["ISO-4217::ETB"] = new Currency("ETB", 230, 2, "Ethiopian birr", "Br"), // (Latin) or ብር (Ethiopic)
    //         ["ISO-4217::EUR"] = new Currency("EUR", 978, 2, "Euro", "€"),
    //         ["ISO-4217::FJD"] = new Currency("FJD", 242, 2, "Fiji dollar", "$"), // or FJ$
    //         ["ISO-4217::FKP"] = new Currency("FKP", 238, 2, "Falkland Islands pound", "£"),
    //         ["ISO-4217::GBP"] = new Currency("GBP", 826, 2, "Pound sterling", "£"),
    //         ["ISO-4217::GEL"] = new Currency("GEL", 981, 2, "Georgian lari", "ლ."), // TODO: new symbol since July 18, 2014 => see http://en.wikipedia.org/wiki/Georgian_lari
    //         ["ISO-4217::GHS"] = new Currency("GHS", 936, 2, "Ghanaian cedi", "GH¢"), // or GH₵
    //         ["ISO-4217::GIP"] = new Currency("GIP", 292, 2, "Gibraltar pound", "£"),
    //         ["ISO-4217::GMD"] = new Currency("GMD", 270, 2, "Gambian dalasi", "D"),
    //         ["ISO-4217::GNF"] = new Currency("GNF", 324, 0, "Guinean Franc", "FG"), // (possibly also Fr or GFr)  GUINEA
    //         ["ISO-4217::GTQ"] = new Currency("GTQ", 320, 2, "Guatemalan quetzal", "Q"),
    //         ["ISO-4217::GYD"] = new Currency("GYD", 328, 2, "Guyanese dollar", "$"), // or G$
    //         ["ISO-4217::HKD"] = new Currency("HKD", 344, 2, "Hong Kong dollar", "HK$"), // or $
    //         ["ISO-4217::HNL"] = new Currency("HNL", 340, 2, "Honduran lempira", "L"),
    //         ["ISO-4217::HTG"] = new Currency("HTG", 332, 2, "Haitian gourde", "G"),
    //         ["ISO-4217::HUF"] = new Currency("HUF", 348, 2, "Hungarian forint", "Ft"),
    //         ["ISO-4217::IDR"] = new Currency("IDR", 360, 2, "Indonesian rupiah", "Rp"),
    //         ["ISO-4217::ILS"] = new Currency("ILS", 376, 2, "Israeli new shekel", "₪"),
    //         ["ISO-4217::INR"] = new Currency("INR", 356, 2, "Indian rupee", "₹"),
    //         ["ISO-4217::IQD"] = new Currency("IQD", 368, 3, "Iraqi dinar", "د.ع"),
    //         ["ISO-4217::IRR"] = new Currency("IRR", 364, 2, "Iranian rial", "ريال"),
    //         ["ISO-4217::ISK"] = new Currency("ISK", 352, 0, "Icelandic króna", "kr"),
    //         ["ISO-4217::JMD"] = new Currency("JMD", 388, 2, "Jamaican dollar", "J$"), // or $
    //         ["ISO-4217::JOD"] = new Currency("JOD", 400, 3, "Jordanian dinar", "د.ا.‏"),
    //         ["ISO-4217::JPY"] = new Currency("JPY", 392, 0, "Japanese yen", "¥"),
    //         ["ISO-4217::KES"] = new Currency("KES", 404, 2, "Kenyan shilling", "KSh"),
    //         ["ISO-4217::KGS"] = new Currency("KGS", 417, 2, "Kyrgyzstani som", "сом"),
    //         ["ISO-4217::KHR"] = new Currency("KHR", 116, 2, "Cambodian riel", "៛"),
    //         ["ISO-4217::KMF"] = new Currency("KMF", 174, 0, "Comorian Franc", "CF"), // COMOROS (THE)
    //         ["ISO-4217::KPW"] = new Currency("KPW", 408, 2, "North Korean won", "₩"),
    //         ["ISO-4217::KRW"] = new Currency("KRW", 410, 0, "South Korean won", "₩"),
    //         ["ISO-4217::KWD"] = new Currency("KWD", 414, 3, "Kuwaiti dinar", "د.ك"), // or K.D.
    //         ["ISO-4217::KYD"] = new Currency("KYD", 136, 2, "Cayman Islands dollar", "$"),
    //         ["ISO-4217::KZT"] = new Currency("KZT", 398, 2, "Kazakhstani tenge", "₸"),
    //         ["ISO-4217::LAK"] = new Currency("LAK", 418, 2, "Lao Kip", "₭"), // or ₭N,  LAO PEOPLE’S DEMOCRATIC REPUBLIC(THE), ISO says minor unit=2 but wiki says Historically, one kip was divided into 100 att (ອັດ).
    //         ["ISO-4217::LBP"] = new Currency("LBP", 422, 2, "Lebanese pound", "ل.ل"),
    //         ["ISO-4217::LKR"] = new Currency("LKR", 144, 2, "Sri Lankan rupee", "Rs"), // or රු
    //         ["ISO-4217::LRD"] = new Currency("LRD", 430, 2, "Liberian dollar", "$"), // or L$, LD$
    //         ["ISO-4217::LSL"] = new Currency("LSL", 426, 2, "Lesotho loti", "L"), // L or M (pl.)
    //         ["ISO-4217::LYD"] = new Currency("LYD", 434, 3, "Libyan dinar", "ل.د"), // or LD
    //         ["ISO-4217::MAD"] = new Currency("MAD", 504, 2, "Moroccan dirham", "د.م."),
    //         ["ISO-4217::MDL"] = new Currency("MDL", 498, 2, "Moldovan leu", "L"),
    //         ["ISO-4217::MGA"] = new Currency("MGA", 969, Z07Byte, "Malagasy ariary", "Ar"),  // divided into five subunits rather than by a power of ten. 5 is 10 to the power of 0.69897...
    //         ["ISO-4217::MKD"] = new Currency("MKD", 807, 2, "Macedonian denar", "ден"),
    //         ["ISO-4217::MMK"] = new Currency("MMK", 104, 2, "Myanma kyat", "K"),
    //         ["ISO-4217::MNT"] = new Currency("MNT", 496, 2, "Mongolian tugrik", "₮"),
    //         ["ISO-4217::MOP"] = new Currency("MOP", 446, 2, "Macanese pataca", "MOP$"),
    //         ["ISO-4217::MRU"] = new Currency("MRU", 929, Z07Byte, "Mauritanian ouguiya", "UM", validFrom: new DateTime(2018, 01, 01)), // divided into five subunits rather than by a power of ten. 5 is 10 to the power of 0.69897...
    //         ["ISO-4217::MUR"] = new Currency("MUR", 480, 2, "Mauritian rupee", "Rs"),
    //         ["ISO-4217::MVR"] = new Currency("MVR", 462, 2, "Maldivian rufiyaa", "Rf"), // or , MRf, MVR, .ރ or /-
    //         ["ISO-4217::MWK"] = new Currency("MWK", 454, 2, "Malawi kwacha", "MK"),
    //         ["ISO-4217::MXN"] = new Currency("MXN", 484, 2, "Mexican peso", "$"),
    //         ["ISO-4217::MXV"] = new Currency("MXV", 979, 2, "Mexican Unidad de Inversion (UDI) (funds code)", Currency.GenericCurrencySign),  // <==== not found
    //         ["ISO-4217::MYR"] = new Currency("MYR", 458, 2, "Malaysian ringgit", "RM"),
    //         ["ISO-4217::MZN"] = new Currency("MZN", 943, 2, "Mozambican metical", "MTn"), // or MTN
    //         ["ISO-4217::NAD"] = new Currency("NAD", 516, 2, "Namibian dollar", "N$"), // or $
    //         ["ISO-4217::NGN"] = new Currency("NGN", 566, 2, "Nigerian naira", "₦"),
    //         ["ISO-4217::NIO"] = new Currency("NIO", 558, 2, "Nicaraguan córdoba", "C$"),
    //         ["ISO-4217::NOK"] = new Currency("NOK", 578, 2, "Norwegian krone", "kr"),
    //         ["ISO-4217::NPR"] = new Currency("NPR", 524, 2, "Nepalese rupee", "Rs"), // or ₨ or रू
    //         ["ISO-4217::NZD"] = new Currency("NZD", 554, 2, "New Zealand dollar", "$"),
    //         ["ISO-4217::OMR"] = new Currency("OMR", 512, 3, "Omani rial", "ر.ع."),
    //         ["ISO-4217::PAB"] = new Currency("PAB", 590, 2, "Panamanian balboa", "B/."),
    //         ["ISO-4217::PEN"] = new Currency("PEN", 604, 2, "Peruvian sol", "S/."),
    //         ["ISO-4217::PGK"] = new Currency("PGK", 598, 2, "Papua New Guinean kina", "K"),
    //         ["ISO-4217::PHP"] = new Currency("PHP", 608, 2, "Philippine Peso", "₱"), // or P or PHP or PhP
    //         ["ISO-4217::PKR"] = new Currency("PKR", 586, 2, "Pakistani rupee", "Rs"),
    //         ["ISO-4217::PLN"] = new Currency("PLN", 985, 2, "Polish złoty", "zł"),
    //         ["ISO-4217::PYG"] = new Currency("PYG", 600, 0, "Paraguayan guaraní", "₲"),
    //         ["ISO-4217::QAR"] = new Currency("QAR", 634, 2, "Qatari riyal", "ر.ق"), // or QR
    //         ["ISO-4217::RON"] = new Currency("RON", 946, 2, "Romanian new leu", "lei"),
    //         ["ISO-4217::RSD"] = new Currency("RSD", 941, 2, "Serbian dinar", "РСД"), // or RSD (or дин or d./д)
    //         ["ISO-4217::RUB"] = new Currency("RUB", 643, 2, "Russian rouble", "₽"), // or R or руб (both onofficial)
    //         ["ISO-4217::RWF"] = new Currency("RWF", 646, 0, "Rwandan franc", "RFw"), // or RF, R₣
    //         ["ISO-4217::SAR"] = new Currency("SAR", 682, 2, "Saudi riyal", "ر.س"), // or SR (Latin) or ﷼‎ (Unicode)
    //         ["ISO-4217::SBD"] = new Currency("SBD", 090, 2, "Solomon Islands dollar", "SI$"),
    //         ["ISO-4217::SCR"] = new Currency("SCR", 690, 2, "Seychelles rupee", "SR"), // or SRe
    //         ["ISO-4217::SDG"] = new Currency("SDG", 938, 2, "Sudanese pound", "ج.س."),
    //         ["ISO-4217::SEK"] = new Currency("SEK", 752, 2, "Swedish krona/kronor", "kr"),
    //         ["ISO-4217::SGD"] = new Currency("SGD", 702, 2, "Singapore dollar", "S$"), // or $
    //         ["ISO-4217::SHP"] = new Currency("SHP", 654, 2, "Saint Helena pound", "£"),
    //         ["ISO-4217::SOS"] = new Currency("SOS", 706, 2, "Somali shilling", "S"), // or Sh.So.
    //         ["ISO-4217::SRD"] = new Currency("SRD", 968, 2, "Surinamese dollar", "$"),
    //         ["ISO-4217::SSP"] = new Currency("SSP", 728, 2, "South Sudanese pound", "£"), // not sure about symbol...
    //         ["ISO-4217::SVC"] = new Currency("SVC", 222, 2, "El Salvador Colon", "₡"),
    //         ["ISO-4217::SYP"] = new Currency("SYP", 760, 2, "Syrian pound", "ܠ.ܣ.‏"), // or LS or £S (or £)
    //         ["ISO-4217::SZL"] = new Currency("SZL", 748, 2, "Swazi lilangeni", "L"), // or E (plural)
    //         ["ISO-4217::THB"] = new Currency("THB", 764, 2, "Thai baht", "฿"),
    //         ["ISO-4217::TJS"] = new Currency("TJS", 972, 2, "Tajikistani somoni", "смн"),
    //         ["ISO-4217::TMT"] = new Currency("TMT", 934, 2, "Turkmenistani manat", "m"), // or T?
    //         ["ISO-4217::TND"] = new Currency("TND", 788, 3, "Tunisian dinar", "د.ت"), // or DT (Latin)
    //         ["ISO-4217::TOP"] = new Currency("TOP", 776, 2, "Tongan paʻanga", "T$"), // (sometimes PT)
    //         ["ISO-4217::TRY"] = new Currency("TRY", 949, 2, "Turkish lira", "₺"),
    //         ["ISO-4217::TTD"] = new Currency("TTD", 780, 2, "Trinidad and Tobago dollar", "$"), // or TT$
    //         ["ISO-4217::TWD"] = new Currency("TWD", 901, 2, "New Taiwan dollar", "NT$"), // or $
    //         ["ISO-4217::TZS"] = new Currency("TZS", 834, 2, "Tanzanian shilling", "x/y"), // or TSh
    //         ["ISO-4217::UAH"] = new Currency("UAH", 980, 2, "Ukrainian hryvnia", "₴"),
    //         ["ISO-4217::UGX"] = new Currency("UGX", 800, 0, "Ugandan shilling", "USh"),
    //         ["ISO-4217::USD"] = new Currency("USD", 840, 2, "United States dollar", "$"), // or US$
    //         ["ISO-4217::USN"] = new Currency("USN", 997, 2, "United States dollar (next day) (funds code)", "$"),
    //         ["ISO-4217::UYI"] = new Currency("UYI", 940, 0, "Uruguay Peso en Unidades Indexadas (UI) (funds code)", Currency.GenericCurrencySign), // List two
    //         ["ISO-4217::UYU"] = new Currency("UYU", 858, 2, "Uruguayan peso", "$"), // or $U
    //         ["ISO-4217::UZS"] = new Currency("UZS", 860, 2, "Uzbekistan som", "лв"), // or сўм ?
    //         ["ISO-4217::VES"] = new Currency("VES", 928, 2, "Venezuelan Bolívar Soberano", "Bs.", validFrom: new DateTime(2018, 8, 20)), // or Bs.F. , Amendment 167 talks about delay but from multiple sources on the web the date seems to be 20 aug. // Will be replaced by VED/926
    //         ["ISO-4217::VND"] = new Currency("VND", 704, 0, "Vietnamese dong", "₫"),
    //         ["ISO-4217::VUV"] = new Currency("VUV", 548, 0, "Vanuatu vatu", "VT"),
    //         ["ISO-4217::WST"] = new Currency("WST", 882, 2, "Samoan tala", "WS$"), // sometimes SAT, ST or T
    //         ["ISO-4217::XAF"] = new Currency("XAF", 950, 0, "CFA franc BEAC", "FCFA"),
    //         ["ISO-4217::XAG"] = new Currency("XAG", 961, NotApplicableByte, "Silver (one troy ounce)", Currency.GenericCurrencySign),
    //         ["ISO-4217::XAU"] = new Currency("XAU", 959, NotApplicableByte, "Gold (one troy ounce)", Currency.GenericCurrencySign),
    //         ["ISO-4217::XBA"] = new Currency("XBA", 955, NotApplicableByte, "European Composite Unit (EURCO) (bond market unit)", Currency.GenericCurrencySign),
    //         ["ISO-4217::XBB"] = new Currency("XBB", 956, NotApplicableByte, "European Monetary Unit (E.M.U.-6) (bond market unit)", Currency.GenericCurrencySign),
    //         ["ISO-4217::XBC"] = new Currency("XBC", 957, NotApplicableByte, "European Unit of Account 9 (E.U.A.-9) (bond market unit)", Currency.GenericCurrencySign),
    //         ["ISO-4217::XBD"] = new Currency("XBD", 958, NotApplicableByte, "European Unit of Account 17 (E.U.A.-17) (bond market unit)", Currency.GenericCurrencySign),
    //         ["ISO-4217::XCD"] = new Currency("XCD", 951, 2, "East Caribbean dollar", "$"), // or EC$
    //         ["ISO-4217::XDR"] = new Currency("XDR", 960, NotApplicableByte, "Special drawing rights", Currency.GenericCurrencySign),
    //         ["ISO-4217::XOF"] = new Currency("XOF", 952, 0, "CFA franc BCEAO", "CFA"),
    //         ["ISO-4217::XPD"] = new Currency("XPD", 964, NotApplicableByte, "Palladium (one troy ounce)", Currency.GenericCurrencySign),
    //         ["ISO-4217::XPF"] = new Currency("XPF", 953, 0, "CFP franc", "F"),
    //         ["ISO-4217::XPT"] = new Currency("XPT", 962, NotApplicableByte, "Platinum (one troy ounce)", Currency.GenericCurrencySign),
    //         ["ISO-4217::XSU"] = new Currency("XSU", 994, NotApplicableByte, "SUCRE", Currency.GenericCurrencySign),
    //         ["ISO-4217::XTS"] = new Currency("XTS", 963, NotApplicableByte, "Code reserved for testing purposes", Currency.GenericCurrencySign),
    //         ["ISO-4217::XUA"] = new Currency("XUA", 965, NotApplicableByte, "ADB Unit of Account", Currency.GenericCurrencySign),
    //         ["ISO-4217::XXX"] = new Currency("XXX", 999, NotApplicableByte, "No currency", Currency.GenericCurrencySign),
    //         ["ISO-4217::YER"] = new Currency("YER", 886, 2, "Yemeni rial", "﷼"), // or ر.ي.‏‏ ?
    //         ["ISO-4217::ZAR"] = new Currency("ZAR", 710, 2, "South African rand", "R"),
    //         ["ISO-4217::ZMW"] = new Currency("ZMW", 967, 2, "Zambian kwacha", "ZK"), // or ZMW
    //         ["ISO-4217::ZWL"] = new Currency("ZWL", 932, 2, "Zimbabwean dollar", "$"),
    //         ["ISO-4217::STN"] = new Currency("STN", 930, 2, "Dobra", "Db", validFrom: new DateTime(2018, 1, 1)), // New Currency of São Tomé and Príncipe from 1 Jan 2018 (Amendment 164)
    //         ["ISO-4217::STD"] = new Currency("STD", 678, 2, "Dobra", "Db", validTo: new DateTime(2018, 1, 1)), // To be replaced Currency of São Tomé and Príncipe from 1 Jan 2018 (Amendment 164),  inflation has rendered the cêntimo obsolete
    //         ["ISO-4217::UYW"] = new Currency("UYW", 927, 4, "Unidad Previsional", "Db", validFrom: new DateTime(2018, 8, 29)),
    //         // The Bolívar Soberano (VES) is redenominated by removing six zeros from the denominations. A new currency code VED/926 representing the new valuation (1,000,000 times old VES/928) is introduced on
    //         // 1 October 2021 for any internal needs during the redenomination process, but is not replacing VES as the official currency code (Amendment 170)
    //         ["ISO-4217::VED"] = new Currency("VED", 926, 2, "Venezuelan Bolívar Soberano", "Bs.", validFrom: new DateTime(2021, 10, 01)),
    //         ["ISO-4217::SLE"] = new Currency("SLE", 925, 2, "Sierra Leonean leone", "Le", validFrom: new DateTime(2021, 04, 01)), // replaces SLL/694
    //         ["ISO-4217::ANG"] = new Currency("ANG", 532, 2, "Netherlands Antillean guilder", "ƒ", validTo: new DateTime(2025, 03, 31)), // Amendment 176, replaced by XCG/532
    //         //["ISO-4217::XCG"] = new Currency("XCG", 532, 2, "Caribbean Guilder", "ƒ", validFrom: new DateTime(2025, 03, 31)), // Amendment 176, replaces ANG/532 => Activate 31 March 2025
    //
    //         // Historic ISO-4217 currencies (list three)
    //         ["ISO-4217-HISTORIC::HRK"] = new Currency("HRK", 191, 2, "Croatian kuna", "kn", Iso4217Historic, validTo: new DateTime(2022, 12, 31)), // replaced by EUR/978
    //         ["ISO-4217-HISTORIC::SLL"] = new Currency("SLL", 694, 2, "Sierra Leonean leone", "Le", Iso4217Historic, validTo: new DateTime(2022, 9, 30)), // replaced by SLE/925, redenominated by removing three (3) zeros from the denominations
    //         ["ISO-4217-HISTORIC::BYR"] = new Currency("BYR", 974, 0, "Belarusian ruble", "Br", Iso4217Historic, validTo: new DateTime(2016, 12, 31), validFrom: new DateTime(2000, 01, 01)),
    //         ["ISO-4217-HISTORIC::VEF"] = new Currency("VEF", 937, 2, "Venezuelan bolívar", "Bs.", Iso4217Historic, new DateTime(2018, 8, 20)), // replaced by VEF, The conversion rate is 1000 (old) Bolívar to 1 (new) Bolívar Soberano (1000:1). The expiration date of the current bolívar will be defined later and communicated by the Central Bank of Venezuela in due time.
    //         ["ISO-4217-HISTORIC::MRO"] = new Currency("MRO", 478, Z07Byte, "Mauritanian ouguiya", "UM", Iso4217Historic, new DateTime(2018, 1, 1)), // replaced by MRU
    //         ["ISO-4217-HISTORIC::ESA"] = new Currency("ESA", 996, NotApplicableByte, "Spanish peseta (account A)", "Pta", Iso4217Historic, new DateTime(2002, 3, 1)), // replaced by ESP (EUR)
    //         ["ISO-4217-HISTORIC::ESB"] = new Currency("ESB", 995, NotApplicableByte, "Spanish peseta (account B)", "Pta", Iso4217Historic, new DateTime(2002, 3, 1)), // replaced by ESP (EUR)
    //         ["ISO-4217-HISTORIC::LTL"] = new Currency("LTL", 440, 2, "Lithuanian litas", "Lt", Iso4217Historic, new DateTime(2014, 12, 31), new DateTime(1993, 1, 1)), // replaced by EUR
    //         ["ISO-4217-HISTORIC::USS"] = new Currency("USS", 998, 2, "United States dollar (same day) (funds code)", "$", Iso4217Historic, new DateTime(2014, 3, 28)), // replaced by (no successor)
    //         ["ISO-4217-HISTORIC::LVL"] = new Currency("LVL", 428, 2, "Latvian lats", "Ls", Iso4217Historic, new DateTime(2013, 12, 31), new DateTime(1992, 1, 1)), // replaced by EUR
    //         ["ISO-4217-HISTORIC::XFU"] = new Currency("XFU",   0, NotApplicableByte, "UIC franc (special settlement currency) International Union of Railways", Currency.GenericCurrencySign, Iso4217Historic, new DateTime(2013, 11, 7)), // replaced by EUR
    //         ["ISO-4217-HISTORIC::ZMK"] = new Currency("ZMK", 894, 2, "Zambian kwacha", "ZK", Iso4217Historic, new DateTime(2013, 1, 1), new DateTime(1968, 1, 16)), // replaced by ZMW
    //         ["ISO-4217-HISTORIC::EEK"] = new Currency("EEK", 233, 2, "Estonian kroon", "kr", Iso4217Historic, new DateTime(2010, 12, 31), new DateTime(1992, 1, 1)), // replaced by EUR
    //         ["ISO-4217-HISTORIC::ZWR"] = new Currency("ZWR", 935, 2, "Zimbabwean dollar A/09", "$", Iso4217Historic, new DateTime(2009, 2, 2), new DateTime(2008, 8, 1)), // replaced by ZWL
    //         ["ISO-4217-HISTORIC::SKK"] = new Currency("SKK", 703, 2, "Slovak koruna", "Sk", Iso4217Historic, new DateTime(2008, 12, 31), new DateTime(1993, 2, 8)), // replaced by EUR
    //         ["ISO-4217-HISTORIC::TMM"] = new Currency("TMM", 795, 0, "Turkmenistani manat", "T", Iso4217Historic, new DateTime(2008, 12, 31), new DateTime(1993, 11, 1)), // replaced by TMT
    //         ["ISO-4217-HISTORIC::ZWN"] = new Currency("ZWN", 942, 2, "Zimbabwean dollar A/08", "$", Iso4217Historic, new DateTime(2008, 7, 31), new DateTime(2006, 8, 1)), // replaced by ZWR
    //         ["ISO-4217-HISTORIC::VEB"] = new Currency("VEB", 862, 2, "Venezuelan bolívar", "Bs.", Iso4217Historic, new DateTime(2008, 1, 1)), // replaced by VEF
    //         ["ISO-4217-HISTORIC::CYP"] = new Currency("CYP", 196, 2, "Cypriot pound", "£", Iso4217Historic, new DateTime(2007, 12, 31), new DateTime(1879, 1, 1)), // replaced by EUR
    //         ["ISO-4217-HISTORIC::MTL"] = new Currency("MTL", 470, 2, "Maltese lira", "₤", Iso4217Historic, new DateTime(2007, 12, 31), new DateTime(1972, 1, 1)), // replaced by EUR
    //         ["ISO-4217-HISTORIC::GHC"] = new Currency("GHC", 288, 0, "Ghanaian cedi", "GH₵", Iso4217Historic, new DateTime(2007, 7, 1), new DateTime(1967, 1, 1)), // replaced by GHS
    //         ["ISO-4217-HISTORIC::SDD"] = new Currency("SDD", 736, NotApplicableByte, "Sudanese dinar", "£Sd", Iso4217Historic, new DateTime(2007, 1, 10), new DateTime(1992, 6, 8)), // replaced by SDG
    //         ["ISO-4217-HISTORIC::SIT"] = new Currency("SIT", 705, 2, "Slovenian tolar", Currency.GenericCurrencySign, Iso4217Historic, new DateTime(2006, 12, 31), new DateTime(1991, 10, 8)), // replaced by EUR
    //         ["ISO-4217-HISTORIC::ZWD"] = new Currency("ZWD", 716, 2, "Zimbabwean dollar A/06", "$", Iso4217Historic, new DateTime(2006, 7, 31), new DateTime(1980, 4, 18)), // replaced by ZWN
    //         ["ISO-4217-HISTORIC::MZM"] = new Currency("MZM", 508, 0, "Mozambican metical", "MT", Iso4217Historic, new DateTime(2006, 6, 30), new DateTime(1980, 1, 1)), // replaced by MZN
    //         ["ISO-4217-HISTORIC::AZM"] = new Currency("AZM", 031, 0, "Azerbaijani manat", "₼", Iso4217Historic, new DateTime(2006, 1, 1), new DateTime(1992, 8, 15)), // replaced by AZN
    //         ["ISO-4217-HISTORIC::CSD"] = new Currency("CSD", 891, 2, "Serbian dinar", Currency.GenericCurrencySign, Iso4217Historic, new DateTime(2006, 12, 31), new DateTime(2003, 7, 3)), // replaced by RSD
    //         ["ISO-4217-HISTORIC::MGF"] = new Currency("MGF", 450, 2, "Malagasy franc", Currency.GenericCurrencySign, Iso4217Historic, new DateTime(2005, 1, 1), new DateTime(1963, 7, 1)), // replaced by MGA
    //         ["ISO-4217-HISTORIC::ROL"] = new Currency("ROL", 642, NotApplicableByte, "Romanian leu A/05", Currency.GenericCurrencySign, Iso4217Historic, new DateTime(2005, 12, 31), new DateTime(1952, 1, 28)), // replaced by RON
    //         ["ISO-4217-HISTORIC::TRL"] = new Currency("TRL", 792, 0, "Turkish lira A/05", "₺", Iso4217Historic, new DateTime(2005, 12, 31)), // replaced by TRY
    //         ["ISO-4217-HISTORIC::SRG"] = new Currency("SRG", 740, NotApplicableByte, "Suriname guilder", "ƒ", Iso4217Historic, new DateTime(2004, 12, 31)), // replaced by SRD
    //         ["ISO-4217-HISTORIC::YUM"] = new Currency("YUM", 891, 2, "Yugoslav dinar", "дин.", Iso4217Historic, new DateTime(2003, 7, 2), new DateTime(1994, 1, 24)), // replaced by CSD
    //         ["ISO-4217-HISTORIC::AFA"] = new Currency("AFA", 004, NotApplicableByte, "Afghan afghani", "؋", Iso4217Historic, new DateTime(2003, 12, 31), new DateTime(1925, 1, 1)), // replaced by AFN
    //         ["ISO-4217-HISTORIC::XFO"] = new Currency("XFO",   0, NotApplicableByte, "Gold franc (special settlement currency)", Currency.GenericCurrencySign, Iso4217Historic, new DateTime(2003, 12, 31), new DateTime(1803, 1, 1)), // replaced by XDR
    //         ["ISO-4217-HISTORIC::GRD"] = new Currency("GRD", 300, 2, "Greek drachma", "₯", Iso4217Historic, new DateTime(2000, 12, 31), new DateTime(1954, 1, 1)), // replaced by EUR
    //         ["ISO-4217-HISTORIC::TJR"] = new Currency("TJR", 762, NotApplicableByte, "Tajikistani ruble", Currency.GenericCurrencySign, Iso4217Historic, new DateTime(2000, 10, 30), new DateTime(1995, 5, 10)), // replaced by TJS
    //         ["ISO-4217-HISTORIC::ECV"] = new Currency("ECV", 983, NotApplicableByte, "Ecuador Unidad de Valor Constante (funds code)", Currency.GenericCurrencySign, Iso4217Historic, new DateTime(2000, 1, 9), new DateTime(1993, 1, 1)), // replaced by (no successor)
    //         ["ISO-4217-HISTORIC::ECS"] = new Currency("ECS", 218, 0, "Ecuadorian sucre", "S/.", Iso4217Historic, new DateTime(2000, 12, 31), new DateTime(1884, 1, 1)), // replaced by USD
    //         ["ISO-4217-HISTORIC::BYB"] = new Currency("BYB", 112, 2, "Belarusian ruble", "Br", Iso4217Historic, new DateTime(1999, 12, 31), new DateTime(1992, 1, 1)), // replaced by BYR
    //         ["ISO-4217-HISTORIC::AOR"] = new Currency("AOR", 982, 0, "Angolan kwanza readjustado", "Kz", Iso4217Historic, new DateTime(1999, 11, 30), new DateTime(1995, 7, 1)), // replaced by AOA
    //         ["ISO-4217-HISTORIC::BGL"] = new Currency("BGL", 100, 2, "Bulgarian lev A/99", "лв.", Iso4217Historic, new DateTime(1999, 7, 5), new DateTime(1962, 1, 1)), // replaced by BGN
    //         ["ISO-4217-HISTORIC::ADF"] = new Currency("ADF",   0, 2, "Andorran franc (1:1 peg to the French franc)", "Fr", Iso4217Historic, new DateTime(1998, 12, 31), new DateTime(1960, 1, 1)), // replaced by EUR
    //         ["ISO-4217-HISTORIC::ADP"] = new Currency("ADP", 020, 0, "Andorran peseta (1:1 peg to the Spanish peseta)", Currency.GenericCurrencySign, Iso4217Historic, new DateTime(1998, 12, 31), new DateTime(1869, 1, 1)), // replaced by EUR
    //         ["ISO-4217-HISTORIC::ATS"] = new Currency("ATS", 040, 2, "Austrian schilling", "öS", Iso4217Historic, new DateTime(1998, 12, 31), new DateTime(1945, 1, 1)), // replaced by EUR
    //         ["ISO-4217-HISTORIC::BEF"] = new Currency("BEF", 056, 2, "Belgian franc (currency union with LUF)", "fr.", Iso4217Historic, new DateTime(1998, 12, 31), new DateTime(1832, 1, 1)), // replaced by EUR
    //         ["ISO-4217-HISTORIC::DEM"] = new Currency("DEM", 276, 2, "German mark", "DM", Iso4217Historic, new DateTime(1998, 12, 31), new DateTime(1948, 1, 1)), // replaced by EUR
    //         ["ISO-4217-HISTORIC::ESP"] = new Currency("ESP", 724, 0, "Spanish peseta", "Pta", Iso4217Historic, new DateTime(1998, 12, 31), new DateTime(1869, 1, 1)), // replaced by EUR
    //         ["ISO-4217-HISTORIC::FIM"] = new Currency("FIM", 246, 2, "Finnish markka", "mk", Iso4217Historic, new DateTime(1998, 12, 31), new DateTime(1860, 1, 1)), // replaced by EUR
    //         ["ISO-4217-HISTORIC::FRF"] = new Currency("FRF", 250, 2, "French franc", "Fr", Iso4217Historic, new DateTime(1998, 12, 31), new DateTime(1960, 1, 1)), // replaced by EUR
    //         ["ISO-4217-HISTORIC::IEP"] = new Currency("IEP", 372, 2, "Irish pound (punt in Irish language)", "£", Iso4217Historic, new DateTime(1998, 12, 31), new DateTime(1938, 1, 1)), // replaced by EUR
    //         ["ISO-4217-HISTORIC::ITL"] = new Currency("ITL", 380, 0, "Italian lira", "₤", Iso4217Historic, new DateTime(1998, 12, 31), new DateTime(1861, 1, 1)), // replaced by EUR
    //         ["ISO-4217-HISTORIC::LUF"] = new Currency("LUF", 442, 2, "Luxembourg franc (currency union with BEF)", "fr.", Iso4217Historic, new DateTime(1998, 12, 31), new DateTime(1944, 1, 1)), // replaced by EUR
    //         ["ISO-4217-HISTORIC::MCF"] = new Currency("MCF",   0, 2, "Monegasque franc (currency union with FRF)", "fr.", Iso4217Historic, new DateTime(1998, 12, 31), new DateTime(1960, 1, 1)), // replaced by EUR
    //         ["ISO-4217-HISTORIC::NLG"] = new Currency("NLG", 528, 2, "Dutch guilder", "ƒ", Iso4217Historic, new DateTime(1998, 12, 31), new DateTime(1810, 1, 1)), // replaced by EUR
    //         ["ISO-4217-HISTORIC::PTE"] = new Currency("PTE", 620, 0, "Portuguese escudo", "$", Iso4217Historic, new DateTime(1998, 12, 31), new DateTime(4160, 1, 1)), // replaced by EUR
    //         ["ISO-4217-HISTORIC::SML"] = new Currency("SML",   0, 0, "San Marinese lira (currency union with ITL and VAL)", "₤", Iso4217Historic, new DateTime(1998, 12, 31), new DateTime(1864, 1, 1)), // replaced by EUR
    //         ["ISO-4217-HISTORIC::VAL"] = new Currency("VAL",   0, 0, "Vatican lira (currency union with ITL and SML)", "₤", Iso4217Historic, new DateTime(1998, 12, 31), new DateTime(1929, 1, 1)), // replaced by EUR
    //         ["ISO-4217-HISTORIC::XEU"] = new Currency("XEU", 954, NotApplicableByte, "European Currency Unit (1 XEU = 1 EUR)", "ECU", Iso4217Historic, new DateTime(1998, 12, 31), new DateTime(1979, 3, 13)), // replaced by EUR
    //         ["ISO-4217-HISTORIC::BAD"] = new Currency("BAD",   0, 2, "Bosnia and Herzegovina dinar", Currency.GenericCurrencySign, Iso4217Historic, new DateTime(1998, 12, 31), new DateTime(1992, 7, 1)), // replaced by BAM
    //         ["ISO-4217-HISTORIC::RUR"] = new Currency("RUR", 810, 2, "Russian ruble A/97", "₽", Iso4217Historic, new DateTime(1997, 12, 31), new DateTime(1992, 1, 1)), // replaced by RUB
    //         ["ISO-4217-HISTORIC::GWP"] = new Currency("GWP", 624, NotApplicableByte, "Guinea-Bissau peso", Currency.GenericCurrencySign, Iso4217Historic, new DateTime(1997, 12, 31), new DateTime(1975, 1, 1)), // replaced by XOF
    //         ["ISO-4217-HISTORIC::ZRN"] = new Currency("ZRN", 180, 2, "Zaïrean new zaïre", "Ƶ", Iso4217Historic, new DateTime(1997, 12, 31), new DateTime(1993, 1, 1)), // replaced by CDF
    //         ["ISO-4217-HISTORIC::UAK"] = new Currency("UAK", 804, NotApplicableByte, "Ukrainian karbovanets", Currency.GenericCurrencySign, Iso4217Historic, new DateTime(1996, 9, 1), new DateTime(1992, 10, 1)), // replaced by UAH
    //         ["ISO-4217-HISTORIC::YDD"] = new Currency("YDD", 720, NotApplicableByte, "South Yemeni dinar", Currency.GenericCurrencySign, Iso4217Historic, new DateTime(1996, 6, 11)), // replaced by YER
    //         ["ISO-4217-HISTORIC::AON"] = new Currency("AON", 024, 0, "Angolan new kwanza", "Kz", Iso4217Historic, new DateTime(1995, 6, 30), new DateTime(1990, 9, 25)), // replaced by AOR
    //         ["ISO-4217-HISTORIC::ZAL"] = new Currency("ZAL", 991, NotApplicableByte, "South African financial rand (funds code)", Currency.GenericCurrencySign, Iso4217Historic, new DateTime(1995, 3, 13), new DateTime(1985, 9, 1)), // replaced by (no successor)
    //         ["ISO-4217-HISTORIC::PLZ"] = new Currency("PLZ", 616, NotApplicableByte, "Polish zloty A/94", "zł", Iso4217Historic, new DateTime(1994, 12, 31), new DateTime(1950, 10, 30)), // replaced by PLN
    //         ["ISO-4217-HISTORIC::BRR"] = new Currency("BRR",   0, 2, "Brazilian cruzeiro real", "CR$", Iso4217Historic, new DateTime(1994, 6, 30), new DateTime(1993, 8, 1)), // replaced by BRL
    //         ["ISO-4217-HISTORIC::HRD"] = new Currency("HRD",   0, NotApplicableByte, "Croatian dinar", Currency.GenericCurrencySign, Iso4217Historic, new DateTime(1994, 5, 30), new DateTime(1991, 12, 23)), // replaced by HRK
    //         ["ISO-4217-HISTORIC::YUG"] = new Currency("YUG",   0, 2, "Yugoslav dinar", "дин.", Iso4217Historic, new DateTime(1994, 1, 23), new DateTime(1994, 1, 1)), // replaced by YUM
    //         ["ISO-4217-HISTORIC::YUO"] = new Currency("YUO",   0, 2, "Yugoslav dinar", "дин.", Iso4217Historic, new DateTime(1993, 12, 31), new DateTime(1993, 10, 1)), // replaced by YUG
    //         ["ISO-4217-HISTORIC::YUR"] = new Currency("YUR",   0, 2, "Yugoslav dinar", "дин.", Iso4217Historic, new DateTime(1993, 9, 30), new DateTime(1992, 7, 1)), // replaced by YUO
    //         ["ISO-4217-HISTORIC::BRE"] = new Currency("BRE",   0, 2, "Brazilian cruzeiro", "₢", Iso4217Historic, new DateTime(1993, 8, 1), new DateTime(1990, 3, 15)), // replaced by BRR
    //         ["ISO-4217-HISTORIC::UYN"] = new Currency("UYN", 858, NotApplicableByte, "Uruguay Peso", "$U", Iso4217Historic, new DateTime(1993, 3, 1), new DateTime(1975, 7, 1)), // replaced by UYU
    //         ["ISO-4217-HISTORIC::CSK"] = new Currency("CSK", 200, NotApplicableByte, "Czechoslovak koruna", "Kčs", Iso4217Historic, new DateTime(1993, 2, 8), new DateTime(7040, 1, 1)), // replaced by CZK and SKK (CZK and EUR)
    //         ["ISO-4217-HISTORIC::MKN"] = new Currency("MKN", 0, NotApplicableByte, "Old Macedonian denar A/93", "ден", Iso4217Historic, new DateTime(1993, 12, 31)), // replaced by MKD
    //         ["ISO-4217-HISTORIC::MXP"] = new Currency("MXP", 484, NotApplicableByte, "Mexican peso", "$", Iso4217Historic, new DateTime(1993, 12, 31)), // replaced by MXN
    //         ["ISO-4217-HISTORIC::ZRZ"] = new Currency("ZRZ",   0, 3, "Zaïrean zaïre", "Ƶ", Iso4217Historic, new DateTime(1993, 12, 31), new DateTime(1967, 1, 1)), // replaced by ZRN
    //         ["ISO-4217-HISTORIC::YUN"] = new Currency("YUN",   0, 2, "Yugoslav dinar", "дин.", Iso4217Historic, new DateTime(1992, 6, 30), new DateTime(1990, 1, 1)), // replaced by YUR
    //         ["ISO-4217-HISTORIC::SDP"] = new Currency("SDP", 736, NotApplicableByte, "Sudanese old pound", "ج.س.", Iso4217Historic, new DateTime(1992, 6, 8), new DateTime(1956, 1, 1)), // replaced by SDD
    //         ["ISO-4217-HISTORIC::ARA"] = new Currency("ARA",   0, 2, "Argentine austral", "₳", Iso4217Historic, new DateTime(1991, 12, 31), new DateTime(1985, 6, 15)), // replaced by ARS
    //         ["ISO-4217-HISTORIC::PEI"] = new Currency("PEI",   0, NotApplicableByte, "Peruvian inti", "I/.", Iso4217Historic, new DateTime(1991, 10, 1), new DateTime(1985, 2, 1)), // replaced by PEN
    //         ["ISO-4217-HISTORIC::SUR"] = new Currency("SUR", 810, NotApplicableByte, "Soviet Union Ruble", "руб", Iso4217Historic, new DateTime(1991, 12, 31), new DateTime(1961, 1, 1)), // replaced by RUR
    //         ["ISO-4217-HISTORIC::AOK"] = new Currency("AOK", 024, 0, "Angolan kwanza", "Kz", Iso4217Historic, new DateTime(1990, 9, 24), new DateTime(1977, 1, 8)), // replaced by AON
    //         ["ISO-4217-HISTORIC::DDM"] = new Currency("DDM", 278, NotApplicableByte, "East German Mark of the GDR (East Germany)", Currency.GenericCurrencySign, Iso4217Historic, new DateTime(1990, 7, 1), new DateTime(1948, 6, 21)), // replaced by DEM (EUR)
    //         ["ISO-4217-HISTORIC::BRN"] = new Currency("BRN",   0, 2, "Brazilian cruzado novo", "NCz$", Iso4217Historic, new DateTime(1990, 3, 15), new DateTime(1989, 1, 16)), // replaced by BRE
    //         ["ISO-4217-HISTORIC::YUD"] = new Currency("YUD", 891, 2, "New Yugoslavian Dinar", "дин.", Iso4217Historic, new DateTime(1989, 12, 31), new DateTime(1966, 1, 1)), // replaced by YUN
    //         ["ISO-4217-HISTORIC::BRC"] = new Currency("BRC",   0, 2, "Brazilian cruzado", "Cz$", Iso4217Historic, new DateTime(1989, 1, 15), new DateTime(1986, 2, 28)), // replaced by BRN
    //         ["ISO-4217-HISTORIC::BOP"] = new Currency("BOP", 068, 2, "Peso boliviano", "b$.", Iso4217Historic, new DateTime(1987, 1, 1), new DateTime(1963, 1, 1)), // replaced by BOB
    //         ["ISO-4217-HISTORIC::UGS"] = new Currency("UGS", 800, NotApplicableByte, "Ugandan shilling A/87", "USh", Iso4217Historic, new DateTime(1987, 12, 31)), // replaced by UGX
    //         ["ISO-4217-HISTORIC::BRB"] = new Currency("BRB", 076, 2, "Brazilian cruzeiro", "₢", Iso4217Historic, new DateTime(1986, 2, 28), new DateTime(1970, 1, 1)), // replaced by BRC
    //         ["ISO-4217-HISTORIC::ILR"] = new Currency("ILR", 376, 2, "Israeli shekel", Currency.GenericCurrencySign, Iso4217Historic, new DateTime(1985, 12, 31), new DateTime(1980, 2, 24)), // replaced by ILS
    //         ["ISO-4217-HISTORIC::ARP"] = new Currency("ARP",   0, 2, "Argentine peso argentino", "$a", Iso4217Historic, new DateTime(1985, 6, 14), new DateTime(1983, 6, 6)), // replaced by ARA
    //         ["ISO-4217-HISTORIC::PEH"] = new Currency("PEH", 604, NotApplicableByte, "Peruvian old sol", Currency.GenericCurrencySign, Iso4217Historic, new DateTime(1985, 2, 1), new DateTime(1863, 1, 1)), // replaced by PEI
    //         ["ISO-4217-HISTORIC::GQE"] = new Currency("GQE",   0, NotApplicableByte, "Equatorial Guinean ekwele", Currency.GenericCurrencySign, Iso4217Historic, new DateTime(1985, 12, 31), new DateTime(1975, 1, 1)), // replaced by XAF
    //         ["ISO-4217-HISTORIC::GNE"] = new Currency("GNE", 324, NotApplicableByte, "Guinean syli", Currency.GenericCurrencySign, Iso4217Historic, new DateTime(1985, 12, 31), new DateTime(1971, 1, 1)), // replaced by GNF
    //         ["ISO-4217-HISTORIC::MLF"] = new Currency("MLF",   0, NotApplicableByte, "Mali franc", "MAF", Iso4217Historic, new DateTime(1984, 12, 31)), // replaced by XOF
    //         ["ISO-4217-HISTORIC::ARL"] = new Currency("ARL",   0, 2, "Argentine peso ley", "$L", Iso4217Historic, new DateTime(1983, 5, 5), new DateTime(1970, 1, 1)), // replaced by ARP
    //         ["ISO-4217-HISTORIC::ISJ"] = new Currency("ISJ", 352, 2, "Icelandic krona", "kr", Iso4217Historic, new DateTime(1981, 12, 31), new DateTime(1922, 1, 1)), // replaced by ISK
    //         ["ISO-4217-HISTORIC::MVQ"] = new Currency("MVQ", 462, NotApplicableByte, "Maldivian rupee", "Rf", Iso4217Historic, new DateTime(1981, 12, 31)), // replaced by MVR
    //         ["ISO-4217-HISTORIC::ILP"] = new Currency("ILP", 376, 3, "Israeli lira", "I£", Iso4217Historic, new DateTime(1980, 12, 31), new DateTime(1948, 1, 1)), // ISRAEL Pound,  replaced by ILR
    //         ["ISO-4217-HISTORIC::ZWC"] = new Currency("ZWC", 716, 2, "Rhodesian dollar", "$", Iso4217Historic, new DateTime(1980, 12, 31), new DateTime(1970, 2, 17)), // replaced by ZWD
    //         ["ISO-4217-HISTORIC::LAJ"] = new Currency("LAJ", 418, NotApplicableByte, "Pathet Lao Kip", "₭", Iso4217Historic, new DateTime(1979, 12, 31)), // replaced by LAK
    //         ["ISO-4217-HISTORIC::TPE"] = new Currency("TPE",   0, NotApplicableByte, "Portuguese Timorese escudo", Currency.GenericCurrencySign, Iso4217Historic, new DateTime(1976, 12, 31), new DateTime(1959, 1, 1)), // replaced by IDR
    //         ["ISO-4217-HISTORIC::UYP"] = new Currency("UYP", 858, NotApplicableByte, "Uruguay Peso", "$", Iso4217Historic, new DateTime(1975, 7, 1), new DateTime(1896, 1, 1)), // replaced by UYN
    //         ["ISO-4217-HISTORIC::CLE"] = new Currency("CLE",   0, NotApplicableByte, "Chilean escudo", "Eº", Iso4217Historic, new DateTime(1975, 12, 31), new DateTime(1960, 1, 1)), // replaced by CLP
    //         ["ISO-4217-HISTORIC::MAF"] = new Currency("MAF",   0, NotApplicableByte, "Moroccan franc", Currency.GenericCurrencySign, Iso4217Historic, new DateTime(1976, 12, 31), new DateTime(1921, 1, 1)), // replaced by MAD
    //         ["ISO-4217-HISTORIC::PTP"] = new Currency("PTP",   0, NotApplicableByte, "Portuguese Timorese pataca", Currency.GenericCurrencySign, Iso4217Historic, new DateTime(1958, 12, 31), new DateTime(1894, 1, 1)), // replaced by TPE
    //         ["ISO-4217-HISTORIC::TNF"] = new Currency("TNF",   0, 2, "Tunisian franc", "F", Iso4217Historic, new DateTime(1958, 12, 31), new DateTime(1991, 7, 1)), // replaced by TND
    //         ["ISO-4217-HISTORIC::NFD"] = new Currency("NFD",   0, 2, "Newfoundland dollar", "$", Iso4217Historic, new DateTime(1949, 12, 31), new DateTime(1865, 1, 1)), // replaced by CAD
    //
    //         // Added historic currencies of amendment 164 (research dates and other info)
    //         ["ISO-4217-HISTORIC::VNC"] = new Currency("VNC", 704, 2, "Old Dong", "₫", Iso4217Historic, new DateTime(2014, 1, 1)), // VIETNAM, replaced by VND with same number! Formerly, it was subdivided into 10 hào.
    //         ["ISO-4217-HISTORIC::GNS"] = new Currency("GNS", 324, NotApplicableByte, "Guinean Syli", Currency.GenericCurrencySign, Iso4217Historic, new DateTime(1970, 12, 31)), // GUINEA, replaced by GNE?
    //         ["ISO-4217-HISTORIC::UGW"] = new Currency("UGW", 800, NotApplicableByte, "Old Shilling", Currency.GenericCurrencySign, Iso4217Historic, new DateTime(2017, 9, 22)), // UGANDA
    //         ["ISO-4217-HISTORIC::RHD"] = new Currency("RHD", 716, NotApplicableByte, "Rhodesian Dollar", Currency.GenericCurrencySign, Iso4217Historic, new DateTime(2017, 9, 22)), // SOUTHERN RHODESIA
    //         ["ISO-4217-HISTORIC::ROK"] = new Currency("ROK", 642, NotApplicableByte, "Leu A/52", Currency.GenericCurrencySign, Iso4217Historic, new DateTime(2017, 9, 22)), // ROMANIA
    //         ["ISO-4217-HISTORIC::NIC"] = new Currency("NIC", 558, NotApplicableByte, "Cordoba", Currency.GenericCurrencySign, Iso4217Historic, new DateTime(2017, 9, 22)), // NICARAGUA
    //         ["ISO-4217-HISTORIC::MZE"] = new Currency("MZE", 508, NotApplicableByte, "Mozambique Escudo", Currency.GenericCurrencySign, Iso4217Historic, new DateTime(2017, 9, 22)), // MOZAMBIQUE
    //         ["ISO-4217-HISTORIC::MTP"] = new Currency("MTP", 470, NotApplicableByte, "Maltese Pound", Currency.GenericCurrencySign, Iso4217Historic, new DateTime(2017, 9, 22)), // MALTA
    //         ["ISO-4217-HISTORIC::LSM"] = new Currency("LSM", 426, NotApplicableByte, "Loti", Currency.GenericCurrencySign, Iso4217Historic, new DateTime(2017, 9, 22)), // LESOTHO
    //         ["ISO-4217-HISTORIC::GWE"] = new Currency("GWE", 624, NotApplicableByte, "Guinea Escudo", Currency.GenericCurrencySign, Iso4217Historic, new DateTime(2017, 9, 22)), // GUINEA-BISSAU
    //         ["ISO-4217-HISTORIC::CSJ"] = new Currency("CSJ", 203, NotApplicableByte, "Krona A/53", Currency.GenericCurrencySign, Iso4217Historic, new DateTime(2017, 9, 22)), // CZECHOSLOVAKIA
    //         ["ISO-4217-HISTORIC::BUK"] = new Currency("BUK", 104, NotApplicableByte, "Kyat", Currency.GenericCurrencySign, Iso4217Historic, new DateTime(2017, 9, 22)), // BURMA
    //         ["ISO-4217-HISTORIC::BGK"] = new Currency("BGK", 100, NotApplicableByte, "Lev A / 62", Currency.GenericCurrencySign, Iso4217Historic, new DateTime(2017, 9, 22)), // BULGARIA
    //         ["ISO-4217-HISTORIC::BGJ"] = new Currency("BGJ", 100, NotApplicableByte, "Lev A / 52", Currency.GenericCurrencySign, Iso4217Historic, new DateTime(2017, 9, 22)), // BULGARIA
    //         ["ISO-4217-HISTORIC::ARY"] = new Currency("ARY", 032, NotApplicableByte, "Peso", Currency.GenericCurrencySign, Iso4217Historic, new DateTime(2017, 9, 22)), // ARGENTINA
    //     };
    // }

    // private static Currency[] InitializeIsoCurrenciesArray()
    // {
    //     // TODO: Move to resource file.
    //     return new Currency[]
    //     {
    //         // ISO-4217 currencies (list one)
    //         new Currency("AED", 784, 2, "United Arab Emirates dirham", "د.إ"),
    //         new Currency("AFN", 971, 2, "Afghan afghani", "؋"),
    //         new Currency("ALL", 008, 2, "Albanian lek", "L"),
    //         new Currency("AMD", 051, 2, "Armenian dram", "֏"),
    //         new Currency("AOA", 973, 2, "Angolan kwanza", "Kz"),
    //         new Currency("ARS", 032, 2, "Argentine peso", "$"),
    //         new Currency("AUD", 036, 2, "Australian dollar", "$"),
    //         new Currency("AWG", 533, 2, "Aruban florin", "ƒ"),
    //         new Currency("AZN", 944, 2, "Azerbaijan Manat", "ман"), // AZERBAIJAN
    //         new Currency("BAM", 977, 2, "Bosnia and Herzegovina convertible mark", "KM"),
    //         new Currency("BBD", 052, 2, "Barbados dollar", "$"),
    //         new Currency("BDT", 050, 2, "Bangladeshi taka", "৳"), // or Tk
    //         new Currency("BGN", 975, 2, "Bulgarian lev", "лв."),
    //         new Currency("BHD", 048, 3, "Bahraini dinar", "BD"), // or د.ب. (switched for unit tests to work)
    //         new Currency("BIF", 108, 0, "Burundian franc", "FBu"),
    //         new Currency("BMD", 060, 2, "Bermudian dollar", "$"),
    //         new Currency("BND", 096, 2, "Brunei dollar", "$"), // or B$
    //         new Currency("BOB", 068, 2, "Boliviano", "Bs."), // or BS or $b
    //         new Currency("BOV", 984, 2, "Bolivian Mvdol (funds code)", Currency.GenericCurrencySign), // <==== not found
    //         new Currency("BRL", 986, 2, "Brazilian real", "R$"),
    //         new Currency("BSD", 044, 2, "Bahamian dollar", "$"),
    //         new Currency("BTN", 064, 2, "Bhutanese ngultrum", "Nu."),
    //         new Currency("BWP", 072, 2, "Botswana pula", "P"),
    //         new Currency("BYN", 933, 2, "Belarusian ruble", "Br", validFrom: new DateTime(2006, 06, 01)),
    //         new Currency("BZD", 084, 2, "Belize dollar", "BZ$"),
    //         new Currency("CAD", 124, 2, "Canadian dollar", "$"),
    //         new Currency("CDF", 976, 2, "Congolese franc", "FC"),
    //         new Currency("CHE", 947, 2, "WIR Euro (complementary currency)", "CHE"),
    //         new Currency("CHF", 756, 2, "Swiss franc", "fr."), // or CHF
    //         new Currency("CHW", 948, 2, "WIR Franc (complementary currency)", "CHW"),
    //         new Currency("CLF", 990, 4, "Unidad de Fomento (funds code)", "CLF"),
    //         new Currency("CLP", 152, 0, "Chilean peso", "$"),
    //         new Currency("CNY", 156, 2, "Chinese yuan", "¥"),
    //         new Currency("COP", 170, 2, "Colombian peso", "$"),
    //         new Currency("COU", 970, 2, "Unidad de Valor Real", Currency.GenericCurrencySign), // ???
    //         new Currency("CRC", 188, 2, "Costa Rican colon", "₡"),
    //         new Currency("CUC", 931, 2, "Cuban convertible peso", "CUC$"), // $ or CUC
    //         new Currency("CUP", 192, 2, "Cuban peso", "$"), // or ₱ (obsolete?)
    //         new Currency("CVE", 132, 2, "Cape Verde escudo", "$"),
    //         new Currency("CZK", 203, 2, "Czech koruna", "Kč"),
    //         new Currency("DJF", 262, 0, "Djiboutian franc", "Fdj"),
    //         new Currency("DKK", 208, 2, "Danish krone", "kr."),
    //         new Currency("DOP", 214, 2, "Dominican peso", "RD$"), // or $
    //         new Currency("DZD", 012, 2, "Algerian dinar", "DA"), // (Latin) or د.ج (Arabic)
    //         new Currency("EGP", 818, 2, "Egyptian pound", "LE"), // or E£ or ج.م (Arabic)
    //         new Currency("ERN", 232, 2, "Eritrean nakfa", "ERN"),
    //         new Currency("ETB", 230, 2, "Ethiopian birr", "Br"), // (Latin) or ብር (Ethiopic)
    //         new Currency("EUR", 978, 2, "Euro", "€"),
    //         new Currency("FJD", 242, 2, "Fiji dollar", "$"), // or FJ$
    //         new Currency("FKP", 238, 2, "Falkland Islands pound", "£"),
    //         new Currency("GBP", 826, 2, "Pound sterling", "£"),
    //         new Currency("GEL", 981, 2, "Georgian lari", "ლ."), // TODO: new symbol since July 18, 2014 => see http://en.wikipedia.org/wiki/Georgian_lari
    //         new Currency("GHS", 936, 2, "Ghanaian cedi", "GH¢"), // or GH₵
    //         new Currency("GIP", 292, 2, "Gibraltar pound", "£"),
    //         new Currency("GMD", 270, 2, "Gambian dalasi", "D"),
    //         new Currency("GNF", 324, 0, "Guinean Franc", "FG"), // (possibly also Fr or GFr)  GUINEA
    //         new Currency("GTQ", 320, 2, "Guatemalan quetzal", "Q"),
    //         new Currency("GYD", 328, 2, "Guyanese dollar", "$"), // or G$
    //         new Currency("HKD", 344, 2, "Hong Kong dollar", "HK$"), // or $
    //         new Currency("HNL", 340, 2, "Honduran lempira", "L"),
    //         new Currency("HTG", 332, 2, "Haitian gourde", "G"),
    //         new Currency("HUF", 348, 2, "Hungarian forint", "Ft"),
    //         new Currency("IDR", 360, 2, "Indonesian rupiah", "Rp"),
    //         new Currency("ILS", 376, 2, "Israeli new shekel", "₪"),
    //         new Currency("INR", 356, 2, "Indian rupee", "₹"),
    //         new Currency("IQD", 368, 3, "Iraqi dinar", "د.ع"),
    //         new Currency("IRR", 364, 2, "Iranian rial", "ريال"),
    //         new Currency("ISK", 352, 0, "Icelandic króna", "kr"),
    //         new Currency("JMD", 388, 2, "Jamaican dollar", "J$"), // or $
    //         new Currency("JOD", 400, 3, "Jordanian dinar", "د.ا.‏"),
    //         new Currency("JPY", 392, 0, "Japanese yen", "¥"),
    //         new Currency("KES", 404, 2, "Kenyan shilling", "KSh"),
    //         new Currency("KGS", 417, 2, "Kyrgyzstani som", "сом"),
    //         new Currency("KHR", 116, 2, "Cambodian riel", "៛"),
    //         new Currency("KMF", 174, 0, "Comorian Franc", "CF"), // COMOROS (THE)
    //         new Currency("KPW", 408, 2, "North Korean won", "₩"),
    //         new Currency("KRW", 410, 0, "South Korean won", "₩"),
    //         new Currency("KWD", 414, 3, "Kuwaiti dinar", "د.ك"), // or K.D.
    //         new Currency("KYD", 136, 2, "Cayman Islands dollar", "$"),
    //         new Currency("KZT", 398, 2, "Kazakhstani tenge", "₸"),
    //         new Currency("LAK", 418, 2, "Lao Kip", "₭"), // or ₭N,  LAO PEOPLE’S DEMOCRATIC REPUBLIC(THE), ISO says minor unit=2 but wiki says Historically, one kip was divided into 100 att (ອັດ).
    //         new Currency("LBP", 422, 2, "Lebanese pound", "ل.ل"),
    //         new Currency("LKR", 144, 2, "Sri Lankan rupee", "Rs"), // or රු
    //         new Currency("LRD", 430, 2, "Liberian dollar", "$"), // or L$, LD$
    //         new Currency("LSL", 426, 2, "Lesotho loti", "L"), // L or M (pl.)
    //         new Currency("LYD", 434, 3, "Libyan dinar", "ل.د"), // or LD
    //         new Currency("MAD", 504, 2, "Moroccan dirham", "د.م."),
    //         new Currency("MDL", 498, 2, "Moldovan leu", "L"),
    //         new Currency("MGA", 969, Z07Byte, "Malagasy ariary", "Ar"),  // divided into five subunits rather than by a power of ten. 5 is 10 to the power of 0.69897...
    //         new Currency("MKD", 807, 2, "Macedonian denar", "ден"),
    //         new Currency("MMK", 104, 2, "Myanma kyat", "K"),
    //         new Currency("MNT", 496, 2, "Mongolian tugrik", "₮"),
    //         new Currency("MOP", 446, 2, "Macanese pataca", "MOP$"),
    //         new Currency("MRU", 929, Z07Byte, "Mauritanian ouguiya", "UM", validFrom: new DateTime(2018, 01, 01)), // divided into five subunits rather than by a power of ten. 5 is 10 to the power of 0.69897...
    //         new Currency("MUR", 480, 2, "Mauritian rupee", "Rs"),
    //         new Currency("MVR", 462, 2, "Maldivian rufiyaa", "Rf"), // or , MRf, MVR, .ރ or /-
    //         new Currency("MWK", 454, 2, "Malawi kwacha", "MK"),
    //         new Currency("MXN", 484, 2, "Mexican peso", "$"),
    //         new Currency("MXV", 979, 2, "Mexican Unidad de Inversion (UDI) (funds code)", Currency.GenericCurrencySign),  // <==== not found
    //         new Currency("MYR", 458, 2, "Malaysian ringgit", "RM"),
    //         new Currency("MZN", 943, 2, "Mozambican metical", "MTn"), // or MTN
    //         new Currency("NAD", 516, 2, "Namibian dollar", "N$"), // or $
    //         new Currency("NGN", 566, 2, "Nigerian naira", "₦"),
    //         new Currency("NIO", 558, 2, "Nicaraguan córdoba", "C$"),
    //         new Currency("NOK", 578, 2, "Norwegian krone", "kr"),
    //         new Currency("NPR", 524, 2, "Nepalese rupee", "Rs"), // or ₨ or रू
    //         new Currency("NZD", 554, 2, "New Zealand dollar", "$"),
    //         new Currency("OMR", 512, 3, "Omani rial", "ر.ع."),
    //         new Currency("PAB", 590, 2, "Panamanian balboa", "B/."),
    //         new Currency("PEN", 604, 2, "Peruvian sol", "S/."),
    //         new Currency("PGK", 598, 2, "Papua New Guinean kina", "K"),
    //         new Currency("PHP", 608, 2, "Philippine Peso", "₱"), // or P or PHP or PhP
    //         new Currency("PKR", 586, 2, "Pakistani rupee", "Rs"),
    //         new Currency("PLN", 985, 2, "Polish złoty", "zł"),
    //         new Currency("PYG", 600, 0, "Paraguayan guaraní", "₲"),
    //         new Currency("QAR", 634, 2, "Qatari riyal", "ر.ق"), // or QR
    //         new Currency("RON", 946, 2, "Romanian new leu", "lei"),
    //         new Currency("RSD", 941, 2, "Serbian dinar", "РСД"), // or RSD (or дин or d./д)
    //         new Currency("RUB", 643, 2, "Russian rouble", "₽"), // or R or руб (both onofficial)
    //         new Currency("RWF", 646, 0, "Rwandan franc", "RFw"), // or RF, R₣
    //         new Currency("SAR", 682, 2, "Saudi riyal", "ر.س"), // or SR (Latin) or ﷼‎ (Unicode)
    //         new Currency("SBD", 090, 2, "Solomon Islands dollar", "SI$"),
    //         new Currency("SCR", 690, 2, "Seychelles rupee", "SR"), // or SRe
    //         new Currency("SDG", 938, 2, "Sudanese pound", "ج.س."),
    //         new Currency("SEK", 752, 2, "Swedish krona/kronor", "kr"),
    //         new Currency("SGD", 702, 2, "Singapore dollar", "S$"), // or $
    //         new Currency("SHP", 654, 2, "Saint Helena pound", "£"),
    //         new Currency("SOS", 706, 2, "Somali shilling", "S"), // or Sh.So.
    //         new Currency("SRD", 968, 2, "Surinamese dollar", "$"),
    //         new Currency("SSP", 728, 2, "South Sudanese pound", "£"), // not sure about symbol...
    //         new Currency("SVC", 222, 2, "El Salvador Colon", "₡"),
    //         new Currency("SYP", 760, 2, "Syrian pound", "ܠ.ܣ.‏"), // or LS or £S (or £)
    //         new Currency("SZL", 748, 2, "Swazi lilangeni", "L"), // or E (plural)
    //         new Currency("THB", 764, 2, "Thai baht", "฿"),
    //         new Currency("TJS", 972, 2, "Tajikistani somoni", "смн"),
    //         new Currency("TMT", 934, 2, "Turkmenistani manat", "m"), // or T?
    //         new Currency("TND", 788, 3, "Tunisian dinar", "د.ت"), // or DT (Latin)
    //         new Currency("TOP", 776, 2, "Tongan paʻanga", "T$"), // (sometimes PT)
    //         new Currency("TRY", 949, 2, "Turkish lira", "₺"),
    //         new Currency("TTD", 780, 2, "Trinidad and Tobago dollar", "$"), // or TT$
    //         new Currency("TWD", 901, 2, "New Taiwan dollar", "NT$"), // or $
    //         new Currency("TZS", 834, 2, "Tanzanian shilling", "x/y"), // or TSh
    //         new Currency("UAH", 980, 2, "Ukrainian hryvnia", "₴"),
    //         new Currency("UGX", 800, 0, "Ugandan shilling", "USh"),
    //         new Currency("USD", 840, 2, "United States dollar", "$"), // or US$
    //         new Currency("USN", 997, 2, "United States dollar (next day) (funds code)", "$"),
    //         new Currency("UYI", 940, 0, "Uruguay Peso en Unidades Indexadas (UI) (funds code)", Currency.GenericCurrencySign), // List two
    //         new Currency("UYU", 858, 2, "Uruguayan peso", "$"), // or $U
    //         new Currency("UZS", 860, 2, "Uzbekistan som", "лв"), // or сўм ?
    //         new Currency("VES", 928, 2, "Venezuelan Bolívar Soberano", "Bs.", validFrom: new DateTime(2018, 8, 20)), // or Bs.F. , Amendment 167 talks about delay but from multiple sources on the web the date seems to be 20 aug.
    //         new Currency("VND", 704, 0, "Vietnamese dong", "₫"),
    //         new Currency("VUV", 548, 0, "Vanuatu vatu", "VT"),
    //         new Currency("WST", 882, 2, "Samoan tala", "WS$"), // sometimes SAT, ST or T
    //         new Currency("XAF", 950, 0, "CFA franc BEAC", "FCFA"),
    //         new Currency("XAG", 961, NotApplicableByte, "Silver (one troy ounce)", Currency.GenericCurrencySign),
    //         new Currency("XAU", 959, NotApplicableByte, "Gold (one troy ounce)", Currency.GenericCurrencySign),
    //         new Currency("XBA", 955, NotApplicableByte, "European Composite Unit (EURCO) (bond market unit)", Currency.GenericCurrencySign),
    //         new Currency("XBB", 956, NotApplicableByte, "European Monetary Unit (E.M.U.-6) (bond market unit)", Currency.GenericCurrencySign),
    //         new Currency("XBC", 957, NotApplicableByte, "European Unit of Account 9 (E.U.A.-9) (bond market unit)", Currency.GenericCurrencySign),
    //         new Currency("XBD", 958, NotApplicableByte, "European Unit of Account 17 (E.U.A.-17) (bond market unit)", Currency.GenericCurrencySign),
    //         new Currency("XCD", 951, 2, "East Caribbean dollar", "$"), // or EC$
    //         new Currency("XDR", 960, NotApplicableByte, "Special drawing rights", Currency.GenericCurrencySign),
    //         new Currency("XOF", 952, 0, "CFA franc BCEAO", "CFA"),
    //         new Currency("XPD", 964, NotApplicableByte, "Palladium (one troy ounce)", Currency.GenericCurrencySign),
    //         new Currency("XPF", 953, 0, "CFP franc", "F"),
    //         new Currency("XPT", 962, NotApplicableByte, "Platinum (one troy ounce)", Currency.GenericCurrencySign),
    //         new Currency("XSU", 994, NotApplicableByte, "SUCRE", Currency.GenericCurrencySign),
    //         new Currency("XTS", 963, NotApplicableByte, "Code reserved for testing purposes", Currency.GenericCurrencySign),
    //         new Currency("XUA", 965, NotApplicableByte, "ADB Unit of Account", Currency.GenericCurrencySign),
    //         new Currency("XXX", 999, NotApplicableByte, "No currency", Currency.GenericCurrencySign),
    //         new Currency("YER", 886, 2, "Yemeni rial", "﷼"), // or ر.ي.‏‏ ?
    //         new Currency("ZAR", 710, 2, "South African rand", "R"),
    //         new Currency("ZMW", 967, 2, "Zambian kwacha", "ZK"), // or ZMW
    //         new Currency("ZWL", 932, 2, "Zimbabwean dollar", "$"),
    //         new Currency("STN", 930, 2, "Dobra", "Db", validFrom: new DateTime(2018, 1, 1)), // New Currency of São Tomé and Príncipe from 1 Jan 2018 (Amendment 164)
    //         new Currency("STD", 678, 2, "Dobra", "Db", validTo: new DateTime(2018, 1, 1)), // To be replaced Currency of São Tomé and Príncipe from 1 Jan 2018 (Amendment 164),  inflation has rendered the cêntimo obsolete
    //         new Currency("UYW", 927, 4, "Unidad Previsional", "Db", validFrom: new DateTime(2018, 8, 29)), // The Central Bank of Uruguay is applying for new Fund currency code (Amendment 169)
    //         new Currency("VED", 926, 2, "Venezuelan Bolívar Soberano", "Bs.", validFrom: new DateTime(2021, 10, 01)),
    //         new Currency("SLE", 925, 2, "Sierra Leonean leone", "Le", validFrom: new DateTime(2021, 04, 01)), // replaces SLL/694
    //         new Currency("ANG", 532, 2, "Netherlands Antillean guilder", "ƒ", validTo: new DateTime(2025, 03, 31)), // Amendment 176, replaced by XCG/532
    //         // new Currency("XCG", 532, 2, "Caribbean Guilder", "ƒ", validFrom: new DateTime(2025, 03, 31)), // Amendment 176, replaces ANG/532 => Activate 31 March 2025
    //
    //         // Historic ISO-4217 currencies (list three)
    //         new Currency("HRK", 191, 2, "Croatian kuna", "kn", Iso4217Historic, validTo: new DateTime(2022, 12, 31)),
    //         new Currency("SLL", 694, 2, "Sierra Leonean leone", "Le", Iso4217Historic, validTo: new DateTime(2022, 9, 30)), // replaced by SLE/925, redenominated by removing three (3) zeros from the denominations
    //         new Currency("BYR", 974, 0, "Belarusian ruble", "Br", Iso4217Historic, validTo: new DateTime(2016, 12, 31), validFrom: new DateTime(2000, 01, 01)),
    //         new Currency("VEF", 937, 2, "Venezuelan bolívar", "Bs.", Iso4217Historic, new DateTime(2018, 8, 20)), // replaced by VEF, The conversion rate is 1000 (old) Bolívar to 1 (new) Bolívar Soberano (1000:1). The expiration date of the current bolívar will be defined later and communicated by the Central Bank of Venezuela in due time.
    //         new Currency("MRO", 478, Z07Byte, "Mauritanian ouguiya", "UM", Iso4217Historic, new DateTime(2018, 1, 1)), // replaced by MRU
    //         new Currency("ESA", 996, NotApplicableByte, "Spanish peseta (account A)", "Pta", Iso4217Historic, new DateTime(2002, 3, 1)), // replaced by ESP (EUR)
    //         new Currency("ESB", 995, NotApplicableByte, "Spanish peseta (account B)", "Pta", Iso4217Historic, new DateTime(2002, 3, 1)), // replaced by ESP (EUR)
    //         new Currency("LTL", 440, 2, "Lithuanian litas", "Lt", Iso4217Historic, new DateTime(2014, 12, 31), new DateTime(1993, 1, 1)), // replaced by EUR
    //         new Currency("USS", 998, 2, "United States dollar (same day) (funds code)", "$", Iso4217Historic, new DateTime(2014, 3, 28)), // replaced by (no successor)
    //         new Currency("LVL", 428, 2, "Latvian lats", "Ls", Iso4217Historic, new DateTime(2013, 12, 31), new DateTime(1992, 1, 1)), // replaced by EUR
    //         new Currency("XFU", 0, NotApplicableByte, "UIC franc (special settlement currency) International Union of Railways", Currency.GenericCurrencySign, Iso4217Historic, new DateTime(2013, 11, 7)), // replaced by EUR
    //         new Currency("ZMK", 894, 2, "Zambian kwacha", "ZK", Iso4217Historic, new DateTime(2013, 1, 1), new DateTime(1968, 1, 16)), // replaced by ZMW
    //         new Currency("EEK", 233, 2, "Estonian kroon", "kr", Iso4217Historic, new DateTime(2010, 12, 31), new DateTime(1992, 1, 1)), // replaced by EUR
    //         new Currency("ZWR", 935, 2, "Zimbabwean dollar A/09", "$", Iso4217Historic, new DateTime(2009, 2, 2), new DateTime(2008, 8, 1)), // replaced by ZWL
    //         new Currency("SKK", 703, 2, "Slovak koruna", "Sk", Iso4217Historic, new DateTime(2008, 12, 31), new DateTime(1993, 2, 8)), // replaced by EUR
    //         new Currency("TMM", 795, 0, "Turkmenistani manat", "T", Iso4217Historic, new DateTime(2008, 12, 31), new DateTime(1993, 11, 1)), // replaced by TMT
    //         new Currency("ZWN", 942, 2, "Zimbabwean dollar A/08", "$", Iso4217Historic, new DateTime(2008, 7, 31), new DateTime(2006, 8, 1)), // replaced by ZWR
    //         new Currency("VEB", 862, 2, "Venezuelan bolívar", "Bs.", Iso4217Historic, new DateTime(2008, 1, 1)), // replaced by VEF
    //         new Currency("CYP", 196, 2, "Cypriot pound", "£", Iso4217Historic, new DateTime(2007, 12, 31), new DateTime(1879, 1, 1)), // replaced by EUR
    //         new Currency("MTL", 470, 2, "Maltese lira", "₤", Iso4217Historic, new DateTime(2007, 12, 31), new DateTime(1972, 1, 1)), // replaced by EUR
    //         new Currency("GHC", 288, 0, "Ghanaian cedi", "GH₵", Iso4217Historic, new DateTime(2007, 7, 1), new DateTime(1967, 1, 1)), // replaced by GHS
    //         new Currency("SDD", 736, NotApplicableByte, "Sudanese dinar", "£Sd", Iso4217Historic, new DateTime(2007, 1, 10), new DateTime(1992, 6, 8)), // replaced by SDG
    //         new Currency("SIT", 705, 2, "Slovenian tolar", Currency.GenericCurrencySign, Iso4217Historic, new DateTime(2006, 12, 31), new DateTime(1991, 10, 8)), // replaced by EUR
    //         new Currency("ZWD", 716, 2, "Zimbabwean dollar A/06", "$", Iso4217Historic, new DateTime(2006, 7, 31), new DateTime(1980, 4, 18)), // replaced by ZWN
    //         new Currency("MZM", 508, 0, "Mozambican metical", "MT", Iso4217Historic, new DateTime(2006, 6, 30), new DateTime(1980, 1, 1)), // replaced by MZN
    //         new Currency("AZM", 031, 0, "Azerbaijani manat", "₼", Iso4217Historic, new DateTime(2006, 1, 1), new DateTime(1992, 8, 15)), // replaced by AZN
    //         new Currency("CSD", 891, 2, "Serbian dinar", Currency.GenericCurrencySign, Iso4217Historic, new DateTime(2006, 12, 31), new DateTime(2003, 7, 3)), // replaced by RSD
    //         new Currency("MGF", 450, 2, "Malagasy franc", Currency.GenericCurrencySign, Iso4217Historic, new DateTime(2005, 1, 1), new DateTime(1963, 7, 1)), // replaced by MGA
    //         new Currency("ROL", 642, NotApplicableByte, "Romanian leu A/05", Currency.GenericCurrencySign, Iso4217Historic, new DateTime(2005, 12, 31), new DateTime(1952, 1, 28)), // replaced by RON
    //         new Currency("TRL", 792, 0, "Turkish lira A/05", "₺", Iso4217Historic, new DateTime(2005, 12, 31)), // replaced by TRY
    //         new Currency("SRG", 740, NotApplicableByte, "Suriname guilder", "ƒ", Iso4217Historic, new DateTime(2004, 12, 31)), // replaced by SRD
    //         new Currency("YUM", 891, 2, "Yugoslav dinar", "дин.", Iso4217Historic, new DateTime(2003, 7, 2), new DateTime(1994, 1, 24)), // replaced by CSD
    //         new Currency("AFA", 004, NotApplicableByte, "Afghan afghani", "؋", Iso4217Historic, new DateTime(2003, 12, 31), new DateTime(1925, 1, 1)), // replaced by AFN
    //         new Currency("XFO", 0, NotApplicableByte, "Gold franc (special settlement currency)", Currency.GenericCurrencySign, Iso4217Historic, new DateTime(2003, 12, 31), new DateTime(1803, 1, 1)), // replaced by XDR
    //         new Currency("GRD", 300, 2, "Greek drachma", "₯", Iso4217Historic, new DateTime(2000, 12, 31), new DateTime(1954, 1, 1)), // replaced by EUR
    //         new Currency("TJR", 762, NotApplicableByte, "Tajikistani ruble", Currency.GenericCurrencySign, Iso4217Historic, new DateTime(2000, 10, 30), new DateTime(1995, 5, 10)), // replaced by TJS
    //         new Currency("ECV", 983, NotApplicableByte, "Ecuador Unidad de Valor Constante (funds code)", Currency.GenericCurrencySign, Iso4217Historic, new DateTime(2000, 1, 9), new DateTime(1993, 1, 1)), // replaced by (no successor)
    //         new Currency("ECS", 218, 0, "Ecuadorian sucre", "S/.", Iso4217Historic, new DateTime(2000, 12, 31), new DateTime(1884, 1, 1)), // replaced by USD
    //         new Currency("BYB", 112, 2, "Belarusian ruble", "Br", Iso4217Historic, new DateTime(1999, 12, 31), new DateTime(1992, 1, 1)), // replaced by BYR
    //         new Currency("AOR", 982, 0, "Angolan kwanza readjustado", "Kz", Iso4217Historic, new DateTime(1999, 11, 30), new DateTime(1995, 7, 1)), // replaced by AOA
    //         new Currency("BGL", 100, 2, "Bulgarian lev A/99", "лв.", Iso4217Historic, new DateTime(1999, 7, 5), new DateTime(1962, 1, 1)), // replaced by BGN
    //         new Currency("ADF", 0, 2, "Andorran franc (1:1 peg to the French franc)", "Fr", Iso4217Historic, new DateTime(1998, 12, 31), new DateTime(1960, 1, 1)), // replaced by EUR
    //         new Currency("ADP", 020, 0, "Andorran peseta (1:1 peg to the Spanish peseta)", Currency.GenericCurrencySign, Iso4217Historic, new DateTime(1998, 12, 31), new DateTime(1869, 1, 1)), // replaced by EUR
    //         new Currency("ATS", 040, 2, "Austrian schilling", "öS", Iso4217Historic, new DateTime(1998, 12, 31), new DateTime(1945, 1, 1)), // replaced by EUR
    //         new Currency("BEF", 056, 2, "Belgian franc (currency union with LUF)", "fr.", Iso4217Historic, new DateTime(1998, 12, 31), new DateTime(1832, 1, 1)), // replaced by EUR
    //         new Currency("DEM", 276, 2, "German mark", "DM", Iso4217Historic, new DateTime(1998, 12, 31), new DateTime(1948, 1, 1)), // replaced by EUR
    //         new Currency("ESP", 724, 0, "Spanish peseta", "Pta", Iso4217Historic, new DateTime(1998, 12, 31), new DateTime(1869, 1, 1)), // replaced by EUR
    //         new Currency("FIM", 246, 2, "Finnish markka", "mk", Iso4217Historic, new DateTime(1998, 12, 31), new DateTime(1860, 1, 1)), // replaced by EUR
    //         new Currency("FRF", 250, 2, "French franc", "Fr", Iso4217Historic, new DateTime(1998, 12, 31), new DateTime(1960, 1, 1)), // replaced by EUR
    //         new Currency("IEP", 372, 2, "Irish pound (punt in Irish language)", "£", Iso4217Historic, new DateTime(1998, 12, 31), new DateTime(1938, 1, 1)), // replaced by EUR
    //         new Currency("ITL", 380, 0, "Italian lira", "₤", Iso4217Historic, new DateTime(1998, 12, 31), new DateTime(1861, 1, 1)), // replaced by EUR
    //         new Currency("LUF", 442, 2, "Luxembourg franc (currency union with BEF)", "fr.", Iso4217Historic, new DateTime(1998, 12, 31), new DateTime(1944, 1, 1)), // replaced by EUR
    //         new Currency("MCF", 0, 2, "Monegasque franc (currency union with FRF)", "fr.", Iso4217Historic, new DateTime(1998, 12, 31), new DateTime(1960, 1, 1)), // replaced by EUR
    //         new Currency("NLG", 528, 2, "Dutch guilder", "ƒ", Iso4217Historic, new DateTime(1998, 12, 31), new DateTime(1810, 1, 1)), // replaced by EUR
    //         new Currency("PTE", 620, 0, "Portuguese escudo", "$", Iso4217Historic, new DateTime(1998, 12, 31), new DateTime(4160, 1, 1)), // replaced by EUR
    //         new Currency("SML", 0, 0, "San Marinese lira (currency union with ITL and VAL)", "₤", Iso4217Historic, new DateTime(1998, 12, 31), new DateTime(1864, 1, 1)), // replaced by EUR
    //         new Currency("VAL", 0, 0, "Vatican lira (currency union with ITL and SML)", "₤", Iso4217Historic, new DateTime(1998, 12, 31), new DateTime(1929, 1, 1)), // replaced by EUR
    //         new Currency("XEU", 954, NotApplicableByte, "European Currency Unit (1 XEU = 1 EUR)", "ECU", Iso4217Historic, new DateTime(1998, 12, 31), new DateTime(1979, 3, 13)), // replaced by EUR
    //         new Currency("BAD", 0, 2, "Bosnia and Herzegovina dinar", Currency.GenericCurrencySign, Iso4217Historic, new DateTime(1998, 12, 31), new DateTime(1992, 7, 1)), // replaced by BAM
    //         new Currency("RUR", 810, 2, "Russian ruble A/97", "₽", Iso4217Historic, new DateTime(1997, 12, 31), new DateTime(1992, 1, 1)), // replaced by RUB
    //         new Currency("GWP", 624, NotApplicableByte, "Guinea-Bissau peso", Currency.GenericCurrencySign, Iso4217Historic, new DateTime(1997, 12, 31), new DateTime(1975, 1, 1)), // replaced by XOF
    //         new Currency("ZRN", 180, 2, "Zaïrean new zaïre", "Ƶ", Iso4217Historic, new DateTime(1997, 12, 31), new DateTime(1993, 1, 1)), // replaced by CDF
    //         new Currency("UAK", 804, NotApplicableByte, "Ukrainian karbovanets", Currency.GenericCurrencySign, Iso4217Historic, new DateTime(1996, 9, 1), new DateTime(1992, 10, 1)), // replaced by UAH
    //         new Currency("YDD", 720, NotApplicableByte, "South Yemeni dinar", Currency.GenericCurrencySign, Iso4217Historic, new DateTime(1996, 6, 11)), // replaced by YER
    //         new Currency("AON", 024, 0, "Angolan new kwanza", "Kz", Iso4217Historic, new DateTime(1995, 6, 30), new DateTime(1990, 9, 25)), // replaced by AOR
    //         new Currency("ZAL", 991, NotApplicableByte, "South African financial rand (funds code)", Currency.GenericCurrencySign, Iso4217Historic, new DateTime(1995, 3, 13), new DateTime(1985, 9, 1)), // replaced by (no successor)
    //         new Currency("PLZ", 616, NotApplicableByte, "Polish zloty A/94", "zł", Iso4217Historic, new DateTime(1994, 12, 31), new DateTime(1950, 10, 30)), // replaced by PLN
    //         new Currency("BRR", 0, 2, "Brazilian cruzeiro real", "CR$", Iso4217Historic, new DateTime(1994, 6, 30), new DateTime(1993, 8, 1)), // replaced by BRL
    //         new Currency("HRD", 0, NotApplicableByte, "Croatian dinar", Currency.GenericCurrencySign, Iso4217Historic, new DateTime(1994, 5, 30), new DateTime(1991, 12, 23)), // replaced by HRK
    //         new Currency("YUG", 0, 2, "Yugoslav dinar", "дин.", Iso4217Historic, new DateTime(1994, 1, 23), new DateTime(1994, 1, 1)), // replaced by YUM
    //         new Currency("YUO", 0, 2, "Yugoslav dinar", "дин.", Iso4217Historic, new DateTime(1993, 12, 31), new DateTime(1993, 10, 1)), // replaced by YUG
    //         new Currency("YUR", 0, 2, "Yugoslav dinar", "дин.", Iso4217Historic, new DateTime(1993, 9, 30), new DateTime(1992, 7, 1)), // replaced by YUO
    //         new Currency("BRE", 0, 2, "Brazilian cruzeiro", "₢", Iso4217Historic, new DateTime(1993, 8, 1), new DateTime(1990, 3, 15)), // replaced by BRR
    //         new Currency("UYN", 858, NotApplicableByte, "Uruguay Peso", "$U", Iso4217Historic, new DateTime(1993, 3, 1), new DateTime(1975, 7, 1)), // replaced by UYU
    //         new Currency("CSK", 200, NotApplicableByte, "Czechoslovak koruna", "Kčs", Iso4217Historic, new DateTime(1993, 2, 8), new DateTime(7040, 1, 1)), // replaced by CZK and SKK (CZK and EUR)
    //         new Currency("MKN", 0, NotApplicableByte, "Old Macedonian denar A/93", "ден", Iso4217Historic, new DateTime(1993, 12, 31)), // replaced by MKD
    //         new Currency("MXP", 484, NotApplicableByte, "Mexican peso", "$", Iso4217Historic, new DateTime(1993, 12, 31)), // replaced by MXN
    //         new Currency("ZRZ", 0, 3, "Zaïrean zaïre", "Ƶ", Iso4217Historic, new DateTime(1993, 12, 31), new DateTime(1967, 1, 1)), // replaced by ZRN
    //         new Currency("YUN", 0, 2, "Yugoslav dinar", "дин.", Iso4217Historic, new DateTime(1992, 6, 30), new DateTime(1990, 1, 1)), // replaced by YUR
    //         new Currency("SDP", 736, NotApplicableByte, "Sudanese old pound", "ج.س.", Iso4217Historic, new DateTime(1992, 6, 8), new DateTime(1956, 1, 1)), // replaced by SDD
    //         new Currency("ARA", 0, 2, "Argentine austral", "₳", Iso4217Historic, new DateTime(1991, 12, 31), new DateTime(1985, 6, 15)), // replaced by ARS
    //         new Currency("PEI", 0, NotApplicableByte, "Peruvian inti", "I/.", Iso4217Historic, new DateTime(1991, 10, 1), new DateTime(1985, 2, 1)), // replaced by PEN
    //         new Currency("SUR", 810, NotApplicableByte, "Soviet Union Ruble", "руб", Iso4217Historic, new DateTime(1991, 12, 31), new DateTime(1961, 1, 1)), // replaced by RUR
    //         new Currency("AOK", 024, 0, "Angolan kwanza", "Kz", Iso4217Historic, new DateTime(1990, 9, 24), new DateTime(1977, 1, 8)), // replaced by AON
    //         new Currency("DDM", 278, NotApplicableByte, "East German Mark of the GDR (East Germany)", Currency.GenericCurrencySign, Iso4217Historic, new DateTime(1990, 7, 1), new DateTime(1948, 6, 21)), // replaced by DEM (EUR)
    //         new Currency("BRN", 0, 2, "Brazilian cruzado novo", "NCz$", Iso4217Historic, new DateTime(1990, 3, 15), new DateTime(1989, 1, 16)), // replaced by BRE
    //         new Currency("YUD", 891, 2, "New Yugoslavian Dinar", "дин.", Iso4217Historic, new DateTime(1989, 12, 31), new DateTime(1966, 1, 1)), // replaced by YUN
    //         new Currency("BRC", 0, 2, "Brazilian cruzado", "Cz$", Iso4217Historic, new DateTime(1989, 1, 15), new DateTime(1986, 2, 28)), // replaced by BRN
    //         new Currency("BOP", 068, 2, "Peso boliviano", "b$.", Iso4217Historic, new DateTime(1987, 1, 1), new DateTime(1963, 1, 1)), // replaced by BOB
    //         new Currency("UGS", 800, NotApplicableByte, "Ugandan shilling A/87", "USh", Iso4217Historic, new DateTime(1987, 12, 31)), // replaced by UGX
    //         new Currency("BRB", 076, 2, "Brazilian cruzeiro", "₢", Iso4217Historic, new DateTime(1986, 2, 28), new DateTime(1970, 1, 1)), // replaced by BRC
    //         new Currency("ILR", 376, 2, "Israeli shekel", Currency.GenericCurrencySign, Iso4217Historic, new DateTime(1985, 12, 31), new DateTime(1980, 2, 24)), // replaced by ILS
    //         new Currency("ARP", 0, 2, "Argentine peso argentino", "$a", Iso4217Historic, new DateTime(1985, 6, 14), new DateTime(1983, 6, 6)), // replaced by ARA
    //         new Currency("PEH", 604, NotApplicableByte, "Peruvian old sol", Currency.GenericCurrencySign, Iso4217Historic, new DateTime(1985, 2, 1), new DateTime(1863, 1, 1)), // replaced by PEI
    //         new Currency("GQE", 0, NotApplicableByte, "Equatorial Guinean ekwele", Currency.GenericCurrencySign, Iso4217Historic, new DateTime(1985, 12, 31), new DateTime(1975, 1, 1)), // replaced by XAF
    //         new Currency("GNE", 324, NotApplicableByte, "Guinean syli", Currency.GenericCurrencySign, Iso4217Historic, new DateTime(1985, 12, 31), new DateTime(1971, 1, 1)), // replaced by GNF
    //         new Currency("MLF", 0, NotApplicableByte, "Mali franc", "MAF", Iso4217Historic, new DateTime(1984, 12, 31)), // replaced by XOF
    //         new Currency("ARL", 0, 2, "Argentine peso ley", "$L", Iso4217Historic, new DateTime(1983, 5, 5), new DateTime(1970, 1, 1)), // replaced by ARP
    //         new Currency("ISJ", 352, 2, "Icelandic krona", "kr", Iso4217Historic, new DateTime(1981, 12, 31), new DateTime(1922, 1, 1)), // replaced by ISK
    //         new Currency("MVQ", 462, NotApplicableByte, "Maldivian rupee", "Rf", Iso4217Historic, new DateTime(1981, 12, 31)), // replaced by MVR
    //         new Currency("ILP", 376, 3, "Israeli lira", "I£", Iso4217Historic, new DateTime(1980, 12, 31), new DateTime(1948, 1, 1)), // ISRAEL Pound,  replaced by ILR
    //         new Currency("ZWC", 716, 2, "Rhodesian dollar", "$", Iso4217Historic, new DateTime(1980, 12, 31), new DateTime(1970, 2, 17)), // replaced by ZWD
    //         new Currency("LAJ", 418, NotApplicableByte, "Pathet Lao Kip", "₭", Iso4217Historic, new DateTime(1979, 12, 31)), // replaced by LAK
    //         new Currency("TPE", 0, NotApplicableByte, "Portuguese Timorese escudo", Currency.GenericCurrencySign, Iso4217Historic, new DateTime(1976, 12, 31), new DateTime(1959, 1, 1)), // replaced by IDR
    //         new Currency("UYP", 858, NotApplicableByte, "Uruguay Peso", "$", Iso4217Historic, new DateTime(1975, 7, 1), new DateTime(1896, 1, 1)), // replaced by UYN
    //         new Currency("CLE", 0, NotApplicableByte, "Chilean escudo", "Eº", Iso4217Historic, new DateTime(1975, 12, 31), new DateTime(1960, 1, 1)), // replaced by CLP
    //         new Currency("MAF", 0, NotApplicableByte, "Moroccan franc", Currency.GenericCurrencySign, Iso4217Historic, new DateTime(1976, 12, 31), new DateTime(1921, 1, 1)), // replaced by MAD
    //         new Currency("PTP", 0, NotApplicableByte, "Portuguese Timorese pataca", Currency.GenericCurrencySign, Iso4217Historic, new DateTime(1958, 12, 31), new DateTime(1894, 1, 1)), // replaced by TPE
    //         new Currency("TNF", 0, 2, "Tunisian franc", "F", Iso4217Historic, new DateTime(1958, 12, 31), new DateTime(1991, 7, 1)), // replaced by TND
    //         new Currency("NFD", 0, 2, "Newfoundland dollar", "$", Iso4217Historic, new DateTime(1949, 12, 31), new DateTime(1865, 1, 1)), // replaced by CAD
    //
    //         // Added historic currencies of amendment 164 (research dates and other info)
    //         new Currency("VNC", 704, 2, "Old Dong", "₫", Iso4217Historic, new DateTime(2014, 1, 1)), // VIETNAM, replaced by VND with same number! Formerly, it was subdivided into 10 hào.
    //         new Currency("GNS", 324, NotApplicableByte, "Guinean Syli", Currency.GenericCurrencySign, Iso4217Historic, new DateTime(1970, 12, 31)), // GUINEA, replaced by GNE?
    //         new Currency("UGW", 800, NotApplicableByte, "Old Shilling", Currency.GenericCurrencySign, Iso4217Historic, new DateTime(2017, 9, 22)), // UGANDA
    //         new Currency("RHD", 716, NotApplicableByte, "Rhodesian Dollar", Currency.GenericCurrencySign, Iso4217Historic, new DateTime(2017, 9, 22)), // SOUTHERN RHODESIA
    //         new Currency("ROK", 642, NotApplicableByte, "Leu A/52", Currency.GenericCurrencySign, Iso4217Historic, new DateTime(2017, 9, 22)), // ROMANIA
    //         new Currency("NIC", 558, NotApplicableByte, "Cordoba", Currency.GenericCurrencySign, Iso4217Historic, new DateTime(2017, 9, 22)), // NICARAGUA
    //         new Currency("MZE", 508, NotApplicableByte, "Mozambique Escudo", Currency.GenericCurrencySign, Iso4217Historic, new DateTime(2017, 9, 22)), // MOZAMBIQUE
    //         new Currency("MTP", 470, NotApplicableByte, "Maltese Pound", Currency.GenericCurrencySign, Iso4217Historic, new DateTime(2017, 9, 22)), // MALTA
    //         new Currency("LSM", 426, NotApplicableByte, "Loti", Currency.GenericCurrencySign, Iso4217Historic, new DateTime(2017, 9, 22)), // LESOTHO
    //         new Currency("GWE", 624, NotApplicableByte, "Guinea Escudo", Currency.GenericCurrencySign, Iso4217Historic, new DateTime(2017, 9, 22)), // GUINEA-BISSAU
    //         new Currency("CSJ", 203, NotApplicableByte, "Krona A/53", Currency.GenericCurrencySign, Iso4217Historic, new DateTime(2017, 9, 22)), // CZECHOSLOVAKIA
    //         new Currency("BUK", 104, NotApplicableByte, "Kyat", Currency.GenericCurrencySign, Iso4217Historic, new DateTime(2017, 9, 22)), // BURMA
    //         new Currency("BGK", 100, NotApplicableByte, "Lev A / 62", Currency.GenericCurrencySign, Iso4217Historic, new DateTime(2017, 9, 22)), // BULGARIA
    //         new Currency("BGJ", 100, NotApplicableByte, "Lev A / 52", Currency.GenericCurrencySign, Iso4217Historic, new DateTime(2017, 9, 22)), // BULGARIA
    //         new Currency("ARY", 032, NotApplicableByte, "Peso", Currency.GenericCurrencySign, Iso4217Historic, new DateTime(2017, 9, 22)), // ARGENTINA
    //     };
    // }
}
