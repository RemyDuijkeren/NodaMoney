using NodaMoney.Context;
using NodaMoney.Tests.Helpers;

namespace NodaMoney.Tests.MoneyNumericInterfaces;

[Collection(nameof(NoParallelization))]
public class AbsoluteValueAndSign
{
    [Fact]
    public void GivenNegativeValueBuiltUnderNonDefaultContext_WhenAbsEvaluatedUnderDifferentCurrentContext_ThenReturnsPositiveValueWithOperandContext()
    {
        // Arrange
        var operandContext = MoneyContext.Create(o => o.RoundingStrategy = new StandardRounding(MidpointRounding.AwayFromZero));
        Money negative;
        using (MoneyContext.CreateScope(operandContext))
        {
            negative = new Money(-10.50m, "EUR");
        }

        var ambientContext = MoneyContext.Create(o => o.RoundingStrategy = new StandardRounding(MidpointRounding.ToEven));

        // Act
        Money result;
        using (MoneyContext.CreateScope(ambientContext))
        {
            result = Money.Abs(negative);
        }

        // Assert
        result.Amount.Should().Be(10.50m);
        result.Context.Should().BeSameAs(operandContext);
    }

    [Fact]
    public void GivenPositiveValue_WhenAbs_ThenReturnsEqualValue()
    {
        // Arrange
        Money positive = new(10.50m, "EUR");

        // Act
        Money result = Money.Abs(positive);

        // Assert
        result.Should().Be(positive);
    }

    [Fact]
    public void GivenZero_WhenAbs_ThenReturnsEqualValue()
    {
        // Arrange
        Money zero = new(0m, "EUR");

        // Act
        Money result = Money.Abs(zero);

        // Assert
        result.Should().Be(zero);
    }

    [Theory]
    [InlineData(0, false)]
    [InlineData(10.50, false)]
    [InlineData(-10.50, true)]
    public void WhenIsNegative_ThenReturnsExpected(double amount, bool expected)
    {
        // Arrange
        Money money = new((decimal)amount, "EUR");

        // Act
        bool result = Money.IsNegative(money);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(0, true)]
    [InlineData(10.50, true)]
    [InlineData(-10.50, false)]
    public void WhenIsPositive_ThenReturnsExpected(double amount, bool expected)
    {
        // Arrange
        Money money = new((decimal)amount, "EUR");

        // Act
        bool result = Money.IsPositive(money);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void GivenOperandsWithDifferentContexts_WhenMinMagnitude_ThenDoesNotThrowAndReturnsSelectedOperandUnchanged()
    {
        // Arrange
        var otherContext = MoneyContext.Create(o => o.RoundingStrategy = new StandardRounding(MidpointRounding.AwayFromZero));
        Money x = new(-5.00m, "EUR");
        Money y;
        using (MoneyContext.CreateScope(otherContext))
        {
            y = new Money(10.00m, "EUR");
        }

        // Act
        Action act = () => Money.MinMagnitude(x, y);

        // Assert
        act.Should().NotThrow();
        Money.MinMagnitude(x, y).Should().Be(x);
    }

    [Fact]
    public void GivenOperandsWithDifferentContexts_WhenMaxMagnitude_ThenDoesNotThrowAndReturnsSelectedOperandUnchanged()
    {
        // Arrange
        var otherContext = MoneyContext.Create(o => o.RoundingStrategy = new StandardRounding(MidpointRounding.AwayFromZero));
        Money x = new(-5.00m, "EUR");
        Money y;
        using (MoneyContext.CreateScope(otherContext))
        {
            y = new Money(10.00m, "EUR");
        }

        // Act
        Action act = () => Money.MaxMagnitude(x, y);

        // Assert
        act.Should().NotThrow();
        Money.MaxMagnitude(x, y).Should().Be(y);
    }
}
