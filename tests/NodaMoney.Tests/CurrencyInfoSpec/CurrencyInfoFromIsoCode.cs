namespace NodaMoney.Tests.CurrencyInfoSpec;

public class CurrencyInfoFromIsoCode
{
    [Fact]
    public void WhenIsoCodeIsExisting_ThenCreatingShouldSucceed()
    {
        var currency = CurrencyInfo.FromCode("EUR");

        currency.Should().NotBeNull();
        currency.Symbol.Should().Be("€");
        currency.Code.Should().Be("EUR");
        currency.EnglishName.Should().Be("Euro");
        currency.IsHistoric.Should().BeFalse();
    }

    [Fact]
    public void WhenIsoCodeIsUnknown_ThenCreatingShouldThrow()
    {
        Action action = () => CurrencyInfo.FromCode("AAA");

        action.Should().Throw<InvalidCurrencyException>();
    }

    [Fact]
    public void WhenIsoCodeIsNull_ThenCreatingShouldThrow()
    {
        Action action = () => CurrencyInfo.FromCode(null);

        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void WhenEstionianKrone_ThenItShouldBeObsolete()
    {
        var currency = CurrencyInfo.FromCode("EEK");

        currency.Should().NotBeNull();
        currency.Symbol.Should().Be("kr");
        currency.IsHistoric.Should().BeTrue();
    }
}
