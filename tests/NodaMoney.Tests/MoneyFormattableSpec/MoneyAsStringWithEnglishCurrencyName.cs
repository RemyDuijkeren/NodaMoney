using System.Threading;
using FluentAssertions;
using Xunit;
using NodaMoney.Tests.Helpers;

namespace NodaMoney.Tests.MoneyFormattableSpec;

[Collection(nameof(NoParallelization))]
public class MoneyAsStringWithEnglishCurrencyName
{
    private Money _yen = new Money(765.4321m, Currency.FromCode("JPY"));
    private Money _euro = new Money(765.4321m, Currency.FromCode("EUR"));
    private Money _dollar = new Money(765.4321m, Currency.FromCode("USD"));
    private Money _dinar = new Money(765.4321m, Currency.FromCode("BHD"));

    [Fact]
    [UseCulture("pt-BR")]
    public void WhenCurrentCulturePTBR_ThenDecimalsFollowsCurrencyAndAmountFollowsCurrentCulturePtBRAndCurrencyNameIsInEnglish()
    {
            Thread.CurrentThread.CurrentCulture.Name.Should().Be("pt-BR");
            _yen.ToString("F").Should().Be("765 Japanese yen");
            _euro.ToString("F").Should().Be("765,43 Euro");
            _dollar.ToString("F").Should().Be("765,43 United States dollar");
            _dinar.ToString("F").Should().Be("765,432 Bahraini dinar");
        }

    [Fact]
    [UseCulture("en-US")]
    public void WhenCurrentCultureEnUS_ThenDecimalsFollowsCurrencyAndAmountFollowsCurrentCultureEnUSAndCurrencyNameIsInEnglish()
    {
            Thread.CurrentThread.CurrentCulture.Name.Should().Be("en-US");
            _yen.ToString("F").Should().Be("765 Japanese yen");
            _euro.ToString("F").Should().Be("765.43 Euro");
            _dollar.ToString("F").Should().Be("765.43 United States dollar");
            _dinar.ToString("F").Should().Be("765.432 Bahraini dinar");
        }

    [Fact]
    [UseCulture("en-US")]
    public void WhenZeroDecimals_ThenThisShouldSucceed()
    {
            Thread.CurrentThread.CurrentCulture.Name.Should().Be("en-US");
            _yen.ToString("F0").Should().Be("765 Japanese yen");
            _euro.ToString("F0").Should().Be("765 Euro");
            _dollar.ToString("F0").Should().Be("765 United States dollar");
            _dinar.ToString("F0").Should().Be("765 Bahraini dinar");
        }

    [Fact]
    [UseCulture("en-US")]
    public void WhenOneDecimals_ThenThisShouldSucceed()
    {
            Thread.CurrentThread.CurrentCulture.Name.Should().Be("en-US");
            _yen.ToString("F1").Should().Be("765.0 Japanese yen");
            _euro.ToString("F1").Should().Be("765.4 Euro");
            _dollar.ToString("F1").Should().Be("765.4 United States dollar");
            _dinar.ToString("F1").Should().Be("765.4 Bahraini dinar");
        }

    [Fact]
    [UseCulture("en-US")]
    public void WhenTwoDecimals_ThenThisShouldSucceed()
    {
            Thread.CurrentThread.CurrentCulture.Name.Should().Be("en-US");
            _yen.ToString("F2").Should().Be("765.00 Japanese yen");
            _euro.ToString("F2").Should().Be("765.43 Euro");
            _dollar.ToString("F2").Should().Be("765.43 United States dollar");
            _dinar.ToString("F2").Should().Be("765.43 Bahraini dinar");
        }

    [Fact]
    [UseCulture("en-US")]
    public void WhenThreeDecimals_ThenThisShouldSucceed()
    {
            Thread.CurrentThread.CurrentCulture.Name.Should().Be("en-US");
            _yen.ToString("F3").Should().Be("765.000 Japanese yen");
            _euro.ToString("F3").Should().Be("765.430 Euro");
            _dollar.ToString("F3").Should().Be("765.430 United States dollar");
            _dinar.ToString("F3").Should().Be("765.432 Bahraini dinar");
        }

    [Fact]
    [UseCulture("en-US")]
    public void WhenFourDecimals_ThenThisShouldSucceed()
    {
            Thread.CurrentThread.CurrentCulture.Name.Should().Be("en-US");
            _yen.ToString("F4").Should().Be("765.0000 Japanese yen");
            _euro.ToString("F4").Should().Be("765.4300 Euro");
            _dollar.ToString("F4").Should().Be("765.4300 United States dollar");
            _dinar.ToString("F4").Should().Be("765.4320 Bahraini dinar");
        }
}
