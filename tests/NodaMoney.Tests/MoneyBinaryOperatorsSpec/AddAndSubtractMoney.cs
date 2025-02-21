using System.Collections.Generic;

namespace NodaMoney.Tests.MoneyBinaryOperatorsSpec;

public class AddAndSubtractMoney
{
    public static IEnumerable<object[]> TestData =>
    [
        [101m, 99m, 200m], // whole numbers
        [100m, 0.01m, 100.01m], // fractions
        [100.999m, 0.9m, 101.899m], // overflow
        [100.5m, 0.9m, 101.4m], // overflow
        [100.999m, -0.9m, 100.099m], // negative
        [-100.999m, -0.9m, -101.899m] // negative
    ];

    [Theory, MemberData(nameof(TestData))]
    public void WhenAddOperator_ReturnSumMoney(decimal value1, decimal value2, decimal expected)
    {
        var money1 = new Money(value1);
        var money2 = new Money(value2);

        var result = money1 + money2;

        result.Should().Be(new Money(expected));
        result.Should().NotBeSameAs(money1);
        result.Should().NotBeSameAs(money2);
    }

    [Theory, MemberData(nameof(TestData))]
    public void WhenAddMethod_ReturnSumMoney(decimal value1, decimal value2, decimal expected)
    {
        var money1 = new Money(value1);
        var money2 = new Money(value2);

        var result = Money.Add(money1, money2);

        result.Should().Be(new Money(expected));
        result.Should().NotBeSameAs(money1);
        result.Should().NotBeSameAs(money2);
    }

    [Fact]
    public void WhenAddIsMoreThenMaxValue_ThrowOverflowException()
    {
        // Arrange
        Money maxValueMoney = new(decimal.MaxValue);
        Money moneyToAdd = new(1);

        // Act
        Action action = () => Money.Add(maxValueMoney, moneyToAdd);

        // Assert
        action.Should().Throw<OverflowException>().WithMessage("Value was either too large or too small for a Money.");
    }

    [Fact]
    public void WhenSubtractIsMoreThenMinValue_ThrowOverflowException()
    {
        // Arrange
        Money maxValueMoney = new(decimal.MinValue);
        Money moneyToAdd = new(1);

        // Act
        Action action = () => Money.Subtract(maxValueMoney, moneyToAdd);

        // Assert
        action.Should().Throw<OverflowException>().WithMessage("Value was either too large or too small for a Money.");
    }

    [Theory, MemberData(nameof(TestData))]
    public void WhenSubtractOperator_ReturnSubtractedMoney(decimal expected, decimal value2, decimal value1)
    {
        var money1 = new Money(value1);
        var money2 = new Money(value2);

        var result = money1 - money2;

        result.Should().Be(new Money(expected));
        result.Should().NotBeSameAs(money1);
        result.Should().NotBeSameAs(money2);
    }

    [Theory, MemberData(nameof(TestData))]
    public void WhenSubtractMethod_ReturnSubtractedMoney(decimal expected, decimal value2, decimal value1)
    {
        var money1 = new Money(value1);
        var money2 = new Money(value2);

        var result = Money.Subtract(money1, money2);

        result.Should().Be(new Money(expected));
        result.Should().NotBeSameAs(money1);
        result.Should().NotBeSameAs(money2);
    }

    [Fact]
    public void AddOperator_WhenAddWithZeroInDifferentCurrency_ReturnSumMoney()
    {
        // Arrange
        Money money = new(123.45m, "EUR");
        Money zero = new(0m, "USD");

        // Act
        Money result = money + zero;

        // Assert
        result.Should().Be(money);
        result.Should().NotBeSameAs(zero);
    }

    [Fact]
    public void AddMethod_WhenAddWithZeroInDifferentCurrency_ReturnSumMoney()
    {
        // Arrange
        Money money = new(123.45m, "EUR");
        Money zero = new(0m, "USD");

        // Act
        Money result = Money.Add(money, zero);

        // Assert
        result.Should().Be(money);
        result.Should().NotBeSameAs(zero);
    }

    [Fact]
    public void SubtractOperator_WhenSubtractWithZeroInDifferentCurrency_ReturnSubtractedMoney()
    {
        // Arrange
        Money money = new(123.45m, "EUR");
        Money zero = new(0m, "USD");

        // Act
        Money result = money - zero;

        // Assert
        result.Should().Be(money);
        result.Should().NotBeSameAs(zero);
    }

    [Fact]
    public void SubtractMethod_WhenSubstractWithZeroInDifferentCurrency_ReturnSubtractedMoney()
    {
        // Arrange
        Money money = new(123.45m, "EUR");
        Money zero = new(0m, "USD");

        // Act
        Money result = Money.Subtract(money, zero);

        // Assert
        result.Should().Be(money);
        result.Should().NotBeSameAs(zero);
    }

    [Theory, MemberData(nameof(TestData))]
#pragma warning disable xUnit1026 // Theory methods should use all of their parameters
    public void AddOperator_WhenAddWithDifferentCurrency_ThrowException(decimal value1, decimal value2, decimal expected)
    {
        var money1 = new Money(value1, "EUR");
        var money2 = new Money(value2, "USD");

        Action action = () => { var result = money1 + money2; };

        action.Should().Throw<InvalidCurrencyException>().WithMessage("Currency mismatch*");
    }

    [Theory, MemberData(nameof(TestData))]
    public void AddMethod_WhenAddWithDifferentCurrency_ThrowException(decimal value1, decimal value2, decimal expected)
    {
        var money1 = new Money(value1, "EUR");
        var money2 = new Money(value2, "USD");

        Action action = () => Money.Add(money1, money2);

        action.Should().Throw<InvalidCurrencyException>().WithMessage("Currency mismatch*");
    }

    [Theory, MemberData(nameof(TestData))]
    public void AddOperator_WhenSubtractWithDifferentCurrency_ThrowException(decimal value1, decimal value2, decimal expected)
    {
        var money1 = new Money(value1, "EUR");
        var money2 = new Money(value2, "USD");

        Action action = () => { var result = money1 - money2; };

        action.Should().Throw<InvalidCurrencyException>().WithMessage("Currency mismatch*");
    }

    [Theory, MemberData(nameof(TestData))]
    public void SubtractMethod_WhenSubtractWithDifferentCurrency_ThrowException(decimal value1, decimal value2, decimal expected)
    {
        var money1 = new Money(value1, "EUR");
        var money2 = new Money(value2, "USD");

        Action action = () => Money.Subtract(money1, money2);

        action.Should().Throw<InvalidCurrencyException>().WithMessage("Currency mismatch*");
    }
#pragma warning restore xUnit1026 // Theory methods should use all of their parameters
}
