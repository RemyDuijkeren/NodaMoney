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
    private readonly Money _yen = new Money(2765.4321m, CurrencyInfo.FromCode("JPY"));
    private readonly Money _euro = new Money(2765.4321m, CurrencyInfo.FromCode("EUR"));
    private readonly Money _dollar = new Money(2765.4321m, CurrencyInfo.FromCode("USD"));
    private readonly Money _dinar = new Money(2765.4321m, CurrencyInfo.FromCode("BHD"));

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
        _yen.ToString().Should().Be("¥2,765");
        _euro.ToString().Should().Be("€2,765.43");
        _dollar.ToString().Should().Be("$2,765.43");
        _dinar.ToString().Should().Be("BD2,765.432");
    }

    [Fact]
    [UseCulture("nl-NL")]
    public void WhenCurrentCultureNL_ThenDecimalsFollowsCurrencyAndAmountFollowsCurrentCultureNL()
    {
        Thread.CurrentThread.CurrentCulture.Name.Should().Be("nl-NL");
        _yen.ToString().Should().Be("¥ 2.765");
        _euro.ToString().Should().Be("€ 2.765,43");
        _dollar.ToString().Should().Be("$ 2.765,43");
        _dinar.ToString().Should().Be("BD 2.765,432");
    }

    [Fact]
    [UseCulture("fr-FR")]
    public void WhenCurrentCultureFR_ThenDecimalsFollowsCurrencyAndAmountFollowsCurrentCultureFR()
    {
        Thread.CurrentThread.CurrentCulture.Name.Should().Be("fr-FR");
        _yen.ToString().Should().Be("2 765 ¥");
        _euro.ToString().Should().Be("2 765,43 €");
        _dollar.ToString().Should().Be("2 765,43 $");
        _dinar.ToString().Should().Be("2 765,432 BD");
    }

    [Fact]
    [UseCulture(null)]
    public void WhenCurrentCultureInvariant_ThenDecimalsFollowsCurrencyAndAmountFollowsCurrentCulture()
    {
        Thread.CurrentThread.CurrentCulture.Name.Should().Be(""); // InvariantCulture
        _yen.ToString().Should().Be("¥2,765");
        _euro.ToString().Should().Be("€2,765.43");
        _dollar.ToString().Should().Be("$2,765.43");
        _dinar.ToString().Should().Be("BD2,765.432");
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
        _yen.ToString(ci).Should().Be("¥ 2.765");
        _euro.ToString(ci).Should().Be("€ 2.765,43");
        _dollar.ToString(ci).Should().Be("$ 2.765,43");
        _dinar.ToString(ci).Should().Be("BD 2.765,432");
    }

    [Fact]
    [UseCulture("en-US")]
    public void WhenGivenNumberFormat_ThenDecimalsFollowsCurrencyAndAmountFollowsGivenNumberFormat()
    {
        var nfi = new CultureInfo("nl-NL").NumberFormat;

        Thread.CurrentThread.CurrentCulture.Name.Should().Be("en-US");
        _yen.ToString(nfi).Should().Be("¥ 2.765");
        _euro.ToString(nfi).Should().Be("€ 2.765,43");
        _dollar.ToString(nfi).Should().Be("$ 2.765,43");
        _dinar.ToString(nfi).Should().Be("BD 2.765,432");
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
    public void WhenUsingToStringWithGFormat_ThenReturnsTheSameAsTheDefaultFormat()
    {
        // according to https://docs.microsoft.com/en-us/dotnet/api/system.iformattable.tostring?view=netframework-4.7.2#notes-to-implementers
        // the result of using "G" should return the same result as using <null>
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
        var xdr = new Money(765.4321m, Currency.FromCode("XDR"));

        Action action = () => xdr.ToString();

        action.Should().NotThrow<Exception>();
    }
}
