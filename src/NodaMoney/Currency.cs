using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;
#if !PORTABLE
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;
#endif

namespace NodaMoney
{
    /// <summary>A unit of exchange, a currency of <see cref="Money" />.</summary>
    /// <remarks>See http://en.wikipedia.org/wiki/Currency .</remarks>
    [DataContract]
    [DebuggerDisplay("{Code}")]
    public struct Currency : IEquatable<Currency>
#if !PORTABLE
        , IXmlSerializable
#endif
    {
        internal static readonly CurrencyRegistry Registry = new CurrencyRegistry();

        /// <summary>Initializes a new instance of the <see cref="Currency" /> struct.</summary>
        /// <param name="code">The code.</param>
        /// <param name="number">The number.</param>
        /// <param name="decimalDigits">The decimal digits.</param>
        /// <param name="englishName">Name of the english.</param>
        /// <param name="symbol">The currency symbol.</param>
        /// <param name="namespace">The namespace of the currency.</param>
        /// <param name="validTo">The valid until the specified date.</param>
        /// <param name="validFrom">The valid from the specified date.</param>
        /// <exception cref="System.ArgumentNullException">code or number or englishName or symbol is null</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">DecimalDigits of code must be greater or equal to zero!</exception>
        internal Currency(string code, string number, double decimalDigits, string englishName, string symbol, string @namespace = "ISO-4217", DateTime? validTo = null, DateTime? validFrom = null)
            : this()
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(code));
            if (number == null)
                throw new ArgumentNullException(nameof(number));
            if (string.IsNullOrWhiteSpace(englishName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(englishName));
            if (string.IsNullOrWhiteSpace(symbol))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(symbol));
            if (string.IsNullOrWhiteSpace(@namespace))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(@namespace));
            if (decimalDigits < 0 && decimalDigits != CurrencyRegistry.NotApplicable)
                throw new ArgumentOutOfRangeException(nameof(code), "DecimalDigits must greater or equal to zero!");

            Code = code;
            Number = number;
            DecimalDigits = decimalDigits;
            EnglishName = englishName;
            Symbol = symbol;
            Namespace = @namespace;
            ValidTo = validTo;
            ValidFrom = validFrom;
        }

        /// <summary>Gets the Currency that represents the country/region used by the current thread.</summary>
        /// <value>The Currency that represents the country/region used by the current thread.</value>
        public static Currency CurrentCurrency => FromRegion(RegionInfo.CurrentRegion);

        /// <summary>Gets the currency sign (¤), a character used to denote an unspecified currency.</summary>
        /// <remarks><seealso cref="https://en.wikipedia.org/wiki/Currency_sign_(typography)"/></remarks>
        public static string CurrencySign => CultureInfo.InvariantCulture.NumberFormat.CurrencySymbol;

        /// <summary>Gets the currency symbol.</summary>
        public string Symbol { get; private set; }

        /// <summary>Gets the english name of the currency.</summary>
        public string EnglishName { get; private set; }

        /// <summary>Gets the three-character ISO-4217 currency code.</summary>
        [DataMember]
        public string Code { get; private set; }

        /// <summary>Gets the numeric ISO-4217 currency code.</summary>
        public string Number { get; private set; }

        /// <summary>Gets the namespace of the currency.</summary>
        public string Namespace { get; private set; }

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

        /// <summary>Gets the date when the currency is valid from.</summary>
        /// <value>The from date when the currency is valid.</value>
        public DateTime? ValidFrom { get; internal set; }

        /// <summary>Gets the date when the currency is valid to.</summary>
        /// <value>The to date when the currency is valid.</value>
        public DateTime? ValidTo { get; internal set; }

        /// <summary>Gets the major currency unit.</summary>
