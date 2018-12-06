using System;
using System.Globalization;
using System.Threading;

using FluentAssertions;
using Xunit;
using NodaMoney.Tests.Helpers;

namespace NodaMoney.Tests.MoneyFormattableSpec
{
    public class GivenIWantMoneyAsString
    {
        private Money _yen = new Money(765.4321m, Currency.FromCode("JPY"));
        private Money _euro = new Money(765.4321m, Currency.FromCode("EUR"));
        private Money _dollar = new Money(765.4321m, Currency.FromCode("USD"));
        private Money _dinar = new Money(765.4321m, Currency.FromCode("BHD"));

        [Fact]
        [UseCulture("en-US")]
        public void WhenToStringAndCurrentCultureUS_ThenDecimalsFollowsCurrencyAndAmountFollowsCurrentCultureUS()
        {
            _yen.ToString().Should().Be("¥765");
            _euro.ToString().Should().Be("€765.43");
            _dollar.ToString().Should().Be("$765.43");
            _dinar.ToString().Should().Be("BD765.432");
        }

        [Fact]
        [UseCulture("nl-NL")]
        public void WhenToStringAndCurrentCultureNL_ThenDecimalsFollowsCurrencyAndAmountFollowsCurrentCultureNL()
        {
            Thread.CurrentThread.CurrentCulture.Name.Should().Be("nl-NL");
            _yen.ToString().Should().Be("¥ 765");
            _euro.ToString().Should().Be("€ 765,43");
            _dollar.ToString().Should().Be("$ 765,43");
            _dinar.ToString().Should().Be("BD 765,432");
        }

        [Fact]
        [UseCulture("fr-FR")]
        public void WhenToStringAndCurrentCultureFR_ThenDecimalsFollowsCurrencyAndAmountFollowsCurrentCultureFR()
        {
            Thread.CurrentThread.CurrentCulture.Name.Should().Be("fr-FR");
            _yen.ToString().Should().Be("765 ¥");
            _euro.ToString().Should().Be("765,43 €");
            _dollar.ToString().Should().Be("765,43 $");
            _dinar.ToString().Should().Be("765,432 BD");
        }

        [Fact]
        public void WhenNullFormat_ThenThisShouldNotThrow()
        {
            Action action = () => _yen.ToString((string)null);

            action.ShouldNotThrow<ArgumentNullException>();
        }

        [Fact]
        [UseCulture("en-US")]
        public void WhenGivenCultureInfo_ThenDecimalsFollowsCurrencyAndAmountFollowsGivenCultureInfo()
        {
            var ci = new CultureInfo("nl-NL");

            _yen.ToString(ci).Should().Be("¥ 765");
            _euro.ToString(ci).Should().Be("€ 765,43");
            _dollar.ToString(ci).Should().Be("$ 765,43");
            _dinar.ToString(ci).Should().Be("BD 765,432");
        }

        [Fact]
        [UseCulture("en-US")]
        public void WhenGivenNumberFormat_ThenDecimalsFollowsCurrencyAndAmountFollowsGivenNumberFormat()
        {
            var nfi = new CultureInfo("nl-NL").NumberFormat;

            _yen.ToString(nfi).Should().Be("¥ 765");
            _euro.ToString(nfi).Should().Be("€ 765,43");
            _dollar.ToString(nfi).Should().Be("$ 765,43");
            _dinar.ToString(nfi).Should().Be("BD 765,432");
        }

        [Fact]
        public void WhenNullFormatNumberFormatIsUsed_ThenThisShouldNotThrow()
        {
            Action action = () => _yen.ToString((NumberFormatInfo)null);

            action.ShouldNotThrow<ArgumentNullException>();
        }

        [Fact]
        public void WhenUsingToStringWithOneStringArgument_ThenExpectsTheSameAsWithTwoArguments()
        {
            Func<string> oneParameter = () => _yen.ToString((string)null);
            Func<string> twoParameter = () => _yen.ToString((string)null, null);

            oneParameter().Should().Be(twoParameter());
        }

        [Fact]
        public void WhenUsingToStringWithOneProviderArgument_ThenExpectsTheSameAsWithTwoArguments()
        {
            Func<string> oneParameter = () => _yen.ToString((IFormatProvider)null);
            Func<string> twoParameter = () => _yen.ToString(null, null);

            oneParameter().Should().Be(twoParameter());
        }

    }

    public class GivenIWantMoneyAsStringWithCurrencySymbol
    {
        private Money _yen = new Money(765.4321m, Currency.FromCode("JPY"));
        private Money _euro = new Money(765.4321m, Currency.FromCode("EUR"));
        private Money _dollar = new Money(765.4321m, Currency.FromCode("USD"));
        private Money _dinar = new Money(765.4321m, Currency.FromCode("BHD"));

        [Fact]
        [UseCulture("en-US")]
        public void WhenCurrentCulturUS_ThenDecimalsFollowsCurrencyAndAmountFollowsCurrentCultureNL()
        {
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
            _yen.ToString("C0").Should().Be("¥765");
            _euro.ToString("C0").Should().Be("€765");
            _dollar.ToString("C0").Should().Be("$765");
            _dinar.ToString("C0").Should().Be("BD765");
        }

        [Fact]
        [UseCulture("en-US")]
        public void WhenOneDecimals_ThenThisShouldSucceed()
        {
            _yen.ToString("C1").Should().Be("¥765.0");
            _euro.ToString("C1").Should().Be("€765.4");
            _dollar.ToString("C1").Should().Be("$765.4");
            _dinar.ToString("C1").Should().Be("BD765.4");
        }

