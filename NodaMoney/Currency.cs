using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;

namespace NodaMoney
{
    /// <summary>A unit of exchange, a currency of <see cref="Money" />.</summary>
    /// <remarks>See <see cref="http://en.wikipedia.org/wiki/Currency"/>.</remarks>
    [DataContract]
    [DebuggerDisplay("{Code}")]
    public struct Currency : IEquatable<Currency>
    {
        // The Malagasy ariary and the Mauritanian ouguiya are technically divided into five subunits (the iraimbilanja and
        // khoum respectively), rather than by a power of ten. The coins display "1/5" on their face and are referred to as
        // a "fifth" (Khoum/cinquième). These are not used in practice, but when written out, a single significant digit is
        // used. E.g. 1.2 UM.
        // To represent this in decimal we do the following steps: 5 is 10 to the power of log(5) = 0.69897... ~ 0.7
        internal const double Z07 = 0.69897000433601880478626110527551; // Math.Log10(5);
        internal const double DOT = -1;
        private static readonly Dictionary<string, Currency> Currencies = InitializeCurrencies();

        internal Currency(string code, string number, double decimalDigits, string englishName, string sign)
            : this()
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentNullException("code");
            if (string.IsNullOrWhiteSpace(number))
                throw new ArgumentNullException("number");
            if (string.IsNullOrWhiteSpace(englishName)) 
                throw new ArgumentNullException("englishName");
            if (string.IsNullOrWhiteSpace(sign)) 
                throw new ArgumentNullException("sign");
            if (decimalDigits < -1 || decimalDigits > 3) 
                throw new ArgumentOutOfRangeException("code", "DecimalDigits must be between -1 and 3!");

            Code = code;
            Number = number;            
            DecimalDigits = decimalDigits;
            EnglishName = englishName;
            Sign = sign;
        }

        /// <summary>Gets the Currency that represents the country/region used by the current thread.</summary>
        /// <value>The Currency that represents the country/region used by the current thread.</value>
        public static Currency CurrentCurrency
        {
            get { return Currency.FromRegion(RegionInfo.CurrentRegion); }
        }   

        /// <summary>Gets the currency sign.</summary>
        public string Sign { get; private set; }

        /// <summary>Gets the english name of the currency.</summary>
        public string EnglishName { get; private set; }

        /// <summary>Gets the three-character ISO 4217 currency code.</summary>
        [DataMember]
        public string Code { get; private set; }

        /// <summary>Gets the numeric ISO 4217 currency code.</summary>
        public string Number { get; private set; }

        /// <summary>Gets the number of digits after the decimal separator.</summary>
        /// <remarks>
        /// <para>
        /// For example, the default number of fraction digits for the Euro is 2, while for the Japanese Yen it's 0. In the
        /// case of pseudo-currencies, such as IMF Special Drawing Rights, -1 is returned.
        /// </para>
        /// <para>
        /// The Malagasy ariary and the Mauritanian ouguiya are divided into five subunits (the iraimbilanja and khoum respectively)
        /// rather than by a power of ten. 5 is 10 to the power of log(5) = 0.69897... ~ 0.7
        /// </para>
        /// </remarks>
        public double DecimalDigits { get; private set; }

        /// <summary>Gets the major currency unit.</summary>
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Member of Currency type!")]
        public decimal MajorUnit
        {
            get { return 1; }
        }

        /// <summary>Gets the minor currency unit.</summary>
        public decimal MinorUnit
        {
            get 
            {
                if (DecimalDigits == DOT)
                    return MajorUnit;

                return new Decimal(1 / Math.Pow(10, DecimalDigits));
            }
        }

