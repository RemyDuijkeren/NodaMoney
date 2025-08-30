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
    readonly FastMoney _euro10fast = new(10, "EUR");
    readonly FastMoney _euro20fast = new(20, "EUR");
    readonly FastMoney _dollar10fast = new(10, "USD");

    [Benchmark(Baseline = true)]
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
    public bool Equal()
    {
        return _euro10 == _euro10; // false
    }

    [Benchmark]
    public bool NotEqualValue()
    {
        return _euro10 == _euro20; // false
    }

    [Benchmark]
    public bool NotEqualCurrency()
    {
        return _euro10 == _dollar10; // false
    }

    [Benchmark]
    public bool EqualOrBigger()
    {
        return _euro20 >= _euro10; // true
    }

    [Benchmark]
    public bool Bigger()
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

    [Benchmark]
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
    public FastMoney fMultiple()
    {
        return _euro10fast * 2.2m;
    }

    [Benchmark]
    public FastMoney fMultipleWholeNumber()
    {
        return _euro10fast * 2m;
    }

    [Benchmark]
    public FastMoney fMultipleLong()
    {
        return _euro10fast * 2L;
    }

    [Benchmark]
    public FastMoney fDivide()
    {
        return _euro10fast / 2.2m;
    }

    [Benchmark]
    public FastMoney fDivideWholeNumber()
    {
        return _euro10fast / 2m;
    }

    [Benchmark]
    public FastMoney fDivideLong()
    {
        return _euro10fast / 2L;
    }

    [Benchmark]
    public bool fEqual()
    {
        return _euro10fast == _euro10fast; // true
    }

    [Benchmark]
    public bool fNotEqualValue()
    {
        return _euro10fast == _euro20fast; // false
    }

    [Benchmark]
    public bool fNotEqualCurrency()
    {
        return _euro10fast == _dollar10fast; // false
    }

    [Benchmark]
    public bool fEqualOrBigger()
    {
        return _euro20fast >= _euro10fast; // true
    }

    [Benchmark]
    public bool fBigger()
    {
        return _euro20fast > _euro10fast; // true
    }
}
