using System;
using System.Globalization;
using System.Threading;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NodaMoney.UnitTests.Helpers;

namespace NodaMoney.UnitTests
{
    public class MoneyTests
    {
        [TestClass]
        public class GivenIWantToExplicitCastMoneyToANumericType
        {
            readonly Money _euros = new Money(10.00m, "EUR");

            [TestMethod]
            public void WhenExplicitCastingToDecimal_ThenCastingShouldSucceed()
            {
                var m = (decimal)_euros;

                m.Should().Be(10.00m);
            }

            [TestMethod]
            public void WhenExplicitCastingToDouble_ThenCastingShouldSucceed()
            {
                var d = (double)_euros;

                d.Should().Be(10.00d);
            }

            [TestMethod]
            public void WhenExplicitCastingToFloat_ThenCastingShouldSucceed()
            {
                var f = (float)_euros;

                f.Should().Be(10.00f);
            }

            [TestMethod]
            public void WhenExplicitCastingToLong_ThenCastingShouldSucceed()
            {
                var l = (long)_euros;

                l.Should().Be(10L);
            }

            [TestMethod]
            public void WhenExplicitCastingToByte_ThenCastingShouldSucceed()
            {
                var b = (byte)_euros;

                b.Should().Be(10);
            }

            [TestMethod]
            public void WhenExplicitCastingToShort_ThenCastingShouldSucceed()
            {
                var s = (short)_euros;

                s.Should().Be(10);
            }

            [TestMethod]
            public void WhenExplicitCastingToInt_ThenCastingShouldSucceed()
            {
                var i = (int)_euros;

                i.Should().Be(10);
            }
        }

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

                action.ShouldThrow<InvalidCurrencyException>().WithMessage("*don't have the same Currency*");
            }

