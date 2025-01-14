using FluentAssertions;
using NodaMoney.Tests.Helpers;
using Xunit;

namespace NodaMoney.Tests.ExchangeRateSpec;

[Collection(nameof(NoParallelization))]
public class GivenIWantToConvertExchangeRateToString
{
    ExchangeRate fx = new ExchangeRate(Currency.FromCode("EUR"), Currency.FromCode("USD"), 1.2524);

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
}
