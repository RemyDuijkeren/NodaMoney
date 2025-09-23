# Feature Proposal: Parsing Behavior (CLDR-aligned)

Status: proposal/spec
Source: docs/feature-cldr-formatting-and-parsing.md

## Objective
Parse locale-formatted currency strings into Money with robust handling of symbols, codes, spacing, grouping, and accounting parentheses per CLDR.

## Key Behaviors
- Locale-aware tokenization: recognize decimal/group symbols (including NBSP, narrow NBSP), plus/minus, parentheses.
- Currency resolution: longest-match policy across per-locale tries of tokens: symbol, narrow symbol, and ISO code; fallback to locale defaults for ambiguous symbols (e.g., $).
- Sign handling: detect accounting (parentheses) and explicit minus; support SignDisplay rules where relevant.
- Grouping validation: lenient by default; optionally strict in future options.
- Fraction digits: accept inputs with varying fraction digits up to a reasonable maximum; rounding handled by MoneyContext afterward if needed.

## Possible Implementations

### Implementation 1 — Table-driven tokenizer + tries
- Prebuild per-locale token tries (from generated data) for symbol/narrow/ISO.
- Scan the input with a small DFA, classifying whitespace, digits, separators, and currency tokens.
- Pros: Fast and deterministic; robust to multiple whitespace kinds.
- Cons: More code; must maintain per-locale token tables.

### Implementation 2 — Regex-assisted parsing (limited)
- Use compiled regex per-locale to split currency token and numeric part; post-process per rules.
- Pros: Rapid prototype.
- Cons: Harder to keep AOT-friendly and allocation-lean; brittle for edge cases.

## Ambiguity and Policies
- When symbol is ambiguous, prefer the currency associated with the locale (e.g., en-CA → CAD). Expose override hooks in MoneyFormatOptions (future) or ambient MoneyContext default currency.
- If currency is missing and amount is zero, respect EnforceZeroCurrencyMatching policy from MoneyContext.

## Error Handling
- TryParse returns false for unrecognized tokens or invalid number formats.
- Provide best-effort diagnostics internally for test assertions (not public API).

## Acceptance Criteria
- Round-trip: values formatted by CLDR formatter parse back to identical Money for tested locales.
- Golden parsing tests: real-world variants (with/without spaces, narrow NBSP, symbol vs code) succeed.
- Deterministic results across platforms.
