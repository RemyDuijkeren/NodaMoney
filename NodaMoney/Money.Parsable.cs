using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace NodaMoney
{
    /// <summary>Represents Money, an amount defined in a specific Currency.</summary>
    public partial struct Money
    {
        /// <summary>Parses the specified s.</summary>
        /// <param name="s">The s.</param>
        /// <returns></returns>
        /// <exception cref="System.FormatException">Currency sign matches multiple known currencies! Specify currency or culture expliciet.</exception>
        public static Money Parse(string s)
        {
            if (string.IsNullOrWhiteSpace(s))
                throw new ArgumentNullException("s");

            // TODO: How to handle alternative symbols, like US$
            var currencyCharacters = new List<char>();
            var amountCharacters = new List<char>();
            foreach (char character in s)
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
                        "Currency sign matches multiple known currencies! Specify currency or culture expliciet.");
                }

                currency = match[0];
            }

            var nfi = GetNumberFormatInfo(currency, null);
            decimal d = Decimal.Parse(amountAsString, nfi);
            return new Money(d, currency);
        }
    }
}