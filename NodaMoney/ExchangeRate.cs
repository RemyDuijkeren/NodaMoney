using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;

namespace NodaMoney
{
    /// <summary>A conversion of money of one currency into money of another currency</summary>
    /// <remarks>See http://en.wikipedia.org/wiki/Exchange_rate .</remarks>
    [DataContract]
    public struct ExchangeRate : IEquatable<ExchangeRate>
    {
        /// <summary>Initializes a new instance of the <see cref="ExchangeRate"/> struct.</summary>
        /// <param name="baseCurrency">The base currency.</param>
        /// <param name="quoteCurrency">The quote currency.</param>
        /// <param name="rate">The rate of the exchange.</param>
        /// <param name="quoteTime">The quote time. (optional)</param>
        /// <exception cref="ArgumentNullException">The value of 'baseCurrency' or 'quoteCurrency' cannot be null. </exception>
        /// <exception cref="ArgumentOutOfRangeException">Rate must be greater than zero!</exception>
        /// <exception cref="ArgumentException">The base and quote currency can't be equal!</exception>
        public ExchangeRate(Currency baseCurrency, Currency quoteCurrency, decimal rate, DateTime? quoteTime = default(DateTime?))
            : this()
        {
            if (baseCurrency == null)
                throw new ArgumentNullException(nameof(baseCurrency));
            if (rate < 0)
                throw new ArgumentOutOfRangeException(nameof(rate), "Rate must be greater than zero!");
            if (quoteCurrency == null)
                throw new ArgumentNullException(nameof(quoteCurrency));
            if (baseCurrency == quoteCurrency)
                throw new ArgumentException("The base and quote currency can't be equal!");

            BaseCurrency = baseCurrency;
            QuoteCurrency = quoteCurrency;
            Value = Math.Round(rate, 4); // value is a ratio 
            QuoteTime = (quoteTime ?? DateTime.UtcNow).ToUniversalTime();
        }

        /// <summary>Initializes a new instance of the <see cref="ExchangeRate"></see> struct.</summary>
        /// <param name="baseCurrency">The base currency.</param>
        /// <param name="quoteCurrency">The quote currency.</param>
        /// <param name="rate">The rate of the exchange.</param>
        /// <param name="quoteTime">The quote time. (optional)</param>
        public ExchangeRate(Currency baseCurrency, Currency quoteCurrency, double rate, DateTime? quoteTime = default(DateTime?))
            : this(baseCurrency, quoteCurrency, (decimal)rate, quoteTime)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="ExchangeRate"/> struct.</summary>
        /// <param name="baseCode">The code of the base currency.</param>
        /// <param name="quoteCode">The code of the quote currency.</param>
        /// <param name="rate">The rate of the exchange.</param>
        /// <param name="quoteTime">The quote time. (optional)</param>
        public ExchangeRate(string baseCode, string quoteCode, decimal rate, DateTime? quoteTime = default(DateTime?))
            : this(Currency.FromCode(baseCode), Currency.FromCode(quoteCode), rate, quoteTime)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="ExchangeRate"/> struct.</summary>
        /// <param name="baseCode">The code of the base currency.</param>
        /// <param name="quoteCode">The code of the quote currency.</param>
        /// <param name="rate">The rate of the exchange.</param>
        /// <param name="quoteTime">The quote time. (optional)</param>
        public ExchangeRate(string baseCode, string quoteCode, double rate, DateTime? quoteTime = default(DateTime?))
            : this(Currency.FromCode(baseCode), Currency.FromCode(quoteCode), rate, quoteTime)
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

        /// <summary>Gets the (UTC) quote time of the exchange rate.</summary>
        /// <value>The quote time of the exchange rate in UTC format.</value>
        public DateTime QuoteTime { get; private set; }

        /// <summary>Indicates if the exchange rate is available on or after specified date</summary>
        /// <param name="date"></param>
        /// <returns>true if <paramref name="date"/> equals or greater than quote time, otherwise false.</returns>
        public bool IsAvailable(DateTime date)
        {
            if (DateTimeKind.Unspecified == date.Kind)
                date = DateTime.SpecifyKind(date, DateTimeKind.Utc);
            else if (DateTimeKind.Local == date.Kind)
                return date.ToUniversalTime() >= QuoteTime;

            return date.Date.Equals(QuoteTime.Date);
        }

        /// <summary>Gets the exchange rate quotes.</summary>
        /// <returns>The exchange rate quotes.</returns>
        /// <remarks>If no exchange rate provider is defined, the dictionary contains the current quote.</remarks>
        public SortedDictionary<DateTime, decimal> GetQuotes()
        {
            return new SortedDictionary<DateTime, decimal>() { { QuoteTime, Value } };
        }

        /// <summary>Gets the quote on specified date.</summary>
        /// <param name="quoteDate">Date of quote</param>
        /// <returns>If a match is found, the quote is returned.</returns>
        /// <exception cref="NoExchangeRateQuoteFoundException">No available qoute found on specified date!</exception>
        public decimal GetDayQuote(DateTime quoteDate)
        {
            decimal quote;
            if (!TryGetDayQuote(quoteDate, out quote))
            {
                throw new NoExchangeRateQuoteFoundException(quoteDate.ToString());
            }
            return quote;
        }

