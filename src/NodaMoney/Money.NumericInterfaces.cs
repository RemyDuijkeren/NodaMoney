using System.Numerics;
using System.Runtime.CompilerServices;

namespace NodaMoney;

public partial struct Money
#if NET7_0_OR_GREATER
    : IMinMaxValue<Money>, IMultiplicativeIdentity<Money, decimal>, IAdditiveIdentity<Money, Money>
    // Decimal implements IFloatingPoint, INumber, INumberBase, ISignedNumber, IFloatingPointConstants (E, Pi, Tua)
    //,INumber<Money>, INumberBase<Money>, ISignedNumber<Money> ,IDivisionOperators<Money, Money, Money>, IMultiplicativeIdentity<Money, Money>
#endif
{
    /// <inheritdoc cref="IMinMaxValue{TSelf}.MinValue" />
    public static Money MinValue { get; } = new(decimal.MinValue, Currency.NoCurrency);

    /// <inheritdoc cref="IMinMaxValue{TSelf}.MaxValue" />
    public static Money MaxValue { get; } = new(decimal.MaxValue, Currency.NoCurrency);

    /// <inheritdoc cref="IMultiplicativeIdentity{TSelf, TResult}.MultiplicativeIdentity" />
    public static decimal MultiplicativeIdentity => decimal.One;

    /// <inheritdoc cref="IAdditiveIdentity{TSelf, TResult}.AdditiveIdentity" />
    public static Money AdditiveIdentity => new(decimal.Zero, Currency.NoCurrency);

    /// <inheritdoc cref="INumberBase{TSelf}.Radix" />
    public static int Radix => 10;

    /// <inheritdoc cref="INumberBase{TSelf}.Zero" />
    public static Money Zero => new(0m, Currency.NoCurrency);

    /// <inheritdoc cref="INumberBase{TSelf}.One" />
    public static Money One => new(1m, Currency.NoCurrency);

    /// <inheritdoc cref="ISignedNumber{TSelf}.NegativeOne" />
    public static Money NegativeOne => new(-1m, Currency.NoCurrency);

    /// <inheritdoc cref="INumberBase{TSelf}.Abs(TSelf)" />
    public static Money Abs(Money value) => new(Math.Abs(value.Amount), value.Currency);

    /// <inheritdoc cref="INumberBase{TSelf}.IsNegative(TSelf)" />
    public static bool IsNegative(Money value) => value.Amount < 0;

    /// <inheritdoc cref="INumberBase{TSelf}.IsPositive(TSelf)" />
    public static bool IsPositive(Money value) => value.Amount >= 0;

    /// <inheritdoc cref="INumberBase{TSelf}.IsZero(TSelf)" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsZero(in Money value) => (value._low | value._mid | value._high) == 0;
    //public static bool IsZero(Money value) => (value._low | value._mid | value._high) == 0;

    /// <inheritdoc cref="INumberBase{TSelf}.MinMagnitude(TSelf, TSelf)" />
    public static Money MinMagnitude(Money x, Money y)
    {
        Money ax = Abs(x);
        Money ay = Abs(y);

        if (ax < ay)
        {
            return x;
        }

        if (ax == ay)
        {
            return IsNegative(x) ? x : y;
        }

        return y;
    }

    /// <inheritdoc cref="INumberBase{TSelf}.MaxMagnitude(TSelf, TSelf)" />
    public static Money MaxMagnitude(Money x, Money y)
    {
        Money ax = Abs(x);
        Money ay = Abs(y);

        if (ax > ay)
        {
            return x;
        }

        if (ax == ay)
        {
            return IsNegative(x) ? y : x;
        }

        return y;
    }

// #if NET7_0_OR_GREATER
//
// #region INumberBase<Money> but doesn't make sense
//
//     static Money IDivisionOperators<Money, Money, Money>.operator /(Money left, Money right) => throw new NotImplementedException();
//
//     static Money IMultiplicativeIdentity<Money, Money>.MultiplicativeIdentity => throw new NotImplementedException();
//
//     public static Money operator *(Money left, Money right) => throw new NotImplementedException();
//
// #endregion
//
// #region INumberBase<Money>
//
//     /// <inheritdoc cref="INumberBase{TSelf}.IsCanonical(TSelf)" />
//     public static bool IsCanonical(Money value) => decimal.IsCanonical(value.Amount);
//
//     /// <inheritdoc cref="INumberBase{TSelf}.IsComplexNumber(TSelf)" />
//     static bool INumberBase<Money>.IsComplexNumber(Money value) => false;
//
//     /// <inheritdoc cref="INumberBase{TSelf}.IsEvenInteger(TSelf)" />
//     public static bool IsEvenInteger(Money value) => decimal.IsEvenInteger(value.Amount);
//
//     /// <inheritdoc cref="INumberBase{TSelf}.IsFinite(TSelf)" />
//     static bool INumberBase<Money>.IsFinite(Money value) => true;
//
//     /// <inheritdoc cref="INumberBase{TSelf}.IsImaginaryNumber(TSelf)" />
//     static bool INumberBase<Money>.IsImaginaryNumber(Money value) => false;
//
//     /// <inheritdoc cref="INumberBase{TSelf}.IsInfinity(TSelf)" />
//     static bool INumberBase<Money>.IsInfinity(Money value) => false;
//
//     /// <inheritdoc cref="INumberBase{TSelf}.IsInteger(TSelf)" />
//     public static bool IsInteger(Money value) => value.Amount == decimal.Truncate(value.Amount);
//
//     /// <inheritdoc cref="INumberBase{TSelf}.IsInteger(TSelf)" />
//     static bool INumberBase<Money>.IsNaN(Money value) => false;
//
//     /// <inheritdoc cref="INumberBase{TSelf}.IsNegativeInfinity(TSelf)" />
//     static bool INumberBase<Money>.IsNegativeInfinity(Money value) => false;
//
//     /// <inheritdoc cref="INumberBase{TSelf}.IsNormal(TSelf)" />
//     static bool INumberBase<Money>.IsNormal(Money value) => value.Amount != 0;
//
//     /// <inheritdoc cref="INumberBase{TSelf}.IsOddInteger(TSelf)" />
//     public static bool IsOddInteger(Money value) => decimal.IsOddInteger(value.Amount);
//
//     /// <inheritdoc cref="INumberBase{TSelf}.IsPositiveInfinity(TSelf)" />
//     static bool INumberBase<Money>.IsPositiveInfinity(Money value) => false;
//
//     /// <inheritdoc cref="INumberBase{TSelf}.IsRealNumber(TSelf)" />
//     static bool INumberBase<Money>.IsRealNumber(Money value) => true;
//
//     /// <inheritdoc cref="INumberBase{TSelf}.IsSubnormal(TSelf)" />
//     static bool INumberBase<Money>.IsSubnormal(Money value) => false;
//
//     /// <inheritdoc cref="INumberBase{TSelf}.MaxMagnitudeNumber(TSelf, TSelf)" />
//     static Money INumberBase<Money>.MaxMagnitudeNumber(Money x, Money y) => MaxMagnitude(x, y);
//
//     /// <inheritdoc cref="INumberBase{TSelf}.MinMagnitudeNumber(TSelf, TSelf)" />
//     static Money INumberBase<Money>.MinMagnitudeNumber(Money x, Money y) => MinMagnitude(x, y);
//
//     /// <inheritdoc cref="INumberBase{TSelf}.TryConvertFromChecked{TOther}(TOther, out TSelf)" />
//     //[MethodImpl(MethodImplOptions.AggressiveInlining)]
//     static bool INumberBase<Money>.TryConvertFromChecked<TOther>(TOther value, out Money result)
//     {
//         // Copied implementation of Decimal
//         if (typeof(TOther) == typeof(byte))
//         {
//             byte actualValue = (byte)(object)value;
//             result = new Money(actualValue);
//             return true;
//         }
//         else if (typeof(TOther) == typeof(char))
//         {
//             char actualValue = (char)(object)value;
//             result = new Money(actualValue);
//             return true;
//         }
//         else if (typeof(TOther) == typeof(ushort))
//         {
//             ushort actualValue = (ushort)(object)value;
//             result = new Money(actualValue);
//             return true;
//         }
//         else if (typeof(TOther) == typeof(uint))
//         {
//             uint actualValue = (uint)(object)value;
//             result = new Money(actualValue);
//             return true;
//         }
//         else if (typeof(TOther) == typeof(ulong))
//         {
//             ulong actualValue = (ulong)(object)value;
//             result = new Money(actualValue);
//             return true;
//         }
//         else if (typeof(TOther) == typeof(UInt128))
//         {
//             UInt128 actualValue = (UInt128)(object)value;
//             result = new Money(checked((decimal)actualValue));
//             return true;
//         }
//         else if (typeof(TOther) == typeof(nuint))
//         {
//             nuint actualValue = (nuint)(object)value;
//             result = new Money(actualValue);
//             return true;
//         }
//         else
//         {
//             result = default;
//             return false;
//         }
//     }
//
//     /// <inheritdoc cref="INumberBase{TSelf}.TryConvertFromSaturating{TOther}(TOther, out TSelf)" />
//     //[MethodImpl(MethodImplOptions.AggressiveInlining)]
//     static bool INumberBase<Money>.TryConvertFromSaturating<TOther>(TOther value, out Money result) => TryConvertFrom(value, out result);
//
//     /// <inheritdoc cref="INumberBase{TSelf}.TryConvertFromTruncating{TOther}(TOther, out TSelf)" />
//     //[MethodImpl(MethodImplOptions.AggressiveInlining)]
//     static bool INumberBase<Money>.TryConvertFromTruncating<TOther>(TOther value, out Money result) => TryConvertFrom(value, out result);
//
//     //[MethodImpl(MethodImplOptions.AggressiveInlining)]
//     private static bool TryConvertFrom<TOther>(TOther value, out Money result) where TOther : INumberBase<TOther>
//     {
//         if (typeof(TOther) == typeof(byte))
//         {
//             byte actualValue = (byte)(object)value;
//             result = new Money(actualValue);
//             return true;
//         }
//         else if (typeof(TOther) == typeof(char))
//         {
//             char actualValue = (char)(object)value;
//             result = new Money(actualValue);
//             return true;
//         }
//         else if (typeof(TOther) == typeof(ushort))
//         {
//             ushort actualValue = (ushort)(object)value;
//             result = new Money(actualValue);
//             return true;
//         }
//         else if (typeof(TOther) == typeof(uint))
//         {
//             uint actualValue = (uint)(object)value;
//             result = new Money(actualValue);
//             return true;
//         }
//         else if (typeof(TOther) == typeof(ulong))
//         {
//             ulong actualValue = (ulong)(object)value;
//             result = new Money(actualValue);
//             return true;
//         }
//         else if (typeof(TOther) == typeof(UInt128))
//         {
//             UInt128 actualValue = (UInt128)(object)value;
//             decimal actualValue1 = (actualValue >= new UInt128(0x0000_0000_FFFF_FFFF, 0xFFFF_FFFF_FFFF_FFFF)) ? decimal.MaxValue : (decimal)actualValue;
//             result = new Money(actualValue1);
//             return true;
//         }
//         else if (typeof(TOther) == typeof(nuint))
//         {
//             nuint actualValue = (nuint)(object)value;
//             result = new Money(actualValue);
//             return true;
//         }
//         else
//         {
//             result = default;
//             return false;
//         }
//     }
//
//     /// <inheritdoc cref="INumberBase{TSelf}.TryConvertToChecked{TOther}(TSelf, out TOther)" />
//     //[MethodImpl(MethodImplOptions.AggressiveInlining)]
//     static bool INumberBase<Money>.TryConvertToChecked<TOther>(Money value, [MaybeNullWhen(false)] out TOther result)
//     {
//         // Copied implementation of Decimal
//         if (typeof(TOther) == typeof(double))
//         {
//             double actualResult = checked((double)value);
//             result = (TOther)(object)actualResult;
//             return true;
//         }
//         else if (typeof(TOther) == typeof(Half))
//         {
//             Half actualResult = checked((Half)value.Amount);
//             result = (TOther)(object)actualResult;
//             return true;
//         }
//         else if (typeof(TOther) == typeof(short))
//         {
//             short actualResult = checked((short)value);
//             result = (TOther)(object)actualResult;
//             return true;
//         }
//         else if (typeof(TOther) == typeof(int))
//         {
//             int actualResult = checked((int)value);
//             result = (TOther)(object)actualResult;
//             return true;
//         }
//         else if (typeof(TOther) == typeof(long))
//         {
//             long actualResult = checked((long)value);
//             result = (TOther)(object)actualResult;
//             return true;
//         }
//         else if (typeof(TOther) == typeof(Int128))
//         {
//             Int128 actualResult = checked((Int128)value.Amount);
//             result = (TOther)(object)actualResult;
//             return true;
//         }
//         else if (typeof(TOther) == typeof(nint))
//         {
//             nint actualResult = checked((nint)value);
//             result = (TOther)(object)actualResult;
//             return true;
//         }
//         else if (typeof(TOther) == typeof(sbyte))
//         {
//             sbyte actualResult = checked((sbyte)value);
//             result = (TOther)(object)actualResult;
//             return true;
//         }
//         else if (typeof(TOther) == typeof(float))
//         {
//             float actualResult = checked((float)value);
//             result = (TOther)(object)actualResult;
//             return true;
//         }
//         else
//         {
//             result = default;
//             return false;
//         }
//     }
//
//     /// <inheritdoc cref="INumberBase{TSelf}.TryConvertToSaturating{TOther}(TSelf, out TOther)" />
//     //[MethodImpl(MethodImplOptions.AggressiveInlining)]
//     static bool INumberBase<Money>.TryConvertToSaturating<TOther>(Money value, [MaybeNullWhen(false)] out TOther result) =>
//         TryConvertTo(value, out result);
//
//     /// <inheritdoc cref="INumberBase{TSelf}.TryConvertToTruncating{TOther}(TSelf, out TOther)" />
//     //[MethodImpl(MethodImplOptions.AggressiveInlining)]
//     static bool INumberBase<Money>.TryConvertToTruncating<TOther>(Money value, [MaybeNullWhen(false)] out TOther result) =>
//         TryConvertTo(value, out result);
//
//     //[MethodImpl(MethodImplOptions.AggressiveInlining)]
//     private static bool TryConvertTo<TOther>(Money value, [MaybeNullWhen(false)] out TOther result)
//         where TOther : INumberBase<TOther>
//     {
//         // Copied implementation from Decimal
//         if (typeof(TOther) == typeof(double))
//         {
//             double actualResult = (double)value;
//             result = (TOther)(object)actualResult;
//             return true;
//         }
//         else if (typeof(TOther) == typeof(Half))
//         {
//             Half actualResult = (Half)value.Amount;
//             result = (TOther)(object)actualResult;
//             return true;
//         }
//         else if (typeof(TOther) == typeof(short))
//         {
//             short actualResult = (value.Amount >= short.MaxValue) ? short.MaxValue :
//                 (value.Amount <= short.MinValue) ? short.MinValue : (short)value;
//             result = (TOther)(object)actualResult;
//             return true;
//         }
//         else if (typeof(TOther) == typeof(int))
//         {
//             int actualResult = (value.Amount >= int.MaxValue) ? int.MaxValue :
//                 (value.Amount <= int.MinValue) ? int.MinValue : (int)value;
//             result = (TOther)(object)actualResult;
//             return true;
//         }
//         else if (typeof(TOther) == typeof(long))
//         {
//             long actualResult = (value.Amount >= long.MaxValue) ? long.MaxValue :
//                 (value.Amount <= long.MinValue) ? long.MinValue : (long)value;
//             result = (TOther)(object)actualResult;
//             return true;
//         }
//         else if (typeof(TOther) == typeof(Int128))
//         {
//             Int128 actualResult = (Int128)value.Amount;
//             result = (TOther)(object)actualResult;
//             return true;
//         }
//         else if (typeof(TOther) == typeof(nint))
//         {
//             nint actualResult = (value.Amount >= nint.MaxValue) ? nint.MaxValue :
//                 (value.Amount <= nint.MinValue) ? nint.MinValue : (nint)value;
//             result = (TOther)(object)actualResult;
//             return true;
//         }
//         else if (typeof(TOther) == typeof(sbyte))
//         {
//             sbyte actualResult = (value.Amount >= sbyte.MaxValue) ? sbyte.MaxValue :
//                 (value.Amount <= sbyte.MinValue) ? sbyte.MinValue : (sbyte)value;
//             result = (TOther)(object)actualResult;
//             return true;
//         }
//         else if (typeof(TOther) == typeof(float))
//         {
//             float actualResult = (float)value;
//             result = (TOther)(object)actualResult;
//             return true;
//         }
//         else
//         {
//             result = default;
//             return false;
//         }
//     }
// #endregion
//
// #endif
}
