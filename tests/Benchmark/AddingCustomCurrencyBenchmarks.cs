using BenchmarkDotNet.Attributes;
using NodaMoney;

namespace Benchmark;

[MemoryDiagnoser]
public class AddingCustomCurrencyBenchmarks
{
    readonly CurrencyInfoBuilder _builder = new CurrencyInfoBuilder("BTX")
    {
        EnglishName = "Bitcoin",
        Symbol = "฿",
        NumericCode = "123", // iso number
        DecimalDigits = 8
    };

    [Benchmark]
    public CurrencyInfoBuilder CreateBuilder()
    {
        return new CurrencyInfoBuilder("BTX")
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
        return CurrencyInfoBuilder.Unregister("USD");
    }

    // [Benchmark] NodaMoney.InvalidCurrencyException: The currency BTC is already registered in virtual
    public Currency Replace()
    {
        CurrencyInfo oldEuro = CurrencyInfoBuilder.Unregister("EUR");

        var builder = new CurrencyInfoBuilder("EUR");
        builder.LoadDataFromCurrencyInfo(oldEuro);
        builder.EnglishName = "New Euro";
        builder.DecimalDigits = 1;

        return builder.Register();
    }
}
