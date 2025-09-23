# Validation Policies for Zero Amounts and Currency Checks

Why it matters
- Your TODO mentions strict vs relaxed zero-currency validation. Consistent policy across parsing, comparisons, and arithmetic avoids surprises.

Proposal
- Add ZeroCurrencyPolicy (Ignore | RequireMatch | InferDefault) and integrate with MoneyContext.EnforceZeroCurrencyMatching and parsing/formatting behaviors.

Possible implementation
- MoneyContextOptions:
  - ZeroCurrencyPolicy enum; EnforceZeroCurrencyMatching becomes a derived convenience.
- Parsing:
  - When amount is zero and currency missing/ambiguous: follow policy (fail, infer DefaultCurrency, or ignore).
- Comparisons/arithmetic:
  - For zero amounts across different currencies, allow comparisons or throw per policy.
- Analyzer hints:
  - Optional Roslyn analyzer to flag ambiguous zero-currency operations in strict mode.

Risks / considerations
- Backward compatibility with existing zero handling.
- Document behavior in docs/README.md clearly.

Open questions
- Should policy differ for serialization vs user input parsing?
- Do we allow per-operation overrides?
