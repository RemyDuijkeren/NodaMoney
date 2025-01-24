using System.Threading;
using FluentAssertions;
using NodaMoney.Tests.Helpers;
using Xunit;

namespace NodaMoney.Tests.MoneyParsableSpec;

[Collection(nameof(NoParallelization))]
public class TryParseImplicitCurrency
{
    [Fact, UseCulture(null)]
    public void WhenInvariant_ThenThisShouldSucceed()
    {
        Thread.CurrentThread.CurrentCulture.Name.Should().Be(""); // InvariantCulture

        Money euro;
        Money.TryParse("€ 765.43", out euro).Should().BeTrue();

        euro.Should().Be(new Money(765.43m, "EUR"));
    }

    [Fact, UseCulture("nl-BE")]
    public void WhenInBelgiumDutchSpeaking_ThenThisShouldSucceed()
    {
        Thread.CurrentThread.CurrentCulture.Name.Should().Be("nl-BE");
        Money euro;
        Money.TryParse("€ 765,43", out euro).Should().BeTrue();

        euro.Should().Be(new Money(765.43m, "EUR"));
    }

    [Fact, UseCulture("fr-BE")]
    public void WhenInBelgiumFrenchSpeaking_ThenThisShouldSucceed()
    {
        Thread.CurrentThread.CurrentCulture.Name.Should().Be("fr-BE");
        Money euro;
        Money.TryParse("765,43 €", out euro).Should().BeTrue();

        euro.Should().Be(new Money(765.43, "EUR"));
    }

    [Fact, UseCulture("nl-NL")]
    public void WhenParsingNumberWithoutCurrency_ThenThisUseCurrencyOfCurrentCulture()
    {
        Thread.CurrentThread.CurrentCulture.Name.Should().Be("nl-NL");
        Money euro;
        Money.TryParse("765,43", out euro).Should().BeTrue();

        euro.Should().Be(new Money(765.43, "EUR"));
    }

    [Fact, UseCulture("ja-JP")]
    public void WhenParsingYenYuanSymbolInJapan_ThenThisShouldReturnJapaneseYen()
    {
        Thread.CurrentThread.CurrentCulture.Name.Should().Be("ja-JP");
        Money yen;
        Money.TryParse("¥ 765", out yen).Should().BeTrue();

        yen.Should().Be(new Money(765m, "JPY"));
    }

    [Fact, UseCulture("zh-CN")]
    public void WhenParsingYenYuanSymbolInChina_ThenThisShouldReturnChineseYuan()
    {
        Thread.CurrentThread.CurrentCulture.Name.Should().Be("zh-CN");
        Money yuan;
        Money.TryParse("¥ 765", out yuan).Should().BeTrue();

        yuan.Should().Be(new Money(765m, "CNY"));
    }

    [Fact, UseCulture("nl-NL")]
    public void WhenParsingYenYuanInNetherlands_ThenThisShouldReturnFalse()
    {
        Thread.CurrentThread.CurrentCulture.Name.Should().Be("nl-NL");
        // ¥ symbol is used for Japanese yen and Chinese yuan
        Money money;
        Money.TryParse("¥ 765", out money).Should().BeFalse();

        money.Should().Be(new Money(0m, Currency.FromCode("XXX")));
    }

    [Fact, UseCulture("en-US")]
    public void WhenParsingDollarSymbolInUSA_ThenThisShouldReturnUSDollar()
    {
        Thread.CurrentThread.CurrentCulture.Name.Should().Be("en-US");
        Money dollar;
        Money.TryParse("$765.43", out dollar).Should().BeTrue();

        dollar.Should().Be(new Money(765.43m, "USD"));
    }

    [Fact, UseCulture("es-AR")]
    public void WhenParsingDollarSymbolInArgentina_ThenThisShouldReturnArgentinePeso()
    {
        Thread.CurrentThread.CurrentCulture.Name.Should().Be("es-AR");
        Money peso;
        Money.TryParse("$765,43", out peso).Should().BeTrue();

        peso.Should().Be(new Money(765.43m, "ARS"));
    }

    [Fact, UseCulture("nl-NL")]
    public void WhenParsingDollarSymbolInNetherlands_ThenThisShouldReturnFalse()
    {
        Thread.CurrentThread.CurrentCulture.Name.Should().Be("nl-NL");
        // $ symbol is used for multiple currencies
        Money money;
        Money.TryParse("$ 765,43", out money).Should().BeFalse();

        money.Should().Be(new Money(0m, Currency.FromCode("XXX")));
    }

    [Fact, UseCulture("nl-NL")]
    public void WhenValueIsNull_ThenReturnFalse()
    {
        Thread.CurrentThread.CurrentCulture.Name.Should().Be("nl-NL");
        Money money;
        Money.TryParse(null, out money).Should().BeFalse();

        money.Should().Be(new Money(0m, Currency.FromCode("XXX")));
    }

    [Fact, UseCulture("nl-NL")]
    public void WhenValueIsEmpty_ThenReturnFalse()
    {
        Thread.CurrentThread.CurrentCulture.Name.Should().Be("nl-NL");
        Money money;
        Money.TryParse("", out money).Should().BeFalse();

        money.Should().Be(new Money(0m, Currency.FromCode("XXX")));
    }
}
