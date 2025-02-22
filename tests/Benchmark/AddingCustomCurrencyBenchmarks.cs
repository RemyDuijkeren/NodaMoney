using BenchmarkDotNet.Attributes;
using NodaMoney;

namespace Benchmark;

[MemoryDiagnoser]
public class AddingCustomCurrencyBenchmarks
{
    readonly CurrencyInfo _newCurrency = CurrencyInfo.Create("BTX") with
    {
        EnglishName = "Bitcoin",
        Symbol = "฿",
        Number = 123, // iso number
        MinorUnit = MinorUnit.Eight,
    };

    [Benchmark]
    public CurrencyInfo Build()
    {
        var ci = CurrencyInfo.Create("BTX") with
        {
            EnglishName = "Bitcoin",
            Symbol = "฿",
            Number = 123, // iso number
            MinorUnit = MinorUnit.Eight,
        };

        return ci;
    }

    [Benchmark]
    public void Register()
    {
        CurrencyInfo.Register(_newCurrency);
    }

    [Benchmark]
    public CurrencyInfo Unregister()
    {
        return CurrencyInfo.Unregister("USD");
    }

    [Benchmark]
    public void Replace()
    {
        CurrencyInfo oldEuro = CurrencyInfo.Unregister("EUR");

        var newEuro = oldEuro with
        {
            EnglishName = "New Euro",
            MinorUnit = MinorUnit.One
        };

        CurrencyInfo.Register(newEuro);
    }
}
