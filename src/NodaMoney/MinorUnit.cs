namespace NodaMoney;

// store minor unit in 4 bits (0-15). // Power of 10, Math.Log10(2);
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
}
