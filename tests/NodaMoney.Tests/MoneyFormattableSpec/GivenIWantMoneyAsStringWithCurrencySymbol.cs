using System.Threading;
using FluentAssertions;
using NodaMoney.Tests.Helpers;
using Xunit;

namespace NodaMoney.Tests.MoneyFormattableSpec;

[Collection(nameof(NoParallelization))]
public class GivenIWantMoneyAsStringWithCurrencySymbol
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
        _yen.ToString("C").Should().Be("¥765");
        _euro.ToString("C").Should().Be("€765.43");
        _dollar.ToString("C").Should().Be("$765.43");
        _dinar.ToString("C").Should().Be("BD765.432");
    }

    [Fact]
    [UseCulture("nl-NL")]
    public void WhenCurrentCultureNL_ThenDecimalsFollowsCurrencyAndAmountFollowsCurrentCultureNL()
    {
        Thread.CurrentThread.CurrentCulture.Name.Should().Be("nl-NL");
        _yen.ToString("C").Should().Be("¥ 765");
        _euro.ToString("C").Should().Be("€ 765,43");
        _dollar.ToString("C").Should().Be("$ 765,43");
        _dinar.ToString("C").Should().Be("BD 765,432");
    }

    [Fact]
    [UseCulture("fr-FR")]
    public void WhenCurrentCultureFR_ThenDecimalsFollowsCurrencyAndAmountFollowsCurrentCultureFR()
    {
        Thread.CurrentThread.CurrentCulture.Name.Should().Be("fr-FR");
        _yen.ToString("C").Should().Be("765 ¥");
        _euro.ToString("C").Should().Be("765,43 €");
        _dollar.ToString("C").Should().Be("765,43 $");
        _dinar.ToString("C").Should().Be("765,432 BD");
    }

    [Fact]
    [UseCulture("en-US")]
    public void WhenZeroDecimals_ThenThisShouldSucceed()
    {
        Thread.CurrentThread.CurrentCulture.Name.Should().Be("en-US");
        _yen.ToString("C0").Should().Be("¥765");
        _euro.ToString("C0").Should().Be("€765");
        _dollar.ToString("C0").Should().Be("$765");
        _dinar.ToString("C0").Should().Be("BD765");
    }

    [Fact]
    [UseCulture("en-US")]
    public void WhenOneDecimals_ThenThisShouldSucceed()
    {
        Thread.CurrentThread.CurrentCulture.Name.Should().Be("en-US");
        _yen.ToString("C1").Should().Be("¥765.0");
        _euro.ToString("C1").Should().Be("€765.4");
        _dollar.ToString("C1").Should().Be("$765.4");
        _dinar.ToString("C1").Should().Be("BD765.4");
    }

    [Fact]
    [UseCulture("en-US")]
    public void WhenTwoDecimals_ThenThisShouldSucceed()
    {
        Thread.CurrentThread.CurrentCulture.Name.Should().Be("en-US");
        _yen.ToString("C2").Should().Be("¥765.00");
        _euro.ToString("C2").Should().Be("€765.43");
        _dollar.ToString("C2").Should().Be("$765.43");
        _dinar.ToString("C2").Should().Be("BD765.43");
    }

    [Fact]
    [UseCulture("en-US")]
    public void WhenThreeDecimals_ThenThisShouldSucceed()
    {
        Thread.CurrentThread.CurrentCulture.Name.Should().Be("en-US");
        _yen.ToString("C3").Should().Be("¥765.000");
        _euro.ToString("C3").Should().Be("€765.430");
        _dollar.ToString("C3").Should().Be("$765.430");
        _dinar.ToString("C3").Should().Be("BD765.432");
    }

    [Fact]
    [UseCulture("en-US")]
    public void WhenFourDecimals_ThenThisShouldSucceed()
    {
        Thread.CurrentThread.CurrentCulture.Name.Should().Be("en-US");
        _yen.ToString("C4").Should().Be("¥765.0000");
        _euro.ToString("C4").Should().Be("€765.4300");
        _dollar.ToString("C4").Should().Be("$765.4300");
        _dinar.ToString("C4").Should().Be("BD765.4320");
    }
}
