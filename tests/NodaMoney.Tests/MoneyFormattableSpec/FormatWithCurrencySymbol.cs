using System.Threading;
using FluentAssertions;
using NodaMoney.Tests.Helpers;
using Xunit;

namespace NodaMoney.Tests.MoneyFormattableSpec;

[Collection(nameof(NoParallelization))]
public class FormatWithCurrencySymbol
{
    private readonly Money _yen = new Money(-98_765.4321m, CurrencyInfo.FromCode("JPY"));
    private readonly Money _euro = new Money(-98_765.4321m, CurrencyInfo.FromCode("EUR"));
    private readonly Money _dollar = new Money(-98_765.4321m, CurrencyInfo.FromCode("USD"));
    private readonly Money _dinar = new Money(-98_765.4321m, CurrencyInfo.FromCode("BHD"));
    private readonly Money _swissFranc = new Money(-98_765.4321m, CurrencyInfo.FromCode("CHF"));

    [Fact]
    [UseCulture(null)]
    public void WhenCurrentCultureInvariant_ThenDecimalsFollowsCurrencyAndAmountFollowsCurrentCulture()
    {
        Thread.CurrentThread.CurrentCulture.Name.Should().Be(""); // Invariant Culture
        _yen.ToString("C").Should().Be("(¥98,765)");
        _euro.ToString("C").Should().Be("(€98,765.43)");
        _dollar.ToString("C").Should().Be("($98,765.43)");
        _dinar.ToString("C").Should().Be("(BD98,765.432)");
        _swissFranc.ToString("C").Should().Be("(CHF98,765.43)");
    }

    [Fact]
    [UseCulture("en-US")]
    public void WhenCurrentCultureUS_ThenDecimalsFollowsCurrencyAndAmountFollowsCurrentCultureNL()
    {
        Thread.CurrentThread.CurrentCulture.Name.Should().Be("en-US");
        _yen.ToString("C").Should().Be("-¥98,765");
        _euro.ToString("C").Should().Be("-€98,765.43");
        _dollar.ToString("C").Should().Be("-$98,765.43");
        _dinar.ToString("C").Should().Be("-BD98,765.432");
        _swissFranc.ToString("C").Should().Be("-CHF98,765.43");
    }

    [Fact]
    [UseCulture("nl-NL")]
    public void WhenCurrentCultureNL_ThenDecimalsFollowsCurrencyAndAmountFollowsCurrentCultureNL()
    {
        Thread.CurrentThread.CurrentCulture.Name.Should().Be("nl-NL");
        _yen.ToString("C").Should().Be("¥ -98.765");
        _euro.ToString("C").Should().Be("€ -98.765,43");
        _dollar.ToString("C").Should().Be("$ -98.765,43");
        _dinar.ToString("C").Should().Be("BD -98.765,432");
        _swissFranc.ToString("C").Should().Be("CHF -98.765,43");
    }

    [Fact]
    [UseCulture("fr-FR")]
    public void WhenCurrentCultureFR_ThenDecimalsFollowsCurrencyAndAmountFollowsCurrentCultureFR()
    {
        Thread.CurrentThread.CurrentCulture.Name.Should().Be("fr-FR");
        _yen.ToString("C").Should().Be("-98 765 ¥");
        _euro.ToString("C").Should().Be("-98 765,43 €");
        _dollar.ToString("C").Should().Be("-98 765,43 $");
        _dinar.ToString("C").Should().Be("-98 765,432 BD");
        _swissFranc.ToString("C").Should().Be("-98 765,43 CHF");
    }

    [Fact]
    [UseCulture("en-US")]
    public void WhenZeroDecimals_ThenThisShouldSucceed()
    {
        Thread.CurrentThread.CurrentCulture.Name.Should().Be("en-US");
        _yen.ToString("C0").Should().Be("-¥98,765");
        _euro.ToString("C0").Should().Be("-€98,765");
        _dollar.ToString("C0").Should().Be("-$98,765");
        _dinar.ToString("C0").Should().Be("-BD98,765");
        _swissFranc.ToString("C0").Should().Be("-CHF98,765");
    }

    [Fact]
    [UseCulture("en-US")]
    public void WhenOneDecimals_ThenThisShouldSucceed()
    {
        Thread.CurrentThread.CurrentCulture.Name.Should().Be("en-US");
        _yen.ToString("C1").Should().Be("-¥98,765.0");
        _euro.ToString("C1").Should().Be("-€98,765.4");
        _dollar.ToString("C1").Should().Be("-$98,765.4");
        _dinar.ToString("C1").Should().Be("-BD98,765.4");
        _swissFranc.ToString("C1").Should().Be("-CHF98,765.4");
    }

    [Fact]
    [UseCulture("en-US")]
    public void WhenTwoDecimals_ThenThisShouldSucceed()
    {
        Thread.CurrentThread.CurrentCulture.Name.Should().Be("en-US");
        _yen.ToString("C2").Should().Be("-¥98,765.00");
        _euro.ToString("C2").Should().Be("-€98,765.43");
        _dollar.ToString("C2").Should().Be("-$98,765.43");
        _dinar.ToString("C2").Should().Be("-BD98,765.43");
        _swissFranc.ToString("C2").Should().Be("-CHF98,765.43");
    }

    [Fact]
    [UseCulture("en-US")]
    public void WhenThreeDecimals_ThenThisShouldSucceed()
    {
        Thread.CurrentThread.CurrentCulture.Name.Should().Be("en-US");
        _yen.ToString("C3").Should().Be("-¥98,765.000");
        _euro.ToString("C3").Should().Be("-€98,765.430");
        _dollar.ToString("C3").Should().Be("-$98,765.430");
        _dinar.ToString("C3").Should().Be("-BD98,765.432");
        _swissFranc.ToString("C3").Should().Be("-CHF98,765.430");
    }

    [Fact]
    [UseCulture("en-US")]
    public void WhenFourDecimals_ThenThisShouldSucceed()
    {
        Thread.CurrentThread.CurrentCulture.Name.Should().Be("en-US");
        _yen.ToString("C4").Should().Be("-¥98,765.0000");
        _euro.ToString("C4").Should().Be("-€98,765.4300");
        _dollar.ToString("C4").Should().Be("-$98,765.4300");
        _dinar.ToString("C4").Should().Be("-BD98,765.4320");
        _swissFranc.ToString("C4").Should().Be("-CHF98,765.4300");
    }
}
