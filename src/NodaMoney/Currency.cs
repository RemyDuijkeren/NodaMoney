using System.Text;

namespace NodaMoney;

/// <summary>A unit of exchange of value, a currency of <see cref="Money" />.</summary>
/// <remarks>See http://en.wikipedia.org/wiki/Currency</remarks>
public readonly record struct Currency
{
    const string NoCurrencyCode = "XXX";

    // ushort = 2bytes, only 15bits needed for code, 1bit left => use last bit to indicate flag for ...?
    readonly ushort _code;

    // store minor unit in 4 bits (0-15) and currency list in 2 bits (0-3) : 4+2=6 bits (2bits left for 4 distinct values)
    //readonly byte _listAndMinorUnit;

    public static readonly Currency NoCurrency = new (NoCurrencyCode);

    /// <summary>Initializes a new instance of the <see cref="Currency"/> struct.</summary>
    /// <param name="code">The (ISO-4217) three-character code of the currency</param>
    public Currency(string code)
    {
        Code = code;
        IsIso4217 = true;
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
                return;
            }

            _code = 0;
            foreach (var c in value)
            {
                if (c is < 'A' or > 'Z') throw new ArgumentException("Currency code should only exist out of capital letters", nameof(value));

                // A-Z (65-90 in ASCII) => 1-26 (fits in 5 bits). We use 0 for 'XXX' (No Currency)
                // store in ushort (2 bytes) by shifting 5 bits to the left for each byte
                _code = (ushort)(_code << 5 | (c - 'A' + 1));
            }
        }
    }

    /// <summary>Gets a value indicating whether this currency is a ISO-4217 currency.</summary>
    public bool IsIso4217
    {
        get
        {
            return (_code & 1 << 15) != 1; // get last bit
        }
        init
        {
            if (!value) _code |= 1 << 15; // set last bit to 1 if not ISO-4217 (so default is 0=true!)
        }
    }

    /// <summary>Create an instance of the <see cref="Currency"/> based on a ISO 4217 currency code.</summary>
    /// <param name="code">A ISO 4217 currency code, like EUR or USD.</param>
    /// <returns>An instance of the type <see cref="Currency"/>.</returns>
    /// <exception cref="ArgumentNullException">The value of 'code' cannot be null.</exception>
    /// <exception cref="ArgumentException">The 'code' is an unknown ISO 4217 currency code.</exception>
    public static Currency FromCode(string code) => CurrencyInfo.FromCode(code).CurrencyUnit;

    /// <summary>Gets the smallest amount of the currency unit.</summary>
    public decimal MinimalAmount => CurrencyInfo.FromCode(Code).MinimalAmount;

    /// <summary>Gets the currency symbol.</summary>
    public string Symbol => CurrencyInfo.FromCode(Code).Symbol;

    /// <summary>Deconstructs the current instance into its components.</summary>
    /// <param name="code">The code.</param>
    /// <param name="symbol">The currency symbol.</param>
    public void Deconstruct(out string code, out string symbol)
    {
        code = Code;
        symbol = Symbol;
    }
}
