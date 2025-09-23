# Ambiguous Currency Symbol Policies (Parsing & Formatting)

Why it matters
- Many currencies share symbols (e.g., "$"). Disambiguation should be deterministic, culture-aware, and optionally policy-driven. Also relates to zero-amount validation.

Proposal
- Introduce MoneyFormatOptions and MoneyParseOptions with explicit symbol resolution, strictness, and zero-currency policies. Tie to MoneyContext.DefaultCurrency and optional PreferredCurrencies per context.

Possible implementation
- New option records:
  - MoneyFormatOptions: Culture, UseIsoCode, UseSymbol, SymbolPlacement, SpacePolicy, NegativeStyle, Min/MaxDecimals, PreserveTrailingZeros, QuantizeToCurrencyMinorUnits, CashDisplay.
  - MoneyParseOptions: Culture, DefaultCurrency, Strict, SymbolResolutionPolicy (Fail|CultureBased|PreferList), PreferredCurrencies, ZeroCurrencyPolicy (Ignore|RequireMatch|InferDefault).
- Parsing rules:
  - If symbol ambiguous, resolve by: PreferredCurrencies > Culture region mapping > MoneyContext.DefaultCurrency > fail (if Strict).
  - Recognize US$, CA$, A$ variants, trailing minus, parentheses, Unicode spaces/digits.
- Formatting rules:
  - Presets for ISO vs symbol placement; provide NonBreaking/Narrow spaces where CLDR suggests.
  - CashDisplay toggle routes through CashDenominationRounding for display only.
- Integration:
  - Extend Money.Parse/TryParse overloads to accept MoneyParseOptions.
  - CurrencyInfo formatter to accept MoneyFormatOptions presets.

Risks / considerations
- Culture-specific defaults must be well-documented.
- Ensure backward compatibility with existing single-letter format specifiers.

Open questions
- Should PreferredCurrencies be part of MoneyContextOptions for global policy?
- Do we need diagnostics (reason codes) on TryParse failure?
