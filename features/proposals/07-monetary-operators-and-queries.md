# Monetary Operators and Queries (Composable Pipelines)

Why it matters
- JavaMoney idioms (MonetaryOperator/MonetaryQuery) enable reusable, DI-discoverable transformations (e.g., tax, discount) and queries with metadata.

Proposal
- Support both delegate-based and interface-based operators/queries for ergonomics and extensibility. Provide a small standard library and a Money.With(...) helper.

Possible implementation
- Interfaces:
  - interface IMoneyOperator { Money Apply(in Money value, MoneyContext? context = null); }
  - interface IMoneyQuery<T> { T Evaluate(in Money value, MoneyContext? context = null); }
- Extension helpers:
  - Money With(Func<Money, Money> op) => op(value)
  - Money With(IMoneyOperator op, MoneyContext? ctx = null) => op.Apply(value, ctx)
  - T Query<T>(Func<Money, T> query) / T Query<T>(IMoneyQuery<T> query, MoneyContext? ctx = null)
- Built-ins:
  - readonly struct PercentageOperator : IMoneyOperator { decimal Percent; Apply => value * (1 + Percent/100m); }
  - TaxOperator, DiscountOperator, CapFloorOperator
- DI integration:
  - Register named IMoneyOperator implementations in NodaMoney.DependencyInjection.

Risks / considerations
- Avoid allocations by using readonly structs for built-ins.
- Keep semantics consistent with Money operators and MoneyContext rounding.

Open questions
- Do we need metadata (id/version) on operators for audit?
- Should context be mandatory for certain operators (e.g., cash-specific)?
