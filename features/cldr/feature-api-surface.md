# Feature Proposal: Public API Surface for CLDR Formatting/Parsing

Status: proposal/spec (non-breaking)
Source: docs/feature-cldr-formatting-and-parsing.md

## Objective
Introduce a small, clear, and AOT-friendly API surface to opt-in to CLDR-accurate formatting and parsing without changing existing Money.ToString() behavior.

## Proposed APIs

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
    public string? NumberingSystem { get; init; }
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

## Alternatives Considered
- Add overloads on Money.ToString(MoneyFormatOptions options):
  - Pros: discoverable; fewer registrations.
  - Cons: Pulls formatting concerns into the core type; risks public API bloat and confusion with IFormattable.
- Use AmountFormatQuery-like builder:
  - Pros: Fluent; extensible attribute bag.
  - Cons: More allocation; less explicit than a strongly-typed options class.

## Possible Implementations
- Core (src/NodaMoney): define enums, options, and interfaces only (no behavior).
- Providers/packages implement IMoneyLocalePatternProvider and the concrete IMoneyFormatter/IMoneyParser.
- Add extension methods for ergonomics in a separate static class if desired (e.g., MoneyExtensions.ToString(options)).

## Backward Compatibility
- No changes to existing ToString or parsing behavior.
- All new APIs are additive and optional; legacy code remains unaffected.

## Acceptance Criteria
- APIs compile across TFMs listed in repo guidelines.
- Nullable reference annotations included; analyzers pass.
- Surface is minimal yet expressive enough for CLDR alignment.
