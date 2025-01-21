using FluentAssertions;
using NodaMoney.Tests.Helpers;
using Xunit;

namespace NodaMoney.Tests.MoneyParsableSpec;

[Collection(nameof(NoParallelization))]
public class TryParseExplicitCurrency
{
    [Fact, UseCulture("nl-NL")]
    public void WhenParsingYenInNetherlands_ThenThisShouldSucceed()
    {
        // Arrange
        CurrencyInfo currencyInfo = CurrencyInfo.FromCode("JPY");

        // Act
        bool parseResult = Money.TryParse("¥ 765", currencyInfo, out Money yen);

        // Assert
        parseResult.Should().BeTrue();
        yen.Should().Be(new Money(765, currencyInfo));
    }

    [Fact, UseCulture("en-US")]
    public void WhenParsingArgentinePesoInUSA_ThenThisShouldReturnArgentinePeso()
    {
        // Arrange
        CurrencyInfo currencyInfo = CurrencyInfo.FromCode("ARS");

        // Act
        bool parseResult = Money.TryParse("$765.43", currencyInfo, out Money peso);

        // Assert
        parseResult.Should().BeTrue();
        peso.Should().Be(new Money(765.43m, currencyInfo));
    }

    [Fact, UseCulture("es-AR")]
    public void WhenParsingUSDollarSymbolInArgentina_ThenThisShouldReturnUSDollar()
    {
        // Arrange
        CurrencyInfo currencyInfo = CurrencyInfo.FromCode("USD");

        // Act
        bool parseResult = Money.TryParse("$765,43", currencyInfo, out Money dollar);

        // Assert
        parseResult.Should().BeTrue();
        dollar.Should().Be(new Money(765.43m, currencyInfo));
    }

    [Fact, UseCulture("nl-NL")]
    public void WhenParsingUSDollarInNetherlands_ThenThisShouldSucceed()
    {
        // Arrange
        CurrencyInfo currencyInfo = CurrencyInfo.FromCode("USD"); // $ symbol is used for multiple currencies

        // Act
        bool parseResult = Money.TryParse("$765,43", currencyInfo, out Money dollar);

        // Assert
        parseResult.Should().BeTrue();
        dollar.Should().Be(new Money(765.43m, currencyInfo));
    }

    [Fact, UseCulture("nl-BE")]
    public void WhenInBelgiumDutchSpeaking_ThenThisShouldSucceed()
    {
        // Arrange
        CurrencyInfo currencyInfo = CurrencyInfo.FromCode("EUR");

        // Act
        bool parseResult = Money.TryParse("€ 765,43", currencyInfo, out Money euro);

        // Assert
        parseResult.Should().BeTrue();
        euro.Should().Be(new Money(765.43m, currencyInfo));
    }

    [Fact, UseCulture("fr-BE")]
    public void WhenInBelgiumFrenchSpeaking_ThenThisShouldSucceed()
    {
        // Arrange
        CurrencyInfo currencyInfo = CurrencyInfo.FromCode("EUR");

        // Act
        bool parseResult = Money.TryParse("765,43 €", currencyInfo, out Money euro);

        // Assert
        parseResult.Should().BeTrue();
        euro.Should().Be(new Money(765.43, currencyInfo));
    }

    [Fact, UseCulture("nl-NL")]
    public void WhenParsingNumberWithoutCurrency_ThenThisShouldSucceed()
    {
        // Arrange
        CurrencyInfo currencyInfo = CurrencyInfo.FromCode("USD");

        // Act
        bool parseResult = Money.TryParse("765,43", currencyInfo, out Money euro);

        // Assert
        parseResult.Should().BeTrue();
        euro.Should().Be(new Money(765.43, currencyInfo));
    }

    [Fact, UseCulture("nl-NL")]
    public void WhenParsingUSDollarWithEuroCurrency_ThenThisShouldReturnFalse()
    {
        // Arrange
        CurrencyInfo currencyInfo = CurrencyInfo.FromCode("USD");

        // Act
        bool parseResult = Money.TryParse("€ 765,43", currencyInfo, out Money money);

        // Assert
        parseResult.Should().BeFalse();
        money.Should().Be(new Money(0m, CurrencyInfo.NoCurrency));
    }
}
