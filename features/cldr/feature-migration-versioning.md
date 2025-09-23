# Feature Proposal: Migration & Versioning

Status: proposal/spec
Source: docs/feature-cldr-formatting-and-parsing.md

## Objectives
Introduce the CLDR-aware formatting/parsing capabilities as a non-breaking enhancement, clearly versioned and easy to adopt.

## Release Strategy
- Add new APIs (interfaces, options) in a minor release of NodaMoney core.
- Ship NodaMoney.Globalization.Cldr as a separate package with generated data.
- Optionally ship NodaMoney.Globalization.Icu4n adapter package.

## Version Pinning
- Pin CLDR dataset version (e.g., 46) in the generated code and expose CldrInfo.Version.
- Document update cadence (e.g., quarterly or on-demand) and changelog entries.

## Compatibility
- Money.ToString() remains unchanged; new behavior is opt-in via IMoneyFormatter/IMoneyParser.
- DI defaults: prefer CLDR provider when package referenced; otherwise Culture fallback.
- Parsing and formatting differences vs legacy path are documented with examples.

## Migration Guide (Outline)
- How to reference the CLDR package and enable it via DI.
- How to call the formatter/parser explicitly.
- How to set UseCashDigits and understand effects on rounding and fraction digits.
- Known differences vs CultureInfo-based formatting.

## Acceptance Criteria
- Clear docs in docs/README.md referencing the new features and packages.
- SemVer respected; no breaking changes to public APIs.
- CLDR version surfaced programmatically for diagnostics.
