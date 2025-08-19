using BenchmarkDotNet.Attributes;
using NodaMoney;

namespace Benchmark;

[MemoryDiagnoser]
public class InitializingCurrencyBenchmarks
{
    [Benchmark]
    public Currency CurrencyFromCode()
    {
        Currency currency = CurrencyInfo.FromCode("EUR");
        return currency;
    }

    [Benchmark]
    public CurrencyInfo CurrencyInfoFromCode()
    {
        CurrencyInfo currency = CurrencyInfo.FromCode("EUR");
        return currency;
    }

    [Benchmark]
    public CurrencyInfo CurrencyInfoTryFromCode()
    {
        bool result = CurrencyInfo.TryFromCode("EUR", out CurrencyInfo currency);
        return currency;
    }
}
