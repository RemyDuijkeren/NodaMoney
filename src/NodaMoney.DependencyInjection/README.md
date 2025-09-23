NodaMoney Dependency Injection Extensions
=========================================

[![NuGet](https://img.shields.io/nuget/dt/NodaMoney.DependencyInjection.svg?logo=nuget)](https://www.nuget.org/packages/NodaMoney.DependencyInjection)
[![NuGet](https://img.shields.io/nuget/v/NodaMoney.DependencyInjection.svg?logo=nuget)](https://www.nuget.org/packages/NodaMoney.DependencyInjection)
[![Pre-release NuGet](https://img.shields.io/github/v/tag/RemyDuijkeren/NodaMoney?label=pre-release%20nuget&logo=github)](https://github.com/users/RemyDuijkeren/packages/nuget/package/NodaMoney.DependencyInjection)
[![CI](https://github.com/RemyDuijkeren/NodaMoney/actions/workflows/ci.yml/badge.svg)](https://github.com/RemyDuijkeren/NodaMoney/actions/workflows/ci.yml)

About
-----
This package provides extension methods to easily integrate NodaMoney's MoneyContext with .NET's dependency injection system.

Usage
-----

```csharp
// Add default MoneyContext with standard configuration
services.AddMoneyContext();

// Access the MoneyContext in your services
public class PaymentService
{
    private readonly MoneyContext _moneyContext;

    public PaymentService(MoneyContext moneyContext)
    {
        _moneyContext = moneyContext;
    }

    // Use _moneyContext for calculations
}
```

Configuration Options
---------------------

NodaMoney provides several ways to configure the MoneyContext, following Microsoft's recommended patterns for library authors.

### Using Action-based Configuration

```csharp
services.AddMoneyContext(options => {
    options.DefaultCurrency = Currency.FromCode("USD");
    options.RoundingStrategy = new HalfUpRounding();
    options.Precision = 28;
    options.MaxScale = 2;
});
```

### Using Configuration System

```csharp
// Option 1: Using pre-scoped configuration section
var moneyContextSection = Configuration.GetSection("MoneyContext");
services.AddMoneyContext(moneyContextSection);

// Option 2: Using root configuration with section path
services.AddMoneyContext(
    Configuration,
    "MoneyContext" // or any custom path like "App:Finance:MoneyContext"
);
```

Example `appsettings.json`:

```json
{
  "MoneyContext": {
    "DefaultCurrency": "EUR",
    "Precision": 28,
    "MaxScale": 2
  }
}
```

### Using Direct Options Instance

```csharp
var options = new MoneyContextOptions {
    DefaultCurrency = Currency.FromCode("USD"),
    RoundingStrategy = new HalfUpRounding()
};
services.AddMoneyContext(options);
```

## Named Contexts

For applications that need to work with multiple currencies or rounding rules:

```csharp
// Register multiple named contexts
services.AddMoneyContext(options => {
    options.DefaultCurrency = Currency.FromCode("USD");
    options.RoundingStrategy = new HalfUpRounding();
}, name: "US");

services.AddMoneyContext(options => {
    options.DefaultCurrency = Currency.FromCode("EUR");
    options.RoundingStrategy = new HalfEvenRounding();
}, name: "EU");

// Access named contexts in your services
public class InternationalPaymentService
{
    private readonly IMoneyContextResolver _contextResolver;

    public InternationalPaymentService(IMoneyContextResolver contextResolver)
    {
        _contextResolver = contextResolver;
    }

    public Money CalculatePrice(string region, decimal amount)
    {
        // Get appropriate context based on region
        var contextName = region == "US" ? "US" : "EU";
        var context = _contextResolver.GetContext(contextName);

        // Use context for calculation
        using var scope = MoneyContext.CreateScope(context);
        return new Money(amount); // Uses the scoped context
    }
}
```

## Real-world Scenarios for Named Contexts

Named contexts are particularly useful in scenarios where different parts of an application need to handle money with varying rules, such as different currencies, rounding strategies, or precision settings. Here are some real-world scenarios where named contexts would be beneficial:

1. **Multi-regional Applications**: Applications that operate in multiple countries and need to handle different currencies and rounding rules.
2. **Financial Services**: Companies that process transactions in multiple currencies with different rounding rules for different types of operations.
3. **Accounting Systems**: Systems that need to maintain accounts in both local and reporting currencies.
4. **E-commerce Platforms**: Platforms that sell to customers worldwide and need to calculate prices, taxes, and shipping costs in different currencies.
5. **Microservices Architecture**: When different services might need different money handling configurations.

Named contexts provide flexibility and isolation, allowing different parts of an application to work with different money-handling rules simultaneously without interfering with each other.
