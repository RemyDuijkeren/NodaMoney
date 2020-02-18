using System;
using System.Globalization;

namespace NodaMoney
{
    /// <summary>Represents Money, an amount defined in a specific Currency.</summary>
    public partial struct Money : IFormattable
    {
        /// <summary>Converts this <see cref="Money"/> instance to its equivalent <see cref="string"/> representation.</summary>
        /// <returns>A string that represents this <see cref="Money"/> instance.</returns>
        /// <remarks>
        /// Converting will use the <see cref="NumberFormatInfo"/> object for the current culture if this has the same
        /// ISOCurrencySymbol, otherwise the <see cref="NumberFormatInfo"/> from the <see cref="Currency"/> will be used.
        /// </remarks>
        public override string ToString() => ConvertToString(null, null);

        /// <summary>Converts the <see cref="Money"/> value of this instance to its equivalent <see cref="string"/> representation
        /// using the specified format.</summary>
        /// <param name="format">A numeric format string.</param>
        /// <returns>The string representation of this <see cref="Money"/> instance as specified by the format.</returns>
        public string ToString(string format)
        {
            return ConvertToString(format, null);
        }

        /// <summary>Converts this <see cref="Money"/> instance to its equivalent <see cref="string"/> representation using the
        /// specified culture-specific format information.</summary>
        /// <param name="formatProvider">An <see cref="IFormatProvider"/> that supplies culture-specific formatting information.</param>
        /// <returns>The string representation of this <see cref="Money"/> instance as specified by formatProvider.</returns>
        public string ToString(IFormatProvider formatProvider)
        {
            return ConvertToString(null, formatProvider);
        }

        /// <summary>Converts the <see cref="Money"/> value of this instance to its equivalent <see cref="string"/> representation
        /// using the specified format and culture-specific format information.</summary>
        /// <param name="format">A numeric format string.</param>
        /// <param name="formatProvider">An <see cref="IFormatProvider"/> that supplies culture-specific formatting information.</param>
        /// <returns>The string representation of this <see cref="Money"/> instance as specified by the format and formatProvider.</returns>
        public string ToString(string format, IFormatProvider formatProvider)
        {
            return ConvertToString(format, formatProvider);
        }

        private static IFormatProvider GetFormatProvider(Currency currency, IFormatProvider formatProvider, bool useCode = false)
        {
            CultureInfo cc = CultureInfo.CurrentCulture;

            // var numberFormatInfo = (NumberFormatInfo)NumberFormatInfo.CurrentInfo.Clone();
            var numberFormatInfo = (NumberFormatInfo)cc.NumberFormat.Clone();

            if (formatProvider != null)
            {
                if (formatProvider is CultureInfo ci)
                    numberFormatInfo = (NumberFormatInfo)ci.NumberFormat.Clone();

                if (formatProvider is NumberFormatInfo nfi)
                    numberFormatInfo = (NumberFormatInfo)nfi.Clone();
            }

            numberFormatInfo.CurrencyDecimalDigits = (int)currency.DecimalDigits;
            numberFormatInfo.CurrencySymbol = currency.Symbol;

            if (useCode)
            {
                // Replace symbol with the code
                numberFormatInfo.CurrencySymbol = currency.Code;

                // Add spacing to PositivePattern and NegativePattern
                if (numberFormatInfo.CurrencyPositivePattern == 0) // $n
                    numberFormatInfo.CurrencyPositivePattern = 2; // $ n
                if (numberFormatInfo.CurrencyPositivePattern == 1) // n$
                    numberFormatInfo.CurrencyPositivePattern = 3; // n $

                switch (numberFormatInfo.CurrencyNegativePattern)
                {
                    case 0: // ($n)
                        numberFormatInfo.CurrencyNegativePattern = 14; // ($ n)
                        break;
                    case 1: // -$n
                        numberFormatInfo.CurrencyNegativePattern = 9; // -$ n
                        break;
                    case 2: // $-n
                        numberFormatInfo.CurrencyNegativePattern = 12; // $ -n
                        break;
                    case 3: // $n-
                        numberFormatInfo.CurrencyNegativePattern = 11; // $ n-
                        break;
                    case 4: // (n$)
                        numberFormatInfo.CurrencyNegativePattern = 15; // (n $)
                        break;
                    case 5: // -n$
                        numberFormatInfo.CurrencyNegativePattern = 8; // -n $
                        break;
                    case 6: // n-$
                        numberFormatInfo.CurrencyNegativePattern = 13; // n- $
                        break;
                    case 7: // n$-
                        numberFormatInfo.CurrencyNegativePattern = 10; // n $-
                        break;
                }
            }

            return numberFormatInfo;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Globalization",
            "CA1307:Specify StringComparison",
            Justification = "Invalid overload; known bug in code analysis, see https://github.com/dotnet/roslyn-analyzers/issues/1552")]
        private string ConvertToString(string format, IFormatProvider formatProvider)
        {
            // TODO: ICustomFormat : http://msdn.microsoft.com/query/dev12.query?appId=Dev12IDEF1&l=EN-US&k=k(System.IFormatProvider);k(TargetFrameworkMoniker-.NETPortable,Version%3Dv4.6);k(DevLang-csharp)&rd=true

            // TODO: Hacked solution, solve with better implementation
            IFormatProvider provider;
            if (!string.IsNullOrWhiteSpace(format) && format.StartsWith("I", StringComparison.Ordinal) && format.Length >= 1 && format.Length <= 2)
            {
#if NETFRAMEWORK || NETSTANDARD2_0
                format = format.Replace("I", "C");
#else
                format = format.Replace("I", "C", StringComparison.Ordinal);
#endif
                provider = GetFormatProvider(Currency, formatProvider, true);
            }
            else
            {
                provider = GetFormatProvider(Currency, formatProvider);
            }

            if (format == null || format == "G")
            {
                format = "C";
            }

            if (format.StartsWith("F", StringComparison.Ordinal))
            {
#if NETFRAMEWORK || NETSTANDARD2_0
                format = format.Replace("F", "N");
#else
                format = format.Replace("F", "N", StringComparison.Ordinal);
#endif
                if (format.Length == 1)
                {
                    format += Currency.DecimalDigits;
                }

                return $"{Amount.ToString(format, provider)} {Currency.EnglishName}";
            }

            return Amount.ToString(format ?? "C", provider);
        }
    }
}
