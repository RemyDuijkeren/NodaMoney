using FluentAssertions;
using NodaMoney.Tests.Helpers;
using Xunit;

namespace NodaMoney.Tests.CurrencyInfoSpec;

[Collection(nameof(NoParallelization))]
public class GivenIWantCurrentCurrency
{
    [Fact]
    [UseCulture("en-US")]
    public void WhenCurrentCultureIsUS_ThenCurrencyIsDollar()
    {
        var currency = CurrencyInfo.CurrentCurrency;

        currency.Should().Be(CurrencyInfo.FromCode("USD"));
    }

    [Fact]
    [UseCulture("nl-NL")]
    public void WhenCurrentCultureIsNL_ThenCurrencyIsEuro()
    {
        var currency = CurrencyInfo.CurrentCurrency;

        currency.Should().Be(CurrencyInfo.FromCode("EUR"));
    }

    [Fact]
    [UseCulture(null)]
    public void WhenCurrentCultureIsInvariant_ThenCurrencyIsDefault()
    {
        var currency = CurrencyInfo.CurrentCurrency;

        currency.Should().Be(CurrencyInfo.NoCurrency);
    }
}
