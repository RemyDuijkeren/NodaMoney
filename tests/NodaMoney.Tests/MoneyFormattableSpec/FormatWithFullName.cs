using System.Threading;
using FluentAssertions;
using Xunit;
using NodaMoney.Tests.Helpers;

namespace NodaMoney.Tests.MoneyFormattableSpec;

[Collection(nameof(NoParallelization))]
public class FormatWithFullName
{
    private readonly Money _yen = new Money(2765.4321m, CurrencyInfo.FromCode("JPY"));
    private readonly Money _euro = new Money(2765.4321m, CurrencyInfo.FromCode("EUR"));
    private readonly Money _dollar = new Money(2765.4321m, CurrencyInfo.FromCode("USD"));
    private readonly Money _dinar = new Money(2765.4321m, CurrencyInfo.FromCode("BHD"));

    [Fact]
    [UseCulture("pt-BR")]
    public void WhenCurrentCulturePTBR_ThenDecimalsFollowsCurrencyAndAmountFollowsCurrentCulturePtBRAndCurrencyNameIsInEnglish()
    {
            Thread.CurrentThread.CurrentCulture.Name.Should().Be("pt-BR");
            _yen.ToString("L").Should().Be("2.765 Japanese yen");
            _euro.ToString("L").Should().Be("2.765,43 Euro");
            _dollar.ToString("L").Should().Be("2.765,43 United States dollar");
            _dinar.ToString("L").Should().Be("2.765,432 Bahraini dinar");
        }

    [Fact]
    [UseCulture("en-US")]
    public void WhenCurrentCultureEnUS_ThenDecimalsFollowsCurrencyAndAmountFollowsCurrentCultureEnUSAndCurrencyNameIsInEnglish()
    {
            Thread.CurrentThread.CurrentCulture.Name.Should().Be("en-US");
            _yen.ToString("L").Should().Be("2,765 Japanese yen");
            _euro.ToString("L").Should().Be("2,765.43 Euro");
            _dollar.ToString("L").Should().Be("2,765.43 United States dollar");
            _dinar.ToString("L").Should().Be("2,765.432 Bahraini dinar");
        }

    [Fact]
    [UseCulture("en-US")]
    public void WhenZeroDecimals_ThenThisShouldSucceed()
    {
            Thread.CurrentThread.CurrentCulture.Name.Should().Be("en-US");
            _yen.ToString("L0").Should().Be("2,765 Japanese yen");
            _euro.ToString("L0").Should().Be("2,765 Euro");
            _dollar.ToString("L0").Should().Be("2,765 United States dollar");
            _dinar.ToString("L0").Should().Be("2,765 Bahraini dinar");
        }

    [Fact]
    [UseCulture("en-US")]
    public void WhenOneDecimals_ThenThisShouldSucceed()
    {
            Thread.CurrentThread.CurrentCulture.Name.Should().Be("en-US");
            _yen.ToString("L1").Should().Be("2,765.0 Japanese yen");
            _euro.ToString("L1").Should().Be("2,765.4 Euro");
            _dollar.ToString("L1").Should().Be("2,765.4 United States dollar");
            _dinar.ToString("L1").Should().Be("2,765.4 Bahraini dinar");
        }

    [Fact]
    [UseCulture("en-US")]
    public void WhenTwoDecimals_ThenThisShouldSucceed()
    {
            Thread.CurrentThread.CurrentCulture.Name.Should().Be("en-US");
            _yen.ToString("L2").Should().Be("2,765.00 Japanese yen");
            _euro.ToString("L2").Should().Be("2,765.43 Euro");
            _dollar.ToString("L2").Should().Be("2,765.43 United States dollar");
            _dinar.ToString("L2").Should().Be("2,765.43 Bahraini dinar");
        }

    [Fact]
    [UseCulture("en-US")]
    public void WhenThreeDecimals_ThenThisShouldSucceed()
    {
            Thread.CurrentThread.CurrentCulture.Name.Should().Be("en-US");
            _yen.ToString("L3").Should().Be("2,765.000 Japanese yen");
            _euro.ToString("L3").Should().Be("2,765.430 Euro");
            _dollar.ToString("L3").Should().Be("2,765.430 United States dollar");
            _dinar.ToString("L3").Should().Be("2,765.432 Bahraini dinar");
        }

    [Fact]
    [UseCulture("en-US")]
    public void WhenFourDecimals_ThenThisShouldSucceed()
    {
            Thread.CurrentThread.CurrentCulture.Name.Should().Be("en-US");
            _yen.ToString("L4").Should().Be("2,765.0000 Japanese yen");
            _euro.ToString("L4").Should().Be("2,765.4300 Euro");
            _dollar.ToString("L4").Should().Be("2,765.4300 United States dollar");
            _dinar.ToString("L4").Should().Be("2,765.4320 Bahraini dinar");
        }
}
