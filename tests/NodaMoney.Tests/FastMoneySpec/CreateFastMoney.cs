using NodaMoney.Context;

namespace NodaMoney.Tests.FastMoneySpec;

public class CreateFastMoney
{
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
