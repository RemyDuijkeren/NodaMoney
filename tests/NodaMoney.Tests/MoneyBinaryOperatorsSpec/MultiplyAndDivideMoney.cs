using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace NodaMoney.Tests.MoneyBinaryOperatorsSpec;

public class MultiplyAndDivideMoney
{
    public static IEnumerable<object[]> TestDataDecimal =>
    [
        [10m, 15m, 150m],
        [100.12m, 0.5m, 50.06m],
        [100.12m, 5m, 500.60m],
        [-100.12m, 0.5m, -50.06m],
        [-100.12m, 5m, -500.60m]
    ];

    [Theory, MemberData(nameof(TestDataDecimal))]
    public void MoneyIsMultiplied_When_MultiplyByDecimal_UsingOperator(decimal value, decimal multiplier, decimal expected)
    {
        var money = new Money(value, "EUR");

        var result1 = money * multiplier;
        var result2 = multiplier * money;

        result1.Should().Be(new Money(expected, "EUR"));
        result1.Should().NotBeSameAs(money);
        result2.Should().Be(new Money(expected, "EUR"));
        result2.Should().NotBeSameAs(money);
    }

    [Theory, MemberData(nameof(TestDataDecimal))]
    public void MoneyIsMultiplied_When_MultiplyByDecimal_UsingMethod(decimal value, decimal multiplier, decimal expected)
    {
        var money = new Money(value, "EUR");

        var result = Money.Multiply(money, multiplier);

        result.Should().Be(new Money(expected, "EUR"));
        result.Should().NotBeSameAs(money);
    }

    [Theory, MemberData(nameof(TestDataDecimal))]
    public void MoneyIsDivided_When_DividedByDecimal_UsingOperator(decimal expected, decimal divider, decimal value)
    {
        var money = new Money(value, "EUR");

        var result = money / divider;

        result.Should().Be(new Money(expected, "EUR"));
        result.Should().NotBeSameAs(money);
    }

    [Theory, MemberData(nameof(TestDataDecimal))]
    public void MoneyIsDivided_When_DividedByDecimal_UsingMethod(decimal expected, decimal divider, decimal value)
    {
        // Arrange
        var money = new Money(value, "EUR");

        // Act
        var result = Money.Divide(money, divider);

        // Assert
        result.Should().Be(new Money(expected, "EUR"));
        result.Should().NotBeSameAs(money);
    }

    public static IEnumerable<object[]> TestDataInteger =>
    [
        [10m, 15, 150],
        [100.12m, 3, 300.36m],
        [100.12m, 5, 500.60m],
        [-100.12m, 3, -300.36m],
        [-100.12m, 5, -500.60m]
    ];

    [Theory, MemberData(nameof(TestDataInteger))]
    public void MoneyIsMultiplied_When_MultiplyByInt_UsingOperator(decimal value, int multiplier, decimal expected)
    {
        var money = new Money(value, "EUR");

        var result1 = money * multiplier;
        var result2 = multiplier * money;

        result1.Should().Be(new Money(expected, "EUR"));
        result1.Should().NotBeSameAs(money);
        result2.Should().Be(new Money(expected, "EUR"));
        result2.Should().NotBeSameAs(money);
    }

    [Theory, MemberData(nameof(TestDataInteger))]
    public void MoneyIsMultiplied_When_MultiplyByInt_UsingMethod(decimal value, int multiplier, decimal expected)
    {
        var money = new Money(value, "EUR");

        var result = Money.Multiply(money, multiplier);

        result.Should().Be(new Money(expected, "EUR"));
        result.Should().NotBeSameAs(money);
    }

    [Theory, MemberData(nameof(TestDataInteger))]
    public void MoneyIsDivided_When_DivideByInt_UsingOperator(decimal expected, int divider, decimal value)
    {
        var money = new Money(value, "EUR");

        var result = money / divider;

        result.Should().Be(new Money(expected, "EUR"));
        result.Should().NotBeSameAs(money);
    }

    [Theory, MemberData(nameof(TestDataInteger))]
    public void MoneyIsDivided_When_DivideByInt_UsingMethod(decimal expected, int divider, decimal value)
    {
        var money = new Money(value, "EUR");

        var result = Money.Divide(money, divider);

        result.Should().Be(new Money(expected, "EUR"));
        result.Should().NotBeSameAs(money);
    }

    public static IEnumerable<object[]> TestDataMoney =>
    [
        [150m, 15m, 10m],
        [100.12m, 3m, decimal.Divide(100.12m, 3m)],
        [100m, 3m, decimal.Divide(100m, 3m)]
    ];

    [Theory, MemberData(nameof(TestDataMoney))]
    public void ReturnDecimalRatio_When_DivideByMoney_UsingOperator(decimal value1, decimal value2, decimal expected)
    {
        var money1 = new Money(value1, "EUR");
        var money2 = new Money(value2, "EUR");

        var result = money1 / money2;

        result.Should().BeOfType(typeof(decimal));
        result.Should().Be(expected); // ratio
    }

    [Theory, MemberData(nameof(TestDataMoney))]
    public void ReturnDecimalRatio_When_DivideByMoney_UsingMethod(decimal value1, decimal value2, decimal expected)
    {
        var money1 = new Money(value1, "EUR");
        var money2 = new Money(value2, "EUR");

        var result = Money.Divide(money1, money2);

        result.Should().BeOfType(typeof(decimal));
        result.Should().Be(expected); // ratio
    }
}
