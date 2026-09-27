using System.Globalization;

namespace NodaMoney.Tests.MoneyParsableSpec;

public class ParseWithoutAllocating
{
    static long AllocatedPerCall(Func<int> action)
    {
        for (int i = 0; i < 3000; i++) action(); // warm up so tiering and lazy caches are settled
        long before = GC.GetAllocatedBytesForCurrentThread();
        for (int i = 0; i < 1000; i++) action();
        return (GC.GetAllocatedBytesForCurrentThread() - before) / 1000;
    }

#if NET9_0_OR_GREATER
    [Theory]
    [InlineData("€ 765,43")]
    [InlineData("EUR 765,43")]
    [InlineData("765,43")]
    public void WhenParsingUnderNlCulture_ThenNothingIsAllocated(string input)
    {
        var previous = CultureInfo.CurrentCulture;
        CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("nl-NL");
        try
        {
            AllocatedPerCall(() => Money.Parse(input).Scale).Should().Be(0);
        }
        finally
        {
            CultureInfo.CurrentCulture = previous;
        }
    }
#endif

    [Fact]
    public void WhenParsingAmbiguousSymbolWithSpecifiedCurrency_ThenTheSpecifiedCurrencyWins()
    {
        var money = Money.Parse("$ 765.43", CurrencyInfo.FromCode("CAD"));

        money.Currency.Should().Be((Currency)CurrencyInfo.FromCode("CAD"));
        money.Amount.Should().Be(765.43m);
    }

    [Fact]
    public void WhenParsingAmbiguousSymbolUnderMatchingCulture_ThenTheCultureCurrencyWins()
    {
        var previous = CultureInfo.CurrentCulture;
        CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
        try
        {
            var money = Money.Parse("$ 765.43");

            money.Currency.Should().Be((Currency)CurrencyInfo.FromCode("USD"));
        }
        finally
        {
            CultureInfo.CurrentCulture = previous;
        }
    }
}
