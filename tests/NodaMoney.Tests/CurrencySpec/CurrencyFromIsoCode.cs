namespace NodaMoney.Tests.CurrencySpec;

public class CurrencyFromIsoCode
{
    [Fact]
    public void WhenIsoCodeIsExisting_ThenCreatingShouldSucceed()
    {
        // Arrange / Act
        Currency currency = Currency.FromCode("EUR");

        // Assert
        currency.Should().NotBeNull();
        currency.Code.Should().Be("EUR");
        currency.Symbol.Should().Be("€");
        currency.IsIso4217.Should().BeTrue();
    }

    [Fact]
    public void WhenNonIsoCode_NonIsoCurrencyIsCreated()
    {
        // Arrange / Act
        Currency currency = Currency.FromCode("BTC");

        // Assert
        currency.Should().NotBeNull();
        currency.Code.Should().Be("BTC");
        currency.Symbol.Should().Be("₿");
        currency.IsIso4217.Should().BeFalse();
    }

    [Fact]
    public void WhenIsoCodeIsUnknown_ThenCreatingShouldThrow()
    {
        Action action = () => Currency.FromCode("AAA");

        action.Should().Throw<InvalidCurrencyException>();
    }

    [Fact]
    public void WhenIsoCodeIsNull_ThenCreatingShouldThrow()
    {
        Action action = () => Currency.FromCode(null);

        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void WhenCreatingWithIsoCodeFromCurrencyInfo_ThenCreatingShouldSucceed()
    {
        // Arrange / Act
        Currency currency = CurrencyInfo.FromCode("EUR");

        // Assert
        currency.Should().NotBeNull();
        currency.Code.Should().Be("EUR");
        currency.Symbol.Should().Be("€");
        currency.IsIso4217.Should().BeTrue();
    }

    [Fact]
    public void WhenWithNonIsoCodeFromCurrencyInfo_NonIsoCurrencyIsCreated()
    {
        // Arrange / Act
        CurrencyInfo currency1 = CurrencyInfo.FromCode("BTC");
        Currency currency2 = currency1;
        Currency currency = CurrencyInfo.FromCode("BTC");

        // Assert
        currency.Should().NotBeNull();
        currency.Code.Should().Be("BTC");
        currency.Symbol.Should().Be("₿");
        currency.IsIso4217.Should().BeFalse();
    }
}
