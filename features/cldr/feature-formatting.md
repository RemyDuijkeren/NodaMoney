# Feature Proposal: Formatting Behavior (CLDR-aligned)

Status: proposal/spec
Source: docs/feature-cldr-formatting-and-parsing.md

## Objective
Produce locale-accurate currency strings aligned with CLDR patterns, spacing rules, sign/accounting behavior, and fraction digit policies.

## Key Behaviors
- Pattern selection: Use standard vs accounting patterns based on SignDisplay and numeric sign.
- Fraction digits: Use CLDR digits (cash vs standard) unless overridden by options (Minimum/MaximumFractionDigits). Optionally apply cash rounding quantum via MoneyContext.
- Number formatting: Honor grouping and decimal symbols for the locale (including Indian 3;2;2 pattern), minimize allocations via span APIs.
- Currency display: Replace ¤ in the pattern with symbol/narrow/code/name per options and locale mappings.
- Currency spacing: Apply before/after rules to insert NBSP/narrow NBSP where digit and currency symbol meet.
- Sign display: Auto/Always/Never/ExceptZero; Accounting uses parentheses where pattern provides a negative subpattern.

## Possible Implementations

### Implementation 1 — Splice CLDR currency layer on top of .NET number formatting
- Format the numeric part using decimal.TryFormat with a NumberFormatInfo configured for the locale’s digits/grouping.
- Inject currency token at ¤ positions and apply spacing rules.
- Pros: Leverages fast .NET base formatting; simpler; reliable across TFMs.
- Cons: Must manage grouping patterns not directly supported in some locales; still need custom spacing/sign logic.

### Implementation 2 — Full custom number formatting for currencies
- Implement grouping and decimal insertion via span algorithms; supports Indian grouping and custom patterns precisely.
- Pros: Maximum control and consistency.
- Cons: More code; higher maintenance risk; ensure perf is competitive.

## Performance Considerations
- Cache compiled patterns by (culture, CurrencyDisplay, SignDisplay, cash flag) using FrozenDictionary or ConditionalWeakTable.
- Avoid string concatenations; use stackalloc and value-type builders where safe.
- Reuse common strings (NBSP, narrow NBSP, symbols) via interning tables from the data generator.

## Edge Cases
- Narrow symbol equals standard or missing → fallback to symbol or code.
- Negative zero display under ExceptZero.
- Large magnitudes and small fractions with TrimTrailingZeros.
- Locales with symbol-after-number patterns and spacing afterCurrency rules.

## Acceptance Criteria
- Outputs match ICU/CLDR for the golden test matrix.
- Deterministic across Windows/Linux/macOS.
- Meets allocation/throughput targets comparable to Money.ToString() within reasonable overhead (<2–3x when CLDR features engaged).
