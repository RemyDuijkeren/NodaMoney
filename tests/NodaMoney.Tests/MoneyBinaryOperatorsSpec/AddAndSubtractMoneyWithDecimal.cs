using System.Collections.Generic;
using NodaMoney.Tests.Helpers;

namespace NodaMoney.Tests.MoneyBinaryOperatorsSpec;

[Collection(nameof(NoParallelization))]
public class AddAndSubtractMoneyWithDecimal
{
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
    [UseCulture("en-us")]
    public void WhenAddOperator_ReturnSumMoney(decimal value1, decimal value2, decimal expected)
    {
        // Arrange
        var money1 = new Money(value1, "EUR");

        // Act
        Money result1 = money1 + value2;
        Money result2 = value2 + money1;

        // Assert
        result1.Should().Be(new Money(expected, "EUR"));
        result1.Should().NotBeSameAs(money1);
        result2.Should().Be(new Money(expected, "EUR"));
        result2.Should().NotBeSameAs(money1);
    }

    [Theory, MemberData(nameof(TestData))]
    [UseCulture("en-us")]
    public void WhenAddMethod_ReturnSumMoney(decimal value1, decimal value2, decimal expected)
    {
        // Arrange
        var money1 = new Money(value1, "EUR");

        // Act
        var result = Money.Add(money1, value2);

        // Assert
        result.Should().Be(new Money(expected, "EUR"));
        result.Should().NotBeSameAs(money1);
    }

    [Theory, MemberData(nameof(TestData))]
    [UseCulture("en-us")]
    public void WhenSubtractOperator_ReturnSubtractedMoney(decimal expected, decimal value2, decimal value1)
    {
        // Arrange
        var money1 = new Money(value1, "EUR");

        // Act
        Money result1 = money1 - value2;
        Money result2 = value2 - money1;

        // Assert
        result1.Should().Be(new Money(expected, "EUR"));
        result1.Should().NotBeSameAs(money1);
        result2.Should().Be(new Money(expected, "EUR"));
        result2.Should().NotBeSameAs(money1);
    }

    [Theory, MemberData(nameof(TestData))]
    [UseCulture("en-us")]
    public void WhenSubtractMethod_ReturnSubtractedMoney(decimal expected, decimal value2, decimal value1)
    {
        // Arrange
        var money1 = new Money(value1, "EUR");

        // Act
        var result = Money.Subtract(money1, value2);

        // Assert
        result.Should().Be(new Money(expected, "EUR"));
        result.Should().NotBeSameAs(money1);
    }

    [Fact]
    public void AddOperator_WhenAddWithNull_ReturnNull()
    {
        // Arrange
        Money money = new(100m, "EUR");
        decimal? nullMoney = null;

        // Act
        var result = money + nullMoney;

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void SubtractOperator_WhenSubtractWithNull_ReturnNull()
    {
        // Arrange
        Money money = new(100m, "EUR");
        decimal? nullMoney = null;

        // Act
        var result = money - nullMoney;

        // Assert
        result.Should().BeNull();
    }
}
