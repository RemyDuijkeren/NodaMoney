namespace NodaMoney.Tests.UnroundedMoneySpec;

public class DefaultMoney
{
    [Fact]
    public void WhenCreatingDefault_ThenItShouldBeNoCurrency()
    {
        UnroundedMoney money = default;

        money.Should().NotBeNull();
        money.Currency.Should().Be(default(Currency));
        money.Amount.Should().Be(default(decimal));
    }
}
