using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NodaMoney.Context;

namespace NodaMoney.DependencyInjection.Tests;

public class AddMoneyContext_WithConfigureOptionsAction
{
    [Fact]
    public void ThrowArgumentNullException_When_ServicesIsNull()
    {
        // Arrange
        IServiceCollection? services = null;
        Action<MoneyContextOptions> configureOptions = _ => { };

        // Act
        Action act = () => services!.AddMoneyContext(configureOptions);

        // Assert
        act.Should().Throw<ArgumentNullException>()
           .WithParameterName("services");
    }

    [Fact]
    public void ThrowArgumentNullException_When_ConfigureOptionsIsNull()
    {
        // Arrange
        var services = new ServiceCollection();
        Action<MoneyContextOptions>? configureOptions = null;

        // Act
        Action act = () => services.AddMoneyContext(configureOptions!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
           .WithParameterName("configureOptions");
    }

    [Fact]
    public void RegisterOptionsWithConfiguration_When_ValidParametersProvided()
    {
        // Arrange
        var services = new ServiceCollection();
        var defaultCurrency = CurrencyInfo.FromCode("EUR");
        var precision = 15;
        var maxScale = 2;

        // Act
        services.AddMoneyContext(options =>
        {
            options.DefaultCurrency = defaultCurrency;
            options.Precision = precision;
            options.MaxScale = maxScale;
        });

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var options = serviceProvider.GetRequiredService<IOptions<MoneyContextOptions>>().Value;

        options.Should().NotBeNull();
        options.DefaultCurrency.Should().BeEquivalentTo(defaultCurrency);
        options.Precision.Should().Be(precision);
        options.MaxScale.Should().Be(maxScale);
    }

    [Fact]
    public void RegisterMoneyContext_When_ValidParametersProvided()
    {
        // Arrange
        var services = new ServiceCollection();
        var customRoundingStrategy = new NoRounding();
        var defaultCurrency = CurrencyInfo.FromCode("EUR");

        // Act
        services.AddMoneyContext(options =>
        {
            options.RoundingStrategy = customRoundingStrategy;
            options.DefaultCurrency = defaultCurrency;
        });

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var moneyContext = serviceProvider.GetRequiredService<MoneyContext>();

        moneyContext.Should().NotBeNull();
        moneyContext.RoundingStrategy.Should().BeSameAs(customRoundingStrategy);
        moneyContext.DefaultCurrency.Should().BeEquivalentTo(defaultCurrency);
    }

    [Fact]
    public void SetAsDefaultThreadContext_When_ValidParametersProvided()
    {
        // Arrange
        var services = new ServiceCollection();
        var options = new MoneyContextOptions
        {
            Precision = 10,
            MaxScale = 2,
            RoundingStrategy = new NoRounding(),
            DefaultCurrency = CurrencyInfo.FromCode("EUR")
        };

        // Act
        services.AddMoneyContext(o =>
        {
            o.Precision = options.Precision;
            o.MaxScale = options.MaxScale;
            o.RoundingStrategy = options.RoundingStrategy;
            o.DefaultCurrency = options.DefaultCurrency;
        });

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var registeredContext = serviceProvider.GetRequiredService<MoneyContext>();

        MoneyContext.DefaultThreadContext.Should().BeSameAs(registeredContext);
    }

    [Fact]
    public void ReturnSameServiceCollection_When_ValidParametersProvided()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var result = services.AddMoneyContext(options => { });

        // Assert
        result.Should().BeSameAs(services);
    }
}
