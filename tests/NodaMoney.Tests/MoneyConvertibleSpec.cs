using System.Threading;

using FluentAssertions;
using Xunit;

namespace NodaMoney.Tests.MoneyConvertibleSpec
{
    public class GivenIWantToConvertMoney
    {
        readonly Money _euros = new Money(765.43m, "EUR");

        [Fact]
        public void WhenConvertingToDecimal_ThenThisShouldSucceed()
        {
            var result = Money.ToDecimal(_euros);

            result.Should().Be(765.43m);
        }

        [Fact]
        public void WhenConvertingToDouble_ThenThisShouldSucceed()
        {
            var result = Money.ToDouble(_euros);

            result.Should().BeApproximately(765.43d, 0.001d);
        }

        [Fact]
        public void WhenConvertingToSingle_ThenThisShouldSucceed()
        {
            var result = Money.ToSingle(_euros);

            result.Should().BeApproximately(765.43f, 0.001f);
        }
    }

    public class GivenIWantToExplicitCastMoneyToNumericType
    {
        readonly Money _euros = new Money(10.00m, "EUR");

        [Fact]
        public void WhenExplicitCastingToDecimal_ThenCastingShouldSucceed()
        {
            var m = (decimal)_euros;

            m.Should().Be(10.00m);
        }

        [Fact]
        public void WhenExplicitCastingToDouble_ThenCastingShouldSucceed()
        {
            var d = (double)_euros;

            d.Should().Be(10.00d);
        }

        [Fact]
        public void WhenExplicitCastingToFloat_ThenCastingShouldSucceed()
        {
            var f = (float)_euros;

            f.Should().Be(10.00f);
        }

        [Fact]
        public void WhenExplicitCastingToLong_ThenCastingShouldSucceed()
        {
            var l = (long)_euros;

            l.Should().Be(10L);
        }

        [Fact]
        public void WhenExplicitCastingToByte_ThenCastingShouldSucceed()
        {
            var b = (byte)_euros;

            b.Should().Be(10);
        }

        [Fact]
        public void WhenExplicitCastingToShort_ThenCastingShouldSucceed()
        {
            var s = (short)_euros;

            s.Should().Be(10);
        }

        [Fact]
        public void WhenExplicitCastingToInt_ThenCastingShouldSucceed()
        {
            var i = (int)_euros;

            i.Should().Be(10);
        }
    }

    public class GivenIWantToCastNumericTypeToMoneyWithImplicitCurrencyFromTheCurrentCulture
    {
        private readonly Currency _currentCurrency = Currency.FromCulture(Thread.CurrentThread.CurrentCulture);

        [Fact]
        public void WhenValueIsByte_ThenCreatingShouldSucceed()
        {
            const byte byteValue = 50;
            Money money = byteValue;

            money.Currency.Should().Be(_currentCurrency);
            money.Amount.Should().Be(50);
        }

        [Fact]
        public void WhenValueIsSbyte_ThenCreatingShouldSucceed()
        {
            const sbyte sbyteValue = 75;
            Money money = sbyteValue;

            money.Currency.Should().Be(_currentCurrency);
            money.Amount.Should().Be(75);
        }

        [Fact]
        public void WhenValueIsInt16_ThenCreatingShouldSucceed()
        {
            const short int16Value = 100;
            Money money = int16Value;

            money.Currency.Should().Be(_currentCurrency);
            money.Amount.Should().Be(100);
        }

        [Fact]
        public void WhenValueIsInt32_ThenCreatingShouldSucceed()
        {
            const int int32Value = 200;
            Money money = int32Value;

            money.Currency.Should().Be(_currentCurrency);
            money.Amount.Should().Be(200);
        }

        [Fact]
        public void WhenValueIsInt64_ThenCreatingShouldSucceed()
        {
            const long int64Value = 300;
            Money money = int64Value;

            money.Currency.Should().Be(_currentCurrency);
            money.Amount.Should().Be(300);
        }

        [Fact]
        public void WhenValueIsUint16_ThenCreatingShouldSucceed()
        {
            const ushort uInt16Value = 400;
            Money money = uInt16Value;

            money.Currency.Should().Be(_currentCurrency);
            money.Amount.Should().Be(400);
        }

        [Fact]
        public void WhenValueIsUint32_ThenCreatingShouldSucceed()
        {
            const uint uInt32Value = 500;
            Money money = uInt32Value;

            money.Currency.Should().Be(_currentCurrency);
            money.Amount.Should().Be(500);
        }

        [Fact]
        public void WhenValueIsUint64_ThenCreatingShouldSucceed()
        {
            const ulong uInt64Value = 600;
            Money money = uInt64Value;

            money.Currency.Should().Be(_currentCurrency);
            money.Amount.Should().Be(600);
        }

        [Fact]
        public void WhenValueIsSingleAndExplicitCast_ThenCreatingShouldSucceed()
        {
            const float singleValue = 700;
            var money = (Money)singleValue;

            money.Currency.Should().Be(_currentCurrency);
            money.Amount.Should().Be(700);
        }

        [Fact]
        public void WhenValueIsDoubleAndExplicitCast_ThenCreatingShouldSucceed()
        {
            var money = (Money)25.00;

            money.Currency.Should().Be(_currentCurrency);
            money.Amount.Should().Be(25.00m);
        }

        [Fact]
        public void WhenValueIsDecimal_ThenCreatingShouldSucceed()
        {
            Money money = 25.00m;

            money.Currency.Should().Be(_currentCurrency);
            money.Amount.Should().Be(25.00m);
        }
    }
}