using NodaMoney.Exchange;

namespace NodaMoney.Tests.ExchangeRateSpec;

public class DeconstructExchangeRate
{
    [Fact]
    public void WhenDeconstruct_ThenShouldSucceed()
    {
        var fx = new ExchangeRate(Currency.FromCode("EUR"), Currency.FromCode("USD"), 1.2591m);

        var (baseCurrency, quoteCurrency, rate) = fx;

        rate.Should().Be(1.2591m);
        baseCurrency.Should().Be(Currency.FromCode("EUR"));
        quoteCurrency.Should().Be(Currency.FromCode("USD"));
    }
}
