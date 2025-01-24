using System;
using FluentAssertions;
using NodaMoney.Exchange;
using Xunit;

namespace NodaMoney.Tests.ExchangeRateSpec;

public class CreateAnExchangeRateWithCurrencies
{
    private readonly Currency _euro = Currency.FromCode("EUR");

    private readonly Currency _dollar = Currency.FromCode("USD");

    [Fact]
    public void WhenRateIsDoubleAndNoNumberRoundingDecimalsIsGiven_ThenCreatingShouldSucceedWithValueRoundedToSixDecimals()
    {
        var fx = new ExchangeRate(_euro, _dollar, 1.2591478D);

        fx.BaseCurrency.Should().Be(_euro);
        fx.QuoteCurrency.Should().Be(_dollar);
        fx.Value.Should().Be(1.259148M);
    }

    [Fact]
    public void WhenRateIsDoubleAndANumberOfRoundingDecimalsIsGiven_ThenCreatingShouldSucceedWithValueRoundedThatNumberOfDecimals()
    {
        var fx = new ExchangeRate(_euro, _dollar, 1.2591478D, 3);

        fx.BaseCurrency.Should().Be(_euro);
        fx.QuoteCurrency.Should().Be(_dollar);
        fx.Value.Should().Be(1.259M);
    }

    [Fact]
    public void WhenRateIsDecimal_ThenCreatingShouldSucceed()
    {
        var fx = new ExchangeRate(_euro, _dollar, 1.2591M);

        fx.BaseCurrency.Should().Be(_euro);
        fx.QuoteCurrency.Should().Be(_dollar);
        fx.Value.Should().Be(1.2591M);
    }

    [Fact]
    public void WhenRateIsFloatAndNoNumberRoundingDecimalsIsGiven_ThenCreatingShouldSucceedWithValueRoundedToSixDecimals()
    {
        var fx = new ExchangeRate(_euro, _dollar, 1.2591478F);

        fx.BaseCurrency.Should().Be(_euro);
        fx.QuoteCurrency.Should().Be(_dollar);
        fx.Value.Should().Be(1.259148M);
    }

    [Fact]
    public void WhenRateIsFloatAndANumberOfRoundingDecimalsIsGiven_ThenCreatingShouldSucceedWithValueRoundedThatNumberOfDecimals()
    {
        var fx = new ExchangeRate(_euro, _dollar, 1.2591478F, 3);

        fx.BaseCurrency.Should().Be(_euro);
        fx.QuoteCurrency.Should().Be(_dollar);
        fx.Value.Should().Be(1.259M);
    }

    [Fact]
    public void WhenBaseAndQuoteCurrencyAreTheSame_ThenThrowException()
    {
        // Arrange, Act
        Action action = () => new ExchangeRate(_euro, _euro, 1.2591F);

        // Assert
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void WhenRateIsLessThenZero_ThenThrowException()
    {
        // Arrange, Act
        Action action = () => new ExchangeRate(_euro, _dollar, -1.2F);

        // Assert
        action.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void WhenBaseCurrencyIsDefault_ThenCreatingShouldSucceed()
    {
        var fx = new ExchangeRate(default, _dollar, 1.2591M);

        fx.BaseCurrency.Should().Be(default(Currency));
        fx.QuoteCurrency.Should().Be(_dollar);
        fx.Value.Should().Be(1.2591M);
    }

    [Fact]
    public void WhenQuoteCurrencyIsDefault_ThenCreatingShouldSucceed()
    {
        var fx = new ExchangeRate(_euro, default, 1.2591M);

        fx.BaseCurrency.Should().Be(_euro);
        fx.QuoteCurrency.Should().Be(default(Currency));
        fx.Value.Should().Be(1.2591M);
    }
}
