using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using NodaMoney.Tests.Helpers;

namespace NodaMoney.Tests.MoneyFormattableSpec;

[Collection(nameof(NoParallelization))]
public class FormatCacheIsolation
{
    [Fact]
    public void WhenFormattedTwice_ThenReturnsEqualStringsAndReusesCachedNumberFormatInfo()
    {
        var currencyInfo = CurrencyInfo.FromCode("EUR");
        var money = new Money(1234.56m, currencyInfo);
        CultureInfo original = Thread.CurrentThread.CurrentCulture;
        try
        {
            // A read-only (runtime-cached) culture is the cacheable case
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("nl-NL");

            string first = money.ToString();
            string second = money.ToString();

            first.Should().Be(second);

            // The cached NumberFormatInfo instance (reached the same way ToString does, through GetFormat) is reused,
            // not rebuilt, on the second call.
            object nfi1 = currencyInfo.GetFormat(typeof(NumberFormatInfo));
            object nfi2 = currencyInfo.GetFormat(typeof(NumberFormatInfo));
            ReferenceEquals(nfi1, nfi2).Should().BeTrue();
        }
        finally
        {
            Thread.CurrentThread.CurrentCulture = original;
        }
    }

    [Fact]
    public void WhenCurrentCultureIsMutableAndCustomized_ThenFormattingHonorsTheCustomizationAndDoesNotCache()
    {
        var currencyInfo = CurrencyInfo.FromCode("EUR");
        var money = new Money(1234.56m, currencyInfo);
        CultureInfo original = Thread.CurrentThread.CurrentCulture;
        try
        {
            var customized = new CultureInfo("nl-NL");
            customized.NumberFormat.CurrencyGroupSeparator = "'";
            Thread.CurrentThread.CurrentCulture = customized;

            money.ToString().Should().Be("€ 1'234,56");

            // A later customization is still picked up, because a mutable culture is cloned per call, never cached
            customized.NumberFormat.CurrencyGroupSeparator = "_";
            money.ToString().Should().Be("€ 1_234,56");

            object nfi1 = currencyInfo.GetFormat(typeof(NumberFormatInfo));
            object nfi2 = currencyInfo.GetFormat(typeof(NumberFormatInfo));
            ReferenceEquals(nfi1, nfi2).Should().BeFalse();
        }
        finally
        {
            Thread.CurrentThread.CurrentCulture = original;
        }
    }

    [Fact]
    public void WhenCurrentCultureChangesBetweenCalls_ThenEachCallUsesTheCultureAtCallTime()
    {
        var money = new Money(1234.56m, CurrencyInfo.FromCode("EUR"));
        CultureInfo original = Thread.CurrentThread.CurrentCulture;
        try
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("nl-NL");
            string nl1 = money.ToString();

            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
            string us = money.ToString();

            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("nl-NL");
            string nl2 = money.ToString();

            nl1.Should().Be(nl2);
            nl1.Should().NotBe(us);
            nl1.Should().Be(money.ToString(CultureInfo.GetCultureInfo("nl-NL")));
            us.Should().Be(money.ToString(CultureInfo.GetCultureInfo("en-US")));
        }
        finally
        {
            Thread.CurrentThread.CurrentCulture = original;
        }
    }

    [Fact]
    public async Task WhenFormattingConcurrentlyOnDifferentCultures_ThenEachTaskGetsItsOwnCultureOutput()
    {
        var money = new Money(1234.56m, CurrencyInfo.FromCode("EUR"));
        string expectedNl = money.ToString(CultureInfo.GetCultureInfo("nl-NL"));
        string expectedUs = money.ToString(CultureInfo.GetCultureInfo("en-US"));
        expectedNl.Should().NotBe(expectedUs); // sanity: the two cultures format differently

        Task<string> RunOnCulture(string cultureName) => Task.Run(() =>
        {
            CultureInfo original = Thread.CurrentThread.CurrentCulture;
            try
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo(cultureName);
                return money.ToString();
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = original;
            }
        });

        Task<string> nlTask = RunOnCulture("nl-NL");
        Task<string> usTask = RunOnCulture("en-US");
        string[] results = await Task.WhenAll(nlTask, usTask);

        results[0].Should().Be(expectedNl);
        results[1].Should().Be(expectedUs);
    }

    [Fact]
    public void WhenGivenExplicitNumberFormatInfoOrCultureInfo_ThenBothProduceTheSameOutput()
    {
        var money = new Money(1234.56m, CurrencyInfo.FromCode("EUR"));
        var culture = new CultureInfo("nl-NL");
        NumberFormatInfo nfi = culture.NumberFormat;

        string viaCulture = money.ToString(culture);
        string viaNumberFormatInfo = money.ToString(nfi);

        viaCulture.Should().Be(viaNumberFormatInfo);
        viaCulture.Should().Be("€ 1.234,56");
    }

    [Fact]
    public void WhenRetrievedThroughGetFormat_ThenTheCachedNumberFormatInfoIsReadOnly()
    {
        CultureInfo original = Thread.CurrentThread.CurrentCulture;
        try
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("nl-NL");
            var currencyInfo = CurrencyInfo.FromCode("EUR");

            var nfi = (NumberFormatInfo)currencyInfo.GetFormat(typeof(NumberFormatInfo));

            nfi.IsReadOnly.Should().BeTrue();
            Action mutate = () => nfi.CurrencySymbol = "X";
            mutate.Should().Throw<InvalidOperationException>();
        }
        finally
        {
            Thread.CurrentThread.CurrentCulture = original;
        }
    }
}
