# DI Registration Methods: Priority Ranking

Here's my prioritized ranking of the extension methods for registering `MoneyContext` in DI, from most important to least important:

## 1. Action-based Configuration (Highest Priority)
```csharp
public static IServiceCollection AddDefaultMoneyContext(
    this IServiceCollection services,
    Action<MoneyContextOptions> configureOptions)
```


**Why it's important:**
- Follows the standard .NET Options pattern that developers are familiar with
- Highly versatile for most common scenarios
- Compatible with Microsoft's recommended practices
- Your test suite already validates this implementation
- Allows clean, fluent configuration with lambda expressions

**Example use case:** Basic application setup with explicit configuration.

## 2. Configuration-based Setup (High Priority)
```csharp
public static IServiceCollection AddDefaultMoneyContext(
    this IServiceCollection services,
    IConfiguration configuration,
    string sectionName = "MoneyContext")
```


**Why it's important:**
- Seamlessly integrates with ASP.NET Core's configuration system
- Enables configuration through appsettings.json, environment variables, etc.
- Very common pattern for modern .NET applications
- Reduces code needed for basic setup scenarios
- Allows for deployment-specific configuration without code changes

**Example use case:** Production applications where configuration might change between environments.

## 3. Named Contexts Support (Medium-High Priority)
```csharp
public static IServiceCollection AddNamedMoneyContext(
    this IServiceCollection services,
    string name,
    Action<MoneyContextOptions> configureOptions)
```


**Why it's important:**
- Enables more complex scenarios with multiple contexts
- Critical for international applications or multi-region financial systems
- Provides isolation between different financial contexts
- Follows the pattern of other named configuration in .NET (like named HttpClients)

**Example use case:** Multi-region financial application with different currencies and rounding rules.

## 4. Factory-based Configuration (Medium Priority)
```csharp
public static IServiceCollection AddDefaultMoneyContext(
    this IServiceCollection services,
    Func<IServiceProvider, MoneyContextOptions> optionsFactory)
```


**Why it's important:**
- Enables advanced scenarios where options depend on other services
- Provides maximum flexibility for complex configuration requirements
- Useful for integrating with legacy systems or external dependencies
- Allows runtime decisions about configuration

**Example use case:** When context configuration needs to be determined by external services or databases.

## 5. Simplified/Shorthand Configuration (Lower Priority)
```csharp
public static IServiceCollection AddDefaultMoneyContext(
    this IServiceCollection services,
    string defaultCurrencyCode,
    int precision = 28,
    int? maxScale = null)
```


**Why it's less important:**
- Primarily a convenience method that doesn't add major new functionality
- Can lead to method proliferation with variations for different parameters
- The action-based configuration is already quite concise
- May need to be updated if the options class changes

**Example use case:** Quick configuration for the most common scenario.

## 6. Direct Options Instance Method (Lowest Priority)
```csharp
public static IServiceCollection AddDefaultMoneyContext(
    this IServiceCollection services,
    MoneyContextOptions options)
```


**Why it's least important:**
- Very similar to the action-based method with minimal added value
- Doesn't integrate with the standard Options pattern
- Pre-configured options can still be passed to the action-based method
- Less flexible than other approaches for most scenarios

**Example use case:** Unit testing or when options are created programmatically.

## Implementation Recommendation

Based on this prioritization and your performance-focused library, I would recommend implementing:

1. **Action-based Configuration** (already implemented)
2. **Configuration-based Setup** (implement next)
3. **Named Contexts Support** (implement if multi-context scenarios are important)

If you can only implement one or two more methods, focus on these as they cover the most common usage patterns while keeping your API surface concise and maintainable.

The factory-based approach and other methods can be added later if users request them or if you identify specific scenarios where they would provide significant value.

## Names Context

### 1. Registering Named Contexts
``` csharp
// In Startup.ConfigureServices
services.AddNamedMoneyContext("US", options =>
{
    options.DefaultCurrency = CurrencyInfo.FromCode("USD");
    options.RoundingStrategy = new HalfUpRounding();
});

services.AddNamedMoneyContext("EU", options =>
{
    options.DefaultCurrency = CurrencyInfo.FromCode("EUR");
    options.RoundingStrategy = new HalfEvenRounding();
});
```

### 2. Using Named Contexts in Services
``` csharp
public class InternationalOrderService
{
    private readonly IMoneyContextResolver _contextResolver;

    public InternationalOrderService(IMoneyContextResolver contextResolver)
    {
        _contextResolver = contextResolver;
    }

    public Money CalculatePrice(string regionCode, decimal amount)
    {
        // Get the appropriate context for the region
        string contextName = regionCode switch
        {
            "US" => "US",
            "CA" => "US", // Use US context for Canada too
            "JP" => "JP",
            _ => "EU"     // Default to EU for other regions
        };

        // Use the context to create a money object
        using var scope = MoneyContext.CreateScope(_contextResolver.GetContext(contextName));
        return new Money(amount); // Uses the context's default currency and rounding
    }
}
```
### 2. Using Named Contexts in Application Code
``` csharp
public class InternationalOrderService
{
    public Money CalculatePrice(string regionCode, decimal amount)
    {
        // Get the appropriate context for the region
        string contextName = regionCode switch
        {
            "US" or "CA" => "US",
            "JP" => "JP",
            _ => "EU"
        };

        // Use the context to create a money object
        using var scope = MoneyContext.CreateNamedScope(contextName);
        return new Money(amount); // Uses the context's default currency and rounding
    }
}
```

## Real-world Scenarios for Named Contexts
1. **Multi-regional Applications**: Applications that operate in multiple countries and need to handle different currencies and rounding rules.
2. **Financial Services**: Companies that process transactions in multiple currencies with different rounding rules for different types of operations.
3. **Accounting Systems**: Systems that need to maintain accounts in both local and reporting currencies.
4. **E-commerce Platforms**: Platforms that sell to customers worldwide and need to calculate prices, taxes, and shipping costs in different currencies.
5. **Microservices Architecture**: When different services might need different money handling configurations.

Named contexts provide flexibility and isolation, allowing different parts of an application to work with different money-handling rules simultaneously without interfering with each other.
