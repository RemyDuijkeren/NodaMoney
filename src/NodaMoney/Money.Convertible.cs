using System;

namespace NodaMoney
{
    /// <summary>Represents Money, an amount defined in a specific Currency.</summary>
    public partial struct Money
#if !NETSTANDARD1_0
        : IConvertible
#endif
    {
        /// <summary>Performs an explicit conversion from <see cref="NodaMoney.Money"/> to <see cref="double"/>.</summary>
        /// <param name="money">The money.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator double(Money money)
        {
            return Convert.ToDouble(money.Amount);
        }

        /// <summary>Performs an explicit conversion from <see cref="NodaMoney.Money"/> to <see cref="long"/>.</summary>
        /// <param name="money">The money.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator long(Money money)
        {
            return Convert.ToInt64(money.Amount);
        }

        /// <summary>Performs an explicit conversion from <see cref="NodaMoney.Money"/> to <see cref="decimal"/>.</summary>
        /// <param name="money">The money.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator decimal(Money money)
        {
            return money.Amount;
        }

        /// <summary>Performs an implicit conversion from <see cref="long"/> to <see cref="NodaMoney.Money"/>.</summary>
        /// <param name="money">The money.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator Money(long money)
        {
            return new Money(money);
        }

        /// <summary>Performs an implicit conversion from <see cref="ulong"/> to <see cref="NodaMoney.Money"/>.</summary>
        /// <param name="money">The money.</param>
        /// <returns>The result of the conversion.</returns>
        [CLSCompliant(false)]
        public static implicit operator Money(ulong money)
        {
            return new Money(money);
        }

        /// <summary>Performs an implicit conversion from <see cref="byte"/> to <see cref="NodaMoney.Money"/>.</summary>
        /// <param name="money">The money.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator Money(byte money)
        {
            return new Money(money);
        }

        /// <summary>Performs an implicit conversion from <see cref="ushort"/> to <see cref="NodaMoney.Money"/>.</summary>
        /// <param name="money">The money.</param>
        /// <returns>The result of the conversion.</returns>
        [CLSCompliant(false)]
        public static implicit operator Money(ushort money)
        {
            return new Money(money);
        }

        /// <summary>Performs an implicit conversion from <see cref="uint"/> to <see cref="NodaMoney.Money"/>.</summary>
        /// <param name="money">The money.</param>
        /// <returns>The result of the conversion.</returns>
        [CLSCompliant(false)]
        public static implicit operator Money(uint money)
        {
            return new Money(money);
        }

        /// <summary>Performs an implicit conversion from <see cref="double"/> to <see cref="NodaMoney.Money"/>.</summary>
        /// <param name="money">The money.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator Money(double money)
        {
            return new Money((decimal)money);
        }

        /// <summary>Performs an implicit conversion from <see cref="decimal"/> to <see cref="NodaMoney.Money"/>.</summary>
        /// <param name="money">The money.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator Money(decimal money)
        {
            return new Money(money);
        }

        /// <summary>Converts the value of this instance to an <see cref="float"/>.</summary>
        /// <param name="money">A <see cref="Money"/> value.</param>
        /// <returns>The value of the <see cref="Money"/> instance, converted to a <see cref="float"/>.</returns>
        /// <remarks>Because a <see cref="float"/> has fewer significant digits than a <see cref="Money"/> value, this operation may
        /// produce round-off errors. Also the <see cref="Currency"/> information is lost.</remarks>
        public static float ToSingle(Money money)
        {
            return Convert.ToSingle(money.Amount);
        }

        /// <summary>Converts the value of this instance to an <see cref="double"/>.</summary>
        /// <param name="money">A <see cref="Money"/> value.</param>
        /// <returns>The value of the current instance, converted to a <see cref="double"/>.</returns>
        /// <remarks>Because a Double has fewer significant digits than a <see cref="Money"/> value, this operation may produce round-off
        /// errors.</remarks>
        public static double ToDouble(Money money)
        {
            return Convert.ToDouble(money.Amount);
        }

        /// <summary>Converts the value of this instance to an <see cref="decimal"/>.</summary>
        /// <param name="money">A <see cref="Money"/> value.</param>
        /// <returns>The value of the <see cref="Money"/> instance, converted to a <see cref="decimal"/>.</returns>
        /// <remarks>The <see cref="Currency"/> information is lost.</remarks>
        public static decimal ToDecimal(Money money)
        {
            return money.Amount;
        }

#if !NETSTANDARD1_0
        /// <summary>
        /// Returns the <see cref="T:System.TypeCode"/> for this instance.
        /// </summary>
        /// <returns>
        /// The enumerated constant that is the <see cref="T:System.TypeCode"/> of the class or value type that implements this interface.
        /// </returns>
        public TypeCode GetTypeCode()
        {
            return TypeCode.Object;
        }
#endif

        /// <summary>Converts the value of this instance to an equivalent Boolean value using the specified culture-specific
        /// formatting information.</summary>
        /// <param name="provider">An <see cref="T:System.IFormatProvider"/> interface implementation that supplies
        /// culture-specific formatting information.</param>
        /// <returns>A Boolean value equivalent to the value of this instance.</returns>
        public bool ToBoolean(IFormatProvider provider)
        {
            return Convert.ToBoolean(Amount, provider);
        }

        /// <summary>Converts the value of this instance to an equivalent 8-bit unsigned integer using the specified
        /// culture-specific formatting information.</summary>
        /// <param name="provider">An <see cref="T:System.IFormatProvider"/> interface implementation that supplies
        /// culture-specific formatting information.</param>
        /// <returns>An 8-bit unsigned integer equivalent to the value of this instance.</returns>
        public byte ToByte(IFormatProvider provider)
        {
            return Convert.ToByte(Amount, provider);
        }

        /// <summary>Converts the value of this instance to an equivalent Unicode character using the specified culture-specific
        /// formatting information.</summary>
        /// <param name="provider">An <see cref="T:System.IFormatProvider"/> interface implementation that supplies
        /// culture-specific formatting information.</param>
        /// <returns>A Unicode character equivalent to the value of this instance.</returns>
        public char ToChar(IFormatProvider provider)
        {
            return Convert.ToChar(Amount, provider);
        }

        /// <summary>Converts the value of this instance to an equivalent <see cref="T:System.DateTime"/> using the specified
        /// culture-specific formatting information.</summary>
        /// <param name="provider">An <see cref="T:System.IFormatProvider"/> interface implementation that supplies
        /// culture-specific formatting information.</param>
        /// <returns>A <see cref="T:System.DateTime"/> instance equivalent to the value of this instance.</returns>
        public DateTime ToDateTime(IFormatProvider provider)
        {
            return Convert.ToDateTime(Amount, provider);
        }

        /// <summary>Converts the value of this instance to an equivalent <see cref="T:System.Decimal"/> number using the specified
        /// culture-specific formatting information.</summary>
        /// <param name="provider">An <see cref="T:System.IFormatProvider"/> interface implementation that supplies
        /// culture-specific formatting information.</param>
        /// <returns>A <see cref="T:System.Decimal"/> number equivalent to the value of this instance.</returns>
        public decimal ToDecimal(IFormatProvider provider)
        {
            return Convert.ToDecimal(Amount, provider);
        }

        /// <summary>Converts the value of this instance to an equivalent double-precision floating-point number using the
        /// specified culture-specific formatting information.</summary>
        /// <param name="provider">An <see cref="T:System.IFormatProvider"/> interface implementation that supplies
        /// culture-specific formatting information.</param>
        /// <returns>A double-precision floating-point number equivalent to the value of this instance.</returns>
        public double ToDouble(IFormatProvider provider)
        {
            return Convert.ToDouble(Amount, provider);
        }

        /// <summary>Converts the value of this instance to an equivalent 16-bit signed integer using the specified
        /// culture-specific formatting information.</summary>
        /// <param name="provider">An <see cref="T:System.IFormatProvider"/> interface implementation that supplies
        /// culture-specific formatting information.</param>
        /// <returns>An 16-bit signed integer equivalent to the value of this instance.</returns>
        public short ToInt16(IFormatProvider provider)
        {
            return Convert.ToInt16(Amount, provider);
        }

        /// <summary>Converts the value of this instance to an equivalent 32-bit signed integer using the specified
        /// culture-specific formatting information.</summary>
        /// <param name="provider">An <see cref="T:System.IFormatProvider"/> interface implementation that supplies
        /// culture-specific formatting information.</param>
        /// <returns>An 32-bit signed integer equivalent to the value of this instance.</returns>
        public int ToInt32(IFormatProvider provider)
        {
            return Convert.ToInt32(Amount, provider);
        }

        /// <summary>Converts the value of this instance to an equivalent 64-bit signed integer using the specified
        /// culture-specific formatting information.</summary>
        /// <param name="provider">An <see cref="T:System.IFormatProvider"/> interface implementation that supplies
        /// culture-specific formatting information.</param>
        /// <returns>An 64-bit signed integer equivalent to the value of this instance.</returns>
        public long ToInt64(IFormatProvider provider)
        {
            return Convert.ToInt64(Amount, provider);
        }

        /// <summary>Converts the value of this instance to an equivalent 8-bit signed integer using the specified culture-specific
        /// formatting information.</summary>
        /// <param name="provider">An <see cref="T:System.IFormatProvider"/> interface implementation that supplies
        /// culture-specific formatting information.</param>
        /// <returns>An 8-bit signed integer equivalent to the value of this instance.</returns>
        [CLSCompliant(false)]
        public sbyte ToSByte(IFormatProvider provider)
        {
            return Convert.ToSByte(Amount, provider);
        }

        /// <summary>Converts the value of this instance to an equivalent single-precision floating-point number using the
        /// specified culture-specific formatting information.</summary>
        /// <param name="provider">An <see cref="T:System.IFormatProvider"/> interface implementation that supplies
        /// culture-specific formatting information.</param>
        /// <returns>A single-precision floating-point number equivalent to the value of this instance.</returns>
        public float ToSingle(IFormatProvider provider)
        {
            return Convert.ToSingle(Amount, provider);
        }

        /// <summary>Converts the value of this instance to an <see cref="T:System.Object"/> of the specified<see cref="T:System.Type"/> that has an equivalent value, using the specified culture-specific formatting information.</summary>
        /// <param name="conversionType">The <see cref="T:System.Type"/> to which the value of this instance is converted.</param>
        /// <param name="provider">An <see cref="T:System.IFormatProvider"/> interface implementation that supplies
        /// culture-specific formatting information.</param>
        /// <returns>An <see cref="T:System.Object"/> instance of type <paramref name="conversionType"/> whose value is equivalent
        /// to the value of this instance.</returns>
        public object ToType(Type conversionType, IFormatProvider provider)
        {
            return Convert.ChangeType(Amount, conversionType, provider);
        }

        /// <summary>Converts the value of this instance to an equivalent 16-bit unsigned integer using the specified
        /// culture-specific formatting information.</summary>
        /// <param name="provider">An <see cref="T:System.IFormatProvider"/> interface implementation that supplies
        /// culture-specific formatting information.</param>
        /// <returns>An 16-bit unsigned integer equivalent to the value of this instance.</returns>
        [CLSCompliant(false)]
        public ushort ToUInt16(IFormatProvider provider)
        {
            return Convert.ToUInt16(Amount, provider);
        }

        /// <summary>Converts the value of this instance to an equivalent 32-bit unsigned integer using the specified
        /// culture-specific formatting information.</summary>
        /// <param name="provider">An <see cref="T:System.IFormatProvider"/> interface implementation that supplies
        /// culture-specific formatting information.</param>
        /// <returns>An 32-bit unsigned integer equivalent to the value of this instance.</returns>
        [CLSCompliant(false)]
        public uint ToUInt32(IFormatProvider provider)
        {
            return Convert.ToUInt32(Amount, provider);
        }

        /// <summary>Converts the value of this instance to an equivalent 64-bit unsigned integer using the specified
        /// culture-specific formatting information.</summary>
        /// <param name="provider">An <see cref="T:System.IFormatProvider"/> interface implementation that supplies
        /// culture-specific formatting information.</param>
        /// <returns>An 64-bit unsigned integer equivalent to the value of this instance.</returns>
        [CLSCompliant(false)]
        public ulong ToUInt64(IFormatProvider provider)
        {
            return Convert.ToUInt64(Amount, provider);
        }
    }
}