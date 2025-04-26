Setting up a `MoneyContext` at the startup can be done elegantly using the **Dependency Injection (DI)** system
that is built into .NET. This approach ensures that your application's `Money` objects can always rely on a properly
configured and accessible `MoneyContext`, whether globally or per-thread.

Here's how you can set up the `MoneyContext` in a `.NET 8` app:

### Steps to Setup `MoneyContext` at App Startup

#### 1. Register `MoneyContext` in `Program.cs`
You can register one or more `MoneyContext` configurations in the DI container within the `Program.cs` file.
- For **global context**: Register a singleton `MoneyContext` that serves as the default global configuration for your entire application.
- For **customized contexts**: If needed, you can also add scoped services for per-thread or per-request `MoneyContext`.

``` csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Example: Register a default global MoneyContext (MidpointRounding.ToEven)
builder.Services.AddSingleton(MoneyContext.CreateDefault());

// Optional: Register other contexts for specific use cases
builder.Services.AddSingleton(MoneyContext.CreateRetail());      // Retail rounding
builder.Services.AddSingleton(MoneyContext.CreateAccounting()); // Accounting precision
builder.Services.AddSingleton(MoneyContext.CreateNoRounding()); // No rounding context

var app = builder.Build();

app.MapGet("/", (Money money) =>
{
    // Example usage of a Money object with the global MoneyContext
    return $"Rounded Amount: {money.Round()}";
});

app.Run();
```

### 2. Accessing the `MoneyContext` Globally
In scenarios where the `MoneyContext` is global across all threads and users, you can directly set the `DefaultGlobal` property in `Program.cs`.

#### Example - Set `DefaultGlobal` in Initialization:
``` csharp
// Set global MoneyContext at application startup
MoneyContext.DefaultGlobal = MoneyContext.Create(MidpointRounding.AwayFromZero, 18, 2);
```
This ensures that the `DefaultGlobal` `MoneyContext` is used throughout the application for all `Money` calculations unless overridden.

### 3. Using Dependency Injection for Request-Level or Scoped Contexts
If your application requires **scoped or request-specific `MoneyContext`**, use a scoped service in the DI system.

#### Example - Scoped MoneyContext:
Imagine an e-commerce system where:
- **Default rounding (global)**: Used across the system.
- **Retail rounding (per request)**: Specific to an endpoint or a service during a user transaction.

Here’s how you can handle scoped contexts:
**Program.cs Changes:**
``` csharp
builder.Services.AddScoped<MoneyContext>(provider =>
{
    // Example: Create a request-specific context (could come dynamically based on user input)
    return MoneyContext.Create(MidpointRounding.AwayFromZero, 28, 2);
});

```
**Request-Specific Usage in Controllers or Endpoints:**
``` csharp
[ApiController]
[Route("[controller]")]
public class CheckoutController : ControllerBase
{
    private readonly MoneyContext _moneyContext;

    public CheckoutController(MoneyContext moneyContext)
    {
        _moneyContext = moneyContext;
    }

    [HttpPost]
    public IActionResult FinalizePurchase([FromBody] decimal totalAmount)
    {
        // Example currency setup
        var eur = new CurrencyInfo("EUR", 2, 0.01m);

        // Use the scoped MoneyContext
        var roundedMoney = new Money(totalAmount, eur, _moneyContext);
        return Ok(new { amount = roundedMoney.Round() });
    }
}
```

### 4. Setting Up Thread-Local MoneyContext
If an operation, such as a batch job or background service, needs to set a **thread-local MoneyContext** temporarily, you can directly assign to `MoneyContext.ThreadContext`.
Here’s how you can set up `ThreadContext` during an operation:

#### Example: Thread-Scoped MoneyContext for Background Processing
``` csharp
using Microsoft.Extensions.Hosting;

public class BatchProcessService : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Set a thread-local MoneyContext for this batch job
        MoneyContext.ThreadContext = MoneyContext.CreateRetail();

        // Example processing logic
        var usd = new CurrencyInfo("USD", 2, 0.01m);
        var money = new Money(123.457m, usd); // Uses thread-local context
        Console.WriteLine($"Thread-Scoped Rounded Amount: {money.Round()}");

        // Unset the thread-local MoneyContext (restore to default behavior)
        MoneyContext.ThreadContext = null;

        return Task.CompletedTask;
    }
}
```
This approach ensures that the `MoneyContext` is contextually correct for the duration of the operation's execution without interfering with other parts of the application.

### 5. Dynamic Context Setup in Middleware
For an API application, you might want to set the `MoneyContext` dynamically based on client preferences (e.g., passed in HTTP headers or query parameters).

#### Example Middleware to Dynamically Configure MoneyContext:
``` csharp
public class MoneyContextMiddleware
{
    private readonly RequestDelegate _next;

    public MoneyContextMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Example: Read rounding preferences from a query parameter or header
        var roundingPolicy = context.Request.Query["roundingPolicy"];
        MoneyContext.ThreadContext = roundingPolicy switch
        {
            "Retail" => MoneyContext.CreateRetail(),
            "Accounting" => MoneyContext.CreateAccounting(),
            _ => MoneyContext.CreateDefault()
        };

        try
        {
            await _next(context);
        }
        finally
        {
            // Reset the thread-local context after the request is handled
            MoneyContext.ThreadContext = null;
        }
    }
}
```

**Register the Middleware in `Program.cs`:**
``` csharp
app.UseMiddleware<MoneyContextMiddleware>();

app.MapGet("/round", (decimal amount) =>
{
    var usd = new CurrencyInfo("USD", 2, 0.01m);
    var money = new Money(amount, usd); // Uses dynamically set ThreadContext
    return $"Rounded Amount: {money.Round()}";
});
```
Now, you can pass a query parameter like `?roundingPolicy=Retail` or `?roundingPolicy=Accounting` to apply different rounding policies dynamically.

### Summary of Setup Patterns
1. **Global Context**:
  - Set `MoneyContext.DefaultGlobal` once at application startup.
  - All `Money` objects default to this context unless explicitly overridden.

2. **Scoped Context (Request-Specific)**:
  - Register `MoneyContext` as a **scoped service** for APIs or specific threads.
  - Use the injected `MoneyContext` to handle operations on a per-request basis.

3. **Thread-Local Context**:
  - Set `MoneyContext.ThreadContext` before executing tasks (e.g., in batch jobs or dynamic middleware).
  - Reset it afterward to avoid cross-thread interference.

4. **Dynamic Middleware**:
  - Dynamically assign `MoneyContext` based on client requests (e.g., query parameters or headers).
