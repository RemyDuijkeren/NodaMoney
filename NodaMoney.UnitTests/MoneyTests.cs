using System;
using System.Globalization;
using System.Threading;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NodaMoney.UnitTests.Helpers;

namespace NodaMoney.UnitTests
{
    [TestClass]
    public class MoneyTests
    {
        //// TODO: Test serializing http://stackoverflow.com/questions/236599/how-to-unit-test-if-my-object-is-really-serializable
        [TestClass]
        public class GivenIWantToExplicitCastMoneyToANumericType
        {
            Money euro10 = new Money(10.00m, "EUR");

            [TestMethod]
            public void WhenExplicitCastingToDecimal_ThenCastingShouldSucceed()
            {
                var m = (decimal)euro10;

                m.Should().Be(10.00m);
            }

            [TestMethod]
            public void WhenExplicitCastingToDouble_ThenCastingShouldSucceed()
            {
                var d = (double)euro10;

                d.Should().Be(10.00d);
            }

            [TestMethod]
            public void WhenExplicitCastingToFloat_ThenCastingShouldSucceed()
            {
                var f = (float)euro10;

                f.Should().Be(10.00f);
            }

            [TestMethod]
            public void WhenExplicitCastingToLong_ThenCastingShouldSucceed()
            {
                var l = (long)euro10;

                l.Should().Be(10L);
            }

            [TestMethod]
            public void WhenExplicitCastingToByte_ThenCastingShouldSucceed()
            {
                var b = (byte)euro10;

                b.Should().Be((byte)10);
            }

            [TestMethod]
            public void WhenExplicitCastingToShort_ThenCastingShouldSucceed()
            {
                var s = (short)euro10;

                s.Should().Be((short)10);
            }

            [TestMethod]
            public void WhenExplicitCastingToInt_ThenCastingShouldSucceed()
            {
                var i = (int)euro10;

                i.Should().Be(10);
            }
        }

        [TestClass]
        public class GivenIWantToCompareMoney
        {
            private Money euro10 = new Money(10.00m, "EUR");
            private Money euro10_1 = new Money(10.00m, "EUR");
            private Money euro20 = new Money(20.00m, "EUR");
            private Money dollar10 = new Money(10.00m, "USD");

            [TestMethod]
            public void WhenCurrencyAndValueAreEqual_ThenMoneyShouldBeEqual()
            {
                Assert.AreEqual(euro10, euro10_1);
                Assert.IsTrue(euro10.Equals(euro10_1)); //using Equal()
                Assert.IsTrue(Money.Equals(euro10, euro10_1)); //using static Equals()            
                Assert.IsTrue(euro10 == euro10_1); //using Euality operators
                Assert.IsFalse(euro10 != euro10_1); //using Euality operators
                Assert.AreEqual(euro10.GetHashCode(), euro10_1.GetHashCode()); //using GetHashCode()
            }

            [TestMethod]
            public void WhenValueIsDifferent_ThenMoneyShouldNotBeEqual()
            {
                Assert.AreNotEqual(euro10, dollar10);
                Assert.IsFalse(euro10.Equals(dollar10)); //using Equal()
                Assert.IsFalse(Money.Equals(euro10, dollar10)); //using static Equals()
                Assert.IsFalse(euro10 == dollar10); //using Euality operators
                Assert.IsTrue(euro10 != dollar10); //using Euality operators
                Assert.AreNotEqual(euro10.GetHashCode(), euro20.GetHashCode()); //using GetHashCode()
            }

            [TestMethod]
            public void WhenCurrencyIsDifferent_ThenMoneyShouldNotBeEqual()
            {
                Assert.AreNotEqual(euro10, dollar10);
                Assert.IsFalse(euro10.Equals(dollar10)); //using Equal()
                Assert.IsFalse(Money.Equals(euro10, dollar10)); //using static Equals()
                Assert.IsFalse(euro10 == dollar10); //using Euality operators
                Assert.IsTrue(euro10 != dollar10); //using Euality operators            
                Assert.AreNotEqual(euro10.GetHashCode(), dollar10.GetHashCode());//using GetHashCode()
            }

            [TestMethod]
            public void WhenComparingWithNull_ThenMoneyShouldNotBeEqual()
            {
                Assert.AreNotEqual(euro10, null);
            }

