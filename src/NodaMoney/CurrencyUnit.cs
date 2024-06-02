using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;

namespace NodaMoney;

/// <summary>A unit of exchange of value, a currency of <see cref="Money" />.</summary>
/// <remarks>See http://en.wikipedia.org/wiki/Currency</remarks>
public readonly record struct CurrencyUnit
{
    // readonly byte _code1;
    // readonly byte _code2;
    readonly ushort _code;
    //readonly byte _namespace;

    /// <summary>Initializes a new instance of the <see cref="CurrencyUnit"/> struct.</summary>
    /// <param name="code">The (ISO-4217) three-character code of the currency</param>
    /// <param name="flag"></param>
    public CurrencyUnit(string code, byte @namespace = 0, bool flag = false)
    {
        if (code == null) throw new ArgumentNullException(nameof(code));
        
        var chars = code.ToCharArray();
        if (chars.Length != 3) throw new ArgumentException("Currency code must be three characters long", nameof(code));
        
        // don't use LINQ here, it's faster to check the individual characters
        var bytes = new byte[3];
        for (var i = 0; i < chars.Length; i++)
        {
            var c = chars[i];
            if (c is < 'A' or > 'Z') throw new ArgumentException("Currency code should only exist out of capital letters", nameof(code));
            bytes[i] = (byte)(c - 'A' + 1); // A-Z (65-90 in ASCII) => 1-26 (fits in 5 bits)
        }

        // store in ushort (2 bytes) by shifting 5 bits to the left for each byte
        _code = (ushort)(bytes[0] << 10 | bytes[1] << 5 | bytes[2]);

        // ushort = 2bytes, only 15bits needed for code, 1bit left => use last bit to indicate flag for ...? IsIso4217?
        if (flag) _code |= 1 << 15; // set last bit to 1

        if (_code == 25368) _code = 0; // 25368 = 'XXX' (No Currency) => set to 0 (default)
        
        //_code1 = (byte)_code;
        //_code2 = (byte)(_code >> 8);

        // TODO: store namespace
        // ushort for storing code (2bytes) = 15bits needed, 1bit left => use 1bit to mark if ISO? ISO=0, 1=other?
        // byte for storing namespace (4bits=15 or 3bits=7) and minor unit (4bits=15 or 5bit=31)? or use CurrencyInfo to retrieve?
        //_namespace = @namespace;
    }

    /// <summary>Gets the (ISO-4217) three-character code of the currency.</summary>
    public string Code
    {
        get
        {
            if (_code == 0) return "XXX";
            //if (_code1 == 0 && _code2 == 0) return "XXX";

            // var _code = _code1 | _code2 << 8;
            
            // shifting back into separate bytes with clearing the left 3 bits using '& 0b_0001_1111' (= '& 0x1F')
            var bytes = new[] { (byte)(_code >> 10 & 0x1F), (byte)(_code >> 5 & 0x1F), (byte)(_code & 0x1F) };
            
            return new string(bytes.Select(b => (char)(b + 'A' - 1)).ToArray()); // 1-26 => A-Z (65-90 in ASCII)
        }
    }
    
    /// <summary>Gets a value indicating whether this currency is a special currency that is not a part of the ISO-4217 standard.</summary>
    //public bool Flag => (_code & 1 << 15) != 0;
}

    [StructLayout(LayoutKind.Sequential)]
    public readonly struct MoneyUnit : IEquatable<MoneyUnit>
    {
        /// <summary>Initializes a new instance of the <see cref="Money"/> struct, based on a ISO 4217 Currency code.</summary>
        /// <param name="amount">The Amount of money as <see langword="decimal"/>.</param>
        /// <param name="code">A ISO 4217 Currency code, like EUR or USD.</param>
        /// <remarks>The amount will be rounded to the number of decimal digits of the specified currency
        /// (<see cref="NodaMoney.Currency.DecimalDigits"/>). As rounding mode, MidpointRounding.ToEven is used
        /// (<see cref="System.MidpointRounding"/>). The behavior of this method follows IEEE Standard 754, section 4. This
        /// kind of rounding is sometimes called rounding to nearest, or banker's rounding. It minimizes rounding errors that
        /// result from consistently rounding a midpoint value in a single direction.</remarks>
        public MoneyUnit(decimal amount, string code)
            : this(amount, new CurrencyUnit(code))
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
            : this(amount, new CurrencyUnit(code), rounding)
        {
        }
        
        public MoneyUnit(decimal amount, CurrencyUnit currency, MidpointRounding rounding)
            : this()
        {
            Currency = currency;
            Amount = Round(amount, currency, rounding);
        }

        /// <summary>Gets the amount of money.</summary>
        public decimal Amount { get; }

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