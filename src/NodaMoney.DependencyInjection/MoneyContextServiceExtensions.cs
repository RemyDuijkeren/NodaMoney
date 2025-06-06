using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NodaMoney.Context;

namespace NodaMoney.DependencyInjection;

public static class MoneyContextServiceCollectionExtensions
{
    /// <summary>Adds and configures the default MoneyContext to the specified service collection.</summary>
    /// <param name="services">The service collection to add the MoneyContext to.</param>
    /// <param name="configureOptions">An action to configure the MoneyContext options.</param>
    /// <returns>The updated service collection.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the services parameter is null or the configureOptions parameter is null.</exception>
    public static IServiceCollection AddMoneyContext(this IServiceCollection services, Action<MoneyContextOptions> configureOptions)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));
        if (configureOptions == null) throw new ArgumentNullException(nameof(configureOptions));

        services.AddOptions();

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

    public static IServiceCollection AddMoneyContext(this IServiceCollection services, Func<IServiceProvider, MoneyContextOptions> optionsFactory)
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

    /// <summary>Adds and configures the default MoneyContext to the specified service collection using configuration settings.</summary>
    /// <param name="services">The service collection to add the MoneyContext to.</param>
    /// <param name="configuration">The configuration instance containing MoneyContext settings.</param>
    /// <param name="sectionName">The configuration section name where MoneyContext settings are defined. Defaults to "MoneyContext".</param>
    /// <returns>The updated service collection.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the services or configuration parameter is null.</exception>
    /// <remarks>
    /// This method allows configuring the MoneyContext through application configuration (e.g., appsettings.json).
    /// Example configuration section:
    /// {
    ///   "MoneyContext": {
    ///     "DefaultCurrency": "USD",
    ///     "Precision": 28,
    ///     "MaxScale": 2
    ///   }
    /// }
    /// </remarks>
    public static IServiceCollection AddMoneyContext(this IServiceCollection services, IConfiguration configuration, string sectionName = "MoneyContext")
    {
        if (services == null) throw new ArgumentNullException(nameof(services));
        if (configuration == null) throw new ArgumentNullException(nameof(configuration));

        services.AddOptions();

        // Bind configuration from the specified section
        services.Configure<MoneyContextOptions>(configuration.GetSection(sectionName));

        // Register the MoneyContext
        RegisterMoneyContext(services, sp =>
        {
            var options = sp.GetRequiredService<IOptions<MoneyContextOptions>>().Value;
            return MoneyContext.CreateAndSetDefault(options);
        });

        return services;
    }

    private static void RegisterMoneyContext(IServiceCollection services, Func<IServiceProvider, MoneyContext> contextFactory)
        => services.AddSingleton(contextFactory);
}
