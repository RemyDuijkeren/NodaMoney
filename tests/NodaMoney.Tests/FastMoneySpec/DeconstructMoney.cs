namespace NodaMoney.Tests.FastMoneySpec;

public class DeconstructMoney
{
    [Fact]
    public void WhenDeConstructing_ThenShouldSucceed()
    {
        var money = new FastMoney(10m, "EUR");

        var (amount, currency) = money;

        amount.Should().Be(10m);
        currency.Should().Be(Currency.FromCode("EUR"));
    }
}
