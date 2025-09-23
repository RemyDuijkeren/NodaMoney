# Test Kits and Property-Based Tests

Why it matters
- Mature monetary libraries emphasize correctness through comprehensive tests and invariants. Shipping a test kit helps integrators validate custom providers; property tests catch edge cases.

Proposal
- Provide a small test kit for rounding and exchange providers and add property-based tests for arithmetic/allocations invariants.

Possible implementation
- Test kit:
  - Helpers to validate IRoundingProvider contracts (idempotence, currency-aware scale) and IExchangeRateProvider invariants (inverse consistency, triangulation checks).
  - Sample fixtures: InMemoryExchangeRateProvider usable in tests.
- Property-based tests:
  - Arithmetic invariants under rounding constraints (e.g., associativity where applicable).
  - Allocation invariants: sum(parts) == original (except TowardZero), deterministic remainder distribution.
- CI integration:
  - Cover multiple TFMs; collect coverage via coverlet.collector.

Risks / considerations
- Avoid over-constraining providers that legitimately diverge (e.g., pricing layers).
- Keep tests deterministic and CI-friendly (no network).

Open questions
- Which property testing library to standardize on (FsCheck/QuickCheck-like for C#)?
- Should test kit live under tests/ or be a separate package (NodaMoney.TestKit)?
