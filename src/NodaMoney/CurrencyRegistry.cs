using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace NodaMoney
{
    /// <summary>Represent the central thread-safe registry for currencies.</summary>
    internal class CurrencyRegistry
    {
        /// <summary>
        /// The Malagasy ariary and the Mauritanian ouguiya are technically divided into five subunits (the iraimbilanja and
        /// khoum respectively), rather than by a power of ten. The coins display "1/5" on their face and are referred to as
        /// a "fifth" (Khoum/cinquième). These are not used in practice, but when written out, a single significant digit is
        /// used. E.g. 1.2 UM.
        /// To represent this in decimal we do the following steps: 5 is 10 to the power of log(5) = 0.69897... ~ 0.7
        /// </summary>
        internal const double Z07 = 0.69897000433601880478626110527551; // Math.Log10(5);

        /// <summary>Used for indication that the number of decimal digits doesn't matter, for example for gold or silver.</summary>
        internal const double NotApplicable = -1;

        private static readonly ConcurrentDictionary<string, Currency> Currencies = new ConcurrentDictionary<string, Currency>(InitializeIsoCurrencies());
        private static readonly ConcurrentDictionary<string, byte> Namespaces = new ConcurrentDictionary<string, byte>(new Dictionary<string, byte> { ["ISO-4217"] = default(byte), ["ISO-4217-HISTORIC"] = default(byte) });

        /// <summary>Tries the get <see cref="Currency"/> of the given code and namespace.</summary>
        /// <param name="code">A currency code, like EUR or USD.</param>
        /// <param name="currency">When this method returns, contains the <see cref="Currency"/> that has the specified code, or the default value of the type if the operation failed.</param>
        /// <returns><b>true</b> if <see cref="CurrencyRegistry"/> contains a <see cref="Currency"/> with the specified code; otherwise, <b>false</b>.</returns>
        /// <exception cref="System.ArgumentNullException">The value of 'code' cannot be null or empty.</exception>
        public bool TryGet(string code, out Currency currency)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentNullException(nameof(code));

            var found = new List<Currency>();
            foreach (var ns in Namespaces.Keys)
            {
                // don't use string.Format(), string concat much faster in this case!
                if (Currencies.TryGetValue(ns + "::" + code, out Currency c))
                {
                    found.Add(c);
                }
            }

            currency = found.FirstOrDefault(); // TODO: If more than one, sort by prio.
            return !currency.Equals(default(Currency));
        }

        /// <summary>Tries the get <see cref="Currency"/> of the given code and namespace.</summary>
        /// <param name="code">A currency code, like EUR or USD.</param>
        /// <param name="namespace">A namespace, like ISO-4217.</param>
        /// <param name="currency">When this method returns, contains the <see cref="Currency"/> that has the specified code and namespace, or the default value of the type if the operation failed.</param>
        /// <returns><b>true</b> if <see cref="CurrencyRegistry"/> contains a <see cref="Currency"/> with the specified code; otherwise, <b>false</b>.</returns>
        /// <exception cref="System.ArgumentNullException">The value of 'code' or 'namespace' cannot be null or empty.</exception>
        public bool TryGet(string code, string @namespace, out Currency currency)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentNullException(nameof(code));
            if (string.IsNullOrWhiteSpace(@namespace))
                throw new ArgumentNullException(nameof(@namespace));

            return Currencies.TryGetValue(@namespace + "::" + code, out currency); // don't use string.Format(), string concat much faster in this case!
        }

        /// <summary>Attempts to add the <see cref="Currency"/> of the given code and namespace.</summary>
        /// <param name="code">A currency code, like EUR or USD.</param>
        /// <param name="namespace">A namespace, like ISO-4217.</param>
        /// <param name="currency">When this method returns, contains the <see cref="Currency"/> that has the specified code and namespace, or the default value of the type if the operation failed.</param>
        /// <returns><b>true</b> if the <see cref="Currency"/> with the specified code is added; otherwise, <b>false</b>.</returns>
        /// <exception cref="System.ArgumentNullException">The value of 'code' or 'namespace' cannot be null or empty.</exception>
        public bool TryAdd(string code, string @namespace, Currency currency)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentNullException(nameof(code));
            if (string.IsNullOrWhiteSpace(@namespace))
                throw new ArgumentNullException(nameof(@namespace));

            Namespaces[@namespace] = default(byte);
            return Currencies.TryAdd(@namespace + "::" + code, currency);
        }

        /// <summary>Attempts to remove the <see cref="Currency"/> of the given code and namespace.</summary>
        /// <param name="code">A currency code, like EUR or USD.</param>
        /// <param name="namespace">A namespace, like ISO-4217.</param>
        /// <param name="currency">When this method returns, contains the <see cref="Currency"/> that has the specified code and namespace, or the default value of the type if the operation failed.</param>
        /// <returns><b>true</b> if the <see cref="Currency"/> with the specified code is removed; otherwise, <b>false</b>.</returns>
        /// <exception cref="System.ArgumentNullException">The value of 'code' or 'namespace' cannot be null or empty.</exception>
        public bool TryRemove(string code, string @namespace, out Currency currency)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentNullException(nameof(code));
            if (string.IsNullOrWhiteSpace(@namespace))
                throw new ArgumentNullException(nameof(@namespace));

            // Namespaces[@namespace] = null; // TODO: Count currencies in namespace and when zero, remove namespace
            return Currencies.TryRemove(@namespace + "::" + code, out currency);
        }

        /// <summary>Get all registered currencies.</summary>
        /// <returns>An <see cref="IEnumerable{Currency}"/> of all registered currencies.</returns>
        public IEnumerable<Currency> GetAllCurrencies()
        {
            return Currencies.Values.AsEnumerable();
        }

        private static IDictionary<string, Currency> InitializeIsoCurrencies()
        {
            // TODO: Move to resource file.
            return new Dictionary<string, Currency>
            {
                // ISO-4217 currencies (list one)
                ["ISO-4217::AED"] = new Currency("AED", "784", 2, "United Arab Emirates dirham", "د.إ"),
                ["ISO-4217::AFN"] = new Currency("AFN", "971", 2, "Afghan afghani", "؋"),
                ["ISO-4217::ALL"] = new Currency("ALL", "008", 2, "Albanian lek", "L"),
                ["ISO-4217::AMD"] = new Currency("AMD", "051", 2, "Armenian dram", "֏"),
                ["ISO-4217::ANG"] = new Currency("ANG", "532", 2, "Netherlands Antillean guilder", "ƒ"),
                ["ISO-4217::AOA"] = new Currency("AOA", "973", 2, "Angolan kwanza", "Kz"),
                ["ISO-4217::ARS"] = new Currency("ARS", "032", 2, "Argentine peso", "$"),
                ["ISO-4217::AUD"] = new Currency("AUD", "036", 2, "Australian dollar", "$"),
                ["ISO-4217::AWG"] = new Currency("AWG", "533", 2, "Aruban florin", "ƒ"),
                ["ISO-4217::AZN"] = new Currency("AZN", "944", 2, "Azerbaijan Manat", "ман"), // AZERBAIJAN
                ["ISO-4217::BAM"] = new Currency("BAM", "977", 2, "Bosnia and Herzegovina convertible mark", "KM"),
                ["ISO-4217::BBD"] = new Currency("BBD", "052", 2, "Barbados dollar", "$"),
                ["ISO-4217::BDT"] = new Currency("BDT", "050", 2, "Bangladeshi taka", "৳"), // or Tk
                ["ISO-4217::BGN"] = new Currency("BGN", "975", 2, "Bulgarian lev", "лв."),
                ["ISO-4217::BHD"] = new Currency("BHD", "048", 3, "Bahraini dinar", "BD"), // or د.ب. (switched for unit tests to work)
                ["ISO-4217::BIF"] = new Currency("BIF", "108", 0, "Burundian franc", "FBu"),
                ["ISO-4217::BMD"] = new Currency("BMD", "060", 2, "Bermudian dollar", "$"),
                ["ISO-4217::BND"] = new Currency("BND", "096", 2, "Brunei dollar", "$"), // or B$
                ["ISO-4217::BOB"] = new Currency("BOB", "068", 2, "Boliviano", "Bs."), // or BS or $b
                ["ISO-4217::BOV"] = new Currency("BOV", "984", 2, "Bolivian Mvdol (funds code)", Currency.GenericCurrencySign), // <==== not found
                ["ISO-4217::BRL"] = new Currency("BRL", "986", 2, "Brazilian real", "R$"),
                ["ISO-4217::BSD"] = new Currency("BSD", "044", 2, "Bahamian dollar", "$"),
                ["ISO-4217::BTN"] = new Currency("BTN", "064", 2, "Bhutanese ngultrum", "Nu."),
                ["ISO-4217::BWP"] = new Currency("BWP", "072", 2, "Botswana pula", "P"),
                ["ISO-4217::BYR"] = new Currency("BYR", "974", 0, "Belarusian ruble", "Br", validTo: new DateTime(2016, 12, 31), validFrom: new DateTime(2000, 01, 01)),
                ["ISO-4217::BYN"] = new Currency("BYN", "974", 0, "Belarusian ruble", "Br", validFrom: new DateTime(2006, 06, 01)),
                ["ISO-4217::BZD"] = new Currency("BZD", "084", 2, "Belize dollar", "BZ$"),
                ["ISO-4217::CAD"] = new Currency("CAD", "124", 2, "Canadian dollar", "$"),
                ["ISO-4217::CDF"] = new Currency("CDF", "976", 2, "Congolese franc", "FC"),
                ["ISO-4217::CHE"] = new Currency("CHE", "947", 2, "WIR Euro (complementary currency)", "CHE"),
                ["ISO-4217::CHF"] = new Currency("CHF", "756", 2, "Swiss franc", "fr."), // or CHF
                ["ISO-4217::CHW"] = new Currency("CHW", "948", 2, "WIR Franc (complementary currency)", "CHW"),
                ["ISO-4217::CLF"] = new Currency("CLF", "990", 4, "Unidad de Fomento (funds code)", "CLF"),
                ["ISO-4217::CLP"] = new Currency("CLP", "152", 0, "Chilean peso", "$"),
                ["ISO-4217::CNY"] = new Currency("CNY", "156", 2, "Chinese yuan", "¥"),
                ["ISO-4217::COP"] = new Currency("COP", "170", 2, "Colombian peso", "$"),
                ["ISO-4217::COU"] = new Currency("COU", "970", 2, "Unidad de Valor Real", Currency.GenericCurrencySign), // ???
                ["ISO-4217::CRC"] = new Currency("CRC", "188", 2, "Costa Rican colon", "₡"),
                ["ISO-4217::CUC"] = new Currency("CUC", "931", 2, "Cuban convertible peso", "CUC$"), // $ or CUC
                ["ISO-4217::CUP"] = new Currency("CUP", "192", 2, "Cuban peso", "$"), // or ₱ (obsolete?)
                ["ISO-4217::CVE"] = new Currency("CVE", "132", 0, "Cape Verde escudo", "$"),
                ["ISO-4217::CZK"] = new Currency("CZK", "203", 2, "Czech koruna", "Kč"),
                ["ISO-4217::DJF"] = new Currency("DJF", "262", 0, "Djiboutian franc", "Fdj"),
                ["ISO-4217::DKK"] = new Currency("DKK", "208", 2, "Danish krone", "kr."),
                ["ISO-4217::DOP"] = new Currency("DOP", "214", 2, "Dominican peso", "RD$"), // or $
                ["ISO-4217::DZD"] = new Currency("DZD", "012", 2, "Algerian dinar", "DA"), // (Latin) or د.ج (Arabic)
                ["ISO-4217::EGP"] = new Currency("EGP", "818", 2, "Egyptian pound", "LE"), // or E£ or ج.م (Arabic)
                ["ISO-4217::ERN"] = new Currency("ERN", "232", 2, "Eritrean nakfa", "ERN"),
                ["ISO-4217::ETB"] = new Currency("ETB", "230", 2, "Ethiopian birr", "Br"), // (Latin) or ብር (Ethiopic)
                ["ISO-4217::EUR"] = new Currency("EUR", "978", 2, "Euro", "€"),
                ["ISO-4217::FJD"] = new Currency("FJD", "242", 2, "Fiji dollar", "$"), // or FJ$
                ["ISO-4217::FKP"] = new Currency("FKP", "238", 2, "Falkland Islands pound", "£"),
                ["ISO-4217::GBP"] = new Currency("GBP", "826", 2, "Pound sterling", "£"),
                ["ISO-4217::GEL"] = new Currency("GEL", "981", 2, "Georgian lari", "ლ."), // TODO: new symbol since July 18, 2014 => see http://en.wikipedia.org/wiki/Georgian_lari
                ["ISO-4217::GHS"] = new Currency("GHS", "936", 2, "Ghanaian cedi", "GH¢"), // or GH₵
                ["ISO-4217::GIP"] = new Currency("GIP", "292", 2, "Gibraltar pound", "£"),
                ["ISO-4217::GMD"] = new Currency("GMD", "270", 2, "Gambian dalasi", "D"),
                ["ISO-4217::GNF"] = new Currency("GNF", "324", 0, "Guinean Franc", "FG"), // (possibly also Fr or GFr)  GUINEA
                ["ISO-4217::GTQ"] = new Currency("GTQ", "320", 2, "Guatemalan quetzal", "Q"),
                ["ISO-4217::GYD"] = new Currency("GYD", "328", 2, "Guyanese dollar", "$"), // or G$
                ["ISO-4217::HKD"] = new Currency("HKD", "344", 2, "Hong Kong dollar", "HK$"), // or $
                ["ISO-4217::HNL"] = new Currency("HNL", "340", 2, "Honduran lempira", "L"),
                ["ISO-4217::HRK"] = new Currency("HRK", "191", 2, "Croatian kuna", "kn"),
                ["ISO-4217::HTG"] = new Currency("HTG", "332", 2, "Haitian gourde", "G"),
                ["ISO-4217::HUF"] = new Currency("HUF", "348", 2, "Hungarian forint", "Ft"),
                ["ISO-4217::IDR"] = new Currency("IDR", "360", 2, "Indonesian rupiah", "Rp"),
                ["ISO-4217::ILS"] = new Currency("ILS", "376", 2, "Israeli new shekel", "₪"),
                ["ISO-4217::INR"] = new Currency("INR", "356", 2, "Indian rupee", "₹"),
                ["ISO-4217::IQD"] = new Currency("IQD", "368", 3, "Iraqi dinar", "د.ع"),
                ["ISO-4217::IRR"] = new Currency("IRR", "364", 0, "Iranian rial", "ريال"),
                ["ISO-4217::ISK"] = new Currency("ISK", "352", 0, "Icelandic króna", "kr"),
                ["ISO-4217::JMD"] = new Currency("JMD", "388", 2, "Jamaican dollar", "J$"), // or $
                ["ISO-4217::JOD"] = new Currency("JOD", "400", 3, "Jordanian dinar", "د.ا.‏"),
                ["ISO-4217::JPY"] = new Currency("JPY", "392", 0, "Japanese yen", "¥"),
                ["ISO-4217::KES"] = new Currency("KES", "404", 2, "Kenyan shilling", "KSh"),
                ["ISO-4217::KGS"] = new Currency("KGS", "417", 2, "Kyrgyzstani som", "сом"),
                ["ISO-4217::KHR"] = new Currency("KHR", "116", 2, "Cambodian riel", "៛"),
                ["ISO-4217::KMF"] = new Currency("KMF", "174", 0, "Comorian Franc", "CF"), // COMOROS (THE)
                ["ISO-4217::KPW"] = new Currency("KPW", "408", 0, "North Korean won", "₩"),
                ["ISO-4217::KRW"] = new Currency("KRW", "410", 0, "South Korean won", "₩"),
                ["ISO-4217::KWD"] = new Currency("KWD", "414", 3, "Kuwaiti dinar", "د.ك"), // or K.D.
                ["ISO-4217::KYD"] = new Currency("KYD", "136", 2, "Cayman Islands dollar", "$"),
                ["ISO-4217::KZT"] = new Currency("KZT", "398", 2, "Kazakhstani tenge", "₸"),
                ["ISO-4217::LAK"] = new Currency("LAK", "418", 0, "Lao Kip", "₭"), // or ₭N,  LAO PEOPLE’S DEMOCRATIC REPUBLIC(THE), ISO says minor unit=2 but wiki syas Historically, one kip was divided into 100 att (ອັດ).
                ["ISO-4217::LBP"] = new Currency("LBP", "422", 0, "Lebanese pound", "ل.ل"),
                ["ISO-4217::LKR"] = new Currency("LKR", "144", 2, "Sri Lankan rupee", "Rs"), // or රු
                ["ISO-4217::LRD"] = new Currency("LRD", "430", 2, "Liberian dollar", "$"), // or L$, LD$
                ["ISO-4217::LSL"] = new Currency("LSL", "426", 2, "Lesotho loti", "L"), // L or M (pl.)
                ["ISO-4217::LYD"] = new Currency("LYD", "434", 3, "Libyan dinar", "ل.د"), // or LD
                ["ISO-4217::MAD"] = new Currency("MAD", "504", 2, "Moroccan dirham", "د.م."),
                ["ISO-4217::MDL"] = new Currency("MDL", "498", 2, "Moldovan leu", "L"),
                ["ISO-4217::MGA"] = new Currency("MGA", "969", Z07, "Malagasy ariary", "Ar"),  // divided into five subunits rather than by a power of ten. 5 is 10 to the power of 0.69897...
                ["ISO-4217::MKD"] = new Currency("MKD", "807", 0, "Macedonian denar", "ден"),
                ["ISO-4217::MMK"] = new Currency("MMK", "104", 0, "Myanma kyat", "K"),
                ["ISO-4217::MNT"] = new Currency("MNT", "496", 2, "Mongolian tugrik", "₮"),
                ["ISO-4217::MOP"] = new Currency("MOP", "446", 2, "Macanese pataca", "MOP$"),
                ["ISO-4217::MRU"] = new Currency("MRU", "929", Z07, "Mauritanian ouguiya", "UM", validFrom: new DateTime(2018, 01, 01)), // divided into five subunits rather than by a power of ten. 5 is 10 to the power of 0.69897...
                ["ISO-4217::MUR"] = new Currency("MUR", "480", 2, "Mauritian rupee", "Rs"),
                ["ISO-4217::MVR"] = new Currency("MVR", "462", 2, "Maldivian rufiyaa", "Rf"), // or , MRf, MVR, .ރ or /-
                ["ISO-4217::MWK"] = new Currency("MWK", "454", 2, "Malawi kwacha", "MK"),
                ["ISO-4217::MXN"] = new Currency("MXN", "484", 2, "Mexican peso", "$"),
                ["ISO-4217::MXV"] = new Currency("MXV", "979", 2, "Mexican Unidad de Inversion (UDI) (funds code)", Currency.GenericCurrencySign),  // <==== not found
                ["ISO-4217::MYR"] = new Currency("MYR", "458", 2, "Malaysian ringgit", "RM"),
                ["ISO-4217::MZN"] = new Currency("MZN", "943", 2, "Mozambican metical", "MTn"), // or MTN
                ["ISO-4217::NAD"] = new Currency("NAD", "516", 2, "Namibian dollar", "N$"), // or $
                ["ISO-4217::NGN"] = new Currency("NGN", "566", 2, "Nigerian naira", "₦"),
                ["ISO-4217::NIO"] = new Currency("NIO", "558", 2, "Nicaraguan córdoba", "C$"),
                ["ISO-4217::NOK"] = new Currency("NOK", "578", 2, "Norwegian krone", "kr"),
                ["ISO-4217::NPR"] = new Currency("NPR", "524", 2, "Nepalese rupee", "Rs"), // or ₨ or रू
                ["ISO-4217::NZD"] = new Currency("NZD", "554", 2, "New Zealand dollar", "$"),
                ["ISO-4217::OMR"] = new Currency("OMR", "512", 3, "Omani rial", "ر.ع."),
                ["ISO-4217::PAB"] = new Currency("PAB", "590", 2, "Panamanian balboa", "B/."),
                ["ISO-4217::PEN"] = new Currency("PEN", "604", 2, "Peruvian sol", "S/."),
                ["ISO-4217::PGK"] = new Currency("PGK", "598", 2, "Papua New Guinean kina", "K"),
                ["ISO-4217::PHP"] = new Currency("PHP", "608", 2, "Philippine Peso", "₱"), // or P or PHP or PhP
                ["ISO-4217::PKR"] = new Currency("PKR", "586", 2, "Pakistani rupee", "Rs"),
                ["ISO-4217::PLN"] = new Currency("PLN", "985", 2, "Polish złoty", "zł"),
                ["ISO-4217::PYG"] = new Currency("PYG", "600", 0, "Paraguayan guaraní", "₲"),
                ["ISO-4217::QAR"] = new Currency("QAR", "634", 2, "Qatari riyal", "ر.ق"), // or QR
                ["ISO-4217::RON"] = new Currency("RON", "946", 2, "Romanian new leu", "lei"),
                ["ISO-4217::RSD"] = new Currency("RSD", "941", 2, "Serbian dinar", "РСД"), // or RSD (or дин or d./д)
                ["ISO-4217::RUB"] = new Currency("RUB", "643", 2, "Russian rouble", "₽"), // or R or руб (both onofficial)
                ["ISO-4217::RWF"] = new Currency("RWF", "646", 0, "Rwandan franc", "RFw"), // or RF, R₣
                ["ISO-4217::SAR"] = new Currency("SAR", "682", 2, "Saudi riyal", "ر.س"), // or SR (Latin) or ﷼‎ (Unicode)
                ["ISO-4217::SBD"] = new Currency("SBD", "090", 2, "Solomon Islands dollar", "SI$"),
                ["ISO-4217::SCR"] = new Currency("SCR", "690", 2, "Seychelles rupee", "SR"), // or SRe
                ["ISO-4217::SDG"] = new Currency("SDG", "938", 2, "Sudanese pound", "ج.س."),
                ["ISO-4217::SEK"] = new Currency("SEK", "752", 2, "Swedish krona/kronor", "kr"),
                ["ISO-4217::SGD"] = new Currency("SGD", "702", 2, "Singapore dollar", "S$"), // or $
                ["ISO-4217::SHP"] = new Currency("SHP", "654", 2, "Saint Helena pound", "£"),
                ["ISO-4217::SLL"] = new Currency("SLL", "694", 0, "Sierra Leonean leone", "Le"),
                ["ISO-4217::SOS"] = new Currency("SOS", "706", 2, "Somali shilling", "S"), // or Sh.So.
                ["ISO-4217::SRD"] = new Currency("SRD", "968", 2, "Surinamese dollar", "$"),
                ["ISO-4217::SSP"] = new Currency("SSP", "728", 2, "South Sudanese pound", "£"), // not sure about symbol...
                ["ISO-4217::SVC"] = new Currency("SVC", "222", 2, "El Salvador Colon", "₡"),
                ["ISO-4217::SYP"] = new Currency("SYP", "760", 2, "Syrian pound", "ܠ.ܣ.‏"), // or LS or £S (or £)
                ["ISO-4217::SZL"] = new Currency("SZL", "748", 2, "Swazi lilangeni", "L"), // or E (plural)
                ["ISO-4217::THB"] = new Currency("THB", "764", 2, "Thai baht", "฿"),
                ["ISO-4217::TJS"] = new Currency("TJS", "972", 2, "Tajikistani somoni", "смн"),
                ["ISO-4217::TMT"] = new Currency("TMT", "934", 2, "Turkmenistani manat", "m"), // or T?
                ["ISO-4217::TND"] = new Currency("TND", "788", 3, "Tunisian dinar", "د.ت"), // or DT (Latin)
                ["ISO-4217::TOP"] = new Currency("TOP", "776", 2, "Tongan paʻanga", "T$"), // (sometimes PT)
                ["ISO-4217::TRY"] = new Currency("TRY", "949", 2, "Turkish lira", "₺"),
                ["ISO-4217::TTD"] = new Currency("TTD", "780", 2, "Trinidad and Tobago dollar", "$"), // or TT$
                ["ISO-4217::TWD"] = new Currency("TWD", "901", 2, "New Taiwan dollar", "NT$"), // or $
                ["ISO-4217::TZS"] = new Currency("TZS", "834", 2, "Tanzanian shilling", "x/y"), // or TSh
                ["ISO-4217::UAH"] = new Currency("UAH", "980", 2, "Ukrainian hryvnia", "₴"),
                ["ISO-4217::UGX"] = new Currency("UGX", "800", 2, "Ugandan shilling", "USh"),
                ["ISO-4217::USD"] = new Currency("USD", "840", 2, "United States dollar", "$"), // or US$
                ["ISO-4217::USN"] = new Currency("USN", "997", 2, "United States dollar (next day) (funds code)", "$"),
                ["ISO-4217::UYI"] = new Currency("UYI", "940", 0, "Uruguay Peso en Unidades Indexadas (UI) (funds code)", Currency.GenericCurrencySign), // List two
                ["ISO-4217::UYU"] = new Currency("UYU", "858", 2, "Uruguayan peso", "$"), // or $U
                ["ISO-4217::UZS"] = new Currency("UZS", "860", 2, "Uzbekistan som", "лв"), // or сўм ?
                ["ISO-4217::VES"] = new Currency("VES", "928", 2, "Venezuelan Bolívar Soberano", "Bs.", validFrom: new DateTime(2018, 8, 20)), // or Bs.F. , Amendment 167 talks about delay but from multiple sources on the web the date seems to be 20 aug.
                ["ISO-4217::VND"] = new Currency("VND", "704", 0, "Vietnamese dong", "₫"),
                ["ISO-4217::VUV"] = new Currency("VUV", "548", 0, "Vanuatu vatu", "VT"),
                ["ISO-4217::WST"] = new Currency("WST", "882", 2, "Samoan tala", "WS$"), // sometimes SAT, ST or T
                ["ISO-4217::XAF"] = new Currency("XAF", "950", 0, "CFA franc BEAC", "FCFA"),
                ["ISO-4217::XAG"] = new Currency("XAG", "961", NotApplicable, "Silver (one troy ounce)", Currency.GenericCurrencySign),
                ["ISO-4217::XAU"] = new Currency("XAU", "959", NotApplicable, "Gold (one troy ounce)", Currency.GenericCurrencySign),
                ["ISO-4217::XBA"] = new Currency("XBA", "955", NotApplicable, "European Composite Unit (EURCO) (bond market unit)", Currency.GenericCurrencySign),
                ["ISO-4217::XBB"] = new Currency("XBB", "956", NotApplicable, "European Monetary Unit (E.M.U.-6) (bond market unit)", Currency.GenericCurrencySign),
                ["ISO-4217::XBC"] = new Currency("XBC", "957", NotApplicable, "European Unit of Account 9 (E.U.A.-9) (bond market unit)", Currency.GenericCurrencySign),
                ["ISO-4217::XBD"] = new Currency("XBD", "958", NotApplicable, "European Unit of Account 17 (E.U.A.-17) (bond market unit)", Currency.GenericCurrencySign),
                ["ISO-4217::XCD"] = new Currency("XCD", "951", 2, "East Caribbean dollar", "$"), // or EC$
                ["ISO-4217::XDR"] = new Currency("XDR", "960", NotApplicable, "Special drawing rights", Currency.GenericCurrencySign),
                ["ISO-4217::XOF"] = new Currency("XOF", "952", 0, "CFA franc BCEAO", "CFA"),
                ["ISO-4217::XPD"] = new Currency("XPD", "964", NotApplicable, "Palladium (one troy ounce)", Currency.GenericCurrencySign),
                ["ISO-4217::XPF"] = new Currency("XPF", "953", 0, "CFP franc", "F"),
                ["ISO-4217::XPT"] = new Currency("XPT", "962", NotApplicable, "Platinum (one troy ounce)", Currency.GenericCurrencySign),
                ["ISO-4217::XSU"] = new Currency("XSU", "994", NotApplicable, "SUCRE", Currency.GenericCurrencySign),
                ["ISO-4217::XTS"] = new Currency("XTS", "963", NotApplicable, "Code reserved for testing purposes", Currency.GenericCurrencySign),
                ["ISO-4217::XUA"] = new Currency("XUA", "965", NotApplicable, "ADB Unit of Account", Currency.GenericCurrencySign),
                ["ISO-4217::XXX"] = new Currency("XXX", "999", NotApplicable, "No currency", Currency.GenericCurrencySign),
                ["ISO-4217::YER"] = new Currency("YER", "886", 2, "Yemeni rial", "﷼"), // or ر.ي.‏‏ ?
                ["ISO-4217::ZAR"] = new Currency("ZAR", "710", 2, "South African rand", "R"),
                ["ISO-4217::ZMW"] = new Currency("ZMW", "967", 2, "Zambian kwacha", "ZK"), // or ZMW
                ["ISO-4217::ZWL"] = new Currency("ZWL", "932", 2, "Zimbabwean dollar", "$"),
                ["ISO-4217::STN"] = new Currency("STN", "930", 2, "Dobra", "Db", validFrom: new DateTime(2018, 1, 1)), // New Currency of São Tomé and Príncipe from 1 Jan 2018 (Amendment 164)
                ["ISO-4217::STD"] = new Currency("STD", "678", 2, "Dobra", "Db", validTo: new DateTime(2018, 1, 1)), // To be replaced Currency of São Tomé and Príncipe from 1 Jan 2018 (Amendment 164),  inflation has rendered the cêntimo obsolete
                ["ISO-4217::UYW"] = new Currency("UYW", "927", 2, "Unidad Previsional", "Db", validFrom: new DateTime(2018, 8, 29)), // The Central Bank of Uruguay is applying for new Fund currency code (Amendment 169)

                // Historic ISO-4217 currencies (list three)
                ["ISO-4217-HISTORIC::VEF"] = new Currency("VEF", "937", 2, "Venezuelan bolívar", "Bs.", "ISO-4217-HISTORIC", new DateTime(2018, 8, 20)), // replaced by VEF, The conversion rate is 1000 (old) Bolívar to 1 (new) Bolívar Soberano (1000:1). The expiration date of the current bolívar will be defined later and communicated by the Central Bank of Venezuela in due time.
                ["ISO-4217-HISTORIC::MRO"] = new Currency("MRO", "478", Z07, "Mauritanian ouguiya", "UM", "ISO-4217-HISTORIC", new DateTime(2018, 1, 1)), // replaced by MRU
                ["ISO-4217-HISTORIC::ESA"] = new Currency("ESA", "996", NotApplicable, "Spanish peseta (account A)", "Pta", "ISO-4217-HISTORIC", new DateTime(2002, 3, 1)), // replaced by ESP (EUR)
                ["ISO-4217-HISTORIC::ESB"] = new Currency("ESB", "995", NotApplicable, "Spanish peseta (account B)", "Pta", "ISO-4217-HISTORIC", new DateTime(2002, 3, 1)), // replaced by ESP (EUR)
                ["ISO-4217-HISTORIC::LTL"] = new Currency("LTL", "440", 2, "Lithuanian litas", "Lt", "ISO-4217-HISTORIC", new DateTime(2014, 12, 31), new DateTime(1993, 1, 1)), // replaced by EUR
                ["ISO-4217-HISTORIC::USS"] = new Currency("USS", "998", 2, "United States dollar (same day) (funds code)", "$", "ISO-4217-HISTORIC", new DateTime(2014, 3, 28)), // replaced by (no successor)
                ["ISO-4217-HISTORIC::LVL"] = new Currency("LVL", "428", 2, "Latvian lats", "Ls", "ISO-4217-HISTORIC", new DateTime(2013, 12, 31), new DateTime(1992, 1, 1)), // replaced by EUR
                ["ISO-4217-HISTORIC::XFU"] = new Currency("XFU", string.Empty, NotApplicable, "UIC franc (special settlement currency) International Union of Railways", Currency.GenericCurrencySign, "ISO-4217-HISTORIC", new DateTime(2013, 11, 7)), // replaced by EUR
                ["ISO-4217-HISTORIC::ZMK"] = new Currency("ZMK", "894", 2, "Zambian kwacha", "ZK", "ISO-4217-HISTORIC", new DateTime(2013, 1, 1), new DateTime(1968, 1, 16)), // replaced by ZMW
                ["ISO-4217-HISTORIC::EEK"] = new Currency("EEK", "233", 2, "Estonian kroon", "kr", "ISO-4217-HISTORIC", new DateTime(2010, 12, 31), new DateTime(1992, 1, 1)), // replaced by EUR
                ["ISO-4217-HISTORIC::ZWR"] = new Currency("ZWR", "935", 2, "Zimbabwean dollar A/09", "$", "ISO-4217-HISTORIC", new DateTime(2009, 2, 2), new DateTime(2008, 8, 1)), // replaced by ZWL
                ["ISO-4217-HISTORIC::SKK"] = new Currency("SKK", "703", 2, "Slovak koruna", "Sk", "ISO-4217-HISTORIC", new DateTime(2008, 12, 31), new DateTime(1993, 2, 8)), // replaced by EUR
                ["ISO-4217-HISTORIC::TMM"] = new Currency("TMM", "795", 0, "Turkmenistani manat", "T", "ISO-4217-HISTORIC", new DateTime(2008, 12, 31), new DateTime(1993, 11, 1)), // replaced by TMT
                ["ISO-4217-HISTORIC::ZWN"] = new Currency("ZWN", "942", 2, "Zimbabwean dollar A/08", "$", "ISO-4217-HISTORIC", new DateTime(2008, 7, 31), new DateTime(2006, 8, 1)), // replaced by ZWR
                ["ISO-4217-HISTORIC::VEB"] = new Currency("VEB", "862", 2, "Venezuelan bolívar", "Bs.", "ISO-4217-HISTORIC", new DateTime(2008, 1, 1)), // replaced by VEF
                ["ISO-4217-HISTORIC::CYP"] = new Currency("CYP", "196", 2, "Cypriot pound", "£", "ISO-4217-HISTORIC", new DateTime(2007, 12, 31), new DateTime(1879, 1, 1)), // replaced by EUR
                ["ISO-4217-HISTORIC::MTL"] = new Currency("MTL", "470", 2, "Maltese lira", "₤", "ISO-4217-HISTORIC", new DateTime(2007, 12, 31), new DateTime(1972, 1, 1)), // replaced by EUR
                ["ISO-4217-HISTORIC::GHC"] = new Currency("GHC", "288", 0, "Ghanaian cedi", "GH₵", "ISO-4217-HISTORIC", new DateTime(2007, 7, 1), new DateTime(1967, 1, 1)), // replaced by GHS
                ["ISO-4217-HISTORIC::SDD"] = new Currency("SDD", "736", NotApplicable, "Sudanese dinar", "£Sd", "ISO-4217-HISTORIC", new DateTime(2007, 1, 10), new DateTime(1992, 6, 8)), // replaced by SDG
                ["ISO-4217-HISTORIC::SIT"] = new Currency("SIT", "705", 2, "Slovenian tolar", Currency.GenericCurrencySign, "ISO-4217-HISTORIC", new DateTime(2006, 12, 31), new DateTime(1991, 10, 8)), // replaced by EUR
                ["ISO-4217-HISTORIC::ZWD"] = new Currency("ZWD", "716", 2, "Zimbabwean dollar A/06", "$", "ISO-4217-HISTORIC", new DateTime(2006, 7, 31), new DateTime(1980, 4, 18)), // replaced by ZWN
                ["ISO-4217-HISTORIC::MZM"] = new Currency("MZM", "508", 0, "Mozambican metical", "MT", "ISO-4217-HISTORIC", new DateTime(2006, 6, 30), new DateTime(1980, 1, 1)), // replaced by MZN
                ["ISO-4217-HISTORIC::AZM"] = new Currency("AZM", "031", 0, "Azerbaijani manat", "₼", "ISO-4217-HISTORIC", new DateTime(2006, 1, 1), new DateTime(1992, 8, 15)), // replaced by AZN
                ["ISO-4217-HISTORIC::CSD"] = new Currency("CSD", "891", 2, "Serbian dinar", Currency.GenericCurrencySign, "ISO-4217-HISTORIC", new DateTime(2006, 12, 31), new DateTime(2003, 7, 3)), // replaced by RSD
                ["ISO-4217-HISTORIC::MGF"] = new Currency("MGF", "450", 2, "Malagasy franc", Currency.GenericCurrencySign, "ISO-4217-HISTORIC", new DateTime(2005, 1, 1), new DateTime(1963, 7, 1)), // replaced by MGA
                ["ISO-4217-HISTORIC::ROL"] = new Currency("ROL", "642", NotApplicable, "Romanian leu A/05", Currency.GenericCurrencySign, "ISO-4217-HISTORIC", new DateTime(2005, 12, 31), new DateTime(1952, 1, 28)), // replaced by RON
                ["ISO-4217-HISTORIC::TRL"] = new Currency("TRL", "792", 0, "Turkish lira A/05", "₺", "ISO-4217-HISTORIC", new DateTime(2005, 12, 31)), // replaced by TRY
                ["ISO-4217-HISTORIC::SRG"] = new Currency("SRG", "740", NotApplicable, "Suriname guilder", "ƒ", "ISO-4217-HISTORIC", new DateTime(2004, 12, 31)), // replaced by SRD
                ["ISO-4217-HISTORIC::YUM"] = new Currency("YUM", "891", 2, "Yugoslav dinar", "дин.", "ISO-4217-HISTORIC", new DateTime(2003, 7, 2), new DateTime(1994, 1, 24)), // replaced by CSD
                ["ISO-4217-HISTORIC::AFA"] = new Currency("AFA", "004", NotApplicable, "Afghan afghani", "؋", "ISO-4217-HISTORIC", new DateTime(2003, 12, 31), new DateTime(1925, 1, 1)), // replaced by AFN
                ["ISO-4217-HISTORIC::XFO"] = new Currency("XFO", string.Empty, NotApplicable, "Gold franc (special settlement currency)", Currency.GenericCurrencySign, "ISO-4217-HISTORIC", new DateTime(2003, 12, 31), new DateTime(1803, 1, 1)), // replaced by XDR
                ["ISO-4217-HISTORIC::GRD"] = new Currency("GRD", "300", 2, "Greek drachma", "₯", "ISO-4217-HISTORIC", new DateTime(2000, 12, 31), new DateTime(1954, 1, 1)), // replaced by EUR
                ["ISO-4217-HISTORIC::TJR"] = new Currency("TJR", "762", NotApplicable, "Tajikistani ruble", Currency.GenericCurrencySign, "ISO-4217-HISTORIC", new DateTime(2000, 10, 30), new DateTime(1995, 5, 10)), // replaced by TJS
                ["ISO-4217-HISTORIC::ECV"] = new Currency("ECV", "983", NotApplicable, "Ecuador Unidad de Valor Constante (funds code)", Currency.GenericCurrencySign, "ISO-4217-HISTORIC", new DateTime(2000, 1, 9), new DateTime(1993, 1, 1)), // replaced by (no successor)
                ["ISO-4217-HISTORIC::ECS"] = new Currency("ECS", "218", 0, "Ecuadorian sucre", "S/.", "ISO-4217-HISTORIC", new DateTime(2000, 12, 31), new DateTime(1884, 1, 1)), // replaced by USD
                ["ISO-4217-HISTORIC::BYB"] = new Currency("BYB", "112", 2, "Belarusian ruble", "Br", "ISO-4217-HISTORIC", new DateTime(1999, 12, 31), new DateTime(1992, 1, 1)), // replaced by BYR
                ["ISO-4217-HISTORIC::AOR"] = new Currency("AOR", "982", 0, "Angolan kwanza readjustado", "Kz", "ISO-4217-HISTORIC", new DateTime(1999, 11, 30), new DateTime(1995, 7, 1)), // replaced by AOA
                ["ISO-4217-HISTORIC::BGL"] = new Currency("BGL", "100", 2, "Bulgarian lev A/99", "лв.", "ISO-4217-HISTORIC", new DateTime(1999, 7, 5), new DateTime(1962, 1, 1)), // replaced by BGN
                ["ISO-4217-HISTORIC::ADF"] = new Currency("ADF", string.Empty, 2, "Andorran franc (1:1 peg to the French franc)", "Fr", "ISO-4217-HISTORIC", new DateTime(1998, 12, 31), new DateTime(1960, 1, 1)), // replaced by EUR
                ["ISO-4217-HISTORIC::ADP"] = new Currency("ADP", "020", 0, "Andorran peseta (1:1 peg to the Spanish peseta)", Currency.GenericCurrencySign, "ISO-4217-HISTORIC", new DateTime(1998, 12, 31), new DateTime(1869, 1, 1)), // replaced by EUR
                ["ISO-4217-HISTORIC::ATS"] = new Currency("ATS", "040", 2, "Austrian schilling", "öS", "ISO-4217-HISTORIC", new DateTime(1998, 12, 31), new DateTime(1945, 1, 1)), // replaced by EUR
                ["ISO-4217-HISTORIC::BEF"] = new Currency("BEF", "056", 2, "Belgian franc (currency union with LUF)", "fr.", "ISO-4217-HISTORIC", new DateTime(1998, 12, 31), new DateTime(1832, 1, 1)), // replaced by EUR
                ["ISO-4217-HISTORIC::DEM"] = new Currency("DEM", "276", 2, "German mark", "DM", "ISO-4217-HISTORIC", new DateTime(1998, 12, 31), new DateTime(1948, 1, 1)), // replaced by EUR
                ["ISO-4217-HISTORIC::ESP"] = new Currency("ESP", "724", 0, "Spanish peseta", "Pta", "ISO-4217-HISTORIC", new DateTime(1998, 12, 31), new DateTime(1869, 1, 1)), // replaced by EUR
                ["ISO-4217-HISTORIC::FIM"] = new Currency("FIM", "246", 2, "Finnish markka", "mk", "ISO-4217-HISTORIC", new DateTime(1998, 12, 31), new DateTime(1860, 1, 1)), // replaced by EUR
                ["ISO-4217-HISTORIC::FRF"] = new Currency("FRF", "250", 2, "French franc", "Fr", "ISO-4217-HISTORIC", new DateTime(1998, 12, 31), new DateTime(1960, 1, 1)), // replaced by EUR
                ["ISO-4217-HISTORIC::IEP"] = new Currency("IEP", "372", 2, "Irish pound (punt in Irish language)", "£", "ISO-4217-HISTORIC", new DateTime(1998, 12, 31), new DateTime(1938, 1, 1)), // replaced by EUR
                ["ISO-4217-HISTORIC::ITL"] = new Currency("ITL", "380", 0, "Italian lira", "₤", "ISO-4217-HISTORIC", new DateTime(1998, 12, 31), new DateTime(1861, 1, 1)), // replaced by EUR
                ["ISO-4217-HISTORIC::LUF"] = new Currency("LUF", "442", 2, "Luxembourg franc (currency union with BEF)", "fr.", "ISO-4217-HISTORIC", new DateTime(1998, 12, 31), new DateTime(1944, 1, 1)), // replaced by EUR
                ["ISO-4217-HISTORIC::MCF"] = new Currency("MCF", string.Empty, 2, "Monegasque franc (currency union with FRF)", "fr.", "ISO-4217-HISTORIC", new DateTime(1998, 12, 31), new DateTime(1960, 1, 1)), // replaced by EUR
                ["ISO-4217-HISTORIC::NLG"] = new Currency("NLG", "528", 2, "Dutch guilder", "ƒ", "ISO-4217-HISTORIC", new DateTime(1998, 12, 31), new DateTime(1810, 1, 1)), // replaced by EUR
                ["ISO-4217-HISTORIC::PTE"] = new Currency("PTE", "620", 0, "Portuguese escudo", "$", "ISO-4217-HISTORIC", new DateTime(1998, 12, 31), new DateTime(4160, 1, 1)), // replaced by EUR
                ["ISO-4217-HISTORIC::SML"] = new Currency("SML", string.Empty, 0, "San Marinese lira (currency union with ITL and VAL)", "₤", "ISO-4217-HISTORIC", new DateTime(1998, 12, 31), new DateTime(1864, 1, 1)), // replaced by EUR
                ["ISO-4217-HISTORIC::VAL"] = new Currency("VAL", string.Empty, 0, "Vatican lira (currency union with ITL and SML)", "₤", "ISO-4217-HISTORIC", new DateTime(1998, 12, 31), new DateTime(1929, 1, 1)), // replaced by EUR
                ["ISO-4217-HISTORIC::XEU"] = new Currency("XEU", "954", NotApplicable, "European Currency Unit (1 XEU = 1 EUR)", "ECU", "ISO-4217-HISTORIC", new DateTime(1998, 12, 31), new DateTime(1979, 3, 13)), // replaced by EUR
                ["ISO-4217-HISTORIC::BAD"] = new Currency("BAD", string.Empty, 2, "Bosnia and Herzegovina dinar", Currency.GenericCurrencySign, "ISO-4217-HISTORIC", new DateTime(1998, 12, 31), new DateTime(1992, 7, 1)), // replaced by BAM
                ["ISO-4217-HISTORIC::RUR"] = new Currency("RUR", "810", 2, "Russian ruble A/97", "₽", "ISO-4217-HISTORIC", new DateTime(1997, 12, 31), new DateTime(1992, 1, 1)), // replaced by RUB
                ["ISO-4217-HISTORIC::GWP"] = new Currency("GWP", "624", NotApplicable, "Guinea-Bissau peso", Currency.GenericCurrencySign, "ISO-4217-HISTORIC", new DateTime(1997, 12, 31), new DateTime(1975, 1, 1)), // replaced by XOF
                ["ISO-4217-HISTORIC::ZRN"] = new Currency("ZRN", "180", 2, "Zaïrean new zaïre", "Ƶ", "ISO-4217-HISTORIC", new DateTime(1997, 12, 31), new DateTime(1993, 1, 1)), // replaced by CDF
                ["ISO-4217-HISTORIC::UAK"] = new Currency("UAK", "804", NotApplicable, "Ukrainian karbovanets", Currency.GenericCurrencySign, "ISO-4217-HISTORIC", new DateTime(1996, 9, 1), new DateTime(1992, 10, 1)), // replaced by UAH
                ["ISO-4217-HISTORIC::YDD"] = new Currency("YDD", "720", NotApplicable, "South Yemeni dinar", Currency.GenericCurrencySign, "ISO-4217-HISTORIC", new DateTime(1996, 6, 11)), // replaced by YER
                ["ISO-4217-HISTORIC::AON"] = new Currency("AON", "024", 0, "Angolan new kwanza", "Kz", "ISO-4217-HISTORIC", new DateTime(1995, 6, 30), new DateTime(1990, 9, 25)), // replaced by AOR
                ["ISO-4217-HISTORIC::ZAL"] = new Currency("ZAL", "991", NotApplicable, "South African financial rand (funds code)", Currency.GenericCurrencySign, "ISO-4217-HISTORIC", new DateTime(1995, 3, 13), new DateTime(1985, 9, 1)), // replaced by (no successor)
                ["ISO-4217-HISTORIC::PLZ"] = new Currency("PLZ", "616", NotApplicable, "Polish zloty A/94", "zł", "ISO-4217-HISTORIC", new DateTime(1994, 12, 31), new DateTime(1950, 10, 30)), // replaced by PLN
                ["ISO-4217-HISTORIC::BRR"] = new Currency("BRR", string.Empty, 2, "Brazilian cruzeiro real", "CR$", "ISO-4217-HISTORIC", new DateTime(1994, 6, 30), new DateTime(1993, 8, 1)), // replaced by BRL
                ["ISO-4217-HISTORIC::HRD"] = new Currency("HRD", string.Empty, NotApplicable, "Croatian dinar", Currency.GenericCurrencySign, "ISO-4217-HISTORIC", new DateTime(1994, 5, 30), new DateTime(1991, 12, 23)), // replaced by HRK
                ["ISO-4217-HISTORIC::YUG"] = new Currency("YUG", string.Empty, 2, "Yugoslav dinar", "дин.", "ISO-4217-HISTORIC", new DateTime(1994, 1, 23), new DateTime(1994, 1, 1)), // replaced by YUM
                ["ISO-4217-HISTORIC::YUO"] = new Currency("YUO", string.Empty, 2, "Yugoslav dinar", "дин.", "ISO-4217-HISTORIC", new DateTime(1993, 12, 31), new DateTime(1993, 10, 1)), // replaced by YUG
                ["ISO-4217-HISTORIC::YUR"] = new Currency("YUR", string.Empty, 2, "Yugoslav dinar", "дин.", "ISO-4217-HISTORIC", new DateTime(1993, 9, 30), new DateTime(1992, 7, 1)), // replaced by YUO
                ["ISO-4217-HISTORIC::BRE"] = new Currency("BRE", string.Empty, 2, "Brazilian cruzeiro", "₢", "ISO-4217-HISTORIC", new DateTime(1993, 8, 1), new DateTime(1990, 3, 15)), // replaced by BRR
                ["ISO-4217-HISTORIC::UYN"] = new Currency("UYN", "858", NotApplicable, "Uruguay Peso", "$U", "ISO-4217-HISTORIC", new DateTime(1993, 3, 1), new DateTime(1975, 7, 1)), // replaced by UYU
                ["ISO-4217-HISTORIC::CSK"] = new Currency("CSK", "200", NotApplicable, "Czechoslovak koruna", "Kčs", "ISO-4217-HISTORIC", new DateTime(1993, 2, 8), new DateTime(7040, 1, 1)), // replaced by CZK and SKK (CZK and EUR)
                ["ISO-4217-HISTORIC::MKN"] = new Currency("MKN", string.Empty, NotApplicable, "Old Macedonian denar A/93", "ден", "ISO-4217-HISTORIC", new DateTime(1993, 12, 31)), // replaced by MKD
                ["ISO-4217-HISTORIC::MXP"] = new Currency("MXP", "484", NotApplicable, "Mexican peso", "$", "ISO-4217-HISTORIC", new DateTime(1993, 12, 31)), // replaced by MXN
                ["ISO-4217-HISTORIC::ZRZ"] = new Currency("ZRZ", string.Empty, 3, "Zaïrean zaïre", "Ƶ", "ISO-4217-HISTORIC", new DateTime(1993, 12, 31), new DateTime(1967, 1, 1)), // replaced by ZRN
                ["ISO-4217-HISTORIC::YUN"] = new Currency("YUN", string.Empty, 2, "Yugoslav dinar", "дин.", "ISO-4217-HISTORIC", new DateTime(1992, 6, 30), new DateTime(1990, 1, 1)), // replaced by YUR
                ["ISO-4217-HISTORIC::SDP"] = new Currency("SDP", "736", NotApplicable, "Sudanese old pound", "ج.س.", "ISO-4217-HISTORIC", new DateTime(1992, 6, 8), new DateTime(1956, 1, 1)), // replaced by SDD
                ["ISO-4217-HISTORIC::ARA"] = new Currency("ARA", string.Empty, 2, "Argentine austral", "₳", "ISO-4217-HISTORIC", new DateTime(1991, 12, 31), new DateTime(1985, 6, 15)), // replaced by ARS
                ["ISO-4217-HISTORIC::PEI"] = new Currency("PEI", string.Empty, NotApplicable, "Peruvian inti", "I/.", "ISO-4217-HISTORIC", new DateTime(1991, 10, 1), new DateTime(1985, 2, 1)), // replaced by PEN
                ["ISO-4217-HISTORIC::SUR"] = new Currency("SUR", "810", NotApplicable, "Soviet Union Ruble", "руб", "ISO-4217-HISTORIC", new DateTime(1991, 12, 31), new DateTime(1961, 1, 1)), // replaced by RUR
                ["ISO-4217-HISTORIC::AOK"] = new Currency("AOK", "024", 0, "Angolan kwanza", "Kz", "ISO-4217-HISTORIC", new DateTime(1990, 9, 24), new DateTime(1977, 1, 8)), // replaced by AON
                ["ISO-4217-HISTORIC::DDM"] = new Currency("DDM", "278", NotApplicable, "East German Mark of the GDR (East Germany)", Currency.GenericCurrencySign, "ISO-4217-HISTORIC", new DateTime(1990, 7, 1), new DateTime(1948, 6, 21)), // replaced by DEM (EUR)
                ["ISO-4217-HISTORIC::BRN"] = new Currency("BRN", string.Empty, 2, "Brazilian cruzado novo", "NCz$", "ISO-4217-HISTORIC", new DateTime(1990, 3, 15), new DateTime(1989, 1, 16)), // replaced by BRE
                ["ISO-4217-HISTORIC::YUD"] = new Currency("YUD", "891", 2, "New Yugoslavian Dinar", "дин.", "ISO-4217-HISTORIC", new DateTime(1989, 12, 31), new DateTime(1966, 1, 1)), // replaced by YUN
                ["ISO-4217-HISTORIC::BRC"] = new Currency("BRC", string.Empty, 2, "Brazilian cruzado", "Cz$", "ISO-4217-HISTORIC", new DateTime(1989, 1, 15), new DateTime(1986, 2, 28)), // replaced by BRN
                ["ISO-4217-HISTORIC::BOP"] = new Currency("BOP", "068", 2, "Peso boliviano", "b$.", "ISO-4217-HISTORIC", new DateTime(1987, 1, 1), new DateTime(1963, 1, 1)), // replaced by BOB
                ["ISO-4217-HISTORIC::UGS"] = new Currency("UGS", "800", NotApplicable, "Ugandan shilling A/87", "USh", "ISO-4217-HISTORIC", new DateTime(1987, 12, 31)), // replaced by UGX
                ["ISO-4217-HISTORIC::BRB"] = new Currency("BRB", "076", 2, "Brazilian cruzeiro", "₢", "ISO-4217-HISTORIC", new DateTime(1986, 2, 28), new DateTime(1970, 1, 1)), // replaced by BRC
                ["ISO-4217-HISTORIC::ILR"] = new Currency("ILR", "376", 2, "Israeli shekel", Currency.GenericCurrencySign, "ISO-4217-HISTORIC", new DateTime(1985, 12, 31), new DateTime(1980, 2, 24)), // replaced by ILS
                ["ISO-4217-HISTORIC::ARP"] = new Currency("ARP", string.Empty, 2, "Argentine peso argentino", "$a", "ISO-4217-HISTORIC", new DateTime(1985, 6, 14), new DateTime(1983, 6, 6)), // replaced by ARA
                ["ISO-4217-HISTORIC::PEH"] = new Currency("PEH", "604", NotApplicable, "Peruvian old sol", Currency.GenericCurrencySign, "ISO-4217-HISTORIC", new DateTime(1985, 2, 1), new DateTime(1863, 1, 1)), // replaced by PEI
                ["ISO-4217-HISTORIC::GQE"] = new Currency("GQE", string.Empty, NotApplicable, "Equatorial Guinean ekwele", Currency.GenericCurrencySign, "ISO-4217-HISTORIC", new DateTime(1985, 12, 31), new DateTime(1975, 1, 1)), // replaced by XAF
                ["ISO-4217-HISTORIC::GNE"] = new Currency("GNE", "324", NotApplicable, "Guinean syli", Currency.GenericCurrencySign, "ISO-4217-HISTORIC", new DateTime(1985, 12, 31), new DateTime(1971, 1, 1)), // replaced by GNF
                ["ISO-4217-HISTORIC::MLF"] = new Currency("MLF", string.Empty, NotApplicable, "Mali franc", "MAF", "ISO-4217-HISTORIC", new DateTime(1984, 12, 31)), // replaced by XOF
                ["ISO-4217-HISTORIC::ARL"] = new Currency("ARL", string.Empty, 2, "Argentine peso ley", "$L", "ISO-4217-HISTORIC", new DateTime(1983, 5, 5), new DateTime(1970, 1, 1)), // replaced by ARP
                ["ISO-4217-HISTORIC::ISJ"] = new Currency("ISJ", "352", 2, "Icelandic krona", "kr", "ISO-4217-HISTORIC", new DateTime(1981, 12, 31), new DateTime(1922, 1, 1)), // replaced by ISK
                ["ISO-4217-HISTORIC::MVQ"] = new Currency("MVQ", "462", NotApplicable, "Maldivian rupee", "Rf", "ISO-4217-HISTORIC", new DateTime(1981, 12, 31)), // replaced by MVR
                ["ISO-4217-HISTORIC::ILP"] = new Currency("ILP", "376", 3, "Israeli lira", "I£", "ISO-4217-HISTORIC", new DateTime(1980, 12, 31), new DateTime(1948, 1, 1)), // ISRAEL Pound,  replaced by ILR
                ["ISO-4217-HISTORIC::ZWC"] = new Currency("ZWC", "716", 2, "Rhodesian dollar", "$", "ISO-4217-HISTORIC", new DateTime(1980, 12, 31), new DateTime(1970, 2, 17)), // replaced by ZWD
                ["ISO-4217-HISTORIC::LAJ"] = new Currency("LAJ", "418", NotApplicable, "Pathet Lao Kip", "₭", "ISO-4217-HISTORIC", new DateTime(1979, 12, 31)), // replaced by LAK
                ["ISO-4217-HISTORIC::TPE"] = new Currency("TPE", string.Empty, NotApplicable, "Portuguese Timorese escudo", Currency.GenericCurrencySign, "ISO-4217-HISTORIC", new DateTime(1976, 12, 31), new DateTime(1959, 1, 1)), // replaced by IDR
                ["ISO-4217-HISTORIC::UYP"] = new Currency("UYP", "858", NotApplicable, "Uruguay Peso", "$", "ISO-4217-HISTORIC", new DateTime(1975, 7, 1), new DateTime(1896, 1, 1)), // replaced by UYN
                ["ISO-4217-HISTORIC::CLE"] = new Currency("CLE", string.Empty, NotApplicable, "Chilean escudo", "Eº", "ISO-4217-HISTORIC", new DateTime(1975, 12, 31), new DateTime(1960, 1, 1)), // replaced by CLP
                ["ISO-4217-HISTORIC::MAF"] = new Currency("MAF", string.Empty, NotApplicable, "Moroccan franc", Currency.GenericCurrencySign, "ISO-4217-HISTORIC", new DateTime(1976, 12, 31), new DateTime(1921, 1, 1)), // replaced by MAD
                ["ISO-4217-HISTORIC::PTP"] = new Currency("PTP", string.Empty, NotApplicable, "Portuguese Timorese pataca", Currency.GenericCurrencySign, "ISO-4217-HISTORIC", new DateTime(1958, 12, 31), new DateTime(1894, 1, 1)), // replaced by TPE
                ["ISO-4217-HISTORIC::TNF"] = new Currency("TNF", string.Empty, 2, "Tunisian franc", "F", "ISO-4217-HISTORIC", new DateTime(1958, 12, 31), new DateTime(1991, 7, 1)), // replaced by TND
                ["ISO-4217-HISTORIC::NFD"] = new Currency("NFD", string.Empty, 2, "Newfoundland dollar", "$", "ISO-4217-HISTORIC", new DateTime(1949, 12, 31), new DateTime(1865, 1, 1)), // replaced by CAD

                // Added historic currencies of amendment 164 (research dates and other info)
                ["ISO-4217-HISTORIC::VNC"] = new Currency("VNC", "704", 2, "Dong", "$", "ISO-4217-HISTORIC"), // VIETNAM
                ["ISO-4217-HISTORIC::GNS"] = new Currency("GNS", "324", NotApplicable, "Guinean Syli", Currency.GenericCurrencySign, "ISO-4217-HISTORIC", new DateTime(1970, 12, 31)), // GUINEA, replaced by GNE?
                ["ISO-4217-HISTORIC::UGW"] = new Currency("UGW", "800", NotApplicable, "Old Shilling", Currency.GenericCurrencySign, "ISO-4217-HISTORIC", new DateTime(2017, 9, 22)), // UGANDA
                ["ISO-4217-HISTORIC::RHD"] = new Currency("RHD", "716", NotApplicable, "Rhodesian Dollar", Currency.GenericCurrencySign, "ISO-4217-HISTORIC", new DateTime(2017, 9, 22)), // SOUTHERN RHODESIA
                ["ISO-4217-HISTORIC::ROK"] = new Currency("ROK", "642", NotApplicable, "Leu A/52", Currency.GenericCurrencySign, "ISO-4217-HISTORIC", new DateTime(2017, 9, 22)), // ROMANIA
                ["ISO-4217-HISTORIC::NIC"] = new Currency("NIC", "558", NotApplicable, "Cordoba", Currency.GenericCurrencySign, "ISO-4217-HISTORIC", new DateTime(2017, 9, 22)), // NICARAGUA
                ["ISO-4217-HISTORIC::MZE"] = new Currency("MZE", "508", NotApplicable, "Mozambique Escudo", Currency.GenericCurrencySign, "ISO-4217-HISTORIC", new DateTime(2017, 9, 22)), // MOZAMBIQUE
                ["ISO-4217-HISTORIC::MTP"] = new Currency("MTP", "470", NotApplicable, "Maltese Pound", Currency.GenericCurrencySign, "ISO-4217-HISTORIC", new DateTime(2017, 9, 22)), // MALTA
                ["ISO-4217-HISTORIC::LSM"] = new Currency("LSM", "426", NotApplicable, "Loti", Currency.GenericCurrencySign, "ISO-4217-HISTORIC", new DateTime(2017, 9, 22)), // LESOTHO
                ["ISO-4217-HISTORIC::GWE"] = new Currency("GWE", "624", NotApplicable, "Guinea Escudo", Currency.GenericCurrencySign, "ISO-4217-HISTORIC", new DateTime(2017, 9, 22)), // GUINEA-BISSAU
                ["ISO-4217-HISTORIC::CSJ"] = new Currency("CSJ", "203", NotApplicable, "Krona A/53", Currency.GenericCurrencySign, "ISO-4217-HISTORIC", new DateTime(2017, 9, 22)), // CZECHOSLOVAKIA
                ["ISO-4217-HISTORIC::BUK"] = new Currency("BUK", "104", NotApplicable, "Kyat", Currency.GenericCurrencySign, "ISO-4217-HISTORIC", new DateTime(2017, 9, 22)), // BURMA
                ["ISO-4217-HISTORIC::BGK"] = new Currency("BGK", "100", NotApplicable, "Lev A / 62", Currency.GenericCurrencySign, "ISO-4217-HISTORIC", new DateTime(2017, 9, 22)), // BULGARIA
                ["ISO-4217-HISTORIC::BGJ"] = new Currency("BGJ", "100", NotApplicable, "Lev A / 52", Currency.GenericCurrencySign, "ISO-4217-HISTORIC", new DateTime(2017, 9, 22)), // BULGARIA
                ["ISO-4217-HISTORIC::ARY"] = new Currency("ARY", "032", NotApplicable, "Peso", Currency.GenericCurrencySign, "ISO-4217-HISTORIC", new DateTime(2017, 9, 22)), // ARGENTINA
            };
        }
    }
}
