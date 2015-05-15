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
        /// <exception cref="System.FormatException">Currency sign matches with multiple known currencies! Specify currency or culture explicit.</exception>
        public static Money Parse(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException("value");

            Currency currency = ExtractCurrencyFromString(value);

            return Parse(value, NumberStyles.Currency, GetNumberFormatInfo(currency, null), currency);
        }

        public static Money Parse(string value, Currency currency)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException("value");

            return Parse(value, NumberStyles.Currency, GetNumberFormatInfo(currency, null), currency);
        }

        public static Money Parse(string value, NumberStyles style, IFormatProvider provider, Currency currency)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException("value");

            decimal amount = decimal.Parse(value, style, GetNumberFormatInfo(currency, provider));
            return new Money(amount, currency);
        }

        public static bool TryParse(string value, out Money result)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                result = new Money(0, Currency.FromCode("XXX"));
                return false;
            }

            Currency currency;
            try
            {
                currency = ExtractCurrencyFromString(value);
            }
            catch (FormatException)
            {
                result = new Money(0, Currency.FromCode("XXX"));
                return false;
            }

            return TryParse(value, NumberStyles.Currency, GetNumberFormatInfo(currency, null), currency, out result);
        }

        public static bool TryParse(string value, Currency currency, out Money result)
        {
            return TryParse(value, NumberStyles.Currency, GetNumberFormatInfo(currency, null), currency, out result);
        }

        public static bool TryParse(string value, NumberStyles style, IFormatProvider provider, Currency currency, out Money result)
        {
            decimal amount;
            bool isParsingSuccessful = decimal.TryParse(value, style, GetNumberFormatInfo(currency, provider), out amount);

            if (isParsingSuccessful)
            {
                result = new Money(amount, currency);
                return true;
            }

            result = new Money(0, Currency.FromCode("XXX"));
            return false;
        }

        private static Currency ExtractCurrencyFromString(string value)
        {
            // TODO: How to handle alternative symbols, like US$
            var currencyCharacters = new List<char>();
            var amountCharacters = new List<char>();
            foreach (char character in value)
            {
                if (char.IsDigit(character) || char.IsWhiteSpace(character) || character == '.' || character == ','
                    || character == '(' || character == ')' || character == '+' || character == '-')
                    amountCharacters.Add(character);
                else
                    currencyCharacters.Add(character);
            }

            // string amountAsString = new string(amountCharacters.ToArray());
            string currencyAsString = new string(currencyCharacters.ToArray());

            if (currencyAsString.Length == 0 || Currency.CurrentCurrency.Sign == currencyAsString
                || Currency.CurrentCurrency.Code == currencyAsString)
                return Currency.CurrentCurrency;

            List<Currency> match =
                Currency.GetAllCurrencies().Where(c => c.Sign == currencyAsString || c.Code == currencyAsString).ToList();

            if (match.Count == 0)
            {
                throw new FormatException(
                    string.Format(CultureInfo.InvariantCulture, "{0} is an unknown currency sign or code!", currencyAsString));
            }

            if (match.Count > 1)
            {
                throw new FormatException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Currency sign {0} matches with multiple known currencies! Specify currency or culture explicit.",
                        currencyAsString));
            }

            return match[0];
        }
    }
}