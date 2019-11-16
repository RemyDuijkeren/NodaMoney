using System;
using FluentAssertions;
using Xunit;

namespace NodaMoney.Tests.MoneyComparableSpec
{
    public class GivenIWantToCompareMoney
    {
        private Money _default = default(Money);
        private Money _zeroEuro = new Money(0m, "EUR");
        private Money _tenEuro1 = new Money(10.00m, "EUR");
        private Money _tenEuro2 = new Money(10.00m, "EUR");
        private Money _twentyEuro = new Money(20.00m, "EUR");
        private Money _tenDollar = new Money(10.00m, "USD");
        private Money _twentyDollar = new Money(20.00m, "USD");

        [Fact]
        public void WhenCurrencyAndValueAreEqual_ThenMoneyShouldBeEqual()
        {
            _tenEuro1.Should().Be(_tenEuro2);
            _tenEuro1.Equals(_tenEuro2).Should().BeTrue(); //using Equal()
            Money.Equals(_tenEuro1, _tenEuro2).Should().BeTrue(); //using static Equals()            
            (_tenEuro1 == _tenEuro2).Should().BeTrue(); //using Euality operators
            (_tenEuro1 != _tenEuro2).Should().BeFalse(); //using Euality operators
            _tenEuro1.GetHashCode().Should().Be(_tenEuro2.GetHashCode()); //using GetHashCode()
        }

        [Fact]
        public void WhenValueIsDifferent_ThenMoneyShouldNotBeEqual()
        {
            _tenEuro1.Should().NotBe(_twentyEuro);
            _tenEuro1.Equals(_twentyEuro).Should().BeFalse(); //using Equal()
            Money.Equals(_tenEuro1, _twentyEuro).Should().BeFalse(); //using static Equals()
            (_tenEuro1 == _twentyEuro).Should().BeFalse(); //using Euality operators
            (_tenEuro1 != _twentyEuro).Should().BeTrue(); //using Euality operators
            _tenEuro1.GetHashCode().Should().NotBe(_twentyEuro.GetHashCode()); //using GetHashCode()
        }

        [Fact]
        public void WhenCurrencyIsDifferent_ThenMoneyShouldNotBeEqual()
        {
            //_tenEuro1.Should().NotBe(_tenDollar);
            _tenEuro1.Equals(_tenDollar).Should().BeFalse(); //using Equal()
            Money.Equals(_tenEuro1, _tenDollar).Should().BeFalse(); //using static Equals()
            (_tenEuro1 == _tenDollar).Should().BeFalse(); //using Euality operators
            (_tenEuro1 != _tenDollar).Should().BeTrue(); //using Euality operators
            _tenEuro1.GetHashCode().Should().NotBe(_tenDollar.GetHashCode()); //using GetHashCode()
        }

        [Fact]
        public void WhenCurrencyAndValueIsDifferent_ThenMoneyShouldNotBeEqual()
        {
            //_tenEuro1.Should().NotBe(_twentyDollar);
            _tenEuro1.Equals(_twentyDollar).Should().BeFalse(); //using Equal()
            Money.Equals(_tenEuro1, _twentyDollar).Should().BeFalse(); //using static Equals()
            (_tenEuro1 == _twentyDollar).Should().BeFalse(); //using Euality operators
            (_tenEuro1 != _twentyDollar).Should().BeTrue(); //using Euality operators
            _tenEuro1.GetHashCode().Should().NotBe(_twentyDollar.GetHashCode()); //using GetHashCode()

            (_tenEuro1 == default).Should().BeFalse();
            (_tenEuro1 != default).Should().BeTrue();
            (_tenEuro1.Equals(default)).Should().BeFalse();
            (_zeroEuro.Equals(default)).Should().BeTrue();
            (_tenEuro1.CompareTo(default)).Should().NotBe(0);
        }

        [Fact]
        public void WhenComparingWithNull_ThenMoneyShouldNotBeEqual()
        {
            (_default == null).Should().BeFalse();
            _default.CompareTo(null).Should().NotBe(0);
        }

        [Fact]
        public void When0IsComparedWith0_ThenMoneyShouldBeEqual()
        {
            (_default == 0).Should().BeTrue();
            _default.CompareTo(0).Should().Be(0);
            Money.Compare(_default, 0).Should().Be(0);
        }

        [Fact]
        public void WhenComparingWith0_ThenMoneyShouldNotBeEqual()
        {
            (_tenEuro1 == 0).Should().BeFalse();
            _tenEuro1.CompareTo(0).Should().Be(1);
            Money.Compare(_tenEuro1, 0).Should().Be(1);
        }

        [Fact]
        public void WhenComparingWithDifferentObject_ThenMoneyShouldNotBeEqual()
        {
            // Comparing Currency to a different object should fail!
            _tenEuro1.Equals(new object()).Should().BeFalse();
        }

        [Fact]
        public void WhenCurrencyAndValueAreEqual_ThenComparingShouldBeEqual()
        {
            _tenEuro1.CompareTo(_tenEuro2).Should().Be(0); //using CompareTo()
            _tenEuro1.CompareTo((object)_tenEuro2).Should().Be(0); //using CompareTo()
            Money.Compare(_tenEuro1, _tenEuro2).Should().Be(0); //using static Compare

            //using Compareble operators
            (_tenEuro1 <= _tenEuro2).Should().BeTrue();
            (_tenEuro1 >= _tenEuro2).Should().BeTrue();
            (_tenEuro1 > _tenEuro2).Should().BeFalse();
            (_tenEuro1 < _tenEuro2).Should().BeFalse();
        }

        [Fact]
        public void WhenValueIsDifferent_ThenComparingShouldNotBeEqual()
        {
            //using CompareTo()
            _tenEuro1.CompareTo(_twentyEuro).Should().Be(-1);
            _twentyEuro.CompareTo(_tenEuro1).Should().Be(1);
            _twentyEuro.CompareTo((object)_tenEuro1).Should().Be(1);

            //using static Compare
            Money.Compare(_tenEuro1, _twentyEuro).Should().Be(-1);
            Money.Compare(_twentyEuro, _tenEuro1).Should().Be(1);

            //using Compareble operators
            (_tenEuro1 < _twentyEuro).Should().BeTrue();
            (_tenEuro1 > _twentyEuro).Should().BeFalse();
            (_tenEuro1 <= _twentyEuro).Should().BeTrue();
            (_tenEuro1 >= _twentyEuro).Should().BeFalse();
        }

        [Fact]
        public void WhenCurrencyIsDifferent_ThenComparingShouldFail()
        {
            Action action = () => _tenEuro1.CompareTo(_tenDollar);

            action.Should().Throw<InvalidCurrencyException>().WithMessage("The requested operation expected the currency*");
        }

        [Fact]
        public void WhenComparingToString_ThenComparingShouldFail()
        {
            Action action = () => _tenEuro1.CompareTo("10.00");

            action.Should().Throw<ArgumentException>().WithMessage("obj is not the same type as this instance*");
        }

        [Fact]
        public void WhenComparingWith0_ThenComparingShouldNotBeEqual()
        {
            _tenEuro1.CompareTo(_default).Should().NotBe(0); //using CompareTo()
            _tenEuro1.CompareTo((object)_default).Should().NotBe(0); //using CompareTo()
            Money.Compare(_tenEuro1, _default).Should().NotBe(0); //using static Compare

            //using Compareble operators
            (_tenEuro1 <= _default).Should().BeFalse();
            (_tenEuro1 >= _default).Should().BeTrue();
            (_tenEuro1 > _default).Should().BeTrue();
            (_tenEuro1 < _default).Should().BeFalse();
        }
    }
}
