using System;
using System.Collections.Generic;
using FluentAssertions;
using Xunit;

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
    public void MoneyIsAdded_When_AddUsingOperator(decimal value1, decimal value2, decimal expected)
    {
        var money1 = new Money(value1);
        var money2 = new Money(value2);

        var result = money1 + money2;

        result.Should().Be(new Money(expected));
        result.Should().NotBeSameAs(money1);
        result.Should().NotBeSameAs(money2);
    }

    [Theory, MemberData(nameof(TestData))]
    public void MoneyIsAdded_When_AddUsingMethod(decimal value1, decimal value2, decimal expected)
    {
        var money1 = new Money(value1);
        var money2 = new Money(value2);

        var result = Money.Add(money1, money2);

        result.Should().Be(new Money(expected));
        result.Should().NotBeSameAs(money1);
        result.Should().NotBeSameAs(money2);
    }

    // [Fact]
    // public void ThrowOverflowException_When_AddIsMoreThenMaxValue()
    // {
    //     // Arrange
    //     Money maxValueMoney = new(decimal.MaxValue);
    //     Money moneyToAdd = new(1);
    //
    //     // Act
    //     Action action = () => Money.Add(maxValueMoney, moneyToAdd);
    //
    //     // Assert
    //     action.Should().Throw<OverflowException>(); // Value was either too large or too small for a Decimal.
    // }

    [Theory, MemberData(nameof(TestData))]
    public void MoneyIsSubtracted_When_SubtractUsingOperator(decimal expected, decimal value2, decimal value1)
    {
        var money1 = new Money(value1);
        var money2 = new Money(value2);

        var result = money1 - money2;

        result.Should().Be(new Money(expected));
        result.Should().NotBeSameAs(money1);
        result.Should().NotBeSameAs(money2);
    }

    [Theory, MemberData(nameof(TestData))]
    public void MoneyIsSubtracted_When_SubtractUsingMethod(decimal expected, decimal value2, decimal value1)
    {
        var money1 = new Money(value1);
        var money2 = new Money(value2);

        var result = Money.Subtract(money1, money2);

        result.Should().Be(new Money(expected));
        result.Should().NotBeSameAs(money1);
        result.Should().NotBeSameAs(money2);
    }

    [Theory, MemberData(nameof(TestData))]
#pragma warning disable xUnit1026 // Theory methods should use all of their parameters
    public void ThrowException_When_AddWithDifferentCurrency_UsingOperator(decimal value1, decimal value2, decimal expected)
    {
        var money1 = new Money(value1, "EUR");
        var money2 = new Money(value2, "USD");

        Action action = () => { var result = money1 + money2; };

        action.Should().Throw<InvalidCurrencyException>().WithMessage("The requested operation expected the currency*");
    }

    [Theory, MemberData(nameof(TestData))]
    public void ThrowException_When_AddWithDifferentCurrency_UsingMethod(decimal value1, decimal value2, decimal expected)
    {
        var money1 = new Money(value1, "EUR");
        var money2 = new Money(value2, "USD");

        Action action = () => Money.Add(money1, money2);

        action.Should().Throw<InvalidCurrencyException>().WithMessage("The requested operation expected the currency*");
    }

    [Theory, MemberData(nameof(TestData))]
    public void ThrowException_When_SubtractWithDifferentCurrency_UsingOperator(decimal value1, decimal value2, decimal expected)
    {
        var money1 = new Money(value1, "EUR");
        var money2 = new Money(value2, "USD");

        Action action = () => { var result = money1 - money2; };

        action.Should().Throw<InvalidCurrencyException>().WithMessage("The requested operation expected the currency*");
    }

    [Theory, MemberData(nameof(TestData))]
    public void ThrowException_When_SubtractWithDifferentCurrency_UsingMethod(decimal value1, decimal value2, decimal expected)
    {
        var money1 = new Money(value1, "EUR");
        var money2 = new Money(value2, "USD");

        Action action = () => Money.Subtract(money1, money2);

        action.Should().Throw<InvalidCurrencyException>().WithMessage("The requested operation expected the currency*");
    }
#pragma warning restore xUnit1026 // Theory methods should use all of their parameters
}
