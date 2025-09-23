# CLDR Formatting & Parsing — Feature Proposals (Index)

This folder contains per-feature proposals split out from the consolidated spec at the feature specified below. Each document describes the feature intent, rationale, and possible implementations, aligned with the repository’s guidelines and AOT/trimming goals.

Documents:
- feature-provider-model.md — Provider model and selection (CLDR subset, ICU adapter, Culture fallback)
- feature-api-surface.md — Public API surface (interfaces and options)
- feature-data-generation.md — CLDR data subset and generation/packaging
- feature-formatting.md — Formatting behavior and implementation options
- feature-parsing.md — Parsing behavior and implementation options
- feature-di-and-configuration.md — DI integration and MoneyContext configuration
- feature-aot-trimming.md — AOT/trimming considerations and multi-TFM notes
- feature-testing-strategy.md — Testing strategy and golden comparisons
- feature-migration-versioning.md — Migration, versioning, and compatibility
- feature-roadmap.md — Roadmap and milestones
- feature-examples.md — Usage examples and snippets

# CLDR-based currency formatting and parsing for NodaMoney

This document consolidates prior design notes and proposes a single, actionable specification for adding locale‑aware currency formatting and robust parsing to NodaMoney, driven by CLDR data. The feature is designed to be deterministic across platforms, AOT‑friendly, and opt‑in via a small public API surface.

Status: proposal/spec (non‑breaking feature)

CLDR version baseline: to be pinned at package generation time (e.g., 46)

---

## Summary

Goal: Provide accurate, locale‑aware currency formatting and parsing aligned with CLDR (via ICU data), including:
- Currency display variants: symbol, narrow symbol, ISO code, and localized name
- Currency spacing rules (before/after symbol)
- Sign display policies including accounting (parentheses)
- Cash vs standard fraction digits and optional cash rounding quantum
- Locale number symbols and grouping patterns, per numbering system (initially default system per locale)

Design choice: Hybrid provider model
- Default provider: a lightweight CLDR subset compiled into code (no heavy runtime dependency)
- Optional fallback provider: best‑effort CultureInfo‑based formatting when size trumps fidelity
- Optional adapter (future): ICU4N‑based provider for strict ICU parity

Motivation: CultureInfo/NumberFormatInfo alone do not expose all CLDR features (spacing rules, narrow symbols, accounting patterns, cash digits). Platform ICU versions can change behavior; we want deterministic, version‑pinned outputs.

---

## Scope and non‑goals

In scope (v1):
- Formatting with symbol placement and spacing, standard vs accounting patterns
- Sign display policies (Auto/Always/Never/ExceptZero/Accounting)
- Currency display selection (Symbol/NarrowSymbol/Code/Name)
- Fraction digits selection (standard vs cash), optional trimming of trailing zeros
- Parsing of formatted text honoring locale separators, spacing, accounting parentheses; resolve currency via locale‑aware tokens (symbol, narrow, ISO code)

Not in scope (v1):
- Full plural rules for spelled‑out unit names in sentences (beyond providing a localized currency display name string)
- Multiple numbering systems per locale (initial release uses defaultNumberingSystem; extensible later)
- Historical legal‑tender windows enforcement during parsing

---

## Public API surface (non‑breaking additions)

```csharp
public enum CurrencyDisplay { Symbol, NarrowSymbol, Code, Name }
public enum SignDisplay { Auto, Always, Never, ExceptZero, Accounting }

public sealed class MoneyFormatOptions
{
    public CultureInfo Culture { get; init; } = CultureInfo.InvariantCulture;
    public CurrencyDisplay CurrencyDisplay { get; init; } = CurrencyDisplay.Symbol;
    public SignDisplay SignDisplay { get; init; } = SignDisplay.Auto;
    public bool UseCashDigits { get; init; }
    public int? MinimumFractionDigits { get; init; }
    public int? MaximumFractionDigits { get; init; }
    public string? NumberingSystem { get; init; } // optional; default from locale
    public bool Grouping { get; init; } = true;
    public bool TrimTrailingZeros { get; init; }
}

public interface IMoneyLocalePatternProvider
{
    CurrencyPattern GetPattern(CultureInfo culture);
    CurrencyDisplayInfo GetCurrencyDisplayInfo(CultureInfo culture, Currency currency);
    CurrencyDigits GetDigits(Currency currency, bool cash);
}

public interface IMoneyFormatter
{
    string Format(Money money, MoneyFormatOptions options);
}

public interface IMoneyParser
{
    bool TryParse(ReadOnlySpan<char> input, CultureInfo culture, out Money result, MoneyFormatOptions? options = null);
}
```

