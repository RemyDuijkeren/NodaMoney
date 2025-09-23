# Big/Unscaled Amount Type (BigMoney analog)

Why it matters
- Arbitrary-scale intermediates are useful for precise calculations where currency minor units should not be enforced until finalization (Joda-Money BigMoney pattern).

Proposal
- Introduce a BigMoney-like type (name TBD) that does not enforce currency scale during arithmetic. Integrate with MoneyContext for finalization/quantization when converting back to Money.

Possible implementation
- Type design:
  - Immutable struct/class holding decimal (or BigInteger + scale if needed) and CurrencyInfo.
  - Methods for arithmetic that preserve full precision; defer rounding.
  - ToMoney(MoneyContext?) applies context rounding/scale.
- Interop rules:
  - Explicit cast Money -> Big-like preserves numeric value; Big-like -> Money applies rounding.
  - Operators between Money and Big-like promote to Big-like, then allow explicit finalization.
- Context integration:
  - MoneyContext exposes preferred calculation scale/precision for Big-like operations.

Risks / considerations
- Performance and memory if using high precision. Start with decimal until proven insufficient.
- Clear documentation of when rounding happens to avoid surprises.

Open questions
- Do we add analyzers to flag implicit lossy conversions?
- Should Big-like be limited to internal calculations or fully public API?
