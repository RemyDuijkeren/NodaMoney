namespace NodaMoney.Context;

/// <summary>Represents a unique index limited to 7 bits (0–127), used for the MoneyContext.</summary>
/// <remarks>This is a <see cref="byte"/> structure, but limited to 7-bit (0-127), instead of 8-bit.
/// It can be implicitly cast to a <see cref="byte"/> and explicit back.</remarks>
public readonly struct MoneyContextIndex
{
    /// <summary>Maximum value for the MoneyContextIndex (7 bits)</summary>
    /// <remarks>Limited to 7 bits (0-127) due to storage space in the Money type.</remarks>
    private const byte MaxValue = 127; // 7 bits

    /// <summary>Tracks next to be assigned index</summary>
    private static byte s_nextIndex;

    private readonly byte _value;

    /// <summary>Private constructor to enforce creation through New().</summary>
    /// <param name="value">The actual index value.</param>
    private MoneyContextIndex(byte value)
    {
        _value = value;
    }

    /// <summary>Allocates and returns the next available MoneyContextIndex.</summary>
    /// <exception cref="InvalidOperationException">The maximum number of MoneyContexts is reached (=128)</exception>
    public static MoneyContextIndex New()
    {
        if (s_nextIndex > MaxValue)
        {
            throw new InvalidOperationException($"Maximum number of MoneyContexts ({MaxValue + 1}) reached.");
        }

        return new MoneyContextIndex(s_nextIndex++);
    }

    public static implicit operator byte(MoneyContextIndex index) => index._value;

    public static explicit operator MoneyContextIndex(byte value) =>
        new(value <= MaxValue
            ? value
            : throw new ArgumentOutOfRangeException(nameof(value), value, $"MoneyContextIndex must be between 0 and {MaxValue} (7 bits)."));

    public override bool Equals(object? obj) =>
        obj is MoneyContextIndex other && _value == other._value;

    public override int GetHashCode() => _value;

    public static bool operator ==(MoneyContextIndex left, MoneyContextIndex right) => left._value == right._value;

    public static bool operator !=(MoneyContextIndex left, MoneyContextIndex right) => left._value != right._value;

    public static bool operator <(MoneyContextIndex left, MoneyContextIndex right) => left._value < right._value;

    public static bool operator >(MoneyContextIndex left, MoneyContextIndex right) => left._value > right._value;

    public static bool operator <=(MoneyContextIndex left, MoneyContextIndex right) => left._value <= right._value;

    public static bool operator >=(MoneyContextIndex left, MoneyContextIndex right) => left._value >= right._value;

    public override string ToString() => _value.ToString();
}
