using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NodaMoney.Tests
{
    static internal class MoneyComparableTests
    {
        [TestClass]
        public class GivenIWantToCompareMoney
        {
            private Money _tenEuro1 = new Money(10.00m, "EUR");
            private Money _tenEuro2 = new Money(10.00m, "EUR");
            private Money _twentyEuro = new Money(20.00m, "EUR");
            private Money _tenDollar = new Money(10.00m, "USD");

            [TestMethod]
            public void WhenCurrencyAndValueAreEqual_ThenMoneyShouldBeEqual()
            {
                Assert.AreEqual(_tenEuro1, _tenEuro2);
                Assert.IsTrue(_tenEuro1.Equals(_tenEuro2)); //using Equal()
                Assert.IsTrue(Equals(_tenEuro1, _tenEuro2)); //using static Equals()            
                Assert.IsTrue(_tenEuro1 == _tenEuro2); //using Euality operators
                Assert.IsFalse(_tenEuro1 != _tenEuro2); //using Euality operators
                Assert.AreEqual(_tenEuro1.GetHashCode(), _tenEuro2.GetHashCode()); //using GetHashCode()
            }

            [TestMethod]
            public void WhenValueIsDifferent_ThenMoneyShouldNotBeEqual()
            {
                Assert.AreNotEqual(_tenEuro1, _tenDollar);
                Assert.IsFalse(_tenEuro1.Equals(_tenDollar)); //using Equal()
                Assert.IsFalse(Equals(_tenEuro1, _tenDollar)); //using static Equals()
                Assert.IsFalse(_tenEuro1 == _tenDollar); //using Euality operators
                Assert.IsTrue(_tenEuro1 != _tenDollar); //using Euality operators
                Assert.AreNotEqual(_tenEuro1.GetHashCode(), _twentyEuro.GetHashCode()); //using GetHashCode()
            }

            [TestMethod]
            public void WhenCurrencyIsDifferent_ThenMoneyShouldNotBeEqual()
            {
                Assert.AreNotEqual(_tenEuro1, _tenDollar);
                Assert.IsFalse(_tenEuro1.Equals(_tenDollar)); //using Equal()
                Assert.IsFalse(Equals(_tenEuro1, _tenDollar)); //using static Equals()
                Assert.IsFalse(_tenEuro1 == _tenDollar); //using Euality operators
                Assert.IsTrue(_tenEuro1 != _tenDollar); //using Euality operators            
                Assert.AreNotEqual(_tenEuro1.GetHashCode(), _tenDollar.GetHashCode());//using GetHashCode()
            }

            [TestMethod]
            public void WhenComparingWithNull_ThenMoneyShouldNotBeEqual()
            {
                Assert.AreNotEqual(_tenEuro1, null);
                _tenEuro1.CompareTo(null).Should().Be(1);
            }

            [TestMethod]
            public void WhenComparingWithDifferentObject_ThenMoneyShouldNotBeEqual()
            {
                Assert.AreNotEqual(_tenEuro1, new object(), "Comparing Currency to a different object should fail!");
            }

            [TestMethod]
            public void WhenCurrencyAndValueAreEqual_ThenComparingShouldBeEqual()
            {
                Assert.AreEqual(_tenEuro1.CompareTo(_tenEuro2), 0); //using CompareTo()
                Assert.AreEqual(_tenEuro1.CompareTo((object)_tenEuro2), 0);  //using CompareTo()
                Assert.AreEqual(Money.Compare(_tenEuro1, _tenEuro2), 0); //using static Compare

                //using Compareble operators
                Assert.IsTrue(_tenEuro1 <= _tenEuro2);
                Assert.IsTrue(_tenEuro1 >= _tenEuro2);
                Assert.IsFalse(_tenEuro1 > _tenEuro2);
                Assert.IsFalse(_tenEuro1 < _tenEuro2);
            }

            [TestMethod]
            public void WhenValueIsDifferent_ThenComparingShouldNotBeEqual()
            {
                //using CompareTo()
                Assert.AreEqual(_tenEuro1.CompareTo(_twentyEuro), -1);
                Assert.AreEqual(_twentyEuro.CompareTo(_tenEuro1), 1);
                Assert.AreEqual(_twentyEuro.CompareTo((object)_tenEuro1), 1);

                //using static Compare
                Assert.AreEqual(Money.Compare(_tenEuro1, _twentyEuro), -1);
                Assert.AreEqual(Money.Compare(_twentyEuro, _tenEuro1), 1);

                //using Compareble operators
                Assert.IsTrue(_tenEuro1 < _twentyEuro);
                Assert.IsFalse(_tenEuro1 > _twentyEuro);
                Assert.IsTrue(_tenEuro1 <= _twentyEuro);
                Assert.IsFalse(_tenEuro1 >= _twentyEuro);
            }

            [TestMethod]
            public void WhenCurrencyIsDifferent_ThenComparingShouldFail()
            {
                Action action = () => _tenEuro1.CompareTo(_tenDollar);

                action.ShouldThrow<InvalidCurrencyException>().WithMessage("*are not of the same Currency*");
            }

            [TestMethod]
            public void WhenComparingToString_ThenComparingShouldFail()
            {
                Action action = () => _tenEuro1.CompareTo("10.00");

                action.ShouldThrow<ArgumentException>().WithMessage("obj is not the same type as this instance*");
            }
        }
    }
}