        [Fact]
        [UseCulture("en-US")]
        public void WhenTwoDecimals_ThenThisShouldSucceed()
        {
            _yen.ToString("C2").Should().Be("¥765.00");
            _euro.ToString("C2").Should().Be("€765.43");
            _dollar.ToString("C2").Should().Be("$765.43");
            _dinar.ToString("C2").Should().Be("BD765.43");
        }

        [Fact]
        [UseCulture("en-US")]
        public void WhenThreeDecimals_ThenThisShouldSucceed()
        {
            _yen.ToString("C3").Should().Be("¥765.000");
            _euro.ToString("C3").Should().Be("€765.430");
            _dollar.ToString("C3").Should().Be("$765.430");
            _dinar.ToString("C3").Should().Be("BD765.432");
        }

        [Fact]
        [UseCulture("en-US")]
        public void WhenFourDecimals_ThenThisShouldSucceed()
        {
            _yen.ToString("C4").Should().Be("¥765.0000");
            _euro.ToString("C4").Should().Be("€765.4300");
            _dollar.ToString("C4").Should().Be("$765.4300");
            _dinar.ToString("C4").Should().Be("BD765.4320");
        }
    }

    public class GivenIWantMoneyAsStringWithCurrencyCode
    {
        private Money _yen = new Money(765.4321m, Currency.FromCode("JPY"));
        private Money _euro = new Money(765.4321m, Currency.FromCode("EUR"));
        private Money _dollar = new Money(765.4321m, Currency.FromCode("USD"));
        private Money _dinar = new Money(765.4321m, Currency.FromCode("BHD"));

        [Fact]
        [UseCulture("en-US")]
        public void WhenCurrentCulturUS_ThenDecimalsFollowsCurrencyAndAmountFollowsCurrentCultureNL()
        {
            _yen.ToString("I").Should().Be("JPY 765");
            _euro.ToString("I").Should().Be("EUR 765.43");
            _dollar.ToString("I").Should().Be("USD 765.43");
            _dinar.ToString("I").Should().Be("BHD 765.432");
        }

        [Fact]
        [UseCulture("nl-NL")]
        public void WhenCurrentCultureNL_ThenDecimalsFollowsCurrencyAndAmountFollowsCurrentCultureNL()
        {
            Thread.CurrentThread.CurrentCulture.Name.Should().Be("nl-NL");
            _yen.ToString("I").Should().Be("JPY 765");
            _euro.ToString("I").Should().Be("EUR 765,43");
            _dollar.ToString("I").Should().Be("USD 765,43");
            _dinar.ToString("I").Should().Be("BHD 765,432");
        }

        [Fact]
        [UseCulture("fr-FR")]
        public void WhenCurrentCultureFR_ThenDecimalsFollowsCurrencyAndAmountFollowsCurrentCultureFR()
        {
            Thread.CurrentThread.CurrentCulture.Name.Should().Be("fr-FR");
            _yen.ToString("I").Should().Be("765 JPY");
            _euro.ToString("I").Should().Be("765,43 EUR");
            _dollar.ToString("I").Should().Be("765,43 USD");
            _dinar.ToString("I").Should().Be("765,432 BHD");
        }

        [Fact]
        [UseCulture("en-US")]
        public void WhenZeroDecimals_ThenThisShouldSucceed()
        {
            _yen.ToString("I0").Should().Be("JPY 765");
            _euro.ToString("I0").Should().Be("EUR 765");
            _dollar.ToString("I0").Should().Be("USD 765");
            _dinar.ToString("I0").Should().Be("BHD 765");
        }

        [Fact]
        [UseCulture("en-US")]
        public void WhenOneDecimals_ThenThisShouldSucceed()
        {
            _yen.ToString("I1").Should().Be("JPY 765.0");
            _euro.ToString("I1").Should().Be("EUR 765.4");
            _dollar.ToString("I1").Should().Be("USD 765.4");
            _dinar.ToString("I1").Should().Be("BHD 765.4");
        }

        [Fact]
        [UseCulture("en-US")]
        public void WhenTwoDecimals_ThenThisShouldSucceed()
        {
            _yen.ToString("I2").Should().Be("JPY 765.00");
            _euro.ToString("I2").Should().Be("EUR 765.43");
            _dollar.ToString("I2").Should().Be("USD 765.43");
            _dinar.ToString("I2").Should().Be("BHD 765.43");
        }

        [Fact]
        [UseCulture("en-US")]
        public void WhenThreeDecimals_ThenThisShouldSucceed()
        {
            _yen.ToString("I3").Should().Be("JPY 765.000");
            _euro.ToString("I3").Should().Be("EUR 765.430");
            _dollar.ToString("I3").Should().Be("USD 765.430");
            _dinar.ToString("I3").Should().Be("BHD 765.432");
        }

        [Fact]
        [UseCulture("en-US")]
        public void WhenFourDecimals_ThenThisShouldSucceed()
        {
            _yen.ToString("I4").Should().Be("JPY 765.0000");
            _euro.ToString("I4").Should().Be("EUR 765.4300");
            _dollar.ToString("I4").Should().Be("USD 765.4300");
            _dinar.ToString("I4").Should().Be("BHD 765.4320");
        }
    }
}