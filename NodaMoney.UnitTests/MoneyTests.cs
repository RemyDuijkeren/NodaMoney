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
