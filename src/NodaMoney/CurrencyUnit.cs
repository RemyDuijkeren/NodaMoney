using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text;

namespace NodaMoney;

public enum CurrencyList : byte
{
    Iso4217 = 0,
    Iso4217Historic = 1,
    Other = 2
}

public enum MinorUnit : byte
{
    Zero = 0,
    One = 1,
    Two = 2,
    Three = 3,
    Four = 4,
    Five = 5,
    Six = 6,
    Seven = 7,
    Eight = 8,
    Nine = 9,
    Ten = 10,
    Eleven = 11,
    Twelve = 12,
    Thirteen = 13,
    Z07Byte = 14,
    NotApplicable = 15,
}

/// <summary>A unit of exchange of value, a currency of <see cref="Money" />.</summary>
/// <remarks>See http://en.wikipedia.org/wiki/Currency</remarks>
public readonly record struct CurrencyUnit
{
    const string NoCurrencyCode = "XXX";
    readonly ushort _code;
    readonly byte _listAndMinorUnit;
    public static readonly CurrencyUnit NoCurrency = new CurrencyUnit(NoCurrencyCode, MinorUnit.NotApplicable);

    /// <summary>Initializes a new instance of the <see cref="CurrencyUnit"/> struct.</summary>
    /// <param name="code">The (ISO-4217) three-character code of the currency</param>
    /// <param name="flag"></param>
    public CurrencyUnit(string code, MinorUnit minorUnit, CurrencyList currencyList = 0)
    {
        if (code == null) throw new ArgumentNullException(nameof(code));
        if (code.Length != 3) throw new ArgumentException("Currency code must be three characters long", nameof(code));

        if (code == NoCurrencyCode)
        {
            _code = 0; // 25368 = 'XXX' (No Currency) => set to 0 (default)
            _listAndMinorUnit = 0; // No MinorUnit for 'XXX'
            return;
        }
        
        // don't use LINQ here, it's faster to check the individual characters
        _code = 0;
        for (var i = 0; i < code.Length; i++)
        {
            var c = code[i];
            if (c is < 'A' or > 'Z') throw new ArgumentException("Currency code should only exist out of capital letters", nameof(code));
            
            // A-Z (65-90 in ASCII) => 1-26 (fits in 5 bits). We use 0 for 'XXX' (No Currency)
            // store in ushort (2 bytes) by shifting 5 bits to the left for each byte
            _code = (ushort)(_code << 5 | (c - 'A' + 1));
        }

        // ushort = 2bytes, only 15bits needed for code, 1bit left => use last bit to indicate flag for ...?
        // IsIso4217? Or MinorUnit is known?
        //if (flag) _code |= 1 << 15; // set last bit to 1
        
        // store minor unit in 4 bits (0-15) and currency list in 2 bits (0-3)
        if ((byte)currencyList > 3)
            throw new ArgumentOutOfRangeException(nameof(currencyList), "Currency list must be between 0 and 3");
        
        if ((byte)minorUnit > 15) 
            throw new ArgumentOutOfRangeException(nameof(minorUnit), "Minor unit must be between 0 and 15");
        
        _listAndMinorUnit = (byte)((byte)minorUnit << 2 | (byte)currencyList);
    }

    /// <summary>Gets the (ISO-4217) three-character code of the currency.</summary>
    public string Code
    {
        get
        {
            if (_code == 0) return NoCurrencyCode;
            
            // shifting back into separate bytes with clearing the left 3 bits using '& 0b_0001_1111' (= '& 0x1F')
            var sb = new StringBuilder(3);
            sb.Append((char)((_code >> 10 & 0x1F) + 'A' - 1)); // 1-26 => A-Z (65-90 in ASCII)
            sb.Append((char)((_code >> 5 & 0x1F) + 'A' - 1));
            sb.Append((char)((_code & 0x1F) + 'A' - 1));

            return sb.ToString();
        }
    }
    
    /// <summary>Gets a value indicating whether this currency is a special currency that is not a part of the ISO-4217 standard.</summary>
    //public bool Flag => (_code & 1 << 15) != 0;
}

    [StructLayout(LayoutKind.Sequential)]
    public readonly struct MoneyUnit : IEquatable<MoneyUnit>
    {
        readonly long _amount;

        /// <summary>Initializes a new instance of the <see cref="Money"/> struct, based on a ISO 4217 Currency code.</summary>
        /// <param name="amount">The Amount of money as <see langword="decimal"/>.</param>
        /// <param name="code">A ISO 4217 Currency code, like EUR or USD.</param>
        /// <remarks>The amount will be rounded to the number of decimal digits of the specified currency
        /// (<see cref="NodaMoney.Currency.DecimalDigits"/>). As rounding mode, MidpointRounding.ToEven is used
        /// (<see cref="System.MidpointRounding"/>). The behavior of this method follows IEEE Standard 754, section 4. This
        /// kind of rounding is sometimes called rounding to nearest, or banker's rounding. It minimizes rounding errors that
        /// result from consistently rounding a midpoint value in a single direction.</remarks>
        public MoneyUnit(decimal amount, string code)
            : this(amount, new CurrencyUnit(code,0))
        {
        }

        /// <summary>Initializes a new instance of the <see cref="Money"/> struct.</summary>
        /// <param name="amount">The Amount of money as <see langword="decimal"/>.</param>
        /// <param name="currency">The Currency of the money.</param>
        /// <remarks>The amount will be rounded to the number of decimal digits of the specified currency
        /// (<see cref="NodaMoney.Currency.DecimalDigits"/>). As rounding mode, MidpointRounding.ToEven is used
        /// (<see cref="System.MidpointRounding"/>). The behavior of this method follows IEEE Standard 754, section 4. This
        /// kind of rounding is sometimes called rounding to nearest, or banker's rounding. It minimizes rounding errors that
        /// result from consistently rounding a midpoint value in a single direction.</remarks>
        public MoneyUnit(decimal amount, CurrencyUnit currency)
            : this(amount, currency, MidpointRounding.ToEven)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="Money"/> struct, based on a ISO 4217 Currency code.</summary>
        /// <param name="amount">The Amount of money as <see langword="decimal"/>.</param>
        /// <param name="code">A ISO 4217 Currency code, like EUR or USD.</param>
        /// <param name="rounding">The rounding mode.</param>
        /// <remarks>The amount will be rounded to the number of decimal digits of the specified currency
        /// (<see cref="NodaMoney.Currency.DecimalDigits"/>).</remarks>
        public MoneyUnit(decimal amount, string code, MidpointRounding rounding)
            : this(amount, new CurrencyUnit(code, 0), rounding)
        {
        }
        
        public MoneyUnit(decimal amount, CurrencyUnit currency, MidpointRounding rounding)
            : this()
        {
            Currency = currency;
            
            decimal rounded = Round(amount, currency, rounding);
            _amount = (long)(rounded * (decimal)Math.Pow(10, 2));
        }

        /// <summary>Gets the amount of money.</summary>
        public decimal Amount => _amount / (decimal)Math.Pow(10, 2);

        /// <summary>Gets the <see cref="Currency"/> of the money.</summary>
        public CurrencyUnit Currency { get; } 

        /// <summary>Returns a value indicating whether two instances of <see cref="Money"/> are equal.</summary>
        /// <param name="left">A <see cref="Money"/> object on the left side.</param>
        /// <param name="right">A <see cref="Money"/> object on the right side.</param>
        /// <returns>true if left and right are equal; otherwise, false.</returns>
        public static bool operator ==(MoneyUnit left, MoneyUnit right) => left.Equals(right);

        /// <summary>Returns a value indicating whether two instances of <see cref="Money"/> are not equal.</summary>
        /// <param name="left">A <see cref="Money"/> object on the left side.</param>
        /// <param name="right">A <see cref="Money"/> object on the right side.</param>
        /// <returns>true if left and right are not equal; otherwise, false.</returns>
        public static bool operator !=(MoneyUnit left, MoneyUnit right) => !(left == right);

        /// <summary>Returns a value indicating whether this instance and a specified <see cref="Money"/> object represent the same
        /// value.</summary>
        /// <param name="other">A <see cref="Money"/> object.</param>
        /// <returns>true if value is equal to this instance; otherwise, false.</returns>
        public bool Equals(MoneyUnit other) => Amount == other.Amount && Currency == other.Currency;

        /// <summary>Returns a value indicating whether this instance and a specified <see cref="object"/> represent the same type
        /// and value.</summary>
        /// <param name="obj">An <see cref="object"/>.</param>
        /// <returns>true if value is equal to this instance; otherwise, false.</returns>
        public override bool Equals(object obj) => obj is MoneyUnit money && this.Equals(money);

        /// <summary>Returns the hash code for this instance.</summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = (hash * 23) + Amount.GetHashCode();
                return (hash * 23) + Currency.GetHashCode();
            }
        }

        /// <summary>Deconstructs the current instance into its components.</summary>
        /// <param name="amount">The Amount of money as <see langword="decimal"/>.</param>
        /// <param name="currency">The Currency of the money.</param>
        public void Deconstruct(out decimal amount, out CurrencyUnit currency)
        {
            amount = Amount;
            currency = Currency;
        }

        private static decimal Round(in decimal amount, in CurrencyUnit currencyUnit, in MidpointRounding rounding)
        {
            // https://stackoverflow.com/questions/43289478/how-can-i-tell-if-a-number-is-a-power-of-10-in-kotlin-or-java
            static bool IsPowerOf10(long n)
            {
                while (n > 9 && n % 10 == 0)
                {
                    n /= 10;
                }

                return n == 1;
            }

            // NOT GOOD, IS ONLY FOR INT. EXTEND
            static bool IsPowerOfTen(in double x)
            {
                return x == 1
                    || x == 10
                    || x == 100
                    || x == 1000
                    || x == 10000
                    || x == 100000
                    || x == 1000000
                    || x == 10000000
                    || x == 100000000
                    || x == 1000000000;
            }

            Currency currency = NodaMoney.Currency.FromCode(currencyUnit.Code);
            return IsPowerOfTen(currency.MinorUnit)
                ? Math.Round(amount, currency.DecimalDigits, rounding)
                : Math.Round(amount / currency.MinimalAmount, 0, rounding) * currency.MinimalAmount;
        }

        [SuppressMessage(
            "Microsoft.Globalization",
            "CA1305:SpecifyIFormatProvider",
            MessageId = "System.String.Format(System.String,System.Object[])",
            Justification = "Test fail when Invariant is used. Inline JIT bug? When cloning CultureInfo it works.")]
        private static void AssertIsSameCurrency(in Money left, in Money right)
        {
            if (left.Currency != right.Currency)
                throw new InvalidCurrencyException(left.Currency, right.Currency);
        }
    }