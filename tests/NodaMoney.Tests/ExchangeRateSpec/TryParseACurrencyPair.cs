using NodaMoney.Exchange;
using NodaMoney.Tests.Helpers;

namespace NodaMoney.Tests.ExchangeRateSpec;

[Collection(nameof(NoParallelization))]
public class TryParseACurrencyPair
{
    [Fact, UseCulture("en-US")]
    public void WhenCurrencyPairInUsCulture_ThenParsingShouldSucceed()
    {
        ExchangeRate fx1;
        var succeeded1 = ExchangeRate.TryParse("EUR/USD 1.2591", out fx1);

        succeeded1.Should().BeTrue();
        fx1.BaseCurrency.Code.Should().Be("EUR");
        fx1.QuoteCurrency.Code.Should().Be("USD");
        fx1.Value.Should().Be(1.2591M);

        ExchangeRate fx2;
        var succeeded2 = ExchangeRate.TryParse("EUR/USD1.2591", out fx2);

        succeeded2.Should().BeTrue();
        fx2.BaseCurrency.Code.Should().Be("EUR");
        fx2.QuoteCurrency.Code.Should().Be("USD");
        fx2.Value.Should().Be(1.2591M);
    }

    [Fact, UseCulture("nl-NL")]
    public void WhenCurrencyPairInNlCulture_ThenParsingShouldSucceed()
    {
        ExchangeRate fx1;
        var succeeded1 = ExchangeRate.TryParse("EUR/USD 1,2591", out fx1);

        succeeded1.Should().BeTrue();
        fx1.BaseCurrency.Code.Should().Be("EUR");
        fx1.QuoteCurrency.Code.Should().Be("USD");
        fx1.Value.Should().Be(1.2591M);

        ExchangeRate fx2;
        var succeeded2 = ExchangeRate.TryParse("EUR/USD1,2591", out fx2);

        succeeded2.Should().BeTrue();
        fx2.BaseCurrency.Code.Should().Be("EUR");
        fx2.QuoteCurrency.Code.Should().Be("USD");
        fx2.Value.Should().Be(1.2591M);
    }

    [Fact]
    public void WhenCurrencyPairIsNotANumber_ThenParsingFails()
    {
        ExchangeRate fx;
        var succeeded = ExchangeRate.TryParse("EUR/USD 1,ABC", out fx);

        succeeded.Should().BeFalse();
        fx.BaseCurrency.Code.Should().Be("XXX");
        fx.QuoteCurrency.Code.Should().Be("XXX");
        fx.Value.Should().Be(0M);

        fx.Should().Be(default(ExchangeRate));
    }

    [Fact, UseCulture("en-US")]
    public void WhenCurrencyPairHasSameCurrencies_ThenParsingFails()
    {
        ExchangeRate fx;
        var succeeded = ExchangeRate.TryParse("EUR/EUR 1.2591", out fx);

        succeeded.Should().BeFalse();
        fx.BaseCurrency.Code.Should().Be("XXX");
        fx.QuoteCurrency.Code.Should().Be("XXX");
        fx.Value.Should().Be(0M);

        fx.Should().Be(default(ExchangeRate));
    }

    [Fact]
    public void WhenCurrencyPairIsNull_ThenParsingFails()
    {
        ExchangeRate fx;
        var succeeded = ExchangeRate.TryParse(null, out fx);

        succeeded.Should().BeFalse();
        fx.BaseCurrency.Code.Should().Be("XXX");
        fx.QuoteCurrency.Code.Should().Be("XXX");
        fx.Value.Should().Be(0M);

        fx.Should().Be(default(ExchangeRate));
    }

    [Fact]
    public void WhenCurrencyPairIsEmpty_ThenParsingFails()
    {
        ExchangeRate fx;
        var succeeded = ExchangeRate.TryParse("", out fx);

        succeeded.Should().BeFalse();
        fx.BaseCurrency.Code.Should().Be("XXX");
        fx.QuoteCurrency.Code.Should().Be("XXX");
        fx.Value.Should().Be(0M);

        fx.Should().Be(default(ExchangeRate));
    }
}
