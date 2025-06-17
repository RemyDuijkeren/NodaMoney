using System.Numerics;

namespace NodaMoney;

/// <summary>Represents Money, an amount defined in a specific Currency.</summary>
public partial struct Money
#if NET7_0_OR_GREATER
    : IAdditionOperators<Money, Money, Money>, IAdditionOperators<Money, decimal, Money>,
        ISubtractionOperators<Money, Money, Money>, ISubtractionOperators<Money, decimal, Money>,
        IMultiplyOperators<Money, decimal, Money>, IDivisionOperators<Money, decimal, Money>,
        IDivisionOperators<Money, Money, decimal>, IModulusOperators<Money, Money, Money>
#endif
{
    /// <summary>Adds two specified <see cref="Money"/> values.</summary>
    /// <param name="left">A <see cref="Money"/> object on the left side.</param>
    /// <param name="right">A <see cref="Money"/> object on the right side.</param>
    /// <returns>The <see cref="Money"/> result of adding left and right.</returns>
    public static Money operator +(Money left, Money right) => Add(left, right);

    /// <summary>Add the <see cref="Money"/> value with the given value.</summary>
    /// <param name="left">A <see cref="Money"/> object on the left side.</param>
    /// <param name="right">A <see cref="decimal"/> object on the right side.</param>
    /// <returns>The <see cref="Money"/> result of adding left and right.</returns>
    public static Money operator +(Money left, decimal right) => Add(left, right);

    /// <summary>Add the <see cref="Money"/> value with the given value.</summary>
    /// <param name="left">A <see cref="decimal"/> object on the left side.</param>
    /// <param name="right">A <see cref="Money"/> object on the right side.</param>
    /// <returns>The <see cref="Money"/> result of adding left and right.</returns>
    public static Money operator +(decimal left, Money right) => Add(right, left);

    /// <summary>Subtracts two specified <see cref="Money"/> values.</summary>
    /// <param name="left">A <see cref="Money"/> object on the left side.</param>
    /// <param name="right">A <see cref="Money"/> object on the right side.</param>
    /// <returns>The <see cref="Money"/> result of subtracting right from left.</returns>
    public static Money operator -(Money left, Money right) => Subtract(left, right);

    /// <summary>Subtracts <see cref="Money"/> value with the given value.</summary>
    /// <param name="left">A <see cref="Money"/> object on the left side.</param>
    /// <param name="right">A <see cref="decimal"/> object on the right side.</param>
    /// <returns>The <see cref="Money"/> result of subtracting right from left.</returns>
    public static Money operator -(Money left, decimal right) => Subtract(left, right);

    /// <summary>Subtracts <see cref="Money"/> value with the given value.</summary>
    /// <param name="left">A <see cref="decimal"/> object on the left side.</param>
    /// <param name="right">A <see cref="Money"/> object on the right side.</param>
    /// <returns>The <see cref="Money"/> result of subtracting right from left.</returns>
    public static Money operator -(decimal left, Money right) => Subtract(right, left);

    /// <summary>Multiplies the <see cref="Money"/> value by the given value.</summary>
    /// <param name="left">A <see cref="Money"/> object on the left side.</param>
    /// <param name="right">A <see cref="decimal"/> object on the right side.</param>
    /// <returns>The <see cref="Money"/> result of multiplying right with left.</returns>
    public static Money operator *(Money left, decimal right) => Multiply(left, right);

    /// <summary>Multiplies the <see cref="Money"/> value by the given value.</summary>
    /// <param name="left">A <see cref="decimal"/> object on the left side.</param>
    /// <param name="right">A <see cref="Money"/> object on the right side.</param>
    /// <returns>The <see cref="Money"/> result of multiplying left with right.</returns>
    public static Money operator *(decimal left, Money right) => Multiply(right, left);

    /// <summary>Divides the <see cref="Money"/> value by the given value.</summary>
    /// <param name="left">A <see cref="Money"/> object on the left side.</param>
    /// <param name="right">A <see cref="decimal"/> object on the right side.</param>
    /// <returns>The <see cref="Money"/> result of dividing left with right.</returns>
    /// <remarks>This division can lose money! Use <see cref="MoneyExtensions.Split()"/> to do a safe division.</remarks>
    public static Money operator /(Money left, decimal right) => Divide(left, right);

    /// <summary>Divides the <see cref="Money"/> value by the given value.</summary>
    /// <param name="left">A <see cref="Money"/> object on the left side.</param>
    /// <param name="right">A <see cref="Money"/> object on the right side.</param>
    /// <returns>The <see cref="decimal"/> result of dividing left with right.</returns>
    /// <remarks>Division of Money by Money, means the unit is lost, so the result will be a ratio <see cref="decimal"/>.</remarks>
    public static decimal operator /(Money left, Money right) => Divide(left, right);

    /// <summary>Divides two <see cref="Money"/> values togheter to compute their modulus or remainder.</summary>
    /// <param name="left">The <see cref="Money"/> value which rights divides.</param>
    /// <param name="right">The <see cref="Money"/> value which divides left.</param>
    /// <returns>The <see cref="Money"/> modulus or remainder of <see cref="left"/> divide by <see cref="right"/>.</returns>
    public static Money operator %(Money left, Money right) => Remainder(left, right);

    /// <summary>Adds two specified <see cref="Money"/> values.</summary>
    /// <param name="money1">The first <see cref="Money"/> object.</param>
    /// <param name="money2">The second <see cref="Money"/> object.</param>
    /// <returns>A <see cref="Money"/> object with the values of both <see cref="Money"/> objects added.</returns>
    public static Money Add(in Money money1, in Money money2)
    {
        // If one of the amounts is zero, then no need to check currency: Just return input value.
        if (money1.Amount == 0m)
            return money2;
        if (money2.Amount == 0m)
            return money1;

        EnsureSameCurrency(money1, money2);
        try
        {
            return new Money(decimal.Add(money1.Amount, money2.Amount), money1.Currency);
        }
        catch (OverflowException ex) when (ex.Message == "Value was either too large or too small for a Decimal.")
        {
            throw new OverflowException("Value was either too large or too small for a Money.", ex);
        }
    }

    /// <summary>Adds two specified <see cref="Money"/> values.</summary>
    /// <param name="money1">The first <see cref="Money"/> object.</param>
    /// <param name="money2">The second <see cref="decimal"/> object.</param>
    /// <returns>A <see cref="Money"/> object with the values of both <see cref="decimal"/> objects added.</returns>
    public static Money Add(in Money money1, in decimal money2)
    {
        if (money2 == decimal.Zero) return money1;
        if (money1.Amount == decimal.Zero) return new Money(money2, money1.Currency);
        return new Money(decimal.Add(money1.Amount, money2), money1.Currency);
    }

    /// <summary>Subtracts one specified <see cref="Money"/> value from another.</summary>
    /// <param name="money1">The first <see cref="Money"/> object.</param>
    /// <param name="money2">The second <see cref="Money"/> object.</param>
    /// <returns>A <see cref="Money"/> object where the second <see cref="Money"/> object is subtracted from the first.</returns>
    public static Money Subtract(in Money money1, in Money money2)
    {
        // If one of the amounts is zero, then no need to check currency: Just return input value.
        if (money1.Amount == 0m)
            return -money2;
        if (money2.Amount == 0m)
            return money1;

        EnsureSameCurrency(money1, money2);
        try
        {
            return new Money(decimal.Subtract(money1.Amount, money2.Amount), money1.Currency);
        }
        catch (OverflowException ex) when (ex.Message == "Value was either too large or too small for a Decimal.")
        {
            throw new OverflowException("Value was either too large or too small for a Money.", ex);
        }
    }

    /// <summary>Subtracts one specified <see cref="Money"/> value from another.</summary>
    /// <param name="money1">The first <see cref="Money"/> object.</param>
    /// <param name="money2">The second <see cref="decimal"/> object.</param>
    /// <returns>A <see cref="Money"/> object where the second <see cref="decimal"/> object is subtracted from the first.</returns>
    public static Money Subtract(in Money money1, in decimal money2)
    {
        if (money2 == decimal.Zero) return money1;
        if (money1.Amount == decimal.Zero) return new Money(-money2, money1.Currency);
        return new Money(decimal.Subtract(money1.Amount, money2), money1.Currency);
    }

    /// <summary>Multiplies the specified money.</summary>
    /// <param name="money">The money.</param>
    /// <param name="multiplier">The multiplier.</param>
    /// <returns>The result as <see cref="Money"/> after multiplying.</returns>
    public static Money Multiply(in Money money, in decimal multiplier)
    {
        if (multiplier == MultiplicativeIdentity) return money;
        try
        {
            return new Money(decimal.Multiply(money.Amount, multiplier), money.Currency);
        }
        catch (OverflowException ex) when (ex.Message == "Value was either too large or too small for a Decimal.")
        {
            throw new OverflowException("Value was either too large or too small for a Money.", ex);
        }
    }

    /// <summary>Divides the specified money.</summary>
    /// <param name="money">The money.</param>
    /// <param name="divisor">The divider.</param>
    /// <returns>The division as <see cref="Money"/>.</returns>
    /// <remarks>This division can lose money! Use <see cref="MoneyExtensions.Split"/> to do a safe division.</remarks>
    public static Money Divide(in Money money, in decimal divisor)
    {
        if (divisor == MultiplicativeIdentity) return money;
        return new Money(decimal.Divide(money.Amount, divisor), money.Currency);
    }

    /// <summary>Divides the specified money.</summary>
    /// <param name="money1">The money.</param>
    /// <param name="money2">The divider.</param>
    /// <returns>The <see cref="decimal"/> result of dividing left with right.</returns>
    /// <remarks>Division of Money by Money, means the unit is lost, so the result will be Decimal.</remarks>
    public static decimal Divide(in Money money1, in Money money2)
    {
        EnsureSameCurrency(money1, money2);
        return decimal.Divide(money1.Amount, money2.Amount);
    }

    /// <summary>Computes the <see cref="Money"/> remainder after dividing two <see cref="Money"/> values.</summary>
    /// <param name="money1">The <see cref="Money"/> dividend.</param>
    /// <param name="money2">The <see cref="Money"/> divisor.</param>
    /// <returns>The <see cref="Money"/> remainder after dividing <see cref="money1"/> by <see cref="money2"/>.</returns>
    public static Money Remainder(in Money money1, in Money money2)
    {
        EnsureSameCurrency(money1, money2);
        return new Money(decimal.Remainder(money1.Amount, money2.Amount), money1.Currency);
    }
}
