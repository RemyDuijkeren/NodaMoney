using NodaMoney.Tests.Helpers;

namespace NodaMoney.Tests.MoneyConvertibleSpec;

[Collection(nameof(NoParallelization))]
public class CastNumericTypeToMoneyWithImplicitCurrencyFromTheCurrentCulture
{
    private readonly Currency _euro = CurrencyInfo.FromCode("EUR");

    [Fact]
    [UseCulture("nl-NL")]
    public void WhenValueIsByte_ThenCreatingShouldSucceed()
    {
            const byte byteValue = 50;
            Money money = (Money)byteValue;

            money.Currency.Should().Be(_euro);
            money.Amount.Should().Be(50);
        }

    [Fact]
    [UseCulture("nl-NL")]
    public void WhenValueIsSbyte_ThenCreatingShouldSucceed()
    {
            const sbyte sbyteValue = 75;
            Money money = (Money)sbyteValue;

            money.Currency.Should().Be(_euro);
            money.Amount.Should().Be(75);
        }

    [Fact]
    [UseCulture("nl-NL")]
    public void WhenValueIsInt16_ThenCreatingShouldSucceed()
    {
            const short int16Value = 100;
            Money money = (Money)int16Value;

            money.Currency.Should().Be(_euro);
            money.Amount.Should().Be(100);
        }

    [Fact]
    [UseCulture("nl-NL")]
    public void WhenValueIsInt32_ThenCreatingShouldSucceed()
    {
            const int int32Value = 200;
            Money money = (Money)int32Value;

            money.Currency.Should().Be(_euro);
            money.Amount.Should().Be(200);
        }

    [Fact]
    [UseCulture("nl-NL")]
    public void WhenValueIsInt64_ThenCreatingShouldSucceed()
    {
            const long int64Value = 300;
            Money money = (Money)int64Value;

            money.Currency.Should().Be(_euro);
            money.Amount.Should().Be(300);
        }

    [Fact]
    [UseCulture("nl-NL")]
    public void WhenValueIsUint16_ThenCreatingShouldSucceed()
    {
            const ushort uInt16Value = 400;
            Money money = (Money)uInt16Value;

            money.Currency.Should().Be(_euro);
            money.Amount.Should().Be(400);
        }

    [Fact]
    [UseCulture("nl-NL")]
    public void WhenValueIsUint32_ThenCreatingShouldSucceed()
    {
            const uint uInt32Value = 500;
            Money money = (Money)uInt32Value;

            money.Currency.Should().Be(_euro);
            money.Amount.Should().Be(500);
        }

    [Fact]
    [UseCulture("nl-NL")]
    public void WhenValueIsUint64_ThenCreatingShouldSucceed()
    {
            const ulong uInt64Value = 600;
            Money money = (Money)uInt64Value;

            money.Currency.Should().Be(_euro);
            money.Amount.Should().Be(600);
        }

    [Fact]
    [UseCulture("nl-NL")]
    public void WhenValueIsSingle_ThenCreatingShouldSucceed()
    {
            const float singleValue = 700;
            var money = (Money)singleValue;

            money.Currency.Should().Be(_euro);
            money.Amount.Should().Be(700);
        }

    [Fact]
    [UseCulture("nl-NL")]
    public void WhenValueIsDoubl_ThenCreatingShouldSucceed()
    {
            var money = (Money)25.00;

            money.Currency.Should().Be(_euro);
            money.Amount.Should().Be(25.00m);
        }

    [Fact]
    [UseCulture("nl-NL")]
    public void WhenValueIsDecimal_ThenCreatingShouldSucceed()
    {
            Money money = (Money)25.00m;

            money.Currency.Should().Be(_euro);
            money.Amount.Should().Be(25.00m);
        }
}