            [TestMethod]
            public void WhenComparingWithDifferentObject_ThenMoneyShouldNotBeEqual()
            {
                Assert.AreNotEqual(euro10, new object(), "Comparing Currency to a different object should fail!");
            }

            [TestMethod]
            public void WhenCurrencyAndValueAreEqual_ThenComparingShouldBeEqual()
            {
                Assert.AreEqual(euro10.CompareTo(euro10_1), 0); //using CompareTo()
                Assert.AreEqual(euro10.CompareTo((object)euro10_1), 0);  //using CompareTo()
                Assert.AreEqual(Money.Compare(euro10, euro10_1), 0); //using static Compare

                //using Compareble operators
                Assert.IsTrue(euro10 <= euro10_1);
                Assert.IsTrue(euro10 >= euro10_1);
                Assert.IsFalse(euro10 > euro10_1);
                Assert.IsFalse(euro10 < euro10_1);
            }

            [TestMethod]
            public void WhenValueIsDifferent_ThenComparingShouldNotBeEqual()
            {
                //using CompareTo()
                Assert.AreEqual(euro10.CompareTo(euro20), -1);
                Assert.AreEqual(euro20.CompareTo(euro10), 1);
                Assert.AreEqual(euro20.CompareTo((object)euro10), 1);

                //using static Compare
                Assert.AreEqual(Money.Compare(euro10, euro20), -1);
                Assert.AreEqual(Money.Compare(euro20, euro10), 1);

                //using Compareble operators
                Assert.IsTrue(euro10 < euro20);
                Assert.IsFalse(euro10 > euro20);
                Assert.IsTrue(euro10 <= euro20);
                Assert.IsFalse(euro10 >= euro20);
            }

            [TestMethod]
            [ExpectedException(typeof(ArgumentException))]
            public void WhenCurrencyIsDifferent_ThenComparingShouldFail()
            {
                var result = euro10.CompareTo(dollar10);
            }

            [TestMethod]
            [ExpectedException(typeof(ArgumentException))]
            public void WhenComparingToString_ThenComparingShouldFail()
            {
                var result = euro10.CompareTo("10.00");
            }
        }

        [TestClass]
        public class GivenIWantToAddAndSubtractMoney
        {
            private Money euro10 = new Money(10.00m, "EUR");
            private Money euro10_1 = new Money(10.00m, "EUR");
            private Money euro20 = new Money(20.00m, "EUR");
            private Money dollar10 = new Money(10.00m, "USD");

            [TestMethod]
            public void WhenCurrencyIsEqual_ThenAdditionAndSubtractionShouldBePossible()
            {
                // whole number
                Money money1 = new Money(101m);
                Money money2 = new Money(99m);

                Assert.AreEqual(new Money(200), money1 + money2);
                Assert.AreEqual(new Money(2), money1 - money2);

                // using CLR methods for languages that don't support operators
                Assert.AreEqual(new Money(200), Money.Add(money1, money2));
                Assert.AreEqual(new Money(2), Money.Subtract(money1, money2));

                // fractions
                Money money3 = new Money(100.00m);
                Money money4 = new Money(0.01m);

                Assert.AreEqual(new Money(100.01m), money3 + money4);
                Assert.AreEqual(new Money(99.99m), money3 - money4);

                // overflow
                Money money5 = new Money(100.999m);
                Money money6 = new Money(100.5m);
                Money money7 = new Money(0.9m);

                Assert.AreEqual(new Money(101.899m), money5 + money7);
                Assert.AreEqual(new Money(100.099m), money5 - money7);
                Assert.AreEqual(new Money(101.4m), money6 + money7);
                Assert.AreEqual(new Money(99.6m), money6 - money7);

                // negative
                Money money8 = new Money(100.999m);
                Money money9 = new Money(-0.9m);
                Money money10 = new Money(-100.999m);

                Assert.AreEqual(new Money(100.099m), money8 + money9);
                Assert.AreEqual(new Money(101.899m), money8 - money9);
                Assert.AreEqual(new Money(-101.899m), money10 + money9);
                Assert.AreEqual(new Money(-100.099M), money10 - money9);
            }

            [TestMethod]
            [ExpectedException(typeof(ArgumentException))]
            public void WhenCurrencyIsDifferent_ThenAdditionShouldFail()
            {
                Money euro10 = new Money(10.00m, "EUR");
                Money dollar10 = new Money(10.00m, "USD");

                Money result = euro10 + dollar10;
            }

