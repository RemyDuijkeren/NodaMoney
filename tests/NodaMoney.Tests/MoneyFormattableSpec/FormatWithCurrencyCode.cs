using NodaMoney.Context;
using NodaMoney.Tests.Helpers;

namespace NodaMoney.Tests.MoneyFormattableSpec;

[Collection(nameof(NoParallelization))]
public class FormatWithCurrencyCode
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
        _yen.ToString("G").Should().Be("(JPY 98,765)");
        _euro.ToString("G").Should().Be("(EUR 98,765.43)");
        _dollar.ToString("G").Should().Be("(USD 98,765.43)");
        _dinar.ToString("G").Should().Be("(BHD 98,765.432)");
        _swissFranc.ToString("G").Should().Be("(CHF 98,765.43)");
    }

    [Fact]
    [UseCulture("en-US")]
    public void WhenCurrentCultureUS_ThenDecimalsFollowsCurrencyAndAmountFollowsCurrentCultureNL()
    {
        // The US default negative pattern for .NET4.8 is Pattern 0 `($n)`, where >.NET6.0 uses Pattern 1 `-$n`

        // The currency code is always followed by a space, like '-USD 98,765.43', which is different
        // when you use a symbol, like '-$98,765.4'
        Thread.CurrentThread.CurrentCulture.Name.Should().Be("en-US");
        _yen.ToString("G").Should().BeOneOf("-JPY 98,765", "(JPY 98,765)");
        _euro.ToString("G").Should().BeOneOf("-EUR 98,765.43", "(EUR 98,765.43)");
        _dollar.ToString("G").Should().BeOneOf("-USD 98,765.43", "(USD 98,765.43)");
        _dinar.ToString("G").Should().BeOneOf("-BHD 98,765.432", "(BHD 98,765.432)");
        _swissFranc.ToString("G").Should().BeOneOf("-CHF 98,765.43", "(CHF 98,765.43)");
    }

    [Fact]
    [UseCulture("nl-NL")]
    public void WhenCurrentCultureNL_ThenDecimalsFollowsCurrencyAndAmountFollowsCurrentCultureNL()
    {
        // The currency code is always followed by a space, like '-USD 98,765.43', which is different
        // when you use a symbol, like '-$98,765.4'
        Thread.CurrentThread.CurrentCulture.Name.Should().Be("nl-NL");
        _yen.ToString("G").Should().Be("JPY -98.765", "currency code is always followed by a space");
        _euro.ToString("G").Should().Be("EUR -98.765,43", "currency code is always followed by a space");
        _dollar.ToString("G").Should().Be("USD -98.765,43", "currency code is always followed by a space");
        _dinar.ToString("G").Should().Be("BHD -98.765,432", "currency code is always followed by a space");
        _swissFranc.ToString("G").Should().Be("CHF -98.765,43", "currency code is always followed by a space");
    }

    [Fact]
    [UseCulture("fr-FR")]
    public void WhenCurrentCultureFR_ThenDecimalsFollowsCurrencyAndAmountFollowsCurrentCultureFR()
    {
        Thread.CurrentThread.CurrentCulture.Name.Should().Be("fr-FR");
        _yen.ToString("G").Should().Be("-98 765 JPY");
        _euro.ToString("G").Should().Be("-98 765,43 EUR");
        _dollar.ToString("G").Should().Be("-98 765,43 USD");
        _dinar.ToString("G").Should().Be("-98 765,432 BHD");
        _swissFranc.ToString("G").Should().Be("-98 765,43 CHF");
    }

    [Fact]
    [UseCulture("nl-NL")]
    public void WhenZeroDecimals_ThenThisShouldSucceed()
    {
        Thread.CurrentThread.CurrentCulture.Name.Should().Be("nl-NL");
        MoneyContext.CurrentContext.Should().Be(MoneyContext.Create(MoneyContextOptions.Default));
        _yen.ToString("G0").Should().Be("JPY -98.765");
        _euro.ToString("G0").Should().Be("EUR -98.765");
        _dollar.ToString("G0").Should().Be("USD -98.765");
        _dinar.ToString("G0").Should().Be("BHD -98.765");
        _swissFranc.ToString("G0").Should().Be("CHF -98.765");
    }

    [Fact]
    [UseCulture("nl-NL")]
    public void WhenOneDecimals_ThenThisShouldSucceed()
    {
        Thread.CurrentThread.CurrentCulture.Name.Should().Be("nl-NL");
        MoneyContext.CurrentContext.Should().Be(MoneyContext.Create(MoneyContextOptions.Default));
        _yen.ToString("G1").Should().Be("JPY -98.765,0");
        _euro.ToString("G1").Should().Be("EUR -98.765,4");
        _dollar.ToString("G1").Should().Be("USD -98.765,4");
        _dinar.ToString("G1").Should().Be("BHD -98.765,4");
        _swissFranc.ToString("G1").Should().Be("CHF -98.765,4");
    }

    [Fact]
    [UseCulture("nl-NL")]
    public void WhenTwoDecimals_ThenThisShouldSucceed()
    {
        Thread.CurrentThread.CurrentCulture.Name.Should().Be("nl-NL");
        MoneyContext.CurrentContext.Should().Be(MoneyContext.Create(MoneyContextOptions.Default));
        _yen.ToString("G2").Should().Be("JPY -98.765,00");
        _euro.ToString("G2").Should().Be("EUR -98.765,43");
        _dollar.ToString("G2").Should().Be("USD -98.765,43");
        _dinar.ToString("G2").Should().Be("BHD -98.765,43");
        _swissFranc.ToString("G2").Should().Be("CHF -98.765,43");
    }

    [Fact]
    [UseCulture("nl-NL")]
    public void WhenThreeDecimals_ThenThisShouldSucceed()
    {
        Thread.CurrentThread.CurrentCulture.Name.Should().Be("nl-NL");
        MoneyContext.CurrentContext.Should().Be(MoneyContext.Create(MoneyContextOptions.Default));
        _yen.ToString("G3").Should().Be("JPY -98.765,000");
        _euro.ToString("G3").Should().Be("EUR -98.765,430");
        _dollar.ToString("G3").Should().Be("USD -98.765,430");
        _dinar.ToString("G3").Should().Be("BHD -98.765,432");
        _swissFranc.ToString("G3").Should().Be("CHF -98.765,430");
    }

    [Fact]
    [UseCulture("nl-NL")]
    public void WhenFourDecimals_ThenThisShouldSucceed()
    {
        Thread.CurrentThread.CurrentCulture.Name.Should().Be("nl-NL");
        MoneyContext.CurrentContext.Should().Be(MoneyContext.Create(MoneyContextOptions.Default));
        _yen.ToString("G4").Should().Be("JPY -98.765,0000");
        _euro.ToString("G4").Should().Be("EUR -98.765,4300");
        _dollar.ToString("G4").Should().Be("USD -98.765,4300");
        _dinar.ToString("G4").Should().Be("BHD -98.765,4320");
        _swissFranc.ToString("G4").Should().Be("CHF -98.765,4300");
    }
}
