using System;
using FluentAssertions;
using Xunit;

namespace NodaMoney.Tests.ExchangeRateSpec;

public class GivenIWantToConvertMoney
{
    private readonly Currency _euro = Currency.FromCode("EUR");

    private readonly Currency _dollar = Currency.FromCode("USD");

    private ExchangeRate _exchangeRate = new ExchangeRate(Currency.FromCode("EUR"), Currency.FromCode("USD"), 1.2591);
    // EUR/USD 1.2591

    [Fact]
    public void WhenConvertingEurosToDollars_ThenConversionShouldBeCorrect()
    {
        // When Converting €100,99 With EUR/USD 1.2591, Then Result Should Be $127.16

        // Convert €100,99 to $127.156509 (= €100.99 * 1.2591)
        var converted = _exchangeRate.Convert(Money.Euro(100.99M));

        converted.Currency.Should().Be(_dollar);
        converted.Amount.Should().Be(127.16M);
    }

    [Fact]
    public void WhenConvertingEurosToDollarsAndThenBack_ThenEndResultShouldBeTheSameAsInTheBeginning()
    {
        // Convert €100,99 to $127.156509 (= €100.99 * 1.2591)
        var converted = _exchangeRate.Convert(Money.Euro(100.99M));

        // Convert $127.16 to €100,99 (= $127.16 / 1.2591)
        var revert = _exchangeRate.Convert(converted);

        revert.Currency.Should().Be(_euro);
        revert.Amount.Should().Be(100.99M);
    }

    [Fact]
    public void WhenConvertingWithExchangeRateWithDifferentCurrencies_ThenThrowException()
    {
        // Arrange, Act
        Action action = () => _exchangeRate.Convert(Money.Yen(324));

        // Assert
        action.Should().Throw<ArgumentException>().WithMessage("Money should have the same currency as the base currency or the quote currency!*");
    }
}
