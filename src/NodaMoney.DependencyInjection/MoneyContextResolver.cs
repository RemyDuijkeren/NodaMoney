using Microsoft.Extensions.Options;
using NodaMoney.Context;

namespace NodaMoney.DependencyInjection;

// Interface for resolving named contexts
public interface IMoneyContextResolver
{
    MoneyContext GetContext(string? name = null);
}

// Implementation
public class MoneyContextResolver : IMoneyContextResolver
{
    private readonly IOptionsMonitor<MoneyContextOptions> _optionsMonitor;

    public MoneyContextResolver(IOptionsMonitor<MoneyContextOptions> optionsMonitor)
    {
        _optionsMonitor = optionsMonitor;
    }

    public MoneyContext GetContext(string? name = null)
    {
        // For unnamed/default contexts
        if (string.IsNullOrEmpty(name))
        {
            // Try to get the default context
            return MoneyContext.DefaultThreadContext;
        }

        // Try to get the named context
        var namedContext = MoneyContext.Get(name);
        if (namedContext != null)
        {
            return namedContext;
        }

        // Create on demand using the current options
        var options = _optionsMonitor.Get(name);
        return MoneyContext.Create(options, name);
    }
}
