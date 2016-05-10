using System;
using FluentAssertions;
using Xunit;
using NodaMoney.Tests.Helpers;

namespace NodaMoney.Tests
{
    public class ExchangeRateTests
    {
        public class GivenIWantToParseACurrencyPair
        {
            [Fact]
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

            [Fact]
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

            [Fact]
            public void WhenCurrencyPairIsNotANumber_ThenParsingShouldThrow()
            {
                Action action = () => ExchangeRate.Parse("EUR/USD 1,ABC");

                action.ShouldThrow<FormatException>();
            }

            [Fact]
            public void WhenCurrencyPairHasSameCurrencies_ThenParsingShouldThrow()
            {
                using (new SwitchCulture("en-US"))
                {
                    Action action = () => ExchangeRate.Parse("EUR/EUR 1.2591");

                    action.ShouldThrow<FormatException>();
                }
            }
        }
        
        public class GivenIWantToConvertMoney
        {
            private readonly Currency _euro = Currency.FromCode("EUR");
            private readonly Currency _dollar = Currency.FromCode("USD");
            private ExchangeRate _exchangeRate = new ExchangeRate(Currency.FromCode("EUR"), Currency.FromCode("USD"), 1.2591); // EUR/USD 1.2591

            [Fact]
            public void WhenConvertingEurosToDollars_ThenConversionShouldBeCorrect()
            {
                // When Converting €100,99 With EUR/USD 1.2591, Then Result Should Be $127.16

                // Convert €100,99 to $127.156509 (= €100.99 * 1.2591)
                var converted = _exchangeRate.Convert(Money.Euro(100.99M));

                converted.Currency.Should().Be(_dollar);
                converted.Amount.Should().Be(127.16M);
            }

            [Fact]
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

        public class GivenIWantToCreateAnExchangeRateWithCurrencies
        {
            private readonly Currency _euro = Currency.FromCode("EUR");
            private readonly Currency _dollar = Currency.FromCode("USD");

            [Fact]
            public void WhenRateIsDouble_ThenCreatingShouldSucceed()
            {
                var fx = new ExchangeRate(_euro, _dollar, 1.2591);

                fx.BaseCurrency.Should().Be(_euro);
                fx.QuoteCurrency.Should().Be(_dollar);
                // TODO: Can doubles be compared for equality? See https://github.com/dennisdoomen/fluentassertions/wiki
                fx.Value.Should().Be(1.2591M);
            }

            [Fact]
            public void WhenRateIsDecimal_ThenCreatingShouldSucceed()
            {
                var fx = new ExchangeRate(_euro, _dollar, 1.2591M);

                fx.BaseCurrency.Should().Be(_euro);
                fx.QuoteCurrency.Should().Be(_dollar);
                fx.Value.Should().Be(1.2591M);
            }

            [Fact]
            public void WhenRateIsFloat_ThenCreatingShouldSucceed()
            {
                var fx = new ExchangeRate(_euro, _dollar, 1.2591F);

                fx.BaseCurrency.Should().Be(_euro);
                fx.QuoteCurrency.Should().Be(_dollar);
                fx.Value.Should().Be(1.2591M);
            }

            [Fact]
            public void WhenBaseAndQuoteCurrencyAreTheSame_ThenCreatingShouldThrow()
            {
                // Arrange, Act
                Action action = () => new ExchangeRate(_euro, _euro, 1.2591F);

                // Assert
                action.ShouldThrow<ArgumentException>();
            }
        }
        
        public class GivenIWantToCreateAnExchangeRateWithCurrenciesAsStrings
        {
            private readonly string _euroAsString = "EUR";
            private readonly string _dollarAsString = "USD";
            private readonly Currency _euro = Currency.FromCode("EUR");
            private readonly Currency _dollar = Currency.FromCode("USD");

            [Fact]
            public void WhenRateIsDouble_ThenCreatingShouldSucceed()
            {
                var fx = new ExchangeRate(_euroAsString, _dollarAsString, 1.2591);

                fx.BaseCurrency.Should().Be(_euro);
                fx.QuoteCurrency.Should().Be(_dollar);
                // TODO: Can doubles be compared for equality? See https://github.com/dennisdoomen/fluentassertions/wiki
                fx.Value.Should().Be(1.2591M);
            }

            [Fact]
            public void WhenRateIsDecimal_ThenCreatingShouldSucceed()
            {
                var fx = new ExchangeRate(_euroAsString, _dollarAsString, 1.2591M);

                fx.BaseCurrency.Should().Be(_euro);
                fx.QuoteCurrency.Should().Be(_dollar);
                fx.Value.Should().Be(1.2591M);
            }

            [Fact]
            public void WhenRateIsFloat_ThenCreatingShouldSucceed()
            {
                var fx = new ExchangeRate(_euroAsString, _dollarAsString, 1.2591F);

                fx.BaseCurrency.Should().Be(_euro);
                fx.QuoteCurrency.Should().Be(_dollar);
                fx.Value.Should().Be(1.2591M);
            }

            [Fact]
            public void WhenBaseAndQuoteCurrencyAreTheSame_ThenCreatingShouldThrow()
            {
                // Arrange, Act
                Action action = () => new ExchangeRate(_euroAsString, _euroAsString, 1.2591F);

                // Assert
                action.ShouldThrow<ArgumentException>();
            }
        }
        
        public class GivenIWantToConvertExchangeRateToString
        {
            ExchangeRate fx = new ExchangeRate(Currency.FromCode("EUR"), Currency.FromCode("USD"), 1.2524);

            [Fact]
            public void WhenShowingExchangeRateInAmerica_ThenReturnCurrencyPairWithDot()
            {
                using (new SwitchCulture("en-US"))
                {
                    fx.ToString().Should().Be("EUR/USD 1.2524");
                }
            }

            [Fact]
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
