using System.Globalization;

namespace NodaMoney.Tests.MoneyFormattableSpec;

public class FormatWithoutBoxing
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
    [InlineData(null)]
    [InlineData("C")]
    [InlineData("G")]
    [InlineData("I")]
    public void WhenFormattingUnderNlCulture_ThenOnlyTheResultStringIsAllocated(string format)
    {
        var previous = CultureInfo.CurrentCulture;
        CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("nl-NL");
        try
        {
            var money = new Money(765.43m, "EUR");
            string result = money.ToString(format);
            long stringSize = (22 + 2 * result.Length + 7) / 8 * 8; // object header, length and chars, 8-byte aligned

            AllocatedPerCall(() => money.ToString(format).Length).Should().BeLessThanOrEqualTo(stringSize);
        }
        finally
        {
            CultureInfo.CurrentCulture = previous;
        }
    }
#endif

    [Fact]
    public void WhenFormattingThroughICustomFormatter_ThenBoxedMoneyIsStillFormatted()
    {
        var money = new Money(765.43m, "EUR");
        CurrencyInfo eur = CurrencyInfo.FromCode("EUR");

        string viaFormatter = string.Format(eur, "{0:G}", money);

        viaFormatter.Should().Be(money.ToString("G"));
    }
}
