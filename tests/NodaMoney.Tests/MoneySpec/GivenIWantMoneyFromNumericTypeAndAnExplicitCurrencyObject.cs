using FluentAssertions;
using Xunit;

namespace NodaMoney.Tests.MoneySpec;

public class GivenIWantMoneyFromNumericTypeAndAnExplicitCurrencyObject
{
    private readonly Currency _euro = CurrencyInfo.FromCode("EUR");

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
