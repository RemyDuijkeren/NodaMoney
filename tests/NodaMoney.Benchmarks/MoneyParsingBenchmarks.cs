using BenchmarkDotNet.Attributes;

namespace NodaMoney.Benchmarks;

[MemoryDiagnoser]
public class MoneyParsingBenchmarks
{
    [Benchmark]
    public Money Implicit()
    {
        return Money.Parse("€ 765,43"); // or € 765.43
    }

    [Benchmark]
    public Money ImplicitTry()
    {
        Money.TryParse("€ 765,43", out Money euro); // or € 765.43
        return euro;
    }

    [Benchmark]
    public Money Explicit()
    {
        return Money.Parse("€ 765,43", Currency.FromCode("EUR"));  // or € 765.43
    }

    [Benchmark]
    public Money ExplicitTry()
    {
        Money.TryParse("€ 765,43", Currency.FromCode("EUR"), out Money euro);
        return euro;
    }
}