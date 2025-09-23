# Allocation / Proration APIs (Split)

Why it matters
- Deterministic allocation of amounts into equal or ratio-based parts while preserving totals is a common need (Joda-Money allocate/allocateByRatios).

Proposal
- Provide first-class Split APIs as instance/extension methods on Money and FastMoney with policy-driven remainder handling and MoneyContext-aware rounding.

Possible implementation
- Methods:
  - Money[] Split(int parts, SplitRemainderPolicy policy = LargestRemainder, MoneyContext? context = null)
  - Money[] Split(SplitRemainderPolicy policy, params int[] ratios)
  - Money[] SplitByWeights(ReadOnlySpan<decimal> weights, SplitRemainderPolicy policy = LargestRemainder, MoneyContext? context = null)
  - Money[] SplitExact(ReadOnlySpan<Money> partsTemplate)
- Policies:
  - LargestRemainder (default), RoundRobin, TowardZero, AwayFromZero.
- Algorithm:
  - Quantize original amount using context scale, convert to smallest units (long), distribute base units + remainders deterministically.
- Integration:
  - Works for negative amounts; uses MoneyContext.RoundingStrategy.

API sketches and tests
- See features/proposals/split-design.md for detailed algorithms and unit test examples.

Risks / considerations
- Overflow if parts/ratios extremely large; add guards.
- Document TowardZero behavior (sum(parts) != original).

Open questions
- Should zero-ratio indices be excluded from remainder distribution?
- Provide span-based overloads to reduce allocations?
