using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace NodaMoney
{
    /// <summary>Represents Money, an amount defined in a specific Currency.</summary>
    public partial struct Money
    {
        /// <summary>Parses the specified string.</summary>
        /// <param name="value">The string to parse to a Money instance.</param>
        /// <returns>A Money instance from the specified string.</returns>
        /// <exception cref="System.FormatException">Currency sign matches multiple known currencies! Specify currency or culture explicit.</exception>
        public static Money Parse(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException("value");

            // TODO: How to handle alternative symbols, like US$
            var currencyCharacters = new List<char>();
            var amountCharacters = new List<char>();
            foreach (char character in value)
            {
                if (char.IsDigit(character) || character == '.' || character == ',')
                {
                    amountCharacters.Add(character);
                }
                else if (!char.IsWhiteSpace(character))
                {
                    currencyCharacters.Add(character);
                }
            }

            string amountAsString = new string(amountCharacters.ToArray());
            string currencyAsString = new string(currencyCharacters.ToArray());

            var currency = Currency.FromCulture(CultureInfo.CurrentCulture);
            if (currency.Sign != currencyAsString && currency.Code != currencyAsString)
            {
                List<Currency> match =
                    Currency.GetAllCurrencies().Where(c => c.Sign == currencyAsString || c.Code == currencyAsString).ToList();

                if (match.Count == 0 && currencyAsString.Length != 0)
                {
                    throw new FormatException(string.Format(CultureInfo.InvariantCulture, "{0} is an unknown currency sign or code!", currencyAsString));
                }

                if (match.Count > 1)
                {
                    throw new FormatException(
                        "Currency sign matches multiple known currencies! Specify currency or culture explicit.");
                }

                currency = match[0];
            }

            var nfi = GetNumberFormatInfo(currency, null);
            decimal d = decimal.Parse(amountAsString, nfi);
            return new Money(d, currency);
        }
    }
}