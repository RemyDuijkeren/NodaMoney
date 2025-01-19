using System;
using FluentAssertions;
using NodaMoney.Tests.Helpers;
using Xunit;

namespace NodaMoney.Tests.MoneyParsableSpec;

[Collection(nameof(NoParallelization))]
public class GivenIWantToParseImplicitCurrency
{
    [Fact, UseCulture("nl-BE")]
    public void WhenInBelgiumDutchSpeaking_ThenThisShouldSucceed()
    {
        var euro = Money.Parse("€ 765,43");

        euro.Should().Be(new Money(765.43m, "EUR"));
    }

    [Fact, UseCulture("fr-BE")]
    public void WhenInBelgiumFrenchSpeaking_ThenThisShouldSucceed()
    {
        var euro = Money.Parse("765,43 €");

        euro.Should().Be(new Money(765.43, "EUR"));
    }

    [Fact, UseCulture("nl-NL")]
    public void WhenParsingNumberWithoutCurrency_ThenThisUseCurrencyOfCurrentCulture()
    {
        var euro = Money.Parse("765,43");

        euro.Should().Be(new Money(765.43, "EUR"));
    }

    [Fact, UseCulture("ja-JP")]
    public void WhenParsingYenYuanSymbolInJapan_ThenThisShouldReturnJapaneseYen()
    {
        var yen = Money.Parse("¥ 765");

        yen.Should().Be(new Money(765m, "JPY"));
    }

    [Fact, UseCulture("zh-CN")]
    public void WhenParsingYenYuanSymbolInChina_ThenThisShouldReturnChineseYuan()
    {
        var yuan = Money.Parse("¥ 765");

        yuan.Should().Be(new Money(765m, "CNY"));
    }

    [Fact, UseCulture("nl-NL")]
    public void WhenParsingYenYuanInNetherlands_ThenThisShouldFail()
    {
        // ¥ symbol is used for Japanese yen and Chinese yuan
        Action action = () => Money.Parse("¥ 765");

        action.Should().Throw<FormatException>().WithMessage("*multiple known currencies*");
    }

    [Fact, UseCulture("en-US")]
    public void WhenParsingDollarSymbolInUSA_ThenThisShouldReturnUSDollar()
    {
        var dollar = Money.Parse("$765.43");

        dollar.Should().Be(new Money(765.43m, "USD"));
    }

    [Fact, UseCulture("es-AR")]
    public void WhenParsingDollarSymbolInArgentina_ThenThisShouldReturnArgentinePeso()
    {
        var peso = Money.Parse("$765,43");

        peso.Should().Be(new Money(765.43m, "ARS"));
    }

    [Fact, UseCulture("nl-NL")]
    public void WhenParsingDollarSymbolInNetherlands_ThenThisShouldFail()
    {
        // $ symbol is used for multiple currencies
        Action action = () => Money.Parse("$ 765,43");

        action.Should().Throw<FormatException>().WithMessage("*multiple known currencies*");
    }

    [Fact, UseCulture("en-US")]
    public void WhenParsingEuroSymbolInUSA_ThenThisShouldReturnEuro()
    {
        var euro = Money.Parse("€765.43");

        euro.Should().Be(new Money(765.43m, "EUR"));
    }

    [Fact]
    public void WhenValueIsNull_ThenThrowException()
    {
        Action action = () => Money.Parse(null);

        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void WhenValueIsEmpty_ThenThrowFormatException()
    {
        Action action = () => Money.Parse("");

        action.Should().Throw<FormatException>().WithMessage("*not in a correct format*");
    }

    [Fact, UseCulture("nl-NL")]
    public void WhenCurrencyIsUnknown_ThenThrowException()
    {
        Action action = () => Money.Parse("XYZ 765,43");

        action.Should().Throw<FormatException>().WithMessage("*unknown currency*");
    }
}
