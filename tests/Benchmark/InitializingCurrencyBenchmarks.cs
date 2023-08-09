using BenchmarkDotNet.Attributes;
using NodaMoney;

namespace Benchmark;

[MemoryDiagnoser]
public class InitializingCurrencyBenchmarks
{
    [Benchmark]
    public Currency FromCode()
    {
        Currency currency = Currency.FromCode("EUR");
        return currency;
    }

    [Benchmark]
    public Currency FromCodeBeRef()
    {
        ref Currency currency = ref Currency.FromCode("EUR");
        return currency;
    }
}