Notes:
- Money.ToString() existing behavior remains unchanged. The new behavior is opt‑in via the new interfaces/APIs.
- DI integration is provided to register a default provider/formatter/parser.

---

## Providers and selection

Providers:
- CldrMoneyLocalePatternProvider (default): Implements CLDR‑accurate patterns, spacing rules, display variants, and digits using an embedded compact CLDR subset. Deterministic across platforms.
- CultureMoneyLocalePatternProvider (fallback): Best‑effort mapping using CultureInfo/NumberFormatInfo; no currencySpacing beyond standard whitespace; no narrow symbols. For minimal installs.
- (Optional) Icu4nMoneyLocalePatternProvider: Adapter to ICU4N for strict ICU behavior where desired.

Configuration:
- MoneyContext and NodaMoney.DependencyInjection will expose options to choose the active provider:
  - MoneyContextOptions.FormattingProvider = Cldr | Culture (ICU optional)
  - DI helpers: services.AddMoneyFormatting().UseCldr() or .UseCulture()

Parent fallback:
- If a specific culture (xx-YY) is missing in the CLDR subset, fall back to its parent (xx), then to a configured default (e.g., en).

---

## Data model and generation (CLDR subset)

Data sources (CLDR JSON):
- cldr-core/supplemental/currencyData.json → fraction digits and cash policies per ISO
- cldr-numbers-full/main/{locale}/numbers.json → defaultNumberingSystem, number symbols, currency patterns (standard/accounting), currencySpacing rules
- cldr-localenames-full/main/{locale}/currencies.json → localized currency symbol, narrow symbol, name

Compact runtime schema (conceptual):
- CurrencyDigits: StandardFractionDigits, CashFractionDigits, CashRoundingQuantum
- LocaleNumberSymbols: decimal, group, plus, minus, percent
- CurrencySpacing: before/after currency matchers and insertBetween (simplified matching rules)
- CurrencyPatterns: standard, accounting
- LocaleCurrencyFormatData: per locale (and numbering system) bundle of symbols, spacing, and patterns
- LocaleCurrencyDisplay: (locale, currency) → symbol, narrow, name

Generation tool (outside runtime): tools/NodaMoney.CldrGen
- Inputs: CLDR version, locale list (or modern coverage), currency universe
- Process: normalize locales, extract data, compact strings with deduplication, assign ids
- Outputs: generated C# tables (default) or a compact binary blob + tiny loader
- AOT/trimming: Prefer source‑generated code with FrozenDictionary on net8+/9+/10; safe fallbacks for older TFMs

---

## Formatting behavior (high level)

- Choose pattern: standard vs accounting based on SignDisplay and sign of the value
- Determine fraction digits: from CLDR digits (cash or standard) unless overridden by Minimum/MaximumFractionDigits; optionally apply cash rounding quantum via MoneyContext/IRoundingStrategy when UseCashDigits
- Format the numeric part using span‑based APIs; honor grouping per locale (including Indian 3;2;2 pattern)
- Resolve currency display token (Symbol/Narrow/Code/Name) and substitute for ¤ in the pattern
- Apply currencySpacing rules: insert NBSP or narrow NBSP as specified when symbol abuts digits or signs per CLDR
- Apply SignDisplay policies: Auto/Always/Never/ExceptZero; Accounting may use parentheses

Performance:
- Cache compiled patterns per (culture, display, sign mode, cash) using FrozenDictionary or ConditionalWeakTable
- Avoid intermediate strings; use stackalloc/Span where reasonable

---

## Parsing behavior (high level)

- Tokenize input honoring locale decimal/group symbols; tolerate whitespace variants (space, NBSP, narrow NBSP)
- Detect accounting parentheses for negative values
- Resolve currency by longest‑match against per‑locale tries of tokens (symbol, narrow, ISO code); disambiguate using locale defaults when necessary
- Validate grouping leniently by default; strict mode may be added later via options
- Produce Money with parsed amount and resolved Currency