#if !PORTABLE
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Member of Currency type! Implementation can change in the future.")]
#endif
        public decimal MajorUnit => 1;

        /// <summary>Gets the minor currency unit.</summary>
        public decimal MinorUnit
        {
            get
            {
                if (DecimalDigits == CurrencyRegistry.NotApplicable)
                    return MajorUnit;

                return new decimal(1 / Math.Pow(10, DecimalDigits));
            }
        }

        /// <summary>Gets a value indicating whether currency is obsolete.</summary>
        /// <value><c>true</c> if this instance is obsolete; otherwise, <c>false</c>.</value>
        public bool IsObsolete => ValidTo.HasValue && ValidTo.Value < DateTime.Today;

        /// <summary>Create an instance of the <see cref="Currency"/>, based on a ISO 4217 currency code.</summary>
        /// <param name="code">A ISO 4217 currency code, like EUR or USD.</param>
        /// <returns>An instance of the type <see cref="Currency"/>.</returns>
        /// <exception cref="ArgumentNullException">The value of 'code' cannot be null.</exception>
        /// <exception cref="ArgumentException">The 'code' is an unknown ISO 4217 currency code!</exception>
        public static Currency FromCode(string code)
        {
            Currency currency;
            if (!Registry.TryGet(code, out currency))
                throw new ArgumentException($"{code} is an unknown currency code!");

            return currency;
        }

        /// <summary>Create an instance of the <see cref="Currency"/> of the given code and namespace.</summary>
        /// <param name="code">A currency code, like EUR or USD.</param>
        /// <param name="namespace">A namespace, like ISO-4217.</param>
        /// <returns>An instance of the type <see cref="Currency"/>.</returns>
        /// <exception cref="ArgumentNullException">The value of 'code' or 'namespace' cannot be null or empty.</exception>
        /// <exception cref="ArgumentException">The 'code' in the given namespace is an unknown!</exception>
        public static Currency FromCode(string code, string @namespace)
        {
            Currency currency;
            if (!Registry.TryGet(code, @namespace, out currency))
                throw new ArgumentException($"{code} is an unknown {@namespace} currency code!");

            return currency;
        }

        /// <summary>Creates an instance of the <see cref="Currency"/> used within the specified <see cref="RegionInfo"/>.</summary>
        /// <param name="region"><see cref="RegionInfo"/> to get a <see cref="Currency"/> for.</param>
        /// <returns>The <see cref="Currency"/> instance used within the specified <see cref="RegionInfo"/>.</returns>
        /// <exception cref="ArgumentNullException">The value of 'region' cannot be null.</exception>
        /// <exception cref="ArgumentException">The 'code' is an unknown ISO 4217 currency code!</exception>
        public static Currency FromRegion(RegionInfo region)
        {
            if (region == null)
                throw new ArgumentNullException(nameof(region));

            return FromCode(region.ISOCurrencySymbol, "ISO-4217");
        }

        /// <summary>Creates an instance of the <see cref="Currency"/> used within the specified <see cref="CultureInfo"/>.</summary>
        /// <param name="culture"><see cref="CultureInfo"/> to get a <see cref="Currency"/> for.</param>
        /// <returns>The <see cref="Currency"/> instance used within the specified <see cref="CultureInfo"/>.</returns>
        /// <exception cref="ArgumentNullException">The value of 'culture' cannot be null.</exception>
        /// <exception cref="ArgumentException">
        /// Culture is a neutral culture, from which no region information can be extracted -or-
        /// The 'code' is an unknown ISO 4217 currency code!
        /// </exception>
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
        /// country/region. See also <seealso cref="System.Globalization.RegionInfo(string)"/>.</para>
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
        /// <returns>An <see cref="IEnumerable{Currency}"/> of all registered currencies.</returns>
        public static IEnumerable<Currency> GetAllCurrencies()
        {
            return Registry.GetAllCurrencies();
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
#if !PORTABLE
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Calling override method")]
#endif
        public static bool Equals(Currency left, Currency right)
        {
            return left.Equals(right);
        }

        /// <summary>Returns a value indicating whether this instance and a specified <see cref="object"/> represent the same type and value.</summary>
        /// <param name="obj">An <see cref="object"/>.</param>
        /// <returns>true if value is equal to this instance; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            return (obj is Currency) && Equals((Currency)obj);
        }

        /// <summary>Returns a value indicating whether this instance and a specified <see cref="Currency"/> object represent the same value.</summary>
        /// <param name="other">A <see cref="Currency"/> object.</param>
        /// <returns>true if value is equal to this instance; otherwise, false.</returns>
#if !PORTABLE
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Calling override method")]
#endif
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
                throw new ArgumentNullException(nameof(reader));

            this = FromCode(reader["Currency"]);
        }

        /// <summary>
        /// Converts an object into its XML representation.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Xml.XmlWriter" /> stream to which the object is serialized.</param>
        /// <exception cref="ArgumentNullException">The value of 'writer' cannot be null. </exception>
        public void WriteXml(XmlWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            writer.WriteAttributeString("Currency", Code);
        }
    }
}