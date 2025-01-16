using FluentAssertions;
using Xunit;

namespace NodaMoney.Tests.MoneySpec;

public class GivenIWantDefaultMoney
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
