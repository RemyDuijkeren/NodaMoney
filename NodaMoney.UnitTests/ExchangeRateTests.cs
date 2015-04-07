using System;
using FluentAssertions;
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

                    fx1.BaseCurrency.Code.Should().Be("EUR");
                    fx1.QuoteCurrency.Code.Should().Be("USD");
                    fx1.Value.Should().Be(1.2591M);

                    var fx2 = ExchangeRate.Parse("EUR/USD1.2591");

                    fx2.BaseCurrency.Code.Should().Be("EUR");
                    fx2.QuoteCurrency.Code.Should().Be("USD");
                    fx2.Value.Should().Be(1.2591M);
                }
            }

            [TestMethod]
            public void WhenCurrencyPairInNlCulture_ThenParsingShouldSucceed()
            {
                using (new SwitchCulture("nl-NL"))
                {
                    var fx1 = ExchangeRate.Parse("EUR/USD 1,2591");

                    fx1.BaseCurrency.Code.Should().Be("EUR");
                    fx1.QuoteCurrency.Code.Should().Be("USD");
                    fx1.Value.Should().Be(1.2591M);

                    var fx2 = ExchangeRate.Parse("EUR/USD1,2591");

                    fx2.BaseCurrency.Code.Should().Be("EUR");
                    fx2.QuoteCurrency.Code.Should().Be("USD");
                    fx2.Value.Should().Be(1.2591M);
                }
            }

            [TestMethod]
            public void WhenCurrencyPairIsNotANumber_ThenParsingShouldThrow()
            {
                Action action = () => ExchangeRate.Parse("EUR/USD 1,ABC");

                action.ShouldThrow<FormatException>();
            }

            [TestMethod]
            public void WhenCurrencyPairHasSameCurrencies_ThenParsingShouldThrow()
            {
                using (new SwitchCulture("en-US"))
                {
                    Action action = () => ExchangeRate.Parse("EUR/EUR 1.2591");

                    action.ShouldThrow<FormatException>();
                }
            }
        }

        [TestClass]
        public class GivenIWantToConvertMoney
        {
            private readonly Currency _euro = Currency.FromCode("EUR");
            private readonly Currency _dollar = Currency.FromCode("USD");
            private ExchangeRate _exchangeRate = new ExchangeRate(Currency.FromCode("EUR"), Currency.FromCode("USD"), 1.2591); // EUR/USD 1.2591

            [TestMethod, Description("When Converting €100,99 With EUR/USD 1.2591, Then Result Should Be $127.16")]
            public void WhenConvertingEurosToDollars_ThenConversionShouldBeCorrect()
            {
                // Convert €100,99 to $127.156509 (= €100.99 * 1.2591)
                var converted = _exchangeRate.Convert(Money.Euro(100.99M));

                converted.Currency.Should().Be(_dollar);
                converted.Amount.Should().Be(127.16M);
            }

            [TestMethod]
            public void WhenConvertingEurosToDollarsAndThenBack_ThenEndResultShouldBeTheSameAsInTheBeginning()
            {
                // Convert €100,99 to $127.156509 (= €100.99 * 1.2591)
                var converted = _exchangeRate.Convert(Money.Euro(100.99M));

                // Convert $127.16 to €100,99 (= $127.16 / 1.2591)
                var revert = _exchangeRate.Convert(converted);

                revert.Currency.Should().Be(_euro);
                revert.Amount.Should().Be(100.99M);
            }
        }

        [TestClass]
        public class GivenIWantToCreateAnExchangeRateWithCurrencies
        {
            private readonly Currency _euro = Currency.FromCode("EUR");
            private readonly Currency _dollar = Currency.FromCode("USD");

            [TestMethod]
            public void WhenRateIsDouble_ThenCreatingShouldSucceed()
            {
                var fx = new ExchangeRate(_euro, _dollar, 1.2591);

                fx.BaseCurrency.Should().Be(_euro);
                fx.QuoteCurrency.Should().Be(_dollar);
                // TODO: Can doubles be compared for equality? See https://github.com/dennisdoomen/fluentassertions/wiki
                fx.Value.Should().Be(1.2591M);
            }

            [TestMethod]
            public void WhenRateIsDecimal_ThenCreatingShouldSucceed()
            {
                var fx = new ExchangeRate(_euro, _dollar, 1.2591M);

                fx.BaseCurrency.Should().Be(_euro);
                fx.QuoteCurrency.Should().Be(_dollar);
                fx.Value.Should().Be(1.2591M);
            }

            [TestMethod]
            public void WhenRateIsFloat_ThenCreatingShouldSucceed()
            {
                var fx = new ExchangeRate(_euro, _dollar, 1.2591F);

                fx.BaseCurrency.Should().Be(_euro);
                fx.QuoteCurrency.Should().Be(_dollar);
                fx.Value.Should().Be(1.2591M);
            }

            [TestMethod]
            public void WhenBaseAndQuoteCurrencyAreTheSame_ThenCreatingShouldThrow()
            {
                // Arrange, Act
                Action action = () => new ExchangeRate(_euro, _euro, 1.2591F);

                // Assert
                action.ShouldThrow<ArgumentException>();
            }
        }

        [TestClass]
        public class GivenIWantToCreateAnExchangeRateWithCurrenciesAsStrings
        {
            private readonly string _euroAsString = "EUR";
            private readonly string _dollarAsString = "USD";
            private readonly Currency _euro = Currency.FromCode("EUR");
            private readonly Currency _dollar = Currency.FromCode("USD");

            [TestMethod]
            public void WhenRateIsDouble_ThenCreatingShouldSucceed()
            {
                var fx = new ExchangeRate(_euroAsString, _dollarAsString, 1.2591);

                fx.BaseCurrency.Should().Be(_euro);
                fx.QuoteCurrency.Should().Be(_dollar);
                // TODO: Can doubles be compared for equality? See https://github.com/dennisdoomen/fluentassertions/wiki
                fx.Value.Should().Be(1.2591M);
            }

            [TestMethod]
            public void WhenRateIsDecimal_ThenCreatingShouldSucceed()
            {
                var fx = new ExchangeRate(_euroAsString, _dollarAsString, 1.2591M);

                fx.BaseCurrency.Should().Be(_euro);
                fx.QuoteCurrency.Should().Be(_dollar);
                fx.Value.Should().Be(1.2591M);
            }

            [TestMethod]
            public void WhenRateIsFloat_ThenCreatingShouldSucceed()
            {
                var fx = new ExchangeRate(_euroAsString, _dollarAsString, 1.2591F);

                fx.BaseCurrency.Should().Be(_euro);
                fx.QuoteCurrency.Should().Be(_dollar);
                fx.Value.Should().Be(1.2591M);
            }

            [TestMethod]
            public void WhenBaseAndQuoteCurrencyAreTheSame_ThenCreatingShouldThrow()
            {
                // Arrange, Act
                Action action = () => new ExchangeRate(_euroAsString, _euroAsString, 1.2591F);

                // Assert
                action.ShouldThrow<ArgumentException>();
            }
        }

        [TestClass]
        public class GivenIWantToConvertExchangeRateToString
        {
            ExchangeRate fx = new ExchangeRate(Currency.FromCode("EUR"), Currency.FromCode("USD"), 1.2524);

            [TestMethod]
            public void WhenShowingExchangeRateInAmerica_ThenReturnCurrencyPairWithDot()
            {
                using (new SwitchCulture("en-US"))
                {
                    fx.ToString().Should().Be("EUR/USD 1.2524");
                }
            }

            [TestMethod]
            public void WhenShowingExchangeRateInNetherlands_ThenReturnCurrencyPairWithComma()
            {
                using (new SwitchCulture("nl-NL"))
                {
                    fx.ToString().Should().Be("EUR/USD 1,2524");
                }
            }
        }
    }
}
