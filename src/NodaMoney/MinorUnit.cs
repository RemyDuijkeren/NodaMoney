namespace NodaMoney;

/// <summary>
/// The MinorUnit enum represents the minor unit as the power oo ten of a currency. The minor unit specifies the number of decimal places used
/// when representing fractional amounts of a currency. Each value of the MinorUnit enum corresponds to a specific number
/// of decimal places, ranging from 0 to 13.
/// </summary>
/// <remarks>store minor unit in 4 bits (0-15). // Power of 10, Math.Log10(2);</remarks>
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
    NotApplicable = 14,
    /// <summary>
    /// Mauritania does not use a decimal division of units, setting 1 ouguiya (UM) equal to 5 khoums, and Madagascar has 1 ariary =
    /// 5 iraimbilanja. The coins display "1/5" on their face and are referred to as a "fifth". These are not used in practice, but when
    /// written out, a single significant digit is used. E.g. 1.2 UM.
    /// </summary>
    OneFifth = 15, // Z07 = 0.69897000433601880478626110527551; // Math.Log10(5);

    // TODO: 0-13 is enough for ISO-4217 and most crypto, but Ethereum (ETH) has 18 decimal places (wei)
    // If now fits in 4bits, but if we need more, we can use 5 bits (0-31) or 6 bits (0-63) for minor unit

}
