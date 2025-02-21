using System.Threading;
using NodaMoney.Tests.Helpers;

namespace NodaMoney.Tests.MoneyFormattableSpec;

[Collection(nameof(NoParallelization))]
public class FormatWithFullName
{
    private readonly Money _yen = new Money(-98_765.4321m, CurrencyInfo.FromCode("JPY"));
    private readonly Money _euro = new Money(-98_765.4321m, CurrencyInfo.FromCode("EUR"));
    private readonly Money _dollar = new Money(-98_765.4321m, CurrencyInfo.FromCode("USD"));
    private readonly Money _dinar = new Money(-98_765.4321m, CurrencyInfo.FromCode("BHD"));
    private readonly Money _swissFranc = new Money(-98_765.4321m, CurrencyInfo.FromCode("CHF"));

    [Fact]
    [UseCulture(null)]
    public void WhenCurrentCultureInvariant_ThenDecimalsFollowsCurrencyAndAmountFollowsCurrentCultureAndCurrencyNameIsInEnglish()
    {
        Thread.CurrentThread.CurrentCulture.Name.Should().Be(""); // Invariant Culture
        _yen.ToString("L").Should().Be("-98,765 Japanese yen");
        _euro.ToString("L").Should().Be("-98,765.43 Euro");
        _dollar.ToString("L").Should().Be("-98,765.43 United States dollar");
        _dinar.ToString("L").Should().Be("-98,765.432 Bahraini dinar");
        _swissFranc.ToString("L").Should().Be("-98,765.43 Swiss franc");
    }

    [Fact]
    [UseCulture("pt-BR")]
    public void WhenCurrentCulturePTBR_ThenDecimalsFollowsCurrencyAndAmountFollowsCurrentCulturePtBRAndCurrencyNameIsInEnglish()
    {
        Thread.CurrentThread.CurrentCulture.Name.Should().Be("pt-BR");
        _yen.ToString("L").Should().Be("-98.765 Japanese yen");
        _euro.ToString("L").Should().Be("-98.765,43 Euro");
        _dollar.ToString("L").Should().Be("-98.765,43 United States dollar");
        _dinar.ToString("L").Should().Be("-98.765,432 Bahraini dinar");
        _swissFranc.ToString("L").Should().Be("-98.765,43 Swiss franc");
    }

    [Fact]
    [UseCulture("en-US")]
    public void WhenCurrentCultureEnUS_ThenDecimalsFollowsCurrencyAndAmountFollowsCurrentCultureEnUSAndCurrencyNameIsInEnglish()
    {
        Thread.CurrentThread.CurrentCulture.Name.Should().Be("en-US");
        _yen.ToString("L").Should().Be("-98,765 Japanese yen");
        _euro.ToString("L").Should().Be("-98,765.43 Euro");
        _dollar.ToString("L").Should().Be("-98,765.43 United States dollar");
        _dinar.ToString("L").Should().Be("-98,765.432 Bahraini dinar");
        _swissFranc.ToString("L").Should().Be("-98,765.43 Swiss franc");
    }

    [Fact]
    [UseCulture("en-US")]
    public void WhenZeroDecimals_ThenThisShouldSucceed()
    {
        Thread.CurrentThread.CurrentCulture.Name.Should().Be("en-US");
        _yen.ToString("L0").Should().Be("-98,765 Japanese yen");
        _euro.ToString("L0").Should().Be("-98,765 Euro");
        _dollar.ToString("L0").Should().Be("-98,765 United States dollar");
        _dinar.ToString("L0").Should().Be("-98,765 Bahraini dinar");
        _swissFranc.ToString("L0").Should().Be("-98,765 Swiss franc");
    }

    [Fact]
    [UseCulture("en-US")]
    public void WhenOneDecimals_ThenThisShouldSucceed()
    {
        Thread.CurrentThread.CurrentCulture.Name.Should().Be("en-US");
        _yen.ToString("L1").Should().Be("-98,765.0 Japanese yen");
        _euro.ToString("L1").Should().Be("-98,765.4 Euro");
        _dollar.ToString("L1").Should().Be("-98,765.4 United States dollar");
        _dinar.ToString("L1").Should().Be("-98,765.4 Bahraini dinar");
        _swissFranc.ToString("L1").Should().Be("-98,765.4 Swiss franc");
    }

    [Fact]
    [UseCulture("en-US")]
    public void WhenTwoDecimals_ThenThisShouldSucceed()
    {
        Thread.CurrentThread.CurrentCulture.Name.Should().Be("en-US");
        _yen.ToString("L2").Should().Be("-98,765.00 Japanese yen");
        _euro.ToString("L2").Should().Be("-98,765.43 Euro");
        _dollar.ToString("L2").Should().Be("-98,765.43 United States dollar");
        _dinar.ToString("L2").Should().Be("-98,765.43 Bahraini dinar");
        _swissFranc.ToString("L2").Should().Be("-98,765.43 Swiss franc");
    }

    [Fact]
    [UseCulture("en-US")]
    public void WhenThreeDecimals_ThenThisShouldSucceed()
    {
        Thread.CurrentThread.CurrentCulture.Name.Should().Be("en-US");
        _yen.ToString("L3").Should().Be("-98,765.000 Japanese yen");
        _euro.ToString("L3").Should().Be("-98,765.430 Euro");
        _dollar.ToString("L3").Should().Be("-98,765.430 United States dollar");
        _dinar.ToString("L3").Should().Be("-98,765.432 Bahraini dinar");
        _swissFranc.ToString("L3").Should().Be("-98,765.430 Swiss franc");
    }

    [Fact]
    [UseCulture("en-US")]
    public void WhenFourDecimals_ThenThisShouldSucceed()
    {
        Thread.CurrentThread.CurrentCulture.Name.Should().Be("en-US");
        _yen.ToString("L4").Should().Be("-98,765.0000 Japanese yen");
        _euro.ToString("L4").Should().Be("-98,765.4300 Euro");
        _dollar.ToString("L4").Should().Be("-98,765.4300 United States dollar");
        _dinar.ToString("L4").Should().Be("-98,765.4320 Bahraini dinar");
        _swissFranc.ToString("L4").Should().Be("-98,765.4300 Swiss franc");
    }
}
