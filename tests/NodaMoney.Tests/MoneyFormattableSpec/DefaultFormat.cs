using System;
using System.Globalization;
using System.Threading;
using FluentAssertions;
using NodaMoney.Tests.Helpers;
using Xunit;

namespace NodaMoney.Tests.MoneyFormattableSpec;

[Collection(nameof(NoParallelization))]
public class DefaultFormat
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
        Thread.CurrentThread.CurrentCulture.Name.Should().Be(""); // InvariantCulture
        _yen.ToString().Should().Be("(¥98,765)");
        _euro.ToString().Should().Be("(€98,765.43)");
        _dollar.ToString().Should().Be("($98,765.43)");
        _dinar.ToString().Should().Be("(BD98,765.432)");
        _swissFranc.ToString().Should().Be("(Fr.98,765.43)");
    }

    [Fact]
    [UseCulture("en-US")]
    public void ThenEqualToFormatC()
    {
        Thread.CurrentThread.CurrentCulture.Name.Should().Be("en-US");
        _yen.ToString().Should().Be(_yen.ToString("C"));
        _euro.ToString().Should().Be(_euro.ToString("C"));
        _dollar.ToString().Should().Be(_dollar.ToString("C"));
        _dinar.ToString().Should().Be(_dinar.ToString("C"));
    }

    [Fact]
    [UseCulture("en-US")]
    public void WhenCurrentCultureUS_ThenDecimalsFollowsCurrencyAndAmountFollowsCurrentCultureUS()
    {
        Thread.CurrentThread.CurrentCulture.Name.Should().Be("en-US");
        _yen.ToString().Should().Be("-¥98,765");
        _euro.ToString().Should().Be("-€98,765.43");
        _dollar.ToString().Should().Be("-$98,765.43");
        _dinar.ToString().Should().Be("-BD98,765.432");
        _swissFranc.ToString().Should().Be("-Fr.98,765.43");
    }

    [Fact]
    [UseCulture("nl-NL")]
    public void WhenCurrentCultureNL_ThenDecimalsFollowsCurrencyAndAmountFollowsCurrentCultureNL()
    {
        Thread.CurrentThread.CurrentCulture.Name.Should().Be("nl-NL");
        _yen.ToString().Should().Be("¥ -98.765");
        _euro.ToString().Should().Be("€ -98.765,43");
        _dollar.ToString().Should().Be("$ -98.765,43");
        _dinar.ToString().Should().Be("BD -98.765,432");
        _swissFranc.ToString().Should().Be("Fr. -98.765,43");
    }

    [Fact]
    [UseCulture("fr-FR")]
    public void WhenCurrentCultureFR_ThenDecimalsFollowsCurrencyAndAmountFollowsCurrentCultureFR()
    {
        //   is a non-breaking space for group separator
        Thread.CurrentThread.CurrentCulture.Name.Should().Be("fr-FR");
        _yen.ToString().Should().Be("-98 765 ¥");
        _euro.ToString().Should().Be("-98 765,43 €");
        _dollar.ToString().Should().Be("-98 765,43 $");
        _dinar.ToString().Should().Be("-98 765,432 BD");
        _swissFranc.ToString().Should().Be("-98 765,43 Fr.");
    }

    [Fact]
    [UseCulture("de-CH")]
    public void WhenToStringAndCurrentCultureDeCH_ThenDecimalsFollowsCurrencyAndAmountFollowsCurrentCultureFR()
    {
        Thread.CurrentThread.CurrentCulture.Name.Should().Be("de-CH");
        _yen.ToString().Should().Be("¥-98’765");
        _euro.ToString().Should().Be("€-98’765.43");
        _dollar.ToString().Should().Be("$-98’765.43");
        _dinar.ToString().Should().Be("BD-98’765.432");
        _swissFranc.ToString().Should().Be("Fr.-98’765.43");
    }

    [Fact]
    public void WhenNullFormat_ThenThisShouldNotThrow()
    {
        Action action = () => _yen.ToString((string)null);

        action.Should().NotThrow<ArgumentNullException>();
    }

    [Fact]
    [UseCulture("en-US")]
    public void WhenGivenCultureInfo_ThenDecimalsFollowsCurrencyAndAmountFollowsGivenCultureInfo()
    {
        var ci = new CultureInfo("nl-NL");

        Thread.CurrentThread.CurrentCulture.Name.Should().Be("en-US");
        _yen.ToString(ci).Should().Be("¥ -98.765");
        _euro.ToString(ci).Should().Be("€ -98.765,43");
        _dollar.ToString(ci).Should().Be("$ -98.765,43");
        _dinar.ToString(ci).Should().Be("BD -98.765,432");
        _swissFranc.ToString(ci).Should().Be("Fr. -98.765,43");
    }

    [Fact]
    [UseCulture("en-US")]
    public void WhenGivenNumberFormat_ThenDecimalsFollowsCurrencyAndAmountFollowsGivenNumberFormat()
    {
        var nfi = new CultureInfo("nl-NL").NumberFormat;

        Thread.CurrentThread.CurrentCulture.Name.Should().Be("en-US");
        _yen.ToString(nfi).Should().Be("¥ -98.765");
        _euro.ToString(nfi).Should().Be("€ -98.765,43");
        _dollar.ToString(nfi).Should().Be("$ -98.765,43");
        _dinar.ToString(nfi).Should().Be("BD -98.765,432");
        _swissFranc.ToString(nfi).Should().Be("Fr. -98.765,43");
    }

    [Fact]
    public void WhenNullFormatNumberFormatIsUsed_ThenThisShouldNotThrow()
    {
        Action action = () => _yen.ToString((NumberFormatInfo)null);

        action.Should().NotThrow<ArgumentNullException>();
    }

    [Fact]
    public void WhenUsingToStringWithOneStringArgument_ThenExpectsTheSameAsWithTwoArguments()
    {
        string oneParameter() => _yen.ToString((string)null);
        string twoParameter() => _yen.ToString(null, null);

        oneParameter().Should().Be(twoParameter());
    }

    [Fact]
    public void WhenUsingToStringWithOneProviderArgument_ThenExpectsTheSameAsWithTwoArguments()
    {
        string oneParameter() => _yen.ToString((IFormatProvider)null);
        string twoParameter() => _yen.ToString(null, null);

        oneParameter().Should().Be(twoParameter());
    }

    [Fact]
    public void WhenUsingToStringWithCFormat_ThenReturnsTheSameAsTheDefaultFormat()
    {
        _yen.ToString("C", null).Should().Be(_yen.ToString(null, null));
    }

    [Fact]
    public void WhenUsingToStringWithCFormatWithCulture_ThenReturnsTheSameAsTheDefaultFormatWithThatCulture()
    {
        _yen.ToString("C", CultureInfo.InvariantCulture).Should().Be(_yen.ToString(null, CultureInfo.InvariantCulture));
        _yen.ToString("C", CultureInfo.GetCultureInfo("nl-NL")).Should().Be(_yen.ToString(null, CultureInfo.GetCultureInfo("nl-NL")));
        _yen.ToString("C", CultureInfo.GetCultureInfo("fr-FR")).Should().Be(_yen.ToString(null, CultureInfo.GetCultureInfo("fr-FR")));
    }

    [Fact]
    public void WhenNumberOfDecimalsIsNotApplicable_ThenToStringShouldNotFail()
    {
        var xdr = new Money(765.4321m, CurrencyInfo.FromCode("XDR"));

        Action action = () => xdr.ToString();

        action.Should().NotThrow<Exception>();
    }
}
