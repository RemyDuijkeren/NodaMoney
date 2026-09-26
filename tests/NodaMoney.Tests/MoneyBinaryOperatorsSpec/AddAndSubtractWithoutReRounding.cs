using NodaMoney.Context;
using NodaMoney.Tests.Helpers;

namespace NodaMoney.Tests.MoneyBinaryOperatorsSpec;

/// <summary>Covers U5: <see cref="Money.Add(in Money, in Money)"/> and <see cref="Money.Subtract(in Money, in Money)"/>
/// skip re-rounding when the sum's scale is already within the context's target scale, and still take the full
/// rounding path otherwise.</summary>
public class AddAndSubtractWithoutReRounding
{
    [Fact]
    public void AddOperator_WholeCentsSameContext_ReturnsSumWithScale2AndOperandsContext()
    {
        // Arrange
        Money money1 = new(10.00m, "EUR");
        Money money2 = new(20.00m, "EUR");

        // Act
        Money result = money1 + money2;

        // Assert
        result.Amount.Should().Be(30.00m);
        result.Scale.Should().Be(2);
        result.ContextIndex.Should().Be(money1.ContextIndex);
    }

    [Fact]
    public void SubtractOperator_WholeCentsSameContext_ReturnsDifferenceWithScale2AndOperandsContext()
    {
        // Arrange
        Money money1 = new(30.00m, "EUR");
        Money money2 = new(20.00m, "EUR");

        // Act
        Money result = money1 - money2;

        // Assert
        result.Amount.Should().Be(10.00m);
        result.Scale.Should().Be(2);
        result.ContextIndex.Should().Be(money1.ContextIndex);
    }

    [Fact]
    public void AddOperator_MixedScaleOperands_ReturnsSumAtScale2()
    {
        // Arrange
        Money money1 = new(10.5m, "EUR");
        Money money2 = new(20.25m, "EUR");

        // Act
        Money result = money1 + money2;

        // Assert
        result.Amount.Should().Be(30.75m);
        result.Scale.Should().Be(2);
    }

    [Fact]
    public void SubtractOperator_MixedScaleOperands_ReturnsDifferenceAtScale2()
    {
        // Arrange
        Money money1 = new(30.75m, "EUR");
        Money money2 = new(20.25m, "EUR");

        // Act
        Money result = money1 - money2;

        // Assert
        result.Amount.Should().Be(10.5m);
        result.Scale.Should().Be(2);
    }

    [Fact]
    public void AddOperator_UnderMaxScale4Context_ReturnsSumUnrounded()
    {
        // Arrange
        MoneyContext maxScale4Context = MoneyContext.Create(options => options.MaxScale = 4);
        Money money1 = new(1.2345m, "EUR", maxScale4Context);
        Money money2 = new(1.0001m, "EUR", maxScale4Context);

        // Act
        Money result = money1 + money2;

        // Assert
        result.Amount.Should().Be(2.2346m);
        result.Scale.Should().Be(4);
    }

    [Fact]
    public void SubtractOperator_UnderMaxScale4Context_ReturnsDifferenceUnrounded()
    {
        // Arrange
        MoneyContext maxScale4Context = MoneyContext.Create(options => options.MaxScale = 4);
        Money money1 = new(2.2346m, "EUR", maxScale4Context);
        Money money2 = new(1.0001m, "EUR", maxScale4Context);

        // Act
        Money result = money1 - money2;

        // Assert
        result.Amount.Should().Be(1.2345m);
        result.Scale.Should().Be(4);
    }

    [Fact]
    public void AddOperator_RelabeledToNarrowerMaxScaleContext_TakesFullRoundingPath()
    {
        // Arrange: money1 is re-labeled to a MaxScale=0 context without going through the rounding constructor, so it
        // still carries its original scale-2 amount. The sum's scale (2) then exceeds the context's target (0), so
        // the full path must round it, exactly as before this change.
        MoneyContext maxScaleZeroContext = MoneyContext.Create(options => options.MaxScale = 0);
        Money money1 = new Money(10.55m, "EUR") with { Context = maxScaleZeroContext };
        Money money2 = new(1.00m, "EUR", maxScaleZeroContext);

        // Act
        Money result = money1 + money2;

        // Assert
        result.Amount.Should().Be(12m);
    }

    [Fact]
    public void SubtractOperator_RelabeledToNarrowerMaxScaleContext_TakesFullRoundingPath()
    {
        // Arrange: mirrors the addition case above.
        MoneyContext maxScaleZeroContext = MoneyContext.Create(options => options.MaxScale = 0);
        Money money1 = new Money(12.55m, "EUR") with { Context = maxScaleZeroContext };
        Money money2 = new(1.00m, "EUR", maxScaleZeroContext);

        // Act
        Money result = money1 - money2;

        // Assert
        result.Amount.Should().Be(12m);
    }

    [Fact]
    public void AddOperator_CustomRoundingStrategyContext_StillCallsStrategy()
    {
        // Arrange
        var strategy = new RecordingRoundingStrategy(30m);
        MoneyContext customContext = MoneyContext.Create(options => options.RoundingStrategy = strategy);
        Money money1 = new Money(10m, "EUR") with { Context = customContext };
        Money money2 = new Money(20m, "EUR") with { Context = customContext };

        // Act
        Money result = money1 + money2;

        // Assert
        strategy.CallCount.Should().Be(1);
        result.Amount.Should().Be(30m);
    }

    [Fact]
    public void SubtractOperator_CustomRoundingStrategyContext_StillCallsStrategy()
    {
        // Arrange
        var strategy = new RecordingRoundingStrategy(10m);
        MoneyContext customContext = MoneyContext.Create(options => options.RoundingStrategy = strategy);
        Money money1 = new Money(30m, "EUR") with { Context = customContext };
        Money money2 = new Money(20m, "EUR") with { Context = customContext };

        // Act
        Money result = money1 - money2;

        // Assert
        strategy.CallCount.Should().Be(1);
        result.Amount.Should().Be(10m);
    }

    [Fact]
    public void AddOperator_NoRoundingContext_ReturnsSumUnrounded()
    {
        // Arrange
        Money money1 = new(0.005m, "EUR", MoneyContext.NoRounding);
        Money money2 = new(0.005m, "EUR", MoneyContext.NoRounding);

        // Act
        Money result = money1 + money2;

        // Assert
        result.Amount.Should().Be(0.010m);
        result.Scale.Should().Be(3);
    }

    [Fact]
    public void SubtractOperator_NoRoundingContext_ReturnsDifferenceUnrounded()
    {
        // Arrange
        Money money1 = new(0.015m, "EUR", MoneyContext.NoRounding);
        Money money2 = new(0.005m, "EUR", MoneyContext.NoRounding);

        // Act
        Money result = money1 - money2;

        // Assert
        result.Amount.Should().Be(0.010m);
        result.Scale.Should().Be(3);
    }
}
