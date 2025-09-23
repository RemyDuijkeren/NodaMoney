# Feature Proposal: Provider Model for CLDR-based Formatting & Parsing

Status: proposal/spec (non-breaking)
Source: docs/feature-cldr-formatting-and-parsing.md

## Problem
NodaMoney needs deterministic, locale-accurate currency formatting and parsing aligned with CLDR. Relying only on CultureInfo yields gaps (no currencySpacing, accounting patterns, narrow symbols) and platform variance.

## Goals
- Deterministic behavior across platforms by pinning a CLDR version.
- Keep core library lean and AOT-friendly.
- Allow consumers to choose between a lightweight CLDR subset, an ICU-based adapter, or a minimal Culture fallback.

## Options (Possible Implementations)

### Approach A — ICU adapter (ICU4N)
- Implement Icu4nMoneyLocalePatternProvider using ICU4N for data and formatting.
- Pros: Maximum CLDR fidelity; low maintenance of rules.
- Cons: Larger dependency; potential AOT/trimming issues; version coupling.
- When to use: Enterprises requiring exact ICU parity.

### Approach B — Curated CLDR subset (default)
- Implement CldrMoneyLocalePatternProvider with embedded compact tables generated from CLDR JSON.
- Pros: Deterministic, lightweight, AOT-friendly. No heavy runtime deps.
- Cons: Own a small data refresh pipeline.
- When to use: Default for most users.

### Approach C — Culture-based fallback
- Implement CultureMoneyLocalePatternProvider using CultureInfo/NumberFormatInfo.
- Pros: Zero additional data/deps; smallest footprint.
- Cons: Feature gaps vs CLDR; platform variance.
- When to use: Constrained environments; diagnostic output.

### Approach D — Hybrid model (recommended)
- Ship B as default, C as fallback, and A as optional adapter.
- Select provider via DI/MoneyContext options; allow per-scope overrides.

## Selection and Resolution
- DI extension: services.AddMoneyFormatting().UseCldr() | .UseCulture() | .UseIcu4n()
- MoneyContextOptions.FormattingProvider: enum Cldr | Culture | Icu4n (optional)
- Scope override: using MoneyContext.CreateScope(options => options.FormattingProvider = ...)
- Culture parent fallback: if xx-YY missing, try xx, else configured default (e.g., en).

## Non-Goals
- Forcing ICU on all users.
- Embedding full CLDR datasets (only a tiny subset needed).

## Risks & Mitigations
- Data drift (CLDR updates): Pin version; provide generator tool; document cadence.
- Ambiguous symbols ($): Resolve by locale; expose override hooks in options.

## Acceptance Criteria
- Pluggable providers behind a single IMoneyLocalePatternProvider abstraction.
- Default behavior deterministic across platforms when CLDR provider is active.
- DI/Options allow easy selection and scoping.
