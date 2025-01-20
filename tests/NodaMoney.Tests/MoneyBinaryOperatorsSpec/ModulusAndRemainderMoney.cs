using System;
using FluentAssertions;
using Xunit;

namespace NodaMoney.Tests.MoneyBinaryOperatorsSpec;

public class ModulusAndRemainderMoney
{
    [Fact]
    public void WithSameCurrency_ReturnsCorrectResult()
    {
        // Arrange
        var left = new Money(105.50m, new Currency("USD"));
        var right = new Money(20m, new Currency("USD"));

        // Act
        var result = left % right;

        // Assert
        result.Should().Be(new Money(5.50m, new Currency("USD")));
    }

    [Fact]
    public void WithExactDivision_ReturnsZero()
    {
        // Arrange
        var left = new Money(100m, new Currency("USD"));
        var right = new Money(20m, new Currency("USD"));

        // Act
        var result = left % right;

        // Assert
        result.Should().Be(new Money(0m, new Currency("USD")));
    }

    [Fact]
    public void WhenNegativeDividend_ReturnsCorrectResult()
    {
        // Arrange
        var left = new Money(-105.50m, new Currency("USD"));
        var right = new Money(20m, new Currency("USD"));

        // Act
        var result = left % right;

        // Assert
        result.Should().Be(new Money(-5.50m, new Currency("USD")));
    }

    [Fact]
    public void WhenNegativeDivisor_ReturnsCorrectResult()
    {
        // Arrange
        var left = new Money(105.50m, new Currency("USD"));
        var right = new Money(-20m, new Currency("USD"));

        // Act
        var result = left % right;

        // Assert
        result.Should().Be(new Money(5.50m, new Currency("USD")));
    }

    [Fact]
    public void WithDifferentCurrencies_ThrowsInvalidCurrencyException()
    {
        // Arrange
        var left = new Money(100m, new Currency("USD"));
        var right = new Money(20m, new Currency("EUR"));

        // Act
        Action act = () => { var result = left % right; };

        // Assert
        act.Should().Throw<InvalidCurrencyException>().WithMessage("Currency mismatch*");
    }

    [Fact]
    public void WithZeroDivisor_ThrowsDivideByZeroException()
    {
        // Arrange
        var left = new Money(100m, new Currency("USD"));
        var right = new Money(0m, new Currency("USD"));

        // Act
        Action act = () => { var result = left % right; };

        // Assert
        act.Should().Throw<DivideByZeroException>();
    }

    [Fact]
    public void WithZeroDividend_ReturnsZero()
    {
        // Arrange
        var left = new Money(0m, new Currency("USD"));
        var right = new Money(20m, new Currency("USD"));

        // Act
        var result = left % right;

        // Assert
        result.Should().Be(new Money(0m, new Currency("USD")));
    }
}
