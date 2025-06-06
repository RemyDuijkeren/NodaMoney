using Microsoft.Extensions.DependencyInjection;
using NodaMoney.Context;

namespace NodaMoney.DependencyInjection.Tests;

public class AddMoneyContext_WithOptionsFactory
{
    [Fact]
    public void RegisterMoneyContext_When_ValidParametersProvided()
    {
        // Arrange
        var services = new ServiceCollection();
        var defaultCurrency = CurrencyInfo.FromCode("USD");
        Func<IServiceProvider, MoneyContextOptions> optionsFactory = _ => new MoneyContextOptions
        {
            DefaultCurrency = defaultCurrency,
            Precision = 10,
            MaxScale = 2
        };

        // Act
        var result = services.AddMoneyContext(optionsFactory);

        // Assert
        result.Should().BeSameAs(services);
        var serviceProvider = services.BuildServiceProvider();
        var moneyContext = serviceProvider.GetService<MoneyContext>();

        moneyContext.Should().NotBeNull();
        moneyContext!.DefaultCurrency.Should().Be(defaultCurrency);
        moneyContext.Precision.Should().Be(10);
        moneyContext.MaxScale.Should().Be(2);
    }

    [Fact]
    public void ThrowArgumentNullException_When_ServicesIsNull()
    {
        // Arrange
        IServiceCollection services = null!;
        Func<IServiceProvider, MoneyContextOptions> optionsFactory = _ => new MoneyContextOptions();

        // Act
        Action act = () => services.AddMoneyContext(optionsFactory);

        // Assert
        act.Should().Throw<ArgumentNullException>()
           .And.ParamName.Should().Be("services");
    }

    [Fact]
    public void ThrowArgumentNullException_When_OptionsFactoryIsNull()
    {
        // Arrange
        var services = new ServiceCollection();
        Func<IServiceProvider, MoneyContextOptions> optionsFactory = null!;

        // Act
        Action act = () => services.AddMoneyContext(optionsFactory);

        // Assert
        act.Should().Throw<ArgumentNullException>()
           .And.ParamName.Should().Be("optionsFactory");
    }

    [Fact]
    public void UseServiceProvider_When_ConfiguringOptions()
    {
        // Arrange
        var services = new ServiceCollection();

        // Add a service that will be used by the options factory
        var mockService = new TestService();
        services.AddSingleton(mockService);

        bool optionsFactoryWasCalled = false;
        Func<IServiceProvider, MoneyContextOptions> optionsFactory = sp =>
        {
            // Verify we can resolve services from the provider
            var service = sp.GetRequiredService<TestService>();
            service.Should().BeSameAs(mockService);

            optionsFactoryWasCalled = true;
            return new MoneyContextOptions();
        };

        // Act
        services.AddMoneyContext(optionsFactory);
        var serviceProvider = services.BuildServiceProvider();
        _ = serviceProvider.GetRequiredService<MoneyContext>();

        // Assert
        optionsFactoryWasCalled.Should().BeTrue();
    }

    [Fact]
    public void CreateAndSetDefaultMoneyContext_When_OptionsFactoryProvided()
    {
        // Arrange
        var services = new ServiceCollection();
        var defaultCurrency = CurrencyInfo.FromCode("EUR");
        Func<IServiceProvider, MoneyContextOptions> optionsFactory = _ => new MoneyContextOptions
        {
            DefaultCurrency = defaultCurrency
        };

        // Act
        services.AddMoneyContext(optionsFactory);
        var serviceProvider = services.BuildServiceProvider();
        _ = serviceProvider.GetRequiredService<MoneyContext>();

        // Assert
        MoneyContext.DefaultThreadContext.DefaultCurrency.Should().Be(defaultCurrency);
    }

    // Helper class for testing service resolution
    private class TestService
    {
        public string Value { get; set; } = string.Empty;
    }

}
