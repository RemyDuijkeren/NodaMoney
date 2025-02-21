namespace NodaMoney.Tests.CurrencySpec;

public class DeconstructCurrency
{
    [Fact]
    public void WhenDeConstructing_ThenShouldSucceed()
    {
        Currency currency = CurrencyInfo.FromCode("EUR");

        var (code, symbol) = currency;

        code.Should().Be("EUR");
        symbol.Should().Be("€");
    }
}
