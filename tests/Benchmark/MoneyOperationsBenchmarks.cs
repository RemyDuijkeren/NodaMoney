using BenchmarkDotNet.Attributes;
using NodaMoney;

namespace Benchmark;

[MemoryDiagnoser]
public class MoneyOperationsBenchmarks
{
    readonly Money _euro10 = Money.Euro(10);
    readonly Money _euro20 = Money.Euro(20);
    readonly Money _dollar10 = Money.USDollar(10);
    Money _euro = new Money(765.43m, "EUR");
    readonly FastMoney _euro10fast = new FastMoney(10, "EUR");
    readonly FastMoney _euro20fast = new FastMoney(20, "EUR");

    [Benchmark(Baseline = true)]
    public Money Addition()
    {
        return _euro10 + _euro20;
    }

    [Benchmark]
    public FastMoney AdditionFastMoney()
    {
        return FastMoney.Add(_euro10fast, _euro20fast);
    }

    [Benchmark]
    public Money Subtraction()
    {
        return _euro20 - _euro10;
    }

    [Benchmark]
    public FastMoney SubtractionFastMoney()
    {
        return FastMoney.Subtract(_euro10fast, _euro20fast);
    }

    [Benchmark]
    public bool CompareSameCurrency()
    {
        return _euro10 == _euro20; // false
    }

    [Benchmark]
    public bool CompareDifferentCurrency()
    {
        return _euro10 == _dollar10; // false
    }

    [Benchmark]
    public bool CompareAmount()
    {
        return _euro20 > _euro10; // true
    }

    [Benchmark]
    public Money Increment()
    {
        return ++_euro;
    }

    [Benchmark]
    public Money Decrement()
    {
        return --_euro;
    }
}
