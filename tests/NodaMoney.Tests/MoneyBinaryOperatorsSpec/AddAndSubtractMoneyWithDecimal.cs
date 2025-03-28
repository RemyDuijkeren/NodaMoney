using System.Collections.Generic;
using NodaMoney.Tests.Helpers;

namespace NodaMoney.Tests.MoneyBinaryOperatorsSpec;

[Collection(nameof(NoParallelization))]
public class AddAndSubtractMoneyWithDecimal
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
    [UseCulture("en-us")]
    public void WhenAddOperator_ReturnSumMoney(decimal value1, decimal value2, decimal expected)
    {
        var money1 = new Money(value1, "EUR");

        Money result1 = money1 + value2;
        Money result2 = value2 + money1;

        result1.Should().Be(new Money(expected, "EUR"));
        result1.Should().NotBeSameAs(money1);
        result2.Should().Be(new Money(expected, "EUR"));
        result2.Should().NotBeSameAs(money1);
    }

    [Theory, MemberData(nameof(TestData))]
    [UseCulture("en-us")]
    public void WhenAddMethod_ReturnSumMoney(decimal value1, decimal value2, decimal expected)
    {
        var money1 = new Money(value1, "EUR");

        var result = Money.Add(money1, value2);

        result.Should().Be(new Money(expected, "EUR"));
        result.Should().NotBeSameAs(money1);
    }

    [Theory, MemberData(nameof(TestData))]
    [UseCulture("en-us")]
    public void WhenSubtractOperator_ReturnSubtractedMoney(decimal expected, decimal value2, decimal value1)
    {
        var money1 = new Money(value1, "EUR");

        Money result1 = money1 - value2;
        Money result2 = value2 - money1;

        result1.Should().Be(new Money(expected, "EUR"));
        result1.Should().NotBeSameAs(money1);
        result2.Should().Be(new Money(expected, "EUR"));
        result2.Should().NotBeSameAs(money1);
    }

    [Theory, MemberData(nameof(TestData))]
    [UseCulture("en-us")]
    public void WhenSubtractMethod_ReturnSubtractedMoney(decimal expected, decimal value2, decimal value1)
    {
        var money1 = new Money(value1, "EUR");

        var result = Money.Subtract(money1, value2);

        result.Should().Be(new Money(expected, "EUR"));
        result.Should().NotBeSameAs(money1);
    }
}