            [TestMethod]
            [ExpectedException(typeof(ArgumentException))]
            public void WhenCurrencyIsDifferent_ThenSubtractionShouldFail()
            {
                Money euro10 = new Money(10.00m, "EUR");
                Money dollar10 = new Money(10.00m, "USD");

                Money result = euro10 - dollar10;
            }

            [TestMethod]
            public void WhenUsingUnaryPlusOperator_ThenThisSucceed()
            {
                var m = +euro10;

                m.Amount.Should().Be(10.00m);
                m.Currency.Code.Should().Be("EUR");
            }

            [TestMethod]
            public void WhenUsingUnaryMinOperator_ThenThisSucceed()
            {
                var m = -euro10;

                m.Amount.Should().Be(-10.00m);
                m.Currency.Code.Should().Be("EUR");
            }
        }

        [TestClass]
        public class GivenIWantMoneyFromANumericTypeAndAnExplicitCurrencyObject
        {
            private Currency euro = Currency.FromCode("EUR");

            [TestMethod]
            public void WhenValueIsByte_ThenCreatingShouldSucceed()
            {
                byte byteValue = 50;
                var money = new Money(byteValue, euro);

                money.Currency.Should().Be(euro);
                money.Amount.Should().Be(50m);
            }

            [TestMethod]
            public void WhenValueIsSbyte_ThenCreatingShouldSucceed()
            {
                sbyte sByteValue = 75;
                var money = new Money(sByteValue, euro);

                money.Currency.Should().Be(euro);
                money.Amount.Should().Be(75m);               
            }

            [TestMethod]
            public void WhenValueIsInt16_ThenCreatingShouldSucceed()
            {
                short int16Value = 100;
                var money = new Money(int16Value, euro);

                money.Currency.Should().Be(euro);
                money.Amount.Should().Be(100m);
            }

            [TestMethod]
            public void WhenValueIsInt32_ThenCreatingShouldSucceed()
            {
                int int32Value = 200;
                var money = new Money(int32Value, euro);

                money.Currency.Should().Be(euro);
                money.Amount.Should().Be(200m);
            }

            [TestMethod]
            public void WhenValueIsInt64_ThenCreatingShouldSucceed()
            {
                long int64Value = 300;
                var money = new Money(int64Value, euro);

                money.Currency.Should().Be(euro);
                money.Amount.Should().Be(300m);
            }

            [TestMethod]
            public void WhenValueIsUint16_ThenCreatingShouldSucceed()
            {
                ushort uInt16Value = 400;
                var money = new Money(uInt16Value, euro);

                money.Currency.Should().Be(euro);
                money.Amount.Should().Be(400m);
            }

            [TestMethod]
            public void WhenValueIsUint32_ThenCreatingShouldSucceed()
            {
                uint uInt32Value = 500;
                var money = new Money(uInt32Value, euro);

                money.Currency.Should().Be(euro);
                money.Amount.Should().Be(500m);
            }

            [TestMethod]
            public void WhenValueIsUint64_ThenCreatingShouldSucceed()
            {
                ulong uInt64Value = 600;
                var money = new Money(uInt64Value, euro);

                money.Currency.Should().Be(euro);
                money.Amount.Should().Be(600m);
            }

            [TestMethod]
            public void WhenValueIsSingle_ThenCreatingShouldSucceed()
            {
                float singleValue = 700;
                var money = new Money(singleValue, euro);

                money.Currency.Should().Be(euro);
                money.Amount.Should().Be(700m);
            }

            [TestMethod]
            public void WhenValueIsDouble_ThenCreatingShouldSucceed()
            {
                double doubleValue = 800;
                var money = new Money(doubleValue, euro);

                money.Currency.Should().Be(euro);
                money.Amount.Should().Be(800m);
            }

            [TestMethod]
            public void WhenValueIsDecimal_ThenCreatingShouldSucceed()
            {
                decimal decimalValue = 900;
                var money = new Money(decimalValue, euro);

                money.Currency.Should().Be(euro);
                money.Amount.Should().Be(900m);
            }
        }

        [TestClass]
        public class GivenIWantMoneyFromANumericTypeAndAnImplicitCurrencyFromTheCurrentCulture
        {
            private Currency currentCurrency = Currency.FromCulture(Thread.CurrentThread.CurrentCulture);