            [TestMethod]
            public void WhenComparingToString_ThenComparingShouldFail()
            {
                Action action = () => _tenEuro1.CompareTo("10.00");

                action.ShouldThrow<ArgumentException>().WithMessage("obj is not the same type as this instance*");
            }
        }

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
        public class GivenIWantMoneyFromANumericTypeAndAnExplicitCurrencyObject
        {
            private readonly Currency _euro = Currency.FromCode("EUR");

            [TestMethod]
            public void WhenValueIsByte_ThenCreatingShouldSucceed()
            {
                const byte byteValue = 50;
                var money = new Money(byteValue, _euro);

                money.Currency.Should().Be(_euro);
                money.Amount.Should().Be(50m);
            }

            [TestMethod]
            public void WhenValueIsSbyte_ThenCreatingShouldSucceed()
            {
                const sbyte sbyteValue = 75;
                var money = new Money(sbyteValue, _euro);

                money.Currency.Should().Be(_euro);
                money.Amount.Should().Be(75m);               
            }

            [TestMethod]
            public void WhenValueIsInt16_ThenCreatingShouldSucceed()
            {
                const short int16Value = 100;
                var money = new Money(int16Value, _euro);

                money.Currency.Should().Be(_euro);
                money.Amount.Should().Be(100m);
            }

            [TestMethod]
            public void WhenValueIsInt32_ThenCreatingShouldSucceed()
            {
                const int int32Value = 200;
                var money = new Money(int32Value, _euro);

                money.Currency.Should().Be(_euro);
                money.Amount.Should().Be(200m);
            }

            [TestMethod]
            public void WhenValueIsInt64_ThenCreatingShouldSucceed()
            {
                const long int64Value = 300;
                var money = new Money(int64Value, _euro);

                money.Currency.Should().Be(_euro);
                money.Amount.Should().Be(300m);
            }

            [TestMethod]
            public void WhenValueIsUint16_ThenCreatingShouldSucceed()
            {
                const ushort uInt16Value = 400;
                var money = new Money(uInt16Value, _euro);

                money.Currency.Should().Be(_euro);
                money.Amount.Should().Be(400m);
            }

            [TestMethod]
            public void WhenValueIsUint32_ThenCreatingShouldSucceed()
            {
                const uint uInt32Value = 500;
                var money = new Money(uInt32Value, _euro);

                money.Currency.Should().Be(_euro);
                money.Amount.Should().Be(500m);
            }

            [TestMethod]
            public void WhenValueIsUint64_ThenCreatingShouldSucceed()
            {
                const ulong uInt64Value = 600;
                var money = new Money(uInt64Value, _euro);

                money.Currency.Should().Be(_euro);
                money.Amount.Should().Be(600m);
            }

            [TestMethod]
            public void WhenValueIsSingle_ThenCreatingShouldSucceed()
            {
                const float singleValue = 700;
                var money = new Money(singleValue, _euro);

                money.Currency.Should().Be(_euro);
                money.Amount.Should().Be(700m);
            }

            [TestMethod]
            public void WhenValueIsDouble_ThenCreatingShouldSucceed()
            {
                const double doubleValue = 800;
                var money = new Money(doubleValue, _euro);

                money.Currency.Should().Be(_euro);
                money.Amount.Should().Be(800m);
            }

            [TestMethod]
            public void WhenValueIsDecimal_ThenCreatingShouldSucceed()
            {
                const decimal decimalValue = 900;
                var money = new Money(decimalValue, _euro);

                money.Currency.Should().Be(_euro);
                money.Amount.Should().Be(900m);
            }
        }

        [TestClass]
        public class GivenIWantMoneyFromANumericTypeAndAnImplicitCurrencyFromTheCurrentCulture
        {
            private readonly Currency _currentCurrency = Currency.FromCulture(Thread.CurrentThread.CurrentCulture);

            [TestMethod]
            public void WhenValueIsByte_ThenCreatingShouldSucceed()
            {
                const byte byteValue = 50;
                Money money = byteValue;

                money.Currency.Should().Be(_currentCurrency);
                money.Amount.Should().Be(50);
            }

            [TestMethod]
            public void WhenValueIsSbyte_ThenCreatingShouldSucceed()
            {
                const sbyte sbyteValue = 75;
                Money money = sbyteValue;

                money.Currency.Should().Be(_currentCurrency);
                money.Amount.Should().Be(75);
            }

            [TestMethod]
            public void WhenValueIsInt16_ThenCreatingShouldSucceed()
            {
                const short int16Value = 100;
                Money money = int16Value;

                money.Currency.Should().Be(_currentCurrency);
                money.Amount.Should().Be(100);
            }

            [TestMethod]
            public void WhenValueIsInt32_ThenCreatingShouldSucceed()
            {
                const int int32Value = 200;
                Money money = int32Value;

                money.Currency.Should().Be(_currentCurrency);
                money.Amount.Should().Be(200);
            }

            [TestMethod]
            public void WhenValueIsInt64_ThenCreatingShouldSucceed()
            {
                const long int64Value = 300;
                Money money = int64Value;

                money.Currency.Should().Be(_currentCurrency);
                money.Amount.Should().Be(300);
            }

            [TestMethod]
            public void WhenValueIsUint16_ThenCreatingShouldSucceed()
            {
                const ushort uInt16Value = 400;
                Money money = uInt16Value;

                money.Currency.Should().Be(_currentCurrency);
                money.Amount.Should().Be(400);
            }

            [TestMethod]
            public void WhenValueIsUint32_ThenCreatingShouldSucceed()
            {
                const uint uInt32Value = 500;
                Money money = uInt32Value;

                money.Currency.Should().Be(_currentCurrency);
                money.Amount.Should().Be(500);
            }

            [TestMethod]
            public void WhenValueIsUint64_ThenCreatingShouldSucceed()
            {
                const ulong uInt64Value = 600;
                Money money = uInt64Value;

                money.Currency.Should().Be(_currentCurrency);
                money.Amount.Should().Be(600);
            }

            [TestMethod]
            public void WhenValueIsSingleAndExplicitCast_ThenCreatingShouldSucceed()
            {
                const float singleValue = 700;
                var money = (Money)singleValue;

                money.Currency.Should().Be(_currentCurrency);
                money.Amount.Should().Be(700);
            }

            [TestMethod]
            public void WhenValueIsDoubleAndExplicitCast_ThenCreatingShouldSucceed()
            {
                var money = (Money)25.00;

                money.Currency.Should().Be(_currentCurrency);
                money.Amount.Should().Be(25.00m);
            }

            [TestMethod]
            public void WhenValueIsDecimal_ThenCreatingShouldSucceed()
            {
                Money money = 25.00m;

                money.Currency.Should().Be(_currentCurrency);
                money.Amount.Should().Be(25.00m);
            }
        }

        [TestClass]
        public class GivenIWantMoneyFromANumericTypeAndAnExplicitIsoCurrencyCode
        {
            private readonly Currency _euro = Currency.FromCode("EUR");

            [TestMethod]
            public void WhenValueIsByte_ThenCreatingShouldSucceed()
            {
                const byte byteValue = 50;
                var money = new Money(byteValue, "EUR");

                money.Currency.Should().Be(_euro);
                money.Amount.Should().Be(50);
            }

            [TestMethod]
            public void WhenValueIsSbyte_ThenCreatingShouldSucceed()
            {
                const sbyte sbyteValue = 75;
                var money = new Money(sbyteValue, "EUR");

                money.Currency.Should().Be(_euro);
                money.Amount.Should().Be(75);
            }

            [TestMethod]
            public void WhenValueIsInt16_ThenCreatingShouldSucceed()
            {
                const short int16Value = 100;
                var money = new Money(int16Value, "EUR");

                money.Currency.Should().Be(_euro);
                money.Amount.Should().Be(100);
            }

            [TestMethod]
            public void WhenValueIsInt32_ThenCreatingShouldSucceed()
            {
                const int int32Value = 200;
                var money = new Money(int32Value, "EUR");

                money.Currency.Should().Be(_euro);
                money.Amount.Should().Be(200);
            }

            [TestMethod]
            public void WhenValueIsInt64_ThenCreatingShouldSucceed()
            {
                const long int64Value = 300;
                var money = new Money(int64Value, "EUR");

                money.Currency.Should().Be(_euro);
                money.Amount.Should().Be(300);
            }

            [TestMethod]
            public void WhenValueIsUint16_ThenCreatingShouldSucceed()
            {
                const ushort uInt16Value = 400;
                var money = new Money(uInt16Value, "EUR");

                money.Currency.Should().Be(_euro);
                money.Amount.Should().Be(400);
            }

            [TestMethod]
            public void WhenValueIsUint32_ThenCreatingShouldSucceed()
            {
                const uint uInt32Value = 500;
                var money = new Money(uInt32Value, "EUR");

                money.Currency.Should().Be(_euro);
                money.Amount.Should().Be(500);
            }

            [TestMethod]
            public void WhenValueIsUint64_ThenCreatingShouldSucceed()
            {
                const ulong uInt64Value = 600;
                var money = new Money(uInt64Value, "EUR");

                money.Currency.Should().Be(_euro);
                money.Amount.Should().Be(600);
            }

            [TestMethod]
            public void WhenValueIsSingle_ThenCreatingShouldSucceed()
            {
                const float singleValue = 700;
                var money = new Money(singleValue, "EUR");

                money.Currency.Should().Be(_euro);
                money.Amount.Should().Be(700);
            }

            [TestMethod]
            public void WhenValueIsDouble_ThenCreatingShouldSucceed()
            {
                const double doubleValue = 800;
                var money = new Money(doubleValue, "EUR");

                money.Currency.Should().Be(_euro);
                money.Amount.Should().Be(800);
            }

            [TestMethod]
            public void WhenValueIsDecimal_ThenCreatingShouldSucceed()
            {
                const decimal decimalValue = 900;
                var money = new Money(decimalValue, "EUR");

                money.Currency.Should().Be(_euro);
                money.Amount.Should().Be(900);
            }

            [TestMethod]
            public void WhenUnknownIsoCurrencySymbol_ThenCreatingShouldFail()
            {
                Action action = () => new Money(123.25M, "XYZ");

                action.ShouldThrow<ArgumentException>();
            }
        }

        [TestClass]
        public class GivenIWantMoneyInOneOfTheFourMostUsedCurrenciesInTheWorld
        {
            [TestMethod]
            public void WhenEurosIsDecimal_ThenCreatingShouldSucceed()
            {
                //from decimal
                var euros = Money.Euro(10.00m);

                euros.Currency.Should().Be(Currency.FromCode("EUR"));
                euros.Amount.Should().Be(10.00m);
            }

            [TestMethod]
            public void WhenEurosIsDouble_ThenCreatingShouldSucceed()
            {
                //from double (float is implicitly converted to double)
                var euros = Money.Euro(10.00);

                euros.Currency.Should().Be(Currency.FromCode("EUR"));
                euros.Amount.Should().Be(10.00m);
            }

            [TestMethod]
            public void WhenEurosIsLong_ThenCreatingShouldSucceed()
            {
                //from long (byte, short and int are implicitly converted to long)
                var euros = Money.Euro(10L);

                euros.Currency.Should().Be(Currency.FromCode("EUR"));
                euros.Amount.Should().Be(10.00m);
            }

            [TestMethod]
            public void WhenDollarsIsDecimal_ThenCreatingShouldSucceed()
            {
                //from decimal (other integral types are implicitly converted to decimal)
                var dollars = Money.USDollar(10.00m);

                dollars.Currency.Should().Be(Currency.FromCode("USD"));
                dollars.Amount.Should().Be(10.00m);
            }

            [TestMethod]
            public void WhenDollarsIsDouble_ThenCreatingShouldSucceed()
            {
                //from double (float is implicitly converted to double)
                var dollars = Money.USDollar(10.00);

                dollars.Currency.Should().Be(Currency.FromCode("USD"));
                dollars.Amount.Should().Be(10.00m);
            }

            [TestMethod]
            public void WhenDollarsIsLong_ThenCreatingShouldSucceed()
            {
                //from long (byte, short and int are implicitly converted to long)
                var dollars = Money.USDollar(10L);

                dollars.Currency.Should().Be(Currency.FromCode("USD"));
                dollars.Amount.Should().Be(10.00m);
            }

            [TestMethod]
            public void WhenYensIsDecimal_ThenCreatingShouldSucceed()
            {
                //from decimal (other integral types are implicitly converted to decimal)
                var yens = Money.Yen(10.00m);

                yens.Should().NotBeNull();
                yens.Currency.Should().Be(Currency.FromCode("JPY"));
                yens.Amount.Should().Be(10.00m);
            }

            [TestMethod]
            public void WhenYensIsDouble_ThenCreatingShouldSucceed()
            {
                //from double (float is implicitly converted to double)
                var yens = Money.Yen(10.00);

                yens.Should().NotBeNull();
                yens.Currency.Should().Be(Currency.FromCode("JPY"));
                yens.Amount.Should().Be(10.00m);
            }

            [TestMethod]
            public void WhenYensIsLong_ThenCreatingShouldSucceed()
            {
                //from long (byte, short and int are implicitly converted to long)
                var yens = Money.Yen(10L);

                yens.Should().NotBeNull();
                yens.Currency.Should().Be(Currency.FromCode("JPY"));
                yens.Amount.Should().Be(10.00m);
            }

            [TestMethod]
            public void WhenPondsIsDecimal_ThenCreatingShouldSucceed()
            {
                //from decimal (other integral types are implicitly converted to decimal)
                var pounds = Money.PoundSterling(10.00m);

                pounds.Should().NotBeNull();
                pounds.Currency.Should().Be(Currency.FromCode("GBP"));
                pounds.Amount.Should().Be(10.00m);
            }

            [TestMethod]
            public void WhenPondsIsDouble_ThenCreatingShouldSucceed()
            {
                //from double (float is implicitly converted to double)
                var pounds = Money.PoundSterling(10.00);

                pounds.Should().NotBeNull();
                pounds.Currency.Should().Be(Currency.FromCode("GBP"));
                pounds.Amount.Should().Be(10.00m);
            }

            [TestMethod]
            public void WhenPondsIsLong_ThenCreatingShouldSucceed()
            {
                //from long (byte, short and int are implicitly converted to long)
                var pounds = Money.PoundSterling(10L);

                pounds.Should().NotBeNull();
                pounds.Currency.Should().Be(Currency.FromCode("GBP"));
                pounds.Amount.Should().Be(10.00m);
            }
        }

        [TestClass]
        public class GivenIWantToConvertMoneyToString
        {
            private Money _yen = new Money(765.4321m, Currency.FromCode("JPY"));
            private Money _euro = new Money(765.4321m, Currency.FromCode("EUR"));
            private Money _dollar = new Money(765.4321m, Currency.FromCode("USD"));
            private Money _dinar = new Money(765.4321m, Currency.FromCode("BHD"));

            [TestMethod]
            public void WhenImplicitConversion_ThenNumberOfDecimalsShouldBeDefaultOfCurrency()
            {
                using (new SwitchCulture("en-US"))
                {
                    _yen.ToString().Should().Be("¥765");
                    _euro.ToString().Should().Be("€765.43");
                    _dollar.ToString().Should().Be("$765.43");
                    _dinar.ToString().Should().Be("BD765.432");
                }
            }

            [TestMethod]
            public void WhenExplicitToZeroDecimals_ThenThisShouldSucceed()
            {
                using (new SwitchCulture("en-US"))
                {
                    _yen.ToString("C0").Should().Be("¥765");
                    _euro.ToString("C0").Should().Be("€765");
                    _dollar.ToString("C0").Should().Be("$765");
                    _dinar.ToString("C0").Should().Be("BD765");
                }
            }

            [TestMethod]
            public void WhenExplicitToOneDecimals_ThenThisShouldSucceed()
            {
                using (new SwitchCulture("en-US"))
                {
                    _yen.ToString("C1").Should().Be("¥765.0");
                    _euro.ToString("C1").Should().Be("€765.4");
                    _dollar.ToString("C1").Should().Be("$765.4");
                    _dinar.ToString("C1").Should().Be("BD765.4");
                }
            }

            [TestMethod]
            public void WhenExplicitToTwoDecimals_ThenThisShouldSucceed()
            {
                using (new SwitchCulture("en-US"))
                {
                    _yen.ToString("C2").Should().Be("¥765.00");
                    _euro.ToString("C2").Should().Be("€765.43");
                    _dollar.ToString("C2").Should().Be("$765.43");
                    _dinar.ToString("C2").Should().Be("BD765.43");
                }
            }

            [TestMethod]
            public void WhenExplicitToThreeDecimals_ThenThisShouldSucceed()
            {
                using (new SwitchCulture("en-US"))
                {
                    _yen.ToString("C3").Should().Be("¥765.000");
                    _euro.ToString("C3").Should().Be("€765.430");
                    _dollar.ToString("C3").Should().Be("$765.430");
                    _dinar.ToString("C3").Should().Be("BD765.432");
                }
            }

            [TestMethod]
            public void WhenExplicitToFourDecimals_ThenThisShouldSucceed()
            {
                using (new SwitchCulture("en-US"))
                {
                    _yen.ToString("C4").Should().Be("¥765.0000");
                    _euro.ToString("C4").Should().Be("€765.4300");
                    _dollar.ToString("C4").Should().Be("$765.4300");
                    _dinar.ToString("C4").Should().Be("BD765.4320");
                }
            }

            [TestMethod]
            public void WhenSpecificCultureIsUsed_ThenCurrencySymbolAndDecimalsOfMoneyShouldStillBeLeading()
            {
                using (new SwitchCulture("en-US"))
                {
                    var ci = new CultureInfo("nl-NL");

                    _yen.ToString(ci).Should().Be("¥ 765");
                    _euro.ToString(ci).Should().Be("€ 765,43");
                    _dollar.ToString(ci).Should().Be("$ 765,43");
                    _dinar.ToString(ci).Should().Be("BD 765,432");
                }
            }

            [TestMethod]
            public void WhenSpecificNumberFormatIsUsed_ThenCurrencySymbolAndDecimalsOfMoneyShouldStillBeLeading()
            {
                using (new SwitchCulture("en-US"))
                {
                    var nfi = new CultureInfo("nl-NL").NumberFormat;

                    _yen.ToString(nfi).Should().Be("¥ 765");
                    _euro.ToString(nfi).Should().Be("€ 765,43");
                    _dollar.ToString(nfi).Should().Be("$ 765,43");
                    _dinar.ToString(nfi).Should().Be("BD 765,432");
                }
            }

            [TestMethod]
            public void WhenShowingMoneyInBelgiumDutchSpeaking_ThenThisShouldSucceed()
            {
                using (new SwitchCulture("nl-BE"))
                {
                    _yen.ToString().Should().Be("¥ 765");
                    _euro.ToString().Should().Be("€ 765,43");
                    _dollar.ToString().Should().Be("$ 765,43");
                    _dinar.ToString().Should().Be("BD 765,432");
                }
            }

            [TestMethod]
            public void WhenShowingMoneyInBelgiumFrenchSpeaking_ThenThisShouldSucceed()
            {
                using (new SwitchCulture("fr-BE"))
                {
                    _yen.ToString().Should().Be("765 ¥");
                    _euro.ToString().Should().Be("765,43 €");
                    _dollar.ToString().Should().Be("765,43 $");
                    _dinar.ToString().Should().Be("765,432 BD");
                }
            }
        }

        [TestClass]
        public class GivenIWantToIncrementMoney
        {
            private Money _yens = new Money(765m, Currency.FromCode("JPY"));
            private Money _euros = new Money(765.43m, Currency.FromCode("EUR"));
            private Money _dollars = new Money(765.43m, Currency.FromCode("USD"));
            private Money _dinars = new Money(765.432m, Currency.FromCode("BHD"));

            [TestMethod]
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

        [TestClass]
        public class GivenIWantToDecrementMoney
        {
            private Money _yens = new Money(765m, Currency.FromCode("JPY"));
            private Money _euros = new Money(765.43m, Currency.FromCode("EUR"));
            private Money _dollars = new Money(765.43m, Currency.FromCode("USD"));
            private Money _dinars = new Money(765.432m, Currency.FromCode("BHD"));

            [TestMethod]
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

        [TestClass]
        public class GivenIWantToSerializeMoney
        {
            //TODO: Add GivenIWantToSerializeMoney unit tests?
            //private Money yen = new Money(765m, Currency.FromCode("JPY"));
            //private Money euro = new Money(765.43m, Currency.FromCode("EUR"));
            //private Money dollar = new Money(765.43m, Currency.FromCode("USD"));
            //private Money dinar = new Money(765.432m, Currency.FromCode("BHD"));

            //[TestMethod]
            //public void WhenSerializing_ThenThisShouldSucceed()
            //{
            //    yen.Should().Be(Clone<Money>(yen));
            //}

            //public static Stream Serialize(object source)
            //{
            //    IFormatter formatter = new BinaryFormatter();
            //    Stream stream = new MemoryStream();
            //    formatter.Serialize(stream, source);
            //    return stream;
            //}

            //public static T Deserialize<T>(Stream stream)
            //{
            //    IFormatter formatter = new BinaryFormatter();
            //    stream.Position = 0;
            //    return (T)formatter.Deserialize(stream);
            //}

            //public static T Clone<T>(object source)
            //{
            //    return Deserialize<T>(Serialize(source));
            //}
        }

        [TestClass]
        public class GivenIWantToConvertMoney
        {
            readonly Money _euros = new Money(765.43m, "EUR");

            [TestMethod]
            public void WhenConvertingToDecimal_ThenThisShouldSucceed()
            {
                var result = Money.ToDecimal(_euros);

                result.Should().Be(765.43m);                
            }

            [TestMethod]
            public void WhenConvertingToDouble_ThenThisShouldSucceed()
            {
                var result = Money.ToDouble(_euros);

                result.Should().BeApproximately(765.43d, 0.001d);
            }

            [TestMethod]
            public void WhenConvertingToSingle_ThenThisShouldSucceed()
            {
                var result = Money.ToSingle(_euros);

                result.Should().BeApproximately(765.43f, 0.001f);
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

        //public void testCloseNumbersNotEqual()
        //{
        //    Money d2_51a = Money.dollars(2.515);
        //    Money d2_51b = Money.dollars(2.5149);
        //    assertTrue(!d2_51a.equals(d2_51b));
        //}

        //public void testRound()
        //{
        //    Money dRounded = Money.dollars(1.2350);
        //    assertEquals(Money.dollars(1.24), dRounded);
        //}

        //public void testSubtraction()
        //{
        //    assertEquals(Money.dollars(12.49), d15.minus(d2_51));
        //}

        //public void testApplyRatio()
        //{
        //    Ratio oneThird = Ratio.of(1, 3);
        //    Money result = Money.dollars(100).applying(oneThird, 1, Rounding.UP);
        //    assertEquals(Money.dollars(33.40), result);
        //}

        //public void testIncremented()
        //{
        //    assertEquals(Money.dollars(2.52), d2_51.incremented());
        //    assertEquals(Money.valueOf(51, JPY), y50.incremented());
        //}

        //[Fact]
        //public void MoneyOperationsInvolvingDifferentCurrencyAllFail()
        //{
        //    Money money1 = new Money(101.5M, Currency.Aud);
        //    Money money2 = new Money(98.5M, Currency.Cad);
        //    Money m;
        //    Boolean b;

        //    Assert.Throws<InvalidOperationException>(() => { m = money1 + money2; });
        //    Assert.Throws<InvalidOperationException>(() => { m = money1 - money2; });
        //    Assert.Throws<InvalidOperationException>(() => { b = money1 == money2; });
        //    Assert.Throws<InvalidOperationException>(() => { b = money1 != money2; });
        //    Assert.Throws<InvalidOperationException>(() => { b = money1 > money2; });
        //    Assert.Throws<InvalidOperationException>(() => { b = money1 < money2; });
        //    Assert.Throws<InvalidOperationException>(() => { b = money1 >= money2; });
        //    Assert.Throws<InvalidOperationException>(() => { b = money1 <= money2; });
        //}        
    }
}
