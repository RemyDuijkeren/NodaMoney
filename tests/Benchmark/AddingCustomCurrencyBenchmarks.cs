using BenchmarkDotNet.Attributes;
using NodaMoney;

namespace Benchmark;

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

    //[Benchmark] NodaMoney.InvalidCurrencyException: The currency BTC is already registered in virtual
    public Currency Register()
    {
        return _builder.Register();
    }

    //[Benchmark] NodaMoney.InvalidCurrencyException: The currency BTC is already registered in virtual
    public Currency Unregister()
    {
        return CurrencyBuilder.Unregister("USD", "ISO-4217");
    }

    // [Benchmark] NodaMoney.InvalidCurrencyException: The currency BTC is already registered in virtual
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
