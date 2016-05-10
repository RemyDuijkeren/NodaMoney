using System;
using System.Collections.Generic;
using System.Globalization;

namespace NodaMoney
{
    /// <summary>Defines a custom currency that is new or based on another currency.</summary>
    public class CurrencyBuilder
    {
        /// <summary>Initializes a new instance of the <see cref="CurrencyBuilder"/> class.</summary>
        /// <param name="code">The code of the currency, normally the three-character ISO 4217 currency code.</param>
        /// <param name="namespace">The namespace for the currency.</param>
        /// <exception cref="ArgumentNullException"><paramref name="code"/> or <paramref name="namespace"/> is <see langword="null" /> or empty.</exception>
        public CurrencyBuilder(string code, string @namespace)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentNullException(nameof(code));
            if (string.IsNullOrWhiteSpace(@namespace))
                throw new ArgumentNullException(nameof(@namespace));

            Code = code;
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

        /// <summary>Gets the namespace of the currency.</summary>
        public string Namespace { get; }

        /// <summary>Gets the code of the currency, normally a three-character ISO 4217 currency code.</summary>
        public string Code { get; }

        /// <summary>Gets or sets a value indicating whether currency is obsolete.</summary>
        /// <value><c>true</c> if this instance is obsolete; otherwise, <c>false</c>.</value>
        public bool IsObsolete { get; set; }

        /// <summary>Gets or sets the date when the currency is valid from.</summary>
        /// <value>The from date when the currency is valid.</value>
        public DateTime? ValidFrom { get; set; }

        /// <summary>Gets or sets the date when the currency is valid to.</summary>
        /// <value>The to date when the currency is valid.</value>
        public DateTime? ValidTo { get; set; }

        /// <summary>Reconstitutes a <see cref="CurrencyBuilder"/> object from a specified XML file that contains a
        /// representation of the object.</summary>
        /// <param name="xmlFileName">A file name that contains the XML representation of a <see cref="CurrencyBuilder"/> object.</param>
        /// <returns>A new object that is equivalent to the information stored in the <i>xmlFileName</i> parameter.</returns>
        public static CurrencyBuilder CreateFromLdml(string xmlFileName)
        {
            throw new NotImplementedException();
        }

        /// <summary>Unregisters the specified currency code from the current AppDomain and returns it.</summary>
        /// <param name="code">The name of the currency to unregister.</param>
        /// <param name="namespace">The namespace of the currency to unregister from.</param>
        /// <returns>An instance of the type <see cref="Currency"/>.</returns>
        /// <exception cref="ArgumentException">code specifies a currency that is not found in the given namespace.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="code" /> or <paramref name="namespace" /> is <see langword="null" /> or empty.</exception>
        public static Currency Unregister(string code, string @namespace)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentNullException(nameof(code));
            if (string.IsNullOrWhiteSpace(@namespace))
                throw new ArgumentNullException(nameof(@namespace));

            Currency currency;
            if (!Currency.Registry.TryRemove(code, @namespace, out currency))
                throw new ArgumentException($"code {code} specifies a currency that is not found in the namespace {@namespace}!");

            return currency;
        }

        /// <summary>Builds the current <see cref="CurrencyBuilder"/> object as a custom currency.</summary>
        /// <returns>A <see cref="Currency"/> instance that is build.</returns>
        //// <exception cref="InvalidOperationException">The current CurrencyBuilder object has a property that must be set before the currency can be registered.</exception>
        public Currency Build()
        {
            // TODO: Add validation?
            // throw new InvalidOperationException("The current CurrencyBuilder object has a property that must be set before the currency can be registered.");
            var currency = new Currency(Code, ISONumber, DecimalDigits, EnglishName, Symbol, Namespace, ValidTo, ValidFrom);

            return currency;
        }

        /// <summary>Registers the current <see cref="CurrencyBuilder"/> object as a custom currency for the current AppDomain.</summary>
        /// <returns>A <see cref="Currency"/> instance that is build and registered.</returns>
        /// <exception cref="InvalidOperationException">
        ///     <para>The custom currency is already registered</para>
        ///     <para>-or-</para>
        ///     <para>The current CurrencyBuilder object has a property that must be set before the currency can be registered.</para>
        /// </exception>
        public Currency Register()
        {
            Currency currency = Build();
            if (!Currency.Registry.TryAdd(Code, Namespace, currency))
                throw new InvalidOperationException("The custom currency is already registered.");

            return currency;
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
            ValidFrom = currency.ValidFrom;
            ValidTo = currency.ValidTo;
        }
    }
}