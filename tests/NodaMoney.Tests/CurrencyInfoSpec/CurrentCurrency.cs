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
            // Act & Assert: nl-NL, then en-US (a different culture reference), then nl-NL again
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

    [Fact]
    public void WhenCurrentCultureIsSetToDifferentInstancesOfSameCulture_ThenCurrencyStaysCorrect()
    {
        // Arrange
        var originalCulture = Thread.CurrentThread.CurrentCulture;
        try
        {
            // Act & Assert: two distinct CultureInfo instances for the same culture must not confuse the cache
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            CurrencyInfo.CurrentCurrency.Should().Be(CurrencyInfo.FromCode("USD"));

            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US"); // different instance, same culture
            CurrencyInfo.CurrentCurrency.Should().Be(CurrencyInfo.FromCode("USD"));
        }
        finally
        {
            Thread.CurrentThread.CurrentCulture = originalCulture;
        }
    }

    [Fact]
    public void WhenCurrencyRegistryMutatesForCurrentCulture_ThenCurrentCurrencyReflectsTheChange()
    {
        // Arrange
        var originalCulture = Thread.CurrentThread.CurrentCulture;
        CurrencyInfo originalEuro = null;
        try
        {
            // Use a cached CultureInfo instance, so the second read below hits the same culture reference.
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("nl-NL");

            CurrencyInfo before = CurrencyInfo.CurrentCurrency;
            before.Should().Be(CurrencyInfo.FromCode("EUR"));

            originalEuro = CurrencyInfo.Unregister("EUR");
            CurrencyInfo modifiedEuro = originalEuro with { EnglishName = "Modified Euro for test" };
            CurrencyInfo.Register(modifiedEuro);

            // Act
            CurrencyInfo after = CurrencyInfo.CurrentCurrency;

            // Assert: must resolve through the registry again, not return the record cached before the mutation
            after.EnglishName.Should().Be("Modified Euro for test");
            after.Should().Be(modifiedEuro);
        }
        finally
        {
            Thread.CurrentThread.CurrentCulture = originalCulture;

            if (originalEuro is not null)
            {
                CurrencyInfo.Unregister("EUR");
                CurrencyInfo.Register(originalEuro);
            }
        }
    }
}
