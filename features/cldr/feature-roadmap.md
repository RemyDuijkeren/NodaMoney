# Feature Proposal: Roadmap & Milestones (CLDR Formatting/Parsing)

Status: proposal/spec
Source: docs/feature-cldr-formatting-and-parsing.md

## Phase 1 — MVP
- Locales: en-US, nl-NL, fr-FR, de-DE, ja-JP
- Currencies: USD, EUR, CHF, JPY
- Deliverables:
  - Data generator (codegen tables)
  - CldrMoneyLocalePatternProvider
  - Formatting: symbol placement, spacing, fraction digits, accounting sign
  - Tests: golden vs ICU, round-trips

## Phase 2 — Parsing & Coverage Expansion
- Add parsing with token tries; ambiguous symbol policies
- Expand locales and currencies; add Indian grouping
- Integrate UseCashDigits with MoneyContext rounding

## Phase 3 — Optimization & Options
- Perf and allocation tuning; caching compiled patterns
- Optional binary data format; feature flags for including currency names

## Phase 4 — Ecosystem Options
- Optional ICU4N adapter package
- DI ergonomics and configuration binding samples

## Phase 5 — Automation & Release
- CI job to regenerate CLDR data on-demand
- Expose CldrInfo.Version; document update cadence and changelog
