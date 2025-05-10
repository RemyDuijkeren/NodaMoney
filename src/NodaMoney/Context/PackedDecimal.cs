namespace NodaMoney.Context;

#pragma warning disable RCS1181

internal readonly struct PackedDecimal
{
    // Masks for the Flags field
    private const int CurrencyMask = 0b_1111_1111_1111_1111;    // Bits 0–15, for Currency (16 bits)
    private const int ScaleMask = 0xFF_00_00;                   // Bits 16-23 for the Decimal scale
    private const int IndexMask = 0b_0111_1111 << 24;           // Bits 24–30, for Index (7 bits)
    private const int SignMask = unchecked((int)0x80_00_00_00); // Bit 31 for the Decimal sign bit (negative)
    private const int MaxIndexValue = 127; // Maximum values for a 7-bit index

    // Fields for storing the components of the decimal representation
    private readonly int _low;
    private readonly int _mid;
    private readonly int _high;
    private readonly int _flags;

    public PackedDecimal(decimal value, Currency currency = new(), byte index = 0)
    {
        if (index > MaxIndexValue)
            throw new ArgumentOutOfRangeException(nameof(index), "Index must be between 0 and 127.");

        // Extract the 4 integers from the decimal value.
#if NET5_0_OR_GREATER
        Span<int> bits = stackalloc int[4];
        decimal.GetBits(value, bits);
#else
        int[] bits = decimal.GetBits(value);
#endif

        _low = bits[0];
        _mid = bits[1];
        _high = bits[2];
        _flags = (bits[3] & ~(CurrencyMask | IndexMask))  // Clear existing Currency and Index bits
                 | (currency.EncodedValue & CurrencyMask) // Store Currency in bits 0–15
                 | ((index << 24) & IndexMask)            // Store Index in bits 24–30
                 | (bits[3] & (ScaleMask | SignMask));    // Preserve Scale Factor (16–23) and Sign (31)
    }

    public decimal Decimal
    {
        get
        {
            // Extract scale (bits 16-23) and sign (bit 31) from Flags
            byte scale = (byte)((_flags & ScaleMask) >> 16); // Extract scale (shift bits 16-23)
            bool isNegative = (_flags & SignMask) != 0; // Is negative if SignMask bit is set

            // Reconstruct the decimal with the correct `Flags` value (index removed)
            return new decimal(_low, _mid, _high, isNegative, scale);
        }
    }

    public Currency Currency => new((ushort)(_flags & CurrencyMask)); // Extract Currency (bits 0–15)
    public byte Scale => (byte)((_flags & ScaleMask) >> 16); // Extract Scale (bits 16-23)
    public byte Index => (byte)((_flags & IndexMask) >> 24); // Extract Index (bits 24–30)

    public override string ToString() => $"{Decimal} (Index: {Index}, Currency: {Currency})";
}
