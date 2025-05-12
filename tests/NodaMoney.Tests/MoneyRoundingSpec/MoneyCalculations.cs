namespace NodaMoney.Tests.MoneyRoundingSpec;

public class MoneyCalculations
{
    [Theory]
    [InlineData(1000, "XXX", 150)]
    [InlineData(1000, "EUR", 150)]
    public void RepeatedMultiplyDivideOneStep(decimal amount, string currency, decimal expectedResult)
    {
        // Arrange
        Money subject = new(amount, currency);

        // Act
        var result = subject * 15 / 100;

        // Assert
        result.Amount.Should().Be(expectedResult); // Test that 1000 * 15 / 100 == 150
    }

    [Theory]
    [InlineData(1000, "XXX", 150)]
    [InlineData(1000, "EUR", 150)]
    public void RepeatedMultiplyDivideTwoStep(decimal amount, string currency, decimal expectedResult)
    {
        // Arrange
        Money subject = new(amount, currency);

        // Act
        var intermediate = subject * 15;
        var result = intermediate / 100;

        // Assert
        result.Amount.Should().Be(expectedResult); // Test that 1000 * 15 / 100 == 150
    }

    [Fact]
    public void Accumulation()
    {
        // Arrange
        Money value = new Money(1m, "USD");

        // Act
        for (int i = 0; i < 10000; i++)
        {
            value += 0.01m;
        }

        // Assert
        value.Amount.Should().Be(101m); // Expect exact addition without accumulative error
    }

    [Theory]
    [InlineData(1234.56789, "XXX", 0.00001, 1234.56788)]
    [InlineData(1234.56789, "EUR", 0.00001, 1234.57)]
    public void SubtractionWithCloseValues(decimal amount, string currency, decimal smallDifference, decimal expectedResult)
    {
        // Arrange
        Money value = new(amount, currency);

        // Act
        Money result = value - new Money(smallDifference, value.Currency);

        // Assert
        result.Amount.Should().Be(expectedResult); // Accuracy when values are close
    }
}
