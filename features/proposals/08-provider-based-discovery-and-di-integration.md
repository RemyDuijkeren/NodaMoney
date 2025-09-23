# Provider-Based Discovery and DI Integration

Why it matters
- In .NET, DI is the idiomatic alternative to Java SPI. Registering rounding providers, exchange providers, formatters, and operators enables modular, testable setups.

Proposal
- Extend NodaMoney.DependencyInjection to register named MoneyContext instances and provider chains (rounding, FX, formatters, operators) with options binding and AOT-friendly configuration.

Possible implementation
- Registration extensions:
  - IServiceCollection AddMoneyContext(name, Action<MoneyContextOptions>) with options binding from IConfiguration.
  - IServiceCollection AddRoundingProvider(name, IRoundingProvider) and AddCompositeRoundingProvider(...)
  - IServiceCollection AddExchangeRates(Func<IServiceProvider, IExchangeRateProvider>) with composite/cached decorators.
  - IServiceCollection AddMoneyOperator(string name, IMoneyOperator)
  - Named registrations resolved via keyed services or IOptionsMonitor keyed by name.
- Configuration binding:
  - Use Options pattern to bind MoneyContextOptions (precision, scale, default currency, flags) per name.
  - Allow provider chains to be described in config (ids, priorities), while actual implementations are registered in code.

Risks / considerations
- Avoid service locator anti-pattern; expose typed facades where possible (e.g., ICurrencyConverter).
- Keep AOT-friendly (no reflection-based discovery by default).

Open questions
- Do we want a default singleton registry for providers outside DI usage?
- How to scope provider lifetimes when MoneyContext scopes are created transiently?
