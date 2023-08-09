using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

[assembly: InternalsVisibleTo("NodaMoney.Tests")]
[assembly: CLSCompliant(true)]

namespace NodaMoney
{
    /// <summary>A unit of exchange of value, a currency of <see cref="Money" />.</summary>
    /// <remarks>See http://en.wikipedia.org/wiki/Currency .</remarks>
    [Serializable]
    [DebuggerDisplay("{Code}")]
    [TypeConverter(typeof(CurrencyTypeConverter))]
    public readonly struct Currency : IEquatable<Currency>, IXmlSerializable, ISerializable
    {
        /// <summary>Gets the currency sign (¤), a character used to denote the generic currency sign, when no currency sign is available.</summary>
        /// <remarks>See https://en.wikipedia.org/wiki/Currency_sign_(typography). </remarks>
        public const string GenericCurrencySign = "¤";

        private readonly byte _minorUnit;
        private readonly byte _namespace;
        private readonly short _number;
        private readonly string _code;
        private readonly string _symbol;
        private readonly string _englishName;
        private readonly DateTime? _validFrom;
        private readonly DateTime? _validTo;

        /// <summary>Initializes a new instance of the <see cref="Currency" /> struct.</summary>
        /// <param name="code">The code.</param>
        /// <param name="namespace">The namespace.</param>
        /// <remarks>Used in serialization.</remarks>
        internal Currency(string code, string @namespace = "ISO-4217")
        {
            ref Currency c = ref FromCode(code, @namespace);

            _code = c.Code;
            _number = c.Number;
            _minorUnit = c._minorUnit;
            _englishName = c.EnglishName;
            _symbol = c.Symbol;
            _namespace = c._namespace;
            _validTo = c.ValidTo;
            _validFrom = c.ValidFrom;
        }

        /// <summary>Initializes a new instance of the <see cref="Currency" /> struct.</summary>
        /// <param name="code">The code.</param>
        /// <param name="number">The number.</param>
        /// <param name="minorUnitAsPowerOfTen">The decimal digits.</param>
        /// <param name="englishName">Name of the english.</param>
        /// <param name="symbol">The currency symbol.</param>
        /// <param name="namespace">The namespace of the currency.</param>
        /// <param name="validTo">The valid until the specified date.</param>
        /// <param name="validFrom">The valid from the specified date.</param>
        /// <exception cref="System.ArgumentNullException">code or number or englishName or symbol is null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">DecimalDigits must greater or equal to zero and smaller or equal to 28, or -1 if not applicable.</exception>
        internal Currency(string code, short number, byte minorUnitAsPowerOfTen, string englishName, string symbol, byte @namespace = 0, DateTime? validTo = null, DateTime? validFrom = null)
            : this()
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(code));
            if (minorUnitAsPowerOfTen != CurrencyRegistry.Z07Byte && minorUnitAsPowerOfTen != CurrencyRegistry.NotApplicableByte && (minorUnitAsPowerOfTen < 0 || minorUnitAsPowerOfTen > 28))
                throw new ArgumentOutOfRangeException(nameof(minorUnitAsPowerOfTen), $"For code {code} DecimalDigits must greater or equal to zero and smaller or equal to 28, or 255 if not applicable!");

            _code = code;
            _number = number;
            _minorUnit = minorUnitAsPowerOfTen;
            _englishName = englishName ?? string.Empty;
            _symbol = symbol ?? GenericCurrencySign;
            _namespace = @namespace;
            _validTo = validTo;
            _validFrom = validFrom;
        }

#pragma warning disable CA1801 // Parameter context of method.ctor is never used.
        private Currency(SerializationInfo info, StreamingContext context)
        : this(info.GetString("code"), info.GetString("namespace"))
        {
        }
