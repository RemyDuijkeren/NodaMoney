using System;
using System.Globalization;
using FluentAssertions;
using Xunit;

namespace NodaMoney.Tests.CurrencyInfoSpec;

public class CurrencyFromRegionOrCulture
{
    [Fact]
    public void WhenUsingRegionInfo_ThenCreatingShouldSucceed()
    {
        var currency = CurrencyInfo.FromRegion(new RegionInfo("NL"));

        currency.Should().NotBeNull();
        currency.Symbol.Should().Be("€");
        currency.Code.Should().Be("EUR");
        currency.EnglishName.Should().Be("Euro");
    }

    [Fact]
    public void WhenRegionInfoIsNull_ThenCreatingShouldThrow()
    {
        Action action = () => CurrencyInfo.FromRegion((RegionInfo)null);

        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void WhenUsingRegionName_ThenCreatingShouldSucceed()
    {
        var currency = CurrencyInfo.FromRegion("NL");

        currency.Should().NotBeNull();
        currency.Symbol.Should().Be("€");
        currency.Code.Should().Be("EUR");
        currency.EnglishName.Should().Be("Euro");
    }

    [Fact]
    public void WhenUsingRegionNameThatIsNull_ThenCreatingShouldThrow()
    {
        Action action = () => CurrencyInfo.FromRegion((string)null);

        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void WhenUsingCultureInfo_ThenCreatingShouldSucceed()
    {
        var currency = CurrencyInfo.FromCulture(CultureInfo.CreateSpecificCulture("nl-NL"));

        currency.Should().NotBeNull();
        currency.Symbol.Should().Be("€");
        currency.Code.Should().Be("EUR");
        currency.EnglishName.Should().Be("Euro");
    }

    [Fact]
    public void WhenCultureInfoIsNull_ThenCreatingShouldThrow()
    {
        Action action = () => CurrencyInfo.FromCulture(null);

        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void WhenCultureInfoIsNeutralCulture_ThenCreatingShouldThrow()
    {
        Action action = () => CurrencyInfo.FromCulture(new CultureInfo("en"));

        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void WhenUsingCultureName_ThenCreatingShouldSucceed()
    {
        var currency = CurrencyInfo.FromRegion("nl-NL");

        currency.Should().NotBeNull();
        currency.Symbol.Should().Be("€");
        currency.Code.Should().Be("EUR");
        currency.EnglishName.Should().Be("Euro");
    }
}
