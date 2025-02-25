namespace NodaMoney.Tests.FastMoneySpec;

public class MoneyFromNumericTypeAndAnExplicitIsoCurrencyCode
{
    private readonly Currency _euro = Currency.FromCode("EUR");

    [Fact]
    public void WhenValueIsByte_ThenCreatingShouldSucceed()
    {
        const byte byteValue = 50;
        var money = new FastMoney(byteValue, "EUR");

        money.Currency.Should().Be(_euro);
        money.Amount.Should().Be(50);
    }

    [Fact]
    public void WhenValueIsSbyte_ThenCreatingShouldSucceed()
    {
        const sbyte sbyteValue = 75;
        var money = new FastMoney(sbyteValue, "EUR");

        money.Currency.Should().Be(_euro);
        money.Amount.Should().Be(75);
    }

    [Fact]
    public void WhenValueIsInt16_ThenCreatingShouldSucceed()
    {
        const short int16Value = 100;
        var money = new FastMoney(int16Value, "EUR");

        money.Currency.Should().Be(_euro);
        money.Amount.Should().Be(100);
    }

    [Fact]
    public void WhenValueIsInt32_ThenCreatingShouldSucceed()
    {
        const int int32Value = 200;
        var money = new FastMoney(int32Value, "EUR");

        money.Currency.Should().Be(_euro);
        money.Amount.Should().Be(200);
    }

    [Fact]
    public void WhenValueIsInt64_ThenCreatingShouldSucceed()
    {
        const long int64Value = 300;
        var money = new FastMoney(int64Value, "EUR");

        money.Currency.Should().Be(_euro);
        money.Amount.Should().Be(300);
    }

    [Fact]
    public void WhenValueIsUint16_ThenCreatingShouldSucceed()
    {
        const ushort uInt16Value = 400;
        var money = new FastMoney(uInt16Value, "EUR");

        money.Currency.Should().Be(_euro);
        money.Amount.Should().Be(400);
    }

    [Fact]
    public void WhenValueIsUint32_ThenCreatingShouldSucceed()
    {
        const uint uInt32Value = 500;
        var money = new FastMoney(uInt32Value, "EUR");

        money.Currency.Should().Be(_euro);
        money.Amount.Should().Be(500);
    }

    [Fact]
    public void WhenValueIsUint64_ThenCreatingShouldSucceed()
    {
        const ulong uInt64Value = 600;
        var money = new FastMoney(uInt64Value, "EUR");

        money.Currency.Should().Be(_euro);
        money.Amount.Should().Be(600);
    }

    [Fact]
    public void WhenValueIsSingle_ThenCreatingShouldSucceed()
    {
        const float singleValue = 700;
        var money = new FastMoney(singleValue, "EUR");

        money.Currency.Should().Be(_euro);
        money.Amount.Should().Be(700);
    }

    [Fact]
    public void WhenValueIsDouble_ThenCreatingShouldSucceed()
    {
        const double doubleValue = 800;
        var money = new FastMoney(doubleValue, "EUR");

        money.Currency.Should().Be(_euro);
        money.Amount.Should().Be(800);
    }

    [Fact]
    public void WhenValueIsDecimal_ThenCreatingShouldSucceed()
    {
        const decimal decimalValue = 900;
        var money = new FastMoney(decimalValue, "EUR");

        money.Currency.Should().Be(_euro);
        money.Amount.Should().Be(900);
    }

    [Fact]
    public void WhenUnknownIsoCurrencySymbol_ThenThrowException()
    {
        Action action = () => new FastMoney(123.25M, "XYZ");

        action.Should().Throw<InvalidCurrencyException>();
    }
}
