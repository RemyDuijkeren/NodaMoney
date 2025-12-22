using System.Diagnostics;

namespace NodaMoney;

/// <summary>A unit of exchange of value, a currency of <see cref="Money" />.</summary>
/// <remarks>See http://en.wikipedia.org/wiki/Currency</remarks>
public readonly partial record struct Currency
{
    /// <summary>Represents an empty or undefined currency, commonly used to indicate the absence of a valid currency.</summary>
    public static readonly Currency NoCurrency = new();

    const string NoCurrencyCode = "XXX";
    const string InvalidCurrencyMessage = "Currency code should only exist out of three capital letters";
#pragma warning disable RCS1181
    const ushort MinorUnit2Mask = 1 << 15; // Bit 15
    const ushort CurrencyCodeMask = unchecked((ushort)~MinorUnit2Mask); // Bits 0-14
#pragma warning restore RCS1181

    /// <summary>ushort = 2 bytes, only 15 bits needed for code, 1bit left that is to indicate the flag 'IsMinorUnit2'.</summary>
    internal ushort EncodedValue { get; private init; }
    internal Currency(ushort encodedValue) => EncodedValue = encodedValue;

    /// <summary>Initializes a new instance of the <see cref="Currency"/> struct.</summary>
    /// <param name="code">The (ISO-4217) three-character code of the currency</param>
    /// <param name="isMinorUnit2">Indicates if the currency has a minor unit of 2. If null, a registry lookup will be performed.</param>
    /// <remarks>Represents a currency using the ISO-4217 three-character code system.</remarks>
    internal Currency(string code, bool? isMinorUnit2 = null) : this(code.AsSpan(), isMinorUnit2) { }

    /// <summary>Initializes a new instance of the <see cref="Currency"/> struct.</summary>
    /// <param name="code">The (ISO-4217) three-character code of the currency</param>
    /// <param name="isMinorUnit2">Indicates if the currency has a minor unit of 2. If null, a registry lookup will be performed.</param>
    /// <remarks>Represents a currency using the ISO-4217 three-character code system.</remarks>
    internal Currency(ReadOnlySpan<char> code, bool? isMinorUnit2 = null)
    {
        if (code.IsEmpty) throw new ArgumentNullException(nameof(code));
        if (code.Length != 3) throw new ArgumentException(InvalidCurrencyMessage, nameof(code));

        // Special handling for no currency. Use 0 for 'XXX' (No Currency)
        if (code.SequenceEqual("XXX".AsSpan()))
        {
            // _encodedValue is default 0
            return;
        }

        // A-Z (65-90 in ASCII) moves to 1-26 so that it fits in 5 bits.
        // Store in ushort (2 bytes) by shifting 5 bits to the left for each char.
        foreach (var c in code)
        {
            if (c is < 'A' or > 'Z')
                throw new ArgumentException(InvalidCurrencyMessage, nameof(code));

            EncodedValue = (ushort)(EncodedValue << 5 | (c - 'A' + 1));
        }

        bool bit = isMinorUnit2 ?? (CurrencyRegistry.TryGet(code.ToString(), out var info) && info.MinorUnit == MinorUnit.Two);
        if (bit)
        {
            EncodedValue |= MinorUnit2Mask;
        }

        Debug.Assert(Code != null, "Code should not be null");
        Debug.Assert(Code!.Length == 3, InvalidCurrencyMessage);
        Debug.Assert(Code.All(c => c is >= 'A' and <= 'Z'), InvalidCurrencyMessage);
    }

    /// <summary>Gets the (ISO-4217) three-character code of the currency.</summary>
    public string Code
    {
        get
        {
            if (EncodedValue is 0 or 25368) // 25368 = Precomputed value of "XXX" + IsMinorUnit2Mask=false
                return NoCurrencyCode;

            // Decode the stored ushort value into a 3-character string by shifting back into
            // separate bytes with clearing the left 3 bits using '& 0b_0001_1111' (= '& 0x1F')
            Span<char> result = stackalloc char[3];
            result[0] = (char)((EncodedValue >> 10 & 0x1F) + 'A' - 1); // 1-26 => A-Z (65-90 in ASCII)
            result[1] = (char)((EncodedValue >> 5 & 0x1F) + 'A' - 1);
            result[2] = (char)((EncodedValue & 0x1F) + 'A' - 1);

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER // Optimize by using Span<char>
            return new string(result);
#else
            return new string(result.ToArray());
#endif
        }
    }

    /// <summary>Gets a value indicating whether this currency is an ISO-4217 currency.</summary>
    public bool IsIso4217 => CurrencyInfo.GetInstance(this).IsIso4217;

    internal bool IsMinorUnit2 => (EncodedValue & MinorUnit2Mask) != 0;

    /// <summary>Create an instance of the <see cref="Currency"/> based on an ISO 4217 currency code.</summary>
    /// <param name="code">An ISO 4217 currency code, like EUR or USD.</param>
    /// <returns>An instance of the type <see cref="Currency"/>.</returns>
    /// <exception cref="ArgumentNullException">The value of 'code' cannot be null.</exception>
    /// <exception cref="ArgumentException">The 'code' is an unknown ISO 4217 currency code.</exception>
    public static Currency FromCode(string code) => CurrencyInfo.FromCode(code);

    /// <summary>Gets the smallest amount of the currency unit.</summary>
    public decimal MinimalAmount => CurrencyInfo.GetInstance(this).MinimalAmount;

    /// <summary>Gets the currency symbol.</summary>
    public string Symbol => CurrencyInfo.GetInstance(this).Symbol;

    /// <summary>Deconstructs the current instance into its components.</summary>
    /// <param name="code">The three-character currency code (ISO-4217) of the current instance.</param>
    /// <param name="symbol">The currency symbol of the current instance.</param>
    public void Deconstruct(out string code, out string symbol)
    {
        code = Code;
        symbol = Symbol;
    }
}
