using System;
using System.Collections.Generic;
using System.Linq;

namespace NodaMoney
{
    /// <summary>Represent the central thread-safe registry for currencies.</summary>
    internal class CurrencyRegistry
    {
        // The Malagasy ariary and the Mauritanian ouguiya are technically divided into five subunits (the iraimbilanja and
        // khoum respectively), rather than by a power of ten. The coins display "1/5" on their face and are referred to as
        // a "fifth" (Khoum/cinquième). These are not used in practice, but when written out, a single significant digit is
        // used. E.g. 1.2 UM.
        // To represent this in decimal we do the following steps: 5 is 10 to the power of log(5) = 0.69897... ~ 0.7
        internal const double Z07 = 0.69897000433601880478626110527551; // Math.Log10(5);
        internal const double NotApplicable = -1;
        private static readonly object SyncLock = new object(); // TODO: Replace with ReaderWriterLock?
        private static readonly Dictionary<string, Currency> Currencies = new Dictionary<string, Currency>(InitializeIsoCurrencies());
        private static readonly Dictionary<string, byte> Namespaces = new Dictionary<string, byte>(new Dictionary<string, byte> { ["ISO-4217"] = default(byte) });

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
            lock (SyncLock)
            {
                foreach (var ns in Namespaces.Keys)
                {
                    Currency c;
                    if (Currencies.TryGetValue(ns + "::" + code, out c))
                    {
                        found.Add(c);
                    }
                }
            }

            if (found.Count == 0)
            {
                currency = default(Currency);
                return false;
            }

            currency = found[0]; // TODO: If more than one, sort by prio. 
            return true;
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

            lock (SyncLock)
            {
                return Currencies.TryGetValue(@namespace + "::" + code, out currency); // don't use string.Format(), string concat much faster in this case!
            }
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

            lock (SyncLock)
            {
                Namespaces[@namespace] = default(byte);
                if (!Currencies.ContainsKey(@namespace + "::" + code))
                {
                    Currencies.Add(@namespace + "::" + code, currency);
                    return true;
                }
            }

            return false;
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

            lock (SyncLock)
            {
                // Namespaces[@namespace] = null; // TODO: Count currencies in namespace and when zero, remove namespace
                if (Currencies.TryGetValue(@namespace + "::" + code, out currency))
                {
                    Currencies.Remove(@namespace + "::" + code);
                    return true;
                }
            }

