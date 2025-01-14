using System;
using FluentAssertions;
using Xunit;

namespace NodaMoney.Tests.MoneySpec;

public class GivenIWantMoneyWithDifferentRounding
{
    [Fact]
    public void WhenOnlyAmount_ThenItShouldRoundUp()
    {
        decimal amount = 0.525m;
        var defaultRounding = new Money(amount, "EUR");
        var differentRounding = new Money(amount, "EUR", MidpointRounding.AwayFromZero);

        defaultRounding.Amount.Should().Be(0.52m);
        differentRounding.Amount.Should().Be(0.53m);
    }

    [Fact]
    public void WhenAmountAndCode_ThenItShouldRoundUp()
    {
        decimal amount = 0.525m;
        var defaultRounding = new Money(amount, "EUR");
        var differentRounding = new Money(amount, "EUR", MidpointRounding.AwayFromZero);

        defaultRounding.Amount.Should().Be(0.52m);
        differentRounding.Amount.Should().Be(0.53m);
    }
}