        /// <summary>Gets the quote on specified date. A return
        /// value indicates whether the retrieval succeeded or failed.</summary>
        /// <param name="quoteDate">Date of quote.</param>
        /// <param name="result">When this method returns, contains the quote.</param>
        /// <returns><b>true</b> if a quote is found on specified date; otherwise, <b>false</b>.</returns>
        public bool TryGetDayQuote(DateTime quoteDate, out decimal result)
        {
            try
            {
                var rate = GetQuotes().First(x => quoteDate.ToUniversalTime().Date.Equals(x.Key.Date));
                result = rate.Value;
                return true;
            }
            catch (Exception)
            {
                result = 0m;
                return false;
            }
        }

        /// <summary>Converts the specified money using available quotes.</summary>
        /// <param name="money">The money.</param>
        /// <param name="exchangeDate">The date of the exchange. (optional)</param>
        /// <returns>The converted money.</returns>
        /// <exception cref="System.ArgumentException">Money should have the same currency as the base currency or the quote
        /// currency!</exception>
        /// <exception cref="NoExchangeRateQuoteFoundException">No available exchange rate qoute found!</exception>
        public Money ConvertWithAvailableQuotes(Money money, DateTime? exchangeDate = default(DateTime?))
        {
            if (money.Currency != BaseCurrency && money.Currency != QuoteCurrency)
            {
                throw new ArgumentException(
                    "Money should have the same currency as the base currency or the quote currency!",
                    nameof(money));
            }

            decimal quote = GetDayQuote((exchangeDate ?? DateTime.UtcNow).ToUniversalTime());

            return money.Currency == BaseCurrency
                       ? new Money(money.Amount * quote, QuoteCurrency)
                       : new Money(money.Amount / quote, BaseCurrency);
        }

        /// <summary>Converts the string representation of an exchange rate to its <see cref="ExchangeRate"/> equivalent.</summary>
        /// <param name="rate">The string representation of the exchange rate to convert.</param>
        /// <returns>The equivalent to the exchange rate contained in rate.</returns>
        /// <exception cref="System.FormatException">rate is not in the correct format!</exception>
        public static ExchangeRate Parse(string rate)
        {
            ExchangeRate fx;
            if (!TryParse(rate, out fx))
            {
                throw new FormatException(
                    "rate is not in the correct format! Currencies are the same or the rate is not a number.");
            }

            return fx;
        }

        /// <summary>Converts the string representation of an exchange rate to its <see cref="ExchangeRate"/> equivalent. A return
        /// value indicates whether the conversion succeeded or failed.</summary>
        /// <param name="rate">The string representation of the exchange rate to convert.</param>
        /// <param name="result">When this method returns, contains the <see cref="ExchangeRate"/> that is equivalent to the exchange rate contained in
        /// rate, if the conversion succeeded,
        /// or is zero if the conversion failed. The conversion fails if the rate parameter is null, is not a exchange rate in a
        /// valid format, or represents a number less than MinValue
        /// or greater than MaxValue. This parameter is passed uninitialized.</param>
        /// <returns><b>true</b> if rate was converted successfully; otherwise, <b>false</b>.</returns>
        public static bool TryParse(string rate, out ExchangeRate result)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(rate))
                    throw new ArgumentNullException(nameof(rate));

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
        /// <exception cref="System.ArgumentException">Money should have the same currency as the base currency or the quote
        /// currency!</exception>
        public Money Convert(Money money)
        {
            if (money.Currency != BaseCurrency && money.Currency != QuoteCurrency)
            {
                throw new ArgumentException(
                    "Money should have the same currency as the base currency or the quote currency!", 
                    nameof(money));
            }

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
        /// <returns>true if <paramref name="other"/> and this instance are the same type and represent the same value; otherwise,
        /// false.</returns>
        public bool Equals(ExchangeRate other)
        {
            return Value == other.Value && BaseCurrency == other.BaseCurrency && QuoteCurrency == other.QuoteCurrency;
        }

        /// <summary>Indicates whether this instance and a specified object are equal.</summary>
        /// <param name="obj">Another object to compare to.</param>
        /// <returns>true if <paramref name="obj"/> and this instance are the same type and represent the same value; otherwise,
        /// false.</returns>
        public override bool Equals(object obj)
        {
            // ReSharper disable once ArrangeThisQualifier
            return (obj is ExchangeRate) && this.Equals((ExchangeRate)obj);
        }

        /// <summary>Converts this <see cref="ExchangeRate"/> instance to its equivalent <see cref="String"/> representation.</summary>
        /// <returns>A string that represents this <see cref="ExchangeRate"/> instance.</returns>
        /// <remarks>See http://en.wikipedia.org/wiki/Currency_Pair for more info about how an ExchangeRate can be presented.</remarks>
        public override string ToString()
        {
            return $"{BaseCurrency.Code}/{QuoteCurrency.Code} {Value}";
        }
    }
}