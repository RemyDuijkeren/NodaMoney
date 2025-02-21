namespace NodaMoney.Tests.CurrencySpec;

public class CurrencyFromIsoCode
{
    [Fact]
    public void WhenIsoCodeIsExisting_ThenCreatingShouldSucceed()
    {
        Currency currency = Currency.FromCode("EUR");

        currency.Should().NotBeNull();
        currency.Code.Should().Be("EUR");
        // currency.Symbol.Should().Be("€");
        // currency.EnglishName.Should().Be("Euro");
        // currency.IsValid.Should().BeTrue();
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
    public void WhenEstionianKrone_ThenItShouldBeObsolete()
    {
        Currency currency = Currency.FromCode("EEK");

        currency.Should().NotBeNull();
        currency.Code.Should().Be("EEK");
        // currency.Symbol.Should().Be("kr");
        // currency.IsValid.Should().BeFalse();
    }
}
