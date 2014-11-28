using System;
using System.Globalization;
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
                Currency[] currencies = Currency.GetAllCurrencies();

                Assert.IsNotNull(currencies);
                Assert.IsTrue(currencies.Length > 100);
            }
        }

        [TestClass]
        public class GivenIWantCurrencyFromIsoCode
        {
            [TestMethod]
            public void WhenIsoCodeIsExisting_ThenCreatingShouldSucceed()
            {
                var currency = Currency.FromCode("EUR");

                Assert.IsNotNull(currency);
                Assert.AreEqual(currency.Sign, "€");
                Assert.AreEqual(currency.Code, "EUR");
                Assert.AreEqual(currency.EnglishName, "Euro");
            }

            [TestMethod]
            [ExpectedException(typeof(ArgumentException))]
            public void WhenIsoCodeIsUnknown_ThenCreatingShouldFail()
            {
                var currency = Currency.FromCode("AAA");
            }
        }

        [TestClass]
        public class GivenIWantCurrencyFromRegionOrCulture
        {
            [TestMethod]
            public void WhenUsingRegionInfo_ThenCreatingShouldSucceed()
            {
                var currency = Currency.FromRegion(new RegionInfo("NL"));

                Assert.IsNotNull(currency);
                Assert.AreEqual(currency.Sign, "€");
                Assert.AreEqual(currency.Code, "EUR");
                Assert.AreEqual(currency.EnglishName, "Euro");
            }

            [TestMethod]
            public void WhenUsingRegionName_ThenCreatingShouldSucceed()
            {
                var currency = Currency.FromRegion("NL");

                Assert.IsNotNull(currency);
                Assert.AreEqual(currency.Sign, "€");
                Assert.AreEqual(currency.Code, "EUR");
                Assert.AreEqual(currency.EnglishName, "Euro");
            }

            [TestMethod]
            [ExpectedException(typeof(ArgumentNullException))]
            public void WhenUsingRegionNameThatIsNull_ThenCreatingShouldFail()
            {
                Currency.FromRegion((string)null);
            }

            [TestMethod]
            public void WhenUsingCultureInfo_ThenCreatingShouldSucceed()
            {
                var currency = Currency.FromCulture(CultureInfo.CreateSpecificCulture("nl-NL"));

                Assert.IsNotNull(currency);
                Assert.AreEqual(currency.Sign, "€");
                Assert.AreEqual(currency.Code, "EUR");
                Assert.AreEqual(currency.EnglishName, "Euro");
            }

            [TestMethod]
            [ExpectedException(typeof(ArgumentNullException))]
            public void WhenCultureInfoIsNull_ThenCreatingShouldFail()
            {
                Currency.FromCulture(null);
            }

            [TestMethod]
            public void WhenUsingCultureName_ThenCreatingShouldSucceed()
            {
                var currency = Currency.FromRegion("nl-NL");

                Assert.IsNotNull(currency);
                Assert.AreEqual(currency.Sign, "€");
                Assert.AreEqual(currency.Code, "EUR");
                Assert.AreEqual(currency.EnglishName, "Euro");
            }

            [TestMethod]
            public void WhenUsingCurrentCurrency_ThenCreatingShouldSucceed()
            {
                var currency = Currency.CurrentCurrency;

                Assert.AreEqual(Currency.FromRegion(RegionInfo.CurrentRegion), currency);
            }
        }

        [TestClass]
        public class GivenIWantToCompareCurrencies
        {
            private Currency euro1 = Currency.FromCode("EUR");
            private Currency euro2 = Currency.FromCode("EUR");
            private Currency dollar = Currency.FromCode("USD");

            [TestMethod]
            public void WhenComparingEquality_ThenCurrencyShouldBeEqual()
            {
                //using Equal()
                Assert.AreEqual(euro1, euro2);
                Assert.AreNotEqual(euro1, dollar);
                Assert.AreNotEqual(euro1, null);
                Assert.AreNotEqual(euro1, new object(), "Comparing Currency to a different object should fail!");
            }

            [TestMethod]
            public void WhenComparingStaticEquality_ThenCurrencyShouldBeEqual()
            {
                //using static Equal()
                Assert.IsTrue(Currency.Equals(euro1, euro2));
                Assert.IsFalse(Currency.Equals(euro1, dollar));
            }

            [TestMethod]
            public void WhenComparingWithEqualityOperator_ThenCurrencyShouldBeEqual()
            {
                //using Euality operators
                Assert.IsTrue(euro1 == euro2);
                Assert.IsTrue(euro1 != dollar);
            }

            [TestMethod]
            public void WhenComparingHashCodes_ThenCurrencyShouldBeEqual()
            {
                //using GetHashCode()
                Assert.AreEqual(euro1.GetHashCode(), euro2.GetHashCode());
                Assert.AreNotEqual(euro1.GetHashCode(), dollar.GetHashCode());
            }
        }
    }
}
