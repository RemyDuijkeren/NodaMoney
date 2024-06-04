using System;
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