---

## Testing strategy

- Golden tests against ICU outputs for a matrix of locales (en-US, fr-FR, nl-NL, de-CH, ar-EG, hi-IN, ja-JP, etc.)
- Cross‑platform determinism tests (Windows/Linux/macOS)
- Round‑trip: format then parse across locales/currencies
- Cash vs standard digits tests (CHF, JPY, SEK, etc.) including accounting style negatives

---

## Migration and versioning

- Ship as a non‑breaking minor release introducing new interfaces and options
- Publish NodaMoney.Globalization.Cldr with an explicit CldrVersion and documented update cadence (e.g., quarterly)
- Document behavior differences vs current Money.ToString() and guidance to opt into CLDR formatting

---

## Example usage

```csharp
var options = new MoneyFormatOptions
{
    Culture = CultureInfo.GetCultureInfo("nl-NL"),
    CurrencyDisplay = CurrencyDisplay.Symbol,
    SignDisplay = SignDisplay.Accounting,
    UseCashDigits = true
};

IMoneyFormatter formatter = services.GetRequiredService<IMoneyFormatter>();
var text = formatter.Format(new Money(1234.5m, Currency.EUR), options); // "€\u00A01.234,50" (NBSP)

IMoneyParser parser = services.GetRequiredService<IMoneyParser>();
if (parser.TryParse("$\u00A0(1,234.50)", CultureInfo.GetCultureInfo("en-US"), out var amount,
    new MoneyFormatOptions { SignDisplay = SignDisplay.Accounting }))
{
    // amount == -1234.50 USD
}
```

---

## Rationale: Why not CultureInfo alone?

- API exposure: NumberFormatInfo does not expose CLDR currencySpacing rules, narrow symbols, accounting patterns, or cash digits uniformly across platforms
- Determinism: Outputs can change with OS ICU upgrades; pinning a CLDR version in our subset provides stable behavior
- Feature completeness: The target features require CLDR details not available through CultureInfo

Fallback guidance: CultureMoneyLocalePatternProvider can be used for minimal installs or diagnostics where exact CLDR fidelity is not critical.

---

## Project layout (proposed)

- src/
  - NodaMoney/ (core; defines abstractions and options)
  - NodaMoney.Globalization.Cldr/ (provider + generated data)
    - CldrMoneyLocalePatternProvider.cs
    - Generated/
      - CldrLocales.g.cs
      - CldrCurrencyDigits.g.cs
      - CldrCurrencyDisplay.g.cs
    - (or) Resources/ + loader if binary format is chosen
- tools/
  - NodaMoney.CldrGen/ (offline generator)
- tests/
  - NodaMoney.Tests/Formatting/* (unit/integration)
  - NodaMoney.Globalization.Cldr.Tests/* (golden output comparisons)

---

## Roadmap

1) MVP: a handful of locales (en-US, nl-NL, fr-FR, de-DE, ja-JP) and currencies (USD, EUR, CHF, JPY); formatting incl. spacing + accounting; codegen tables
2) Expand locales/currencies; add parsing; introduce cash digits/quantum integration with MoneyContext rounding
3) Optional binary data format for smaller IL; add ICU adapter package if demanded
4) CI automation for CLDR regeneration; expose CldrVersion programmatically

---

## Notes and gotchas

- Spacing rules: start with common defaults (insert NBSP between currency symbol and digits when adjacent); advanced regex from CLDR can be simplified while preserving correctness for most locales
- Symbol ambiguity ("$"): rely on locale to disambiguate (en-US → USD, en-CA → CAD); consider option hooks for overrides
- Narrow symbols may be missing or same as standard; implement graceful fallback
- Indian grouping (3;2;2…) needs explicit handling or .NET support where available
- Historical currencies exist in CLDR; filter to active ISO set by default, allow opt‑in later

---

## References

- CLDR JSON repositories: cldr-core, cldr-numbers-full, cldr-localenames-full
- ICU number formatting concepts: currency spacing, accounting patterns
- .NET globalization notes: ICU vs NLS backends; CultureInfo/NumberFormatInfo limitations

---

This hybrid design gives CLDR fidelity without heavy dependencies, keeps the core API small and AOT‑friendly, and allows consumers to choose between deterministic CLDR behavior and a minimal CultureInfo fallback.