            [TestMethod]
            public void WhenValueIsByte_ThenCreatingShouldSucceed()
            {
                byte byteValue = 50;
                Money money = byteValue;

                money.Currency.Should().Be(currentCurrency);
                money.Amount.Should().Be(50);
            }

            [TestMethod]
            public void WhenValueIsSbyte_ThenCreatingShouldSucceed()
            {
                sbyte sByteValue = 75;
                Money money = sByteValue;

                money.Currency.Should().Be(currentCurrency);
                money.Amount.Should().Be(75);
            }

            [TestMethod]
            public void WhenValueIsInt16_ThenCreatingShouldSucceed()
            {
                short int16Value = 100;
                Money money = int16Value;

                money.Currency.Should().Be(currentCurrency);
                money.Amount.Should().Be(100);
            }

            [TestMethod]
            public void WhenValueIsInt32_ThenCreatingShouldSucceed()
            {
                int int32Value = 200;
                Money money = int32Value;

                money.Currency.Should().Be(currentCurrency);
                money.Amount.Should().Be(200);
            }

            [TestMethod]
            public void WhenValueIsInt64_ThenCreatingShouldSucceed()
            {
                long int64Value = 300;
                Money money = int64Value;

                money.Currency.Should().Be(currentCurrency);
                money.Amount.Should().Be(300);
            }

            [TestMethod]
            public void WhenValueIsUint16_ThenCreatingShouldSucceed()
            {
                ushort uInt16Value = 400;
                Money money = uInt16Value;

                money.Currency.Should().Be(currentCurrency);
                money.Amount.Should().Be(400);
            }

            [TestMethod]
            public void WhenValueIsUint32_ThenCreatingShouldSucceed()
            {
                uint uInt32Value = 500;
                Money money = uInt32Value;

                money.Currency.Should().Be(currentCurrency);
                money.Amount.Should().Be(500);
            }

            [TestMethod]
            public void WhenValueIsUint64_ThenCreatingShouldSucceed()
            {
                ulong uInt64Value = 600;
                Money money = uInt64Value;

                money.Currency.Should().Be(currentCurrency);
                money.Amount.Should().Be(600);
            }

            [TestMethod]
            public void WhenValueIsSingleAndExplicitCast_ThenCreatingShouldSucceed()
            {
                float singleValue = 700;
                Money money = (Money)singleValue;

                money.Currency.Should().Be(currentCurrency);
                money.Amount.Should().Be(700);
            }

            [TestMethod]
            public void WhenValueIsDoubleAndExplicitCast_ThenCreatingShouldSucceed()
            {
                Money money = (Money)25.00;

                money.Currency.Should().Be(currentCurrency);
                money.Amount.Should().Be(25.00m);
            }

            [TestMethod]
            public void WhenValueIsDecimal_ThenCreatingShouldSucceed()
            {
                Money money = 25.00m;

                money.Currency.Should().Be(currentCurrency);
                money.Amount.Should().Be(25.00m);
            }
        }

        [TestClass]
        public class GivenIWantMoneyFromANumericTypeAndAnExplicitIsoCurrencyCode
        {
            private Currency euro = Currency.FromCode("EUR");

            [TestMethod]
            public void WhenValueIsByte_ThenCreatingShouldSucceed()
            {
                byte byteValue = 50;
                var money = new Money(byteValue, "EUR");
                Assert.AreEqual(money.Currency, euro);
                Assert.AreEqual(50, money.Amount);
            }

            [TestMethod]
            public void WhenValueIsSbyte_ThenCreatingShouldSucceed()
            {
                sbyte sByteValue = 75;
                var money = new Money(sByteValue, "EUR");
                Assert.AreEqual(money.Currency, euro);
                Assert.AreEqual(75, money.Amount);
            }

            [TestMethod]
            public void WhenValueIsInt16_ThenCreatingShouldSucceed()
            {
                short int16Value = 100;
                var money = new Money(int16Value, "EUR");
                Assert.AreEqual(money.Currency, euro);
                Assert.AreEqual(100, money.Amount);
            }

            [TestMethod]
            public void WhenValueIsInt32_ThenCreatingShouldSucceed()
            {
                int int32Value = 200;
                var money = new Money(int32Value, "EUR");
                Assert.AreEqual(money.Currency, euro);
                Assert.AreEqual(200, money.Amount);
            }

