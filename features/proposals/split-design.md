# Split (Allocation/Proration) – Detailed Design

Goal
- Provide Joda-Money–style allocation/proration with NodaMoney ergonomics via Split(...). Precise, deterministic, context-aware, and fast.

API shape
```csharp
public enum SplitRemainderPolicy
{
    LargestRemainder,
    RoundRobin,
    TowardZero,
    AwayFromZero
}

public static class MoneyExtensions
{
    public static Money[] Split(this Money amount, int parts,
        SplitRemainderPolicy policy = SplitRemainderPolicy.LargestRemainder,
        MoneyContext? context = null);

    public static Money[] Split(this Money amount, params int[] ratios);
    public static Money[] Split(this Money amount, SplitRemainderPolicy policy, params int[] ratios);

    public static Money[] SplitByWeights(this Money amount, ReadOnlySpan<decimal> weights,
        SplitRemainderPolicy policy = SplitRemainderPolicy.LargestRemainder,
        MoneyContext? context = null);

    public static Money[] SplitExact(this Money amount, ReadOnlySpan<Money> partsTemplate);
}
```

Algorithm highlights
- Determine scale via MoneyContext (or currency.MinorUnit). Quantize original amount using context rounding, then operate in smallest units (long) to ensure exactness.
- Initialize base share for each part; distribute remainder units per policy. Support negative totals by adjusting step/sign.

Remainder policies
- LargestRemainder: rank by fractional parts of proportional shares; stable tie-breaking by index.
- RoundRobin: distribute one unit at a time in order.
- TowardZero: leave remainder undistributed (document that sums may differ).
- AwayFromZero: bias to increase magnitude, compensating at the end.

Edge cases
- Zero or near-zero totals; negative amounts; zero ratios; overflow guards for extreme inputs.

Tests
- See features/PossibleFeatures.md Section “Split Design” for xUnit + FluentAssertions examples verifying sums, ordering, negatives, and TowardZero behavior.
