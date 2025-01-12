using BenchmarkDotNet.Attributes;
using NodaMoney;

namespace Benchmark;

[MemoryDiagnoser]
public class HighLoadBenchmarks
{
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

    [Benchmark]
    public FastMoney[] CreatingOneMillionMoneyUnit()
    {
        int max = 1_000_000;
        FastMoney[] money = new FastMoney[max];

        for (int i = 0; i < max; i++)
        {
            if (i % 3 == 0)
                money[i] = new FastMoney(10M, "EUR");
            else if (i % 2 == 0)
                money[i] = new FastMoney(10M, "USD");
            else
                money[i] = new FastMoney(10M, "JPY");
        }

        return money;
    }
}
