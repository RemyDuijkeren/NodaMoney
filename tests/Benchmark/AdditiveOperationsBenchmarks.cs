using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using NodaMoney;

namespace Benchmark;

/// <summary>Add, subtract, increment, decrement and remainder for <c>decimal</c>, <see cref="Money"/> and
/// <see cref="FastMoney"/>. Grouped per operation with the <c>decimal</c> variant as baseline, so the Ratio column
/// reads as "times a raw decimal".</summary>
[MemoryDiagnoser]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
[CategoriesColumn]
public class AdditiveOperationsBenchmarks
{
    readonly decimal _dec10 = 10m;
    readonly decimal _dec20 = 20m;
    decimal _dec = 765.43m;
    readonly Money _euro10 = Money.Euro(10);
    readonly Money _euro20 = Money.Euro(20);
    readonly Money _euro5Half = Money.Euro(5.5m); // scale 1, so adding it to a scale-2 value is a mixed-scale add
    Money _euro = new Money(765.43m, "EUR");
    FastMoney _euro10fast = new(10, "EUR");
    readonly FastMoney _euro20fast = new(20, "EUR");

    [BenchmarkCategory("Add"), Benchmark(Baseline = true)]
    public decimal dAdd() => _dec10 + _dec20;

    [BenchmarkCategory("Add"), Benchmark]
    public Money Add() => _euro10 + _euro20;

    [BenchmarkCategory("Add"), Benchmark]
    public Money AddMixedScale() => _euro10 + _euro5Half;

    [BenchmarkCategory("Add"), Benchmark]
    public FastMoney fAdd() => FastMoney.Add(_euro10fast, _euro20fast);

    [BenchmarkCategory("Subtract"), Benchmark(Baseline = true)]
    public decimal dSubtract() => _dec20 - _dec10;

    [BenchmarkCategory("Subtract"), Benchmark]
    public Money Subtract() => _euro20 - _euro10;

    [BenchmarkCategory("Subtract"), Benchmark]
    public FastMoney fSubtract() => FastMoney.Subtract(_euro20fast, _euro10fast);

    [BenchmarkCategory("Increment"), Benchmark(Baseline = true)]
    public decimal dIncrement() => ++_dec;

    [BenchmarkCategory("Increment"), Benchmark]
    public Money Increment() => ++_euro;

    [BenchmarkCategory("Increment"), Benchmark]
    public FastMoney fIncrement() => ++_euro10fast;

    [BenchmarkCategory("Decrement"), Benchmark(Baseline = true)]
    public decimal dDecrement() => --_dec;

    [BenchmarkCategory("Decrement"), Benchmark]
    public Money Decrement() => --_euro;

    [BenchmarkCategory("Decrement"), Benchmark]
    public FastMoney fDecrement() => --_euro10fast;

    [BenchmarkCategory("Remainder"), Benchmark(Baseline = true)]
    public decimal dRemainder() => _dec20 % _dec10;

    [BenchmarkCategory("Remainder"), Benchmark]
    public Money Remainder() => _euro20 % _euro10;

    [BenchmarkCategory("Remainder"), Benchmark]
    public FastMoney fRemainder() => _euro20fast % _euro10fast;
}
