# AGENTS.md

Guidance for AI coding agents working in the NodaMoney repository. Agent-neutral: `CLAUDE.md` imports this file.

## Commands

```powershell
dotnet build NodaMoney.slnx -c Release
dotnet test                                  # all test projects, all TFMs
dotnet test -f net10.0                       # one target framework
dotnet test tests/NodaMoney.Tests/NodaMoney.Tests.csproj -f net10.0
dotnet test -f net10.0 --filter "FullyQualifiedName~AddAndSubtractMoney"        # one class
dotnet test -f net10.0 --filter "FullyQualifiedName~AddAndSubtractMoney.AddOperator_ReturnSumMoney"
dotnet pack -c Release -o ./artifacts/
```

Non-obvious build facts:

- **Lock files are committed.** `src/*` set `RestorePackagesWithLockFile` and CI runs in locked mode. After adding or bumping a package, regenerate with `dotnet restore --force-evaluate` and commit the `packages.lock.json` changes, otherwise CI restore fails.
- **`net48` needs Windows.** Test projects target `net11.0;net10.0;net9.0;net8.0;net6.0;net48`. A bare `dotnet test` on Linux fails on the `net48` leg. CI runs on ubuntu and tests net6.0, net8.0, net9.0, net10.0, net11.0 only. The `net6.0` and `net48` legs exist to exercise the `netstandard2.1` and `netstandard2.0` code paths.
- **Versioning is MinVer**, driven by git tags. No version in the csproj.
- `EnforceCodeStyleInBuild` is on with Roslynator plus the Microsoft quality analyzers. The build must be warning-free before a PR.
- `global.json` pins SDK `11.0.100` with `rollForward: latestFeature`, so local and CI restores produce the same lock files.

## Architecture

Two packages: `src/NodaMoney` (core) and `src/NodaMoney.DependencyInjection` (Microsoft.Extensions integration). Both target `net11.0;net10.0;net9.0;net8.0;netstandard2.0;netstandard2.1`, AOT-compatible on the .NET 8/9/10/11 legs. `netstandard` legs pull in `System.Text.Json` and use `#if NET5_0_OR_GREATER` / `DotNetCompatibility.cs` shims, so any new code touching spans, frozen collections or `decimal.GetBits(Span<int>)` needs a fallback.

### The bit-packing contract (read this before touching Money, Currency or MoneyContext)

`Money` is the same size as a `decimal` because it stores the decimal's own three 32-bit mantissa words plus a hand-built flags word (`src/NodaMoney/Money.cs:17`):

| Bits  | Content                                      |
|-------|----------------------------------------------|
| 0-15  | Currency (`Currency.EncodedValue`)           |
| 16-23 | Decimal scale                                |
| 24-30 | MoneyContext index                           |
| 31    | Decimal sign                                 |

Consequences that ripple through the codebase:

- `Currency` packs its ISO 4217 code into 15 bits, 5 bits per letter as A-Z mapped to 1-26, with bit 15 flagging "minor unit is 2" so the common case avoids a registry lookup (`src/NodaMoney/Currency.cs:11`). `XXX` encodes as 0.
- Only 7 bits are left for the context, so **at most 128 `MoneyContext` instances can ever exist in a process** (`src/NodaMoney/Context/MoneyContextIndex.cs`). Contexts are deduplicated on registration and indices are never reclaimed. Do not create contexts per-operation.
- Indices 0-4 are reserved and must line up with the `MidpointRounding` enum values (`MoneyContext`'s static constructor asserts this).

`MoneyContext` is a process-wide static registry (`src/NodaMoney/Context/MoneyContext.cs`) holding active contexts by index and by name, with an `AsyncLocal` thread context layered over a default. `MoneyContext.CurrentContext` is what a `Money` constructor picks up when no context is passed. It carries the rounding strategy, max scale, precision and default currency.

Rounding is applied at construction and after every arithmetic operation, through `IRoundingStrategy` (`StandardRounding`, `NoRounding`, `CashDenominationRounding` in `src/NodaMoney/Context/`). The hot paths switch on the concrete strategy type before falling back to the interface call, deliberately, to avoid the virtual dispatch.

`CurrencyRegistry` (`src/NodaMoney/CurrencyRegistry.cs`) is the static ISO 4217 store behind `CurrencyInfo`, using frozen dictionaries on modern TFMs and a `ReaderWriterLockSlim` plus mutable dictionaries on the netstandard legs. Custom currencies go in through `TryAdd`/`TryRemove`.

`FastMoney` is the alternative representation: a single `long` of minor units at a fixed scale of 4 (`src/NodaMoney/FastMoney.cs:22`). It rejects any context with `MaxScale > 4` or `Precision > 19`, and short-circuits rounding entirely when the context is banker's rounding at scale 4.

### File layout conventions

`Money` and `FastMoney` are split across partial files by concern: `.Constructors`, `.BinaryOperators`, `.UnaryOperators`, `.Comparable`, `.Convertible`, `.Formattable`, `.Parsable`, `.NumericInterfaces`, `.Serializable`. Add an operator or interface implementation to the matching partial rather than to the main file. Namespaces mirror folders: `NodaMoney`, `NodaMoney.Context`, `NodaMoney.Serialization`, `NodaMoney.Exchange`.

`features/proposals/` and `features/cldr/` hold design documents for work not yet built. Treat them as intent, not as a description of current behavior.

## Conventions

External style guides this project follows: the Microsoft .NET Framework Design Guidelines and csharpcodingguidelines.com. See `CONTRIBUTING.md` for the contribution workflow.

Code style:

- File-scoped namespaces, nullable reference types enabled, implicit usings enabled, `LangVersion` latest across all projects.
- Immutable domain types (`Money`, `FastMoney`, `Currency`, `CurrencyInfo`, `ExchangeRate`). `readonly`/`const` where they apply, expression-bodied members for simple logic.
- Guard clauses for argument validation, throwing `ArgumentNullException`/`ArgumentException`. Domain errors use the library's own exceptions (`InvalidCurrencyException`, `MoneyContextMismatchException`).
- Pure, side-effect-free operators. Operator semantics stay consistent with the .NET numeric types.
- No allocation in hot paths: spans and value types.
- Roslynator plus the Microsoft quality analyzers run in the build with `EnforceCodeStyleInBuild`. Fix analyzer findings or suppress with a justification.
- The public API stays small and consistent. Internals are testable through `InternalsVisibleTo` for `*.Tests` and `Benchmark` rather than by widening the public surface.

Tests: xUnit with FluentAssertions, NSubstitute and AutoBogus where doubles or fake data are needed, coverage via coverlet.collector. Named Given/When/Then, with the context in the class name and `When[Precondition]_Should[Behavior]` in the method name. Assert public behavior, not internals. Related specs are grouped in a folder per subject, for example `tests/NodaMoney.Tests/MoneyBinaryOperatorsSpec/`. Full detail in `tests/NodaMoney.Tests/README.md`.

Benchmarks live in `tests/Benchmark` and are not part of CI test runs.
