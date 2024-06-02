using BenchmarkDotNet.Attributes;
using NodaMoney;

namespace Benchmark;

[MemoryDiagnoser]
public class HighLoadBenchmarks
{
    // [Benchmark]
    // public CurrencyUnit[] CreatingOneMillionCurrencyUnits()
    // {
    //     int max = 1_000_000;
    //     CurrencyUnit[] currencies = new CurrencyUnit[max];
    //
    //     for (int i = 0; i < max; i++)
    //     {
    //         if (i % 3 == 0)
    //             currencies[i] = new CurrencyUnit("EUR");
    //         else if (i % 2 == 0)
    //             currencies[i] = new CurrencyUnit("USD");
    //         else
    //             currencies[i] = new CurrencyUnit("JPY");
    //     }
    //
    //     return currencies;
    // }
    
    [Benchmark]
    public Currency[] CreatingOneMillionCurrency()
    {
        int max = 1_000_000;
        Currency[] currencies = new Currency[max];

        for (int i = 0; i < max; i++)
        {
            if (i % 3 == 0)
                currencies[i] = Currency.FromCode("EUR");
            else if (i % 2 == 0)
                currencies[i] = Currency.FromCode("USD");
            else
                currencies[i] = Currency.FromCode("JPY");
        }

        return currencies;
    }

    [Benchmark]
    public Money[] CreatingOneMillionMoney()
    {
        int max = 1_000_000;
        Money[] money = new Money[max];

        for (int i = 0; i < max; i++)
        {
            if (i % 3 == 0)
                money[i] = new Money(10M, "EUR");
            else if (i % 2 == 0)
                money[i] = new Money(10M, "USD");
            else
                money[i] = new Money(10M, "JPY");
        }

        return money;
    }
}
