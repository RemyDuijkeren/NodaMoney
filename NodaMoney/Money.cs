using System;
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
    [DataContract] // , ComVisible(true)]
    public partial struct Money : IComparable, IComparable<Money>, IEquatable<Money>, IFormattable  //, IConvertible (not supported in PCL)
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

        #region Other constructors

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
        
        #endregion

        /// <summary>Gets the amount of money.</summary>
        [DataMember]
        public decimal Amount { get; private set; }

        /// <summary>Gets the <see cref="Currency"/> of the money.</summary>
        [DataMember]
        public Currency Currency { get; private set; }

        #region Binary operators and there friendly named alternative methods

        /// <summary>Adds two specified <see cref="Money"/> values.</summary>
        /// <param name="money1">The first <see cref="Money"/> object.</param>
        /// <param name="money2">The second <see cref="Money"/> object.</param>
        /// <returns>A <see cref="Money"/> object with the values of both <see cref="Money"/> objects added.</returns>
        public static Money Add(Money money1, Money money2)
        {
            AssertIsSameCurrency(money1, money2);
            return new Money(Decimal.Add(money1.Amount, money2.Amount), money1.Currency);
        }

        /// <summary>Subtracts one specified <see cref="Money"/> value from another.</summary>
        /// <param name="money1">The first <see cref="Money"/> object.</param>
        /// <param name="money2">The second <see cref="Money"/> object.</param>
        /// <returns>A <see cref="Money"/> object where the second <see cref="Money"/> object is subtracted from the first.</returns>
        public static Money Subtract(Money money1, Money money2)
        {
            AssertIsSameCurrency(money1, money2);
            return new Money(Decimal.Subtract(money1.Amount, money2.Amount), money1.Currency);
        }

        /// <summary>Multiplies the specified money.</summary>
        /// <param name="money">The money.</param>
        /// <param name="multiplier">The multiplier.</param>
        /// <returns>The result as <see cref="Money"/> after multiplying.</returns>
        public static Money Multiply(Money money, decimal multiplier)
        {
            return new Money(Decimal.Multiply(money.Amount, multiplier), money.Currency);
        }

        /// <summary>Divides the specified money.</summary>
        /// <param name="money">The money.</param>
        /// <param name="divisor">The divider.</param>
        /// <returns>The division as <see cref="Money"/>.</returns>
        /// <remarks>This division can lose money! Use <seealso cref="SafeDivide"/> to do a safe division.</remarks>
        public static Money Divide(Money money, decimal divisor)
        {
            return new Money(Decimal.Divide(money.Amount, divisor), money.Currency);
        }

        /// <summary>Divides the specified money.</summary>
        /// <param name="money1">The money.</param>
        /// <param name="money2">The divider.</param>
        /// <returns>The <see cref="System.Decimal"/> result of dividing left with right.</returns>
        /// <remarks>Division of Money by Money, means the unit is lost, so the result will be Decimal.</remarks>
        public static decimal Divide(Money money1, Money money2)
        {
            AssertIsSameCurrency(money1, money2);
            return Decimal.Divide(money1.Amount, money2.Amount);
        }

        ///// <summary>Increments the specified money.</summary>
        ///// <param name="left">The left.</param>
        ///// <param name="right">The right.</param>
        ///// <returns>The incremented money.</returns>
        ////public static Money Increment(Money left, Money right)
        ////{
        ////    return left + right;
        ////}

        ///// <summary>Decrements the specified money.</summary>
        ///// <param name="left">The left.</param>
        ///// <param name="right">The right.</param>
        ///// <returns>The decremented money.</returns>
        ////public static Money Decrement(Money left, Money right)
        ////{
        ////    return left - right;
        ////}

        /// <summary>Adds two specified <see cref="Money"/> values.</summary>
        /// <param name="left">A <see cref="Money"/> object on the left side.</param>
        /// <param name="right">A <see cref="Money"/> object on the right side.</param>
        /// <returns>The <see cref="Money"/> result of adding left and right.</returns>
        public static Money operator +(Money left, Money right)
        {
            return Money.Add(left, right);
        }

        /// <summary>Subtracts two specified <see cref="Money"/> values.</summary>
        /// <param name="left">A <see cref="Money"/> object on the left side.</param>
        /// <param name="right">A <see cref="Money"/> object on the right side.</param>
        /// <returns>The <see cref="Money"/> result of subtracting right from left.</returns>
        public static Money operator -(Money left, Money right)
        {
            return Money.Subtract(left, right);
        }

        /// <summary>Multiplies the <see cref="Money"/> value by the given value.</summary>
        /// <param name="left">A <see cref="Money"/> object on the left side.</param>
        /// <param name="right">A <see cref="System.Decimal"/> object on the right side.</param>
        /// <returns>The <see cref="Money"/> result of multiplying right with left.</returns>
        public static Money operator *(Money left, decimal right)
        {
            return Money.Multiply(left, right);
        }

        /// <summary>Multiplies the <see cref="Money"/> value by the given value.</summary>
        /// <param name="left">A <see cref="System.Decimal"/> object on the left side.</param>
        /// <param name="right">A <see cref="Money"/> object on the right side.</param>
        /// <returns>The <see cref="Money"/> result of multiplying left with right.</returns>
        public static Money operator *(decimal left, Money right)
        {
            return Money.Multiply(right, left);
        }

        /// <summary>Divides the <see cref="Money"/> value by the given value.</summary>
        /// <param name="left">A <see cref="Money"/> object on the left side.</param>
        /// <param name="right">A <see cref="System.Decimal"/> object on the right side.</param>
        /// <returns>The <see cref="Money"/> result of dividing left with right.</returns>
        /// <remarks>This division can lose money! Use <seealso cref="SafeDivide"/> to do a safe division.</remarks>
        public static Money operator /(Money left, decimal right)
        {
            return Money.Divide(left, right);
        }

        /// <summary>Divides the <see cref="Money"/> value by the given value.</summary>
        /// <param name="left">A <see cref="Money"/> object on the left side.</param>
        /// <param name="right">A <see cref="Money"/> object on the right side.</param>
        /// <returns>The <see cref="System.Decimal"/> result of dividing left with right.</returns>
        /// <remarks>Division of Money by Money, means the unit is lost, so the result will be Decimal.</remarks>
        public static decimal operator /(Money left, Money right)
        {
            return Money.Divide(left, right);
        }

        ///// <summary>Implements the operator ++.</summary>
        ///// <param name="money">The money.</param>
        ///// <returns>The result of the operator.</returns>
        ////public static Money operator ++(Money money)
        ////{
        //// TODO: Create in Currency lowest cent value and use this. Not here: it's not responsiblity of money
        ////    decimal minValue = Math.Pow(10M, -1M * money.Currency.DecimalDigits);
        ////    return money + new Money(money.Currency, minValue);
        ////}

        ///// <summary>
        ///// Implements the operator --.
        ///// </summary>
        ///// <param name="money">The money.</param>
        ///// <returns>The result of the operator.</returns>
        ////public static Money operator --(Money money)
        ////{
        ////    double minValue = Math.Pow(10, -1 * money.Currency.DecimalDigits);
        ////    return money - new Money(money.Currency, minValue);
        ////}
        
        #endregion

        #region Unary operators and there friendly named alternative methods

        /// <summary>Pluses the specified money.</summary>
        /// <param name="money">The money.</param>
        /// <returns>The result.</returns>
        public static Money Plus(Money money)
        {
            return money;
        }

        /// <summary>Negates the specified money.</summary>
        /// <param name="money">The money.</param>
        /// <returns>The result.</returns>
        public static Money Negate(Money money)
        {
            return new Money(-money.Amount, money.Currency);
        }

        public static Money Increment(Money money)
        {
            return Add(money, new Money(money.Currency.MinorUnit, money.Currency));
        }

        public static Money Decrement(Money money)
        {
            return Subtract(money, new Money(money.Currency.MinorUnit, money.Currency));
        }

        /// <summary>Implements the operator +.</summary>
        /// <param name="money">The money.</param>
        /// <returns>The result of the operator.</returns>
        public static Money operator +(Money money)
        {
            return Plus(money);
        }

        /// <summary>Implements the operator -.</summary>
        /// <param name="money">The money.</param>
        /// <returns>The result of the operator.</returns>
        public static Money operator -(Money money)
        {
            return Negate(money);
        }

        public static Money operator ++(Money money)
        {
            return Increment(money);
        }

        public static Money operator --(Money money)
        {
            return Decrement(money);
        }

        #endregion

        #region IEquatable<Money> implementation

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

        #endregion

        #region IComparable and IComparable<Money> implementation

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
        public int CompareTo(object obj)
        {
            if (obj == null)
                return 1;

            if (!(obj is Money))
                throw new ArgumentException("obj is not the same type as this instance", "obj");

            return CompareTo((Money)obj);
        }

        /// <summary>Compares this instance to a specified <see cref="Object"/>.</summary>
        /// <param name="other">An <see cref="Object"/> or null.</param>
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
            AssertIsSameCurrency(this, other);

            return Amount.CompareTo(other.Amount);
        }

        /// <summary>Compares two specified <see cref="Money"/> values.</summary>
        /// <param name="money1">The first <see cref="Money"/> object.</param>
        /// <param name="money2">The second <see cref="Money"/> object.</param>
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
        public static int Compare(Money money1, Money money2)
        {
            return money1.CompareTo(money2);
        }

        /// <summary>Returns a value indicating whether a specified <see cref="Money"/> is less than another specified <see cref="Money"/>.</summary>
        /// <param name="left">A <see cref="Money"/> object on the left side.</param>
        /// <param name="right">A <see cref="Money"/> object on the right side.</param>
        /// <returns>true if left is less than right; otherwise, false.</returns>
        public static bool operator <(Money left, Money right)
        {
            return Compare(left, right) < 0;
        }

        /// <summary>Returns a value indicating whether a specified <see cref="Money"/> is greater than another specified <see cref="Money"/>.</summary>
        /// <param name="left">A <see cref="Money"/> object on the left side.</param>
        /// <param name="right">A <see cref="Money"/> object on the right side.</param>
        /// <returns>true if left is greater than right; otherwise, false.</returns>
        public static bool operator >(Money left, Money right)
        {
            return Compare(left, right) > 0;
        }

        /// <summary>Returns a value indicating whether a specified <see cref="Money"/> is less than or equal to another specified <see cref="Money"/>.</summary>
        /// <param name="left">A <see cref="Money"/> object on the left side.</param>
        /// <param name="right">A <see cref="Money"/> object on the right side.</param>
        /// <returns>true if left is less than or equal to right; otherwise, false.</returns>
        public static bool operator <=(Money left, Money right)
        {
            return Compare(left, right) <= 0;
        }

        /// <summary>Returns a value indicating whether a specified <see cref="Money"/> is greater than or equal to another specified <see cref="Money"/>.</summary>
        /// <param name="left">A <see cref="Money"/> object on the left side.</param>
        /// <param name="right">A <see cref="Money"/> object on the right side.</param>
        /// <returns>true if left is greater than or equal to right; otherwise, false.</returns>
        public static bool operator >=(Money left, Money right)
        {
            return Compare(left, right) >= 0;
        }

        #endregion

        #region Conversion Operators and IConvertible implementation

        ///// <summary>
        ///// Returns the <see cref="T:System.TypeCode"/> for this instance.
        ///// </summary>
        ///// <returns>
        ///// The enumerated constant that is the <see cref="T:System.TypeCode"/> of the class or value type that implements this interface.
        ///// </returns>
        ////public TypeCode GetTypeCode()
        ////{
        ////    return TypeCode.Object;
        ////}

        /// <summary>Converts the value of this instance to an equivalent Boolean value using the specified culture-specific formatting information.</summary>
        /// <param name="provider">An <see cref="T:System.IFormatProvider"/> interface implementation that supplies culture-specific formatting information.</param>
        /// <returns>A Boolean value equivalent to the value of this instance.</returns>
        public bool ToBoolean(IFormatProvider provider)
        {
            return Convert.ToBoolean(Amount, provider);
        }

        /// <summary>Converts the value of this instance to an equivalent 8-bit unsigned integer using the specified culture-specific formatting information.</summary>
        /// <param name="provider">An <see cref="T:System.IFormatProvider"/> interface implementation that supplies culture-specific formatting information.</param>
        /// <returns>An 8-bit unsigned integer equivalent to the value of this instance.</returns>
        public byte ToByte(IFormatProvider provider)
        {
            return Convert.ToByte(Amount, provider);
        }

        /// <summary>Converts the value of this instance to an equivalent Unicode character using the specified culture-specific formatting information.</summary>
        /// <param name="provider">An <see cref="T:System.IFormatProvider"/> interface implementation that supplies culture-specific formatting information.</param>
        /// <returns>A Unicode character equivalent to the value of this instance.</returns>
        public char ToChar(IFormatProvider provider)
        {
            return Convert.ToChar(Amount, provider);
        }

        /// <summary>Converts the value of this instance to an equivalent <see cref="T:System.DateTime"/> using the specified culture-specific formatting information.</summary>
        /// <param name="provider">An <see cref="T:System.IFormatProvider"/> interface implementation that supplies culture-specific formatting information.</param>
        /// <returns>A <see cref="T:System.DateTime"/> instance equivalent to the value of this instance.</returns>
        public DateTime ToDateTime(IFormatProvider provider)
        {
            return Convert.ToDateTime(Amount, provider);
        }

        /// <summary>Converts the value of this instance to an equivalent <see cref="T:System.Decimal"/> number using the specified culture-specific formatting information.</summary>
        /// <param name="provider">An <see cref="T:System.IFormatProvider"/> interface implementation that supplies culture-specific formatting information.</param>
        /// <returns>A <see cref="T:System.Decimal"/> number equivalent to the value of this instance.</returns>
        public decimal ToDecimal(IFormatProvider provider)
        {
            return Amount;
        }

        /// <summary>Converts the value of this instance to an equivalent double-precision floating-point number using the specified culture-specific formatting information.</summary>
        /// <param name="provider">An <see cref="T:System.IFormatProvider"/> interface implementation that supplies culture-specific formatting information.</param>
        /// <returns>A double-precision floating-point number equivalent to the value of this instance.</returns>
        public double ToDouble(IFormatProvider provider)
        {
            return Convert.ToDouble(Amount, provider);
        }

        /// <summary>Converts the value of this instance to an equivalent 16-bit signed integer using the specified culture-specific formatting information.</summary>
        /// <param name="provider">An <see cref="T:System.IFormatProvider"/> interface implementation that supplies culture-specific formatting information.</param>
        /// <returns>An 16-bit signed integer equivalent to the value of this instance.</returns>
        public short ToInt16(IFormatProvider provider)
        {
            return Convert.ToInt16(Amount, provider);
        }

        /// <summary>Converts the value of this instance to an equivalent 32-bit signed integer using the specified culture-specific formatting information.</summary>
        /// <param name="provider">An <see cref="T:System.IFormatProvider"/> interface implementation that supplies culture-specific formatting information.</param>
        /// <returns>An 32-bit signed integer equivalent to the value of this instance.</returns>
        public int ToInt32(IFormatProvider provider)
        {
            return Convert.ToInt32(Amount, provider);
        }

        /// <summary>Converts the value of this instance to an equivalent 64-bit signed integer using the specified culture-specific formatting information.</summary>
        /// <param name="provider">An <see cref="T:System.IFormatProvider"/> interface implementation that supplies culture-specific formatting information.</param>
        /// <returns>An 64-bit signed integer equivalent to the value of this instance.</returns>
        public long ToInt64(IFormatProvider provider)
        {
            return Convert.ToInt64(Amount, provider);
        }

        /// <summary>Converts the value of this instance to an equivalent 8-bit signed integer using the specified culture-specific formatting information.</summary>
        /// <param name="provider">An <see cref="T:System.IFormatProvider"/> interface implementation that supplies culture-specific formatting information.</param>
        /// <returns>An 8-bit signed integer equivalent to the value of this instance.</returns>
        [CLSCompliant(false)]
        public sbyte ToSByte(IFormatProvider provider)
        {
            return Convert.ToSByte(Amount, provider);
        }

        /// <summary>Converts the value of this instance to an equivalent single-precision floating-point number using the specified culture-specific formatting information.</summary>
        /// <param name="provider">An <see cref="T:System.IFormatProvider"/> interface implementation that supplies culture-specific formatting information.</param>
        /// <returns>A single-precision floating-point number equivalent to the value of this instance.</returns>
        public float ToSingle(IFormatProvider provider)
        {
            return Convert.ToSingle(Amount, provider);
        }

        /// <summary>Converts the value of this instance to an <see cref="T:System.Object"/> of the specified <see cref="T:System.Type"/> that has an equivalent value, using the specified culture-specific formatting information.</summary>
        /// <param name="conversionType">The <see cref="T:System.Type"/> to which the value of this instance is converted.</param>
        /// <param name="provider">An <see cref="T:System.IFormatProvider"/> interface implementation that supplies culture-specific formatting information.</param>
        /// <returns>An <see cref="T:System.Object"/> instance of type <paramref name="conversionType"/> whose value is equivalent to the value of this instance.</returns>
        public object ToType(Type conversionType, IFormatProvider provider)
        {
            return Convert.ChangeType(Amount, conversionType, provider);
        }

        /// <summary>Converts the value of this instance to an equivalent 16-bit unsigned integer using the specified culture-specific formatting information.</summary>
        /// <param name="provider">An <see cref="T:System.IFormatProvider"/> interface implementation that supplies culture-specific formatting information.</param>
        /// <returns>An 16-bit unsigned integer equivalent to the value of this instance.</returns>
        [CLSCompliant(false)]
        public ushort ToUInt16(IFormatProvider provider)
        {
            return Convert.ToUInt16(Amount, provider);
        }

        /// <summary>Converts the value of this instance to an equivalent 32-bit unsigned integer using the specified culture-specific formatting information.</summary>
        /// <param name="provider">An <see cref="T:System.IFormatProvider"/> interface implementation that supplies culture-specific formatting information.</param>
        /// <returns>An 32-bit unsigned integer equivalent to the value of this instance.</returns>
        [CLSCompliant(false)]
        public uint ToUInt32(IFormatProvider provider)
        {
            return Convert.ToUInt32(Amount, provider);
        }

        /// <summary>Converts the value of this instance to an equivalent 64-bit unsigned integer using the specified culture-specific formatting information.</summary>
        /// <param name="provider">An <see cref="T:System.IFormatProvider"/> interface implementation that supplies culture-specific formatting information.</param>
        /// <returns>An 64-bit unsigned integer equivalent to the value of this instance.</returns>
        [CLSCompliant(false)]
        public ulong ToUInt64(IFormatProvider provider)
        {
            return Convert.ToUInt64(Amount, provider);
        }

        /// <summary>Performs an explicit conversion from <see cref="NodaMoney.Money"/> to <see cref="System.Double"/>.</summary>
        /// <param name="money">The money.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator double(Money money)
        {
            return Convert.ToDouble(money.Amount);
        }

        /// <summary>Performs an explicit conversion from <see cref="NodaMoney.Money"/> to <see cref="System.Int64"/>.</summary>
        /// <param name="money">The money.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator long(Money money)
        {
            return Convert.ToInt64(money.Amount);
        }

        /// <summary>Performs an explicit conversion from <see cref="NodaMoney.Money"/> to <see cref="System.Decimal"/>.</summary>
        /// <param name="money">The money.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator decimal(Money money)
        {
            return money.Amount;
        }

        /// <summary>Performs an implicit conversion from <see cref="System.Int64"/> to <see cref="NodaMoney.Money"/>.</summary>
        /// <param name="money">The money.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator Money(long money)
        {
            return new Money(money);
        }

        /// <summary>Performs an implicit conversion from <see cref="System.UInt64"/> to <see cref="NodaMoney.Money"/>.</summary>
        /// <param name="money">The money.</param>
        /// <returns>The result of the conversion.</returns>
        [CLSCompliant(false)]
        public static implicit operator Money(ulong money)
        {
            return new Money(money);
        }

        /// <summary>Performs an implicit conversion from <see cref="System.Byte"/> to <see cref="NodaMoney.Money"/>.</summary>
        /// <param name="money">The money.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator Money(byte money)
        {
            return new Money(money);
        }

        /// <summary>Performs an implicit conversion from <see cref="System.UInt16"/> to <see cref="NodaMoney.Money"/>.</summary>
        /// <param name="money">The money.</param>
        /// <returns>The result of the conversion.</returns>
        [CLSCompliant(false)]
        public static implicit operator Money(ushort money)
        {
            return new Money(money);
        }

        /// <summary>Performs an implicit conversion from <see cref="System.UInt32"/> to <see cref="NodaMoney.Money"/>.</summary>
        /// <param name="money">The money.</param>
        /// <returns>The result of the conversion.</returns>
        [CLSCompliant(false)]
        public static implicit operator Money(uint money)
        {
            return new Money(money);
        }

        /// <summary>Performs an implicit conversion from <see cref="System.Double"/> to <see cref="NodaMoney.Money"/>.</summary>
        /// <param name="money">The money.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator Money(double money)
        {
            return new Money((decimal)money);
        }

        /// <summary>Performs an implicit conversion from <see cref="System.Decimal"/> to <see cref="NodaMoney.Money"/>.</summary>
        /// <param name="money">The money.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator Money(decimal money)
        {
            return new Money(money);
        }

        /// <summary>Converts the value of this instance to an <see cref="Single"/>.</summary>
        /// <param name="money">A <see cref="Money"/> value.</param>
        /// <returns>The value of the <see cref="Money"/> instance, converted to a <see cref="Single"/>.</returns>
        /// <remarks>
        /// Because a <see cref="Single"/> has fewer significant digits than a <see cref="Money"/> value, this operation may
        /// produce round-off errors. Also the <see cref="Currency"/> information is lost.
        /// </remarks>
        public static float ToSingle(Money money)
        {
            return Convert.ToSingle(money.Amount);
        }

        /// <summary>Converts the value of this instance to an <see cref="Double"/>.</summary>
        /// <param name="money">A <see cref="Money"/> value.</param>
        /// <returns>The value of the current instance, converted to a <see cref="Double"/>.</returns>
        /// <remarks>
        /// Because a Double has fewer significant digits than a <see cref="Money"/> value, this operation may produce round-off
        /// errors.
        /// </remarks>
        public static double ToDouble(Money money)
        {
            return Convert.ToDouble(money.Amount);
        }

        /// <summary>Converts the value of this instance to an <see cref="Decimal"/>.</summary>
        /// <param name="money">A <see cref="Money"/> value.</param>
        /// <returns>The value of the <see cref="Money"/> instance, converted to a <see cref="Decimal"/>.</returns>
        /// <remarks>The <see cref="Currency"/> information is lost.</remarks>
        public static decimal ToDecimal(Money money)
        {
            return money.Amount;
        }

        #endregion

        /// <summary>Converts this <see cref="Money"/> instance to its equivalent <see cref="String"/> representation.</summary>
        /// <returns>A string that represents this <see cref="Money"/> instance.</returns>
        /// <remarks>
        /// Converting will use the <see cref="NumberFormatInfo"/> object for the current culture if this has the same
        /// ISOCurrencySymbol, otherwise the <see cref="NumberFormatInfo"/> from the <see cref="Currency"/> will be used.
        /// </remarks>
        public override string ToString()
        {
            return ConvertToString(null, null);
        }

        /// <summary>Converts the <see cref="Money"/> value of this instance to its equivalent <see cref="String"/> representation using the specified format.</summary>
        /// <param name="format">A numeric format string.</param>
        /// <returns>The string representation of this <see cref="Money"/> instance as specified by the format.</returns>
        public string ToString(string format)
        {
            // http://msdn.microsoft.com/en-us/library/syy068tk.aspx
            if (format == null)
                throw new ArgumentNullException("format");
            
            return ConvertToString(format, null);
        }

        /// <summary>Converts this <see cref="Money"/> instance to its equivalent <see cref="String"/> representation using the specified culture-specific format information.</summary>
        /// <param name="formatProvider">An <see cref="IFormatProvider"/> that supplies culture-specific formatting information.</param>
        /// <returns>The string representation of this <see cref="Money"/> instance as specified by formatProvider.</returns>
        public string ToString(IFormatProvider formatProvider)
        {
            if (formatProvider == null)
                throw new ArgumentNullException("formatProvider");

            return ConvertToString(null, formatProvider);
        }

        /// <summary>Converts the <see cref="Money"/> value of this instance to its equivalent <see cref="String"/> representation using the specified format and culture-specific format information.</summary>
        /// <param name="format">A numeric format string.</param>
        /// <param name="formatProvider">An <see cref="IFormatProvider"/> that supplies culture-specific formatting information.</param>
        /// <returns>The string representation of this <see cref="Money"/> instance as specified by the format and formatProvider.</returns>
        public string ToString(string format, IFormatProvider formatProvider)
        {
            return ConvertToString(format, formatProvider);
        }

        private static void AssertIsSameCurrency(Money left, Money right)
        {
            if (left.Currency != right.Currency)
                throw new InvalidCurrencyException(string.Format("{0} and {1} don't have the same Currency! Use ExchangeRate to convert Money into the correct currency.", left, right));
        }

        private string ConvertToString(string format, IFormatProvider formatProvider)
        {
            // TODO: ICustomFormat : http://msdn.microsoft.com/query/dev12.query?appId=Dev12IDEF1&l=EN-US&k=k(System.IFormatProvider);k(TargetFrameworkMoniker-.NETPortable,Version%3Dv4.6);k(DevLang-csharp)&rd=true
            // TODO: Move to Currency? Currency.GetNumberFormatInfo()
            // TODO: Add custom format to represent USD 12.34, EUR 12.35, etc.
            // The formatting of Money should respect the NumberFormat of the current Culture, except for the CurrencySymbol and  CurrencyDecimalDigits.
            // http://en.wikipedia.org/wiki/Linguistic_issues_concerning_the_euro
            var numberFormatInfo = (NumberFormatInfo)NumberFormatInfo.CurrentInfo.Clone();

            if (formatProvider != null)
            {
                var ci = formatProvider as CultureInfo;
                if (ci != null)
                    numberFormatInfo = ci.NumberFormat;

                var nfi = formatProvider as NumberFormatInfo;
                if (nfi != null)
                    numberFormatInfo = nfi;
            }

            numberFormatInfo.CurrencySymbol = Currency.Sign;
            numberFormatInfo.CurrencyDecimalDigits = (int)Currency.DecimalDigits;

            return Amount.ToString(format ?? "C", numberFormatInfo);            
        }
    }
}