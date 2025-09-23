using NodaMoney.Tests.Helpers;

namespace NodaMoney.Tests.FastMoneySpec;

[Collection(nameof(NoParallelization))]
public class MoneyImplicit
{
    private readonly decimal _decimalValue = 1234.56789m;

    [Fact]
    [UseCulture("en-US")]
    public void WhenCurrentCultureIsUS_ThenCurrencyIsDollar()
    {
        var money = new FastMoney(_decimalValue);

        money.Currency.Should().Be(Currency.FromCode("USD"));
        money.Amount.Should().Be(1234.5679m);
    }

    [Fact]
    [UseCulture("nl-NL")]
    public void WhenCurrentCultureIsNL_ThenCurrencyIsEuro()
    {
        var money = new FastMoney(_decimalValue);

        money.Currency.Should().Be(Currency.FromCode("EUR"));
        money.Amount.Should().Be(1234.5679m);
    }

    [Fact]
    [UseCulture("ja-JP")]
    public void WhenCurrentCultureIsJP_ThenCurrencyIsYen()
    {
        var money = new FastMoney(_decimalValue);

        money.Currency.Should().Be(Currency.FromCode("JPY"));
        money.Amount.Should().Be(1234.5679m);
    }

    [Fact]
    [UseCulture(null)]
    public void WhenCurrentCultureIsInvariant_ThenCurrencyIsDefault()
    {
        var money = new FastMoney(_decimalValue);

        money.Currency.Should().Be(default(Currency));
        money.Amount.Should().Be(1234.5679m, because: "no rounding");
    }
}
