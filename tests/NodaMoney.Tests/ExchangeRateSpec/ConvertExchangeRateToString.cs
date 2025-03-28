using System.Globalization;
using NodaMoney.Exchange;
using NodaMoney.Tests.Helpers;

namespace NodaMoney.Tests.ExchangeRateSpec;

[Collection(nameof(NoParallelization))]
public class ConvertExchangeRateToString
{
    ExchangeRate fx = new ExchangeRate(CurrencyInfo.FromCode("EUR"), CurrencyInfo.FromCode("USD"), 1.2524);

    [Fact, UseCulture("en-US")]
    public void WhenShowingExchangeRateInAmerica_ThenReturnCurrencyPairWithDot()
    {
        fx.ToString().Should().Be("EUR/USD 1.2524");
    }

    [Fact, UseCulture("nl-NL")]
    public void WhenShowingExchangeRateInNetherlands_ThenReturnCurrencyPairWithComma()
    {
        fx.ToString().Should().Be("EUR/USD 1,2524");
    }

    [Fact, UseCulture("nl-NL")]
    public void WhenShowingExchangeRateInNetherlands_ThenReturnCurrencyPairWithDotWhenInvariantCultureIsSpecified()
    {
        fx.ToString(CultureInfo.InvariantCulture).Should().Be("EUR/USD 1.2524");
    }

    [Fact]
    public void WhenShowingExchangeRateWithSpecifiedCulture_ThenReturnCurrencyAccordingly()
    {
        fx.ToString(CultureInfo.GetCultureInfo("en-US")).Should().Be("EUR/USD 1.2524");
        fx.ToString(CultureInfo.GetCultureInfo("nl-NL")).Should().Be("EUR/USD 1,2524");
    }
}
