using BenchmarkDotNet.Attributes;
using NodaMoney;

namespace Benchmark;

[MemoryDiagnoser]
public class InitializingMoneyBenchmarks
{
    readonly Currency _euro = CurrencyInfo.FromCode("EUR");
    readonly Money _money = new Money(10m, "EUR");

    [Benchmark(Baseline = true)]
    public Money ExplicitCurrencyAsString()
    {
        return new Money(6.54m, "EUR");
    }

    [Benchmark]
    public Money ExplicitCurrencyAsStringAndRounding()
    {
        return new Money(765.425m, "EUR", MidpointRounding.AwayFromZero);
    }

    [Benchmark]
    public Money ExplicitCurrencyFromCode()
    {
        return new Money(6.54m, CurrencyInfo.FromCode("EUR"));
    }

    [Benchmark]
    public Money ExplicitCurrencyInfoFromCode()
    {
        return new Money(6.54m, CurrencyInfo.FromCode("EUR"));
    }

    [Benchmark]
    public Money HelperMethod()
    {
        return Money.Euro(6.54m);
    }

    [Benchmark]
    public Money ImplicitCurrencyByConstructor()
    {
        return new Money(6.54m);
    }

    [Benchmark]
    public Money ExplicitCurrencyByCasting()
    {
        Money money = (Money)6.54m;
        return money;
    }

    [Benchmark]
    public (decimal, Currency) Deconstruct()
    {
        var (amount, currency) = _money;
        return (amount, currency);
    }
}
