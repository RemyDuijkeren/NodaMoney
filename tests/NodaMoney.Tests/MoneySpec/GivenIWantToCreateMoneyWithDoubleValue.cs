using FluentAssertions;
using Xunit;

namespace NodaMoney.Tests.MoneySpec;

public class GivenIWantToCreateMoneyWithDoubleValue
{
    [Theory]
    [InlineData(0.03, 0.03)]
    [InlineData(0.3333333333333333, 0.33)]
    public void WhenValueIsDoubleAndWithCurrency_ThenMoneyShouldBeCorrect(double input, decimal expected)
    {
        var money = new Money(input, "EUR");

        money.Amount.Should().Be(expected);
    }

    [Theory]
    [InlineData(0.03, 0.03)]
    [InlineData(0.3333333333333333, 0.33)]
    public void WhenValueIsDoubleWithoutCurrency_ThenMoneyShouldBeCorrect(double input, decimal expected)
    {
        var money = new Money(input, "EUR");

        money.Amount.Should().Be(expected);
    }
}
