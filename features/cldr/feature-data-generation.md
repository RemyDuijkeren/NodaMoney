# Feature Proposal: CLDR Data Generation & Packaging

Status: proposal/spec
Source: docs/feature-cldr-formatting-and-parsing.md

## Objective
Provide a compact, deterministic CLDR subset tailored for Money formatting/parsing, compiled into AOT/trimming-friendly assets consumed by the CLDR provider.

## Scope of Data
- CurrencyDigits per ISO code: standard/cash fraction digits and cash rounding quantum.
- Locale currency formatting data: default numbering system, number symbols (decimal, group, plus, minus), currency patterns (standard/accounting), currencySpacing rules.
- Currency display per locale: symbol, narrow symbol, (optionally) localized name.

## Inputs (CLDR JSON)
- cldr-core/supplemental/currencyData.json
- cldr-numbers-full/main/{locale}/numbers.json
- cldr-localenames-full/main/{locale}/currencies.json

## Generation Tool
- Project: tools/NodaMoney.CldrGen (console app)
- Responsibilities:
  1) Resolve CLDR version and locale list; download or read local zips.
  2) Normalize locales (BCP-47) and choose defaultNumberingSystem.
  3) Extract required nodes into intermediate DTOs.
  4) Compact strings (global string table), assign ids for locales/currencies.
  5) Emit runtime artifacts (see packaging options).

## Packaging Options (Possible Implementations)

### Option 1 — Source-generated C# tables (default)
- Emit readonly arrays and FrozenDictionary indexes under src/NodaMoney.Globalization.Cldr/Generated.
- Pros: No runtime IO; great cold-start; very AOT-friendly.
- Cons: Slightly larger IL than binary.

### Option 2 — Compact binary resource + tiny loader
- Emit Resources/CldrData.bin and a generated CldrLoader.g.cs.
- Pros: Small IL; data can be refreshed without regenerating large code tables.
- Cons: More loader complexity.

## AOT/Trimming
- Prefer value types and sealed types; avoid reflection.
- Use FrozenDictionary on net8+/net9+/net10 with conditional fallbacks.
- Generate parent-locale fallback map statically.

## Versioning
- Pin a CLDR version (e.g., 46) and embed it into generated code (CldrInfo.Version).
- Document update cadence; provide a generator command to refresh.

## Acceptance Criteria
- Generator produces deterministic output from the same CLDR inputs.
- Runtime provider loads data without allocations beyond necessary strings.
- Works across TFMs listed in repo guidelines.
