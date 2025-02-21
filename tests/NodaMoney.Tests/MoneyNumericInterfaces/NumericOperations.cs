namespace NodaMoney.Tests.MoneyNumericInterfaces;

public class NumericOperations
{
    [Fact]
    public void WhenMultiplicativeIdentity_ReturnOne()
    {
        // Act
        decimal result = Money.MultiplicativeIdentity;

        // Assert
        result.Should().Be(1m);
    }


    [Fact]
    public void WhenMultipleWithMultiplicativeIdentity_ReturnStartValue()
    {
        // Arrange
        Money startValue = new(123, "EUR");

        // Act
        Money result = startValue * Money.MultiplicativeIdentity;

        // Assert
        result.Should().Be(startValue);
    }

    [Fact]
    public void WhenAdditiveIdentity_ReturnZeroNoCurrencyMoney()
    {
        // Act
        Money result = Money.AdditiveIdentity;

        // Assert
        result.Should().Be(new Money(0m, Currency.NoCurrency));
    }

    [Fact]
    public void WhenAddingAdditiveIdentity_ReturnStartValue()
    {
        // Arrange
        Money startValue = new(123, "EUR");

        // Act
        Money result = startValue + Money.AdditiveIdentity;

        // Assert
        result.Should().Be(startValue);
    }
}
