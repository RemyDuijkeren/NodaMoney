using System;
using System.Globalization;
using FluentAssertions;
using NodaMoney.Tests.Helpers;
using Xunit;

namespace NodaMoney.Tests.MoneyParsableSpec;

[Collection(nameof(NoParallelization))]
public class ParseExplicitCurrency
{
    [Fact, UseCulture("nl-NL")]
    public void WhenParsingYenInNetherlands_ThenThisShouldSucceed()
    {
        var yen = Money.Parse("¥ 765", CurrencyInfo.FromCode("JPY"));

        yen.Should().Be(new Money(765, "JPY"));
    }

    [Fact, UseCulture("en-US")]
    public void WhenParsingArgentinePesoInUSA_ThenThisShouldReturnArgentinePeso()
    {
        var peso = Money.Parse("$765.43", CurrencyInfo.FromCode("ARS"));

        peso.Should().Be(new Money(765.43m, "ARS"));
    }

    [Fact, UseCulture("es-AR")]
    public void WhenParsingUSDollarSymbolInArgentina_ThenThisShouldReturnUSDollar()
    {
        var dollar = Money.Parse("$765,43", CurrencyInfo.FromCode("USD"));

        dollar.Should().Be(new Money(765.43m, "USD"));
    }

    [Fact, UseCulture("nl-NL")]
    public void WhenParsingUSDollarInNetherlands_ThenThisShouldSucceed()
    {
        // $ symbol is used for multiple currencies
        var dollar = Money.Parse("$765,43", CurrencyInfo.FromCode("USD"));

        dollar.Should().Be(new Money(765.43m, "USD"));
    }

    [Fact, UseCulture("nl-BE")]
    public void WhenInBelgiumDutchSpeaking_ThenThisShouldSucceed()
    {
        var euro = Money.Parse("€ 765,43", CurrencyInfo.FromCode("EUR"));

        euro.Should().Be(new Money(765.43m, "EUR"));
    }

    [Fact, UseCulture("fr-BE")]
    public void WhenInBelgiumFrenchSpeaking_ThenThisShouldSucceed()
    {
        var euro = Money.Parse("765,43 €", CurrencyInfo.FromCode("EUR"));

        euro.Should().Be(new Money(765.43, "EUR"));
    }

    [Fact, UseCulture("nl-NL")]
    public void WhenParsingNumberWithoutCurrency_ThenThisShouldSucceed()
    {
        var euro = Money.Parse("765,43", CurrencyInfo.FromCode("USD"));

        euro.Should().Be(new Money(765.43, "USD"));
    }

    [Fact, UseCulture("nl-NL")]
    public void WhenParsingUSDollarWithEuroCurrency_ThenThisShouldFail()
    {
        Action action = () => Money.Parse("€ 765,43", CurrencyInfo.FromCode("USD"));

        action.Should().Throw<FormatException>(); //.WithMessage("Input string was not in a correct format.");
    }

    [Fact, UseCulture("nl-NL")]
    public void WhenValueIsNull_ThenThrowException()
    {
        Action action = () => Money.Parse((string)null, CurrencyInfo.FromCode("EUR"));

        action.Should().Throw<ArgumentNullException>();
    }

    [Fact, UseCulture("nl-NL")]
    public void WhenValueIsEmpty_ThenThrowFormatException()
    {
        Action action = () => Money.Parse("", CurrencyInfo.FromCode("EUR"));

        action.Should().Throw<FormatException>().WithMessage("*not in a correct format*");
    }

    [Fact, UseCulture("nl-NL")]
    public void WhenValueIsNullWithOverrideMethod_ThenThrowException()
    {
        Action action = () => Money.Parse((string)null, NumberStyles.Currency, CurrencyInfo.FromCode("EUR"));

        action.Should().Throw<ArgumentNullException>();
    }

    [Fact, UseCulture("nl-NL")]
    public void WhenValueIsEmptyWithOverrideMethod_ThenThrowFormatException()
    {
        Action action = () => Money.Parse("", NumberStyles.Currency, CurrencyInfo.FromCode("EUR"));

        action.Should().Throw<FormatException>().WithMessage("*not in a correct format*");
    }
}
