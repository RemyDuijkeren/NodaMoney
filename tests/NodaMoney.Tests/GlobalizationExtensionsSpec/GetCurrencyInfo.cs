using System.Globalization;

namespace NodaMoney.Tests.GlobalizationExtensionsSpec;

public class GetCurrencyInfo
{
    [Fact]
    public void WhenGetCurrencyInfoOnRegionInfo_ShouldReturnCurrencyInfo()
    {
        var region = new RegionInfo("US");
        var currency = region.GetCurrencyInfo();

        currency.Code.Should().Be("USD");
    }

    [Fact]
    public void WhenGetCurrencyInfoOnCultureInfo_ShouldReturnCurrencyInfo()
    {
        var culture = new CultureInfo("en-US");
        var currency = culture.GetCurrencyInfo();

        currency.Code.Should().Be("USD");
    }

    [Fact]
    public void WhenGetCurrencyInfoOnInvariantCulture_ShouldThrow()
    {
        var culture = CultureInfo.InvariantCulture;
        Action action = () => culture.GetCurrencyInfo();

        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void WhenGetCurrencyInfoOnNeutralCulture_ShouldThrow()
    {
        var culture = new CultureInfo("en");
        Action action = () => culture.GetCurrencyInfo();

        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void WhenGetCurrencyInfoOnNullRegion_ShouldThrow()
    {
        RegionInfo region = null;
        Action action = () => region.GetCurrencyInfo();

        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void WhenGetCurrencyInfoOnNullCulture_ShouldThrow()
    {
        CultureInfo culture = null;
        Action action = () => culture.GetCurrencyInfo();

        action.Should().Throw<ArgumentNullException>();
    }
}