#pragma warning restore CA1801 // Parameter context of method.ctor is never used.

        /// <summary>Gets the Currency that represents the country/region used by the current thread.</summary>
        /// <value>The Currency that represents the country/region used by the current thread.</value>
        public static ref Currency CurrentCurrency
        {
            get
            {
                var currentRegion = RegionInfo.CurrentRegion;
                return ref currentRegion.Name == "IV" ? ref FromCode("XXX") : ref FromRegion(currentRegion);
            }
        }

        /// <summary>Gets the currency symbol.</summary>
        public string Symbol
        {
            get
            {
                IfDefaultThenInitializeToNoCurrency();
                return _symbol;
            }
        }

        /// <summary>Gets the english name of the currency.</summary>
        public string EnglishName
        {
            get
            {
                IfDefaultThenInitializeToNoCurrency();
                return _englishName;
            }
        }

        /// <summary>Gets the (ISO-4217) three-character code of the currency.</summary>
        public string Code
        {
            get
            {
                IfDefaultThenInitializeToNoCurrency();
                return _code;
            }
        }

        /// <summary>Gets the (ISO-4217) number of the currency.</summary>
        public short Number
        {
            get
            {
                IfDefaultThenInitializeToNoCurrency();
                return _number;
            }
        }

        /// <summary>Gets the (ISO-4217) three-digit code number of the currency.</summary>
        public string NumericCode => Number.ToString("D3", CultureInfo.InvariantCulture);

        /// <summary>Gets the namespace of the currency, like ISO-4217.</summary>
        public string Namespace
        {
            get
            {
                return CurrencyRegistry.GetNamespace(_namespace);
            }
        }

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
        public int DecimalDigits
        {
            get
            {
                IfDefaultThenInitializeToNoCurrency();
                return _minorUnit switch
                {
                    CurrencyRegistry.NotApplicableByte => 0,
                    CurrencyRegistry.Z07Byte => 1,
                    _ => _minorUnit,
                };
            }
        }

        /// <summary>Gets the smallest amount of the currency unit.</summary>
        public decimal MinimalAmount
        {
            get
            {
                IfDefaultThenInitializeToNoCurrency();
                return MinorUnit == 0 ? 1m : (decimal)(1.0 / Math.Pow(10, MinorUnit));
            }
        }

        /// <summary>Gets the minor unit, as an exponent of base 10, by which the currency unit can be divided in.</summary>
        /// <para>
        /// The US dollar can be divided into 100 cents (1/100), which is 10^2, so the exponent 2 will be returned.
        /// </para>
        /// <para>
        /// Mauritania does not use a decimal division of units, but has 1 ouguiya (UM) which can be divided int 5 khoums (1/5), which is
        /// 10^log10(5) = 10^0.698970004, so the exponent 0.698970004 will be returned.
        /// </para>
        public double MinorUnit
        {
            get
            {
                // https://www.iso.org/obp/ui/#iso:std:iso:4217:ed-8:v1:en
                // unit of recorded value (i.e. as recorded by banks) which is a division of the respective unit of currency or fund
                IfDefaultThenInitializeToNoCurrency();
                return _minorUnit switch
                {
                    CurrencyRegistry.NotApplicableByte => 0,
                    CurrencyRegistry.Z07Byte => Math.Log10(5),
                    _ => _minorUnit,
                };
            }
        }

        /// <summary>Gets the number of minor units by which the currency unit can be divided in.</summary>
        /// <para>
        /// The US dollar can be divided into 100 cents (1/100) so the 100 will be returned.
        /// </para>
        /// <para>
        /// Mauritania does not use a decimal division of units, but has 1 ouguiya (UM) which can be divided int 5 khoums (1/5),
        /// so 5 will be returned.
        /// </para>
        public double MinorUnits
        {
            get
            {
                IfDefaultThenInitializeToNoCurrency();
                return Math.Pow(10, MinorUnit);
            }
        }

        /// <summary>Gets the date when the currency is valid from.</summary>
        /// <value>The from date when the currency is valid.</value>
        public DateTime? ValidFrom
        {
            get
            {
                IfDefaultThenInitializeToNoCurrency();
                return _validFrom;
            }
        }

        /// <summary>Gets the date when the currency is valid to.</summary>
        /// <value>The to date when the currency is valid.</value>
        public DateTime? ValidTo
        {
            get
            {
                IfDefaultThenInitializeToNoCurrency();
                return _validTo;
            }
        }

        /// <summary>Gets a value indicating whether currency is valid.</summary>
        /// <value><c>true</c> if this instance is valid; otherwise, <c>false</c>.</value>
        public bool IsValid => IsValidOn(DateTime.Today);

        /// <summary>Implements the operator ==.</summary>
        /// <param name="left">The left Currency.</param>
        /// <param name="right">The right Currency.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(Currency left, Currency right) => left.Equals(right);

        /// <summary>Implements the operator ==.</summary>
        /// <param name="left">The left Currency.</param>
        /// <param name="right">The right Currency.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(Currency left, Currency right) => !(left == right);

        /// <summary>Create an instance of the <see cref="Currency"/>, based on a ISO 4217 currency code.</summary>
        /// <param name="code">A ISO 4217 currency code, like EUR or USD.</param>
        /// <returns>An instance of the type <see cref="Currency"/>.</returns>
        /// <exception cref="ArgumentNullException">The value of 'code' cannot be null.</exception>
        /// <exception cref="ArgumentException">The 'code' is an unknown ISO 4217 currency code.</exception>
        public static ref Currency FromCode(string code)
        {
            return ref CurrencyRegistry.Get(code);
        }

        /// <summary>Create an instance of the <see cref="Currency"/> of the given code and namespace.</summary>
        /// <param name="code">A currency code, like EUR or USD.</param>
        /// <param name="namespace">A namespace, like ISO-4217.</param>
        /// <returns>An instance of the type <see cref="Currency"/>.</returns>
        /// <exception cref="ArgumentNullException">The value of 'code' or 'namespace' cannot be null or empty.</exception>
        /// <exception cref="ArgumentException">The 'code' in the given namespace is an unknown.</exception>
        public static ref Currency FromCode(string code, string @namespace)
        {
            return ref CurrencyRegistry.Get(code, @namespace);
        }

        /// <summary>Creates an instance of the <see cref="Currency"/> used within the specified <see cref="RegionInfo"/>.</summary>
        /// <param name="region"><see cref="RegionInfo"/> to get a <see cref="Currency"/> for.</param>
        /// <returns>The <see cref="Currency"/> instance used within the specified <see cref="RegionInfo"/>.</returns>
        /// <exception cref="ArgumentNullException">The value of 'region' cannot be null.</exception>
        /// <exception cref="ArgumentException">The 'code' is an unknown ISO 4217 currency code.</exception>
        public static ref Currency FromRegion(RegionInfo region)
        {
            if (region == null)
                throw new ArgumentNullException(nameof(region));

            return ref FromCode(region.ISOCurrencySymbol, "ISO-4217");
        }

        /// <summary>Creates an instance of the <see cref="Currency"/> used within the specified <see cref="CultureInfo"/>.</summary>
        /// <param name="culture"><see cref="CultureInfo"/> to get a <see cref="Currency"/> for.</param>
        /// <returns>The <see cref="Currency"/> instance used within the specified <see cref="CultureInfo"/>.</returns>
        /// <exception cref="ArgumentNullException">The value of 'culture' cannot be null.</exception>
        /// <exception cref="ArgumentException">
        /// Culture is a neutral culture, from which no region information can be extracted -or-
        /// The 'code' is an unknown ISO 4217 currency code.
        /// </exception>
        public static ref Currency FromCulture(CultureInfo culture)
        {
            if (culture == null)
                throw new ArgumentNullException(nameof(culture));
            if (culture.IsNeutralCulture)
                throw new ArgumentException("Culture {0} is a neutral culture, from which no region information can be extracted!", culture.Name);

            return ref FromRegion(culture.Name);
        }

        /// <summary>Creates an instance of the <see cref="Currency"/> used within the specified name of the region or culture.</summary>
        /// <param name="name">
        /// <para>A string that contains a two-letter code defined in ISO 3166 for country/region.</para>
        /// <para>-or-</para>
        /// <para>A string that contains the culture name for a specific culture, custom culture, or Windows-only culture. If the
        /// culture name is not in RFC 4646 format, your application should specify the entire culture name instead of just the
        /// country/region. See also <seealso cref="System.Globalization.RegionInfo(string)"/>.</para>
        /// </param>
        /// <returns>The <see cref="Currency"/> instance used within the specified region.</returns>
        /// <exception cref="ArgumentNullException">The value of 'name' cannot be null.</exception>
        public static ref Currency FromRegion(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));

            return ref FromRegion(new RegionInfo(name));
        }

        /// <summary>Get all currencies.</summary>
        /// <returns>An <see cref="IEnumerable{Currency}"/> of all registered currencies.</returns>
        public static IEnumerable<Currency> GetAllCurrencies() => CurrencyRegistry.GetAllCurrencies();

        /// <summary>Returns a value indicating whether two specified instances of <see cref="Currency"/> represent the same value.</summary>
        /// <param name="left">The first <see cref="Currency"/> object.</param>
        /// <param name="right">The second <see cref="Currency"/> object.</param>
        /// <returns>true if left and right are equal to this instance; otherwise, false.</returns>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Calling override method")]
        public static bool Equals(Currency left, Currency right) => left.Equals(right);

        /// <summary>Returns a value indicating whether this instance and a specified <see cref="object"/> represent the same type and value.</summary>
        /// <param name="obj">An <see cref="object"/>.</param>
        /// <returns>true if value is equal to this instance; otherwise, false.</returns>
        public override bool Equals(object obj) => obj is Currency currency && Equals(currency);

        /// <summary>Returns a value indicating whether this instance and a specified <see cref="Currency"/> object represent the same value.</summary>
        /// <param name="other">A <see cref="Currency"/> object.</param>
        /// <returns>true if value is equal to this instance; otherwise, false.</returns>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Calling override method")]
        public bool Equals(Currency other)
        {
            return Code == other.Code && Namespace == other.Namespace;
        }

        /// <summary>Returns the hash code for this instance.</summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
