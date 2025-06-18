using System.Collections.Generic;

namespace NodaMoney.Tests.MoneyBinaryOperatorsSpec;

public class AddAndSubtractMoney
{
    // Test data to use for:
    // - Addition    (value1, value2, expected) => value1 + value2 = expected
    // - Subtraction (expected, value2, value1) => value1 - value2 = expected
    public static IEnumerable<object[]> TestData =>
    [
        // whole numbers
        [101, 99, 200],
        [1, 10, 11],
        [1, -10, -9],
        [-1, 10, 9],
        [-1, -10, -11],
        [-9, 10, 1],
        [11, -10, 1],
        [-11, 10, -1],
        [9, -10, -1],
        // fractions
        [100, 0.01, 100.01],
        [0.01, 10, 10.01],
        [0.01, -10, -9.99],
        [-9.99, 10, 0.01],
        [10.01, -10, 0.01],
        [100.999, -0.9, 100.099],
        [-100.999, -0.9, -101.899],
        // overflow
        [100.999, 0.9, 101.899],
        [100.5, 0.9, 101.4],
        // zero values
        [0, 0, 0],
        [0, 10, 10],
        [0, -10, -10],
        [-10, 10, 0],
        [10, -10, 0],
        [10, 0, 10],
        [-10, 0, -10]
    ];

    [Theory, MemberData(nameof(TestData))]
    public void WhenDecimalAdd_ValidateTestData(decimal value1, decimal value2, decimal expected)
    {
        var result = value1 + value2;
        result.Should().Be(expected, "decimal result failed so test data is wrong");
    }


    [Theory, MemberData(nameof(TestData))]
    public void WhenDecimalSubtract_ValidateTestData(decimal expected, decimal value2, decimal value1)
    {
        var result = value1 - value2;
        result.Should().Be(expected, "decimal result failed so test data is wrong");
    }

    [Theory, MemberData(nameof(TestData))]
    public void WhenAddOperator_ReturnSumMoney(decimal value1, decimal value2, decimal expected)
    {
        // Arrange
        var money1 = new Money(value1);
        var money2 = new Money(value2);

        // Act
        var result = money1 + money2;

        // Assert
        result.Should().Be(new Money(expected));
        result.Should().NotBeSameAs(money1);
        result.Should().NotBeSameAs(money2);
    }

    [Theory, MemberData(nameof(TestData))]
    public void WhenAddMethod_ReturnSumMoney(decimal value1, decimal value2, decimal expected)
    {
        // Arrange
        var money1 = new Money(value1);
        var money2 = new Money(value2);

        // Act
        var result = Money.Add(money1, money2);

        // Assert
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
        // Arrange
        var money1 = new Money(value1);
        var money2 = new Money(value2);

        // Act
        var result = money1 - money2;

        // Assert
        result.Should().Be(new Money(expected));
        result.Should().NotBeSameAs(money1);
        result.Should().NotBeSameAs(money2);
    }

    [Theory, MemberData(nameof(TestData))]
    public void WhenSubtractMethod_ReturnSubtractedMoney(decimal expected, decimal value2, decimal value1)
    {
        // Arrange
        var money1 = new Money(value1);
        var money2 = new Money(value2);

        // Act
        var result = Money.Subtract(money1, money2);

        // Assert
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

    [SkippableTheory, MemberData(nameof(TestData))]
#pragma warning disable xUnit1026 // Theory methods should use all of their parameters
    public void AddOperator_WhenAddWithDifferentCurrency_ThrowException(decimal value1, decimal value2, decimal expected)
    {
        Skip.If(value1 == 0 || value2 == 0, "Skip for 0 values");

        // Arrange
        var money1 = new Money(value1, "EUR");
        var money2 = new Money(value2, "USD");

        // Act
        Action action = () => { var result = money1 + money2; };

        // Assert
        action.Should().Throw<InvalidCurrencyException>().WithMessage("Currency mismatch*");
    }

    [SkippableTheory, MemberData(nameof(TestData))]
    public void AddMethod_WhenAddWithDifferentCurrency_ThrowException(decimal value1, decimal value2, decimal expected)
    {
        Skip.If(value1 == 0 || value2 == 0, "Skip for 0 values");

        // Arrange
        var money1 = new Money(value1, "EUR");
        var money2 = new Money(value2, "USD");

        // Act
        Action action = () => Money.Add(money1, money2);

        // Arrange
        action.Should().Throw<InvalidCurrencyException>().WithMessage("Currency mismatch*");
    }

    [SkippableTheory, MemberData(nameof(TestData))]
    public void SubtractOperator_WhenSubtractWithDifferentCurrency_ThrowException(decimal expected, decimal value2, decimal value1)
    {
        Skip.If(value1 == 0 || value2 == 0, "Skip for 0 values");

        // Arrange
        var money1 = new Money(value1, "EUR");
        var money2 = new Money(value2, "USD");

        // Act
        Action action = () => { var result = money1 - money2; };

        // Assert
        action.Should().Throw<InvalidCurrencyException>().WithMessage("Currency mismatch*");
    }

    [SkippableTheory, MemberData(nameof(TestData))]
    public void SubtractMethod_WhenSubtractWithDifferentCurrency_ThrowException(decimal expected, decimal value2, decimal value1)
    {
        Skip.If(value1 == 0 || value2 == 0, "Skip for 0 values");

        // Arrange
        var money1 = new Money(value1, "EUR");
        var money2 = new Money(value2, "USD");

        // Act
        Action action = () => Money.Subtract(money1, money2);

        // Assert
        action.Should().Throw<InvalidCurrencyException>().WithMessage("Currency mismatch*");
    }
#pragma warning restore xUnit1026 // Theory methods should use all of their parameters
}
