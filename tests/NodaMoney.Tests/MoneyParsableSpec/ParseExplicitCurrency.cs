using System.Globalization;
using NodaMoney.Tests.Helpers;

namespace NodaMoney.Tests.MoneyParsableSpec;

[Collection(nameof(NoParallelization))]
public class ParseExplicitCurrency
{
    [Fact, UseCulture("nl-NL")]
    public void WhenParsingYenInNetherlands_ThenThisShouldSucceed()
    {
        var yen = Money.Parse("¥ -98765", CurrencyInfo.FromCode("JPY"));

        yen.Should().Be(new Money(-98_765, "JPY"));
    }

    [Fact, UseCulture("en-US")]
    public void WhenParsingArgentinePesoInUSA_ThenThisShouldReturnArgentinePeso()
    {
        var peso = Money.Parse("$-98,765.43", CurrencyInfo.FromCode("ARS"));

        peso.Should().Be(new Money(-98_765.43m, "ARS"));
    }

    [Fact, UseCulture("es-AR")]
    public void WhenParsingUSDollarSymbolInArgentina_ThenThisShouldReturnUSDollar()
    {
        var dollar = Money.Parse("$-98.765,43", CurrencyInfo.FromCode("USD"));

        dollar.Should().Be(new Money(-98_765.43m, "USD"));
    }

    [Fact, UseCulture("nl-NL")]
    public void WhenParsingUSDollarInNetherlands_ThenThisShouldSucceed()
    {
        // $ symbol is used for multiple currencies
        var dollar = Money.Parse("$-98.765,43", CurrencyInfo.FromCode("USD"));

        dollar.Should().Be(new Money(-98_765.43m, "USD"));
    }

    [Fact, UseCulture("nl-NL")]
    public void WhenParsingSwissFrancInNetherlands_ThenThisShouldSucceed()
    {
        var money = Money.Parse("CHF-98.765,43", CurrencyInfo.FromCode("CHF"));

        money.Should().Be(new Money(-98_765.43m, "CHF"));
    }

    [Fact, UseCulture("nl-BE")]
    public void WhenInBelgiumDutchSpeaking_ThenThisShouldSucceed()
    {
        var euro = Money.Parse("€ -98.765,43", CurrencyInfo.FromCode("EUR"));

        euro.Should().Be(new Money(-98_765.43m, "EUR"));
    }

    [Fact, UseCulture("fr-BE")]
    public void WhenInBelgiumFrenchSpeaking_ThenThisShouldSucceed()
    {
        var euro = Money.Parse("-98 765,43 €", CurrencyInfo.FromCode("EUR"));

        euro.Should().Be(new Money(-98_765.43, "EUR"));
    }


    [Fact, UseCulture("de-CH")]
    public void WhenParsingSwissFrancInSwitzerlandGermanSpeaking_ThenThisShouldSucceed()
    {
        var money = Money.Parse("Fr.-98’765.43", CurrencyInfo.FromCode("CHF"));

        money.Should().Be(new Money(-98_765.43m, "CHF"));
    }

    [Fact, UseCulture("de-CH")]
    public void WhenParsingSwissFrancInSwitzerlandGermanSpeakingInternationalSymbol_ThenThisShouldSucceed()
    {
        var money = Money.Parse("CHF-98’765.43", CurrencyInfo.FromCode("CHF"));

        money.Should().Be(new Money(-98_765.43m, "CHF"));
    }

    [Fact, UseCulture("nl-NL")]
    public void WhenParsingNumberWithoutCurrency_ThenThisShouldSucceed()
    {
        var euro = Money.Parse("-98.765,43", CurrencyInfo.FromCode("USD"));

        euro.Should().Be(new Money(-98_765.43, "USD"));
    }

    [Fact, UseCulture("nl-NL")]
    public void WhenParsingUSDollarWithEuroCurrency_ThenThisShouldFail()
    {
        Action action = () => Money.Parse("€ -98.765,43", CurrencyInfo.FromCode("USD"));

        action.Should().Throw<FormatException>("given provider always overrule, even current culture");
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
}
