using System;
using FluentAssertions;
using NodaMoney.Tests.Helpers;
using Xunit;

namespace NodaMoney.Tests.MoneyParsableSpec;

[Collection(nameof(NoParallelization))]
public class ParseImplicitCurrency
{
    [Fact, UseCulture("nl-BE")]
    public void WhenInBelgiumDutchSpeaking_ThenThisShouldSucceed()
    {
        var euro = Money.Parse("€ -98.765,43");

        euro.Should().Be(new Money(-98_765.43m, "EUR"));
    }

    [Fact, UseCulture("fr-BE")]
    public void WhenInBelgiumFrenchSpeaking_ThenThisShouldSucceed()
    {
        var euro = Money.Parse("-98 765,43 €");

        euro.Should().Be(new Money(-98_765.43m, "EUR"));
    }

    [Fact, UseCulture("nl-NL")]
    public void WhenParsingNumberWithoutCurrency_ThenThisUseCurrencyOfCurrentCulture()
    {
        var euro = Money.Parse("-98.765,43");

        euro.Should().Be(new Money(-98_765.43m, "EUR"));
    }

    [Fact, UseCulture("ja-JP")]
    public void WhenParsingYenYuanSymbolInJapan_ThenThisShouldReturnJapaneseYen()
    {
        var yen = Money.Parse("¥ -98,765");

        yen.Should().Be(new Money(-98_765m, "JPY"));
    }

    [Fact, UseCulture("zh-CN")]
    public void WhenParsingYenYuanSymbolInChina_ThenThisShouldReturnChineseYuan()
    {
        var yuan = Money.Parse("¥ -98,765");

        yuan.Should().Be(new Money(-98_765m, "CNY"));
    }

    [Fact, UseCulture("nl-NL")]
    public void WhenParsingYenYuanInNetherlands_ThenThisShouldFail()
    {
        // ¥ symbol is used for Japanese yen and Chinese yuan
        Action action = () => Money.Parse("¥ -98,765");

        action.Should().Throw<FormatException>().WithMessage("*multiple currencies*");
    }

    [Fact, UseCulture("en-US")]
    public void WhenParsingDollarSymbolInUSA_ThenThisShouldReturnUSDollar()
    {
        var dollar = Money.Parse("$-98,765.43");

        dollar.Should().Be(new Money(-98_765.43m, "USD"));
    }

    [Fact, UseCulture("es-AR")]
    public void WhenParsingDollarSymbolInArgentina_ThenThisShouldReturnArgentinePeso()
    {
        var peso = Money.Parse("$-98.765,43");

        peso.Should().Be(new Money(-98_765.43m, "ARS"));
    }

    [Fact, UseCulture("nl-NL")]
    public void WhenParsingDollarSymbolInNetherlands_ThenThisShouldFail()
    {
        // $ symbol is used for multiple currencies
        Action action = () => Money.Parse("$ 765,43");

        action.Should().Throw<FormatException>().WithMessage("*multiple currencies*");
    }

    [Fact, UseCulture("en-US")]
    public void WhenParsingEuroSymbolInUSA_ThenThisShouldReturnEuro()
    {
        var euro = Money.Parse("€ -98,765.43");

        euro.Should().Be(new Money(-98_765.43m, "EUR"));
    }

    [Fact, UseCulture("de-CH")]
    public void WhenParsingChfSymbolInSwitzerlandGermanSpeaking_ThenThisShouldReturnSwissFranc()
    {
        var money = Money.Parse("-98’765.23 Fr.");

        money.Should().Be(new Money(-98_765.23m, "CHF"));
    }

    [Fact, UseCulture("de-CH")]
    public void WhenParsingChfInternationalSymbolInSwitzerlandGermanSpeaking_ThenThisShouldReturnSwissFranc()
    {
        var money = Money.Parse("-98’765.23 CHF");

        money.Should().Be(new Money(-98_765.23m, "CHF"));
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
