using NodaMoney.Context;
using NodaMoney.Tests.Helpers;

namespace NodaMoney.Tests.MoneyRoundingSpec;

[Collection(nameof(NoParallelization))]
public class ApplyCustomRounding
{
    [Fact]
    public void Construction_WhenCustomRoundingStrategy_ShouldCallStrategyAndStoreResult()
    {
        // Arrange
        var strategy = new RecordingRoundingStrategy(42.42m);
        MoneyContext context = MoneyContext.Create(options => options.RoundingStrategy = strategy);

        // Act
        var money = new Money(1.2345m, "EUR", context);

        // Assert
        strategy.CallCount.Should().Be(1);
        money.Amount.Should().Be(42.42m);
    }

    [Fact]
    public void WithAmount_WhenCustomRoundingStrategy_ShouldCallStrategyAndStoreResult()
    {
        // Arrange
        var strategy = new RecordingRoundingStrategy(7.77m);
        MoneyContext context = MoneyContext.Create(options => options.RoundingStrategy = strategy);
        var money = new Money(1m, "EUR", context);
        int callsAfterConstruction = strategy.CallCount;

        // Act
        var result = money with { Amount = 2.5m };

        // Assert
        strategy.CallCount.Should().Be(callsAfterConstruction + 1);
        result.Amount.Should().Be(7.77m);
    }

    [Fact]
    public void WithAmount_WhenStandardRounding_ShouldRoundLikeConstruction()
    {
        // Arrange
        var money = new Money(1234.56789m, "EUR");

        // Act
        var result = money with { Amount = 1234.56789m };

        // Assert
        result.Amount.Should().Be(new Money(1234.56789m, "EUR").Amount);
    }

    [Fact]
    public void WithAmount_WhenNoRounding_ShouldKeepAllDecimals()
    {
        // Arrange
        MoneyContext context = MoneyContext.Create(options => options.RoundingStrategy = new NoRounding());
        var money = new Money(1m, "EUR", context);

        // Act
        var result = money with { Amount = 1.23456789m };

        // Assert
        result.Amount.Should().Be(1.23456789m);
    }
}
