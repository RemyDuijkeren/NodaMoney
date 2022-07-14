using BenchmarkDotNet.Attributes;

namespace NodaMoney.Benchmarks;

[MemoryDiagnoser]
public class AddingCustomCurrencyBenchmarks
{
    readonly CurrencyBuilder _builder = new CurrencyBuilder("BTC", "virtual")
    {
        EnglishName = "Bitcoin",
        Symbol = "฿",
        NumericCode = "123", // iso number
        DecimalDigits = 8
    };

    [Benchmark]
    public CurrencyBuilder CreateBuilder()
    {
        return new CurrencyBuilder("BTC", "virtual")
        {
            EnglishName = "Bitcoin",
            Symbol = "฿",
            NumericCode = "123", // iso number
            DecimalDigits = 8
        };
    }

    [Benchmark]
    public Currency Build()
    {
        return _builder.Build();
    }

    // [Benchmark]
    public Currency Register()
    {
        return _builder.Register();
    }

    // [Benchmark]
    public Currency Unregister()
    {
        return CurrencyBuilder.Unregister("USD", "ISO-4217");
    }

    // [Benchmark]
    public Currency Replace()
    {
        Currency oldEuro = CurrencyBuilder.Unregister("EUR", "ISO-4217");

        var builder = new CurrencyBuilder("EUR", "ISO-4217");
        builder.LoadDataFromCurrency(oldEuro);
        builder.EnglishName = "New Euro";
        builder.DecimalDigits = 1;
        
        return builder.Register();
    }
}