using System;
using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace NodaMoney.Tests.MoneyBinaryOperatorsSpec;

public class GivenIWantToAddAndSubtractMoney
{
    public static IEnumerable<object[]> TestData => new[]
    {
        new object[] { 101m, 99m, 200m }, // whole numbers
        new object[] { 100m, 0.01m, 100.01m }, // fractions
        new object[] { 100.999m, 0.9m, 101.899m }, // overflow
        new object[] { 100.5m, 0.9m, 101.4m }, // overflow
        new object[] { 100.999m, -0.9m, 100.099m }, // negative
        new object[] { -100.999m, -0.9m, -101.899m } // negative
    };

    [Theory, MemberData(nameof(TestData))]
    public void WhenUsingAdditionOperator_ThenMoneyShouldBeAdded(decimal value1, decimal value2, decimal expected)
    {
        var money1 = new Money(value1);
        var money2 = new Money(value2);

        var result = money1 + money2;

        result.Should().Be(new Money(expected));
        result.Should().NotBeSameAs(money1);
        result.Should().NotBeSameAs(money2);
    }

    [Theory, MemberData(nameof(TestData))]
    public void WhenUsingAdditionMethod_ThenMoneyShouldBeAdded(decimal value1, decimal value2, decimal expected)
    {
        var money1 = new Money(value1);
        var money2 = new Money(value2);

        var result = Money.Add(money1, money2);

        result.Should().Be(new Money(expected));
        result.Should().NotBeSameAs(money1);
        result.Should().NotBeSameAs(money2);
    }

    [Theory, MemberData(nameof(TestData))]
    public void WhenUsingSubtractionOperator_ThenMoneyShouldBeSubtracted(decimal expected, decimal value2, decimal value1)
    {
        var money1 = new Money(value1);
        var money2 = new Money(value2);

        var result = money1 - money2;

        result.Should().Be(new Money(expected));
        result.Should().NotBeSameAs(money1);
        result.Should().NotBeSameAs(money2);
    }

    [Theory, MemberData(nameof(TestData))]
    public void WhenUsingSubtractionMethod_ThenMoneyShouldBeSubtracted(decimal expected, decimal value2, decimal value1)
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
    public void WhenUsingAdditionOperatorWithDifferentCurrency_ThenThrowException(decimal value1, decimal value2, decimal expected)
    {
        var money1 = new Money(value1, "EUR");
        var money2 = new Money(value2, "USD");

        Action action = () => { var result = money1 + money2; };

        action.Should().Throw<InvalidCurrencyException>().WithMessage("The requested operation expected the currency*");
    }

    [Theory, MemberData(nameof(TestData))]
    public void WhenUsingAdditionMethodWithDifferentCurrency_ThenThrowException(decimal value1, decimal value2, decimal expected)
    {
        var money1 = new Money(value1, "EUR");
        var money2 = new Money(value2, "USD");

        Action action = () => Money.Add(money1, money2);

        action.Should().Throw<InvalidCurrencyException>().WithMessage("The requested operation expected the currency*");
    }

    [Theory, MemberData(nameof(TestData))]
    public void WhenUsingSubtractionOperatorWithDifferentCurrency_ThenThrowException(decimal value1, decimal value2, decimal expected)
    {
        var money1 = new Money(value1, "EUR");
        var money2 = new Money(value2, "USD");

        Action action = () => { var result = money1 - money2; };

        action.Should().Throw<InvalidCurrencyException>().WithMessage("The requested operation expected the currency*");
    }

    [Theory, MemberData(nameof(TestData))]
    public void WhenUsingSubtractionMethodWithDifferentCurrency_ThenThrowException(decimal value1, decimal value2, decimal expected)
    {
        var money1 = new Money(value1, "EUR");
        var money2 = new Money(value2, "USD");

        Action action = () => Money.Subtract(money1, money2);

        action.Should().Throw<InvalidCurrencyException>().WithMessage("The requested operation expected the currency*");
    }
#pragma warning restore xUnit1026 // Theory methods should use all of their parameters
}
