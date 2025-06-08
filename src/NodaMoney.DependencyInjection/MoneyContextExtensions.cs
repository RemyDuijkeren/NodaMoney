using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using NodaMoney.Context;
#if NET7_0_OR_GREATER
    using System.Diagnostics.CodeAnalysis;
#endif

namespace NodaMoney.DependencyInjection;

public static class MoneyContextExtensions
{
#if NET7_0_OR_GREATER
    private const string RequiresDynamicCodeMessage = "Binding strongly typed objects to configuration values may require generating dynamic code at runtime.";
    private const string TrimmingRequiredUnreferencedCodeMessage = "TOptions's dependent types may have their members trimmed. Ensure all required members are preserved.";
#endif

    /// <summary>Adds and configures the MoneyContext to the specified service collection using the provided or default MoneyContextOptions and an optional name.</summary>
    /// <param name="services">The service collection to add the MoneyContext to.</param>
    /// <param name="options">The MoneyContextOptions instance to configure the MoneyContext.</param>
    /// <param name="name">Optional name for the context configuration.</param>
    /// <returns>The updated service collection.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the services parameter is null.</exception>
    public static IServiceCollection AddMoneyContext(this IServiceCollection services, MoneyContextOptions options, string? name = null)
    {
        if (services is null) throw new ArgumentNullException(nameof(services));
        if (options is null) throw new ArgumentNullException(nameof(options));

        return services.AddMoneyContext(o =>
        {
            o.RoundingStrategy = options.RoundingStrategy;
            o.DefaultCurrency = options.DefaultCurrency;
            o.MaxScale = options.MaxScale;
            o.Precision = options.Precision;
        }, name);
    }

    /// <summary>Adds and configures the MoneyContext to the specified service collection using a configure action.</summary>
    /// <param name="services">The service collection to add the MoneyContext to.</param>
    /// <param name="configureOptions">An action to configure the MoneyContext options.</param>
    /// <param name="name">Optional name for the context configuration.</param>
    /// <returns>The updated service collection.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the services or the configureOptions parameter is null.</exception>
    public static IServiceCollection AddMoneyContext(this IServiceCollection services, Action<MoneyContextOptions> configureOptions, string? name = null)
    {
        if (services is null) throw new ArgumentNullException(nameof(services));
        if (configureOptions is null) throw new ArgumentNullException(nameof(configureOptions));

        // Add Options services if not already added
        services.AddOptions();

        // Register configuration
        if (string.IsNullOrEmpty(name))
        {
            services.Configure(configureOptions);
        }
        else
        {
            services.Configure(name, configureOptions);
        }

        // Register MoneyContext
        RegisterMoneyContextFactory(services, name);

        return services;
    }

    /// <summary>Adds and configures the MoneyContext to the specified service collection using a pre-scoped configuration section.</summary>
    /// <param name="services">The service collection to add the MoneyContext to.</param>
    /// <param name="configurationSection">The pre-scoped configuration section containing MoneyContext settings.</param>
    /// <param name="name">Optional name for the context configuration.</param>
    /// <returns>The updated service collection.</returns>
    /// <exception cref="ArgumentNullException">Thrown when services or configuration is null.</exception>
    /// <remarks>
    /// This method configures MoneyContext using a pre-scoped configuration section.
    /// Example configuration:
    /// <code>
    /// {
    ///   "MoneyContext": {
    ///     "DefaultCurrency": "USD",
    ///     "Precision": 28,
    ///     "MaxScale": 2
    ///   }
    /// }
    /// </code>
    /// </remarks>
#if NET7_0_OR_GREATER
    [RequiresDynamicCode(RequiresDynamicCodeMessage)]
    [RequiresUnreferencedCode(TrimmingRequiredUnreferencedCodeMessage)]
#endif
    public static IServiceCollection AddMoneyContext(this IServiceCollection services, IConfiguration configurationSection, string? name = null)
    {
        if (services is null) throw new ArgumentNullException(nameof(services));
        if (configurationSection is null) throw new ArgumentNullException(nameof(configurationSection));

        // Add Options services if not already added
        services.AddOptions();

        // Configure from the configuration section
        if (string.IsNullOrEmpty(name))
        {
            services.Configure<MoneyContextOptions>(configurationSection);
        }
        else
        {
            services.Configure<MoneyContextOptions>(name, configurationSection);
        }

        // Register MoneyContext
        RegisterMoneyContextFactory(services, name);

        return services;
    }

    /// <summary>Adds and configures the MoneyContext to the specified service collection using the configuration section path and an optional name.</summary>
    /// <param name="services">The service collection to add the MoneyContext to.</param>
    /// <param name="configSectionPath">The path of the configuration section used to configure the MoneyContextOptions.</param>
    /// <param name="name">An optional name for the context configuration.</param>
    /// <returns>The updated service collection.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the services or the configSectionPath parameter is null.</exception>
#if NET7_0_OR_GREATER
    [RequiresDynamicCode(RequiresDynamicCodeMessage)]
    [RequiresUnreferencedCode(TrimmingRequiredUnreferencedCodeMessage)]
#endif
    public static IServiceCollection AddMoneyContext(this IServiceCollection services, string configSectionPath, string? name = null)
    {
        if (services is null) throw new ArgumentNullException(nameof(services));
        if (configSectionPath is null) throw new ArgumentNullException(nameof(configSectionPath));

        // Add Options services if not already added
        services.AddOptions();

        // Configure from the configuration section
        if (string.IsNullOrEmpty(name))
        {
            services.AddOptions<MoneyContextOptions>()
                    .BindConfiguration(configSectionPath);
        }
        else
        {
            services.AddOptions<MoneyContextOptions>(name)
                    .BindConfiguration(configSectionPath);
        }

        // Register MoneyContext
        RegisterMoneyContextFactory(services, name);

        return services;
    }

    private static void RegisterMoneyContextFactory(IServiceCollection services, string? name)
    {
        if (string.IsNullOrEmpty(name))
        {
            // Default context
            services.AddSingleton(sp =>
            {
                var options = sp.GetRequiredService<IOptions<MoneyContextOptions>>().Value;
                return MoneyContext.CreateAndSetDefault(options);
            });
        }
        else
        {
            // Named context
            services.AddSingleton(sp =>
            {
                var optionsMonitor = sp.GetRequiredService<IOptionsMonitor<MoneyContextOptions>>();
                var options = optionsMonitor.Get(name);
                return MoneyContext.Create(options, name);
            });
        }

        // Register IMoneyContextAccessor for application code to use
        services.TryAddSingleton<IMoneyContextResolver, MoneyContextResolver>();
    }
}
