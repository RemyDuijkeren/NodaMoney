# Culture/Locale-Sensitive Defaults and Policies

Why it matters
- Locale affects symbol placement, spacing, and default currency resolution. Surfacing culture in MoneyContext improves formatting/parsing defaults and symbol disambiguation.

Proposal
- Allow MoneyContextOptions to carry Culture/IFormatProvider and a regional currency priority list that formatters/parsers consult by default.

Possible implementation
- MoneyContextOptions additions:
  - IFormatProvider? Culture; IReadOnlyList<string>? RegionalCurrencyPriority.
- Formatter/parser integration:
  - MoneyFormatOptions/MoneyParseOptions default to context Culture when unspecified.
  - Parsing of ambiguous symbols consults RegionalCurrencyPriority.
- Documentation:
  - Matrix of behaviors by culture and specifiers; guidance on space policies (non-breaking, narrow, etc.).

Risks / considerations
- Culture changes at runtime: clarify whether MoneyContext captures a snapshot or references a mutable CultureInfo.
- Ensure thread-safety when using culture-derived caches.

Open questions
- Should DefaultCurrency be inferred from Culture when not set?
- Do we expose helpers to generate priority lists from RegionInfo?
