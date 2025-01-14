using FluentAssertions;
using NodaMoney.Tests.Helpers;
using Xunit;

namespace NodaMoney.Tests.MoneyParsableSpec;

[Collection(nameof(NoParallelization))]
public class GivenIWantToTryParseImplicitCurrency
{
    [Fact, UseCulture("nl-BE")]
    public void WhenInBelgiumDutchSpeaking_ThenThisShouldSucceed()
    {
        Money euro;
        Money.TryParse("€ 765,43", out euro).Should().BeTrue();

        euro.Should().Be(new Money(765.43m, "EUR"));
    }

    [Fact, UseCulture("fr-BE")]
    public void WhenInBelgiumFrenchSpeaking_ThenThisShouldSucceed()
    {
        Money euro;
        Money.TryParse("765,43 €", out euro).Should().BeTrue();

        euro.Should().Be(new Money(765.43, "EUR"));
    }

    [Fact, UseCulture("nl-NL")]
    public void WhenParsingNumberWithoutCurrency_ThenThisUseCurrencyOfCurrentCulture()
    {
        Money euro;
        Money.TryParse("765,43", out euro).Should().BeTrue();

        euro.Should().Be(new Money(765.43, "EUR"));
    }

    [Fact, UseCulture("ja-JP")]
    public void WhenParsingYenYuanSymbolInJapan_ThenThisShouldReturnJapaneseYen()
    {
        Money yen;
        Money.TryParse("¥ 765", out yen).Should().BeTrue();

        yen.Should().Be(new Money(765m, "JPY"));
    }

    [Fact, UseCulture("zh-CN")]
    public void WhenParsingYenYuanSymbolInChina_ThenThisShouldReturnChineseYuan()
    {
        Money yuan;
        Money.TryParse("¥ 765", out yuan).Should().BeTrue();

        yuan.Should().Be(new Money(765m, "CNY"));
    }

    [Fact, UseCulture("nl-NL")]
    public void WhenParsingYenYuanInNetherlands_ThenThisShouldReturnFalse()
    {
        // ¥ symbol is used for Japanese yen and Chinese yuan
        Money money;
        Money.TryParse("¥ 765", out money).Should().BeFalse();

        money.Should().Be(new Money(0m, Currency.FromCode("XXX")));
    }

    [Fact, UseCulture("en-US")]
    public void WhenParsingDollarSymbolInUSA_ThenThisShouldReturnUSDollar()
    {
        Money dollar;
        Money.TryParse("$765.43", out dollar).Should().BeTrue();

        dollar.Should().Be(new Money(765.43m, "USD"));
    }

    [Fact, UseCulture("es-AR")]
    public void WhenParsingDollarSymbolInArgentina_ThenThisShouldReturnArgentinePeso()
    {
        Money peso;
        Money.TryParse("$765,43", out peso).Should().BeTrue();

        peso.Should().Be(new Money(765.43m, "ARS"));
    }

    [Fact, UseCulture("nl-NL")]
    public void WhenParsingDollarSymbolInNetherlands_ThenThisShouldReturnFalse()
    {
        // $ symbol is used for multiple currencies
        Money money;
        Money.TryParse("$ 765,43", out money).Should().BeFalse();

        money.Should().Be(new Money(0m, Currency.FromCode("XXX")));
    }

    [Fact, UseCulture("nl-NL")]
    public void WhenValueIsNull_ThenReturnFalse()
    {
        Money money;
        Money.TryParse(null, out money).Should().BeFalse();

        money.Should().Be(new Money(0m, Currency.FromCode("XXX")));
    }

    [Fact, UseCulture("nl-NL")]
    public void WhenValueIsEmpty_ThenReturnFalse()
    {
        Money money;
        Money.TryParse("", out money).Should().BeFalse();

        money.Should().Be(new Money(0m, Currency.FromCode("XXX")));
    }
}
