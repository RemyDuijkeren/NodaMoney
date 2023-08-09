using System;
using System.Linq;

namespace NodaMoney;

/// <summary>A unit of exchange of value, a currency of <see cref="Money" />.</summary>
/// <remarks>See http://en.wikipedia.org/wiki/Currency</remarks>
public readonly record struct CurrencyUnit
{
    // readonly byte _code1;
    // readonly byte _code2;
    readonly ushort _code;
    readonly byte _namespace;

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

        // ushort for storing code (2bytes) = 15bits needed, 1bit left => use 1bit to mark if ISO? ISO=0, 1=other?
        // byte for storing namespace (4bits=15 or 3bits=7) and minor unit (4bits=15 or 5bit=31)? or use CurrencyInfo to retrieve?
        _namespace = @namespace;
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