using System.Threading;
using FluentAssertions;
using NodaMoney.Tests.Helpers;
using Xunit;

namespace NodaMoney.Tests.MoneyFormattableSpec;

[Collection(nameof(NoParallelization))]
public class FormatWithCurrencyCode
{
    private readonly Money _yen = new Money(2765.4321m, CurrencyInfo.FromCode("JPY"));
    private readonly Money _euro = new Money(2765.4321m, CurrencyInfo.FromCode("EUR"));
    private readonly Money _dollar = new Money(2765.4321m, CurrencyInfo.FromCode("USD"));
    private readonly Money _dinar = new Money(2765.4321m, CurrencyInfo.FromCode("BHD"));

    [Fact]
    [UseCulture("en-US")]
    public void WhenCurrentCultureUS_ThenDecimalsFollowsCurrencyAndAmountFollowsCurrentCultureNL()
    {
        Thread.CurrentThread.CurrentCulture.Name.Should().Be("en-US");
        _yen.ToString("G").Should().Be("JPY 2,765");
        _euro.ToString("G").Should().Be("EUR 2,765.43");
        _dollar.ToString("G").Should().Be("USD 2,765.43");
        _dinar.ToString("G").Should().Be("BHD 2,765.432");
    }

    [Fact]
    [UseCulture("nl-NL")]
    public void WhenCurrentCultureNL_ThenDecimalsFollowsCurrencyAndAmountFollowsCurrentCultureNL()
    {
        Thread.CurrentThread.CurrentCulture.Name.Should().Be("nl-NL");
        _yen.ToString("G").Should().Be("JPY 2.765");
        _euro.ToString("G").Should().Be("EUR 2.765,43");
        _dollar.ToString("G").Should().Be("USD 2.765,43");
        _dinar.ToString("G").Should().Be("BHD 2.765,432");
    }

    [Fact]
    [UseCulture("fr-FR")]
    public void WhenCurrentCultureFR_ThenDecimalsFollowsCurrencyAndAmountFollowsCurrentCultureFR()
    {
        Thread.CurrentThread.CurrentCulture.Name.Should().Be("fr-FR");
        _yen.ToString("G").Should().Be("2 765 JPY");
        _euro.ToString("G").Should().Be("2 765,43 EUR");
        _dollar.ToString("G").Should().Be("2 765,43 USD");
        _dinar.ToString("G").Should().Be("2 765,432 BHD");
    }

    [Fact]
    [UseCulture("en-US")]
    public void WhenZeroDecimals_ThenThisShouldSucceed()
    {
        Thread.CurrentThread.CurrentCulture.Name.Should().Be("en-US");
        _yen.ToString("G0").Should().Be("JPY 2,765");
        _euro.ToString("G0").Should().Be("EUR 2,765");
        _dollar.ToString("G0").Should().Be("USD 2,765");
        _dinar.ToString("G0").Should().Be("BHD 2,765");
    }

    [Fact]
    [UseCulture("en-US")]
    public void WhenOneDecimals_ThenThisShouldSucceed()
    {
        Thread.CurrentThread.CurrentCulture.Name.Should().Be("en-US");
        _yen.ToString("G1").Should().Be("JPY 2,765.0");
        _euro.ToString("G1").Should().Be("EUR 2,765.4");
        _dollar.ToString("G1").Should().Be("USD 2,765.4");
        _dinar.ToString("G1").Should().Be("BHD 2,765.4");
    }

    [Fact]
    [UseCulture("en-US")]
    public void WhenTwoDecimals_ThenThisShouldSucceed()
    {
        Thread.CurrentThread.CurrentCulture.Name.Should().Be("en-US");
        _yen.ToString("G2").Should().Be("JPY 2,765.00");
        _euro.ToString("G2").Should().Be("EUR 2,765.43");
        _dollar.ToString("G2").Should().Be("USD 2,765.43");
        _dinar.ToString("G2").Should().Be("BHD 2,765.43");
    }

    [Fact]
    [UseCulture("en-US")]
    public void WhenThreeDecimals_ThenThisShouldSucceed()
    {
        Thread.CurrentThread.CurrentCulture.Name.Should().Be("en-US");
        _yen.ToString("G3").Should().Be("JPY 2,765.000");
        _euro.ToString("G3").Should().Be("EUR 2,765.430");
        _dollar.ToString("G3").Should().Be("USD 2,765.430");
        _dinar.ToString("G3").Should().Be("BHD 2,765.432");
    }

    [Fact]
    [UseCulture("en-US")]
    public void WhenFourDecimals_ThenThisShouldSucceed()
    {
        Thread.CurrentThread.CurrentCulture.Name.Should().Be("en-US");
        _yen.ToString("G4").Should().Be("JPY 2,765.0000");
        _euro.ToString("G4").Should().Be("EUR 2,765.4300");
        _dollar.ToString("G4").Should().Be("USD 2,765.4300");
        _dinar.ToString("G4").Should().Be("BHD 2,765.4320");
    }
}
