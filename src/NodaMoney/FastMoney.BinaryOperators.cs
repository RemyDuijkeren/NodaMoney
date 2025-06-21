using System.Numerics;

namespace NodaMoney;

internal readonly partial record struct FastMoney
#if NET7_0_OR_GREATER
    : IMinMaxValue<FastMoney>,
        IMultiplicativeIdentity<FastMoney, decimal>,
        IAdditiveIdentity<FastMoney, FastMoney>,
        IAdditionOperators<FastMoney, FastMoney, FastMoney>,
        IAdditionOperators<FastMoney, decimal, FastMoney>,
        ISubtractionOperators<FastMoney, FastMoney, FastMoney>,
        ISubtractionOperators<FastMoney, decimal, FastMoney>,
        IMultiplyOperators<FastMoney, decimal, FastMoney>,
        IDivisionOperators<FastMoney, decimal, FastMoney>,
        IMultiplyOperators<FastMoney, long, FastMoney>,
        IDivisionOperators<FastMoney, long, FastMoney>,
        IDivisionOperators<FastMoney, FastMoney, decimal>,
        IModulusOperators<FastMoney, FastMoney, FastMoney>
#endif
{
    /// <inheritdoc cref="IMinMaxValue{TSelf}.MinValue" />
    public static FastMoney MinValue { get; } = new(MinValueLong, Currency.NoCurrency);

    /// <inheritdoc cref="IMinMaxValue{TSelf}.MaxValue" />
    public static FastMoney MaxValue { get; } = new(MaxValueLong, Currency.NoCurrency);

    /// <inheritdoc cref="IMultiplicativeIdentity{TSelf, TResult}.MultiplicativeIdentity" />
    public static decimal MultiplicativeIdentity => decimal.One;

    /// <inheritdoc cref="IMultiplicativeIdentity{TSelf, TResult}.MultiplicativeIdentity" />
    private static long MultiplicativeIdentityLong => 1L;

    /// <inheritdoc cref="IAdditiveIdentity{TSelf, TResult}.AdditiveIdentity" />
    public static FastMoney AdditiveIdentity => new(0L, Currency.NoCurrency);

    /// <summary>Adds two specified <see cref="FastMoney"/> values.</summary>
    /// <param name="left">A <see cref="FastMoney"/> object on the left side.</param>
    /// <param name="right">A <see cref="FastMoney"/> object on the right side.</param>
    /// <returns>The <see cref="FastMoney"/> result of adding left and right.</returns>
    public static FastMoney operator +(FastMoney left, FastMoney right) => Add(left, right);

    /// <summary>Add the <see cref="FastMoney"/> value with the given value.</summary>
    /// <param name="left">A <see cref="FastMoney"/> object on the left side.</param>
    /// <param name="right">A <see cref="decimal"/> object on the right side.</param>
    /// <returns>The <see cref="FastMoney"/> result of adding left and right.</returns>
    public static FastMoney operator +(FastMoney left, decimal right) => Add(left, right);

    /// <summary>Add the <see cref="FastMoney"/> value with the given value.</summary>
    /// <param name="left">A <see cref="decimal"/> object on the left side.</param>
    /// <param name="right">A <see cref="Money"/> object on the right side.</param>
    /// <returns>The <see cref="FastMoney"/> result of adding left and right.</returns>
    public static FastMoney operator +(decimal left, FastMoney right) => Add(right, left);

    /// <summary>Subtracts two specified <see cref="FastMoney"/> values.</summary>
    /// <param name="left">A <see cref="FastMoney"/> object on the left side.</param>
    /// <param name="right">A <see cref="FastMoney"/> object on the right side.</param>
    /// <returns>The <see cref="FastMoney"/> result of subtracting right from the left.</returns>
    public static FastMoney operator -(FastMoney left, FastMoney right) => Subtract(left, right);

    /// <summary>Subtracts <see cref="FastMoney"/> value with the given value.</summary>
    /// <param name="left">A <see cref="FastMoney"/> object on the left side.</param>
    /// <param name="right">A <see cref="decimal"/> object on the right side.</param>
    /// <returns>The <see cref="FastMoney"/> result of subtracting right from the left.</returns>
    public static FastMoney operator -(FastMoney left, decimal right) => Subtract(left, right);

    /// <summary>Subtracts <see cref="FastMoney"/> value with the given value.</summary>
    /// <param name="left">A <see cref="decimal"/> object on the left side.</param>
    /// <param name="right">A <see cref="FastMoney"/> object on the right side.</param>
    /// <returns>The <see cref="FastMoney"/> result of subtracting right from the left.</returns>
    public static FastMoney operator -(decimal left, FastMoney right) => Subtract(right, left);

    /// <summary>Multiplies the <see cref="FastMoney"/> value by the given value.</summary>
    /// <param name="left">A <see cref="FastMoney"/> object on the left side.</param>
    /// <param name="right">A <see cref="decimal"/> object on the right side.</param>
    /// <returns>The <see cref="FastMoney"/> result of multiplying right with the left.</returns>
    public static FastMoney operator *(FastMoney left, decimal right) => Multiply(left, right);

    /// <summary>Multiplies the <see cref="FastMoney"/> value by the given value.</summary>
    /// <param name="left">A <see cref="decimal"/> object on the left side.</param>
    /// <param name="right">A <see cref="FastMoney"/> object on the right side.</param>
    /// <returns>The <see cref="FastMoney"/> result of multiplying left with right.</returns>
    public static FastMoney operator *(decimal left, FastMoney right) => Multiply(right, left);

    /// <summary>Multiplies the specified <see cref="FastMoney"/> value by a specified <see cref="long"/> value.</summary>
    /// <param name="left">The <see cref="FastMoney"/> value to be multiplied.</param>
    /// <param name="right">The <see cref="long"/> value to multiply by.</param>
    /// <returns>The <see cref="FastMoney"/> result of the multiplication.</returns>
    public static FastMoney operator *(FastMoney left, long right) => Multiply(left, right);

    /// <summary>Multiplies a <see cref="FastMoney"/> value by a specified <see cref="long"/> value.</summary>
    /// <param name="left">The <see cref="long"/> value representing the multiplier.</param>
    /// <param name="right">The <see cref="FastMoney"/> value to be multiplied.</param>
    /// <returns>The <see cref="FastMoney"/> result of the multiplication.</returns>
    public static FastMoney operator *(long left, FastMoney right) => Multiply(right, left);

    /// <summary>Divides the <see cref="FastMoney"/> value by the given value.</summary>
    /// <param name="left">A <see cref="FastMoney"/> object on the left side.</param>
    /// <param name="right">A <see cref="decimal"/> object on the right side.</param>
    /// <returns>The <see cref="FastMoney"/> result of dividing left with right.</returns>
    /// <remarks>This division can lose money! Use <see cref="MoneyExtensions.Split(Money, int)"/> to do a safe division.</remarks>
    public static FastMoney operator /(FastMoney left, decimal right) => Divide(left, right);

    /// <summary>Divides the <see cref="FastMoney"/> value by the given value.</summary>
    /// <param name="left">A <see cref="FastMoney"/> object on the left side.</param>
    /// <param name="right">A <see cref="FastMoney"/> object on the right side.</param>
    /// <returns>The <see cref="decimal"/> result of dividing left with right.</returns>
    /// <remarks>Division of Money by Money, means the unit is lost, so the result will be a ratio <see cref="decimal"/>.</remarks>
    public static decimal operator /(FastMoney left, FastMoney right) => Divide(left, right);

    /// <summary>Divides a specified <see cref="FastMoney"/> value by a specified <see cref="long"/> value.</summary>
    /// <param name="left">The <see cref="FastMoney"/> instance to be divided.</param>
    /// <param name="right">The <see cref="long"/> value by which to divide the left operand.</param>
    /// <returns>The <see cref="FastMoney"/> result of dividing the left operand by the right operand.</returns>
    public static FastMoney operator /(FastMoney left, long right) => Divide(left, right);

    /// <summary>Divides the specified <see cref="FastMoney"/> value by a long integer.</summary>
    /// <param name="left">A long integer divisor.</param>
    /// <param name="right">A <see cref="FastMoney"/> object representing the dividend.</param>
    /// <returns>The <see cref="FastMoney"/> result of the division.</returns>
    public static FastMoney operator /(long left, FastMoney right) => Divide(right, left);

    /// <summary>Divides two <see cref="FastMoney"/> values togheter to compute their modulus or remainder.</summary>
    /// <param name="left">The <see cref="FastMoney"/> value which rights divides.</param>
    /// <param name="right">The <see cref="FastMoney"/> value which divides left.</param>
    /// <returns>The <see cref="FastMoney"/> modulus or remainder of <see cref="left"/> divide by <see cref="right"/>.</returns>
    public static FastMoney operator %(FastMoney left, FastMoney right) => Remainder(left, right);

    /// <summary>Adds two specified <see cref="FastMoney"/> values.</summary>
    /// <param name="money1">The first <see cref="FastMoney"/> object.</param>
    /// <param name="money2">The second <see cref="FastMoney"/> object.</param>
    /// <returns>A <see cref="FastMoney"/> object with the values of both <see cref="FastMoney"/> objects added.</returns>
    public static FastMoney Add(in FastMoney money1, in FastMoney money2)
    {
        EnsureSameContext(money1, money2);
        if (money1.Context.EnforceZeroCurrencyMatching)
        {
            EnsureSameCurrency(money1, money2);

            // If one of the amounts is zero, then return fast
            if (money1.OACurrencyAmount == 0L)
                return money2;
            if (money2.OACurrencyAmount == 0L)
                return money1;
        }
        else
        {
            // If one of the amounts is zero, then return fast
            if (money1.OACurrencyAmount == 0L)
                return money2;
            if (money2.OACurrencyAmount == 0L)
                return money1;

            EnsureSameCurrency(money1, money2);
        }

        try
        {
            long totalAmount = checked(money1.OACurrencyAmount + money2.OACurrencyAmount); // Use checked for overflow
            return money1 with { OACurrencyAmount = totalAmount };
        }
        catch (OverflowException ex) when (ex.Message == "Value was either too large or too small for a Decimal.")
        {
            throw new OverflowException("Value was either too large or too small for a FastMoney.", ex);
        }
    }

    /// <summary>Adds two specified <see cref="FastMoney"/> values.</summary>
    /// <param name="money1">The first <see cref="FastMoney"/> object.</param>
    /// <param name="decimal2">The second <see cref="decimal"/> object.</param>
    /// <returns>A <see cref="FastMoney"/> object with the values of both <see cref="decimal"/> objects added.</returns>
    public static FastMoney Add(in FastMoney money1, in decimal decimal2)
    {
        if (decimal2 == decimal.Zero) return money1;
        if (money1.OACurrencyAmount == 0L) return money1 with { OACurrencyAmount = decimal.ToOACurrency(decimal2) };

        try
        {
            long totalAmount = checked(money1.OACurrencyAmount + decimal.ToOACurrency(decimal2)); // Use checked for overflow
            return money1 with { OACurrencyAmount = totalAmount };
        }
        catch (OverflowException ex) when (ex.Message == "Value was either too large or too small for a Decimal.")
        {
            throw new OverflowException("Value was either too large or too small for a FastMoney.", ex);
        }
    }

    /// <summary>Subtracts one specified <see cref="FastMoney"/> value from another.</summary>
    /// <param name="money1">The first <see cref="FastMoney"/> object.</param>
    /// <param name="money2">The second <see cref="FastMoney"/> object.</param>
    /// <returns>A <see cref="FastMoney"/> object where the second <see cref="FastMoney"/> object is subtracted from the first.</returns>
    public static FastMoney Subtract(in FastMoney money1, in FastMoney money2)
    {
        EnsureSameContext(money1, money2);

        if (money1.Context.EnforceZeroCurrencyMatching)
        {
            EnsureSameCurrency(money1, money2);

            // If one of the amounts is zero, then return fast
            if (money1.OACurrencyAmount == 0L)
                return -money2;
            if (money2.OACurrencyAmount == 0L)
                return money1;
        }
        else
        {
            // If one of the amounts is zero, then return fast
            if (money1.OACurrencyAmount == 0L)
                return -money2;
            if (money2.OACurrencyAmount == 0L)
                return money1;

            EnsureSameCurrency(money1, money2);
        }

        try
        {
            long totalAmount = checked(money1.OACurrencyAmount - money2.OACurrencyAmount); // Use checked for overflow
            return money1 with { OACurrencyAmount = totalAmount };
        }
        catch (OverflowException ex) when (ex.Message == "Value was either too large or too small for a Decimal.")
        {
            throw new OverflowException("Value was either too large or too small for a FastMoney.", ex);
        }
    }

    /// <summary>Subtracts one specified <see cref="FastMoney"/> value from another.</summary>
    /// <param name="money1">The first <see cref="FastMoney"/> object.</param>
    /// <param name="decimal2">The second <see cref="decimal"/> object.</param>
    /// <returns>A <see cref="FastMoney"/> object where the second <see cref="decimal"/> object is subtracted from the first.</returns>
    public static FastMoney Subtract(in FastMoney money1, in decimal decimal2)
    {
        if (decimal2 == decimal.Zero) return money1;
        if (money1.OACurrencyAmount == 0L) return money1 with { OACurrencyAmount = -decimal.ToOACurrency(decimal2) };

        try
        {
            long totalAmount = checked(money1.OACurrencyAmount - decimal.ToOACurrency(decimal2)); // Use checked for overflow
            return money1 with { OACurrencyAmount = totalAmount };
        }
        catch (OverflowException ex) when (ex.Message == "Value was either too large or too small for a Decimal.")
        {
            throw new OverflowException("Value was either too large or too small for a FastMoney.", ex);
        }
    }

    /// <summary>Multiplies the specified money.</summary>
    /// <param name="money">The money.</param>
    /// <param name="multiplier">The multiplier.</param>
    /// <returns>The result as <see cref="FastMoney"/> after multiplying.</returns>
    public static FastMoney Multiply(in FastMoney money, in decimal multiplier)
    {
        if (multiplier == MultiplicativeIdentity) return money;
        if (multiplier == 0m) return money with { OACurrencyAmount = 0 };

        try
        {
#if NET7_0_OR_GREATER
            if (decimal.IsInteger(multiplier))
#else
            if (multiplier == (long)multiplier)
#endif
            {
                // Direct cast from decimal * long to long is more efficient than first multiplying as decimal and then casting
                long totalAmount1 = checked(money.OACurrencyAmount * (long)multiplier);
                // TODO: cast could throw OverFlowExpection if outside min-max values of long!
                return money with { OACurrencyAmount = totalAmount1 };
            }

            // For non-integer multipliers, fall back to decimal multiplication
            //long totalAmount = checked((long)(money.OACurrencyAmount * multiplier));
            decimal totalAmount = decimal.Multiply(money.Amount, multiplier);
            return money with { OACurrencyAmount = decimal.ToOACurrency(totalAmount) }; // ToEven rounding
        }
        catch (OverflowException ex)
        {
            throw new OverflowException("Value was either too large or too small for a FastMoney.", ex);
        }
    }

    /// <summary>Multiplies the specified <see cref="FastMoney"/> value by the given multiplier.</summary>
    /// <param name="money">The <see cref="FastMoney"/> instance to be multiplied.</param>
    /// <param name="multiplier">The multiplier used in the operation.</param>
    /// <returns>A new <see cref="FastMoney"/> instance with the resulting value.</returns>
    /// <exception cref="OverflowException">Thrown when the operation results in an overflow.</exception>
    public static FastMoney Multiply(in FastMoney money, in long multiplier)
    {
        if (multiplier == MultiplicativeIdentityLong) return money;
        if (multiplier == 0L) return money with { OACurrencyAmount = 0 };

        try
        {
            long totalAmount = checked(money.OACurrencyAmount * multiplier);
            return money with { OACurrencyAmount = totalAmount };
        }
        catch (OverflowException ex)
        {
            throw new OverflowException("Value was either too large or too small for a FastMoney.", ex);
        }
    }

    /// <summary>Divides the specified money.</summary>
    /// <param name="money">The money.</param>
    /// <param name="divisor">The divider.</param>
    /// <returns>The division as <see cref="FastMoney"/>.</returns>
    /// <remarks>This division can lose money! Use <see cref="MoneyExtensions.Split(Money,int)"/> to do a safe division.</remarks>
    public static FastMoney Divide(in FastMoney money, in decimal divisor)
    {
        if (divisor == MultiplicativeIdentity) return money;

        try
        {
#if NET7_0_OR_GREATER
            if (decimal.IsInteger(divisor))
#else
            if (divisor == (long)divisor)
#endif
            {
                // Direct cast from decimal * long to long is more efficient than first dividing as decimal and then casting
                long totalAmount1 = checked(money.OACurrencyAmount / (long)divisor);
                return money with { OACurrencyAmount = totalAmount1 };
            }

            // For non-integer multipliers, fall back to decimal multiplication
            decimal totalAmount = decimal.Divide(money.Amount, divisor);
            return money with { OACurrencyAmount = decimal.ToOACurrency(totalAmount) };
        }
        catch (OverflowException ex)
        {
            throw new OverflowException("Value was either too large or too small for a FastMoney.", ex);
        }
    }

    /// <summary>Divides a specified <see cref="FastMoney"/> value by a given divisor.</summary>
    /// <param name="money">The <see cref="FastMoney"/> value to be divided.</param>
    /// <param name="divisor">The divisor by which the <see cref="FastMoney"/> value is divided.</param>
    /// <returns>A new <see cref="FastMoney"/> value representing the result of the division.</returns>
    /// <exception cref="OverflowException">Thrown when the result of the division exceeds the limits of <see cref="FastMoney"/>.</exception>
    public static FastMoney Divide(in FastMoney money, in long divisor)
    {
        if (divisor == MultiplicativeIdentityLong) return money;

        try
        {
            long totalAmount = checked(money.OACurrencyAmount / divisor);
            return money with { OACurrencyAmount = totalAmount };
        }
        catch (OverflowException ex)
        {
            throw new OverflowException("Value was either too large or too small for a FastMoney.", ex);
        }
    }

    /// <summary>Divides the specified money.</summary>
    /// <param name="money1">The money.</param>
    /// <param name="money2">The divider.</param>
    /// <returns>The <see cref="decimal"/> result of dividing left with right.</returns>
    /// <remarks>Division of Money by Money means the unit is lost, so the result will be Decimal.</remarks>
    public static decimal Divide(in FastMoney money1, in FastMoney money2)
    {
        EnsureSameContext(money1, money2);
        EnsureSameCurrency(money1, money2);
        return decimal.Divide(money1.Amount, money2.Amount);
    }

    /// <summary>Computes the <see cref="FastMoney"/> remainder after dividing two <see cref="Money"/> values.</summary>
    /// <param name="money1">The <see cref="FastMoney"/> dividend.</param>
    /// <param name="money2">The <see cref="FastMoney"/> divisor.</param>
    /// <returns>The <see cref="FastMoney"/> remainder after dividing <see cref="money1"/> by <see cref="money2"/>.</returns>
    public static FastMoney Remainder(in FastMoney money1, in FastMoney money2)
    {
        EnsureSameContext(money1, money2);
        EnsureSameCurrency(money1, money2);
        decimal remainder = decimal.Remainder(money1.Amount, money2.Amount);
        return money1 with { OACurrencyAmount = decimal.ToOACurrency(remainder) };
    }
}