            [TestMethod]
            public void WhenValueIsInt64_ThenCreatingShouldSucceed()
            {
                long int64Value = 300;
                var money = new Money(int64Value, "EUR");
                Assert.AreEqual(money.Currency, euro);
                Assert.AreEqual(300, money.Amount);
            }

            [TestMethod]
            public void WhenValueIsUint16_ThenCreatingShouldSucceed()
            {
                ushort uInt16Value = 400;
                var money = new Money(uInt16Value, "EUR");
                Assert.AreEqual(money.Currency, euro);
                Assert.AreEqual(400, money.Amount);
            }

            [TestMethod]
            public void WhenValueIsUint32_ThenCreatingShouldSucceed()
            {
                uint uInt32Value = 500;
                var money = new Money(uInt32Value, "EUR");
                Assert.AreEqual(money.Currency, euro);
                Assert.AreEqual(500, money.Amount);
            }

            [TestMethod]
            public void WhenValueIsUint64_ThenCreatingShouldSucceed()
            {
                ulong uInt64Value = 600;
                var money = new Money(uInt64Value, "EUR");
                Assert.AreEqual(money.Currency, euro);
                Assert.AreEqual(600, money.Amount);
            }

            [TestMethod]
            public void WhenValueIsSingle_ThenCreatingShouldSucceed()
            {
                float singleValue = 700;
                var money = new Money(singleValue, "EUR");
                Assert.AreEqual(money.Currency, euro);
                Assert.AreEqual(700, money.Amount);
            }

            [TestMethod]
            public void WhenValueIsDouble_ThenCreatingShouldSucceed()
            {
                double doubleValue = 800;
                var money = new Money(doubleValue, "EUR");
                Assert.AreEqual(money.Currency, euro);
                Assert.AreEqual(800, money.Amount);
            }

            [TestMethod]
            public void WhenValueIsDecimal_ThenCreatingShouldSucceed()
            {
                decimal decimalValue = 900;
                var money = new Money(decimalValue, "EUR");
                Assert.AreEqual(money.Currency, euro);
                Assert.AreEqual(900, money.Amount);
            }

            [TestMethod]
            [ExpectedException(typeof(ArgumentException))]
            public void WhenUnknownIsoCurrencySymbol_ThenCreatingShouldFail()
            {
                var money = new Money(123.25M, "XYZ");
            }
        }

        [TestClass]
        public class GivenIWantMoneyInOneOfTheFourMostUsedCurrenciesInTheWorld
        {
            [TestMethod]
            public void WhenEurosIsDecimal_ThenCreatingShouldSucceed()
            {
                //from decimal
                Money euros = Money.Euro(10.00m);
                Assert.AreEqual(euros.Currency, Currency.FromCode("EUR"));
                Assert.AreEqual(euros.Amount, 10.00m);
            }

            [TestMethod]
            public void WhenEurosIsDouble_ThenCreatingShouldSucceed()
            {
                //from double (float is implicitly converted to double)
                Money euros1 = Money.Euro(10.00);
                Assert.AreEqual(euros1.Currency, Currency.FromCode("EUR"));
                Assert.AreEqual(euros1.Amount, 10.00m);
            }

            [TestMethod]
            public void WhenEurosIsLong_ThenCreatingShouldSucceed()
            {
                //from long (byte, short and int are implicitly converted to long)
                Money money2 = Money.Euro(10L);
                Assert.AreEqual(money2.Currency, Currency.FromCode("EUR"));
                Assert.AreEqual(money2.Amount, 10.00m);
            }

            [TestMethod]
            public void WhenDollarsIsDecimal_ThenCreatingShouldSucceed()
            {
                //from decimal (other integral types are implicitly converted to decimal)
                Money dollars = Money.USDollar(10.00m);
                Assert.AreEqual(dollars.Currency, Currency.FromCode("USD"));
                Assert.AreEqual(dollars.Amount, 10.00m);
            }

            [TestMethod]
            public void WhenDollarsIsDouble_ThenCreatingShouldSucceed()
            {
                //from double (float is implicitly converted to double)
                Money dollars1 = Money.USDollar(10.00);
                Assert.AreEqual(dollars1.Currency, Currency.FromCode("USD"));
                Assert.AreEqual(dollars1.Amount, 10.00m);
            }

            [TestMethod]
            public void WhenDollarsIsLong_ThenCreatingShouldSucceed()
            {
                //from long (byte, short and int are implicitly converted to long)
                Money money2 = Money.USDollar(10L);
                Assert.AreEqual(money2.Currency, Currency.FromCode("USD"));
                Assert.AreEqual(money2.Amount, 10.00m);
            }

