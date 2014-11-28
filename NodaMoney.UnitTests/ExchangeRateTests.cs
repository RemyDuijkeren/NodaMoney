using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NodaMoney.UnitTests.Helpers;

namespace NodaMoney.UnitTests
{
    public class ExchangeRateTests
    {
        [TestClass]
        public class GivenIWantToParseACurrencyPair
        {
            [TestMethod]
            public void WhenCurrencyPairInUsCulture_ThenParsingShouldSucceed()
            {
                using (new SwitchCulture("en-US"))
                {
                    var fx1 = ExchangeRate.Parse("EUR/USD 1.2591");
                    Assert.AreEqual("EUR", fx1.BaseCurrency.Code);
                    Assert.AreEqual("USD", fx1.QuoteCurrency.Code);
                    Assert.AreEqual(1.2591M, fx1.Value);

                    var fx2 = ExchangeRate.Parse("EUR/USD1.2591");
                    Assert.AreEqual("EUR", fx2.BaseCurrency.Code);
                    Assert.AreEqual("USD", fx2.QuoteCurrency.Code);
                    Assert.AreEqual(1.2591M, fx2.Value);
                }
            }

            [TestMethod]
            public void WhenCurrencyPairInNlCulture_ThenParsingShouldSucceed()
            {
                using (new SwitchCulture("nl-NL"))
                {
                    var fx1 = ExchangeRate.Parse("EUR/USD 1,2591");
                    Assert.AreEqual("EUR", fx1.BaseCurrency.Code);
                    Assert.AreEqual("USD", fx1.QuoteCurrency.Code);
                    Assert.AreEqual(1.2591M, fx1.Value);

                    var fx2 = ExchangeRate.Parse("EUR/USD1,2591");
                    Assert.AreEqual("EUR", fx2.BaseCurrency.Code);
                    Assert.AreEqual("USD", fx2.QuoteCurrency.Code);
                    Assert.AreEqual(1.2591M, fx2.Value);
                }
            }

            [TestMethod]
            [ExpectedException(typeof(FormatException))]
            public void WhenCurrencyPairIsNotANumber_ThenParsingShouldFail()
            {
                var fx = ExchangeRate.Parse("EUR/USD 1,ABC");
            }

            [TestMethod]
            [ExpectedException(typeof(FormatException))]
            public void WhenCurrencyPairHasSameCurrencies_ThenParsingShouldFail()
            {
                using (new SwitchCulture("en-US"))
                {
                    var fx = ExchangeRate.Parse("EUR/EUR 1.2591");
                }
            }
        }

        [TestClass]
        public class GivenIWantToConvertMoney
        {
            private Currency euro = Currency.FromCode("EUR");
            private Currency dollar = Currency.FromCode("USD");
            private ExchangeRate fx = new ExchangeRate(Currency.FromCode("EUR"), Currency.FromCode("USD"), 1.2591); // EUR/USD 1.2591

            [TestMethod, Description("When Converting €100,99 With EUR/USD 1.2591, Then Result Should Be $127.16")]
            public void WhenConvertingEurosToDollars_ThenConversionShouldBeCorrect()
            {
                // Convert €100,99 to $127.156509 (= €100.99 * 1.2591)
                Money converted = fx.Convert(Money.Euro(100.99M));
                Assert.AreEqual(dollar, converted.Currency);
                Assert.AreEqual(127.16M, converted.Amount);
            }

            [TestMethod]
            public void WhenConvertingEurosToDollarsAndThenBack_ThenEndResultShouldBeTheSameAsInTheBeginning()
            {
                // Convert €100,99 to $127.156509 (= €100.99 * 1.2591)
                Money converted = fx.Convert(Money.Euro(100.99M));

                // Convert $127.16 to €100,99 (= $127.16 / 1.2591)
                Money revert = fx.Convert(converted);
                Assert.AreEqual(euro, revert.Currency);
                Assert.AreEqual(100.99M, revert.Amount);
            }
        }

        [TestClass]
        public class GivenIWantToCreateAnExchangeRate
        {
            private Currency euro = Currency.FromCode("EUR");
            private Currency dollar = Currency.FromCode("USD");

            [TestMethod]
            public void WhenRateIsDouble_ThenCreatingShouldSucceed()
            {
                var fx = new ExchangeRate(euro, dollar, 1.2591);
                Assert.AreEqual(euro, fx.BaseCurrency);
                Assert.AreEqual(dollar, fx.QuoteCurrency);
                Assert.AreEqual(1.2591M, fx.Value);
            }

            [TestMethod]
            public void WhenRateIsDecimal_ThenCreatingShouldSucceed()
            {
                var fx = new ExchangeRate(euro, dollar, 1.2591M);
                Assert.AreEqual(euro, fx.BaseCurrency);
                Assert.AreEqual(dollar, fx.QuoteCurrency);
                Assert.AreEqual(1.2591M, fx.Value);

                // float
                fx = new ExchangeRate(euro, dollar, 1.2591F);
                Assert.AreEqual(euro, fx.BaseCurrency);
                Assert.AreEqual(dollar, fx.QuoteCurrency);
                Assert.AreEqual(1.2591M, fx.Value);
            }

            [TestMethod]
            public void WhenRateIsFloat_ThenCreatingShouldSucceed()
            {
                var fx = new ExchangeRate(euro, dollar, 1.2591F);
                Assert.AreEqual(euro, fx.BaseCurrency);
                Assert.AreEqual(dollar, fx.QuoteCurrency);
                Assert.AreEqual(1.2591M, fx.Value);
            }

            [TestMethod]
            [ExpectedException(typeof(ArgumentException))]
            public void WhenBaseAndQuoteCurrencyAreTheSame_ThenCreatingShouldFail()
            {
                var fx = new ExchangeRate(euro, euro, 1.2591F);
            }

            [TestMethod]
            public void WhenConvertingToString_ThenReturnCurrencyPair()
            {
                ExchangeRate fx = new ExchangeRate(Currency.FromCode("EUR"), Currency.FromCode("USD"), 1.2524);

                Assert.AreEqual("EUR/USD 1,2524", fx.ToString());
            }
        }
    }
}
