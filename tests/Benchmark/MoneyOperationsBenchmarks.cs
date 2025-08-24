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
    public decimal fAdd()
    {
        var money = FastMoney.Add(_euro10fast, _euro20fast);
        return money.Amount;
    }

    [Benchmark]
    public long fSubtract()
    {
        var money =  FastMoney.Subtract(_euro20fast, _euro10fast);
        return FastMoney.ToOACurrency(money);
    }

    [Benchmark]
    public long fMultiple()
    {
        var money = _euro10fast * 2.2m;
        return FastMoney.ToOACurrency(money);
    }

    [Benchmark]
    public long fMultipleWholeNumber()
    {
        var money = _euro10fast * 2m;
        return FastMoney.ToOACurrency(money);
    }

    [Benchmark]
    public long fMultipleLong()
    {
        var money = _euro10fast * 2L;
        return FastMoney.ToOACurrency(money);
    }

    [Benchmark]
    public long fDivide()
    {
        var money = _euro10fast / 2.2m;
        return FastMoney.ToOACurrency(money);
    }

    [Benchmark]
    public long fDivideWholeNumber()
    {
        var money = _euro10fast / 2m;
        return FastMoney.ToOACurrency(money);
    }

    [Benchmark]
    public long fDivideLong()
    {
        var money = _euro10fast / 2L;
        return FastMoney.ToOACurrency(money);
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