            [TestMethod]
            public void WhenYensIsDecimal_ThenCreatingShouldSucceed()
            {
                //from decimal (other integral types are implicitly converted to decimal)
                Money yens = Money.Yen(10.00m);
                Assert.IsNotNull(yens);
                Assert.AreEqual(yens.Currency, Currency.FromCode("JPY"));
                Assert.AreEqual(yens.Amount, 10.00m);
            }

            [TestMethod]
            public void WhenYensIsDouble_ThenCreatingShouldSucceed()
            {
                //from double (float is implicitly converted to double)
                Money yens1 = Money.Yen(10.00);
                Assert.IsNotNull(yens1);
                Assert.AreEqual(yens1.Currency, Currency.FromCode("JPY"));
                Assert.AreEqual(yens1.Amount, 10.00m);
            }

            [TestMethod]
            public void WhenYensIsLong_ThenCreatingShouldSucceed()
            {
                //from long (byte, short and int are implicitly converted to long)
                Money money2 = Money.Yen(10L);
                Assert.IsNotNull(money2);
                Assert.AreEqual(money2.Currency, Currency.FromCode("JPY"));
                Assert.AreEqual(money2.Amount, 10.00m);
            }

            [TestMethod]
            public void WhenPondsIsDecimal_ThenCreatingShouldSucceed()
            {
                //from decimal (other integral types are implicitly converted to decimal)
                Money pounds = Money.PoundSterling(10.00m);
                Assert.IsNotNull(pounds);
                Assert.AreEqual(pounds.Currency, Currency.FromCode("GBP"));
                Assert.AreEqual(pounds.Amount, 10.00m);
            }

            [TestMethod]
            public void WhenPondsIsDouble_ThenCreatingShouldSucceed()
            {
                //from double (float is implicitly converted to double)
                Money pounds1 = Money.PoundSterling(10.00);
                Assert.IsNotNull(pounds1);
                Assert.AreEqual(pounds1.Currency, Currency.FromCode("GBP"));
                Assert.AreEqual(pounds1.Amount, 10.00m);
            }

            [TestMethod]
            public void WhenPondsIsLong_ThenCreatingShouldSucceed()
            {
                //from long (byte, short and int are implicitly converted to long)
                Money money2 = Money.PoundSterling(10L);
                Assert.IsNotNull(money2);
                Assert.AreEqual(money2.Currency, Currency.FromCode("GBP"));
                Assert.AreEqual(money2.Amount, 10.00m);
            }
        }

        [TestClass]
        public class GivenIWantToConvertMoneyToString
        {
            private Money yen = new Money(765.4321m, Currency.FromCode("JPY"));
            private Money euro = new Money(765.4321m, Currency.FromCode("EUR"));
            private Money dollar = new Money(765.4321m, Currency.FromCode("USD"));
            private Money dinar = new Money(765.4321m, Currency.FromCode("BHD"));

            [TestMethod]
            public void WhenImplicitConversion_ThenNumberOfDecimalsShouldBeDefaultOfCurrency()
            {
                using (new SwitchCulture("en-US"))
                {
                    yen.ToString().Should().Be("¥765");
                    euro.ToString().Should().Be("€765.43");
                    dollar.ToString().Should().Be("$765.43");
                    dinar.ToString().Should().Be("BD765.432");
                }
            }

            [TestMethod]
            public void WhenExplicitToZeroDecimals_ThenThisShouldSucceed()
            {
                using (new SwitchCulture("en-US"))
                {
                    yen.ToString("C0").Should().Be("¥765");
                    euro.ToString("C0").Should().Be("€765");
                    dollar.ToString("C0").Should().Be("$765");
                    dinar.ToString("C0").Should().Be("BD765");
                }
            }

            [TestMethod]
            public void WhenExplicitToOneDecimals_ThenThisShouldSucceed()
            {
                using (new SwitchCulture("en-US"))
                {
                    yen.ToString("C1").Should().Be("¥765.0");
                    euro.ToString("C1").Should().Be("€765.4");
                    dollar.ToString("C1").Should().Be("$765.4");
                    dinar.ToString("C1").Should().Be("BD765.4");
                }
            }

