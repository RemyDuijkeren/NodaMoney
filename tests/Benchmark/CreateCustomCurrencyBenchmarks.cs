using BenchmarkDotNet.Attributes;
using NodaMoney;

namespace Benchmark;

[MemoryDiagnoser]
public class CreateCustomCurrencyBenchmarks
{
    readonly CurrencyInfo _newCurrency = CurrencyInfo.Create("BTX") with
    {
        EnglishName = "Bitcoin",
        Symbol = "฿",
        Number = 123, // iso number
        MinorUnit = MinorUnit.Eight,
    };

    [Benchmark]
    public CurrencyInfo Create()
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