        /// <summary>Create an instance of the <see cref="Currency"/>, based on a ISO 4217 currency code.</summary>
        /// <param name="code">A ISO 4217 currency code, like EUR or USD.</param>
        /// <returns>An instance of the type <see cref="Currency"/>.</returns>
        public static Currency FromCode(string code)
        {
            if (string.IsNullOrWhiteSpace(code)) 
                throw new ArgumentNullException("code");
            if (!Currencies.ContainsKey(code.ToUpperInvariant())) 
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "{0} is an unknown ISO 4217 currency code!", code));

            return Currencies[code.ToUpperInvariant()];
        }

        /// <summary>Creates an instance of the <see cref="Currency"/> used within the specified <see cref="RegionInfo"/>.</summary>
        /// <param name="region"><see cref="RegionInfo"/> to get a <see cref="Currency"/> for.</param>
        /// <returns>The <see cref="Currency"/> instance used within the specified <see cref="RegionInfo"/>.</returns>
        public static Currency FromRegion(RegionInfo region)
        {
            if (region == null) 
                throw new ArgumentNullException("region");

            return FromCode(region.ISOCurrencySymbol);
        }

        /// <summary>Creates an instance of the <see cref="Currency"/> used within the specified <see cref="CultureInfo"/>.</summary>
        /// <param name="culture"><see cref="CultureInfo"/> to get a <see cref="Currency"/> for.</param>
        /// <returns>The <see cref="Currency"/> instance used within the specified <see cref="CultureInfo"/>.</returns>
        public static Currency FromCulture(CultureInfo culture)
        {
            if (culture == null) 
                throw new ArgumentNullException("culture");
            if (culture.IsNeutralCulture) 
                throw new ArgumentException("Culture {0} is a neutral culture, from which no region information can be extracted!", culture.Name);
            
            return FromRegion(culture.Name);
        }     

        /// <summary>Creates an instance of the <see cref="Currency"/> used within the specified name of the region or culture.</summary>
        /// <param name="name">
        /// <para>A string that contains a two-letter code defined in ISO 3166 for country/region.</para>
        /// <para>-or-</para>
        /// <para>A string that contains the culture name for a specific culture, custom culture, or Windows-only culture. If the 
        /// culture name is not in RFC 4646 format, your application should specify the entire culture name instead of just the
        /// country/region. See also http://msdn.microsoft.com/en-us/library/atwc2921.aspx. </para> 
        /// </param>
        /// <returns>The <see cref="Currency"/> instance used within the specified region.</returns>
        public static Currency FromRegion(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) 
                throw new ArgumentNullException("name");            

            return FromRegion(new RegionInfo(name));
        }

        /// <summary>Get all currencies.</summary>
        /// <returns>An array that contains all the ISO 4217 currencies. The array of currencies is unsorted.</returns>
        public static Currency[] GetAllCurrencies()
        {
            // TODO: IQueryable?
            return Currencies.Values.ToArray();
        }

        /// <summary>Implements the operator ==.</summary>
        /// <param name="left">The left Currency.</param>
        /// <param name="right">The right Currency.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(Currency left, Currency right)
        {
            return left.Equals(right);
        }

        /// <summary>Implements the operator ==.</summary>
        /// <param name="left">The left Currency.</param>
        /// <param name="right">The right Currency.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(Currency left, Currency right)
        {
            return !(left == right);
        }

        /// <summary>Returns a value indicating whether two specified instances of <see cref="Currency"/> represent the same value.</summary>
        /// <param name="currency1">The first <see cref="Currency"/> object.</param>
        /// <param name="currency2">The second <see cref="Currency"/> object.</param>
        /// <returns>true if currency1 and currency2 are equal to this instance; otherwise, false.</returns>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Calling override method")]
        public static bool Equals(Currency currency1, Currency currency2)
        {
            return currency1.Equals(currency2);
        }

        /// <summary>Returns a value indicating whether this instance and a specified <see cref="Object"/> represent the same type and value.</summary>
        /// <param name="obj">An <see cref="Object"/>.</param>
        /// <returns>true if value is equal to this instance; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            return (obj is Currency) && Equals((Currency)obj);
        }

        /// <summary>Returns a value indicating whether this instance and a specified <see cref="Currency"/> object represent the same value.</summary>
        /// <param name="other">A <see cref="Currency"/> object.</param>
        /// <returns>true if value is equal to this instance; otherwise, false.</returns>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Calling override method")]
        public bool Equals(Currency other)
        {
            return Equals(this.Code, other.Code);
        }

        /// <summary>Returns the hash code for this instance.</summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode()
        {
            return Code.GetHashCode();
        }
        
        ///// <summary>Gets the name of the currency, formatted in the native language of the country\region where the currency is used.</summary>
        ///// <param name="culture">The culture.</param>
        ///// <returns>The native name of the currency.</returns>
        ////public string GetNativeName(CultureInfo culture)
        ////{
        ////    //Require.That(culture, "culture").IsNotNull();
        ////    RegionInfo region = new RegionInfo(culture.Name);
        ////    if (!region.ISOCurrencySymbol.Equals(Code))
        ////    {
        ////        throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Resources.NativeNameDoesNotExistForCulture, Code, culture));
        ////    }
        ////    return region.CurrencyNativeName;
        ////}

        private static Dictionary<string, Currency> InitializeCurrencies()
        {
            var currencies = new Dictionary<string, Currency>()
            {
                { "AED", new Currency("AED", "784", 2, "United Arab Emirates dirham", "¤") },
                { "AFN", new Currency("AFN", "971", 2, "Afghan afghani", "¤") },
                { "ALL", new Currency("ALL", "008", 2, "Albanian lek", "¤") },
                { "AMD", new Currency("AMD", "051", 2, "Armenian dram", "¤") },
                { "ANG", new Currency("ANG", "532", 2, "Netherlands Antillean guilder", "ƒ") },
                { "AOA", new Currency("AOA", "973", 2, "Angolan kwanza", "Kz") },
                { "ARS", new Currency("ARS", "032", 2, "Argentine peso", "$") },
                { "AUD", new Currency("AUD", "036", 2, "Australian dollar", "$") },
                { "AWG", new Currency("AWG", "533", 2, "Aruban florin", "ƒ") },
                { "AZN", new Currency("AZN", "944", 2, "Azerbaijani manat", "m") },
                { "BAM", new Currency("BAM", "977", 2, "Bosnia and Herzegovina convertible mark", "KM") },
                { "BBD", new Currency("BBD", "052", 2, "Barbados dollar", "Bds$") },
                { "BDT", new Currency("BDT", "050", 2, "Bangladeshi taka", "¤") },
                { "BGN", new Currency("BGN", "975", 2, "Bulgarian lev", "¤") },
                { "BHD", new Currency("BHD", "048", 3, "Bahraini dinar", "BD") },
                { "BIF", new Currency("BIF", "108", 0, "Burundian franc", "Fbu") },
                { "BMD", new Currency("BMD", "060", 2, "Bermudian dollar (customarily known as Bermuda dollar)", "BD$") },
                { "BND", new Currency("BND", "096", 2, "Brunei dollar", "B$") },
                { "BOB", new Currency("BOB", "068", 2, "Boliviano", "Bs.") },
                { "BOV", new Currency("BOV", "984", 2, "Bolivian Mvdol (funds code)", "¤") },
                { "BRL", new Currency("BRL", "986", 2, "Brazilian real", "R$") },
                { "BSD", new Currency("BSD", "044", 2, "Bahamian dollar", "B$") },
                { "BTN", new Currency("BTN", "064", 2, "Bhutanese ngultrum", "Nu.") },
                { "BWP", new Currency("BWP", "072", 2, "Botswana pula", "P") },
                { "BYR", new Currency("BYR", "974", 0, "Belarusian ruble", "Br") },
                { "BZD", new Currency("BZD", "084", 2, "Belize dollar", "BZ$") },
                { "CAD", new Currency("CAD", "124", 2, "Canadian dollar", "$") },
                { "CDF", new Currency("CDF", "976", 2, "Congolese franc", "F") },
                { "CHE", new Currency("CHE", "947", 2, "WIR Euro (complementary currency)", "¤") },
                { "CHF", new Currency("CHF", "756", 2, "Swiss franc", "CHF") },
                { "CHW", new Currency("CHW", "948", 2, "WIR Franc (complementary currency)", "¤") },
                { "CLF", new Currency("CLF", "990", 0, "Unidad de Fomento (funds code)", "¤") },
                { "CLP", new Currency("CLP", "152", 0, "Chilean peso", "$") },
                { "CNY", new Currency("CNY", "156", 2, "Chinese yuan", "¥") },
                { "COP", new Currency("COP", "170", 2, "Colombian peso", "$") },
                { "COU", new Currency("COU", "970", 2, "Unidad de Valor Real", "¤") },
                { "CRC", new Currency("CRC", "188", 2, "Costa Rican colon", "¢") },
                { "CUC", new Currency("CUC", "931", 2, "Cuban convertible peso", "¤") },
                { "CUP", new Currency("CUP", "192", 2, "Cuban peso", "$") },
                { "CVE", new Currency("CVE", "132", 0, "Cape Verde escudo", "$") },
                { "CZK", new Currency("CZK", "203", 2, "Czech koruna", "Kc") },
                { "DJF", new Currency("DJF", "262", 0, "Djiboutian franc", "Fdj") },
                { "DKK", new Currency("DKK", "208", 2, "Danish krone", "kr") },
                { "DOP", new Currency("DOP", "214", 2, "Dominican peso", "RD$") },
                { "DZD", new Currency("DZD", "012", 2, "Algerian dinar", "DA") },
                { "EGP", new Currency("EGP", "818", 2, "Egyptian pound", "LE") },
                { "ERN", new Currency("ERN", "232", 2, "Eritrean nakfa", "Nfk") },
                { "ETB", new Currency("ETB", "230", 2, "Ethiopian birr", "Br") },
                { "EUR", new Currency("EUR", "978", 2, "Euro", "€") },
                { "FJD", new Currency("FJD", "242", 2, "Fiji dollar", "FJ$") },
                { "FKP", new Currency("FKP", "238", 2, "Falkland Islands pound", "£") },
                { "GBP", new Currency("GBP", "826", 2, "Pound sterling", "£") },
                { "GEL", new Currency("GEL", "981", 2, "Georgian lari", "¤") },
                { "GHS", new Currency("GHS", "936", 2, "Ghanaian cedi", "¤") },
                { "GIP", new Currency("GIP", "292", 2, "Gibraltar pound", "£") },
                { "GMD", new Currency("GMD", "270", 2, "Gambian dalasi", "D") },
                { "GNF", new Currency("GNF", "324", 0, "Guinean franc", "FG") },
                { "GTQ", new Currency("GTQ", "320", 2, "Guatemalan quetzal", "Q") },
                { "GYD", new Currency("GYD", "328", 2, "Guyanese dollar", "$") },
                { "HKD", new Currency("HKD", "344", 2, "Hong Kong dollar", "HK$") },
                { "HNL", new Currency("HNL", "340", 2, "Honduran lempira", "L") },
                { "HRK", new Currency("HRK", "191", 2, "Croatian kuna", "kn") },
                { "HTG", new Currency("HTG", "332", 2, "Haitian gourde", "G") },
                { "HUF", new Currency("HUF", "348", 2, "Hungarian forint", "Ft") },
                { "IDR", new Currency("IDR", "360", 2, "Indonesian rupiah", "Rp") },
                { "ILS", new Currency("ILS", "376", 2, "Israeli new shekel", "₪") },
                { "INR", new Currency("INR", "356", 2, "Indian rupee", "Rp") },
                { "IQD", new Currency("IQD", "368", 3, "Iraqi dinar", "¤") },
                { "IRR", new Currency("IRR", "364", 0, "Iranian rial", "¤") },
                { "ISK", new Currency("ISK", "352", 0, "Icelandic króna", "kr") },
                { "JMD", new Currency("JMD", "388", 2, "Jamaican dollar", "$") },
                { "JOD", new Currency("JOD", "400", 3, "Jordanian dinar", "¤") },
                { "JPY", new Currency("JPY", "392", 0, "Japanese yen", "¥") },
                { "KES", new Currency("KES", "404", 2, "Kenyan shilling", "KSh") },
                { "KGS", new Currency("KGS", "417", 2, "Kyrgyzstani som", "¤") },
                { "KHR", new Currency("KHR", "116", 2, "Cambodian riel", "¤") },
                { "KMF", new Currency("KMF", "174", 0, "Comoro franc", "¤") },
                { "KPW", new Currency("KPW", "408", 0, "North Korean won", "₩") },
                { "KRW", new Currency("KRW", "410", 0, "South Korean won", "₩") },
                { "KWD", new Currency("KWD", "414", 3, "Kuwaiti dinar", "¤") },
                { "KYD", new Currency("KYD", "136", 2, "Cayman Islands dollar", "$") },
                { "KZT", new Currency("KZT", "398", 2, "Kazakhstani tenge", "¤") },
                { "LAK", new Currency("LAK", "418", 0, "Lao kip", "¤") },
                { "LBP", new Currency("LBP", "422", 0, "Lebanese pound", "¤") },
                { "LKR", new Currency("LKR", "144", 2, "Sri Lankan rupee", "Rs") },
                { "LRD", new Currency("LRD", "430", 2, "Liberian dollar", "L$") },
                { "LSL", new Currency("LSL", "426", 2, "Lesotho loti", "¤") },
                { "LTL", new Currency("LTL", "440", 2, "Lithuanian litas", "Lt") },
                // { "LVL", new Currency("LVL", "428", 2, "Latvian lats", "Ls") }, // Until 2014-01-15, replaced by EUR
                { "LYD", new Currency("LYD", "434", 3, "Libyan dinar", "LD") },
                { "MAD", new Currency("MAD", "504", 2, "Moroccan dirham", "¤") },
                { "MDL", new Currency("MDL", "498", 2, "Moldovan leu", "¤") },
                { "MGA", new Currency("MGA", "969", Z07, "Malagasy ariary", "Ar") },  // divided into five subunits rather than by a power of ten. 5 is 10 to the power of 0.69897...
                { "MKD", new Currency("MKD", "807", 0, "Macedonian denar", "¤") },
                { "MMK", new Currency("MMK", "104", 0, "Myanma kyat", "K") },
                { "MNT", new Currency("MNT", "496", 2, "Mongolian tugrik", "¤") },
                { "MOP", new Currency("MOP", "446", 2, "Macanese pataca", "MOP$") },
                { "MRO", new Currency("MRO", "478", Z07, "Mauritanian ouguiya", "UM") }, // divided into five subunits rather than by a power of ten. 5 is 10 to the power of 0.69897...
                { "MUR", new Currency("MUR", "480", 2, "Mauritian rupee", "Rs") },
                { "MVR", new Currency("MVR", "462", 2, "Maldivian rufiyaa", "Rf") },
                { "MWK", new Currency("MWK", "454", 2, "Malawian kwacha", "MK") },
                { "MXN", new Currency("MXN", "484", 2, "Mexican peso", "$") },
                { "MXV", new Currency("MXV", "979", 2, "Mexican Unidad de Inversion (UDI) (funds code)", "¤") },
                { "MYR", new Currency("MYR", "458", 2, "Malaysian ringgit", "RM") },
                { "MZN", new Currency("MZN", "943", 2, "Mozambican metical", "MTn") },
                { "NAD", new Currency("NAD", "516", 2, "Namibian dollar", "N$") },
                { "NGN", new Currency("NGN", "566", 2, "Nigerian naira", "₦") },
                { "NIO", new Currency("NIO", "558", 2, "Nicaraguan córdoba", "C$") },
                { "NOK", new Currency("NOK", "578", 2, "Norwegian krone", "kr") },
                { "NPR", new Currency("NPR", "524", 2, "Nepalese rupee", "Rs") },
                { "NZD", new Currency("NZD", "554", 2, "New Zealand dollar", "$") },
                { "OMR", new Currency("OMR", "512", 3, "Omani rial", "¤") },
                { "PAB", new Currency("PAB", "590", 2, "Panamanian balboa", "B/.") },
                { "PEN", new Currency("PEN", "604", 2, "Peruvian nuevo sol", "S/.") },
                { "PGK", new Currency("PGK", "598", 2, "Papua New Guinean kina", "K") },
                { "PHP", new Currency("PHP", "608", 2, "Philippine peso", "P") },
                { "PKR", new Currency("PKR", "586", 2, "Pakistani rupee", "Rs.") },
                { "PLN", new Currency("PLN", "985", 2, "Polish złoty", "zl") },
                { "PYG", new Currency("PYG", "600", 0, "Paraguayan guaraní", "¤") },
                { "QAR", new Currency("QAR", "634", 2, "Qatari riyal", "QR") },
                { "RON", new Currency("RON", "946", 2, "Romanian new leu", "L") },
                { "RSD", new Currency("RSD", "941", 2, "Serbian dinar", "RSD") },
                { "RUB", new Currency("RUB", "643", 2, "Russian rouble", "PP") },
                { "RWF", new Currency("RWF", "646", 0, "Rwandan franc", "RF") },
                { "SAR", new Currency("SAR", "682", 2, "Saudi riyal", "SR") },
                { "SBD", new Currency("SBD", "090", 2, "Solomon Islands dollar", "SI$") },
                { "SCR", new Currency("SCR", "690", 2, "Seychelles rupee", "SR") },
                { "SDG", new Currency("SDG", "938", 2, "Sudanese pound", "¤") },
                { "SEK", new Currency("SEK", "752", 2, "Swedish krona/kronor", "kr") },
                { "SGD", new Currency("SGD", "702", 2, "Singapore dollar", "S$") },
                { "SHP", new Currency("SHP", "654", 2, "Saint Helena pound", "£") },
                { "SLL", new Currency("SLL", "694", 0, "Sierra Leonean leone", "Le") },
                { "SOS", new Currency("SOS", "706", 2, "Somali shilling", "So.") },
                { "SRD", new Currency("SRD", "968", 2, "Surinamese dollar", "$") },
                { "SSP", new Currency("SSP", "728", 2, "South Sudanese pound", "¤") },
                { "STD", new Currency("STD", "678", 0, "São Tomé and Príncipe dobra", "Db") },
                { "SYP", new Currency("SYP", "760", 2, "Syrian pound", "¤") },
                { "SZL", new Currency("SZL", "748", 2, "Swazi lilangeni", "L") },
                { "THB", new Currency("THB", "764", 2, "Thai baht", "¤") },
                { "TJS", new Currency("TJS", "972", 2, "Tajikistani somoni", "¤") },
                { "TMT", new Currency("TMT", "934", 2, "Turkmenistani manat", "m") },
                { "TND", new Currency("TND", "788", 3, "Tunisian dinar", "¤") },
                { "TOP", new Currency("TOP", "776", 2, "Tongan paʻanga", "T$") },
                { "TRY", new Currency("TRY", "949", 2, "Turkish lira", "YTL") },
                { "TTD", new Currency("TTD", "780", 2, "Trinidad and Tobago dollar", "$") },
                { "TWD", new Currency("TWD", "901", 2, "New Taiwan dollar", "$") },
                { "TZS", new Currency("TZS", "834", 2, "Tanzanian shilling", "x/y") },
                { "UAH", new Currency("UAH", "980", 2, "Ukrainian hryvnia", "¤") },
                { "UGX", new Currency("UGX", "800", 2, "Ugandan shilling", "Ush") },
                { "USD", new Currency("USD", "840", 2, "United States dollar", "$") },
                { "USN", new Currency("USN", "997", 2, "United States dollar (next day) (funds code)", "$") },
                { "USS", new Currency("USS", "998", 2, "United States dollar (same day) (funds code)", "$") },
                { "UYI", new Currency("UYI", "940", 0, "Uruguay Peso en Unidades Indexadas (URUIURUI) (funds code)", "¤") },
                { "UYU", new Currency("UYU", "858", 2, "Uruguayan peso", "$") },
                { "UZS", new Currency("UZS", "860", 2, "Uzbekistan som", "¤") },
                { "VEF", new Currency("VEF", "937", 2, "Venezuelan bolívar fuerte", "Bs.F.") },
                { "VND", new Currency("VND", "704", 0, "Vietnamese dong", "¤") },
                { "VUV", new Currency("VUV", "548", 0, "Vanuatu vatu", "Vt") },
                { "WST", new Currency("WST", "882", 2, "Samoan tala", "WS$") },
                { "XAF", new Currency("XAF", "950", 0, "CFA franc BEAC", "FCFA") },
                { "XAG", new Currency("XAG", "961", DOT, "Silver (one troy ounce)", "¤") },
                { "XAU", new Currency("XAU", "959", DOT, "Gold (one troy ounce)", "¤") },
                { "XBA", new Currency("XBA", "955", DOT, "European Composite Unit (EURCO) (bond market unit)", "¤") },
                { "XBB", new Currency("XBB", "956", DOT, "European Monetary Unit (E.M.U.-6) (bond market unit)", "¤") },
                { "XBC", new Currency("XBC", "957", DOT, "European Unit of Account 9 (E.U.A.-9) (bond market unit)", "¤") },
                { "XBD", new Currency("XBD", "958", DOT, "European Unit of Account 17 (E.U.A.-17) (bond market unit)", "¤") },
                { "XCD", new Currency("XCD", "951", 2, "East Caribbean dollar", "$") },
                { "XDR", new Currency("XDR", "960", DOT, "Special drawing rights", "¤") },
                { "XFU", new Currency("XFU", "Nil", DOT, "UIC franc (special settlement currency)", "¤") },
                { "XOF", new Currency("XOF", "952", 0, "CFA franc BCEAO", "CFA") },
                { "XPD", new Currency("XPD", "964", DOT, "Palladium (one troy ounce)", "¤") },
                { "XPF", new Currency("XPF", "953", 0, "CFP franc", "¤") },
                { "XPT", new Currency("XPT", "962", DOT, "Platinum (one troy ounce)", "¤") },
                { "XTS", new Currency("XTS", "963", DOT, "Code reserved for testing purposes", "¤") },
                { "XXX", new Currency("XXX", "999", DOT, "No currency", "¤") },
                { "YER", new Currency("YER", "886", 2, "Yemeni rial", "¤") },
                { "ZAR", new Currency("ZAR", "710", 2, "South African rand", "R") },
                { "ZWM", new Currency("ZMK", "967", 2, "Zambian kwacha", "ZK") }
                // { "ZMK", new Currency("ZMK", "894", 2, "Zambian kwacha", "ZK") }  // Until 2013-01-01, replaced by ZWM
            }; 

            return currencies;
        }
    }
}