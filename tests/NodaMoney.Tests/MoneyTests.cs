using System;
using System.Globalization;
using System.Threading;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NodaMoney.Tests.Helpers;

namespace NodaMoney.Tests
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
        public class GivenIWantToConvertDoubleToDecimal
        {
            private readonly Currency _euro = Currency.FromCode("EUR");

            [TestMethod]
            public void WhenValueIsNormalValue_ThenCreatingShouldSucceed()
            {
                double value1 = 7922816251426433.7593543950335;
                decimal value2 = new Decimal(value1);
                decimal value3 = (decimal)7922816251426433.7593543950335;
                decimal value4 = 7922816251426433.7593543950335m;
                
                string result1 = value1.ToString(CultureInfo.InvariantCulture);
                string result2 = value2.ToString(CultureInfo.InvariantCulture);
                string result3 = value3.ToString(CultureInfo.InvariantCulture);
                string result4 = value4.ToString(CultureInfo.InvariantCulture);

                Console.WriteLine(result1);
                Console.WriteLine(result2);
                Console.WriteLine(result3);
                Console.WriteLine(result4);
            }

            [TestMethod]
            public void WhenValueIsVeryBigValue_ThenCreatingShouldSucceed()
            {
                double value1 = 79228162514264337593543.950335;
                decimal value2 = new Decimal(value1);
                decimal value3 = (decimal)79228162514264337593543.950335;
                decimal value4 = 79228162514264337593543.950335m;

                string result1 = value1.ToString(CultureInfo.InvariantCulture);
                string result2 = value2.ToString(CultureInfo.InvariantCulture);
                string result3 = value3.ToString(CultureInfo.InvariantCulture);
                string result4 = value4.ToString(CultureInfo.InvariantCulture);

                Console.WriteLine(result1);
                Console.WriteLine(result2);
                Console.WriteLine(result3);
                Console.WriteLine(result4);
            }

            [TestMethod]
            public void WhenValueIsVerySmall_ThenCreatingShouldSucceed()
            {
                double value1 = 0.0079228162514264337593543950335;
                decimal value2 = new Decimal(value1);
                decimal value3 = (decimal)0.0079228162514264337593543950335;
                decimal value4 = 0.0079228162514264337593543950335m;

                string result1 = value1.ToString(CultureInfo.InvariantCulture);
                string result2 = value2.ToString(CultureInfo.InvariantCulture);
                string result3 = value3.ToString(CultureInfo.InvariantCulture);
                string result4 = value4.ToString(CultureInfo.InvariantCulture);

                Console.WriteLine(result1);
                Console.WriteLine(result2);
                Console.WriteLine(result3);
                Console.WriteLine(result4);
            }
        }      
    }
}
