using System;
using FluentAssertions;
using NodaMoney.Exchange;
using Xunit;

namespace NodaMoney.Tests.ExchangeRateSpec;

public class CreateAnExchangeRateWithCurrenciesAsStrings
{
    private readonly string _euroAsString = "EUR";

    private readonly string _dollarAsString = "USD";

    private readonly Currency _euro = Currency.FromCode("EUR");

    private readonly Currency _dollar = Currency.FromCode("USD");

    [Fact]
    public void WhenRateIsDoubleAndNoNumberRoundingDecimalsIsGiven_ThenCreatingShouldSucceedWithValueRoundedToSixDecimals()
    {
        var fx = new ExchangeRate(_euroAsString, _dollarAsString, 1.2591478D);

        fx.BaseCurrency.Should().Be(_euro);
        fx.QuoteCurrency.Should().Be(_dollar);
        fx.Value.Should().Be(1.259148M);
    }

    [Fact]
    public void WhenRateIsDoubleAndANumberOfRoundingDecimalsIsGiven_ThenCreatingShouldSucceedWithValueRoundedThatNumberOfDecimals()
    {
        var fx = new ExchangeRate(_euroAsString, _dollarAsString, 1.2591478D, 3);

        fx.BaseCurrency.Should().Be(_euro);
        fx.QuoteCurrency.Should().Be(_dollar);
        fx.Value.Should().Be(1.259M);
    }

    [Fact]
    public void WhenRateIsDecimal_ThenCreatingShouldSucceed()
    {
        var fx = new ExchangeRate(_euroAsString, _dollarAsString, 1.2591M);

        fx.BaseCurrency.Should().Be(_euro);
        fx.QuoteCurrency.Should().Be(_dollar);
        fx.Value.Should().Be(1.2591M);
    }

    [Fact]
    public void WhenRateIsFloatAndNoNumberOfRoundingDecimalsIsGiven_ThenCreatingShouldSucceedWithValueRoundedToSixDecimals()
    {
        var fx = new ExchangeRate(_euroAsString, _dollarAsString, 1.2591478F);

        fx.BaseCurrency.Should().Be(_euro);
        fx.QuoteCurrency.Should().Be(_dollar);
        fx.Value.Should().Be(1.259148M);
    }

    [Fact]
    public void WhenRateIsFloatAndANumberOfRoundingDecimalsIsGiven_ThenCreatingShouldSucceedWithValueRoundedThatNumberOfDecimals()
    {
        var fx = new ExchangeRate(_euroAsString, _dollarAsString, 1.2591478F, 3);

        fx.BaseCurrency.Should().Be(_euro);
        fx.QuoteCurrency.Should().Be(_dollar);
        fx.Value.Should().Be(1.259M);
    }

    [Fact]
    public void WhenBaseAndQuoteCurrencyAreTheSame_ThenCreatingShouldThrow()
    {
        // Arrange, Act
        Action action = () => new ExchangeRate(_euroAsString, _euroAsString, 1.2591F);

        // Assert
        action.Should().Throw<ArgumentException>();
    }
}
