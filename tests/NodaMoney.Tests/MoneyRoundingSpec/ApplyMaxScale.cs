using NodaMoney.Context;
using NodaMoney.Tests.Helpers;

namespace NodaMoney.Tests.MoneyRoundingSpec;

[Collection(nameof(NoParallelization))]
public class ApplyMaxScale
{
    [Fact]
    public void DefaultScale()
    {
        // Arrange
        var amount = 1234.56789m;

        // Act
        var money = new Money(amount, "EUR");

        // Assert
        money.Scale.Should().Be(2, because: "Default scale of EUR is 2");
        money.Amount.Should().Be(1234.57m, because: "Rounding to 2 decimals");
    }

    [Fact]
    public void MaxScaleSetToFour()
    {
        // Arrange
        var amount = 1234.56789m;
        MoneyContext context = MoneyContext.Create(new StandardRounding(), maxScale: 4); // Rounding to 4 decimals

        // Act
        MoneyContext.CreateScope(context);
        var money = new Money(amount, "EUR");

        // Assert
        money.Context.Should().Be(context);
        money.Amount.Should().Be(1234.5679m, because: "Rounding to 4 decimals");
        money.Scale.Should().Be(4, because: "MaxScale set to 4 in MoneyContext");
    }

    [Fact]
    public void MaxScaleSetToSix()
    {
        // Arrange
        var amount = 1234.56789m;
        MoneyContext context = MoneyContext.Create(new StandardRounding(), maxScale: 6); // Rounding to 4 decimals

        // Act
        MoneyContext.CreateScope(context);
        var money = new Money(amount, "EUR");

        // Assert
        money.Context.Should().Be(context);
        money.Amount.Should().Be(1234.56789m, because: "Rounding to 6 decimals");
        money.Scale.Should().BeInRange(5, 6, because: "MaxScale set to 6 in MoneyContext");
    }
}
