using System;
using System.Globalization;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NodaMoney.UnitTests
{    
    public class CurrencyTests
    {
        [TestClass]
        public class GivenIWantToKnowAllCurrencies
        {
            [TestMethod]
            public void WhenAskingForIt_ThenAllCurrenciesShouldBeReturned()
            {
                var currencies = Currency.GetAllCurrencies();

                currencies.Should().NotBeEmpty();
                currencies.Length.Should().BeGreaterThan(100);
            }
        }

        [TestClass]
        public class GivenIWantCurrencyFromIsoCode
        {
            [TestMethod]
            public void WhenIsoCodeIsExisting_ThenCreatingShouldSucceed()
            {
                var currency = Currency.FromCode("EUR");

                currency.Should().NotBeNull();
                currency.Sign.Should().Be("€");
                currency.Code.Should().Be("EUR");
                currency.EnglishName.Should().Be("Euro");
            }

            [TestMethod]
            public void WhenIsoCodeIsUnknown_ThenCreatingShouldThrow()
            {
                Action action = () => Currency.FromCode("AAA");

                action.ShouldThrow<ArgumentException>();
            }
        }

        [TestClass]
        public class GivenIWantCurrencyFromRegionOrCulture
        {
            [TestMethod]
            public void WhenUsingRegionInfo_ThenCreatingShouldSucceed()
            {
                var currency = Currency.FromRegion(new RegionInfo("NL"));

                currency.Should().NotBeNull();
                currency.Sign.Should().Be("€");
                currency.Code.Should().Be("EUR");
                currency.EnglishName.Should().Be("Euro");
            }

            [TestMethod]
            public void WhenUsingRegionName_ThenCreatingShouldSucceed()
            {
                var currency = Currency.FromRegion("NL");

                currency.Should().NotBeNull();
                currency.Sign.Should().Be("€");
                currency.Code.Should().Be("EUR");
                currency.EnglishName.Should().Be("Euro");
            }

            [TestMethod]
            public void WhenUsingRegionNameThatIsNull_ThenCreatingShouldThrow()
            {
                Action action = () => Currency.FromRegion((string)null);

                action.ShouldThrow<ArgumentNullException>();
            }

            [TestMethod]
            public void WhenUsingCultureInfo_ThenCreatingShouldSucceed()
            {
                var currency = Currency.FromCulture(CultureInfo.CreateSpecificCulture("nl-NL"));

                currency.Should().NotBeNull();
                currency.Sign.Should().Be("€");
                currency.Code.Should().Be("EUR");
                currency.EnglishName.Should().Be("Euro");
            }

            [TestMethod]
            public void WhenCultureInfoIsNull_ThenCreatingShouldThrow()
            {
                Action action = () => Currency.FromCulture(null);

                action.ShouldThrow<ArgumentNullException>();
            }

            [TestMethod]
            public void WhenUsingCultureName_ThenCreatingShouldSucceed()
            {
                var currency = Currency.FromRegion("nl-NL");

                currency.Should().NotBeNull();
                currency.Sign.Should().Be("€");
                currency.Code.Should().Be("EUR");
                currency.EnglishName.Should().Be("Euro");
            }

            [TestMethod]
            public void WhenUsingCurrentCurrency_ThenCreatingShouldSucceed()
            {
                var currency = Currency.CurrentCurrency;

                currency.Should().Be(Currency.FromRegion(RegionInfo.CurrentRegion));
            }
        }

        [TestClass]
        public class GivenIWantToCompareCurrencies
        {
            private Currency _euro1 = Currency.FromCode("EUR");
            private Currency _euro2 = Currency.FromCode("EUR");
            private Currency _dollar = Currency.FromCode("USD");

            [TestMethod]
            public void WhenComparingEquality_ThenCurrencyShouldBeEqual()
            {
                // Compare using Equal()
                _euro1.Should().Be(_euro2);
                _euro1.Should().NotBe(_dollar);
                _euro1.Should().NotBeNull();
                _euro1.Should().NotBe(new object(), "comparing Currency to a different object should fail!");
            }

            [TestMethod]
            public void WhenComparingStaticEquality_ThenCurrencyShouldBeEqual()
            {
                // Compare using static Equal()
                Currency.Equals(_euro1, _euro2).Should().BeTrue();
                Currency.Equals(_euro1, _dollar).Should().BeFalse();
            }

            [TestMethod]
            public void WhenComparingWithEqualityOperator_ThenCurrencyShouldBeEqual()
            {
                // Compare using Euality operators
                (_euro1 == _euro2).Should().BeTrue();
                (_euro1 != _dollar).Should().BeTrue();
            }

            [TestMethod]
            public void WhenComparingHashCodes_ThenCurrencyShouldBeEqual()
            {
                // Compare using GetHashCode()
                _euro1.GetHashCode().Should().Be(_euro2.GetHashCode());
                _euro1.GetHashCode().Should().NotBe(_dollar.GetHashCode());
            }
        }
    }
}
