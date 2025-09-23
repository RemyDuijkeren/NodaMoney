# Feature Proposal: DI & Configuration for Formatting/Parsing Providers

Status: proposal/spec
Source: docs/feature-cldr-formatting-and-parsing.md

## Objective
Provide a simple and explicit way to register and select the active formatting/parsing provider via Microsoft.Extensions.DependencyInjection and MoneyContext options, supporting named contexts and per-scope overrides.

## DI Surface (Possible Implementation)
- services.AddMoneyFormatting() — registers IMoneyFormatter, IMoneyParser, and IMoneyLocalePatternProvider (default flavor).
- Fluent selection:
  - .UseCldr() — registers CldrMoneyLocalePatternProvider (if package referenced) as default.
  - .UseCulture() — registers CultureMoneyLocalePatternProvider.
  - .UseIcu4n() — (optional package) registers ICU provider.
- Named contexts (optional extension): services.AddMoneyFormatting("Sales").UseCldr();

## MoneyContext Integration
- MoneyContextOptions.FormattingProvider: enum { Cldr, Culture, Icu4n } (ICU optional).
- MoneyContext.CreateScope(options => options.FormattingProvider = ...);
- Options binder: allow IConfiguration binding to set defaults (e.g., from appsettings.json).

## Defaults and Probing
- If CLDR package is referenced, default to Cldr; otherwise default to Culture.
- Allow explicit override via DI or options.

## Error Handling
- If chosen provider is not available (missing package), throw during service wiring with a clear message.

## Acceptance Criteria
- Works across DI containers compatible with Microsoft.Extensions.*.
- Supports named contexts or at least per-scope overrides via MoneyContext.CreateScope.
- Clear documentation in docs/README.md and examples.
