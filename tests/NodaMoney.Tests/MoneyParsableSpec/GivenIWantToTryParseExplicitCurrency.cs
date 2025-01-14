using FluentAssertions;
using NodaMoney.Tests.Helpers;
using Xunit;

namespace NodaMoney.Tests.MoneyParsableSpec;

[Collection(nameof(NoParallelization))]
public class GivenIWantToTryParseExplicitCurrency
{
    [Fact, UseCulture("nl-NL")]
    public void WhenParsingYenInNetherlands_ThenThisShouldSucceed()
    {
            Money yen;
            Money.TryParse("¥ 765", Currency.FromCode("JPY"), out yen).Should().BeTrue();

            yen.Should().Be(new Money(765, "JPY"));
        }

    [Fact, UseCulture("en-US")]
    public void WhenParsingArgentinePesoInUSA_ThenThisShouldReturnArgentinePeso()
    {
            Money peso;
            Money.TryParse("$765.43", Currency.FromCode("ARS"), out peso).Should().BeTrue();

            peso.Should().Be(new Money(765.43m, "ARS"));
        }

    [Fact, UseCulture("es-AR")]
    public void WhenParsingUSDollarSymbolInArgentina_ThenThisShouldReturnUSDollar()
    {
            Money dollar;
            Money.TryParse("$765,43", Currency.FromCode("USD"), out dollar).Should().BeTrue();

            dollar.Should().Be(new Money(765.43m, "USD"));
        }

    [Fact, UseCulture("nl-NL")]
    public void WhenParsingUSDollarInNetherlands_ThenThisShouldSucceed()
    {
            // $ symbol is used for multiple currencies
            Money dollar;
            Money.TryParse("$765,43", Currency.FromCode("USD"), out dollar).Should().BeTrue();

            dollar.Should().Be(new Money(765.43m, "USD"));
        }

    [Fact, UseCulture("nl-BE")]
    public void WhenInBelgiumDutchSpeaking_ThenThisShouldSucceed()
    {
            Money euro;
            Money.TryParse("€ 765,43", Currency.FromCode("EUR"), out euro).Should().BeTrue();

            euro.Should().Be(new Money(765.43m, "EUR"));
        }

    [Fact, UseCulture("fr-BE")]
    public void WhenInBelgiumFrenchSpeaking_ThenThisShouldSucceed()
    {
            Money euro;
            Money.TryParse("765,43 €", Currency.FromCode("EUR"), out euro).Should().BeTrue();

            euro.Should().Be(new Money(765.43, "EUR"));
        }

    [Fact, UseCulture("nl-NL")]
    public void WhenParsingNumberWithoutCurrency_ThenThisShouldSucceed()
    {
            Money euro;
            Money.TryParse("765,43", Currency.FromCode("USD"), out euro).Should().BeTrue();

            euro.Should().Be(new Money(765.43, "USD"));
        }

    [Fact, UseCulture("nl-NL")]
    public void WhenParsingUSDollarWithEuroCurrency_ThenThisShouldReturnFalse()
    {
            Money money;
            Money.TryParse("€ 765,43", Currency.FromCode("USD"), out money).Should().BeFalse();

            money.Should().Be(new Money(0m, Currency.FromCode("XXX")));
        }
}
