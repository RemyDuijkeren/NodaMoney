using System;
using System.Globalization;
using System.Threading;
using FluentAssertions;
using NodaMoney.Tests.Helpers;
using Xunit;

namespace NodaMoney.Tests.MoneySpec
{
    [Collection(nameof(NoParallelization))]
    public class GivenIWantMoneyImplicit
    {
        private readonly decimal _decimalValue = 1234.567m;

        [Fact]
        [UseCulture("en-US")]
        public void WhenCurrentCultureIsUS_ThenCurrencyIsDollar()
        {            
            var money = new Money(_decimalValue);

            money.Currency.Should().Be(Currency.FromCode("USD"));
            money.Amount.Should().Be(1234.57m);
        }

        [Fact]
        [UseCulture("nl-NL")]
        public void WhenCurrentCultureIsNL_ThenCurrencyIsEuro()
        {
            var money = new Money(_decimalValue);

            money.Currency.Should().Be(Currency.FromCode("EUR"));
            money.Amount.Should().Be(1234.57m);
        }

        [Fact]
        [UseCulture("ja-JP")]
        public void WhenCurrentCultureIsJP_ThenCurrencyIsYen()
        {
            var money = new Money(_decimalValue);

            money.Currency.Should().Be(Currency.FromCode("JPY"));
            money.Amount.Should().Be(1235m);
        }

        [Fact]
        [UseCulture(null)]
        public void WhenCurrentCultureIsInvariant_ThenCurrencyIsDefault()
        {
            var money = new Money(_decimalValue);

            money.Currency.Should().Be(default(Currency));
            money.Amount.Should().Be(1235m);
        }
    }

    public class GivenIWantMoneyFromNumericTypeAndAnExplicitCurrencyObject
    {
        private readonly Currency _euro = Currency.FromCode("EUR");

        [Fact]
        public void WhenValueIsByte_ThenCreatingShouldSucceed()
        {
            const byte byteValue = 50;
            var money = new Money(byteValue, _euro);

            money.Currency.Should().Be(_euro);
            money.Amount.Should().Be(50m);
        }

        [Fact]
        public void WhenValueIsSbyte_ThenCreatingShouldSucceed()
        {
            const sbyte sbyteValue = 75;
            var money = new Money(sbyteValue, _euro);

            money.Currency.Should().Be(_euro);
            money.Amount.Should().Be(75m);
        }

        [Fact]
        public void WhenValueIsInt16_ThenCreatingShouldSucceed()
        {
            const short int16Value = 100;
            var money = new Money(int16Value, _euro);

            money.Currency.Should().Be(_euro);
            money.Amount.Should().Be(100m);
        }

        [Fact]
        public void WhenValueIsInt32_ThenCreatingShouldSucceed()
        {
            const int int32Value = 200;
            var money = new Money(int32Value, _euro);

            money.Currency.Should().Be(_euro);
            money.Amount.Should().Be(200m);
        }

        [Fact]
        public void WhenValueIsInt64_ThenCreatingShouldSucceed()
        {
            const long int64Value = 300;
            var money = new Money(int64Value, _euro);

            money.Currency.Should().Be(_euro);
            money.Amount.Should().Be(300m);
        }

        [Fact]
        public void WhenValueIsUint16_ThenCreatingShouldSucceed()
        {
            const ushort uInt16Value = 400;
            var money = new Money(uInt16Value, _euro);

            money.Currency.Should().Be(_euro);
            money.Amount.Should().Be(400m);
        }

        [Fact]
        public void WhenValueIsUint32_ThenCreatingShouldSucceed()
        {
            const uint uInt32Value = 500;
            var money = new Money(uInt32Value, _euro);

            money.Currency.Should().Be(_euro);
            money.Amount.Should().Be(500m);
        }

        [Fact]
        public void WhenValueIsUint64_ThenCreatingShouldSucceed()
        {
            const ulong uInt64Value = 600;
            var money = new Money(uInt64Value, _euro);

            money.Currency.Should().Be(_euro);
            money.Amount.Should().Be(600m);
        }

        [Fact]
        public void WhenValueIsSingle_ThenCreatingShouldSucceed()
        {
            const float singleValue = 700;
            var money = new Money(singleValue, _euro);

            money.Currency.Should().Be(_euro);
            money.Amount.Should().Be(700m);
        }

