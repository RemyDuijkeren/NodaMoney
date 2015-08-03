using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace NodaMoney
{
    /// <summary>A unit of exchange, a currency of <see cref="Money" />.</summary>
    /// <remarks>See http://en.wikipedia.org/wiki/Currency .</remarks>
    [DataContract]
    [DebuggerDisplay("{Code}")]
    public struct Currency : IEquatable<Currency>, IXmlSerializable
    {
        // The Malagasy ariary and the Mauritanian ouguiya are technically divided into five subunits (the iraimbilanja and
        // khoum respectively), rather than by a power of ten. The coins display "1/5" on their face and are referred to as
        // a "fifth" (Khoum/cinquième). These are not used in practice, but when written out, a single significant digit is
        // used. E.g. 1.2 UM.
        // To represent this in decimal we do the following steps: 5 is 10 to the power of log(5) = 0.69897... ~ 0.7
        internal const double Z07 = 0.69897000433601880478626110527551; // Math.Log10(5);
        internal const double DOT = -1;
        internal static readonly Dictionary<string, Dictionary<string, Currency>> Currencies = new Dictionary<string, Dictionary<string, Currency>>
                                                                                                   {
                                                                                                       ["ISO-4217"] = InitializeIsoCurrencies()
                                                                                                   };

        /// <summary>Initializes a new instance of the <see cref="Currency"/> struct.</summary>
        /// <param name="code">The code.</param>
        /// <param name="number">The number.</param>
        /// <param name="decimalDigits">The decimal digits.</param>
        /// <param name="englishName">Name of the english.</param>
        /// <param name="symbol">The currency symbol.</param>
        /// <param name="isObsolete">Value indicating whether currency is obsolete.</param>
        /// <exception cref="System.ArgumentNullException">code or number or englishName or symbol is null</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">DecimalDigits of code must be between -1 and 4!</exception>
        internal Currency(string code, string number, double decimalDigits, string englishName, string symbol, bool isObsolete = false)
            : this()
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentNullException("code");
            if (string.IsNullOrWhiteSpace(number))
                throw new ArgumentNullException("number");
            if (string.IsNullOrWhiteSpace(englishName)) 
                throw new ArgumentNullException("englishName");
            if (string.IsNullOrWhiteSpace(symbol)) 
                throw new ArgumentNullException("symbol");
            if (decimalDigits < -1 || decimalDigits > 4)
                throw new ArgumentOutOfRangeException("code", "DecimalDigits must be between -1 and 4!");

            Code = code;
            Number = number;
            DecimalDigits = decimalDigits;
            EnglishName = englishName;
            Symbol = symbol;
            IsObsolete = isObsolete;
        }

        /// <summary>Gets the Currency that represents the country/region used by the current thread.</summary>
        /// <value>The Currency that represents the country/region used by the current thread.</value>
        public static Currency CurrentCurrency
        {
            get { return Currency.FromRegion(RegionInfo.CurrentRegion); }
        }   

        /// <summary>Gets the currency symbol.</summary>
        public string Symbol { get; private set; }

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
        /// For example, the default number of fraction digits for the US Dollar and Euro is 2, while for the Japanese Yen it's 0.
        /// In the case of pseudo-currencies, such as Gold or IMF Special Drawing Rights, -1 is returned.
        /// </para>
        /// <para>
        /// The Malagasy ariary and the Mauritanian ouguiya are technically divided into five subunits (the iraimbilanja and
        /// khoum respectively), rather than by a power of ten. The coins display "1/5" on their face and are referred to as
        /// a "fifth" (Khoum/cinquième). These are not used in practice, but when written out, a single significant digit is
        /// used. E.g. 1.2 UM.
        /// </para>
        /// <para>
        /// To represent this in decimal we do the following steps: 5 is 10 to the power of log(5) = 0.69897... ~ 0.7
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

                return new decimal(1 / Math.Pow(10, DecimalDigits));
            }
        }

        /// <summary>Gets a value indicating whether currency is obsolete.</summary>
        /// <value><c>true</c> if this instance is obsolete; otherwise, <c>false</c>.</value>
        public bool IsObsolete { get; private set; }

        /// <summary>Create an instance of the <see cref="Currency"/>, based on a ISO 4217 currency code.</summary>
        /// <param name="code">A ISO 4217 currency code, like EUR or USD.</param>
        /// <returns>An instance of the type <see cref="Currency"/>.</returns>
        /// <exception cref="ArgumentNullException">The value of 'code' cannot be null.</exception>
        /// <exception cref="ArgumentException">The 'code' is an unknown ISO 4217 currency code!</exception>
        public static Currency FromCode(string code)
        {
            if (string.IsNullOrWhiteSpace(code)) 
                throw new ArgumentNullException(nameof(code));

            // TODO : OrderBy... First ISO-4217 namespace
            // Don't change to LINQ, because of performance!
            foreach (var ns in Currencies)
            {
                if (ns.Value.ContainsKey(code.ToUpperInvariant()))
                    return ns.Value[code.ToUpperInvariant()];
            }

            throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "{0} is an unknown currency code!", code));
        }

        public static Currency FromCode(string code, string @namespace)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentNullException(nameof(code));
            if (string.IsNullOrWhiteSpace(@namespace))
                throw new ArgumentNullException(nameof(@namespace));
            if (!Currencies.ContainsKey(@namespace))
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "{0} is an unknown namespace!", @namespace));
            if (!Currencies[@namespace].ContainsKey(code.ToUpperInvariant()))
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "{0} is an unknown {1} currency code!", code, @namespace));

            return Currencies[@namespace][code.ToUpperInvariant()];
        }

        /// <summary>Creates an instance of the <see cref="Currency"/> used within the specified <see cref="RegionInfo"/>.</summary>
        /// <param name="region"><see cref="RegionInfo"/> to get a <see cref="Currency"/> for.</param>
        /// <returns>The <see cref="Currency"/> instance used within the specified <see cref="RegionInfo"/>.</returns>
        /// <exception cref="ArgumentNullException">The value of 'region' cannot be null.</exception>
        public static Currency FromRegion(RegionInfo region)
        {
            if (region == null) 
                throw new ArgumentNullException(nameof(region));

            return FromCode(region.ISOCurrencySymbol);
        }

        /// <summary>Creates an instance of the <see cref="Currency"/> used within the specified <see cref="CultureInfo"/>.</summary>
        /// <param name="culture"><see cref="CultureInfo"/> to get a <see cref="Currency"/> for.</param>
        /// <returns>The <see cref="Currency"/> instance used within the specified <see cref="CultureInfo"/>.</returns>
        /// <exception cref="ArgumentNullException">The value of 'culture' cannot be null.</exception>
        /// <exception cref="ArgumentException">Culture is a neutral culture, from which no region information can be extracted!</exception>
        public static Currency FromCulture(CultureInfo culture)
        {
            if (culture == null) 
                throw new ArgumentNullException(nameof(culture));
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
        /// country/region. See also <seealso cref="System.Globalization.RegionInfo(System.String)"/>.</para> 
        /// </param>
        /// <returns>The <see cref="Currency"/> instance used within the specified region.</returns>
        /// <exception cref="ArgumentNullException">The value of 'name' cannot be null.</exception>
        public static Currency FromRegion(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) 
                throw new ArgumentNullException(nameof(name));            

            return FromRegion(new RegionInfo(name));
        }

        /// <summary>Get all currencies.</summary>
        /// <returns>An array that contains all the ISO 4217 currencies. The array of currencies is unsorted.</returns>
        public static Currency[] GetAllCurrencies()
        {
            // TODO: IQueryable?
            return Currencies.SelectMany(ns => ns.Value).Select(kv => kv.Value).ToArray();
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
        /// <param name="left">The first <see cref="Currency"/> object.</param>
        /// <param name="right">The second <see cref="Currency"/> object.</param>
        /// <returns>true if left and right are equal to this instance; otherwise, false.</returns>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Calling override method")]
        public static bool Equals(Currency left, Currency right)
        {
            return left.Equals(right);
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

        /// <summary>This method is reserved and should not be used. When implementing the IXmlSerializable interface, you should
        /// return null (Nothing in Visual Basic) from this method, and instead, if specifying a custom schema is required, apply
        /// the <see cref="T:System.Xml.Serialization.XmlSchemaProviderAttribute" /> to the class.</summary>
        /// <returns>An <see cref="T:System.Xml.Schema.XmlSchema" /> that describes the XML representation of the object that is
        /// produced by the <see cref="M:System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter)" /> method and 
        /// consumed by the <see cref="M:System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader)" /> method.
        /// </returns>
        public XmlSchema GetSchema()
        {
            return null;
        }

        /// <summary>Generates an object from its XML representation.</summary>
        /// <param name="reader">The <see cref="T:System.Xml.XmlReader" /> stream from which the object is deserialized.</param>
        /// <exception cref="ArgumentNullException">The value of 'reader' cannot be null. </exception>
        public void ReadXml(XmlReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            this = Currency.FromCode(reader["Currency"]);
        }

        /// <summary>
        /// Converts an object into its XML representation.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Xml.XmlWriter" /> stream to which the object is serialized.</param>
        /// <exception cref="ArgumentNullException">The value of 'writer' cannot be null. </exception>
        public void WriteXml(XmlWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            writer.WriteAttributeString("Currency", Code);
        }

        private static Dictionary<string, Currency> InitializeIsoCurrencies()
        {
            var currencies = new Dictionary<string, Currency>
                                 {
                                     { "AED", new Currency("AED", "784", 2, "United Arab Emirates dirham", "د.إ") },
                                     { "AFN", new Currency("AFN", "971", 2, "Afghan afghani", "؋") },
                                     { "ALL", new Currency("ALL", "008", 2, "Albanian lek", "Lek") },
                                     { "AMD", new Currency("AMD", "051", 2, "Armenian dram", "֏") },
                                     { "ANG", new Currency("ANG", "532", 2, "Netherlands Antillean guilder", "ƒ") },
                                     { "AOA", new Currency("AOA", "973", 2, "Angolan kwanza", "Kz") },
                                     { "ARS", new Currency("ARS", "032", 2, "Argentine peso", "$") },
                                     { "AUD", new Currency("AUD", "036", 2, "Australian dollar", "$") },
                                     { "AWG", new Currency("AWG", "533", 2, "Aruban florin", "ƒ") },
                                     { "AZN", new Currency("AZN", "944", 2, "Azerbaijani manat", "ман") },
                                     { "BAM", new Currency("BAM", "977", 2, "Bosnia and Herzegovina convertible mark", "KM") },
                                     { "BBD", new Currency("BBD", "052", 2, "Barbados dollar", "$") },
                                     { "BDT", new Currency("BDT", "050", 2, "Bangladeshi taka", "৳") }, // or Tk
                                     { "BGN", new Currency("BGN", "975", 2, "Bulgarian lev", "лв") },
                                     { "BHD", new Currency("BHD", "048", 3, "Bahraini dinar", "BD") }, // or د.ب. (switched for unit tests to work)
                                     { "BIF", new Currency("BIF", "108", 0, "Burundian franc", "FBu") },
                                     { "BMD", new Currency("BMD", "060", 2, "Bermudian dollar", "$") },
                                     { "BND", new Currency("BND", "096", 2, "Brunei dollar", "$") }, // or B$
                                     { "BOB", new Currency("BOB", "068", 2, "Boliviano", "Bs.") }, // or BS or $b
                                     { "BOV", new Currency("BOV", "984", 2, "Bolivian Mvdol (funds code)", "¤") }, // <==== not found
                                     { "BRL", new Currency("BRL", "986", 2, "Brazilian real", "R$") },
                                     { "BSD", new Currency("BSD", "044", 2, "Bahamian dollar", "$") },
                                     { "BTN", new Currency("BTN", "064", 2, "Bhutanese ngultrum", "Nu.") },
                                     { "BWP", new Currency("BWP", "072", 2, "Botswana pula", "P") },
                                     { "BYR", new Currency("BYR", "974", 0, "Belarusian ruble", "Br") }, // or p.? wiki controdicts with xe.com
                                     { "BZD", new Currency("BZD", "084", 2, "Belize dollar", "BZ$") },
                                     { "CAD", new Currency("CAD", "124", 2, "Canadian dollar", "$") },
                                     { "CDF", new Currency("CDF", "976", 2, "Congolese franc", "FC") },
                                     { "CHE", new Currency("CHE", "947", 2, "WIR Euro (complementary currency)", "CHE") },
                                     { "CHF", new Currency("CHF", "756", 2, "Swiss franc", "fr.") }, // or CHF
                                     { "CHW", new Currency("CHW", "948", 2, "WIR Franc (complementary currency)", "CHW") },
                                     { "CLF", new Currency("CLF", "990", 4, "Unidad de Fomento (funds code)", "CLF") },
                                     { "CLP", new Currency("CLP", "152", 0, "Chilean peso", "$") },
                                     { "CNY", new Currency("CNY", "156", 2, "Chinese yuan", "¥") },
                                     { "COP", new Currency("COP", "170", 2, "Colombian peso", "$") },
                                     { "COU", new Currency("COU", "970", 2, "Unidad de Valor Real", "¤") }, // ???
                                     { "CRC", new Currency("CRC", "188", 2, "Costa Rican colon", "₡") },
                                     { "CUC", new Currency("CUC", "931", 2, "Cuban convertible peso", "CUC$") }, // $ or CUC
                                     { "CUP", new Currency("CUP", "192", 2, "Cuban peso", "$") }, // or ₱ (obsolete?)
                                     { "CVE", new Currency("CVE", "132", 0, "Cape Verde escudo", "$") },
                                     { "CZK", new Currency("CZK", "203", 2, "Czech koruna", "Kč") },
                                     { "DJF", new Currency("DJF", "262", 0, "Djiboutian franc", "Fdj") },
                                     { "DKK", new Currency("DKK", "208", 2, "Danish krone", "kr") },
                                     { "DOP", new Currency("DOP", "214", 2, "Dominican peso", "RD$") }, // or $
                                     { "DZD", new Currency("DZD", "012", 2, "Algerian dinar", "DA") }, // (Latin) or د.ج (Arabic)
                                     { "EGP", new Currency("EGP", "818", 2, "Egyptian pound", "LE") }, // or E£ or ج.م (Arabic)
                                     { "ERN", new Currency("ERN", "232", 2, "Eritrean nakfa", "ERN") },
                                     { "ETB", new Currency("ETB", "230", 2, "Ethiopian birr", "Br") }, // (Latin) or ብር (Ethiopic)
                                     { "EUR", new Currency("EUR", "978", 2, "Euro", "€") },
                                     { "FJD", new Currency("FJD", "242", 2, "Fiji dollar", "$") }, // or FJ$
                                     { "FKP", new Currency("FKP", "238", 2, "Falkland Islands pound", "£") },
                                     { "GBP", new Currency("GBP", "826", 2, "Pound sterling", "£") },
                                     { "GEL", new Currency("GEL", "981", 2, "Georgian lari", "ლ.") }, // TODO: new symbol since July 18, 2014 => see http://en.wikipedia.org/wiki/Georgian_lari
                                     { "GHS", new Currency("GHS", "936", 2, "Ghanaian cedi", "GH¢") }, // or GH₵
                                     { "GIP", new Currency("GIP", "292", 2, "Gibraltar pound", "£") },
                                     { "GMD", new Currency("GMD", "270", 2, "Gambian dalasi", "D") },
                                     { "GNF", new Currency("GNF", "324", 0, "Guinean franc", "FG") }, // (possibly also Fr or GFr)
                                     { "GTQ", new Currency("GTQ", "320", 2, "Guatemalan quetzal", "Q") },
                                     { "GYD", new Currency("GYD", "328", 2, "Guyanese dollar", "$") }, // or G$
                                     { "HKD", new Currency("HKD", "344", 2, "Hong Kong dollar", "HK$") }, // or $
                                     { "HNL", new Currency("HNL", "340", 2, "Honduran lempira", "L") },
                                     { "HRK", new Currency("HRK", "191", 2, "Croatian kuna", "kn") },
                                     { "HTG", new Currency("HTG", "332", 2, "Haitian gourde", "G") },
                                     { "HUF", new Currency("HUF", "348", 2, "Hungarian forint", "Ft") },
                                     { "IDR", new Currency("IDR", "360", 2, "Indonesian rupiah", "Rp") },
                                     { "ILS", new Currency("ILS", "376", 2, "Israeli new shekel", "₪") },
                                     { "INR", new Currency("INR", "356", 2, "Indian rupee", "₹") },
                                     { "IQD", new Currency("IQD", "368", 3, "Iraqi dinar", "د.ع") },
                                     { "IRR", new Currency("IRR", "364", 0, "Iranian rial", "ريال") },
                                     { "ISK", new Currency("ISK", "352", 0, "Icelandic króna", "kr") },
                                     { "JMD", new Currency("JMD", "388", 2, "Jamaican dollar", "J$") }, // or $
                                     { "JOD", new Currency("JOD", "400", 3, "Jordanian dinar", "د.ا.‏") },
                                     { "JPY", new Currency("JPY", "392", 0, "Japanese yen", "¥") },
                                     { "KES", new Currency("KES", "404", 2, "Kenyan shilling", "KSh") },
                                     { "KGS", new Currency("KGS", "417", 2, "Kyrgyzstani som", "сом") },
                                     { "KHR", new Currency("KHR", "116", 2, "Cambodian riel", "៛") },
                                     { "KMF", new Currency("KMF", "174", 0, "Comoro franc", "CF") },
                                     { "KPW", new Currency("KPW", "408", 0, "North Korean won", "₩") },
                                     { "KRW", new Currency("KRW", "410", 0, "South Korean won", "₩") },
                                     { "KWD", new Currency("KWD", "414", 3, "Kuwaiti dinar", "د.ك") }, // or K.D.
                                     { "KYD", new Currency("KYD", "136", 2, "Cayman Islands dollar", "$") },
                                     { "KZT", new Currency("KZT", "398", 2, "Kazakhstani tenge", "₸") },
                                     { "LAK", new Currency("LAK", "418", 0, "Lao kip", "₭") }, // or ₭N
                                     { "LBP", new Currency("LBP", "422", 0, "Lebanese pound", "ل.ل") },
                                     { "LKR", new Currency("LKR", "144", 2, "Sri Lankan rupee", "Rs") }, // or රු
                                     { "LRD", new Currency("LRD", "430", 2, "Liberian dollar", "$") }, // or L$, LD$
                                     { "LSL", new Currency("LSL", "426", 2, "Lesotho loti", "L") }, // L or M (pl.)
                                     { "LTL", new Currency("LTL", "440", 2, "Lithuanian litas", "Lt", isObsolete: true) }, // Until 2014-12-31, replaced by EUR
                                     { "LVL", new Currency("LVL", "428", 2, "Latvian lats", "Ls", isObsolete: true) }, // Until 2014-01-15, replaced by EUR
                                     { "LYD", new Currency("LYD", "434", 3, "Libyan dinar", "ل.د") }, // or LD
                                     { "MAD", new Currency("MAD", "504", 2, "Moroccan dirham", "د.م.") },
                                     { "MDL", new Currency("MDL", "498", 2, "Moldovan leu", "L") },
                                     { "MGA", new Currency("MGA", "969", Z07, "Malagasy ariary", "Ar") },  // divided into five subunits rather than by a power of ten. 5 is 10 to the power of 0.69897...
                                     { "MKD", new Currency("MKD", "807", 0, "Macedonian denar", "ден") },
                                     { "MMK", new Currency("MMK", "104", 0, "Myanma kyat", "K") },
                                     { "MNT", new Currency("MNT", "496", 2, "Mongolian tugrik", "₮") },
                                     { "MOP", new Currency("MOP", "446", 2, "Macanese pataca", "MOP$") },
                                     { "MRO", new Currency("MRO", "478", Z07, "Mauritanian ouguiya", "UM") }, // divided into five subunits rather than by a power of ten. 5 is 10 to the power of 0.69897...
                                     { "MUR", new Currency("MUR", "480", 2, "Mauritian rupee", "Rs") },
                                     { "MVR", new Currency("MVR", "462", 2, "Maldivian rufiyaa", "Rf") }, // or , MRf, MVR, .ރ or /-
                                     { "MWK", new Currency("MWK", "454", 2, "Malawian kwacha", "MK") },
                                     { "MXN", new Currency("MXN", "484", 2, "Mexican peso", "$") },
                                     { "MXV", new Currency("MXV", "979", 2, "Mexican Unidad de Inversion (UDI) (funds code)", "¤") },  // <==== not found
                                     { "MYR", new Currency("MYR", "458", 2, "Malaysian ringgit", "RM") },
                                     { "MZN", new Currency("MZN", "943", 2, "Mozambican metical", "MTn") }, // or MTN
                                     { "NAD", new Currency("NAD", "516", 2, "Namibian dollar", "N$") }, // or $
                                     { "NGN", new Currency("NGN", "566", 2, "Nigerian naira", "₦") },
                                     { "NIO", new Currency("NIO", "558", 2, "Nicaraguan córdoba", "C$") },
                                     { "NOK", new Currency("NOK", "578", 2, "Norwegian krone", "kr") },
                                     { "NPR", new Currency("NPR", "524", 2, "Nepalese rupee", "Rs") }, // or ₨ or रू
                                     { "NZD", new Currency("NZD", "554", 2, "New Zealand dollar", "$") },
                                     { "OMR", new Currency("OMR", "512", 3, "Omani rial", "ر.ع.") },
                                     { "PAB", new Currency("PAB", "590", 2, "Panamanian balboa", "B/.") },
                                     { "PEN", new Currency("PEN", "604", 2, "Peruvian nuevo sol", "S/.") },
                                     { "PGK", new Currency("PGK", "598", 2, "Papua New Guinean kina", "K") },
                                     { "PHP", new Currency("PHP", "608", 2, "Philippine peso", "₱") }, // or P or PHP or PhP
                                     { "PKR", new Currency("PKR", "586", 2, "Pakistani rupee", "Rs") }, 
                                     { "PLN", new Currency("PLN", "985", 2, "Polish złoty", "zł") },
                                     { "PYG", new Currency("PYG", "600", 0, "Paraguayan guaraní", "₲") },
                                     { "QAR", new Currency("QAR", "634", 2, "Qatari riyal", "ر.ق") }, // or QR
                                     { "RON", new Currency("RON", "946", 2, "Romanian new leu", "lei") },
                                     { "RSD", new Currency("RSD", "941", 2, "Serbian dinar", "РСД") }, // or RSD (or дин. or din. ?)
                                     { "RUB", new Currency("RUB", "643", 2, "Russian rouble", "₽") }, // or R or руб (both onofficial)
                                     { "RWF", new Currency("RWF", "646", 0, "Rwandan franc", "RFw") }, // or RF, R₣
                                     { "SAR", new Currency("SAR", "682", 2, "Saudi riyal", "ر.س") }, // or SR (Latin) or ﷼‎ (Unicode)
                                     { "SBD", new Currency("SBD", "090", 2, "Solomon Islands dollar", "SI$") },
                                     { "SCR", new Currency("SCR", "690", 2, "Seychelles rupee", "SR") }, // or SRe
                                     { "SDG", new Currency("SDG", "938", 2, "Sudanese pound", "ج.س.") },
                                     { "SEK", new Currency("SEK", "752", 2, "Swedish krona/kronor", "kr") },
                                     { "SGD", new Currency("SGD", "702", 2, "Singapore dollar", "S$") }, // or $
                                     { "SHP", new Currency("SHP", "654", 2, "Saint Helena pound", "£") },
                                     { "SLL", new Currency("SLL", "694", 0, "Sierra Leonean leone", "Le") },
                                     { "SOS", new Currency("SOS", "706", 2, "Somali shilling", "S") }, // or Sh.So.
                                     { "SRD", new Currency("SRD", "968", 2, "Surinamese dollar", "$") },
                                     { "SSP", new Currency("SSP", "728", 2, "South Sudanese pound", "£") }, // not sure about symbol...
                                     { "STD", new Currency("STD", "678", 0, "São Tomé and Príncipe dobra", "Db") },
                                     { "SYP", new Currency("SYP", "760", 2, "Syrian pound", "ܠ.ܣ.‏") }, // or LS or £S (or £)
                                     { "SZL", new Currency("SZL", "748", 2, "Swazi lilangeni", "L") }, // or E (plural)
                                     { "THB", new Currency("THB", "764", 2, "Thai baht", "฿") },
                                     { "TJS", new Currency("TJS", "972", 2, "Tajikistani somoni", "смн") },
                                     { "TMT", new Currency("TMT", "934", 2, "Turkmenistani manat", "m") }, // or T?
                                     { "TND", new Currency("TND", "788", 3, "Tunisian dinar", "د.ت") }, // or DT (Latin)
                                     { "TOP", new Currency("TOP", "776", 2, "Tongan paʻanga", "T$") }, // (sometimes PT)
                                     { "TRY", new Currency("TRY", "949", 2, "Turkish lira", "₺") },
                                     { "TTD", new Currency("TTD", "780", 2, "Trinidad and Tobago dollar", "$") }, // or TT$
                                     { "TWD", new Currency("TWD", "901", 2, "New Taiwan dollar", "NT$") }, // or $
                                     { "TZS", new Currency("TZS", "834", 2, "Tanzanian shilling", "x/y") }, // or TSh
                                     { "UAH", new Currency("UAH", "980", 2, "Ukrainian hryvnia", "₴") },
                                     { "UGX", new Currency("UGX", "800", 2, "Ugandan shilling", "USh") },
                                     { "USD", new Currency("USD", "840", 2, "United States dollar", "$") }, // or US$
                                     { "USN", new Currency("USN", "997", 2, "United States dollar (next day) (funds code)", "$") },
                                     { "USS", new Currency("USS", "998", 2, "United States dollar (same day) (funds code)", "$") },
                                     { "UYI", new Currency("UYI", "940", 0, "Uruguay Peso en Unidades Indexadas (URUIURUI) (funds code)", "¤") },
                                     { "UYU", new Currency("UYU", "858", 2, "Uruguayan peso", "$") }, // or $U
                                     { "UZS", new Currency("UZS", "860", 2, "Uzbekistan som", "лв") }, // or сўм ?
                                     { "VEF", new Currency("VEF", "937", 2, "Venezuelan bolívar fuerte", "Bs.F.") }, // or Bs.
                                     { "VND", new Currency("VND", "704", 0, "Vietnamese dong", "₫") },
                                     { "VUV", new Currency("VUV", "548", 0, "Vanuatu vatu", "VT") },
                                     { "WST", new Currency("WST", "882", 2, "Samoan tala", "WS$") }, // sometimes SAT, ST or T
                                     { "XAF", new Currency("XAF", "950", 0, "CFA franc BEAC", "FCFA") },
                                     { "XAG", new Currency("XAG", "961", DOT, "Silver (one troy ounce)", "¤") },
                                     { "XAU", new Currency("XAU", "959", DOT, "Gold (one troy ounce)", "¤") },
                                     { "XBA", new Currency("XBA", "955", DOT, "European Composite Unit (EURCO) (bond market unit)", "¤") },
                                     { "XBB", new Currency("XBB", "956", DOT, "European Monetary Unit (E.M.U.-6) (bond market unit)", "¤") },
                                     { "XBC", new Currency("XBC", "957", DOT, "European Unit of Account 9 (E.U.A.-9) (bond market unit)", "¤") },
                                     { "XBD", new Currency("XBD", "958", DOT, "European Unit of Account 17 (E.U.A.-17) (bond market unit)", "¤") },
                                     { "XCD", new Currency("XCD", "951", 2, "East Caribbean dollar", "$") }, // or EC$
                                     { "XDR", new Currency("XDR", "960", DOT, "Special drawing rights", "¤") },
                                     { "XFU", new Currency("XFU", "Nil", DOT, "UIC franc (special settlement currency)", "¤") },
                                     { "XOF", new Currency("XOF", "952", 0, "CFA franc BCEAO", "CFA") },
                                     { "XPD", new Currency("XPD", "964", DOT, "Palladium (one troy ounce)", "¤") },
                                     { "XPF", new Currency("XPF", "953", 0, "CFP franc", "F") },
                                     { "XPT", new Currency("XPT", "962", DOT, "Platinum (one troy ounce)", "¤") },
                                     { "XSU", new Currency("XSU", "994", DOT, "SUCRE", "¤") },
                                     { "XTS", new Currency("XTS", "963", DOT, "Code reserved for testing purposes", "¤") },
                                     { "XUA", new Currency("XUA", "965", DOT, "ADB Unit of Account", "¤") },
                                     { "XXX", new Currency("XXX", "999", DOT, "No currency", "¤") },
                                     { "YER", new Currency("YER", "886", 2, "Yemeni rial", "﷼") }, // or ر.ي.‏‏ ?
                                     { "ZAR", new Currency("ZAR", "710", 2, "South African rand", "R") },
                                     { "ZMW", new Currency("ZMW", "967", 2, "Zambian kwacha", "ZK") }, // or ZMW
                                     { "ZMK", new Currency("ZMK", "894", 2, "Zambian kwacha", "ZK", isObsolete: true) },  // Until 2013-01-01, replaced by ZWM
                                     { "ZWL", new Currency("ZWL", "932", 2, "Zimbabwean dollar", "$", isObsolete: true) }, // or Z$ (official currency of Zimbabwe from 1980 to 12 April 2009, not used anymore)
                                     { "EEK", new Currency("EEK", "233", 2, "Estonian kroon", "kr", isObsolete: true) }  // From 1992 Until 2010-12-31, replaced by EUR
                                 }; 

            return currencies;
        }
    }
}