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
    readonly FastMoney _euro10fast = new(10, "EUR");
    readonly FastMoney _euro20fast = new(20, "EUR");

    [Benchmark(Baseline = true)]
    public Money Add()
    {
        return _euro10 + _euro20;
    }

    [Benchmark]
    public decimal AddFastMoney()
    {
        var money = FastMoney.Add(_euro10fast, _euro20fast);
        return money.Amount;
    }

    [Benchmark]
    public Money Subtract()
    {
        return _euro20 - _euro10;
    }

    [Benchmark]
    public long SubtractFastMoney()
    {
        var money =  FastMoney.Subtract(_euro20fast, _euro10fast);
        return FastMoney.ToOACurrency(money);
    }

    [Benchmark]
    public Money Multiple()
    {
        return _euro10 * 2.2m;
    }

    [Benchmark]
    public long MultipleFastMoneyDecimal()
    {
        var money = _euro10fast * 2.2m;
        return FastMoney.ToOACurrency(money);
    }

    [Benchmark]
    public long MultipleFastWholeDecimal()
    {
        var money = _euro10fast * 2m;
        return FastMoney.ToOACurrency(money);
    }

    [Benchmark]
    public long MultipleFastMoneyLong()
    {
        var money = _euro10fast * 2L;
        return FastMoney.ToOACurrency(money);
    }

    [Benchmark]
    public Money Divide()
    {
        return _euro10 / 2.2m;
    }

    [Benchmark]
    public long DivideFastMoneyDecimal()
    {
        var money = _euro10fast / 2.2m;
        return FastMoney.ToOACurrency(money);
    }

    [Benchmark]
    public long DivideFastMoneyWholeDecimal()
    {
        var money = _euro10fast / 2m;
        return FastMoney.ToOACurrency(money);
    }

    [Benchmark]
    public long DivideFastMoneyLong()
    {
        var money = _euro10fast / 2L;
        return FastMoney.ToOACurrency(money);
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
