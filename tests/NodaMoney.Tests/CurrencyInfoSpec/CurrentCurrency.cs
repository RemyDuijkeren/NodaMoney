using System.Globalization;
using System.Threading;
using NodaMoney.Tests.Helpers;

namespace NodaMoney.Tests.CurrencyInfoSpec;

[Collection(nameof(NoParallelization))]
public class CurrentCurrency
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

    [Fact]
    public void WhenCurrentCultureSwitchesBackAndForth_ThenCurrencyFollowsEachSwitch()
    {
        // Arrange
        var originalCulture = Thread.CurrentThread.CurrentCulture;
        try
        {
            // Act & Assert: nl-NL, then en-US (invalidates the single-entry cache), then nl-NL again (invalidates it back)
            Thread.CurrentThread.CurrentCulture = new CultureInfo("nl-NL");
            CurrencyInfo.CurrentCurrency.Should().Be(CurrencyInfo.FromCode("EUR"));

            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            CurrencyInfo.CurrentCurrency.Should().Be(CurrencyInfo.FromCode("USD"));

            Thread.CurrentThread.CurrentCulture = new CultureInfo("nl-NL");
            CurrencyInfo.CurrentCurrency.Should().Be(CurrencyInfo.FromCode("EUR"));
        }
        finally
        {
            Thread.CurrentThread.CurrentCulture = originalCulture;
        }
    }
}
