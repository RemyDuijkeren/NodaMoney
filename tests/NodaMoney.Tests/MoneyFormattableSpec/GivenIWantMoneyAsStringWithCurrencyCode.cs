using System.Threading;
using FluentAssertions;
using NodaMoney.Tests.Helpers;
using Xunit;

namespace NodaMoney.Tests.MoneyFormattableSpec;

[Collection(nameof(NoParallelization))]
public class GivenIWantMoneyAsStringWithCurrencyCode
{
    private Money _yen = new Money(765.4321m, Currency.FromCode("JPY"));
    private Money _euro = new Money(765.4321m, Currency.FromCode("EUR"));
    private Money _dollar = new Money(765.4321m, Currency.FromCode("USD"));
    private Money _dinar = new Money(765.4321m, Currency.FromCode("BHD"));

    [Fact]
    [UseCulture("en-US")]
    public void WhenCurrentCultureUS_ThenDecimalsFollowsCurrencyAndAmountFollowsCurrentCultureNL()
    {
        Thread.CurrentThread.CurrentCulture.Name.Should().Be("en-US");
        _yen.ToString("I").Should().Be("JPY 765");
        _euro.ToString("I").Should().Be("EUR 765.43");
        _dollar.ToString("I").Should().Be("USD 765.43");
        _dinar.ToString("I").Should().Be("BHD 765.432");
    }

    [Fact]
    [UseCulture("nl-NL")]
    public void WhenCurrentCultureNL_ThenDecimalsFollowsCurrencyAndAmountFollowsCurrentCultureNL()
    {
        Thread.CurrentThread.CurrentCulture.Name.Should().Be("nl-NL");
        _yen.ToString("I").Should().Be("JPY 765");
        _euro.ToString("I").Should().Be("EUR 765,43");
        _dollar.ToString("I").Should().Be("USD 765,43");
        _dinar.ToString("I").Should().Be("BHD 765,432");
    }

    [Fact]
    [UseCulture("fr-FR")]
    public void WhenCurrentCultureFR_ThenDecimalsFollowsCurrencyAndAmountFollowsCurrentCultureFR()
    {
        Thread.CurrentThread.CurrentCulture.Name.Should().Be("fr-FR");
        _yen.ToString("I").Should().Be("765 JPY");
        _euro.ToString("I").Should().Be("765,43 EUR");
        _dollar.ToString("I").Should().Be("765,43 USD");
        _dinar.ToString("I").Should().Be("765,432 BHD");
    }

    [Fact]
    [UseCulture("en-US")]
    public void WhenZeroDecimals_ThenThisShouldSucceed()
    {
        Thread.CurrentThread.CurrentCulture.Name.Should().Be("en-US");
        _yen.ToString("I0").Should().Be("JPY 765");
        _euro.ToString("I0").Should().Be("EUR 765");
        _dollar.ToString("I0").Should().Be("USD 765");
        _dinar.ToString("I0").Should().Be("BHD 765");
    }

    [Fact]
    [UseCulture("en-US")]
    public void WhenOneDecimals_ThenThisShouldSucceed()
    {
        Thread.CurrentThread.CurrentCulture.Name.Should().Be("en-US");
        _yen.ToString("I1").Should().Be("JPY 765.0");
        _euro.ToString("I1").Should().Be("EUR 765.4");
        _dollar.ToString("I1").Should().Be("USD 765.4");
        _dinar.ToString("I1").Should().Be("BHD 765.4");
    }

    [Fact]
    [UseCulture("en-US")]
    public void WhenTwoDecimals_ThenThisShouldSucceed()
    {
        Thread.CurrentThread.CurrentCulture.Name.Should().Be("en-US");
        _yen.ToString("I2").Should().Be("JPY 765.00");
        _euro.ToString("I2").Should().Be("EUR 765.43");
        _dollar.ToString("I2").Should().Be("USD 765.43");
        _dinar.ToString("I2").Should().Be("BHD 765.43");
    }

    [Fact]
    [UseCulture("en-US")]
    public void WhenThreeDecimals_ThenThisShouldSucceed()
    {
        Thread.CurrentThread.CurrentCulture.Name.Should().Be("en-US");
        _yen.ToString("I3").Should().Be("JPY 765.000");
        _euro.ToString("I3").Should().Be("EUR 765.430");
        _dollar.ToString("I3").Should().Be("USD 765.430");
        _dinar.ToString("I3").Should().Be("BHD 765.432");
    }

    [Fact]
    [UseCulture("en-US")]
    public void WhenFourDecimals_ThenThisShouldSucceed()
    {
        Thread.CurrentThread.CurrentCulture.Name.Should().Be("en-US");
        _yen.ToString("I4").Should().Be("JPY 765.0000");
        _euro.ToString("I4").Should().Be("EUR 765.4300");
        _dollar.ToString("I4").Should().Be("USD 765.4300");
        _dinar.ToString("I4").Should().Be("BHD 765.4320");
    }
}
