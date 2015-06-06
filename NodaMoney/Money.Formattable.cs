using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace NodaMoney
{
    /// <summary>Represents Money, an amount defined in a specific Currency.</summary>
    public partial struct Money : IFormattable
    {
        /// <summary>Converts this <see cref="Money"/> instance to its equivalent <see cref="String"/> representation.</summary>
        /// <returns>A string that represents this <see cref="Money"/> instance.</returns>
        /// <remarks>
        /// Converting will use the <see cref="NumberFormatInfo"/> object for the current culture if this has the same
        /// ISOCurrencySymbol, otherwise the <see cref="NumberFormatInfo"/> from the <see cref="Currency"/> will be used.
        /// </remarks>
        public override string ToString()
        {
            return ConvertToString(null, null);
        }

        /// <summary>Converts the <see cref="Money"/> value of this instance to its equivalent <see cref="String"/> representation
        /// using the specified format.</summary>
        /// <param name="format">A numeric format string.</param>
        /// <returns>The string representation of this <see cref="Money"/> instance as specified by the format.</returns>
        /// <exception cref="ArgumentNullException">The value of 'format' cannot be null.</exception>
        public string ToString(string format)
        {
            // http://msdn.microsoft.com/en-us/library/syy068tk.aspx
            if (format == null)
                throw new ArgumentNullException("format");

            return ConvertToString(format, null);
        }

        /// <summary>Converts this <see cref="Money"/> instance to its equivalent <see cref="String"/> representation using the
        /// specified culture-specific format information.</summary>
        /// <param name="formatProvider">An <see cref="IFormatProvider"/> that supplies culture-specific formatting information.</param>
        /// <returns>The string representation of this <see cref="Money"/> instance as specified by formatProvider.</returns>
        /// <exception cref="ArgumentNullException">The value of 'formatProvider' cannot be null.</exception>
        public string ToString(IFormatProvider formatProvider)
        {
            if (formatProvider == null)
                throw new ArgumentNullException("formatProvider");

            return ConvertToString(null, formatProvider);
        }

        /// <summary>Converts the <see cref="Money"/> value of this instance to its equivalent <see cref="String"/> representation
        /// using the specified format and culture-specific format information.</summary>
        /// <param name="format">A numeric format string.</param>
        /// <param name="formatProvider">An <see cref="IFormatProvider"/> that supplies culture-specific formatting information.</param>
        /// <returns>The string representation of this <see cref="Money"/> instance as specified by the format and formatProvider.</returns>
        public string ToString(string format, IFormatProvider formatProvider)
        {
            return ConvertToString(format, formatProvider);
        }

        private static NumberFormatInfo GetNumberFormatInfo(Currency currency, IFormatProvider formatProvider)
        {
            var numberFormatInfo = (NumberFormatInfo)NumberFormatInfo.CurrentInfo.Clone();

            if (formatProvider != null)
            {
                var ci = formatProvider as CultureInfo;
                if (ci != null)
                    numberFormatInfo = (NumberFormatInfo)ci.NumberFormat.Clone();

                var nfi = formatProvider as NumberFormatInfo;
                if (nfi != null)
                    numberFormatInfo = (NumberFormatInfo)nfi.Clone();
            }

            numberFormatInfo.CurrencySymbol = currency.Symbol;
            numberFormatInfo.CurrencyDecimalDigits = (int)currency.DecimalDigits;
            return numberFormatInfo;
        }

        private string ConvertToString(string format, IFormatProvider formatProvider)
        {
            // TODO: ICustomFormat : http://msdn.microsoft.com/query/dev12.query?appId=Dev12IDEF1&l=EN-US&k=k(System.IFormatProvider);k(TargetFrameworkMoniker-.NETPortable,Version%3Dv4.6);k(DevLang-csharp)&rd=true
            // TODO: Move to Currency? Currency.GetNumberFormatInfo()
            // TODO: Add custom format to represent USD 12.34, EUR 12.35, etc.
            // The formatting of Money should respect the NumberFormat of the current Culture, except for the CurrencySymbol and CurrencyDecimalDigits.
            // http://en.wikipedia.org/wiki/Linguistic_issues_concerning_the_euro
            NumberFormatInfo numberFormatInfo = GetNumberFormatInfo(Currency, formatProvider);

            return Amount.ToString(format ?? "C", numberFormatInfo);
        }
    }
}