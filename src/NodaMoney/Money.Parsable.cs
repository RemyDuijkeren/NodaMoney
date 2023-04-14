using System;
using System.Globalization;
using System.Linq;

namespace NodaMoney
{
    /// <summary>Represents Money, an amount defined in a specific Currency.</summary>
    public partial struct Money
    {
        /// <summary>Converts the string representation of a money value to its <see cref="Money"/> equivalent.</summary>
        /// <param name="value">The string representation of the number to convert.</param>
        /// <returns>The equivalent to the money amount contained in <i>value</i>.</returns>
        /// <exception cref="System.ArgumentNullException"><i>value</i> is <b>null</b> or empty.</exception>
        /// <exception cref="System.FormatException"><i>value</i> is not in the correct format or the currency sign matches with multiple known currencies.</exception>
        /// <exception cref="System.OverflowException"><i>value</i> represents a number less than <see cref="decimal.MinValue"/> or greater than <see cref="decimal.MaxValue"/>.</exception>
        public static Money Parse(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException(nameof(value));

            Currency currency = ExtractCurrencyFromString(value);

            return Parse(value, NumberStyles.Currency, GetFormatProvider(currency, null), currency);
        }

        /// <summary>Converts the string representation of a money value to its <see cref="Money"/> equivalent.</summary>
        /// <param name="value">The string representation of the number to convert.</param>
        /// <param name="currency">The currency to use for parsing the string representation.</param>
        /// <returns>The equivalent to the money amount contained in <i>value</i>.</returns>
        /// <exception cref="System.ArgumentNullException"><i>value</i> is <b>null</b> or empty.</exception>
        /// <exception cref="System.FormatException"><i>value</i> is not in the correct format or the currency sign matches with multiple known currencies.</exception>
        /// <exception cref="System.OverflowException"><i>value</i> represents a number less than <see cref="decimal.MinValue"/> or greater than <see cref="decimal.MaxValue"/>.</exception>
        public static Money Parse(string value, Currency currency)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException(nameof(value));

            return Parse(value, NumberStyles.Currency, GetFormatProvider(currency, null), currency);
        }

        /// <summary>Converts the string representation of a money value to its <see cref="Money"/> equivalent.</summary>
        /// <param name="value">The string representation of the number to convert.</param>
        /// <param name="style">A bitwise combination of enumeration values that indicates the permitted format of value. A typical value to specify is <see cref="NumberStyles.Currency"/>.</param>
        /// <param name="provider">An object that supplies culture-specific parsing information about <i>value</i>.</param>
        /// <param name="currency">The currency to use for parsing the string representation.</param>
        /// <returns>The equivalent to the money amount contained in <i>value</i>.</returns>
        /// <exception cref="System.ArgumentNullException"><i>value</i> is <b>null</b> or empty.</exception>
        /// <exception cref="System.FormatException"><i>value</i> is not in the correct format or the currency sign matches with multiple known currencies.</exception>
        /// <exception cref="System.OverflowException"><i>value</i> represents a number less than <see cref="decimal.MinValue"/> or greater than <see cref="decimal.MaxValue"/>.</exception>
        public static Money Parse(string value, NumberStyles style, IFormatProvider provider, Currency currency)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException(nameof(value));

