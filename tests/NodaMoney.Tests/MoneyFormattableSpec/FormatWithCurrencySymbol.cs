using System.Threading;
using NodaMoney.Tests.Helpers;

namespace NodaMoney.Tests.MoneyFormattableSpec;

[Collection(nameof(NoParallelization))]
public class FormatWithCurrencySymbol
{
    private readonly Money _yen = new(-98_765.4321m, CurrencyInfo.FromCode("JPY"));
    private readonly Money _euro = new(-98_765.4321m, CurrencyInfo.FromCode("EUR"));
    private readonly Money _dollar = new(-98_765.4321m, CurrencyInfo.FromCode("USD"));
    private readonly Money _dinar = new(-98_765.4321m, CurrencyInfo.FromCode("BHD"));
    private readonly Money _swissFranc = new(-98_765.4321m, CurrencyInfo.FromCode("CHF"));

    [Fact]
    [UseCulture(null)]
    public void WhenCurrentCultureInvariant_ThenDecimalsFollowsCurrencyAndAmountFollowsCurrentCulture()
    {
        Thread.CurrentThread.CurrentCulture.Name.Should().Be(""); // Invariant Culture
        _yen.ToString("c").Should().Be("(¥98,765)");
        _euro.ToString("c").Should().Be("(€98,765.43)");
        _dollar.ToString("c").Should().Be("($98,765.43)");
        _dinar.ToString("c").Should().Be("(BD98,765.432)");
        _swissFranc.ToString("c").Should().Be("(Fr.98,765.43)");
    }

    [Fact]
    [UseCulture("en-US")]
    public void WhenCurrentCultureUS_ThenDecimalsFollowsCurrencyAndAmountFollowsCurrentCultureNL()
    {
        // The US default negative pattern for .NET4.8 is Pattern 0 `($n)`, where >.NET6.0 uses Pattern 1 `-$n`

        Thread.CurrentThread.CurrentCulture.Name.Should().Be("en-US");
        _yen.ToString("c").Should().BeOneOf("-¥98,765", "(¥98,765)");
        _euro.ToString("c").Should().BeOneOf("-€98,765.43", "(€98,765.43)");
        _dollar.ToString("c").Should().BeOneOf("-$98,765.43", "($98,765.43)");
        _dinar.ToString("c").Should().BeOneOf("-BD98,765.432", "(BD98,765.432)");
        _swissFranc.ToString("c").Should().BeOneOf("-Fr.98,765.43", "(Fr.98,765.43)");
    }

    [Fact]
    [UseCulture("nl-NL")]
    public void WhenCurrentCultureNL_ThenDecimalsFollowsCurrencyAndAmountFollowsCurrentCultureNL()
    {
        Thread.CurrentThread.CurrentCulture.Name.Should().Be("nl-NL");
        _yen.ToString("c").Should().Be("¥ -98.765");
        _euro.ToString("c").Should().Be("€ -98.765,43");
        _dollar.ToString("c").Should().Be("$ -98.765,43");
        _dinar.ToString("c").Should().Be("BD -98.765,432");
        _swissFranc.ToString("c").Should().Be("Fr. -98.765,43");
    }

    [Fact]
    [UseCulture("fr-FR")]
    public void WhenCurrentCultureFR_ThenDecimalsFollowsCurrencyAndAmountFollowsCurrentCultureFR()
    {
        Thread.CurrentThread.CurrentCulture.Name.Should().Be("fr-FR");
        _yen.ToString("c").Should().Be("-98 765 ¥");
        _euro.ToString("c").Should().Be("-98 765,43 €");
        _dollar.ToString("c").Should().Be("-98 765,43 $");
        _dinar.ToString("c").Should().Be("-98 765,432 BD");
        _swissFranc.ToString("c").Should().Be("-98 765,43 Fr.");
    }

    [Fact]
    [UseCulture("nl-NL")]
    public void WhenZeroDecimals_ThenThisShouldSucceed()
    {
        Thread.CurrentThread.CurrentCulture.Name.Should().Be("nl-NL");
        _yen.ToString("c0").Should().Be("¥ -98.765");
        _euro.ToString("c0").Should().Be("€ -98.765");
        _dollar.ToString("c0").Should().Be("$ -98.765");
        _dinar.ToString("c0").Should().Be("BD -98.765");
        _swissFranc.ToString("c0").Should().Be("Fr. -98.765");
    }

    [Fact]
    [UseCulture("nl-NL")]
    public void WhenOneDecimals_ThenThisShouldSucceed()
    {
        Thread.CurrentThread.CurrentCulture.Name.Should().Be("nl-NL");
        _yen.ToString("c1").Should().Be("¥ -98.765,0");
        _euro.ToString("c1").Should().Be("€ -98.765,4");
        _dollar.ToString("c1").Should().Be("$ -98.765,4");
        _dinar.ToString("c1").Should().Be("BD -98.765,4");
        _swissFranc.ToString("c1").Should().Be("Fr. -98.765,4");
    }

    [Fact]
    [UseCulture("nl-NL")]
    public void WhenTwoDecimals_ThenThisShouldSucceed()
    {
        Thread.CurrentThread.CurrentCulture.Name.Should().Be("nl-NL");
        _yen.ToString("c2").Should().Be("¥ -98.765,00");
        _euro.ToString("c2").Should().Be("€ -98.765,43");
        _dollar.ToString("c2").Should().Be("$ -98.765,43");
        _dinar.ToString("c2").Should().Be("BD -98.765,43");
        _swissFranc.ToString("c2").Should().Be("Fr. -98.765,43");
    }

    [Fact]
    [UseCulture("nl-NL")]
    public void WhenThreeDecimals_ThenThisShouldSucceed()
    {
        Thread.CurrentThread.CurrentCulture.Name.Should().Be("nl-NL");
        _yen.ToString("c3").Should().Be("¥ -98.765,000");
        _euro.ToString("c3").Should().Be("€ -98.765,430");
        _dollar.ToString("c3").Should().Be("$ -98.765,430");
        _dinar.ToString("c3").Should().Be("BD -98.765,432");
        _swissFranc.ToString("c3").Should().Be("Fr. -98.765,430");
    }

    [Fact]
    [UseCulture("nl-NL")]
    public void WhenFourDecimals_ThenThisShouldSucceed()
    {
        Thread.CurrentThread.CurrentCulture.Name.Should().Be("nl-NL");
        _yen.ToString("c4").Should().Be("¥ -98.765,0000");
        _euro.ToString("c4").Should().Be("€ -98.765,4300");
        _dollar.ToString("c4").Should().Be("$ -98.765,4300");
        _dinar.ToString("c4").Should().Be("BD -98.765,4320");
        _swissFranc.ToString("c4").Should().Be("Fr. -98.765,4300");
    }
}
