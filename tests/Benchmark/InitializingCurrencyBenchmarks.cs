using BenchmarkDotNet.Attributes;
using NodaMoney;

namespace Benchmark;

[MemoryDiagnoser]
public class InitializingCurrencyBenchmarks
{
    [Benchmark]
    public Currency CurrencyFromCode()
    {
        Currency currency = Currency.FromCode("EUR");
        return currency;
    }

    [Benchmark]
    public CurrencyInfo CurrencyInfoFromCode()
    {
        CurrencyInfo currency = CurrencyInfo.FromCode("EUR");
        return currency;
    }

    // [Benchmark]
    // public Currency FromCodeBeRef()
    // {
    //     ref Currency currency = ref Currency.FromCode("EUR");
    //     return currency;
    // }
}
