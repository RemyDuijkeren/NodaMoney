using FluentAssertions;
using Xunit;

namespace NodaMoney.Tests.MoneyNumericInterfaces;

public class NumericOperations
{
    [Fact]
    public void ReturnOne_When_MultiplicativeIdentity()
    {
        // Act
        decimal result = Money.MultiplicativeIdentity;

        // Assert
        result.Should().Be(1m);
    }


    [Fact]
    public void ReturnStartValue_When_MultipleWithMultiplicativeIdentity()
    {
        // Arrange
        Money startValue = new(123, "EUR");

        // Act
        Money result = startValue * Money.MultiplicativeIdentity;

        // Assert
        result.Should().Be(startValue);
    }

    [Fact]
    public void ReturnZeroNoCurrencyMoney_When_AdditiveIdentity()
    {
        // Act
        Money result = Money.AdditiveIdentity;

        // Assert
        result.Should().Be(new Money(0m, Currency.NoCurrency));
    }

    [Fact]
    public void ReturnStartValue_When_AddingAdditiveIdentity()
    {
        // Arrange
        Money startValue = new(123, "EUR");

        // Act
        Money result = startValue + Money.AdditiveIdentity;

        // Assert
        result.Should().Be(startValue);
    }
}
