# Formatting and Parsing – Enhancements and Options

Summary
- NodaMoney already supports rich formatting via CurrencyInfo and format specifiers (G, C, R, N, L, l, with precision). This proposal surfaces advanced policies as explicit options/presets and strengthens parsing for tricky locale cases.

Formatting enhancements
- Options (MoneyFormatOptions):
  - IFormatProvider? Culture; bool UseIsoCode; bool UseSymbol; SymbolPlacement (Before|After);
  - SpacePolicy (None|Normal|NonBreaking|NarrowNonBreaking);
  - NegativeStyle (Minus|Parentheses|TrailingMinus);
  - int? MinDecimals; int? MaxDecimals; bool PreserveTrailingZeros;
  - bool QuantizeToCurrencyMinorUnits = true; bool CashDisplay.
- Presets:
  - IsoBefore(culture), SymbolAfter(culture), AccountingNegatives(culture), CashReceipt(culture)
- Policies:
  - CashDisplay uses CashDenominationRounding for display only; no mutation of stored amount.
  - CLDR-aligned spacing and symbol placement where feasible.

Parsing enhancements
- Options (MoneyParseOptions):
  - IFormatProvider? Culture; CurrencyInfo? DefaultCurrency; bool Strict;
  - SymbolResolutionPolicy (Fail|CultureBased|PreferList); IReadOnlyList<string>? PreferredCurrencies;
  - ZeroCurrencyPolicy (Ignore|RequireMatch|InferDefault).
- Acceptance rules:
  - Parentheses for negatives; trailing minus; leading minus; normalize Unicode minus.
  - Accept various grouping separators: apostrophe (CHF), thin space, narrow/non‑breaking spaces.
  - Handle glued codes: "USD1,234.56" and "1,234.56USD".
  - Recognize US$, CA$, A$ variants; choose per policy and culture.
  - Localized digits (optional) for cultures that use non‑Latin numerals.
- Diagnostics:
  - Optionally return reason codes on TryParse failure for better UX when Strict=true.

Integration
- Extend Money.Parse/TryParse overloads to accept MoneyParseOptions; default to MoneyContext.CurrentContext.Culture when options not specified.
- CurrencyInfo formatter can accept MoneyFormatOptions presets; map existing specifiers to equivalent presets for discoverability.
- Symbol disambiguation consults MoneyContext.DefaultCurrency and MoneyContext.RegionalCurrencyPriority before failing.

Test matrix (examples)
- Formatting:
  - Accounting negatives: (€ 1,234.56), (1 234,56 €), (USD 1,234.56)
  - Spaces: NonBreaking vs NarrowNonBreaking in fr‑FR and fr‑CA
  - PreserveTrailingZeros: JPY 12 vs JPY 12.00 (MinDecimals=2)
  - CashDisplay: CHF 1.23 → CHF 1.25 display
- Parsing:
  - (USD 1,234.56), 1 234,56-, -1 234,56 €
  - CHF 1'234.50, 1 234,56 € (narrow NBSP), $1,234.56 in en‑CA mapping to CAD
  - USD1,234.56 / 1,234.56USD (no space)
  - ZeroCurrencyPolicy cases: "0.00" with/without default currency

Notes
- This proposal complements features/proposals/03-ambiguous-currency-symbol-policies.md and features/proposals/10-culture-locale-sensitive-defaults-and-policies.md.
