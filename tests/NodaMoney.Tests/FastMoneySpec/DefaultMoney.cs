namespace NodaMoney.Tests.FastMoneySpec;

public class DefaultMoney
{
    [Fact]
    public void WhenCreatingDefault_ThenItShouldBeNoCurrency()
    {
        FastMoney money = default;

        money.Should().NotBeNull();
        money.Currency.Should().Be(default(Currency));
        money.Amount.Should().Be(default(decimal));
    }
}
