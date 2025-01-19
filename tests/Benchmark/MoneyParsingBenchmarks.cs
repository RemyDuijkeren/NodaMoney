using BenchmarkDotNet.Attributes;
using NodaMoney;

namespace Benchmark;

[MemoryDiagnoser]
public class MoneyParsingBenchmarks
{
    const string EurosString = "€ 765.43";

    [Benchmark]
    public Money Implicit()
    {
        return Money.Parse(EurosString); // or € 765.43
    }

    [Benchmark]
    public Money ImplicitTry()
    {
        Money.TryParse(EurosString, out Money euro); // or € 765.43
        return euro;
    }

    [Benchmark]
    public Money Explicit()
    {
        return Money.Parse(EurosString, CurrencyInfo.FromCode("EUR"));  // or € 765.43
    }

    [Benchmark]
    public Money ExplicitAsSpan()
    {
        return Money.Parse(EurosString.AsSpan(), CurrencyInfo.FromCode("EUR"));  // or € 765.43
    }

    [Benchmark]
    public Money ExplicitTry()
    {
        Money.TryParse(EurosString, CurrencyInfo.FromCode("EUR"), out Money euro);
        return euro;
    }

    [Benchmark]
    public Money ExplicitTryAsSpan()
    {
        Money.TryParse(EurosString.AsSpan(), CurrencyInfo.FromCode("EUR"), out Money euro);
        return euro;
    }
}
