using System.Text;

namespace NodaMoney;

/// <summary>A unit of exchange of value, a currency of <see cref="Money" />.</summary>
/// <remarks>See http://en.wikipedia.org/wiki/Currency</remarks>
public readonly record struct CurrencyUnit
{
    const string NoCurrencyCode = "XXX";

    // ushort = 2bytes, only 15bits needed for code, 1bit left => use last bit to indicate flag for ...?
    readonly ushort _code;

    // store minor unit in 4 bits (0-15) and currency list in 2 bits (0-3) : 4+2=6 bits (2bits left for 4 distinct values)
    readonly byte _listAndMinorUnit;

    public static readonly CurrencyUnit NoCurrency = new CurrencyUnit(NoCurrencyCode, MinorUnit.NotApplicable);

    /// <summary>Initializes a new instance of the <see cref="CurrencyUnit"/> struct.</summary>
    /// <param name="code">The (ISO-4217) three-character code of the currency</param>
    /// <param name="flag"></param>
    public CurrencyUnit(string code, MinorUnit minorUnit, CurrencyList currencyList = 0)
    {
        // if (code == null) throw new ArgumentNullException(nameof(code));
        // if (code.Length != 3) throw new ArgumentException("Currency code must be three characters long", nameof(code));
        //
        // if (code == NoCurrencyCode)
        // {
        //     _code = 0; // 25368 = 'XXX' (No Currency) => set to 0 (default)
        //     _listAndMinorUnit = 0; // No MinorUnit for 'XXX'
        //     return;
        // }

        Code = code;
        MinorUnit = minorUnit;
        CurrencyList = currencyList;

        // ushort = 2bytes, only 15bits needed for code, 1bit left => use last bit to indicate flag for ...?
        // IsIso4217? Or MinorUnit is known?
        //if (flag) _code |= 1 << 15; // set last bit to 1

        // store minor unit in 4 bits (0-15) and currency list in 2 bits (0-3) : 4+2=6 bits (2bits left for 4 distinct values)
        // if ((byte)currencyList > 3)
        //     throw new ArgumentOutOfRangeException(nameof(currencyList), "Currency list must be between 0 and 3");
        //
        // if ((byte)minorUnit > 15)
        //     throw new ArgumentOutOfRangeException(nameof(minorUnit), "Minor unit must be between 0 and 15");
        //
        // _listAndMinorUnit = (byte)((byte)minorUnit << 2 | (byte)currencyList);
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
        init
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            if (value.Length != 3) throw new ArgumentException("Currency code must be three characters long", nameof(value));

            if (value == NoCurrencyCode)
            {
                _code = 0; // 25368 = 'XXX' (No Currency) => set to 0 (default)
                _listAndMinorUnit = 0; // No MinorUnit for 'XXX'
                return;
            }

            _code = 0;
            for (var i = 0; i < value.Length; i++)
            {
                var c = value[i];
                if (c is < 'A' or > 'Z') throw new ArgumentException("Currency code should only exist out of capital letters", nameof(value));

                // A-Z (65-90 in ASCII) => 1-26 (fits in 5 bits). We use 0 for 'XXX' (No Currency)
                // store in ushort (2 bytes) by shifting 5 bits to the left for each byte
                _code = (ushort)(_code << 5 | (c - 'A' + 1));
            }
        }
    }

    public MinorUnit MinorUnit
    {
        get => (MinorUnit)(_listAndMinorUnit >> 2);
        init => _listAndMinorUnit = (byte)((byte)value << 2 | (byte)CurrencyList);
    }

    public CurrencyList CurrencyList
    {
        get => (CurrencyList)(_listAndMinorUnit & 0x03);
        init => _listAndMinorUnit = (byte)((byte)MinorUnit << 2 | (byte)value);
    }

    /// <summary>Gets a value indicating whether this currency is a special currency that is not a part of the ISO-4217 standard.</summary>
    //public bool Flag => (_code & 1 << 15) != 0;
}
