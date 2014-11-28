using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace NodaMoney.Extensions
{
    /// <summary>Extensions for <see cref="T:NodaMoney.Money"/>.</summary>
    public static class MoneyExtensions
    {
        /// <summary>Divide the Money in equal shares, without losing Money.</summary>
        /// <param name="money">The <see cref="T:NodaMoney.Money"/> instance.</param>
        /// <param name="shares">The number of shares to divide in.</param>
        /// <returns>An <see cref="IEnumerable{Money}"/> of Money.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">shares;Number of shares must be greater than 1</exception>
        /// <remarks>
        /// As rounding mode, MidpointRounding.ToEven is used (<seealso cref="http://msdn.microsoft.com/en-us/library/system.midpointrounding.aspx"/>). 
        /// The behavior of this method follows IEEE Standard 754, section 4. This kind of rounding is sometimes called
        /// rounding to nearest, or banker's rounding. It minimizes rounding errors that result from consistently rounding a 
        /// midpoint value in a single direction.
        /// </remarks>
        public static IEnumerable<Money> SafeDivide(this Money money, int shares)
        {
            return SafeDivide(money, shares, MidpointRounding.ToEven);
        }

        /// <summary>Divide the Money in equal shares, without losing Money.</summary>
        /// <param name="money">The <see cref="T:NodaMoney.Money"/> instance.</param>
        /// <param name="shares">The number of shares to divide in.</param>
        /// <param name="rounding">The rounding mode.</param>
        /// <returns>An <see cref="IEnumerable{Money}" /> of Money.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">shares;Number of shares must be greater than 1</exception>
        [SuppressMessage("Microsoft.Usage", "CA2233:OperationsShouldNotOverflow", MessageId = "number-1", Justification = "Can't be lower than zero.")]
        public static IEnumerable<Money> SafeDivide(this Money money, int shares, MidpointRounding rounding)
        {
            if (shares <= 1)
                throw new ArgumentOutOfRangeException("shares", "Number of shares must be greater than 1");

            decimal shareAmount = Math.Round(money.Amount / shares, (int)money.Currency.DecimalDigits, rounding);
            decimal remainder = money.Amount;

            for (int i = 0; i < shares - 1; i++)
            {
                remainder -= shareAmount;
                yield return new Money(shareAmount, money.Currency);
            }

            yield return new Money(remainder, money.Currency);
        }

        /// <summary>Divide the Money in shares with a specific ratio, without losing Money.</summary>
        /// <param name="money">The <see cref="T:NodaMoney.Money"/> instance.</param>
        /// <param name="ratios">The number of shares as an array of ratios.</param>
        /// <returns>An <see cref="IEnumerable{Money}"/> of Money.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">ratios;Sum of ratios must be greater than 1</exception>
        /// <remarks>
        /// As rounding mode, MidpointRounding.ToEven is used (<seealso cref="http://msdn.microsoft.com/en-us/library/system.midpointrounding.aspx"/>). 
        /// The behavior of this method follows IEEE Standard 754, section 4. This kind of rounding is sometimes called
        /// rounding to nearest, or banker's rounding. It minimizes rounding errors that result from consistently rounding a 
        /// midpoint value in a single direction.
        /// </remarks>
        public static IEnumerable<Money> SafeDivide(this Money money, int[] ratios)
        {
            return SafeDivide(money, ratios, MidpointRounding.ToEven);
        }

        /// <summary>Divide the Money in shares with a specific ratio, without losing Money.</summary>
        /// <param name="money">The <see cref="T:NodaMoney.Money"/> instance.</param>
        /// <param name="ratios">The number of shares as an array of ratios.</param>
        /// <param name="rounding">The rounding mode.</param>
        /// <returns>An <see cref="IEnumerable{Money}" /> of Money.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">shares;Number of shares must be greater than 1</exception>
        public static IEnumerable<Money> SafeDivide(this Money money, int[] ratios, MidpointRounding rounding)
        {
            if (ratios.Sum() <= 1)
                throw new ArgumentOutOfRangeException("ratios", "Sum of ratios must be greater than 1");

            decimal shareAmount = Math.Round(money.Amount / ratios.Sum(), (int)money.Currency.DecimalDigits, rounding);
            decimal remainder = money.Amount;

            for (int i = 0; i < ratios.Length - 1; i++)
            {
                decimal ratioAmount = Math.Round(money.Amount * ratios[i] / ratios.Sum(), (int)money.Currency.DecimalDigits, rounding);
                remainder -= ratioAmount;
                yield return new Money(ratioAmount, money.Currency);
            }

            yield return new Money(remainder, money.Currency);
        }
    }
}