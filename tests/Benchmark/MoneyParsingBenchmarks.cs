using System.Globalization;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using NodaMoney;

namespace Benchmark;

/// <summary>Parsing a <see cref="Money"/> next to parsing the bare amount as a <c>decimal</c>. The decimal variant has no
/// currency symbol to resolve, so its time is the floor for the number part alone. Grouped per API shape with the
/// <c>decimal</c> variant as baseline, so the Ratio column reads as "times a raw decimal".</summary>
[MemoryDiagnoser]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
[CategoriesColumn]
public class MoneyParsingBenchmarks
{
    const string EurosString = "€ 765.43";
    const string AmountString = "765.43";

    [BenchmarkCategory("Parse"), Benchmark(Baseline = true)]
    public decimal dParse() => decimal.Parse(AmountString, NumberStyles.Currency);

    [BenchmarkCategory("Parse"), Benchmark]
    public Money Implicit() => Money.Parse(EurosString); // or € 765.43

    [BenchmarkCategory("Parse"), Benchmark]
    public Money Explicit() => Money.Parse(EurosString, CurrencyInfo.FromCode("EUR"));  // or € 765.43

    [BenchmarkCategory("Parse"), Benchmark]
    public Money ExplicitAsSpan() => Money.Parse(EurosString.AsSpan(), CurrencyInfo.FromCode("EUR"));  // or € 765.43

    [BenchmarkCategory("TryParse"), Benchmark(Baseline = true)]
    public decimal dTryParse()
    {
        decimal.TryParse(AmountString, NumberStyles.Currency, null, out decimal amount);
        return amount;
    }

    [BenchmarkCategory("TryParse"), Benchmark]
    public Money ImplicitTry()
    {
        Money.TryParse(EurosString, out Money euro); // or € 765.43
        return euro;
    }

    [BenchmarkCategory("TryParse"), Benchmark]
    public Money ExplicitTry()
    {
        Money.TryParse(EurosString, CurrencyInfo.FromCode("EUR"), out Money euro);
        return euro;
    }

    [BenchmarkCategory("TryParse"), Benchmark]
    public Money ExplicitTryAsSpan()
    {
        Money.TryParse(EurosString.AsSpan(), CurrencyInfo.FromCode("EUR"), out Money euro);
        return euro;
    }
}