        [Fact]
        public void WhenValueIsDouble_ThenCreatingShouldSucceed()
        {
            const double doubleValue = 800;
            var money = new Money(doubleValue, _euro);

            money.Currency.Should().Be(_euro);
            money.Amount.Should().Be(800m);
        }

        [Theory]
        [InlineData(0.03, 0.03)]
        [InlineData(0.3333333333333333, 0.33)]
        [InlineData(251426433.75935, 251426433.76)]
        [InlineData(7922816251426433.7593543950335, 7922816251426433.76)]
        [InlineData(79228162514264337593543.950335, 79228162514264337593543.95)]
        [InlineData(0.0079228162514264337593543950335, 0.01)]
        public void WhenValueIsDecimal_ThenCreatingShouldSucceed(decimal input, decimal expected)
        {
            var money = new Money(input, _euro);

            money.Currency.Should().Be(_euro);
            money.Amount.Should().Be(expected);
        }
    }

    public class GivenIWantMoneyFromNumericTypeAndAnExplicitIsoCurrencyCode
    {
        private readonly Currency _euro = Currency.FromCode("EUR");

        [Fact]
        public void WhenValueIsByte_ThenCreatingShouldSucceed()
        {
            const byte byteValue = 50;
            var money = new Money(byteValue, "EUR");

            money.Currency.Should().Be(_euro);
            money.Amount.Should().Be(50);
        }

        [Fact]
        public void WhenValueIsSbyte_ThenCreatingShouldSucceed()
        {
            const sbyte sbyteValue = 75;
            var money = new Money(sbyteValue, "EUR");

            money.Currency.Should().Be(_euro);
            money.Amount.Should().Be(75);
        }

        [Fact]
        public void WhenValueIsInt16_ThenCreatingShouldSucceed()
        {
            const short int16Value = 100;
            var money = new Money(int16Value, "EUR");

            money.Currency.Should().Be(_euro);
            money.Amount.Should().Be(100);
        }

        [Fact]
        public void WhenValueIsInt32_ThenCreatingShouldSucceed()
        {
            const int int32Value = 200;
            var money = new Money(int32Value, "EUR");

            money.Currency.Should().Be(_euro);
            money.Amount.Should().Be(200);
        }

        [Fact]
        public void WhenValueIsInt64_ThenCreatingShouldSucceed()
        {
            const long int64Value = 300;
            var money = new Money(int64Value, "EUR");

            money.Currency.Should().Be(_euro);
            money.Amount.Should().Be(300);
        }

        [Fact]
        public void WhenValueIsUint16_ThenCreatingShouldSucceed()
        {
            const ushort uInt16Value = 400;
            var money = new Money(uInt16Value, "EUR");

            money.Currency.Should().Be(_euro);
            money.Amount.Should().Be(400);
        }

        [Fact]
        public void WhenValueIsUint32_ThenCreatingShouldSucceed()
        {
            const uint uInt32Value = 500;
            var money = new Money(uInt32Value, "EUR");

            money.Currency.Should().Be(_euro);
            money.Amount.Should().Be(500);
        }

        [Fact]
        public void WhenValueIsUint64_ThenCreatingShouldSucceed()
        {
            const ulong uInt64Value = 600;
            var money = new Money(uInt64Value, "EUR");

            money.Currency.Should().Be(_euro);
            money.Amount.Should().Be(600);
        }

        [Fact]
        public void WhenValueIsSingle_ThenCreatingShouldSucceed()
        {
            const float singleValue = 700;
            var money = new Money(singleValue, "EUR");

            money.Currency.Should().Be(_euro);
            money.Amount.Should().Be(700);
        }

        [Fact]
        public void WhenValueIsDouble_ThenCreatingShouldSucceed()
        {
            const double doubleValue = 800;
            var money = new Money(doubleValue, "EUR");

            money.Currency.Should().Be(_euro);
            money.Amount.Should().Be(800);
        }

        [Fact]
        public void WhenValueIsDecimal_ThenCreatingShouldSucceed()
        {
            const decimal decimalValue = 900;
            var money = new Money(decimalValue, "EUR");

            money.Currency.Should().Be(_euro);
            money.Amount.Should().Be(900);
        }