            [TestMethod]
            public void WhenExplicitToTwoDecimals_ThenThisShouldSucceed()
            {
                using (new SwitchCulture("en-US"))
                {
                    yen.ToString("C2").Should().Be("¥765.00");
                    euro.ToString("C2").Should().Be("€765.43");
                    dollar.ToString("C2").Should().Be("$765.43");
                    dinar.ToString("C2").Should().Be("BD765.43");
                }
            }

            [TestMethod]
            public void WhenExplicitToThreeDecimals_ThenThisShouldSucceed()
            {
                using (new SwitchCulture("en-US"))
                {
                    yen.ToString("C3").Should().Be("¥765.000");
                    euro.ToString("C3").Should().Be("€765.430");
                    dollar.ToString("C3").Should().Be("$765.430");
                    dinar.ToString("C3").Should().Be("BD765.432");
                }
            }

            [TestMethod]
            public void WhenExplicitToFourDecimals_ThenThisShouldSucceed()
            {
                using (new SwitchCulture("en-US"))
                {
                    yen.ToString("C4").Should().Be("¥765.0000");
                    euro.ToString("C4").Should().Be("€765.4300");
                    dollar.ToString("C4").Should().Be("$765.4300");
                    dinar.ToString("C4").Should().Be("BD765.4320");
                }
            }

            [TestMethod]
            public void WhenSpecificCultureIsUsed_ThenCurrencySymbolAndDecimalsOfMoneyShouldStillBeLeading()
            {
                using (new SwitchCulture("en-US"))
                {
                    var ci = new CultureInfo("nl-NL");

                    yen.ToString(ci).Should().Be("¥ 765");
                    euro.ToString(ci).Should().Be("€ 765,43");
                    dollar.ToString(ci).Should().Be("$ 765,43");
                    dinar.ToString(ci).Should().Be("BD 765,432");
                }
            }

            [TestMethod]
            public void WhenSpecificNumberFormatIsUsed_ThenCurrencySymbolAndDecimalsOfMoneyShouldStillBeLeading()
            {
                using (new SwitchCulture("en-US"))
                {
                    var nfi = new CultureInfo("nl-NL").NumberFormat;

                    yen.ToString(nfi).Should().Be("¥ 765");
                    euro.ToString(nfi).Should().Be("€ 765,43");
                    dollar.ToString(nfi).Should().Be("$ 765,43");
                    dinar.ToString(nfi).Should().Be("BD 765,432");
                }
            }

            [TestMethod]
            public void WhenShowingMoneyInBelgiumDutchSpeaking_ThenThisShouldSucceed()
            {
                using (new SwitchCulture("nl-BE"))
                {
                    yen.ToString().Should().Be("¥ 765");
                    euro.ToString().Should().Be("€ 765,43");
                    dollar.ToString().Should().Be("$ 765,43");
                    dinar.ToString().Should().Be("BD 765,432");
                }
            }

            [TestMethod]
            public void WhenShowingMoneyInBelgiumFrenchSpeaking_ThenThisShouldSucceed()
            {
                using (new SwitchCulture("fr-BE"))
                {
                    yen.ToString().Should().Be("765 ¥");
                    euro.ToString().Should().Be("765,43 €");
                    dollar.ToString().Should().Be("765,43 $");
                    dinar.ToString().Should().Be("765,432 BD");
                }
            }
        }

        [TestClass]
        public class GivenIWantToIncrementMoney
        {
            private Money yen = new Money(765m, Currency.FromCode("JPY"));
            private Money euro = new Money(765.43m, Currency.FromCode("EUR"));
            private Money dollar = new Money(765.43m, Currency.FromCode("USD"));
            private Money dinar = new Money(765.432m, Currency.FromCode("BHD"));

            [TestMethod]
            public void WhenIncrementing_ThenAmountShouldIncrementWithMinorUnit()
            {
                var y = ++yen;
                var e = ++euro;
                var d = ++dollar;
                var di = ++dinar;

                y.Amount.Should().Be(766m);
                y.Currency.Should().Be(yen.Currency);
                e.Amount.Should().Be(765.44m);
                e.Currency.Should().Be(euro.Currency);
                d.Amount.Should().Be(765.44m);
                d.Currency.Should().Be(dollar.Currency);
                di.Amount.Should().Be(765.433m);
                di.Currency.Should().Be(dinar.Currency);
            }
        }

