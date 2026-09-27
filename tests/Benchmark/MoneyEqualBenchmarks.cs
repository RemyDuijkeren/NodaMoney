using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using NodaMoney;

namespace Benchmark;

/// <summary>Equality and ordering for <c>decimal</c>, <see cref="Money"/> and <see cref="FastMoney"/>. Grouped per
/// comparison with the <c>decimal</c> variant as baseline, so the Ratio column reads as "times a raw decimal".</summary>
[MemoryDiagnoser]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
[CategoriesColumn]
public class MoneyEqualBenchmarks
{
    readonly decimal _dec10 = 10m;
    readonly decimal _dec10Same = 10m; // *Same fields hold an equal value, so the equal case is not a self-comparison (CS1718)
    readonly decimal _dec20 = 20m;
    readonly Money _euro10 = Money.Euro(10);
    readonly Money _euro10Same = Money.Euro(10);
    readonly Money _euro20 = Money.Euro(20);
    readonly Money _dollar10 = Money.USDollar(10);
    readonly FastMoney _euro10fast = new(10, "EUR");
    readonly FastMoney _euro10fastSame = new(10, "EUR");
    readonly FastMoney _euro20fast = new(20, "EUR");
    readonly FastMoney _dollar10fast = new(10, "USD");

    [BenchmarkCategory("Equal"), Benchmark(Baseline = true)]
    public bool dEqual() => _dec10 == _dec10Same; // true

    [BenchmarkCategory("Equal"), Benchmark]
    public bool Equal() => _euro10 == _euro10Same; // true

    [BenchmarkCategory("Equal"), Benchmark]
    public bool fEqual() => _euro10fast == _euro10fastSame; // true

    [BenchmarkCategory("NotEqual"), Benchmark(Baseline = true)]
    public bool dNotEqualValue() => _dec10 == _dec20; // false

    [BenchmarkCategory("NotEqual"), Benchmark]
    public bool NotEqualValue() => _euro10 == _euro20; // false

    [BenchmarkCategory("NotEqual"), Benchmark]
    public bool NotEqualCurrency() => _euro10 == _dollar10; // false

    [BenchmarkCategory("NotEqual"), Benchmark]
    public bool fNotEqualValue() => _euro10fast == _euro20fast; // false

    [BenchmarkCategory("NotEqual"), Benchmark]
    public bool fNotEqualCurrency() => _euro10fast == _dollar10fast; // false

    [BenchmarkCategory("GreaterOrEqual"), Benchmark(Baseline = true)]
    public bool dEqualOrBigger() => _dec20 >= _dec10; // true

    [BenchmarkCategory("GreaterOrEqual"), Benchmark]
    public bool EqualOrBigger() => _euro20 >= _euro10; // true

    [BenchmarkCategory("GreaterOrEqual"), Benchmark]
    public bool fEqualOrBigger() => _euro20fast >= _euro10fast; // true

    [BenchmarkCategory("Greater"), Benchmark(Baseline = true)]
    public bool dBigger() => _dec20 > _dec10; // true

    [BenchmarkCategory("Greater"), Benchmark]
    public bool Bigger() => _euro20 > _euro10; // true

    [BenchmarkCategory("Greater"), Benchmark]
    public bool fBigger() => _euro20fast > _euro10fast; // true
}
