# Project Guidelines

This document summarizes the practical conventions and patterns used in this repository so new contributors can quickly align with the code and tests. It complements (does not replace):
- CONTRIBUTING.md (coding guidelines, contribution workflow)
- Tests README (tests/NodaMoney.Tests/README.md) for unit/integration testing style and examples


## 1. Coding conventions

Authoritative references we follow (see CONTRIBUTING.md):
- Microsoft .NET Framework Design Guidelines
- C# Coding Guidelines (csharpcodingguidelines.com)
- Roslyn Analyzers and code quality rules are enabled in the build for both src projects

Repository‑specific notes:
- Language and compiler settings
  - LangVersion: preview (src/NodaMoney) and latest (src/NodaMoney.DependencyInjection)
  - Nullable reference types: enabled in src projects; tests may opt out for older TFMs
  - Implicit usings: enabled
  - EnforceCodeStyleInBuild: enabled across src projects (warnings become actionable)
- General C# style
  - Use file‑scoped namespaces and modern C# features where available
  - Prefer immutability for domain types (Money, Currency, CurrencyInfo, ExchangeRate)
  - Use readonly/const where appropriate, and expression‑bodied members for simple logic
  - Prefer guard clauses for argument validation; throw ArgumentNullException/ArgumentException accordingly
  - Favor pure functions and side‑effect‑free operators in core domain types
  - Avoid allocation in hot paths; use spans and value types where applicable
- API design
  - Public API aims to be small, clear, and consistent with .NET design guidelines
  - Use exceptions defined in this library (e.g., InvalidCurrencyException, MoneyContextMismatchException) to express domain errors
  - Keep binary/unary operators of Money/FastMoney consistent with .NET numeric types semantics
- Analyzers and quality gates
  - Roslynator.Analyzers is referenced in src projects; respect analyzer suggestions or suppress with justification
  - Code quality rules from Microsoft analyzers are active; fix or justify warnings before PRs
- Internals for testing
  - InternalsVisibleTo is configured for “.Tests” and “Benchmark” to allow thorough testing without expanding public API


## 2. Code organization and package structure

Solution layout is conventional and multi‑package:
- src/NodaMoney (core library)
  - Domain and primitives: Money (split across partial files), FastMoney, Currency, CurrencyInfo, ExchangeRate, MinorUnit, Price, Transaction
  - Context: src/NodaMoney/Context/* contains MoneyContext and rounding strategies (IRoundingStrategy, StandardRounding, NoRounding, CashDenominationRounding), options, and context indexing
  - Serialization: src/NodaMoney/Serialization/* provides Json and Type converters for Money and Currency (System.Text.Json and type converters)
  - Exchange: src/NodaMoney/Exchange/* provides ExchangeRate related types
  - Exceptions: InvalidCurrencyException, MoneyContextMismatchException
  - Organization pattern:
    - Partial classes for Money operators and interfaces: Money.BinaryOperators.cs, Money.UnaryOperators.cs, Money.Comparable.cs, Money.Convertible.cs, Money.Formattable.cs, Money.Parsable.cs, etc.
    - Extension helpers: MoneyExtensions.Split and FiveMostUsedCurrencies convenience helpers
- src/NodaMoney.DependencyInjection (optional package)
  - MoneyContextExtensions: DI / Options integration for Microsoft.Extensions.*
  - Supports configuration binding via IConfiguration, named/unnamed contexts, and AddOptions
- tests/*
  - NodaMoney.Tests: unit and integration‑style tests exercising core library (multiple TFMs)
  - NodaMoney.DependencyInjection.Tests: tests for DI registration and configuration binding
  - Benchmark project (tests/Benchmark) contains performance experiments and reports (not part of CI tests)
- docs/*
  - docs/README.md: user‑facing documentation and usage examples

Namespaces mirror folders:
- NodaMoney for core types (Money, Currency, etc.)
- NodaMoney.Context for MoneyContext and rounding strategies
- NodaMoney.Serialization for converters
- NodaMoney.Exchange for exchange‑related types
- NodaMoney.DependencyInjection for DI extensions

Target frameworks (TFMs):
- Core library: net10.0; net9.0; net8.0; netstandard2.0; netstandard2.1 (AOT‑compatible on .NET 8/9/10)
- DI package: net10.0; net9.0; net8.0; netstandard2.0; netstandard2.1 (AOT‑compatible on .NET 8/9/10)

Packaging notes:
- SourceLink and symbol packages are enabled
- Readme and icon attached to NuGet packages


## 3. Unit and integration testing approaches

See tests/NodaMoney.Tests/README.md for detailed testing guidance, examples, and rationale. Key points summarized below:

Frameworks and libraries:
- xUnit as the test framework
- FluentAssertions for expressive assertions (with analyzers)
- NSubstitute and AutoBogus may be used where needed (AutoBogus via AutoBogus.NSubstitute binding)
- coverlet.collector for code coverage in test runs
- Xunit.SkippableFact to allow conditional skips when needed
- Serialization tests use Newtonsoft.Json, System.Text.Json; RavenDB tests use RavenDB.TestDriver for embedded/integration‑style scenarios

Target frameworks for tests:
- NodaMoney.Tests: net10.0; net9.0; net8.0; net6.0; net48 (to validate both modern runtime and netstandard compatibility)
- NodaMoney.DependencyInjection.Tests: net9.0; net8.0; net6.0; net48

Structure and style:
- Behavior‑driven naming with Given/When/Then conventions
  - Classes express the "Given" (context) and may include method under test and key preconditions
  - Methods express the "When/Then", typically as When[Preconditions]_Should[ExpectedBehavior]
- Arrange‑Act‑Assert pattern within tests to keep structure clear
- Test organization mirrors product namespaces when helpful (e.g., MoneyBinaryOperatorsSpec, MoneyParsableSpec, Serialization/* specs)
- We test behavior and public surface rather than implementation details; private methods are not tested directly

Integration‑style tests:
- Serialization: round‑trip and compatibility checks across serializers (System.Text.Json, Newtonsoft.Json), including converters in NodaMoney.Serialization
- Persistence‑adjacent: RavenDB tests via RavenDB.TestDriver run against an embedded driver for deterministic CI‑friendly execution
- DI: coverage for AddMoneyContext overloads with configuration binding and named contexts in the DependencyInjection package

Running tests:
- Use the standard .NET test runner; multi‑TFM projects select proper test SDK versions by target
- Coverage is collected automatically via coverlet.collector when supported by the runner

Contribution expectations for tests:
- Add or extend tests alongside code changes to keep coverage and verify behavior
- Prefer clear Given/When/Then naming and AAA structure; use FluentAssertions for readable expectations
- Use NSubstitute for doubles only where necessary; prefer testing with real collaborators if practical to reduce brittleness


---
If anything in this document appears out of date, please update this file and, where applicable, the referenced CONTRIBUTING.md and tests README to keep everything aligned.
