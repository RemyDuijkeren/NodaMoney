using FluentAssertions;
using Xunit;

namespace NodaMoney.Tests
{
    public class MoneyUnaryOperatoresTests
    {
        public class GivenIWantToIncrementMoney
        {
            private Money _yens = new Money(765m, Currency.FromCode("JPY"));
            private Money _euros = new Money(765.43m, Currency.FromCode("EUR"));
            private Money _dollars = new Money(765.43m, Currency.FromCode("USD"));
            private Money _dinars = new Money(765.432m, Currency.FromCode("BHD"));

            [Fact]
            public void WhenIncrementing_ThenAmountShouldIncrementWithMinorUnit()
            {
                var yens = ++_yens;
                var euros = ++_euros;
                var dollars = ++_dollars;
                var dinars = ++_dinars;

                yens.Amount.Should().Be(766m);
                yens.Currency.Should().Be(_yens.Currency);

                euros.Amount.Should().Be(765.44m);
                euros.Currency.Should().Be(_euros.Currency);

                dollars.Amount.Should().Be(765.44m);
                dollars.Currency.Should().Be(_dollars.Currency);

                dinars.Amount.Should().Be(765.433m);
                dinars.Currency.Should().Be(_dinars.Currency);
            }
        }
        
        public class GivenIWantToDecrementMoney
        {
            private Money _yens = new Money(765m, Currency.FromCode("JPY"));
            private Money _euros = new Money(765.43m, Currency.FromCode("EUR"));
            private Money _dollars = new Money(765.43m, Currency.FromCode("USD"));
            private Money _dinars = new Money(765.432m, Currency.FromCode("BHD"));

            [Fact]
            public void WhenDecrementing_ThenAmountShouldDecrementWithMinorUnit()
            {
                var yens = --_yens;
                var euros = --_euros;
                var dollars = --_dollars;
                var dinars = --_dinars;

                yens.Amount.Should().Be(764m);
                yens.Currency.Should().Be(_yens.Currency);

                euros.Amount.Should().Be(765.42m);
                euros.Currency.Should().Be(_euros.Currency);

                dollars.Amount.Should().Be(765.42m);
                dollars.Currency.Should().Be(_dollars.Currency);

                dinars.Amount.Should().Be(765.431m);
                dinars.Currency.Should().Be(_dinars.Currency);
            }
        }

        public class GivenIWantToAddAndSubtractMoneyUnary
        {
            private readonly Money _tenEuroPlus = new Money(10.00m, "EUR");
            private readonly Money _tenEuroMin = new Money(-10.00m, "EUR");

            [Fact]
            public void WhenUsingUnaryPlusOperator_ThenThisSucceed()
            {
                var r1 = +_tenEuroPlus;
                var r2 = +_tenEuroMin;

                r1.Amount.Should().Be(10.00m);
                r1.Currency.Code.Should().Be("EUR");
                r2.Amount.Should().Be(-10.00m);
                r2.Currency.Code.Should().Be("EUR");
            }

            [Fact]
            public void WhenUsingUnaryMinOperator_ThenThisSucceed()
            {
                var r1 = -_tenEuroPlus;
                var r2 = -_tenEuroMin;

                r1.Amount.Should().Be(-10.00m);
                r1.Currency.Code.Should().Be("EUR");
                r2.Amount.Should().Be(10.00m);
                r2.Currency.Code.Should().Be("EUR");
            }

            [Fact]
            public void WhenUsingUnaryMinOperatorWithDefault_ThenThisSucceed()
            {
                (-new Money(0, "USD")).Should().Be(default(Money));
                (-new Money(0, "USD")).Currency.Code.Should().Be("USD");

                (+default(Money)).Should().Be(-default(Money)).And.Be(default(Money));
            }

            [Fact]
            public void WhenUsingUnaryPlusOperatorWithDefault_ThenThisSucceed()
            {
                (+new Money(0, "USD")).Should().Be(default(Money));
                (+new Money(0, "USD")).Currency.Code.Should().Be("USD");
            }
        }
    }
}
