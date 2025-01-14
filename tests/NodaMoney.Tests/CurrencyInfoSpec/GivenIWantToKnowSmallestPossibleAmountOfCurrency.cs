using FluentAssertions;
using Xunit;

namespace NodaMoney.Tests.CurrencyInfoSpec;

public class GivenIWantToKnowSmallestPossibleAmountOfCurrency
{
    private CurrencyInfo _eur = CurrencyInfo.FromCode("EUR");

    private CurrencyInfo _yen = CurrencyInfo.FromCode("JPY");

    private CurrencyInfo _din = CurrencyInfo.FromCode("BHD");

    private CurrencyInfo _mga = CurrencyInfo.FromCode("MGA"); // Malagasy ariary

    private CurrencyInfo _xau = CurrencyInfo.FromCode("XAU"); // Gold

    [Fact]
    public void WhenEuro_ThenShouldBeDividedBy100()
    {
        _eur.MinorUnit.Should().Be(MinorUnit.Two);
        // _eur.MinorUnit.Should().Be(100);
        _eur.MinimalAmount.Should().Be(0.01m);
        _eur.DecimalDigits.Should().Be(2);
    }

    [Fact]
    public void WhenYen_ThenShouldBeDividedByNothing()
    {
        _yen.MinorUnit.Should().Be(MinorUnit.Zero);
        _yen.MinimalAmount.Should().Be(1m);
        _yen.DecimalDigits.Should().Be(0);
    }

    [Fact]
    public void WhenDinar_ThenShouldBeDividedBy1000()
    {
        _din.MinorUnit.Should().Be(MinorUnit.Three);
        // _din.MinorUnit.Should().Be(1000);
        _din.MinimalAmount.Should().Be(0.001m);
        _din.DecimalDigits.Should().Be(3);
    }

    [Fact]
    public void WhenGold_ThenShouldBeDividedByNothing()
    {
        _xau.MinorUnit.Should().Be(MinorUnit.NotApplicable); // N.A.
        _xau.MinimalAmount.Should().Be(1m);
        _xau.DecimalDigits.Should().Be(0);
    }

    [Fact]
    public void WhenMalagasyAriary_ThenShouldBeDividedBy5()
    {
        // The Malagasy ariary are technically divided into five subunits, where the coins display "1/5" on their face and
        // are referred to as a "fifth"; These are not used in practice, but when written out, a single significant digit
        // is used. E.g. 1.2 UM.
        _mga.MinorUnit.Should().Be(MinorUnit.OneFifth); // 1/5 = 10^Log1010(5) => exponent Log10(5)
        // _mga.MinorUnit.Should().Be(5); // 1/5
        _mga.MinimalAmount.Should().Be(0.2m);
        _mga.DecimalDigits.Should().Be(1); // According to ISO-4217 this is 2
    }
}
