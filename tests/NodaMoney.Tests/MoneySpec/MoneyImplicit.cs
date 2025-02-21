using NodaMoney.Tests.Helpers;

namespace NodaMoney.Tests.MoneySpec;

[Collection(nameof(NoParallelization))]
public class MoneyImplicit
{
    private readonly decimal _decimalValue = 1234.567m;

    [Fact]
    [UseCulture("en-US")]
    public void WhenCurrentCultureIsUS_ThenCurrencyIsDollar()
    {
        var money = new Money(_decimalValue);

        money.Currency.Should().Be(Currency.FromCode("USD"));
        money.Amount.Should().Be(1234.57m);
    }

    [Fact]
    [UseCulture("nl-NL")]
    public void WhenCurrentCultureIsNL_ThenCurrencyIsEuro()
    {
        var money = new Money(_decimalValue);

        money.Currency.Should().Be(Currency.FromCode("EUR"));
        money.Amount.Should().Be(1234.57m);
    }

    [Fact]
    [UseCulture("ja-JP")]
    public void WhenCurrentCultureIsJP_ThenCurrencyIsYen()
    {
        var money = new Money(_decimalValue);

        money.Currency.Should().Be(Currency.FromCode("JPY"));
        money.Amount.Should().Be(1235m);
    }

    [Fact]
    [UseCulture(null)]
    public void WhenCurrentCultureIsInvariant_ThenCurrencyIsDefault()
    {
        var money = new Money(_decimalValue);

        money.Currency.Should().Be(default(Currency));
        money.Amount.Should().Be(1235m);
    }
}
