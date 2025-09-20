using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using NodaMoney;

namespace Benchmark;

[MemoryDiagnoser]
public class MoneyOperationsBenchmarks
{
    readonly Money _euro10 = Money.Euro(10);
    readonly Money _euro20 = Money.Euro(20);
    readonly Money _dollar10 = Money.USDollar(10);
    Money _euro = new Money(765.43m, "EUR");
    FastMoney _euro10fast = new(10, "EUR");
    readonly FastMoney _euro20fast = new(20, "EUR");
    readonly FastMoney _dollar10fast = new(10, "USD");

    [Benchmark]
    public Money Add()
    {
        return _euro10 + _euro20;
    }

    [Benchmark]
    public Money Subtract()
    {
        return _euro20 - _euro10;
    }

    [Benchmark]
    public Money Multiple()
    {
        return _euro10 * 2.2m;
    }

    [Benchmark]
    public Money Divide()
    {
        return _euro10 / 2.2m;
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

    [Benchmark]
    public Money Remainder()
    {
        return _euro20 % _euro10;
    }

    [Benchmark(Baseline = true)]
    public FastMoney fAdd()
    {
        return FastMoney.Add(_euro10fast, _euro20fast);
    }

    [Benchmark]
    public FastMoney fSubtract()
    {
        return FastMoney.Subtract(_euro20fast, _euro10fast);
    }

    [Benchmark]
    public FastMoney fMultipleDec()
    {
        return _euro10fast * 2.2m;
    }

    [Benchmark]
    public FastMoney fMultipleDecWholeNumber()
    {
        return _euro10fast * 2m;
    }

    [Benchmark]
    public FastMoney fMultipleLong()
    {
        return _euro10fast * 2L;
    }

    [Benchmark]
    public FastMoney fDivideDec()
    {
        return _euro10fast / 2.2m;
    }

    [Benchmark]
    public FastMoney fDivideDecWholeNumber()
    {
        return _euro10fast / 2m;
    }

    [Benchmark]
    public FastMoney fDivideLong()
    {
        return _euro10fast / 2L;
    }

    [Benchmark]
    public FastMoney fIncrement()
    {
        return ++_euro10fast;
    }

    [Benchmark]
    public FastMoney fDecrement()
    {
        return --_euro10fast;
    }

    [Benchmark]
    public FastMoney fRemainder()
    {
        return _euro20fast % _euro10fast;
    }
}
