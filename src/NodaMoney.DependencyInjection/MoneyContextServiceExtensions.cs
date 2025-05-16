using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NodaMoney.Context;

namespace NodaMoney.DependencyInjection;

public static class MoneyContextServiceCollectionExtensions
{
    public static IServiceCollection AddDefaultMoneyContext(this IServiceCollection services, Action<MoneyContextOptions> configureOptions)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));
        if (configureOptions == null) throw new ArgumentNullException(nameof(configureOptions));

        // Register the options
        services.Configure(configureOptions);

        // Register the MoneyContext
        RegisterMoneyContext(services, sp =>
        {
            var options = sp.GetRequiredService<IOptions<MoneyContextOptions>>().Value;
            return MoneyContext.CreateAndSetDefault(options);
        });

        return services;
    }

    public static IServiceCollection AddDefaultMoneyContext(this IServiceCollection services, Func<IServiceProvider, MoneyContextOptions> optionsFactory)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));
        if (optionsFactory == null) throw new ArgumentNullException(nameof(optionsFactory));

        // Register the MoneyContext
        RegisterMoneyContext(services, sp =>
        {
            var options = optionsFactory(sp);
            return MoneyContext.CreateAndSetDefault(options);
        });

        return services;
    }

    private static void RegisterMoneyContext(IServiceCollection services, Func<IServiceProvider, MoneyContext> contextFactory)
        => services.AddSingleton(contextFactory);
}
