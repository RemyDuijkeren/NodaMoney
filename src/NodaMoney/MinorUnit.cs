namespace NodaMoney;

/// <summary>
/// The MinorUnit enum represents the minor unit as the power oo ten of a currency. The minor unit specifies the number of decimal places used
/// when representing fractional amounts of a currency. Each value of the MinorUnit enum corresponds to a specific number
/// of decimal places, ranging from 0 to 13.
/// </summary>
/// <remarks>store minor unit in 4 bits (0-15). we can use 5 bits (0-31) // Power of 10, Math.Log10(2);</remarks>
/// <remarks>Max scale of <see cref="Decimal"/> is 28.</remarks>
public enum MinorUnit : byte
{
#pragma warning disable RCS1181
    Zero = 0,   // 10^0 = 0 minor units
    One = 1,    // 10^1 = 10 minor units
    Two = 2,    // 10^2 = 100 minor units
    Three = 3,  // 10^3 = 1.000 minor units
    Four = 4,   // 10^4 = 10.000 minor units
    Five = 5,
    Six = 6,
    Seven = 7,
    Eight = 8,
    Nine = 9,
    Ten = 10,
    Eleven = 11,
    Twelve = 12,
    Thirteen = 13,
    Fourteen = 14,
    Fifteen = 15,
    Sixteen = 16,
    Seventeen = 17,
    Eighteen = 18,
    Nineteen = 19,
    Twenty = 20,
    TwentyOne = 21,
    TwentyTwo = 22,
    TwentyThree = 23,
    TwentyFour = 24,
    TwentyFive = 25,
    TwentySix = 26,
    TwentySeven = 27,
    TwentyEight = 28,
    /// <summary>
    /// Mauritania does not use a decimal division of units, setting 1 ouguiya (UM) equal to 5 khoums, and Madagascar has 1 ariary =
    /// 5 iraimbilanja. The coins display "1/5" on their face and are referred to as a "fifth". These are not used in practice, but when
    /// written out, a single significant digit is used. E.g., 1.2 UM.
    /// </summary>
    OneFifth = 254, // 1/5 = 10^log10(5) = 10^0.698970004
    NotApplicable = 255, // For N.A. we use 10^0 = 0 minor units
#pragma warning restore RCS1181
}