        [Fact]
        public void WhenUnknownIsoCurrencySymbol_ThenThrowException()
        {
            Action action = () => new Money(123.25M, "XYZ");

            action.Should().Throw<InvalidCurrencyException>();
        }
    }

    public class GivenIWantMoneyWithDifferentRounding
    {
        [Fact]
        public void WhenOnlyAmount_ThenItShouldRoundUp()
        {
            decimal amount = 0.525m;
            var defaultRounding = new Money(amount);
            var differentRounding = new Money(amount, MidpointRounding.AwayFromZero);

            defaultRounding.Amount.Should().Be(0.52m);
            differentRounding.Amount.Should().Be(0.53m);
        }

        [Fact]
        public void WhenAmountAndCode_ThenItShouldRoundUp()
        {
            decimal amount = 0.525m;
            var defaultRounding = new Money(amount, "EUR");
            var differentRounding = new Money(amount, "EUR", MidpointRounding.AwayFromZero);

            defaultRounding.Amount.Should().Be(0.52m);
            differentRounding.Amount.Should().Be(0.53m);
        }
    }

    public class GivenIWantToCreateMoneyWithDoubleValue
    {
        [Theory]
        [InlineData(0.03, 0.03)]
        [InlineData(0.3333333333333333, 0.33)]
        public void WhenValueIsDoubleAndWithCurrency_ThenMoneyShouldBeCorrect(double input, decimal expected)
        {
            var money = new Money(input, "EUR");

            money.Amount.Should().Be(expected);
        }

        [Theory]
        [InlineData(0.03, 0.03)]
        [InlineData(0.3333333333333333, 0.33)]
        public void WhenValueIsDoubleWithoutCurrency_ThenMoneyShouldBeCorrect(double input, decimal expected)
        {
            var money = new Money(input);

            money.Amount.Should().Be(expected);
        }
    }

    public class GivenIWantMoneyInMalagasyAriaryWhichHasFiveSubunits
    {
        [Theory]
        [InlineData(0.01, 0.0)]
        [InlineData(0.09, 0.0)]
        [InlineData(0.10, 0.0)]
        [InlineData(0.15, 0.2)]
        [InlineData(0.22, 0.2)]
        [InlineData(0.29, 0.2)]
        [InlineData(0.30, 0.4)]
        [InlineData(0.33, 0.4)]
        [InlineData(0.38, 0.4)]
        [InlineData(0.40, 0.4)]
        [InlineData(0.41, 0.4)]
        [InlineData(0.45, 0.4)]
        [InlineData(0.46, 0.4)]
        [InlineData(0.50, 0.4)]
        [InlineData(0.54, 0.6)]
        [InlineData(0.57, 0.6)]
        [InlineData(0.60, 0.6)]
        [InlineData(0.68, 0.6)]
        [InlineData(0.70, 0.8)]
        [InlineData(0.74, 0.8)]
        [InlineData(0.77, 0.8)]
        [InlineData(0.80, 0.8)]
        [InlineData(0.83, 0.8)]
        [InlineData(0.85, 0.8)]
        [InlineData(0.86, 0.8)]
        [InlineData(0.90, 0.8)]
        [InlineData(0.91, 1.0)]
        [InlineData(0.95, 1.0)]
        [InlineData(0.99, 1.0)]
        public void WhenOnlyAmount_ThenItShouldRoundUp(decimal input, decimal expected)
        {
            // 1 MGA = 5 iraimbilanja
            var money = new Money(input, "MGA");

            money.Amount.Should().Be(expected);
        }
    }

    public class GivenIWantToDeconstructMoney
    {
        [Fact]
        public void WhenDeConstructing_ThenShouldSucceed()
        {
            var money = new Money(10m, "EUR");

            var (amount, currency) = money;

            amount.Should().Be(10m);
            currency.Should().Be(Currency.FromCode("EUR"));
        }
    }

    public class GivenIWantDefaultMoney
    {
        [Fact]
        public void WhenCreatingDefault_ThenItShouldBeNoCurrency()
        {
            Money money = default;

            money.Should().NotBeNull();
            money.Currency.Should().Be(default(Currency));
            money.Amount.Should().Be(default(decimal));
        }
    }
}