        [TestMethod]
        public void ShouldConvertToDecimal()
        {
            Money euro = new Money(10.999m, "EUR");
            Assert.AreEqual(Money.ToDecimal(euro), 11.00m);
        }

        [TestMethod]
        public void ShouldConvertToDouble()
        {
            Money euro = new Money(10.999m, "EUR");
            Assert.AreEqual(Money.ToDouble(euro), 11.00d);
        }

        [TestMethod]
        public void ShouldMultiplyCorrectly()
        {
            Money money1 = new Money(100.12);
            Assert.AreEqual(new Money(50.06m), money1 * 0.5m); // decimal
            //Assert.AreEqual(new Money(50.0625m), money1 * 0.5); // double
            Assert.AreEqual(new Money(500.60m), money1 * 5); // int

            Assert.AreEqual(new Money(50.06m), Money.Multiply(money1, 0.5m));
            //Assert.AreEqual(new Money(50.0625m), Money.Multiply(money1, 0.5));
            Assert.AreEqual(new Money(500.60m), Money.Multiply(money1, 5));
            
            Money money2 = new Money(-100.12);
            Assert.AreEqual(new Money(-50.06m), money2 * 0.5m);
            //Assert.AreEqual(new Money(-50.0625m), money2 * 0.5);
            Assert.AreEqual(new Money(-500.60m), money2 * 5);
            
            // multiplier first
            Assert.AreEqual(new Money(-50.06m), 0.5m * money2);
            //Assert.AreEqual(new Money(-50.0625m), 0.5 * money2);
            Assert.AreEqual(new Money(-500.60m), 5 * money2);

            Money money3 = new Money(15);
            Money money4 = new Money(100);
            Assert.AreEqual(new Money(150), money3 * 10);
            //Assert.AreEqual(new Money(1.5), money3 * 0.1);
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
            Money money1 = new Money(100.12);
            Assert.AreEqual(new Money(50.06m), money1 / 2); // int
            Assert.AreEqual(new Money(200.24m), money1 / 0.5m); // decimal
            //Assert.AreEqual(new Money(200.25m), money1 / 0.5); // double

            Assert.AreEqual(new Money(50.06m), Money.Divide(money1, 2));
            Assert.AreEqual(new Money(200.24m), Money.Divide(money1, 0.5m));
            //Assert.AreEqual(new Money(200.25m), Money.Divide(money1, 0.5));

            Money money3 = new Money(-100.12);
            Assert.AreEqual(new Money(-50.06m), money3 / 2);
            Assert.AreEqual(new Money(-200.24m), money3 / 0.5m);
            //Assert.AreEqual(new Money(-200.25m), money3 / 0.5);

            Money money4 = new Money(100);
            Assert.AreEqual(new Money(33.33), money4 / 3);
            Assert.AreEqual(new Money(16.67), money4 / 6);

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

        //[TestMethod]
        //public void ShouldIncrement()
        //{
        //    Money euros = Money.Euro(10);
        //    Assert.AreEqual(Money.Euro(10.01m), ++euros);

        //    Money yens = Money.Yen(10.00m);
        //    Assert.AreEqual(Money.Yen(11), ++yens);
        //}

        //[TestMethod]
        //public void ShouldDecrement()
        //{
        //    Money euros = Money.Euro(10);
        //    Assert.AreEqual(Money.Euro(9.99m), --euros);

        //    Money yens = Money.Yen(10.00m);
        //    Assert.AreEqual(Money.Yen(9), --yens);
        //}

        //public void testMinimumIncrement()
        //{
        //    assertEquals(Money.valueOf(0.01, USD), d100.minimumIncrement());
        //    assertEquals(Money.valueOf(1, JPY), y50.minimumIncrement());
        //}

        //public void testCloseNumbersNotEqual()
        //{
        //    Money d2_51a = Money.dollars(2.515);
        //    Money d2_51b = Money.dollars(2.5149);
        //    assertTrue(!d2_51a.equals(d2_51b));
        //}

        //public void setUp()
        //{
        //    d15 = Money.valueOf(new BigDecimal("15.0"), USD);
        //    d2_51 = Money.valueOf(new BigDecimal("2.51"), USD);
        //    e2_51 = Money.valueOf(new BigDecimal("2.51"), EUR);
        //    y50 = Money.valueOf(new BigDecimal("50"), JPY);
        //    d100 = Money.valueOf(new BigDecimal("100.0"), USD);
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

