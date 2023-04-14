using System;
using System.Text.RegularExpressions;

namespace NodaMoney
{
    /// <summary>Represents Money, an amount defined in a specific Currency.</summary>
    public static class CurrencySymbolParser
    {
        private const int SuffixSymbolGroupIndex = 4;
        private const int PrefixSymbolGroupIndex = 5;

        private static readonly Regex CurrencySymbolMatcher = new Regex(@"^\(?\s*(([-+\d](.*\d)?)\s*([^-+\d\s]+)|([^-+\d\s]+)\s*([-+\d](.*\d)?))\s*\)?$");

        /// <summary>Extracts the currency symbol from a given money value.</summary>
        /// <param name="moneyValue">The string representation of the money value to parse.</param>
        /// <returns>Returns the currency symbol extracted from the specified <paramref name="moneyValue"/>,
        /// or <see cref="string.Empty"/> if no curreny symbol was found.</returns>
        /// <exception cref="System.ArgumentNullException">The <i>moneyValue</i> is <b>null</b> or empty.</exception>
        public static string Parse(string moneyValue)
        {
            if (string.IsNullOrWhiteSpace(moneyValue))
                throw new ArgumentNullException(nameof(moneyValue));

            var match = CurrencySymbolMatcher.Match(moneyValue);
            string symbol;
            if (match.Success)
            {
                var suffixSymbol = match.Groups[SuffixSymbolGroupIndex].Value;
                var prefixSymbol = match.Groups[PrefixSymbolGroupIndex].Value;
                symbol = suffixSymbol.Length == 0
                    ? prefixSymbol
                    : prefixSymbol.Length == 0 ? suffixSymbol : string.Empty;
            }
            else
            {
                symbol = string.Empty;
            }

            return symbol;
        }
    }
}
