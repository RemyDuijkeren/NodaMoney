using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace NodaMoney.Tests.MoneyBinaryOperatorsSpec;

public class GivenIWantToMultiplyAndDivideMoney
{
    public static IEnumerable<object[]> TestDataDecimal => new[]
    {
        new object[] { 10m, 15m, 150m },
        new object[] { 100.12m, 0.5m, 50.06m },
        new object[] { 100.12m, 5m, 500.60m },
        new object[] { -100.12m, 0.5m, -50.06m },
        new object[] { -100.12m, 5m, -500.60m }
    };

    [Theory, MemberData(nameof(TestDataDecimal))]
    public void WhenUsingMultiplyOperatorWithDecimal_ThenMoneyShouldBeMultiplied(decimal value, decimal multiplier, decimal expected)
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
    public void WhenUsingMultiplyMethodWithDecimal_ThenMoneyShouldBeMultiplied(decimal value, decimal multiplier, decimal expected)
    {
        var money = new Money(value, "EUR");

        var result = Money.Multiply(money, multiplier);

        result.Should().Be(new Money(expected, "EUR"));
        result.Should().NotBeSameAs(money);
    }

    [Theory, MemberData(nameof(TestDataDecimal))]
    public void WhenUsingDivisionOperatorWithDecimal_ThenMoneyShouldBeDivided(decimal expected, decimal divider, decimal value)
    {
        var money = new Money(value, "EUR");

        var result = money / divider;

        result.Should().Be(new Money(expected, "EUR"));
        result.Should().NotBeSameAs(money);
    }

    [Theory, MemberData(nameof(TestDataDecimal))]
    public void WhenUsingDivisionMethodWithDecimal_ThenMoneyShouldBeDivided(decimal expected, decimal divider, decimal value)
    {
        // Arrange
        var money = new Money(value, "EUR");

        // Act
        var result = Money.Divide(money, divider);

        // Assert
        result.Should().Be(new Money(expected, "EUR"));
        result.Should().NotBeSameAs(money);
    }

    public static IEnumerable<object[]> TestDataInteger => new[]
    {
        new object[] { 10m, 15, 150 },
        new object[] { 100.12m, 3, 300.36m },
        new object[] { 100.12m, 5, 500.60m },
        new object[] { -100.12m, 3, -300.36m },
        new object[] { -100.12m, 5, -500.60m }
    };

    [Theory, MemberData(nameof(TestDataInteger))]
    public void WhenUsingMultiplyOperatorWithInteger_ThenMoneyShouldBeMultiplied(decimal value, int multiplier, decimal expected)
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
    public void WhenUsingMultiplyMethodWithInteger_ThenMoneyShouldBeMultiplied(decimal value, int multiplier, decimal expected)
    {
        var money = new Money(value, "EUR");

        var result = Money.Multiply(money, multiplier);

        result.Should().Be(new Money(expected, "EUR"));
        result.Should().NotBeSameAs(money);
    }

    [Theory, MemberData(nameof(TestDataInteger))]
    public void WhenUsingDivisionOperatorWithInteger_ThenMoneyShouldBeDivided(decimal expected, int divider, decimal value)
    {
        var money = new Money(value, "EUR");

        var result = money / divider;

        result.Should().Be(new Money(expected, "EUR"));
        result.Should().NotBeSameAs(money);
    }

    [Theory, MemberData(nameof(TestDataInteger))]
    public void WhenUsingDivisionMethodWithInteger_ThenMoneyShouldBeDivided(decimal expected, int divider, decimal value)
    {
        var money = new Money(value, "EUR");

        var result = Money.Divide(money, divider);

        result.Should().Be(new Money(expected, "EUR"));
        result.Should().NotBeSameAs(money);
    }

    public static IEnumerable<object[]> TestDataMoney => new[]
    {
        new object[] { 150m, 15m, 10m },
        new object[] { 100.12m, 3m, decimal.Divide(100.12m, 3m) },
        new object[] { 100m, 3m, decimal.Divide(100m, 3m) },
    };

    [Theory, MemberData(nameof(TestDataMoney))]
    public void WhenUsingDivisionOperatorWithMoney_ThenResultShouldBeRatio(decimal value1, decimal value2, decimal expected)
    {
        var money1 = new Money(value1, "EUR");
        var money2 = new Money(value2, "EUR");

        var result = money1 / money2;

        result.Should().Be(expected); // ratio
    }

    [Theory, MemberData(nameof(TestDataMoney))]
    public void WhenUsingDivisionMethodWithMoney_ThenResultShouldBeRatio(decimal value1, decimal value2, decimal expected)
    {
        var money1 = new Money(value1, "EUR");
        var money2 = new Money(value2, "EUR");

        var result = Money.Divide(money1, money2);

        result.Should().Be(expected); // ratio
    }
}
