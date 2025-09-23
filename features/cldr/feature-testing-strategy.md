# Feature Proposal: Testing Strategy for CLDR Formatting/Parsing

Status: proposal/spec
Source: docs/feature-cldr-formatting-and-parsing.md

## Objectives
Validate correctness, determinism, and performance of the formatting/parsing providers across locales, currencies, and platforms.

## Test Types
- Golden output tests (formatter): compare outputs against ICU (ICU4N in test scope or pre-recorded fixtures) for a matrix of locales and currencies.
- Round-trip tests: format with CLDR provider, parse back, assert equality (amount and currency).
- Determinism tests: run on Windows/Linux/macOS to ensure identical outputs.
- Cash vs standard digits: verify CHF (0.05 cash), JPY (0), SEK cases with accounting pattern.
- Parsing robustness: handle NBSP/narrow NBSP, symbol vs ISO code, parentheses, and ambiguous symbols resolved by locale.
- Performance/allocations: microbenchmarks to track regression (BenchmarkDotNet; not required in CI but useful locally).

## Test Layout
- tests/NodaMoney.Tests/Formatting/* — unit/integration tests for public surface.
- tests/NodaMoney.Globalization.Cldr.Tests/* — provider-specific golden comparisons.

## Data for Golden Tests
- Record expected ICU outputs for specific CLDR version; store as fixtures pinned to the same version.
- Update fixtures only when CLDR version changes; review diffs.

## Acceptance Criteria
- All tests pass across supported TFMs.
- Coverage includes formatting and parsing edge cases listed in proposals.
- Determinism confirmed across OSes in CI matrix or via conditional runs.