            decimal amount = decimal.Parse(value, style, GetFormatProvider(currency, provider));
            return new Money(amount, currency);
        }

        /// <summary>Converts the string representation of a money value to its <see cref="Money"/> equivalent. A return value indicates whether the conversion succeeded or failed.</summary>
        /// <param name="value">The string representation of the money to convert.</param>
        /// <param name="result">When this method returns, contains the <see cref="Money"/> value that is equivalent to the money
        /// value contained in <i>value</i>, if the conversion succeeded, or is Money value of zero with no currency (XXX) if the
        /// conversion failed. The conversion fails if the <i>value</i> parameter is <b>null</b> or <see cref="string.Empty"/>, is not a number
        /// in a valid format, or represents a number less than <see cref="decimal.MinValue"/> or greater than <see cref="decimal.MaxValue"/>. This parameter is passed
        /// uninitialized; any <i>value</i> originally supplied in result will be overwritten.</param>
        /// <returns><b>true</b> if <i>value</i> was converted successfully; otherwise, <b>false</b>.</returns>
        /// <remarks>See <see cref="decimal.TryParse(string, out decimal)"/> for more info and remarks.</remarks>
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

            return TryParse(value, NumberStyles.Currency, GetFormatProvider(currency, null), currency, out result);
        }

        /// <summary>Converts the string representation of a money value to its <see cref="Money"/> equivalent. A return value indicates whether the conversion succeeded or failed.</summary>
        /// <param name="value">The string representation of the money to convert.</param>
        /// <param name="currency">The currency to use for parsing the string representation.</param>
        /// <param name="result">When this method returns, contains the <see cref="Money"/> value that is equivalent to the money
        /// value contained in <i>value</i>, if the conversion succeeded, or is Money value of zero with no currency (XXX) if the
        /// conversion failed. The conversion fails if the <i>value</i> parameter is <b>null</b> or <see cref="string.Empty"/>, is not a number
        /// in a valid format, or represents a number less than <see cref="decimal.MinValue"/> or greater than <see cref="decimal.MaxValue"/>. This parameter is passed
        /// uninitialized; any <i>value</i> originally supplied in result will be overwritten.</param>
        /// <returns><b>true</b> if <i>value</i> was converted successfully; otherwise, <b>false</b>.</returns>
        /// <remarks>See <see cref="decimal.TryParse(string, out decimal)"/> for more info and remarks.</remarks>
        public static bool TryParse(string value, Currency currency, out Money result)
        {
            return TryParse(value, NumberStyles.Currency, GetFormatProvider(currency, null), currency, out result);
        }

        /// <summary>Converts the string representation of a money value to its <see cref="Money"/> equivalent. A return value indicates whether the conversion succeeded or failed.</summary>
        /// <param name="value">The string representation of the money to convert.</param>
        /// <param name="style">A bitwise combination of enumeration values that indicates the permitted format of value. A typical value to specify is <see cref="NumberStyles.Currency"/>.</param>
        /// <param name="provider">An object that supplies culture-specific parsing information about <i>value</i>.</param>
        /// <param name="currency">The currency to use for parsing the string representation.</param>
        /// <param name="result">When this method returns, contains the <see cref="Money"/> value that is equivalent to the money
        /// value contained in <i>value</i>, if the conversion succeeded, or is Money value of zero with no currency (XXX) if the
        /// conversion failed. The conversion fails if the <i>value</i> parameter is <b>null</b> or <see cref="string.Empty"/>, is not a number
        /// in a valid format, or represents a number less than <see cref="decimal.MinValue"/> or greater than <see cref="decimal.MaxValue"/>. This parameter is passed
        /// uninitialized; any <i>value</i> originally supplied in result will be overwritten.</param>
        /// <returns><b>true</b> if <i>value</i> was converted successfully; otherwise, <b>false</b>.</returns>
        /// <remarks>See <see cref="decimal.TryParse(string, NumberStyles, IFormatProvider, out decimal)"/> for more info and remarks.</remarks>
        public static bool TryParse(string value, NumberStyles style, IFormatProvider provider, Currency currency, out Money result)
        {
            bool isParsingSuccessful = decimal.TryParse(value, style, GetFormatProvider(currency, provider), out decimal amount);
            if (isParsingSuccessful)
            {
                result = new Money(amount, currency);
                return true;
            }

            result = new Money(0, Currency.FromCode("XXX"));
            return false;
        }

        private static Currency ExtractCurrencyFromString(string moneyValue)
        {
            // It is not sufficient to check for
            //    !char.IsDigit(character) &&
            //    !char.IsWhiteSpace(character) &&
            //    !char.IsPunctuation(character) &&
            //    !char.IsSeparator(character);
            // because if fails for a number of currency symbols containing '.' or '/'.
            // Of particular interest is a trailing '.' in the currency symbol
            // which must not be treated as part of the amount.
            // TODO: How to handle alternative symbols, like US$
            string symbol = CurrencySymbolParser.Parse(moneyValue);
            Currency currency;
            if (symbol.Length == 0 ||
                string.Equals(symbol, Currency.CurrentCurrency.Symbol, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(symbol, Currency.CurrentCurrency.Code, StringComparison.OrdinalIgnoreCase))
            {
                currency = Currency.CurrentCurrency;
            }
            else
            {
                var currencies = Currency.GetCurrencies(symbol).ToArray();
                if (currencies.Length == 0)
                {
                    if (!Currency.Registry.TryGet(code: symbol, out currency))
                        throw new FormatException($@"""{symbol}"" must be a known currency symbol or code.");
                }
                else if (currencies.Length > 1)
                {
                    throw new FormatException($@"Currency symbol ""{symbol}"" matches with {currencies.Length} known currencies! Specify currency or culture explicitly.");
                }
                else
                {
                    currency = currencies[0];
                }
            }

            return currency;
        }
    }
}
