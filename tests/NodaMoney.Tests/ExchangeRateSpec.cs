using System;
using System.Collections.Generic;

using FluentAssertions;
using Xunit;
using NodaMoney.Tests.Helpers;

namespace NodaMoney.Tests.ExchangeRateSpec
{
    public class GivenIWantToConvertMoney
    {
        private readonly Currency _euro = Currency.FromCode("EUR");

        private readonly Currency _dollar = Currency.FromCode("USD");

        private ExchangeRate _exchangeRate = new ExchangeRate(Currency.FromCode("EUR"), Currency.FromCode("USD"), 1.2591);
                             // EUR/USD 1.2591

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

        [Fact]
        public void WhenConvertingWithExchangeRateWithDifferentCurrencies_ThenThrowException()
        {
            // Arrange, Act
            Action action = () => _exchangeRate.Convert(Money.Yen(324));

            // Assert
            action.Should().Throw<ArgumentException>().WithMessage("Money should have the same currency as the base currency or the quote currency!*");
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
        public void WhenBaseAndQuoteCurrencyAreTheSame_ThenThrowException()
        {
            // Arrange, Act
            Action action = () => new ExchangeRate(_euro, _euro, 1.2591F);

            // Assert
            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void WhenRateIsLessThenZero_ThenThrowException()
        {
            // Arrange, Act
            Action action = () => new ExchangeRate(_euro, _euro, -1.2F);

            // Assert
            action.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void WhenBaseCurrencyIsNull_ThenThrowException()
        {
            // Arrange, Act
            Action action = () => new ExchangeRate(default(Currency), _dollar, 1.2591);

            // Assert
            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void WhenQuoteCurrencyIsNull_ThenThrowException()
        {
            // Arrange, Act
            Action action = () => new ExchangeRate(_euro, default(Currency), 1.2591);

            // Assert
            action.Should().Throw<ArgumentNullException>();
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
            action.Should().Throw<ArgumentException>();
        }
    }

    public class GivenIWantToConvertExchangeRateToString
    {
        ExchangeRate fx = new ExchangeRate(Currency.FromCode("EUR"), Currency.FromCode("USD"), 1.2524);

        [Fact, UseCulture("en-US")]
        public void WhenShowingExchangeRateInAmerica_ThenReturnCurrencyPairWithDot()
        {
            fx.ToString().Should().Be("EUR/USD 1.2524");
        }

        [Fact, UseCulture("nl-NL")]
        public void WhenShowingExchangeRateInNetherlands_ThenReturnCurrencyPairWithComma()
        {
            fx.ToString().Should().Be("EUR/USD 1,2524");
        }
    }

    public class GivenIWantToParseACurrencyPair
    {
        [Fact, UseCulture("en-US")]
        public void WhenCurrencyPairInUsCulture_ThenParsingShouldSucceed()
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

        [Fact, UseCulture("nl-NL")]
        public void WhenCurrencyPairInNlCulture_ThenParsingShouldSucceed()
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

        [Fact]
        public void WhenCurrencyPairIsNotANumber_ThenThrowException()
        {
            Action action = () => ExchangeRate.Parse("EUR/USD 1,ABC");

            action.Should().Throw<FormatException>();
        }

        [Fact, UseCulture("en-US")]
        public void WhenCurrencyPairHasSameCurrencies_ThenThrowException()
        {
            Action action = () => ExchangeRate.Parse("EUR/EUR 1.2591");

            action.Should().Throw<FormatException>();
        }

        [Fact]
        public void WhenCurrencyPairIsNull_ThenThrowException()
        {
            Action action = () => ExchangeRate.Parse(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void WhenCurrencyPairIsEmpty_ThenThrowException()
        {
            Action action = () => ExchangeRate.Parse("");

            action.Should().Throw<FormatException>();
        }
    }

    public class GivenIWantToTryParseACurrencyPair
    {
        [Fact, UseCulture("en-US")]
        public void WhenCurrencyPairInUsCulture_ThenParsingShouldSucceed()
        {
            ExchangeRate fx1;
            var succeeded1 = ExchangeRate.TryParse("EUR/USD 1.2591", out fx1);

            succeeded1.Should().BeTrue();
            fx1.BaseCurrency.Code.Should().Be("EUR");
            fx1.QuoteCurrency.Code.Should().Be("USD");
            fx1.Value.Should().Be(1.2591M);

            ExchangeRate fx2;
            var succeeded2 = ExchangeRate.TryParse("EUR/USD1.2591", out fx2);

            succeeded2.Should().BeTrue();
            fx2.BaseCurrency.Code.Should().Be("EUR");
            fx2.QuoteCurrency.Code.Should().Be("USD");
            fx2.Value.Should().Be(1.2591M);
        }

        [Fact, UseCulture("nl-NL")]
        public void WhenCurrencyPairInNlCulture_ThenParsingShouldSucceed()
        {
            ExchangeRate fx1;
            var succeeded1 = ExchangeRate.TryParse("EUR/USD 1,2591", out fx1);

            succeeded1.Should().BeTrue();
            fx1.BaseCurrency.Code.Should().Be("EUR");
            fx1.QuoteCurrency.Code.Should().Be("USD");
            fx1.Value.Should().Be(1.2591M);

            ExchangeRate fx2;
            var succeeded2 = ExchangeRate.TryParse("EUR/USD1,2591", out fx2);

            succeeded2.Should().BeTrue();
            fx2.BaseCurrency.Code.Should().Be("EUR");
            fx2.QuoteCurrency.Code.Should().Be("USD");
            fx2.Value.Should().Be(1.2591M);
        }

        [Fact]
        public void WhenCurrencyPairIsNotANumber_ThenParsingFails()
        {
            ExchangeRate fx;
            var succeeded = ExchangeRate.TryParse("EUR/USD 1,ABC", out fx);

            succeeded.Should().BeFalse();
            fx.BaseCurrency.Code.Should().Be("XXX");
            fx.QuoteCurrency.Code.Should().Be("XXX");
            fx.Value.Should().Be(0M);
        }

        [Fact, UseCulture("en-US")]
        public void WhenCurrencyPairHasSameCurrencies_ThenParsingFails()
        {
            ExchangeRate fx;
            var succeeded = ExchangeRate.TryParse("EUR/EUR 1.2591", out fx);

            succeeded.Should().BeFalse();
            fx.BaseCurrency.Code.Should().Be("XXX");
            fx.QuoteCurrency.Code.Should().Be("XXX");
            fx.Value.Should().Be(0M);
        }

        [Fact]
        public void WhenCurrencyPairIsNull_ThenParsingFails()
        {
            ExchangeRate fx;
            var succeeded = ExchangeRate.TryParse(null, out fx);

            succeeded.Should().BeFalse();
            fx.BaseCurrency.Code.Should().Be("XXX");
            fx.QuoteCurrency.Code.Should().Be("XXX");
            fx.Value.Should().Be(0M);
        }

        [Fact]
        public void WhenCurrencyPairIsEmpty_ThenParsingFails()
        {
            ExchangeRate fx;
            var succeeded = ExchangeRate.TryParse("", out fx);

            succeeded.Should().BeFalse();
            fx.BaseCurrency.Code.Should().Be("XXX");
            fx.QuoteCurrency.Code.Should().Be("XXX");
            fx.Value.Should().Be(0M);
        }
    }

    public class GivenIWantToCompareExchangeRates
    {
        public static IEnumerable<object[]> TestData => new[]
        {
            new object[] { new ExchangeRate("EUR", "USD", 1.2591), new ExchangeRate("EUR", "USD", 1.2591), true },
            new object[] { new ExchangeRate("EUR", "USD", 0.0), new ExchangeRate("EUR", "USD", 0.0), true },
            new object[] { new ExchangeRate("EUR", "USD", 1.2591), new ExchangeRate("EUR", "USD", 1.600), false },
            new object[] { new ExchangeRate("EUR", "USD", 1.2591), new ExchangeRate("EUR", "AFN", 1.2591), false },
            new object[] { new ExchangeRate("AFN", "USD", 1.2591), new ExchangeRate("EUR", "USD", 1.2591), false }
        };

        [Theory][MemberData("TestData")]
        public void WhenTheAreEquel_ThenComparingShouldBeTrueOtherwiseFalse(ExchangeRate fx1, ExchangeRate fx2, bool areEqual)
        {
            if (areEqual)
                fx1.Should().Be(fx2);
            else
                fx1.Should().NotBe(fx2);

            if (areEqual)
                fx1.GetHashCode().Should().Be(fx2.GetHashCode()); //using GetHashCode()
            else
                fx1.GetHashCode().Should().NotBe(fx2.GetHashCode()); //using GetHashCode()

            fx1.Equals(fx2).Should().Be(areEqual); //using Equal()
            ExchangeRate.Equals(fx1, fx2).Should().Be(areEqual); //using static Equals()            
            (fx1 == fx2).Should().Be(areEqual); //using Euality operators
            (fx1 != fx2).Should().Be(!areEqual); //using Euality operators
        }
    }

    public class GivenIWantToDeconstructExchangeRate
    {
        [Fact]
        public void WhenDeconstructing_ThenShouldSucceed()
        {
            var fx = new ExchangeRate(Currency.FromCode("EUR"), Currency.FromCode("USD"), 1.2591m);

            var (baseCurrency, quoteCurrency, rate) = fx;

            rate.Should().Be(1.2591m);
            baseCurrency.Should().Be(Currency.FromCode("EUR"));
            quoteCurrency.Should().Be(Currency.FromCode("USD"));
        }
    }
}
