using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NodaMoney.UnitTests
{
    static internal class MoneyBinaryOperatorsTests
    {
        [TestClass]
        public class GivenIWantToAddAndSubtractMoney
        {
            private readonly Money _tenEuro = new Money(10.00m, "EUR");
            private readonly Money _tenDollar = new Money(10.00m, "USD");

            [TestMethod]
            public void WhenCurrencyIsEqual_ThenAdditionAndSubtractionShouldBePossible()
            {
                // whole number
                var money1 = new Money(101m);
                var money2 = new Money(99m);

                Assert.AreEqual(new Money(200), money1 + money2);
                Assert.AreEqual(new Money(2), money1 - money2);

                // using CLR methods for languages that don't support operators
                Assert.AreEqual(new Money(200), Money.Add(money1, money2));
                Assert.AreEqual(new Money(2), Money.Subtract(money1, money2));

                // fractions
                var money3 = new Money(100.00m);
                var money4 = new Money(0.01m);

                Assert.AreEqual(new Money(100.01m), money3 + money4);
                Assert.AreEqual(new Money(99.99m), money3 - money4);

                // overflow
                var money5 = new Money(100.999m);
                var money6 = new Money(100.5m);
                var money7 = new Money(0.9m);

                Assert.AreEqual(new Money(101.899m), money5 + money7);
                Assert.AreEqual(new Money(100.099m), money5 - money7);
                Assert.AreEqual(new Money(101.4m), money6 + money7);
                Assert.AreEqual(new Money(99.6m), money6 - money7);

                // negative
                var money8 = new Money(100.999m);
                var money9 = new Money(-0.9m);
                var money10 = new Money(-100.999m);

                Assert.AreEqual(new Money(100.099m), money8 + money9);
                Assert.AreEqual(new Money(101.899m), money8 - money9);
                Assert.AreEqual(new Money(-101.899m), money10 + money9);
                Assert.AreEqual(new Money(-100.099M), money10 - money9);
            }

            [TestMethod]
            public void WhenCurrencyIsDifferent_ThenAdditionShouldFail()
            {
// ReSharper disable once UnusedVariable
                Action action = () => { var result = _tenEuro + _tenDollar; };

                action.ShouldThrow<InvalidCurrencyException>().WithMessage("*don't have the same Currency*");
            }

            [TestMethod]
            public void WhenCurrencyIsDifferent_ThenSubtractionShouldFail()
            {
// ReSharper disable once UnusedVariable
                Action action = () => { var result = _tenEuro - _tenDollar; };

                action.ShouldThrow<InvalidCurrencyException>().WithMessage("*don't have the same Currency*");
            }

            [TestMethod]
            public void WhenUsingUnaryPlusOperator_ThenThisSucceed()
            {
                var m = +_tenEuro;

                m.Amount.Should().Be(10.00m);
                m.Currency.Code.Should().Be("EUR");
            }

            [TestMethod]
            public void WhenUsingUnaryMinOperator_ThenThisSucceed()
            {
                var m = -_tenEuro;

                m.Amount.Should().Be(-10.00m);
                m.Currency.Code.Should().Be("EUR");
            }
        }

        [TestClass]
        public class GivenIWantToMultiplyAndDivideMoney
        {
            [TestMethod]
            public void ShouldMultiplyCorrectly()
            {
                var money1 = new Money(100.12);

                (money1 * 0.5m).Should().Be(new Money(50.06m)); // decimal
                //Assert.AreEqual(new Money(50.0625m), money1 * 0.5); // double
                (money1 * 5).Should().Be(new Money(500.60m)); // int

                Money.Multiply(money1, 0.5m).Should().Be(new Money(50.06m));
                //Assert.AreEqual(new Money(50.0625m), Money.Multiply(money1, 0.5));
                Money.Multiply(money1, 5).Should().Be(new Money(500.60m));

                var money2 = new Money(-100.12);

                (money2 * 0.5m).Should().Be(new Money(-50.06m)); // decimal
                //Assert.AreEqual(new Money(50.0625m), money2 * 0.5); // double
                (money2 * 5).Should().Be(new Money(-500.60m)); // int

                // multiplier first
                (0.5m * money2).Should().Be(new Money(-50.06m));
                //Assert.AreEqual(new Money(50.0625m), money2 * 0.5);
                (5 * money2).Should().Be(new Money(-500.60m));

                var money3 = new Money(15);

                (money3 * 10).Should().Be(new Money(150));
                //(money3 * 0.1).Should().Be(new Money(1.5));

                //var money4 = new Money(100);

                //Assert.AreEqual(new Money(70), money4 * 0.7);

                //assertEquals(Money.dollars(66.67), d100.times(0.66666667));
                //assertEquals(Money.dollars(66.66), d100.times(0.66666667, Rounding.DOWN));

                //assertEquals(Money.dollars(66.67), d100.times(new BigDecimal("0.666666"), Rounding.HALF_EVEN));
                //assertEquals(Money.dollars(66.66), d100.times(new BigDecimal("0.666666"), Rounding.DOWN));
                //assertEquals(Money.dollars(-66.66), d100.negated().times(new BigDecimal("0.666666"), Rounding.DOWN));
            }

            [TestMethod]
            public void ShouldDivideCorrectly()
            {
                var money1 = new Money(100.12);

                (money1 / 2).Should().Be(new Money(50.06m)); // int
                (money1 / 0.5m).Should().Be(new Money(200.24m)); // decimal
                //Assert.AreEqual(new Money(200.25m), money1 / 0.5); // double

                Money.Divide(money1, 2).Should().Be(new Money(50.06m));
                Money.Divide(money1, 0.5m).Should().Be(new Money(200.24m));
                //Assert.AreEqual(new Money(200.25m), Money.Divide(money1, 0.5));

                var money3 = new Money(-100.12);

                (money3 / 2).Should().Be(new Money(-50.06m));
                (money3 / 0.5m).Should().Be(new Money(-200.24m));
                //Assert.AreEqual(new Money(-200.25m), money3 / 0.5);

                var money4 = new Money(100);

                (money4 / 3).Should().Be(new Money(33.33));
                (money4 / 6).Should().Be(new Money(16.67));

                //assertEquals(new BigDecimal(2.50), Money.dollars(5.00).dividedBy(Money.dollars(2.00)).decimalValue(1, Rounding.UNNECESSARY));
                //assertEquals(new BigDecimal(1.25), Money.dollars(5.00).dividedBy(Money.dollars(4.00)).decimalValue(2, Rounding.UNNECESSARY));
                //assertEquals(new BigDecimal(5), Money.dollars(5.00).dividedBy(Money.dollars(1.00)).decimalValue(0, Rounding.UNNECESSARY));
                //try
                //{
                //    Money.dollars(5.00).dividedBy(Money.dollars(2.00)).decimalValue(0, Rounding.UNNECESSARY);
                //    fail("dividedBy(Money) does not allow rounding.");
                //}
                //catch (ArithmeticException correctBehavior)
                //{
                //}
                //try
                //{
                //    Money.dollars(10.00).dividedBy(Money.dollars(3.00)).decimalValue(5, Rounding.UNNECESSARY);
                //    fail("dividedBy(Money) does not allow rounding.");
                //}
                //catch (ArithmeticException correctBehavior)
                //{
                //}
            }
        }
    }
}