using System;
using FluentAssertions;
using Xunit;

namespace NodaMoney.Tests.MoneySpec;

public class CreateMoneyWithDifferentRounding
{
    [Fact]
    public void RoundUp_When_OnlyAmount()
    {
        decimal amount = 0.525m;
        var defaultRounding = new Money(amount, "EUR");
        var differentRounding = new Money(amount, "EUR", MidpointRounding.AwayFromZero);

        defaultRounding.Amount.Should().Be(0.52m);
        differentRounding.Amount.Should().Be(0.53m);
    }

    [Fact]
    public void RoundUp_When_AmountAndCode()
    {
        decimal amount = 0.525m;
        var defaultRounding = new Money(amount, "EUR");
        var differentRounding = new Money(amount, "EUR", MidpointRounding.AwayFromZero);

        defaultRounding.Amount.Should().Be(0.52m);
        differentRounding.Amount.Should().Be(0.53m);
    }
}
