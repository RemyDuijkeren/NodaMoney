using System;
using System.Collections.Generic;

namespace NodaMoney
{
    /// <summary>Defines a custom currency that is new or based on another currency.</summary>
    public class CurrencyBuilder
    {
        /// <summary>Initializes a new instance of the <see cref="CurrencyBuilder"/> class.</summary>
        /// <param name="currencyCode">The code of the currency, normally the three-character ISO 4217 currency code.</param>
        /// <param name="namespace">The namespace for the currency.</param>
        public CurrencyBuilder(string currencyCode, string @namespace)
        {
            Code = currencyCode;
            Namespace = @namespace;
        }

        /// <summary>Gets or sets the english name of the currency.</summary>
        public string EnglishName { get; set; }

        /// <summary>Gets or sets the currency sign.</summary>
        public string Symbol { get; set; }

        /// <summary>Gets or sets the numeric ISO 4217 currency code.</summary>
        // ReSharper disable once InconsistentNaming
        public string ISONumber { get; set; }

        /// <summary>Gets or sets the number of digits after the decimal separator.</summary>
        public double DecimalDigits { get; set; }

        /// <summary>Gets or sets the namespace of the currency.</summary>
        public string Namespace { get; set; }

        /// <summary>Gets or sets the code of the currency, normally a three-character ISO 4217 currency code.</summary>
        public string Code { get; set; }

        /// <summary>Gets or sets a value indicating whether currency is obsolete.</summary>
        /// <value><c>true</c> if this instance is obsolete; otherwise, <c>false</c>.</value>
        public bool IsObsolete { get; set; }
               
        /// <summary>Reconstitutes a <see cref="CurrencyBuilder"/> object from a specified XML file that contains a
        /// representation of the object.</summary>
        /// <param name="xmlFileName">A file name that contains the XML representation of a <see cref="CurrencyBuilder"/> object.</param>
        /// <returns>A new object that is equivalent to the information stored in the <i>xmlFileName</i> parameter.</returns>
        public static CurrencyBuilder CreateFromLdml(string xmlFileName)
        {
            throw new NotImplementedException();
        }

        /// <summary>Unregisters the specified currency code from the current AppDomain.</summary>
        /// <param name="currencyCode">The name of the currency to unregister.</param>
        /// <param name="namespace">The namespace of the currency to unregister.</param>
        /// <exception cref="ArgumentException">currencyCode specifies a currency that is not found.</exception>
        public static void Unregister(string currencyCode, string @namespace)
        {
            if (!(Currency.Currencies.ContainsKey(@namespace) && Currency.Currencies[@namespace].ContainsKey(currencyCode.ToUpperInvariant())))
                throw new ArgumentException(string.Format("currencyCode {0} specifies a currency that is not found!", currencyCode));

            Currency.Currencies[@namespace].Remove(currencyCode.ToUpperInvariant());
        }

        /// <summary>Registers the current <see cref="CurrencyBuilder"/> object as a custom currency for the current AppDomain.</summary>
        /// <exception cref="InvalidOperationException">
        ///     <para>The custom currency is already registered</para>
        ///     <para>-or-</para>
        ///     <para>The current CurrencyBuilder object has a property that must be set before the currency can be registered.</para>
        /// </exception>
        public void Register()
        {
            if (Currency.Currencies.ContainsKey(Namespace) && Currency.Currencies[Namespace].ContainsKey(Code.ToUpperInvariant()))
                throw new InvalidOperationException("The custom currency is already registered.\n-or-\n"
                                                    + "The current CurrencyBuilder object has a property that must be set before the currency can be registered.");

            var currency = new Currency(Code, ISONumber, DecimalDigits, EnglishName, Symbol);

            if (!Currency.Currencies.ContainsKey(Namespace))
                Currency.Currencies[Namespace] = new Dictionary<string, Currency>();

            Currency.Currencies[Namespace].Add(currency.Code.ToUpperInvariant(), currency);
        }

        /// <summary>Writes an XML representation of the current <see cref="CurrencyBuilder"/> object to the specified file.</summary>
        /// <param name="fileName">The name of a file to contain the XML representation of this custom currency.</param>
        /// <remarks>
        ///     <para>
        ///     The Save method writes the current <see cref="CurrencyBuilder"/> object to the file specified by the
        ///     filename parameter in an XML format called Locale Data Markup Language (LDML) version 1.1.
        ///     The <see cref="CreateFromLdml"/> method performs the reverse operation of the Save method.
        ///     </para>
        ///     <para>
        ///     For information about the format of an LDML file, see the CreateFromLdml method. For information about the LDML
        ///     standard, see <see href='http://go.microsoft.com/fwlink/p/?LinkId=252840'>Unicode Technical Standard #35, "Locale
        ///     Data Markup Language (LDML)"</see> on the Unicode Consortium website.
        ///     </para>
        /// </remarks>
        public void Save(string fileName)
        {
            throw new NotImplementedException();
        }

        /// <summary>Sets the properties of the current <see cref="CurrencyBuilder"/> object with the corresponding properties of
        /// the specified <see cref="Currency"/> object, except for the code and namespace.</summary>
        /// <param name="currency">The object whose properties will be used.</param>
        public void LoadDataFromCurrency(Currency currency)
        {
            EnglishName = currency.EnglishName;
            Symbol = currency.Symbol;
            ISONumber = currency.Number;
            DecimalDigits = currency.DecimalDigits;
            IsObsolete = currency.IsObsolete;
        }
    }
}