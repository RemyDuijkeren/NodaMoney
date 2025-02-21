using System.Globalization;

namespace NodaMoney.Tests.CurrencyInfoSpec;

public class CurrencyFromRegionOrCulture
{
    [Fact]
    public void WhenUsingRegionInfo_ThenCreatingShouldSucceed()
    {
        var currency = CurrencyInfo.GetInstance(new RegionInfo("NL"));

        currency.Should().NotBeNull();
        currency.Symbol.Should().Be("€");
        currency.Code.Should().Be("EUR");
        currency.EnglishName.Should().Be("Euro");
    }

    [Fact]
    public void WhenRegionInfoIsNull_ThenCreatingShouldThrow()
    {
        Action action = () => CurrencyInfo.GetInstance((RegionInfo)null);

        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void WhenUsingCultureInfo_ThenCreatingShouldSucceed()
    {
        var currency = CurrencyInfo.GetInstance(CultureInfo.CreateSpecificCulture("nl-NL"));

        currency.Should().NotBeNull();
        currency.Symbol.Should().Be("€");
        currency.Code.Should().Be("EUR");
        currency.EnglishName.Should().Be("Euro");
    }

    [Fact]
    public void WhenUsingNumberFormatInfo_ThenCreatingShouldSucceed()
    {
        NumberFormatInfo nfi = CultureInfo.CreateSpecificCulture("nl-NL").NumberFormat;
        var currency = CurrencyInfo.GetInstance(nfi);

        currency.Should().NotBeNull();
        currency.Symbol.Should().Be("€");
        currency.Code.Should().Be("EUR");
        currency.EnglishName.Should().Be("Euro");
    }

    [Fact]
    public void WhenCultureInfoIsNull_ThenCreatingShouldThrow()
    {
        Action action = () => CurrencyInfo.GetInstance((IFormatProvider)null);

        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void WhenCultureInfoIsNeutralCulture_ThenCreatingShouldThrow()
    {
        Action action = () => CurrencyInfo.GetInstance(new CultureInfo("en"));

        action.Should().Throw<ArgumentException>();
    }
}
