using NodaMoney.Tests.Helpers;

namespace NodaMoney.Tests.ExactMoneySpec;

[Collection(nameof(NoParallelization))]
public class MoneyImplicit
{
    private readonly decimal _decimalValue = 1234.567m;

    [Fact]
    [UseCulture("en-US")]
    public void WhenCurrentCultureIsUS_ThenCurrencyIsDollar()
    {
        var money = new ExactMoney(_decimalValue);

        money.Currency.Should().Be(Currency.FromCode("USD"));
        money.Amount.Should().Be(_decimalValue);
    }

    [Fact]
    [UseCulture("nl-NL")]
    public void WhenCurrentCultureIsNL_ThenCurrencyIsEuro()
    {
        var money = new ExactMoney(_decimalValue);

        money.Currency.Should().Be(Currency.FromCode("EUR"));
        money.Amount.Should().Be(_decimalValue);
    }

    [Fact]
    [UseCulture("ja-JP")]
    public void WhenCurrentCultureIsJP_ThenCurrencyIsYen()
    {
        var money = new ExactMoney(_decimalValue);

        money.Currency.Should().Be(Currency.FromCode("JPY"));
        money.Amount.Should().Be(_decimalValue);
    }

    [Fact]
    [UseCulture(null)]
    public void WhenCurrentCultureIsInvariant_ThenCurrencyIsDefault()
    {
        var money = new ExactMoney(_decimalValue);

        money.Currency.Should().Be(default(Currency));
        money.Amount.Should().Be(_decimalValue);
    }
}
