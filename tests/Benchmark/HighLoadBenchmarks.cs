using System.Data.SqlTypes;
using BenchmarkDotNet.Attributes;
using NodaMoney;
using NodaMoney.Context;

namespace Benchmark;

[MemoryDiagnoser]
//[Config(typeof(InProcessConfig))]
public class HighLoadBenchmarks
{
    //[Params(1, 100, 1_000, 100_000, 1_000_000)]
    public int Count { get; set; } = 1_000_000;

    [Benchmark]
    public Currency[] Create1MCurrency()
    {
        Currency[] currencies = new Currency[Count];

        for (int i = 0; i < Count; i++)
        {
            if (i % 3 == 0)
                currencies[i] = CurrencyInfo.FromCode("EUR");
            else if (i % 2 == 0)
                currencies[i] = CurrencyInfo.FromCode("USD");
            else
                currencies[i] = CurrencyInfo.FromCode("JPY");
        }

        return currencies;
    }

    [Benchmark]
    public decimal Create1MDecimal()
    {
        decimal[] decimals = new decimal[Count];

        for (int i = 0; i < Count; i++)
        {
            if (i % 3 == 0)
                decimals[i] = 10M;
            else if (i % 2 == 0)
                decimals[i] = 20M;
            else
                decimals[i] = 30M;
        }

        return decimals[0];
    }

    [Benchmark(Baseline = true)]
    public Money[] Create1MMoney()
    {
        Money[] money = new Money[Count];

        for (int i = 0; i < Count; i++)
        {
            if (i % 3 == 0)
                money[i] = new Money(10M, "EUR");
            else if (i % 2 == 0)
                money[i] = new Money(20M, "USD");
            else
                money[i] = new Money(30M, "JPY");
        }

        return money;
    }

    [Benchmark]
    public decimal Create1MFastMoney()
    {
        FastMoney[] money = new FastMoney[Count];

        for (int i = 0; i < Count; i++)
        {
            if (i % 3 == 0)
                money[i] = new FastMoney(10M, "EUR");
            else if (i % 2 == 0)
                money[i] = new FastMoney(20M, "USD");
            else
                money[i] = new FastMoney(30M, "JPY");
        }

        return money[0].Amount;
    }

    [Benchmark]
    public SqlMoney Create1MSqlMoney()
    {
        SqlMoney[] money = new SqlMoney[Count];

        for (int i = 0; i < Count; i++)
        {
            if (i % 3 == 0)
                money[i] = new SqlMoney(10M);
            else if (i % 2 == 0)
                money[i] = new SqlMoney(20M);
            else
                money[i] = new SqlMoney(30M);
        }

        return money[0];
    }
}
