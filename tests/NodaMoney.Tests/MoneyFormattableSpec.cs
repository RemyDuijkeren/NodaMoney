using System;
using System.Globalization;
using System.Threading;
using FluentAssertions;
using Xunit;
using NodaMoney.Tests.Helpers;

namespace NodaMoney.Tests.MoneyFormattableSpec
{
    [Collection(nameof(NoParallelization))]
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
            Thread.CurrentThread.CurrentCulture.Name.Should().Be("en-US");
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

            action.Should().NotThrow<ArgumentNullException>();
        }

        [Fact]
        [UseCulture("en-US")]
        public void WhenGivenCultureInfo_ThenDecimalsFollowsCurrencyAndAmountFollowsGivenCultureInfo()
        {
            var ci = new CultureInfo("nl-NL");

            Thread.CurrentThread.CurrentCulture.Name.Should().Be("en-US");
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

            Thread.CurrentThread.CurrentCulture.Name.Should().Be("en-US");
            _yen.ToString(nfi).Should().Be("¥ 765");
            _euro.ToString(nfi).Should().Be("€ 765,43");
            _dollar.ToString(nfi).Should().Be("$ 765,43");
            _dinar.ToString(nfi).Should().Be("BD 765,432");
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
            _yen.ToString("G", null).Should().Be(_yen.ToString(null, null));
        }

        [Fact]
        public void WhenUsingToStringWithGFormatWithCulture_ThenReturnsTheSameAsTheDefaultFormatWithThatCulture()
        {
            _yen.ToString("G", CultureInfo.InvariantCulture).Should().Be(_yen.ToString(null, CultureInfo.InvariantCulture));
            _yen.ToString("G", CultureInfo.GetCultureInfo("nl-NL")).Should().Be(_yen.ToString(null, CultureInfo.GetCultureInfo("nl-NL")));
            _yen.ToString("G", CultureInfo.GetCultureInfo("fr-FR")).Should().Be(_yen.ToString(null, CultureInfo.GetCultureInfo("fr-FR")));
        }

        [Fact]
        public void WhenNumberOfDecimalsIsNotApplicable_ThenToStringShouldNotFail()
        {
            var xdr = new Money(765.4321m, Currency.FromCode("XDR"));
            
            Action action = () => xdr.ToString();

            action.Should().NotThrow<Exception>();
        }
    }

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

    [Collection(nameof(NoParallelization))]
    public class GivenIWantMoneyAsStringWithCurrencyCode
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
            Thread.CurrentThread.CurrentCulture.Name.Should().Be("en-US");
            _yen.ToString("I0").Should().Be("JPY 765");
            _euro.ToString("I0").Should().Be("EUR 765");
            _dollar.ToString("I0").Should().Be("USD 765");
            _dinar.ToString("I0").Should().Be("BHD 765");
        }

        [Fact]
        [UseCulture("en-US")]
        public void WhenOneDecimals_ThenThisShouldSucceed()
        {
            Thread.CurrentThread.CurrentCulture.Name.Should().Be("en-US");
            _yen.ToString("I1").Should().Be("JPY 765.0");
            _euro.ToString("I1").Should().Be("EUR 765.4");
            _dollar.ToString("I1").Should().Be("USD 765.4");
            _dinar.ToString("I1").Should().Be("BHD 765.4");
        }

        [Fact]
        [UseCulture("en-US")]
        public void WhenTwoDecimals_ThenThisShouldSucceed()
        {
            Thread.CurrentThread.CurrentCulture.Name.Should().Be("en-US");
            _yen.ToString("I2").Should().Be("JPY 765.00");
            _euro.ToString("I2").Should().Be("EUR 765.43");
            _dollar.ToString("I2").Should().Be("USD 765.43");
            _dinar.ToString("I2").Should().Be("BHD 765.43");
        }

        [Fact]
        [UseCulture("en-US")]
        public void WhenThreeDecimals_ThenThisShouldSucceed()
        {
            Thread.CurrentThread.CurrentCulture.Name.Should().Be("en-US");
            _yen.ToString("I3").Should().Be("JPY 765.000");
            _euro.ToString("I3").Should().Be("EUR 765.430");
            _dollar.ToString("I3").Should().Be("USD 765.430");
            _dinar.ToString("I3").Should().Be("BHD 765.432");
        }

        [Fact]
        [UseCulture("en-US")]
        public void WhenFourDecimals_ThenThisShouldSucceed()
        {
            Thread.CurrentThread.CurrentCulture.Name.Should().Be("en-US");
            _yen.ToString("I4").Should().Be("JPY 765.0000");
            _euro.ToString("I4").Should().Be("EUR 765.4300");
            _dollar.ToString("I4").Should().Be("USD 765.4300");
            _dinar.ToString("I4").Should().Be("BHD 765.4320");
        }
    }
    
    [Collection(nameof(NoParallelization))]
    public class GivenIWantMoneyAsStringWithEnglishCurrencyName
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
}
