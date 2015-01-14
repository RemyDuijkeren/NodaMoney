using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace NodaMoney
{
    /// <summary>Represents Money, an amount defined in a specific Currency.</summary>
    /// <remarks>
    /// The <see cref="Money" /> structure allows development of applications that handle
    /// various types of Currency. Money will hold the <see cref="Currency" /> and Amount of money,
    /// and ensure that two different currencies cannot be added or subtracted to each other.
    /// </remarks>
    [StructLayout(LayoutKind.Sequential)]
    [DataContract]
    public partial struct Money : IEquatable<Money>
    {
        /// <summary>Initializes a new instance of the Money structure, based on the current culture.</summary>
        /// <param name="amount">The Amount of money as <see langword="decimal"/>.</param>
        /// <remarks>
        /// As rounding mode, MidpointRounding.ToEven is used (<seealso cref="http://msdn.microsoft.com/en-us/library/system.midpointrounding.aspx"/>). 
        /// The behavior of this method follows IEEE Standard 754, section 4. This kind of rounding is sometimes called
        /// rounding to nearest, or banker's rounding. It minimizes rounding errors that result from consistently rounding a 
        /// midpoint value in a single direction.
        /// </remarks>
        public Money(decimal amount)
            : this(amount, Currency.CurrentCurrency)
        {
        }

        /// <summary>Initializes a new instance of the Money structure, based on the current culture.</summary>
        /// <param name="amount">The Amount of money as <see langword="decimal"/>.</param>
        /// <param name="rounding">The rounding mode.</param>        
        public Money(decimal amount, MidpointRounding rounding)
            : this(amount, Currency.CurrentCurrency, rounding)
        {
        }

        /// <summary>Initializes a new instance of the Money structure, based on a ISO 4217 Currency code.</summary>        
        /// <param name="amount">The Amount of money as <see langword="decimal"/>.</param>
        /// <param name="code">A ISO 4217 Currency code, like EUR or USD.</param>
        /// <remarks>
        /// As rounding mode, MidpointRounding.ToEven is used (<seealso cref="http://msdn.microsoft.com/en-us/library/system.midpointrounding.aspx"/>). 
        /// The behavior of this method follows IEEE Standard 754, section 4. This kind of rounding is sometimes called
        /// rounding to nearest, or banker's rounding. It minimizes rounding errors that result from consistently rounding a 
        /// midpoint value in a single direction.
        /// </remarks>
        public Money(decimal amount, string code)
            : this(amount, Currency.FromCode(code))
        {
        }

        /// <summary>Initializes a new instance of the Money structure.</summary>        
        /// <param name="amount">The Amount of money as <see langword="decimal"/>.</param>
        /// <param name="currency">The Currency of the money.</param>
        /// <remarks>
        /// As rounding mode, MidpointRounding.ToEven is used (<seealso cref="http://msdn.microsoft.com/en-us/library/system.midpointrounding.aspx"/>). 
        /// The behavior of this method follows IEEE Standard 754, section 4. This kind of rounding is sometimes called
        /// rounding to nearest, or banker's rounding. It minimizes rounding errors that result from consistently rounding a 
        /// midpoint value in a single direction.
        /// </remarks>
        public Money(decimal amount, Currency currency)
            : this(amount, currency, MidpointRounding.ToEven)
        {
        }

        /// <summary>Initializes a new instance of the Money structure, based on a ISO 4217 Currency code.</summary>        
        /// <param name="amount">The Amount of money as <see langword="decimal"/>.</param>
        /// <param name="code">A ISO 4217 Currency code, like EUR or USD.</param>
        /// <param name="rounding">The rounding mode.</param>
        public Money(decimal amount, string code, MidpointRounding rounding)
            : this(amount, Currency.FromCode(code), rounding)
        {
        }

        /// <summary>Initializes a new instance of the Money structure.</summary>        
        /// <param name="amount">The Amount of money as <see langword="decimal"/>.</param>
        /// <param name="currency">The Currency of the money.</param>
        /// <param name="rounding">The rounding mode.</param>
        public Money(decimal amount, Currency currency, MidpointRounding rounding)
            : this()
        {
            Currency = currency;

            // TODO: Currency.Z07 and Currency.DOT edge case handeling!
            if (Currency.DecimalDigits == Currency.DOT)
                Amount = Math.Round(amount);
            if (Currency.DecimalDigits == Currency.Z07)
                Amount = Math.Round(amount, 1);

            Amount = Math.Round(amount, (int)Currency.DecimalDigits, rounding);
        }

        // int, uint ([CLSCompliant(false)]) // auto-casting to decimal so not needed

        /// <summary>Initializes a new instance of the Money structure.</summary>        
        /// <param name="amount">The Amount of money as <see langword="double"/> or <see langword="float"/> (float is implicitly casted to double).</param>
        /// <param name="currency">The Currency of the money.</param>
        /// <param name="rounding">The rounding mode.</param>
        public Money(double amount, Currency currency, MidpointRounding rounding)
            : this((decimal)amount, currency, rounding)
        {
        }

        /// <summary>Initializes a new instance of the Money structure.</summary>        
        /// <param name="amount">The Amount of money as <see langword="double"/> or <see langword="float"/> (float is implicitly casted to double).</param>
        /// <param name="currency">The Currency of the money.</param>
        /// <remarks>
        /// As rounding mode, MidpointRounding.ToEven is used (<seealso cref="http://msdn.microsoft.com/en-us/library/system.midpointrounding.aspx"/>). 
        /// The behavior of this method follows IEEE Standard 754, section 4. This kind of rounding is sometimes called
        /// rounding to nearest, or banker's rounding. It minimizes rounding errors that result from consistently rounding a 
        /// midpoint value in a single direction.
        /// </remarks>
        public Money(double amount, Currency currency)
            : this((decimal)amount, currency)
        {
        }

        /// <summary>Initializes a new instance of the Money structure.</summary>
        /// <param name="amount">The Amount of money as <see langword="long"/>, <see langword="int"/>, <see langword="short"/> or <see cref="byte"/>.</param>
        /// <param name="currency">The Currency of the money.</param>
        /// <remarks>
        /// The integral types are implicitly converted to long and the result evaluates
        /// to decimal. Therefore you can initialize a Money object using an integer literal,
        /// without the suffix, as follows:
        /// <code>
        /// Money money = new Money(10, Currency.FromIsoSymbol("EUR"));
        /// </code>
        /// As rounding mode, MidpointRounding.ToEven is used (<seealso cref="http://msdn.microsoft.com/en-us/library/system.midpointrounding.aspx"/>). 
        /// The behavior of this method follows IEEE Standard 754, section 4. This kind of rounding is sometimes called
        /// rounding to nearest, or banker's rounding. It minimizes rounding errors that result from consistently rounding a 
        /// midpoint value in a single direction.
        /// </remarks>
        public Money(long amount, Currency currency)
            : this((decimal)amount, currency)
        {
        }

        /// <summary>Initializes a new instance of the Money structure.</summary>
        /// <param name="amount">The Amount of money as <see langword="ulong"/>, <see langword="uint"/>, <see langword="ushort"/> or <see cref="byte"/>.</param>
        /// <param name="currency">The Currency of the money.</param>
        /// <remarks>
        /// The integral types are implicitly converted to long and the result evaluates
        /// to decimal. Therefore you can initialize a Money object using an integer literal,
        /// without the suffix, as follows:
        /// <code>
        /// Money money = new Money(10, Currency.FromIsoSymbol("EUR"));
        /// </code>
        /// As rounding mode, MidpointRounding.ToEven is used (<seealso cref="http://msdn.microsoft.com/en-us/library/system.midpointrounding.aspx"/>). 
        /// The behavior of this method follows IEEE Standard 754, section 4. This kind of rounding is sometimes called
        /// rounding to nearest, or banker's rounding. It minimizes rounding errors that result from consistently rounding a 
        /// midpoint value in a single direction.
        /// </remarks>
        [CLSCompliant(false)]
        public Money(ulong amount, Currency currency)
            : this((decimal)amount, currency)
        {
        }

        /// <summary>Initializes a new instance of the Money structure, based on a ISO 4217 Currency code.</summary>
        /// <param name="amount">The Amount of money as <see langword="double"/> or <see langword="float"/> (float is implicitly casted to double).</param>
        /// <param name="code">A ISO 4217 Currency code, like EUR or USD.</param>
        /// <remarks>
        /// As rounding mode, MidpointRounding.ToEven is used (<seealso cref="http://msdn.microsoft.com/en-us/library/system.midpointrounding.aspx"/>). 
        /// The behavior of this method follows IEEE Standard 754, section 4. This kind of rounding is sometimes called
        /// rounding to nearest, or banker's rounding. It minimizes rounding errors that result from consistently rounding a 
        /// midpoint value in a single direction.
        /// </remarks>
        public Money(double amount, string code)
            : this((decimal)amount, Currency.FromCode(code))
        {
        }

        /// <summary>Initializes a new instance of the Money structure, based on a ISO 4217 Currency code.</summary>
        /// <param name="amount">The Amount of money as <see langword="long"/>, <see langword="int"/>, <see langword="short"/> or <see cref="byte"/>.</param>
        /// <param name="code">A ISO 4217 Currency code, like EUR or USD.</param>
        /// <remarks>
        /// The integral types are implicitly converted to long and the result evaluates
        /// to decimal. Therefore you can initialize a Money object using an integer literal,
        /// without the suffix, as follows:
        /// <code>
        /// Money money = new Money(10, "EUR");
        /// </code>
        /// As rounding mode, MidpointRounding.ToEven is used (<seealso cref="http://msdn.microsoft.com/en-us/library/system.midpointrounding.aspx"/>). 
        /// The behavior of this method follows IEEE Standard 754, section 4. This kind of rounding is sometimes called
        /// rounding to nearest, or banker's rounding. It minimizes rounding errors that result from consistently rounding a 
        /// midpoint value in a single direction.
        /// </remarks>
        public Money(long amount, string code)
            : this((decimal)amount, Currency.FromCode(code))
        {
        }

        /// <summary>Initializes a new instance of the Money structure, based on a ISO 4217 Currency code.</summary>
        /// <param name="amount">The Amount of money as <see langword="ulong"/>, <see langword="uint"/>, <see langword="ushort"/> or <see cref="byte"/>.</param>
        /// <param name="code">A ISO 4217 Currency code, like EUR or USD.</param>
        /// <remarks>
        /// The integral types are implicitly converted to long and the result evaluates
        /// to decimal. Therefore you can initialize a Money object using an integer literal,
        /// without the suffix, as follows:
        /// <code>
        /// Money money = new Money(10, "EUR");
        /// </code>
        /// As rounding mode, MidpointRounding.ToEven is used (<seealso cref="http://msdn.microsoft.com/en-us/library/system.midpointrounding.aspx"/>). 
        /// The behavior of this method follows IEEE Standard 754, section 4. This kind of rounding is sometimes called
        /// rounding to nearest, or banker's rounding. It minimizes rounding errors that result from consistently rounding a 
        /// midpoint value in a single direction.
        /// </remarks>
        [CLSCompliant(false)]
        public Money(ulong amount, string code)
            : this((decimal)amount, Currency.FromCode(code))
        {
        }

        /// <summary>Initializes a new instance of the Money structure, based on the current culture.</summary>
        /// <param name="amount">The Amount of money as <see langword="double"/> or <see langword="float"/> (float is implicitly casted to double).</param>
        /// <remarks>
        /// As rounding mode, MidpointRounding.ToEven is used (<seealso cref="http://msdn.microsoft.com/en-us/library/system.midpointrounding.aspx"/>). 
        /// The behavior of this method follows IEEE Standard 754, section 4. This kind of rounding is sometimes called
        /// rounding to nearest, or banker's rounding. It minimizes rounding errors that result from consistently rounding a 
        /// midpoint value in a single direction.
        /// </remarks>
        public Money(double amount)
            : this((decimal)amount)
        {
        }

        /// <summary>Initializes a new instance of the Money structure, based on the current culture.</summary>
        /// <param name="amount">The Amount of money as <see langword="long"/>, <see langword="int"/>, <see langword="short"/> or <see cref="byte"/>.</param>
        /// <remarks>
        /// The integral types are implicitly converted to long and the result evaluates
        /// to decimal. Therefore you can initialize a Money object using an integer literal,
        /// without the suffix, as follows:
        /// <code>
        /// Money money = new Money(10, Currency.FromIsoSymbol("EUR"));
        /// </code>
        /// As rounding mode, MidpointRounding.ToEven is used (<seealso cref="http://msdn.microsoft.com/en-us/library/system.midpointrounding.aspx"/>). 
        /// The behavior of this method follows IEEE Standard 754, section 4. This kind of rounding is sometimes called
        /// rounding to nearest, or banker's rounding. It minimizes rounding errors that result from consistently rounding a 
        /// midpoint value in a single direction.
        /// </remarks>
        public Money(long amount)
            : this((decimal)amount)
        {
        }

        /// <summary>Initializes a new instance of the Money structure, based on the current culture.</summary>
        /// <param name="amount">The Amount of money as <see langword="ulong"/>, <see langword="uint"/>, <see langword="ushort"/> or <see cref="byte"/>.</param>
        /// <remarks>
        /// The integral types are implicitly converted to long and the result evaluates
        /// to decimal. Therefore you can initialize a Money object using an integer literal,
        /// without the suffix, as follows:
        /// <code>
        /// Money money = new Money(10, Currency.FromIsoSymbol("EUR"));
        /// </code>
        /// As rounding mode, MidpointRounding.ToEven is used (<seealso cref="http://msdn.microsoft.com/en-us/library/system.midpointrounding.aspx"/>). 
        /// The behavior of this method follows IEEE Standard 754, section 4. This kind of rounding is sometimes called
        /// rounding to nearest, or banker's rounding. It minimizes rounding errors that result from consistently rounding a 
        /// midpoint value in a single direction.
        /// </remarks>
        [CLSCompliant(false)]
        public Money(ulong amount)
            : this((decimal)amount)
        {
        }

        /// <summary>Gets the amount of money.</summary>
        [DataMember]
        public decimal Amount { get; private set; }

        /// <summary>Gets the <see cref="Currency"/> of the money.</summary>
        [DataMember]
        public Currency Currency { get; private set; }

        /// <summary>Returns a value indicating whether two instances of <see cref="Money"/> are equal.</summary>
        /// <param name="left">A <see cref="Money"/> object on the left side.</param>
        /// <param name="right">A <see cref="Money"/> object on the right side.</param>
        /// <returns>true if left and right are equal; otherwise, false.</returns>
        public static bool operator ==(Money left, Money right)
        {
            return left.Equals(right);
        }

        /// <summary>Returns a value indicating whether two instances of <see cref="Money"/> are not equal.</summary>
        /// <param name="left">A <see cref="Money"/> object on the left side.</param>
        /// <param name="right">A <see cref="Money"/> object on the right side.</param>
        /// <returns>true if left and right are not equal; otherwise, false.</returns>
        public static bool operator !=(Money left, Money right)
        {
            return !(left == right);
        }

        /// <summary>Returns a value indicating whether this instance and a specified <see cref="Money"/> object represent the same value.</summary>
        /// <param name="other">A <see cref="Money"/> object.</param>
        /// <returns>true if value is equal to this instance; otherwise, false.</returns>
        public bool Equals(Money other)
        {
            return this.Amount == other.Amount && this.Currency == other.Currency;
        }

        /// <summary>Returns a value indicating whether this instance and a specified <see cref="Object"/> represent the same type and value.</summary>
        /// <param name="obj">An <see cref="Object"/>.</param>
        /// <returns>true if value is equal to this instance; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            return (obj is Money) && this.Equals((Money)obj);
        }      

        /// <summary>Returns the hash code for this instance.</summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return Amount.GetHashCode() ^ (397 * Currency.GetHashCode());
            }
        }       

        [SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object[])",
            Justification = "Test fail when Invariant is used. Inline JIT bug? When cloning CultureInfo it works.")]
        private static void AssertIsSameCurrency(Money left, Money right)
        {
            if (left.Currency != right.Currency)         
                throw new InvalidCurrencyException(string.Format("{0} and {1} don't have the same Currency!", left, right));
        }
    }
}