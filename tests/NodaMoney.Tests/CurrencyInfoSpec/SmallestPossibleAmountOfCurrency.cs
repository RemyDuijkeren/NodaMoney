using System.Globalization;

namespace NodaMoney.Tests.CurrencyInfoSpec;

public class SmallestPossibleAmountOfCurrency
{
    private readonly CurrencyInfo _eur = CurrencyInfo.FromCode("EUR");
    private readonly CurrencyInfo _yen = CurrencyInfo.FromCode("JPY");
    private readonly CurrencyInfo _din = CurrencyInfo.FromCode("BHD");
    private readonly CurrencyInfo _xau = CurrencyInfo.FromCode("XAU"); // Gold
    private readonly CurrencyInfo _xxx = CurrencyInfo.FromCode("XXX"); // NoCurrency
    private readonly CurrencyInfo _mga = CurrencyInfo.FromCode("MGA"); // Malagasy ariary
    private readonly CurrencyInfo _btc = CurrencyInfo.FromCode("BTC"); // Bitcoin
    private readonly CurrencyInfo _eth = CurrencyInfo.FromCode("ETH"); // Ethereum

    [Fact]
    public void WhenEuro_ThenShouldScaleBy100()
    {
        _eur.MinorUnit.Should().Be(MinorUnit.Two);
        _eur.MinorUnits.Should().Be(100);
        _eur.ScaleFactor.Should().Be(100);
        _eur.MinorUnitIsDecimalBased.Should().BeTrue();
        _eur.MinimalAmount.Should().Be(0.01m);
        _eur.DecimalDigits.Should().Be(2).And.Subject.Should().Be(_eur.Scale);
    }

    [Fact]
    public void WhenYen_ThenShouldBScaleByOne()
    {
        _yen.MinorUnit.Should().Be(MinorUnit.Zero);
        _yen.MinorUnits.Should().Be(0);
        _yen.ScaleFactor.Should().Be(1); // 10^0 = 1
        _yen.MinorUnitIsDecimalBased.Should().BeTrue();
        _yen.MinimalAmount.Should().Be(1m);
        _yen.DecimalDigits.Should().Be(0).And.Subject.Should().Be(_yen.Scale);
    }

    [Fact]
    public void WhenDinar_ThenShouldBScaleBy1000()
    {
        _din.MinorUnit.Should().Be(MinorUnit.Three);
        _din.MinorUnits.Should().Be(1_000);
        _din.ScaleFactor.Should().Be(1_000);
        _din.MinorUnitIsDecimalBased.Should().BeTrue();
        _din.MinimalAmount.Should().Be(0.001m);
        _din.DecimalDigits.Should().Be(3).And.Subject.Should().Be(_din.Scale);
    }

    [Fact]
    public void WhenMalagasyAriary_ThenScaleBy5()
    {
        // The Malagasy ariary are technically divided into five subunits, where the coins display "1/5" on their face and are referred to
        // as a "fifth"; These are not used in practice, but when written out, a single significant digit is used. E.g., 1.2 UM.
        _mga.MinorUnit.Should().Be(MinorUnit.OneFifth); // 1/5 = 10^Log10(5) => exponent Log10(5)
        _mga.MinorUnits.Should().Be(5);
        _mga.ScaleFactor.Should().Be(5); // 1/5
        _mga.MinorUnitIsDecimalBased.Should().BeFalse();
        _mga.MinimalAmount.Should().Be(0.2m);
        _mga.DecimalDigits.Should().Be(1).And.Subject.Should().Be(_mga.Scale); // According to ISO-4217, this is 2

        // var ci = CultureInfo.GetCultureInfo("mg-MG");
        // ci.NumberFormat.CurrencyDecimalDigits.Should().Be(0);
    }

    [Fact]
    public void WhenGold_ThenShouldBScaleByOne()
    {
        _xau.MinorUnit.Should().Be(MinorUnit.NotApplicable); // N.A.
        _xau.MinorUnits.Should().Be(0);
        _xau.ScaleFactor.Should().Be(1); // 10^0 = 1
        _xau.MinorUnitIsDecimalBased.Should().BeTrue();
        _xau.MinimalAmount.Should().Be(1m);
        _xau.DecimalDigits.Should().Be(0).And.Subject.Should().Be(_xau.Scale);
    }

    [Fact]
    public void WhenNoCurrency_ThenShouldScaleByOne()
    {
        _xxx.MinorUnit.Should().Be(MinorUnit.NotApplicable); // N.A.
        _xxx.MinorUnits.Should().Be(0);
        _xxx.ScaleFactor.Should().Be(1); // 10^0 = 1
        _xxx.MinorUnitIsDecimalBased.Should().BeTrue();
        _xxx.MinimalAmount.Should().Be(1m);
        _xxx.DecimalDigits.Should().Be(0).And.Subject.Should().Be(_xau.Scale);
    }

    [Fact]
    public void WhenBitCoin_ThenShouldScaleCorrect()
    {
        _btc.MinorUnit.Should().Be(MinorUnit.Eight);
        _btc.MinorUnits.Should().Be(100_000_000);
        _btc.ScaleFactor.Should().Be(100_000_000);
        _btc.MinorUnitIsDecimalBased.Should().BeTrue();
        _btc.MinimalAmount.Should().Be(0.00000001m);
        _btc.DecimalDigits.Should().Be(8).And.Subject.Should().Be(_btc.Scale);
    }

    [Fact]
    public void WhenEthereum_ThenShouldScaleCorrect()
    {
        _eth.MinorUnit.Should().Be(MinorUnit.Eighteen);
        _eth.MinorUnits.Should().Be(1_000_000_000_000_000_000);
        _eth.ScaleFactor.Should().Be(1_000_000_000_000_000_000);
        _eth.MinorUnitIsDecimalBased.Should().BeTrue();
        _eth.MinimalAmount.Should().Be(0.000000000000000001m);
        _eth.DecimalDigits.Should().Be(18).And.Subject.Should().Be(_eth.Scale);
    }
}
