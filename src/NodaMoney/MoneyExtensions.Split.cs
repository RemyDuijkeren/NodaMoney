using System.Diagnostics.CodeAnalysis;

namespace NodaMoney;

/// <summary>Extensions for <see cref="Money"/>.</summary>
public static class MoneyExtensions
{
    /// <summary>Divide the Money in equal shares, without losing Money.</summary>
    /// <param name="money">The <see cref="Money"/> instance.</param>
    /// <param name="shares">The number of shares to divide in.</param>
    /// <returns>An <see cref="IEnumerable{Money}"/> of Money.</returns>
    /// <exception cref="System.ArgumentOutOfRangeException">shares;Number of shares must be greater than 1.</exception>
    /// <remarks>As rounding mode, MidpointRounding.ToEven is used (<seealso cref="System.MidpointRounding"/>).
    /// The behavior of this method follows IEEE Standard 754, section 4. This kind of rounding is sometimes called
    /// rounding to nearest, or banker's rounding. It minimizes rounding errors that result from consistently rounding a
    /// midpoint value in a single direction.</remarks>
    public static IEnumerable<Money> Split(this Money money, int shares) => Split(money, shares, MidpointRounding.ToEven);

    /// <summary>Divide the Money in equal shares, without losing Money.</summary>
    /// <param name="money">The <see cref="Money"/> instance.</param>
    /// <param name="shares">The number of shares to divide in.</param>
    /// <param name="rounding">The rounding mode.</param>
    /// <returns>An <see cref="IEnumerable{Money}"/> of Money.</returns>
    /// <exception cref="System.ArgumentOutOfRangeException">shares;Number of shares must be greater than 1.</exception>
    [SuppressMessage("Microsoft.Usage", "CA2233:OperationsShouldNotOverflow", MessageId = "number-1", Justification = "Can't be lower than zero.")]
    public static IEnumerable<Money> Split(this Money money, int shares, MidpointRounding rounding)
    {
        if (shares <= 1)
            throw new ArgumentOutOfRangeException(nameof(shares), "Number of shares must be greater than 1");

        return DistributeIterator();

        IEnumerable<Money> DistributeIterator()
        {
            decimal shareAmount = Math.Round(money.Amount / shares, (int)CurrencyInfo.GetInstance(money.Currency).DecimalDigits, rounding);
            decimal remainder = money.Amount;

            for (int i = 0; i < shares - 1; i++)
            {
                remainder -= shareAmount;
                yield return new Money(shareAmount, money.Currency);
            }

            yield return new Money(remainder, money.Currency);
        }
    }

    /// <summary>Divide the Money in shares with a specific ratio, without losing Money.</summary>
    /// <param name="money">The <see cref="NodaMoney.Money"/> instance.</param>
    /// <param name="ratios">The number of shares as an array of ratios.</param>
    /// <returns>An <see cref="IEnumerable{Money}"/> of Money.</returns>
    /// <exception cref="System.ArgumentOutOfRangeException">ratios;Sum of ratios must be greater than 1.</exception>
    /// <remarks>As rounding mode, MidpointRounding.ToEven is used (<seealso cref="System.MidpointRounding"/>).
    /// The behavior of this method follows IEEE Standard 754, section 4. This kind of rounding is sometimes called
    /// rounding to nearest, or banker's rounding. It minimizes rounding errors that result from consistently rounding a
    /// midpoint value in a single direction.</remarks>
    public static IEnumerable<Money> Split(this Money money, int[] ratios) => Split(money, ratios, MidpointRounding.ToEven);

    /// <summary>Divide the Money in shares with a specific ratio, without losing Money.</summary>
    /// <param name="money">The <see cref="NodaMoney.Money"/> instance.</param>
    /// <param name="ratios">The number of shares as an array of ratios.</param>
    /// <param name="rounding">The rounding mode.</param>
    /// <returns>An <see cref="IEnumerable{Money}"/> of Money.</returns>
    /// <exception cref="ArgumentOutOfRangeException">shares;Number of shares must be greater than 1.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="ratios"/> is <c>null</c>.</exception>
    public static IEnumerable<Money> Split(this Money money, int[] ratios, MidpointRounding rounding)
    {
        if (ratios == null)
            throw new ArgumentNullException(nameof(ratios));
        if (ratios.Any(ratio => ratio < 1))
            throw new ArgumentOutOfRangeException(nameof(ratios), "All ratios must be greater or equal than 1");

        return DistributeIterator();

        IEnumerable<Money> DistributeIterator()
        {
            decimal remainder = money.Amount;

            for (int i = 0; i < ratios.Length - 1; i++)
            {
                decimal ratioAmount = Math.Round(
                    money.Amount * ratios[i] / ratios.Sum(),
                    (int)CurrencyInfo.GetInstance(money.Currency).DecimalDigits,
                    rounding);

                remainder -= ratioAmount;

                yield return new Money(ratioAmount, money.Currency);
            }

            yield return new Money(remainder, money.Currency);
        }
    }
}
