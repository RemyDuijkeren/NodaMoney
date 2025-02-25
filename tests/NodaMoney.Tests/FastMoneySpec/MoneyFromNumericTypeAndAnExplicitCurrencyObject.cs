namespace NodaMoney.Tests.FastMoneySpec;

public class MoneyFromNumericTypeAndAnExplicitCurrencyObject
{
    private readonly Currency _euro = CurrencyInfo.FromCode("EUR");

    [Fact]
    public void WhenValueIsByte_ThenCreatingShouldSucceed()
    {
        const byte byteValue = 50;
        var money = new FastMoney(byteValue, _euro);

        money.Currency.Should().Be(_euro);
        money.Amount.Should().Be(50m);
    }

    [Fact]
    public void WhenValueIsSbyte_ThenCreatingShouldSucceed()
    {
        const sbyte sbyteValue = 75;
        var money = new FastMoney(sbyteValue, _euro);

        money.Currency.Should().Be(_euro);
        money.Amount.Should().Be(75m);
    }

    [Fact]
    public void WhenValueIsInt16_ThenCreatingShouldSucceed()
    {
        const short int16Value = 100;
        var money = new FastMoney(int16Value, _euro);

        money.Currency.Should().Be(_euro);
        money.Amount.Should().Be(100m);
    }

    [Fact]
    public void WhenValueIsInt32_ThenCreatingShouldSucceed()
    {
        const int int32Value = 200;
        var money = new FastMoney(int32Value, _euro);

        money.Currency.Should().Be(_euro);
        money.Amount.Should().Be(200m);
    }

    [Fact]
    public void WhenValueIsInt64_ThenCreatingShouldSucceed()
    {
        const long int64Value = 300;
        var money = new FastMoney(int64Value, _euro);

        money.Currency.Should().Be(_euro);
        money.Amount.Should().Be(300m);
    }

    [Fact]
    public void WhenValueIsUint16_ThenCreatingShouldSucceed()
    {
        const ushort uInt16Value = 400;
        var money = new FastMoney(uInt16Value, _euro);

        money.Currency.Should().Be(_euro);
        money.Amount.Should().Be(400m);
    }

    [Fact]
    public void WhenValueIsUint32_ThenCreatingShouldSucceed()
    {
        const uint uInt32Value = 500;
        var money = new FastMoney(uInt32Value, _euro);

        money.Currency.Should().Be(_euro);
        money.Amount.Should().Be(500m);
    }

    [Fact]
    public void WhenValueIsUint64_ThenCreatingShouldSucceed()
    {
        const ulong uInt64Value = 600;
        var money = new FastMoney(uInt64Value, _euro);

        money.Currency.Should().Be(_euro);
        money.Amount.Should().Be(600m);
    }

    [Fact]
    public void WhenValueIsSingle_ThenCreatingShouldSucceed()
    {
        const float singleValue = 700;
        var money = new FastMoney(singleValue, _euro);

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
    //[InlineData(7922816251426433.7593543950335, 7922816251426433.76)] // OverflowException
    //[InlineData(79228162514264337593543.950335, 79228162514264337593543.95)] // OverflowException
    [InlineData(0.0079228162514264337593543950335, 0.01)]
    public void WhenValueIsDecimal_ThenCreatingShouldSucceed(decimal input, decimal expected)
    {
        var money = new FastMoney(input, _euro);

        money.Currency.Should().Be(_euro);
        money.Amount.Should().Be(expected);
    }
}
