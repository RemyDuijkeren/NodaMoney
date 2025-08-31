using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace NodaMoney;

public partial struct Money : IComparable, IComparable<Money>
#if NET7_0_OR_GREATER
    , IComparisonOperators<Money, Money, bool>
#endif
{
    /// <summary>Returns a value indicating whether a specified <see cref="Money"/> is less than another specified <see cref="Money"/>.</summary>
    /// <param name="left">A <see cref="Money"/> object on the left side.</param>
    /// <param name="right">A <see cref="Money"/> object on the right side.</param>
    /// <returns>true if left is less than right; otherwise, false.</returns>
    public static bool operator <(Money left, Money right) => Compare(left, right) < 0;

    /// <summary>Returns a value indicating whether a specified <see cref="Money"/> is greater than another specified <see cref="Money"/>.</summary>
    /// <param name="left">A <see cref="Money"/> object on the left side.</param>
    /// <param name="right">A <see cref="Money"/> object on the right side.</param>
    /// <returns>true if left is greater than right; otherwise, false.</returns>
    public static bool operator >(Money left, Money right) => Compare(left, right) > 0;

    /// <summary>Returns a value indicating whether a specified <see cref="Money"/> is less than or equal to another specified <see cref="Money"/>.</summary>
    /// <param name="left">A <see cref="Money"/> object on the left side.</param>
    /// <param name="right">A <see cref="Money"/> object on the right side.</param>
    /// <returns>true if left is less than or equal to right; otherwise, false.</returns>
    public static bool operator <=(Money left, Money right) => Compare(left, right) <= 0;

    /// <summary>Returns a value indicating whether a specified <see cref="Money"/> is greater than or equal to another specified <see cref="Money"/>.</summary>
    /// <param name="left">A <see cref="Money"/> object on the left side.</param>
    /// <param name="right">A <see cref="Money"/> object on the right side.</param>
    /// <returns>true if left is greater than or equal to right; otherwise, false.</returns>
    public static bool operator >=(Money left, Money right) => Compare(left, right) >= 0;

    /// <summary>Compares two specified <see cref="Money"/> values.</summary>
    /// <param name="left">The first <see cref="Money"/> object.</param>
    /// <param name="right">The second <see cref="Money"/> object.</param>
    /// <returns>
    /// A signed number indicating the relative values of this instance and value.
    /// <list type="table">
    /// <listheader>
    ///   <term>Return Value</term>
    ///   <description>Meaning</description>
    /// </listheader>
    /// <item>
    ///   <term>Less than zero</term>
    ///   <description>This instance is less than value.</description>
    /// </item>
    /// <item>
    ///   <term>Zero</term>
    ///   <description>This instance is equal to value.</description>
    /// </item>
    /// <item>
    ///   <term>Greater than zero </term>
    ///   <description>This instance is greater than value.</description>
    /// </item>
    /// </list>
    /// </returns>
    public static int Compare(in Money left, in Money right) => left.CompareTo(right);

    /// <summary>Compares this instance to a specified <see cref="Money"/> object.</summary>
    /// <param name="obj">A <see cref="Money"/> object.</param>
    /// <returns>
    /// A signed number indicating the relative values of this instance and value.
    /// <list type="table">
    /// <listheader>
    ///   <term>Return Value</term>
    ///   <description>Meaning</description>
    /// </listheader>
    /// <item>
    ///   <term>Less than zero</term>
    ///   <description>This instance is less than value.</description>
    /// </item>
    /// <item>
    ///   <term>Zero</term>
    ///   <description>This instance is equal to value.</description>
    /// </item>
    /// <item>
    ///   <term>Greater than zero </term>
    ///   <description>This instance is greater than value.</description>
    /// </item>
    /// </list>
    /// </returns>
    /// <exception cref="ArgumentException">object is not the same type as this instance.</exception>
    public int CompareTo(object? obj)
    {
        if (obj == null)
            return 1;

        if (obj is not Money money)
            throw new ArgumentException("obj is not the same type as this instance", nameof(obj));

        return CompareTo(money);
    }

    /// <summary>Compares this instance to a specified <see cref="object"/>.</summary>
    /// <param name="other">An <see cref="object"/> or null.</param>
    /// <returns>
    /// A signed number indicating the relative values of this instance and value.
    /// <list type="table">
    /// <listheader>
    ///   <term>Return Value</term>
    ///   <description>Meaning</description>
    /// </listheader>
    /// <item>
    ///   <term>Less than zero</term>
    ///   <description>This instance is less than value.</description>
    /// </item>
    /// <item>
    ///   <term>Zero</term>
    ///   <description>This instance is equal to value.</description>
    /// </item>
    /// <item>
    ///   <term>Greater than zero </term>
    ///   <description>This instance is greater than value.</description>
    /// </item>
    /// </list>
    /// </returns>
    public int CompareTo(Money other)
    {
        ThrowIfCurrencyIncompatible(other);

        // Fast path: handle zeros (sign is not important for zero)
        uint thisMag = _low | _mid | _high;
        uint otherMag = other._low | other._mid | other._high;
        if ((thisMag | otherMag) == 0u)
            return 0;

        // Fast path: If signs differ, the negative is less
        bool thisNeg = (_flags & SignMask) != 0;
        bool otherNeg = (other._flags & SignMask) != 0;
        if (thisNeg != otherNeg)
            return thisNeg ? -1 : 1;

        // Fast path: If scales are the same, we can compare the 96-bit integers lexicographically
        if (Scale == other.Scale)
        {
            // Compare high, then mid, then low
#pragma warning disable RCS1238
            if (_high != other._high) return _high < other._high ? (thisNeg ? 1 : -1) : (thisNeg ? -1 : 1);
            if (_mid  != other._mid)  return _mid  < other._mid  ? (thisNeg ? 1 : -1) : (thisNeg ? -1 : 1);
            if (_low  != other._low)  return _low  < other._low  ? (thisNeg ? 1 : -1) : (thisNeg ? -1 : 1);
#pragma warning restore RCS1238
            return 0;
        }

        // Fallback when scales differ: rely on decimal comparison which aligns scales
        return Amount.CompareTo(other.Amount);
    }
}
