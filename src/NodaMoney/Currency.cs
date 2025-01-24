namespace NodaMoney;

/// <summary>A unit of exchange of value, a currency of <see cref="Money" />.</summary>
/// <remarks>See http://en.wikipedia.org/wiki/Currency</remarks>
public readonly partial record struct Currency
{
    const string NoCurrencyCode = "XXX";
    const string InvalidCurrencyMessage = "Currency code should only exist out of three capital letters";

    // ushort = 2bytes, only 15bits needed for code, 1bit left that is to indicate flag 'IsIso4217'.
    readonly ushort _encodedValue;

    // TODO: store minor unit in 4 bits (0-15) and currency list in 2 bits (0-3)? : 4+2=6 bits (2bits left for 4 distinct values)
    // readonly byte _listAndMinorUnit;

    public static readonly Currency NoCurrency = new();

    /// <summary>Initializes a new instance of the <see cref="Currency"/> struct.</summary>
    /// <param name="code">The (ISO-4217) three-character code of the currency</param>
    /// <remarks>Represents a currency using the ISO-4217 three-character code system.</remarks>
    public Currency(string code) : this(code.AsSpan()) { }

    /// <summary>Initializes a new instance of the <see cref="Currency"/> struct.</summary>
    /// <param name="code">The (ISO-4217) three-character code of the currency</param>
    /// <remarks>Represents a currency using the ISO-4217 three-character code system.</remarks>
    public Currency(ReadOnlySpan<char> code)
    {
        // Special handling for no currency. Use 0 for 'XXX' (No Currency)
        if (code.Length == 3 && code[0] == 'X' && code[1] == 'X' && code[2] == 'X')
        {
            _encodedValue = 0;
            return;
        }

        if (code.IsEmpty) throw new ArgumentNullException(nameof(code));
        if (code.Length != 3) throw new ArgumentException(InvalidCurrencyMessage, nameof(code));

        // A-Z (65-90 in ASCII), move to 1-26 so that it fits in 5 bits.
        // Store in ushort (2 bytes) by shifting 5 bits to the left for each byte.
        _encodedValue = 0;
        foreach (var c in code)
        {
            if (c is < 'A' or > 'Z')
                throw new ArgumentException(InvalidCurrencyMessage, nameof(code));

            _encodedValue = (ushort)(_encodedValue << 5 | (c - 'A' + 1));
        }

        IsIso4217 = true;
    }

    /// <summary>Gets the (ISO-4217) three-character code of the currency.</summary>
    public string Code
    {
        get
        {
            if (_encodedValue is 0 or 25368) // 25368; // Precomputed value of "XXX"
                return NoCurrencyCode;

            // Decode the stored ushort value into a 3-character string by shifting back into
            // separate bytes with clearing the left 3 bits using '& 0b_0001_1111' (= '& 0x1F')
            Span<char> result = stackalloc char[3];
            result[0] = (char)((_encodedValue >> 10 & 0x1F) + 'A' - 1); // 1-26 => A-Z (65-90 in ASCII)
            result[1] = (char)((_encodedValue >> 5 & 0x1F) + 'A' - 1);
            result[2] = (char)((_encodedValue & 0x1F) + 'A' - 1);

#if NET5_0_OR_GREATER  // Optimize by using Span<char>
            return new string(result);
#else
            return new string(result.ToArray());
#endif
        }
    }

    /// <summary>Gets a value indicating whether this currency is a ISO-4217 currency.</summary>
    public bool IsIso4217
    {
        get
        {
            return (_encodedValue & 1 << 15) != 1; // get last bit
        }
        init
        {
            if (!value) _encodedValue |= 1 << 15; // set last bit to 1 if not ISO-4217 (so default is 0=true!)
        }
    }

    /// <summary>Create an instance of the <see cref="Currency"/> based on a ISO 4217 currency code.</summary>
    /// <param name="code">A ISO 4217 currency code, like EUR or USD.</param>
    /// <returns>An instance of the type <see cref="Currency"/>.</returns>
    /// <exception cref="ArgumentNullException">The value of 'code' cannot be null.</exception>
    /// <exception cref="ArgumentException">The 'code' is an unknown ISO 4217 currency code.</exception>
    [Obsolete("Use CurrencyInfo.FromCode(). This will be removed in a future version.")]
    public static Currency FromCode(string code) => CurrencyInfo.FromCode(code);

    /// <summary>Gets the smallest amount of the currency unit.</summary>
    public decimal MinimalAmount => CurrencyInfo.FromCode(Code).MinimalAmount;

    /// <summary>Gets the currency symbol.</summary>
    public string Symbol => CurrencyInfo.FromCode(Code).Symbol;

    /// <summary>Deconstructs the current instance into its components.</summary>
    /// <param name="code">The three-character currency code (ISO-4217) of the current instance.</param>
    /// <param name="symbol">The currency symbol of the current instance.</param>
    public void Deconstruct(out string code, out string symbol)
    {
        code = Code;
        symbol = Symbol;
    }
}
