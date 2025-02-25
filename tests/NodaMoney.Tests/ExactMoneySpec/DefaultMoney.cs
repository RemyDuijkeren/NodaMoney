namespace NodaMoney.Tests.ExactMoneySpec;

public class DefaultMoney
{
    [Fact]
    public void WhenCreatingDefault_ThenItShouldBeNoCurrency()
    {
        ExactMoney money = default;

        money.Should().NotBeNull();
        money.Currency.Should().Be(default(Currency));
        money.Amount.Should().Be(default(decimal));
    }
}
