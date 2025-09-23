# Deterministic Arithmetic Across Types (Money, FastMoney, Big-like)

Why it matters
- Mixing amount types can lead to surprising results if promotion/demotion and rounding are not clearly defined.

Proposal
- Document and enforce cross-type operation rules: promotion order, rounding points, and context-driven behavior. Add analyzers to guard unsafe implicit conversions.

Possible implementation
- Rules:
  - Promotion order: Money x FastMoney => FastMoney (or Big-like if introduced). Money/FastMoney x Big-like => Big-like.
  - Rounding: apply MoneyContext at finalization (e.g., when creating Money from promoted result) not at every step.
  - Currency checks: operations require same currency unless explicitly converted; zero-amount policy may relax.
- API/Operators:
  - Define explicit/implicit casts where safe. Provide TryDemote for lossy conversions.
- Analyzers:
  - Flag implicit conversions that lose precision; suggest explicit ToMoney(context) or With(context).
- Tests/Docs:
  - Add spec documenting numeric semantics akin to .NET numeric types, plus examples in docs/README.md.

Risks / considerations
- Backward compatibility if existing behavior differs; consider opt-in via analyzer warnings first.

Open questions
- Should promotion prefer precision (Big-like) or performance (FastMoney) by default?
- Provide configuration in MoneyContext to pick a policy?
