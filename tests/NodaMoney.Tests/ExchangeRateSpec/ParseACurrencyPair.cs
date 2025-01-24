using System;
using FluentAssertions;
using NodaMoney.Exchange;
using NodaMoney.Tests.Helpers;
using Xunit;

namespace NodaMoney.Tests.ExchangeRateSpec;

[Collection(nameof(NoParallelization))]
public class ParseACurrencyPair
{
    [Fact, UseCulture("en-US")]
    public void WhenCurrencyPairInUsCulture_ThenParsingShouldSucceed()
    {
        var fx1 = ExchangeRate.Parse("EUR/USD 1.2591");

        fx1.BaseCurrency.Code.Should().Be("EUR");
        fx1.QuoteCurrency.Code.Should().Be("USD");
        fx1.Value.Should().Be(1.2591M);

        var fx2 = ExchangeRate.Parse("EUR/USD1.2591");

        fx2.BaseCurrency.Code.Should().Be("EUR");
        fx2.QuoteCurrency.Code.Should().Be("USD");
        fx2.Value.Should().Be(1.2591M);
    }

    [Fact, UseCulture("nl-NL")]
    public void WhenCurrencyPairInNlCulture_ThenParsingShouldSucceed()
    {
        var fx1 = ExchangeRate.Parse("EUR/USD 1,2591");

        fx1.BaseCurrency.Code.Should().Be("EUR");
        fx1.QuoteCurrency.Code.Should().Be("USD");
        fx1.Value.Should().Be(1.2591M);

        var fx2 = ExchangeRate.Parse("EUR/USD1,2591");

        fx2.BaseCurrency.Code.Should().Be("EUR");
        fx2.QuoteCurrency.Code.Should().Be("USD");
        fx2.Value.Should().Be(1.2591M);
    }

    [Fact]
    public void WhenCurrencyPairIsNotANumber_ThenThrowException()
    {
        Action action = () => ExchangeRate.Parse("EUR/USD 1,ABC");

        action.Should().Throw<FormatException>();
    }

    [Fact, UseCulture("en-US")]
    public void WhenCurrencyPairHasSameCurrencies_ThenThrowException()
    {
        Action action = () => ExchangeRate.Parse("EUR/EUR 1.2591");

        action.Should().Throw<FormatException>();
    }

    [Fact]
    public void WhenCurrencyPairIsNull_ThenThrowException()
    {
        Action action = () => ExchangeRate.Parse(null);

        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void WhenCurrencyPairIsEmpty_ThenThrowException()
    {
        Action action = () => ExchangeRate.Parse("");

        action.Should().Throw<FormatException>();
    }
}
