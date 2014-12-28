using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace NodaMoney
{
    /// <summary>A conversion of money of one currency into money of another currency</summary>
    /// <remarks>See <see cref="http://en.wikipedia.org/wiki/Exchange_rate"/>.</remarks>
    [DataContract]
    [ComVisible(true)]
    public struct ExchangeRate : IEquatable<ExchangeRate>
    {
        /// <summary>Initializes a new instance of the <see cref="ExchangeRate"/> struct.</summary>
        /// <param name="baseCurrency">The base currency.</param>
        /// <param name="quoteCurrency">The quote currency.</param>
        /// <param name="rate">The rate of the exchange.</param>
        public ExchangeRate(Currency baseCurrency, Currency quoteCurrency, decimal rate)
            : this()
        {
            if (baseCurrency == null)
                throw new ArgumentNullException("baseCurrency");
            if (rate < 0)
                throw new ArgumentOutOfRangeException("rate", "Rate must be greater than zero!");
            if (quoteCurrency == null)
                throw new ArgumentNullException("quoteCurrency");
            if (baseCurrency == quoteCurrency)
                throw new ArgumentException("The base and quote currency can't be equal!");

            BaseCurrency = baseCurrency;
            QuoteCurrency = quoteCurrency;
            Value = Math.Round(rate, 4); // value is a ratio 
        }

        /// <summary>Initializes a new instance of the <see cref="ExchangeRate"></see> struct.</summary>
        /// <param name="baseCurrency">The base currency.</param>
        /// <param name="quoteCurrency">The quote currency.</param>
        /// <param name="rate">The rate of the exchange.</param>
        public ExchangeRate(Currency baseCurrency, Currency quoteCurrency, double rate)
            : this(baseCurrency, quoteCurrency, (decimal)rate)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="ExchangeRate"/> struct.</summary>
        /// <param name="baseCode">The code of the base currency.</param>
        /// <param name="quoteCode">The code of the quote currency.</param>
        /// <param name="rate">The rate of the exchange.</param>
        public ExchangeRate(string baseCode, string quoteCode, decimal rate)
            : this(Currency.FromCode(baseCode), Currency.FromCode(quoteCode), rate)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="ExchangeRate"/> struct.</summary>
        /// <param name="baseCode">The code of the base currency.</param>
        /// <param name="quoteCode">The code of the quote currency.</param>
        /// <param name="rate">The rate of the exchange.</param>
        public ExchangeRate(string baseCode, string quoteCode, double rate)
            : this(Currency.FromCode(baseCode), Currency.FromCode(quoteCode), rate)
        {
        }

        /// <summary>Gets the base currency.</summary>
        /// <value>The base currency.</value>
        public Currency BaseCurrency { get; private set; }

        /// <summary>Gets the quote currency.</summary>
        /// <value>The quote currency.</value>
        public Currency QuoteCurrency { get; private set; }

        /// <summary>Gets the value of the exchange rate.</summary>
        /// <value>The value of the exchange rate.</value>
        public decimal Value { get; private set; }

        /// <summary>Converts the string representation of an exchange rate to its <see cref="ExchangeRate"/> equivalent.</summary>
        /// <param name="rate">The string representation of the exchange rate to convert.</param>
        /// <returns>The equivalent to the exchange rate contained in rate.</returns>
        /// <exception cref="System.FormatException">rate is not in the correct format!</exception>
        public static ExchangeRate Parse(string rate)
        {
            ExchangeRate fx;
            if (!TryParse(rate, out fx))
                throw new FormatException("rate is not in the correct format! Currencies are the same or the rate is not a number.");

            return fx;
        }

        /// <summary>Converts the string representation of an exchange rate to its <see cref="ExchangeRate"/> equivalent. A return value indicates whether the conversion succeeded or failed.</summary>
        /// <param name="rate">The string representation of the exchange rate to convert.</param>
        /// <param name="result">When this method returns, contains the <see cref="ExchangeRate"/> that is equivalent to the exchange rate contained in rate, if the conversion succeeded,
        /// or is zero if the conversion failed. The conversion fails if the rate parameter is null, is not a exchange rate in a valid format, or represents a number less than MinValue 
        /// or greater than MaxValue. This parameter is passed uninitialized.</param>
        /// <returns><b>true</b> if rate was converted successfully; otherwise, <b>false</b>.</returns>
        public static bool TryParse(string rate, out ExchangeRate result)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(rate))
                    throw new ArgumentNullException("rate");

                rate = rate.Trim();
                var baseCurrency = Currency.FromCode(rate.Substring(0, 3));
                int index = rate.Substring(3, 1) == "/" ? 4 : 3;
                var quoteCurrency = Currency.FromCode(rate.Substring(index, 3));
                var value = decimal.Parse(rate.Remove(0, index + 3), NumberFormatInfo.CurrentInfo);
                
                result = new ExchangeRate(baseCurrency, quoteCurrency, value);
                return true;
            }
            catch (Exception ex)
            {
                if (ex is FormatException || ex is OverflowException || ex is ArgumentException)
                {
                    result = new ExchangeRate
                    {
                        BaseCurrency = Currency.FromCode("XXX"),
                        QuoteCurrency = Currency.FromCode("XXX"),
                        Value = 0
                    };
                    return false;
                }

                throw;
            }
        }

        /// <summary>Implements the operator ==.</summary>
        /// <param name="left">The left ExchangeRate.</param>
        /// <param name="right">The right ExchangeRate.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(ExchangeRate left, ExchangeRate right)
        {
            return left.Equals(right);
        }

        /// <summary>Implements the operator !=.</summary>
        /// <param name="left">The left ExchangeRate.</param>
        /// <param name="right">The right ExchangeRate.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(ExchangeRate left, ExchangeRate right)
        {
            return !(left == right);
        }

        /// <summary>Converts the specified money.</summary>
        /// <param name="money">The money.</param>
        /// <returns>The converted money.</returns>
        public Money Convert(Money money)
        {
            if (money.Currency != BaseCurrency && money.Currency != QuoteCurrency)
                throw new ArgumentException("Money should have the same currency as the base currency or the quote currency!", "money");
            
            return money.Currency == BaseCurrency
                       ? new Money(money.Amount * Value, QuoteCurrency)
                       : new Money(money.Amount / Value, BaseCurrency);
        }

        /// <summary>Returns the hash code for this instance.</summary>
        /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return Value.GetHashCode() + (397 * BaseCurrency.GetHashCode()) + (397 * QuoteCurrency.GetHashCode());
            }
        }

        /// <summary>Indicates whether this instance and a specified <see cref="ExchangeRate"/> are equal.</summary>
        /// <param name="other">Another object to compare to.</param>
        /// <returns>true if <paramref name="other"/> and this instance are the same type and represent the same value; otherwise, false.</returns>
        public bool Equals(ExchangeRate other)
        {
            return Value == other.Value && BaseCurrency == other.BaseCurrency && QuoteCurrency == other.QuoteCurrency;
        }

        /// <summary>Indicates whether this instance and a specified object are equal.</summary>
        /// <param name="obj">Another object to compare to.</param>
        /// <returns>true if <paramref name="obj"/> and this instance are the same type and represent the same value; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            return (obj is ExchangeRate) && this.Equals((ExchangeRate)obj);
        }

        /// <summary>Converts this <see cref="ExchangeRate"/> instance to its equivalent <see cref="String"/> representation.</summary>
        /// <returns>A string that represents this <see cref="ExchangeRate"/> instance.</returns>
        /// <remarks>See http://en.wikipedia.org/wiki/Currency_Pair for more info about how an ExchangeRate can be presented.</remarks>
        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, "{0}/{1} {2}", BaseCurrency.Code, QuoteCurrency.Code, Value);
        }
    }
}