#pragma warning disable CA1307 // Specify StringComparison
                hash = (hash * 23) + (Code?.GetHashCode() ?? 0);
#pragma warning restore CA1307 // Specify StringComparison
                return (hash * 23) + _namespace.GetHashCode();
            }
        }

        /// <summary>Deconstructs the current instance into its components.</summary>
        /// <param name="code">The code.</param>
        /// <param name="isoNumber">The ISO4127 three-digit code number.</param>
        /// <param name="symbol">The currency symbol.</param>
        public void Deconstruct(out string code, out string isoNumber, out string symbol)
        {
            code = Code;
            isoNumber = NumericCode;
            symbol = Symbol;
        }

        /// <summary>Check a value indication whether currency is valid on a given date.</summary>
        /// <param name="date">The date on which the Currency should be valid.</param>
        /// <returns><c>true</c> when the date is within the valid range of this currency; otherwise <c>false</c>.</returns>
        public bool IsValidOn(DateTime date)
        {
            return
                (!ValidFrom.HasValue || ValidFrom <= date) &&
                (!ValidTo.HasValue || ValidTo >= date);
        }

        /// <summary>This method is reserved and should not be used. When implementing the IXmlSerializable interface, you should
        /// return null (Nothing in Visual Basic) from this method, and instead, if specifying a custom schema is required, apply
        /// the <see cref="XmlSchemaProviderAttribute" /> to the class.</summary>
        /// <returns>An <see cref="XmlSchema" /> that describes the XML representation of the object that is
        /// produced by the <see cref="IXmlSerializable.WriteXml(XmlWriter)" /> method and
        /// consumed by the <see cref="IXmlSerializable.ReadXml(XmlReader)" /> method.
        /// </returns>
        public XmlSchema GetSchema() => null;

        /// <summary>Generates an object from its XML representation.</summary>
        /// <param name="reader">The <see cref="XmlReader" /> stream from which the object is deserialized.</param>
        /// <exception cref="ArgumentNullException">The value of 'reader' cannot be null. </exception>
        public void ReadXml(XmlReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            Unsafe.AsRef(this) = FromCode(reader["Currency"]);
        }

        /// <summary>
        /// Converts an object into its XML representation.
        /// </summary>
        /// <param name="writer">The <see cref="XmlWriter" /> stream to which the object is serialized.</param>
        /// <exception cref="ArgumentNullException">The value of 'writer' cannot be null.</exception>
        public void WriteXml(XmlWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            writer.WriteAttributeString("Currency", Code);
        }

        /// <summary>Populates a <see cref="SerializationInfo" /> with the data needed to serialize the target object.</summary>
        /// <param name="info">The <see cref="SerializationInfo" /> to populate with data. </param>
        /// <param name="context">The destination (see <see cref="StreamingContext" />) for this serialization. </param>
        /// <exception cref="System.Security.SecurityException">The caller does not have the required permission. </exception>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));

            info.AddValue("code", Code);
            info.AddValue("namespace", Namespace);
        }

        /// <summary>Returns a hash code for this instance.</summary>
        /// <param name="code">The code.</param>
        /// <param name="namespace">The @namespace.</param>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        internal static int GetHashCode(in string code, in int @namespace)
        {
            unchecked
            {
                int hash = 17;
#pragma warning disable CA1307 // Specify StringComparison
                hash = (hash * 23) + (code?.GetHashCode() ?? 0);
#pragma warning restore CA1307 // Specify StringComparison
                return (hash * 23) + ((byte)@namespace).GetHashCode();
            }
        }

        /// <summary>Check if default(currency) is used. I so, then initialize it to {XXX, 999, No currency}.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void IfDefaultThenInitializeToNoCurrency()
        {
            if (_code == null)
                Unsafe.AsRef(this) = FromCode("XXX");
        }
    }
}
