using NodaMoney.Context;

namespace NodaMoney.Tests.Helpers;

/// <summary>Test-only <see cref="IRoundingStrategy"/> that records how often it was called and returns a fixed result.</summary>
public sealed class RecordingRoundingStrategy(decimal result) : IRoundingStrategy
{
    public int CallCount { get; private set; }

    public int? LastDecimals { get; private set; }

    public decimal Round(decimal amount, Currency currency, int? decimals)
    {
        CallCount++;
        LastDecimals = decimals;
        return result;
    }
}
