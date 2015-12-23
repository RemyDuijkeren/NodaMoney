using FluentAssertions;
using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NodaMoney.UnitTests.Helpers;
using System;

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

        [TestClass]
        public class GivenIWantToCreateAnExchangeRateWithDateTime
        {
            private readonly Currency _euro = Currency.FromCode("EUR");
            private readonly Currency _dollar = Currency.FromCode("USD");

            [TestMethod]
            public void WhenDateTimeIsUtcTime_ThenQuoteTimeShouldBeEqual()
            {
                DateTime time = new DateTime(2015, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                var fx = new ExchangeRate(_euro, _dollar, 1.2591F, time);

                fx.QuoteTime.Should().Equals(time);
            }

            [TestMethod]
            public void WhenDateTimeIsLocalTime_ThenQuoteTimeShouldBeEqualToUtcTime()
            {
                DateTime time = new DateTime(2015, 1, 1, 0, 0, 0, DateTimeKind.Local);

                var fx = new ExchangeRate(_euro, _dollar, 1.2591F, time);

                fx.QuoteTime.Should().Equals(time.ToUniversalTime());
            }

            [TestMethod]
            public void WhenDateTimeIsNull_ThenQuoteTimeShouldBeUtcNow()
            {
                using (ShimsContext.Create())
                {
                    DateTime utcNow = new DateTime(2015, 1, 1, 0, 0, 0, DateTimeKind.Utc);

                    System.Fakes.ShimDateTime.UtcNowGet = () => { return utcNow; };

                    var fx = new ExchangeRate(_euro, _dollar, 1.2591F);

                    fx.QuoteTime.Should().Equals(utcNow);
                }
            }

            [TestMethod]
            public void WhenDateTimeIsSpecified_ThenAvailablePropertyShouldBeTrue()
            {
                using (ShimsContext.Create())
                {
                    DateTime utcNow = new DateTime(2015, 1, 1, 0, 0, 0, DateTimeKind.Utc);

                    System.Fakes.ShimDateTime.UtcNowGet = () => { return utcNow; };

                    var fx = new ExchangeRate(_euro, _dollar, 1.2591F);

                    fx.QuoteTime.Should().Equals(utcNow);
                    fx.IsAvailable(new DateTime(2014, 12, 31, 0, 0, 0, DateTimeKind.Utc)).Should().BeFalse();
                    fx.IsAvailable(new DateTime(2015, 1, 1, 0, 0, 0, DateTimeKind.Utc)).Should().BeTrue();
                    fx.IsAvailable(new DateTime(2015, 1, 2, 0, 0, 0, DateTimeKind.Utc)).Should().BeFalse();

                    fx.IsAvailable(new DateTime(2014, 12, 31, 0, 0, 0, DateTimeKind.Local)).Should().BeFalse();
                    fx.IsAvailable(new DateTime(2015, 1, 1, 0, 0, 0, DateTimeKind.Local)).Should().BeFalse();
                    fx.IsAvailable(new DateTime(2015, 1, 2, 0, 0, 0, DateTimeKind.Local)).Should().BeTrue();
                }
            }
        }

        [TestClass]
        public class GivenIWantToConvertMoneyWithDateTime
        {
            private readonly Currency _euro = Currency.FromCode("EUR");
            private readonly Currency _dollar = Currency.FromCode("USD");
            private readonly DateTime utcNow = new DateTime(2015, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            [TestMethod]
            public void WhenExchangeRateIsCreated_ThenSingleQuoteShouldExist()
            {
                using (ShimsContext.Create())
                {
                    System.Fakes.ShimDateTime.UtcNowGet = () => { return utcNow; };

                    var fx = new ExchangeRate(_euro, _dollar, 1.2591F);

                    fx.QuoteTime.Should().Equals(utcNow);
                    var quotes = fx.GetQuotes();
                    quotes.Count.Should().Be(1);
                    quotes.ContainsKey(utcNow).Should().BeTrue();
                    quotes[utcNow].Should().Be(1.2591M);
                }
            }

            [TestMethod]
            public void WhenDayQuoteIsAvailable_ThenTryGetDayQuoteShouldSucceed()
            {
                using (ShimsContext.Create())
                {
                    System.Fakes.ShimDateTime.UtcNowGet = () => { return utcNow; };

                    var fx = new ExchangeRate(_euro, _dollar, 1.2591F);
                    decimal quote;

                    fx.QuoteTime.Should().Equals(utcNow);
                    fx.TryGetDayQuote(new DateTime(2015, 1, 1, 0, 0, 0, DateTimeKind.Utc), out quote).Should().BeTrue();
                    quote.Should().Be(1.2591M);
                }
            }

            [TestMethod]
            public void WhenDayQuoteIsNotAvailable_ThenTryGetDayQuoteShouldFail()
            {
                using (ShimsContext.Create())
                {
                    System.Fakes.ShimDateTime.UtcNowGet = () => { return utcNow; };

                    var fx = new ExchangeRate(_euro, _dollar, 1.2591F);
                    decimal quote;

                    fx.QuoteTime.Should().Equals(utcNow);
                    fx.TryGetDayQuote(new DateTime(2015, 1, 2, 0, 0, 0, DateTimeKind.Utc), out quote).Should().BeFalse();
                    quote.Should().Be(0M);
                }
            }

            [TestMethod]
            public void WhenDayQuoteIsAvailable_ThenGetDayQuoteShouldSucceed()
            {
                using (ShimsContext.Create())
                {
                    System.Fakes.ShimDateTime.UtcNowGet = () => { return utcNow; };

                    var fx = new ExchangeRate(_euro, _dollar, 1.2591F);

                    fx.QuoteTime.Should().Equals(utcNow);
                    fx.GetDayQuote(new DateTime(2015, 1, 1, 0, 0, 0, DateTimeKind.Utc)).Should().Be(1.2591M);                                        
                }
            }

            [TestMethod]
            public void WhenDayQuoteIsNotAvailable_ThenGetDayQuoteShouldThrow()
            {
                using (ShimsContext.Create())
                {
                    System.Fakes.ShimDateTime.UtcNowGet = () => { return utcNow; };

                    var fx = new ExchangeRate(_euro, _dollar, 1.2591F);

                    fx.QuoteTime.Should().Equals(utcNow);

                    Action action = () => fx.GetDayQuote(new DateTime(2015, 1, 2, 0, 0, 0, DateTimeKind.Utc));

                    action.ShouldThrow<NoExchangeRateQuoteFoundException>();
                }
            }

            [TestMethod]
            public void WhenDayQuoteIsAvailable_ThenConversionShouldBeCorrect()
            {
                using (ShimsContext.Create())
                {
                    System.Fakes.ShimDateTime.UtcNowGet = () => { return utcNow; };

                    var fx = new ExchangeRate(_euro, _dollar, 1.2591F);

                    var converted = fx.ConvertWithAvailableQuotes(
                                        Money.Euro(100.99M),
                                        new DateTime(2015, 1, 1, 0, 0, 0, DateTimeKind.Utc));

                    converted.Currency.Should().Be(_dollar);
                    converted.Amount.Should().Be(127.16M);
                }
            }
            [TestMethod]
            public void WhenDayQuoteIsNotAvailable_ThenConversionShouldThrow()
            {
                using (ShimsContext.Create())
                {
                    System.Fakes.ShimDateTime.UtcNowGet = () => { return utcNow; };

                    var fx = new ExchangeRate(_euro, _dollar, 1.2591F);

                    Action action = () => fx.ConvertWithAvailableQuotes(
                                            Money.Euro(100.99M),
                                            new DateTime(2015, 1, 2, 0, 0, 0, DateTimeKind.Utc));

                    action.ShouldThrow<NoExchangeRateQuoteFoundException>();
                }
            }
        }
    }
}