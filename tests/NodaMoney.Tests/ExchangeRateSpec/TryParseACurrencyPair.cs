using System.Globalization;
using NodaMoney.Exchange;
using NodaMoney.Tests.Helpers;

namespace NodaMoney.Tests.ExchangeRateSpec;

[Collection(nameof(NoParallelization))]
public class TryParseACurrencyPair
{
    [Fact, UseCulture("en-US")]
    public void WhenCurrencyPairInUsCulture_ThenParsingShouldSucceed()
    {
        var succeeded1 = ExchangeRate.TryParse("EUR/USD 1.2591", out ExchangeRate fx1);

        succeeded1.Should().BeTrue();
        fx1.BaseCurrency.Code.Should().Be("EUR");
        fx1.QuoteCurrency.Code.Should().Be("USD");
        fx1.Value.Should().Be(1.2591M);

        var succeeded2 = ExchangeRate.TryParse("EUR/USD1.2591", out ExchangeRate fx2);

        succeeded2.Should().BeTrue();
        fx2.BaseCurrency.Code.Should().Be("EUR");
        fx2.QuoteCurrency.Code.Should().Be("USD");
        fx2.Value.Should().Be(1.2591M);
    }

    [Fact, UseCulture("nl-NL")]
    public void WhenCurrencyPairInNlCulture_ThenParsingShouldSucceed()
    {
        var succeeded1 = ExchangeRate.TryParse("EUR/USD 1,2591", out ExchangeRate fx1);

        succeeded1.Should().BeTrue();
        fx1.BaseCurrency.Code.Should().Be("EUR");
        fx1.QuoteCurrency.Code.Should().Be("USD");
        fx1.Value.Should().Be(1.2591M);

        var succeeded2 = ExchangeRate.TryParse("EUR/USD1,2591", out ExchangeRate fx2);

        succeeded2.Should().BeTrue();
        fx2.BaseCurrency.Code.Should().Be("EUR");
        fx2.QuoteCurrency.Code.Should().Be("USD");
        fx2.Value.Should().Be(1.2591M);
    }

    [Fact, UseCulture("en-US")]
    public void WhenCurrencyPairInNlCulture_ThenParsingShouldSucceedWithCultureSpecified()
    {
        var succeeded = ExchangeRate.TryParse("EUR/USD 1,2591", CultureInfo.GetCultureInfo("nl-NL"), out ExchangeRate fx);

        succeeded.Should().BeTrue();
        fx.BaseCurrency.Code.Should().Be("EUR");
        fx.QuoteCurrency.Code.Should().Be("USD");
        fx.Value.Should().Be(1.2591M);
    }

    [Fact, UseCulture("nl-NL")]
    public void WhenCurrencyPairInNlCulture_ThenParsingShouldFailWithIncompatibleCultureSpecified()
    {
        var succeeded = ExchangeRate.TryParse("EUR/USD 1,2591", NumberStyles.Currency & ~NumberStyles.AllowThousands, CultureInfo.GetCultureInfo("en-US"), out ExchangeRate fx);

        succeeded.Should().BeFalse();
        fx.BaseCurrency.Code.Should().Be("XXX");
        fx.QuoteCurrency.Code.Should().Be("XXX");
        fx.Value.Should().Be(0M);
    }

    [Fact]
    public void WhenCurrencyPairIsNotANumber_ThenParsingFails()
    {
        var succeeded = ExchangeRate.TryParse("EUR/USD 1,ABC", out ExchangeRate fx);

        succeeded.Should().BeFalse();
        fx.BaseCurrency.Code.Should().Be("XXX");
        fx.QuoteCurrency.Code.Should().Be("XXX");
        fx.Value.Should().Be(0M);

        fx.Should().Be(default(ExchangeRate));
    }

    [Fact, UseCulture("en-US")]
    public void WhenCurrencyPairHasSameCurrenciesAndValueEqualTo1_ThenParsingShouldSucceed()
    {
        var succeeded = ExchangeRate.TryParse("EUR/EUR 1", out ExchangeRate fx);

        succeeded.Should().BeTrue();
        fx.BaseCurrency.Code.Should().Be("EUR");
        fx.QuoteCurrency.Code.Should().Be("EUR");
        fx.Value.Should().Be(1M);
    }

    [Fact, UseCulture("en-US")]
    public void WhenCurrencyPairHasSameCurrenciesAndValueNotEqualTo1_ThenParsingFails()
    {
        var succeeded = ExchangeRate.TryParse("EUR/EUR 1.2591", out ExchangeRate fx);

        succeeded.Should().BeFalse();
        fx.BaseCurrency.Code.Should().Be("XXX");
        fx.QuoteCurrency.Code.Should().Be("XXX");
        fx.Value.Should().Be(0M);

        fx.Should().Be(default(ExchangeRate));
    }

    [Fact]
    public void WhenCurrencyPairIsNull_ThenParsingFails()
    {
        var succeeded = ExchangeRate.TryParse(null, out ExchangeRate fx);

        succeeded.Should().BeFalse();
        fx.BaseCurrency.Code.Should().Be("XXX");
        fx.QuoteCurrency.Code.Should().Be("XXX");
        fx.Value.Should().Be(0M);

        fx.Should().Be(default(ExchangeRate));
    }

    [Fact]
    public void WhenCurrencyPairIsEmpty_ThenParsingFails()
    {
        var succeeded = ExchangeRate.TryParse("", out ExchangeRate fx);

        succeeded.Should().BeFalse();
        fx.BaseCurrency.Code.Should().Be("XXX");
        fx.QuoteCurrency.Code.Should().Be("XXX");
        fx.Value.Should().Be(0M);

        fx.Should().Be(default(ExchangeRate));
    }
}
