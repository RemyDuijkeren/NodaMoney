using System;
using System.Globalization;
using FluentAssertions;
using Xunit;
using NodaMoney.Tests.Helpers;

namespace NodaMoney.Tests.Spec
{
    public class GivenIWantToConvertMoneyToString
    {
        private Money _yen = new Money(765.4321m, Currency.FromCode("JPY"));

        private Money _euro = new Money(765.4321m, Currency.FromCode("EUR"));

        private Money _dollar = new Money(765.4321m, Currency.FromCode("USD"));

        private Money _dinar = new Money(765.4321m, Currency.FromCode("BHD"));

        [Fact]
        [UseCulture("en-US")]
        public void WhenImplicitConversion_ThenNumberOfDecimalsShouldBeDefaultOfCurrency()
        {
            _yen.ToString().Should().Be("¥765");
            _euro.ToString().Should().Be("€765.43");
            _dollar.ToString().Should().Be("$765.43");
            _dinar.ToString().Should().Be("BD765.432");
        }

        [Fact]
        [UseCulture("en-US")]
        public void WhenExplicitToZeroDecimals_ThenThisShouldSucceed()
        {
            _yen.ToString("C0").Should().Be("¥765");
            _euro.ToString("C0").Should().Be("€765");
            _dollar.ToString("C0").Should().Be("$765");
            _dinar.ToString("C0").Should().Be("BD765");
        }

        [Fact]
        [UseCulture("en-US")]
        public void WhenExplicitToOneDecimals_ThenThisShouldSucceed()
        {
            _yen.ToString("C1").Should().Be("¥765.0");
            _euro.ToString("C1").Should().Be("€765.4");
            _dollar.ToString("C1").Should().Be("$765.4");
            _dinar.ToString("C1").Should().Be("BD765.4");
        }

        [Fact]
        [UseCulture("en-US")]
        public void WhenExplicitToTwoDecimals_ThenThisShouldSucceed()
        {
            _yen.ToString("C2").Should().Be("¥765.00");
            _euro.ToString("C2").Should().Be("€765.43");
            _dollar.ToString("C2").Should().Be("$765.43");
            _dinar.ToString("C2").Should().Be("BD765.43");
        }

        [Fact]
        [UseCulture("en-US")]
        public void WhenExplicitToThreeDecimals_ThenThisShouldSucceed()
        {
            _yen.ToString("C3").Should().Be("¥765.000");
            _euro.ToString("C3").Should().Be("€765.430");
            _dollar.ToString("C3").Should().Be("$765.430");
            _dinar.ToString("C3").Should().Be("BD765.432");
        }

        [Fact]
        [UseCulture("en-US")]
        public void WhenExplicitToFourDecimals_ThenThisShouldSucceed()
        {
            _yen.ToString("C4").Should().Be("¥765.0000");
            _euro.ToString("C4").Should().Be("€765.4300");
            _dollar.ToString("C4").Should().Be("$765.4300");
            _dinar.ToString("C4").Should().Be("BD765.4320");
        }

        [Fact]
        public void WhenEmptyFormat_ThenThisShouldThrow()
        {
            Action action = () => _yen.ToString((string)null);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        [UseCulture("en-US")]
        public void WhenSpecificCultureIsUsed_ThenCurrencySymbolAndDecimalsOfMoneyShouldStillBeLeading()
        {
            var ci = new CultureInfo("nl-NL");

            _yen.ToString(ci).Should().Be("¥ 765");
            _euro.ToString(ci).Should().Be("€ 765,43");
            _dollar.ToString(ci).Should().Be("$ 765,43");
            _dinar.ToString(ci).Should().Be("BD 765,432");
        }

        [Fact]
        [UseCulture("en-US")]
        public void WhenSpecificNumberFormatIsUsed_ThenCurrencySymbolAndDecimalsOfMoneyShouldStillBeLeading()
        {
            var nfi = new CultureInfo("nl-NL").NumberFormat;

            _yen.ToString(nfi).Should().Be("¥ 765");
            _euro.ToString(nfi).Should().Be("€ 765,43");
            _dollar.ToString(nfi).Should().Be("$ 765,43");
            _dinar.ToString(nfi).Should().Be("BD 765,432");
        }

        [Fact]
        public void WhenEmptyFormatNumberFormatIsUsed_ThenThisShouldThrow()
        {
            Action action = () => _yen.ToString((NumberFormatInfo)null);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        [UseCulture("nl-BE")]
        public void WhenShowingMoneyInBelgiumDutchSpeaking_ThenThisShouldSucceed()
        {
            _yen.ToString().Should().Be("¥ 765");
            _euro.ToString().Should().Be("€ 765,43");
            _dollar.ToString().Should().Be("$ 765,43");
            _dinar.ToString().Should().Be("BD 765,432");
        }

        [Fact]
        [UseCulture("fr-BE")]
        public void WhenShowingMoneyInBelgiumFrenchSpeaking_ThenThisShouldSucceed()
        {
            _yen.ToString().Should().Be("765 ¥");
            _euro.ToString().Should().Be("765,43 €");
            _dollar.ToString().Should().Be("765,43 $");
            _dinar.ToString().Should().Be("765,432 BD");
        }
    }
}