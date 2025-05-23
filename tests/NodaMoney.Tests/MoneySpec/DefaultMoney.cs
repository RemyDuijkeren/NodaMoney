﻿namespace NodaMoney.Tests.MoneySpec;

public class DefaultMoney
{
    [Fact]
    public void WhenCreatingDefault_ThenItShouldBeNoCurrency()
    {
        Money money = default;

        money.Should().NotBeNull();
        money.Currency.Should().Be(default(Currency));
        money.Amount.Should().Be(default(decimal));
    }
}
