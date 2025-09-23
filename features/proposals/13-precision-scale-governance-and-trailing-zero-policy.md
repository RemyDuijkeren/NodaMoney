# Precision/Scale Governance and Trailing Zero Policy

Why it matters
- Display scale (what users see) often differs from calculation scale. Preserving trailing zeros is important for invoices and regulatory documents.

Proposal
- Add display-focused options separate from calculation options: ScaleDisplayPolicy and PreserveTrailingZeros, with per-currency overrides and a quantize toggle distinct from rounding.

Possible implementation
- MoneyContextOptions additions:
  - int? DisplayMinDecimals; int? DisplayMaxDecimals; bool PreserveTrailingZeros; bool QuantizeToCurrencyMinorUnits (display-time only).
  - Optional per-currency override table.
- Formatter integration:
  - MoneyFormatOptions respects these display settings when present; otherwise uses culture defaults.
- Calculation vs display:
  - Keep MaxScale/Precision for calculations. Display settings affect ToString/formatting only.

Risks / considerations
- Avoid confusing users by clearly separating calculation vs display concerns.
- Rounding strategy should remain authoritative for numerical results; display quantization is cosmetic.

Open questions
- Where to house per-currency overrides (config vs code)?
- Should display policies be part of named MoneyContexts or pass via format options only?
