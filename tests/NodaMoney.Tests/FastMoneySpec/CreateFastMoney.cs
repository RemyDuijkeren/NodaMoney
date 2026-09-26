using NodaMoney.Context;
using NodaMoney.Tests.Helpers;

namespace NodaMoney.Tests.FastMoneySpec;

public class CreateFastMoney
{
    [Fact]
    public void WithCustomRoundingStrategy_ShouldCallStrategyAndStoreResult()
    {
        // Arrange
        var strategy = new RecordingRoundingStrategy(9.99m);
        MoneyContext context = MoneyContext.Create(options =>
        {
            options.MaxScale = 4;
            options.Precision = 19;
            options.RoundingStrategy = strategy;
        });

        // Act
        var money = new FastMoney(1.2345m, "EUR", context);

        // Assert
        strategy.CallCount.Should().Be(1);
        money.Amount.Should().Be(9.99m);
    }

    [Fact]
    public void WithToEvenRoundingAndMaxScaleFour_ShouldSkipStrategyAndRelyOnToOACurrency()
    {
        // Arrange
        var strategy = new StandardRounding(MidpointRounding.ToEven);
        MoneyContext context = MoneyContext.Create(options =>
        {
            options.MaxScale = 4;
            options.Precision = 19;
            options.RoundingStrategy = strategy;
        });

        // Act
        var money = new FastMoney(1.23455m, "EUR", context);

        // Assert: ToOACurrency itself rounds to 4 decimals using ToEven, so the amount is still rounded correctly
        money.Amount.Should().Be(1.2346m);
    }

    [Fact]
    public void WithDifferentContext()
    {
        // Arrange
        MoneyContext otherContext = MoneyContext.Create(opt =>
        {
            opt.MaxScale = 4;
            opt.Precision = 19;
            opt.RoundingStrategy = new NoRounding();
        });
        FastMoney money = new FastMoney(123.4567m, "EUR");

        // Act
        var result = money with { Context = otherContext };

        // Assert
        result.Should().NotBeSameAs(money);
        result.Context.Should().Be(otherContext);
        result.Amount.Should().Be(money.Amount, "changing Context via 'with' must not re-round the stored amount");
        result.Currency.Should().Be(money.Currency);
    }

    [Fact]
    public void WithContext_WhenMaxScaleExceedsFastMoneyLimit_ThenThrowArgumentOutOfRangeException()
    {
        // Arrange
        MoneyContext tooPreciseContext = MoneyContext.Create(opt =>
        {
            opt.MaxScale = 6;
            opt.RoundingStrategy = new NoRounding();
        });
        FastMoney money = new FastMoney(123.4567m, "EUR");

        // Act
        Action action = () => { var result = money with { Context = tooPreciseContext }; };

        // Assert
        action.Should().Throw<ArgumentOutOfRangeException>("FastMoney cannot represent more than 4 decimal places");
    }

    [Fact]
    public void WithContext_WhenPrecisionExceedsFastMoneyLimit_ThenThrowArgumentOutOfRangeException()
    {
        // Arrange
        MoneyContext tooPreciseContext = MoneyContext.Create(opt =>
        {
            opt.Precision = 20;
        });
        FastMoney money = new FastMoney(123.4567m, "EUR");

        // Act
        Action action = () => { var result = money with { Context = tooPreciseContext }; };

        // Assert
        action.Should().Throw<ArgumentOutOfRangeException>("FastMoney cannot exceed 19 significant digits");
    }

    [Fact]
    public void WithDifferentCurrency_WhenCurrencyRequiresMoreThan4Decimals_ThenThrowInvalidCurrencyException()
    {
        // Arrange
        CurrencyInfo bitcoin = CurrencyInfo.FromCode("BTC"); // 8 decimals
        FastMoney money = new FastMoney(1m, "EUR");

        // Act
        Action action = () => { var result = money with { Currency = bitcoin }; };

        // Assert
        action.Should().Throw<InvalidCurrencyException>("FastMoney cannot represent more than 4 decimal places");
    }
}