            return false;
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
            var currencies = new Dictionary<string, Currency>
                                 {
                                     { "ISO-4217::AED", new Currency("AED", "784", 2, "United Arab Emirates dirham", "د.إ") },
                                     { "ISO-4217::AFN", new Currency("AFN", "971", 2, "Afghan afghani", "؋") },
                                     { "ISO-4217::ALL", new Currency("ALL", "008", 2, "Albanian lek", "Lek") },
                                     { "ISO-4217::AMD", new Currency("AMD", "051", 2, "Armenian dram", "֏") },
                                     { "ISO-4217::ANG", new Currency("ANG", "532", 2, "Netherlands Antillean guilder", "ƒ") },
                                     { "ISO-4217::AOA", new Currency("AOA", "973", 2, "Angolan kwanza", "Kz") },
                                     { "ISO-4217::ARS", new Currency("ARS", "032", 2, "Argentine peso", "$") },
                                     { "ISO-4217::AUD", new Currency("AUD", "036", 2, "Australian dollar", "$") },
                                     { "ISO-4217::AWG", new Currency("AWG", "533", 2, "Aruban florin", "ƒ") },
                                     { "ISO-4217::AZN", new Currency("AZN", "944", 2, "Azerbaijani manat", "ман") },
                                     { "ISO-4217::BAM", new Currency("BAM", "977", 2, "Bosnia and Herzegovina convertible mark", "KM") },
                                     { "ISO-4217::BBD", new Currency("BBD", "052", 2, "Barbados dollar", "$") },
                                     { "ISO-4217::BDT", new Currency("BDT", "050", 2, "Bangladeshi taka", "৳") }, // or Tk
                                     { "ISO-4217::BGN", new Currency("BGN", "975", 2, "Bulgarian lev", "лв") },
                                     { "ISO-4217::BHD", new Currency("BHD", "048", 3, "Bahraini dinar", "BD") }, // or د.ب. (switched for unit tests to work)
                                     { "ISO-4217::BIF", new Currency("BIF", "108", 0, "Burundian franc", "FBu") },
                                     { "ISO-4217::BMD", new Currency("BMD", "060", 2, "Bermudian dollar", "$") },
                                     { "ISO-4217::BND", new Currency("BND", "096", 2, "Brunei dollar", "$") }, // or B$
                                     { "ISO-4217::BOB", new Currency("BOB", "068", 2, "Boliviano", "Bs.") }, // or BS or $b
                                     { "ISO-4217::BOV", new Currency("BOV", "984", 2, "Bolivian Mvdol (funds code)", "¤") }, // <==== not found
                                     { "ISO-4217::BRL", new Currency("BRL", "986", 2, "Brazilian real", "R$") },
                                     { "ISO-4217::BSD", new Currency("BSD", "044", 2, "Bahamian dollar", "$") },
                                     { "ISO-4217::BTN", new Currency("BTN", "064", 2, "Bhutanese ngultrum", "Nu.") },
                                     { "ISO-4217::BWP", new Currency("BWP", "072", 2, "Botswana pula", "P") },
                                     { "ISO-4217::BYR", new Currency("BYR", "974", 0, "Belarusian ruble", "Br") }, // or p.? wiki controdicts with xe.com
                                     { "ISO-4217::BZD", new Currency("BZD", "084", 2, "Belize dollar", "BZ$") },
                                     { "ISO-4217::CAD", new Currency("CAD", "124", 2, "Canadian dollar", "$") },
                                     { "ISO-4217::CDF", new Currency("CDF", "976", 2, "Congolese franc", "FC") },
                                     { "ISO-4217::CHE", new Currency("CHE", "947", 2, "WIR Euro (complementary currency)", "CHE") },
                                     { "ISO-4217::CHF", new Currency("CHF", "756", 2, "Swiss franc", "fr.") }, // or CHF
                                     { "ISO-4217::CHW", new Currency("CHW", "948", 2, "WIR Franc (complementary currency)", "CHW") },
                                     { "ISO-4217::CLF", new Currency("CLF", "990", 4, "Unidad de Fomento (funds code)", "CLF") },
                                     { "ISO-4217::CLP", new Currency("CLP", "152", 0, "Chilean peso", "$") },
                                     { "ISO-4217::CNY", new Currency("CNY", "156", 2, "Chinese yuan", "¥") },
                                     { "ISO-4217::COP", new Currency("COP", "170", 2, "Colombian peso", "$") },
                                     { "ISO-4217::COU", new Currency("COU", "970", 2, "Unidad de Valor Real", "¤") }, // ???
                                     { "ISO-4217::CRC", new Currency("CRC", "188", 2, "Costa Rican colon", "₡") },
                                     { "ISO-4217::CUC", new Currency("CUC", "931", 2, "Cuban convertible peso", "CUC$") }, // $ or CUC
                                     { "ISO-4217::CUP", new Currency("CUP", "192", 2, "Cuban peso", "$") }, // or ₱ (obsolete?)
                                     { "ISO-4217::CVE", new Currency("CVE", "132", 0, "Cape Verde escudo", "$") },
                                     { "ISO-4217::CZK", new Currency("CZK", "203", 2, "Czech koruna", "Kč") },
                                     { "ISO-4217::DJF", new Currency("DJF", "262", 0, "Djiboutian franc", "Fdj") },
                                     { "ISO-4217::DKK", new Currency("DKK", "208", 2, "Danish krone", "kr") },
                                     { "ISO-4217::DOP", new Currency("DOP", "214", 2, "Dominican peso", "RD$") }, // or $
                                     { "ISO-4217::DZD", new Currency("DZD", "012", 2, "Algerian dinar", "DA") }, // (Latin) or د.ج (Arabic)
                                     { "ISO-4217::EGP", new Currency("EGP", "818", 2, "Egyptian pound", "LE") }, // or E£ or ج.م (Arabic)
                                     { "ISO-4217::ERN", new Currency("ERN", "232", 2, "Eritrean nakfa", "ERN") },
                                     { "ISO-4217::ETB", new Currency("ETB", "230", 2, "Ethiopian birr", "Br") }, // (Latin) or ብር (Ethiopic)
                                     { "ISO-4217::EUR", new Currency("EUR", "978", 2, "Euro", "€") },
                                     { "ISO-4217::FJD", new Currency("FJD", "242", 2, "Fiji dollar", "$") }, // or FJ$
                                     { "ISO-4217::FKP", new Currency("FKP", "238", 2, "Falkland Islands pound", "£") },
                                     { "ISO-4217::GBP", new Currency("GBP", "826", 2, "Pound sterling", "£") },
                                     { "ISO-4217::GEL", new Currency("GEL", "981", 2, "Georgian lari", "ლ.") }, // TODO: new symbol since July 18, 2014 => see http://en.wikipedia.org/wiki/Georgian_lari
                                     { "ISO-4217::GHS", new Currency("GHS", "936", 2, "Ghanaian cedi", "GH¢") }, // or GH₵
                                     { "ISO-4217::GIP", new Currency("GIP", "292", 2, "Gibraltar pound", "£") },
                                     { "ISO-4217::GMD", new Currency("GMD", "270", 2, "Gambian dalasi", "D") },
                                     { "ISO-4217::GNF", new Currency("GNF", "324", 0, "Guinean franc", "FG") }, // (possibly also Fr or GFr)
                                     { "ISO-4217::GTQ", new Currency("GTQ", "320", 2, "Guatemalan quetzal", "Q") },
                                     { "ISO-4217::GYD", new Currency("GYD", "328", 2, "Guyanese dollar", "$") }, // or G$
                                     { "ISO-4217::HKD", new Currency("HKD", "344", 2, "Hong Kong dollar", "HK$") }, // or $
                                     { "ISO-4217::HNL", new Currency("HNL", "340", 2, "Honduran lempira", "L") },
                                     { "ISO-4217::HRK", new Currency("HRK", "191", 2, "Croatian kuna", "kn") },
                                     { "ISO-4217::HTG", new Currency("HTG", "332", 2, "Haitian gourde", "G") },
                                     { "ISO-4217::HUF", new Currency("HUF", "348", 2, "Hungarian forint", "Ft") },
                                     { "ISO-4217::IDR", new Currency("IDR", "360", 2, "Indonesian rupiah", "Rp") },
                                     { "ISO-4217::ILS", new Currency("ILS", "376", 2, "Israeli new shekel", "₪") },
                                     { "ISO-4217::INR", new Currency("INR", "356", 2, "Indian rupee", "₹") },
                                     { "ISO-4217::IQD", new Currency("IQD", "368", 3, "Iraqi dinar", "د.ع") },
                                     { "ISO-4217::IRR", new Currency("IRR", "364", 0, "Iranian rial", "ريال") },
                                     { "ISO-4217::ISK", new Currency("ISK", "352", 0, "Icelandic króna", "kr") },
                                     { "ISO-4217::JMD", new Currency("JMD", "388", 2, "Jamaican dollar", "J$") }, // or $
                                     { "ISO-4217::JOD", new Currency("JOD", "400", 3, "Jordanian dinar", "د.ا.‏") },
                                     { "ISO-4217::JPY", new Currency("JPY", "392", 0, "Japanese yen", "¥") },
                                     { "ISO-4217::KES", new Currency("KES", "404", 2, "Kenyan shilling", "KSh") },
                                     { "ISO-4217::KGS", new Currency("KGS", "417", 2, "Kyrgyzstani som", "сом") },
                                     { "ISO-4217::KHR", new Currency("KHR", "116", 2, "Cambodian riel", "៛") },
                                     { "ISO-4217::KMF", new Currency("KMF", "174", 0, "Comoro franc", "CF") },
                                     { "ISO-4217::KPW", new Currency("KPW", "408", 0, "North Korean won", "₩") },
                                     { "ISO-4217::KRW", new Currency("KRW", "410", 0, "South Korean won", "₩") },
                                     { "ISO-4217::KWD", new Currency("KWD", "414", 3, "Kuwaiti dinar", "د.ك") }, // or K.D.
                                     { "ISO-4217::KYD", new Currency("KYD", "136", 2, "Cayman Islands dollar", "$") },
                                     { "ISO-4217::KZT", new Currency("KZT", "398", 2, "Kazakhstani tenge", "₸") },
                                     { "ISO-4217::LAK", new Currency("LAK", "418", 0, "Lao kip", "₭") }, // or ₭N
                                     { "ISO-4217::LBP", new Currency("LBP", "422", 0, "Lebanese pound", "ل.ل") },
                                     { "ISO-4217::LKR", new Currency("LKR", "144", 2, "Sri Lankan rupee", "Rs") }, // or රු
                                     { "ISO-4217::LRD", new Currency("LRD", "430", 2, "Liberian dollar", "$") }, // or L$, LD$
                                     { "ISO-4217::LSL", new Currency("LSL", "426", 2, "Lesotho loti", "L") }, // L or M (pl.)
                                     { "ISO-4217::LYD", new Currency("LYD", "434", 3, "Libyan dinar", "ل.د") }, // or LD
                                     { "ISO-4217::MAD", new Currency("MAD", "504", 2, "Moroccan dirham", "د.م.") },
                                     { "ISO-4217::MDL", new Currency("MDL", "498", 2, "Moldovan leu", "L") },
                                     { "ISO-4217::MGA", new Currency("MGA", "969", Z07, "Malagasy ariary", "Ar") },  // divided into five subunits rather than by a power of ten. 5 is 10 to the power of 0.69897...
                                     { "ISO-4217::MKD", new Currency("MKD", "807", 0, "Macedonian denar", "ден") },
                                     { "ISO-4217::MMK", new Currency("MMK", "104", 0, "Myanma kyat", "K") },
                                     { "ISO-4217::MNT", new Currency("MNT", "496", 2, "Mongolian tugrik", "₮") },
                                     { "ISO-4217::MOP", new Currency("MOP", "446", 2, "Macanese pataca", "MOP$") },
                                     { "ISO-4217::MRO", new Currency("MRO", "478", Z07, "Mauritanian ouguiya", "UM") }, // divided into five subunits rather than by a power of ten. 5 is 10 to the power of 0.69897...
                                     { "ISO-4217::MUR", new Currency("MUR", "480", 2, "Mauritian rupee", "Rs") },
                                     { "ISO-4217::MVR", new Currency("MVR", "462", 2, "Maldivian rufiyaa", "Rf") }, // or , MRf, MVR, .ރ or /-
                                     { "ISO-4217::MWK", new Currency("MWK", "454", 2, "Malawian kwacha", "MK") },
                                     { "ISO-4217::MXN", new Currency("MXN", "484", 2, "Mexican peso", "$") },
                                     { "ISO-4217::MXV", new Currency("MXV", "979", 2, "Mexican Unidad de Inversion (UDI) (funds code)", "¤") },  // <==== not found
                                     { "ISO-4217::MYR", new Currency("MYR", "458", 2, "Malaysian ringgit", "RM") },
                                     { "ISO-4217::MZN", new Currency("MZN", "943", 2, "Mozambican metical", "MTn") }, // or MTN
                                     { "ISO-4217::NAD", new Currency("NAD", "516", 2, "Namibian dollar", "N$") }, // or $
                                     { "ISO-4217::NGN", new Currency("NGN", "566", 2, "Nigerian naira", "₦") },
                                     { "ISO-4217::NIO", new Currency("NIO", "558", 2, "Nicaraguan córdoba", "C$") },
                                     { "ISO-4217::NOK", new Currency("NOK", "578", 2, "Norwegian krone", "kr") },
                                     { "ISO-4217::NPR", new Currency("NPR", "524", 2, "Nepalese rupee", "Rs") }, // or ₨ or रू
                                     { "ISO-4217::NZD", new Currency("NZD", "554", 2, "New Zealand dollar", "$") },
                                     { "ISO-4217::OMR", new Currency("OMR", "512", 3, "Omani rial", "ر.ع.") },
                                     { "ISO-4217::PAB", new Currency("PAB", "590", 2, "Panamanian balboa", "B/.") },
                                     { "ISO-4217::PEN", new Currency("PEN", "604", 2, "Peruvian nuevo sol", "S/.") },
                                     { "ISO-4217::PGK", new Currency("PGK", "598", 2, "Papua New Guinean kina", "K") },
                                     { "ISO-4217::PHP", new Currency("PHP", "608", 2, "Philippine peso", "₱") }, // or P or PHP or PhP
                                     { "ISO-4217::PKR", new Currency("PKR", "586", 2, "Pakistani rupee", "Rs") },
                                     { "ISO-4217::PLN", new Currency("PLN", "985", 2, "Polish złoty", "zł") },
                                     { "ISO-4217::PYG", new Currency("PYG", "600", 0, "Paraguayan guaraní", "₲") },
                                     { "ISO-4217::QAR", new Currency("QAR", "634", 2, "Qatari riyal", "ر.ق") }, // or QR
                                     { "ISO-4217::RON", new Currency("RON", "946", 2, "Romanian new leu", "lei") },
                                     { "ISO-4217::RSD", new Currency("RSD", "941", 2, "Serbian dinar", "РСД") }, // or RSD (or дин. or din. ?)
                                     { "ISO-4217::RUB", new Currency("RUB", "643", 2, "Russian rouble", "₽") }, // or R or руб (both onofficial)
                                     { "ISO-4217::RWF", new Currency("RWF", "646", 0, "Rwandan franc", "RFw") }, // or RF, R₣
                                     { "ISO-4217::SAR", new Currency("SAR", "682", 2, "Saudi riyal", "ر.س") }, // or SR (Latin) or ﷼‎ (Unicode)
                                     { "ISO-4217::SBD", new Currency("SBD", "090", 2, "Solomon Islands dollar", "SI$") },
                                     { "ISO-4217::SCR", new Currency("SCR", "690", 2, "Seychelles rupee", "SR") }, // or SRe
                                     { "ISO-4217::SDG", new Currency("SDG", "938", 2, "Sudanese pound", "ج.س.") },
                                     { "ISO-4217::SEK", new Currency("SEK", "752", 2, "Swedish krona/kronor", "kr") },
                                     { "ISO-4217::SGD", new Currency("SGD", "702", 2, "Singapore dollar", "S$") }, // or $
                                     { "ISO-4217::SHP", new Currency("SHP", "654", 2, "Saint Helena pound", "£") },
                                     { "ISO-4217::SLL", new Currency("SLL", "694", 0, "Sierra Leonean leone", "Le") },
                                     { "ISO-4217::SOS", new Currency("SOS", "706", 2, "Somali shilling", "S") }, // or Sh.So.
                                     { "ISO-4217::SRD", new Currency("SRD", "968", 2, "Surinamese dollar", "$") },
                                     { "ISO-4217::SSP", new Currency("SSP", "728", 2, "South Sudanese pound", "£") }, // not sure about symbol...
                                     { "ISO-4217::STD", new Currency("STD", "678", 0, "São Tomé and Príncipe dobra", "Db") },
                                     { "ISO-4217::SYP", new Currency("SYP", "760", 2, "Syrian pound", "ܠ.ܣ.‏") }, // or LS or £S (or £)
                                     { "ISO-4217::SZL", new Currency("SZL", "748", 2, "Swazi lilangeni", "L") }, // or E (plural)
                                     { "ISO-4217::THB", new Currency("THB", "764", 2, "Thai baht", "฿") },
                                     { "ISO-4217::TJS", new Currency("TJS", "972", 2, "Tajikistani somoni", "смн") },
                                     { "ISO-4217::TMT", new Currency("TMT", "934", 2, "Turkmenistani manat", "m") }, // or T?
                                     { "ISO-4217::TND", new Currency("TND", "788", 3, "Tunisian dinar", "د.ت") }, // or DT (Latin)
                                     { "ISO-4217::TOP", new Currency("TOP", "776", 2, "Tongan paʻanga", "T$") }, // (sometimes PT)
                                     { "ISO-4217::TRY", new Currency("TRY", "949", 2, "Turkish lira", "₺") },
                                     { "ISO-4217::TTD", new Currency("TTD", "780", 2, "Trinidad and Tobago dollar", "$") }, // or TT$
                                     { "ISO-4217::TWD", new Currency("TWD", "901", 2, "New Taiwan dollar", "NT$") }, // or $
                                     { "ISO-4217::TZS", new Currency("TZS", "834", 2, "Tanzanian shilling", "x/y") }, // or TSh
                                     { "ISO-4217::UAH", new Currency("UAH", "980", 2, "Ukrainian hryvnia", "₴") },
                                     { "ISO-4217::UGX", new Currency("UGX", "800", 2, "Ugandan shilling", "USh") },
                                     { "ISO-4217::USD", new Currency("USD", "840", 2, "United States dollar", "$") }, // or US$
                                     { "ISO-4217::USN", new Currency("USN", "997", 2, "United States dollar (next day) (funds code)", "$") },
                                     { "ISO-4217::USS", new Currency("USS", "998", 2, "United States dollar (same day) (funds code)", "$") },
                                     { "ISO-4217::UYI", new Currency("UYI", "940", 0, "Uruguay Peso en Unidades Indexadas (URUIURUI) (funds code)", "¤") },
                                     { "ISO-4217::UYU", new Currency("UYU", "858", 2, "Uruguayan peso", "$") }, // or $U
                                     { "ISO-4217::UZS", new Currency("UZS", "860", 2, "Uzbekistan som", "лв") }, // or сўм ?
                                     { "ISO-4217::VEF", new Currency("VEF", "937", 2, "Venezuelan bolívar fuerte", "Bs.F.") }, // or Bs.
                                     { "ISO-4217::VND", new Currency("VND", "704", 0, "Vietnamese dong", "₫") },
                                     { "ISO-4217::VUV", new Currency("VUV", "548", 0, "Vanuatu vatu", "VT") },
                                     { "ISO-4217::WST", new Currency("WST", "882", 2, "Samoan tala", "WS$") }, // sometimes SAT, ST or T
                                     { "ISO-4217::XAF", new Currency("XAF", "950", 0, "CFA franc BEAC", "FCFA") },
                                     { "ISO-4217::XAG", new Currency("XAG", "961", NotApplicable, "Silver (one troy ounce)", "¤") },
                                     { "ISO-4217::XAU", new Currency("XAU", "959", NotApplicable, "Gold (one troy ounce)", "¤") },
                                     { "ISO-4217::XBA", new Currency("XBA", "955", NotApplicable, "European Composite Unit (EURCO) (bond market unit)", "¤") },
                                     { "ISO-4217::XBB", new Currency("XBB", "956", NotApplicable, "European Monetary Unit (E.M.U.-6) (bond market unit)", "¤") },
                                     { "ISO-4217::XBC", new Currency("XBC", "957", NotApplicable, "European Unit of Account 9 (E.U.A.-9) (bond market unit)", "¤") },
                                     { "ISO-4217::XBD", new Currency("XBD", "958", NotApplicable, "European Unit of Account 17 (E.U.A.-17) (bond market unit)", "¤") },
                                     { "ISO-4217::XCD", new Currency("XCD", "951", 2, "East Caribbean dollar", "$") }, // or EC$
                                     { "ISO-4217::XDR", new Currency("XDR", "960", NotApplicable, "Special drawing rights", "¤") },
                                     { "ISO-4217::XFU", new Currency("XFU", "Nil", NotApplicable, "UIC franc (special settlement currency)", "¤") },
                                     { "ISO-4217::XOF", new Currency("XOF", "952", 0, "CFA franc BCEAO", "CFA") },
                                     { "ISO-4217::XPD", new Currency("XPD", "964", NotApplicable, "Palladium (one troy ounce)", "¤") },
                                     { "ISO-4217::XPF", new Currency("XPF", "953", 0, "CFP franc", "F") },
                                     { "ISO-4217::XPT", new Currency("XPT", "962", NotApplicable, "Platinum (one troy ounce)", "¤") },
                                     { "ISO-4217::XSU", new Currency("XSU", "994", NotApplicable, "SUCRE", "¤") },
                                     { "ISO-4217::XTS", new Currency("XTS", "963", NotApplicable, "Code reserved for testing purposes", "¤") },
                                     { "ISO-4217::XUA", new Currency("XUA", "965", NotApplicable, "ADB Unit of Account", "¤") },
                                     { "ISO-4217::XXX", new Currency("XXX", "999", NotApplicable, "No currency", "¤") },
                                     { "ISO-4217::YER", new Currency("YER", "886", 2, "Yemeni rial", "﷼") }, // or ر.ي.‏‏ ?
                                     { "ISO-4217::ZAR", new Currency("ZAR", "710", 2, "South African rand", "R") },
                                     { "ISO-4217::ZMW", new Currency("ZMW", "967", 2, "Zambian kwacha", "ZK") }, // or ZMW
                                     { "ISO-4217::LTL", new Currency("LTL", "440", 2, "Lithuanian litas", "Lt", isObsolete: true) }, // Until 2014-12-31, replaced by EUR
                                     { "ISO-4217::LVL", new Currency("LVL", "428", 2, "Latvian lats", "Ls", isObsolete: true) }, // Until 2014-01-15, replaced by EUR
                                     { "ISO-4217::ZMK", new Currency("ZMK", "894", 2, "Zambian kwacha", "ZK", isObsolete: true) },  // Until 2013-01-01, replaced by ZWM
                                     { "ISO-4217::ZWL", new Currency("ZWL", "932", 2, "Zimbabwean dollar", "$", isObsolete: true) }, // or Z$ (official currency of Zimbabwe from 1980 to 12 April 2009, not used anymore)
                                     { "ISO-4217::EEK", new Currency("EEK", "233", 2, "Estonian kroon", "kr", isObsolete: true) },  // From 1992 Until 2010-12-31, replaced by EUR
                                     { "ISO-4217::NLG", new Currency("NLG", "528", 2, "Dutch guilder", "ƒ", isObsolete: true)} // From 1810 to 1998-12-31
                                 };

            return currencies;
        }
    }
}
