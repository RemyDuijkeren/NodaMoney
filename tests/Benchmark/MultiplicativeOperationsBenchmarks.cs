using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using NodaMoney;

namespace Benchmark;

/// <summary>Multiply and divide for <c>decimal</c>, <see cref="Money"/> and <see cref="FastMoney"/>, by a fractional
/// decimal, a whole-number decimal and a long. Grouped per operation with the <c>decimal</c> variant as baseline, so the
/// Ratio column reads as "times a raw decimal".</summary>
[MemoryDiagnoser]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
[CategoriesColumn]
public class MultiplicativeOperationsBenchmarks
{
    readonly decimal _dec10 = 10m;
    readonly Money _euro10 = Money.Euro(10);
    readonly FastMoney _euro10fast = new(10, "EUR");

    [BenchmarkCategory("Multiply"), Benchmark(Baseline = true)]
    public decimal dMultiple() => _dec10 * 2.2m;

    [BenchmarkCategory("Multiply"), Benchmark]
    public Money Multiple() => _euro10 * 2.2m;

    [BenchmarkCategory("Multiply"), Benchmark]
    public FastMoney fMultipleDec() => _euro10fast * 2.2m;

    [BenchmarkCategory("Multiply"), Benchmark]
    public FastMoney fMultipleDecWholeNumber() => _euro10fast * 2m;

    [BenchmarkCategory("Multiply"), Benchmark]
    public FastMoney fMultipleLong() => _euro10fast * 2L;

    [BenchmarkCategory("Divide"), Benchmark(Baseline = true)]
    public decimal dDivide() => _dec10 / 2.2m;

    [BenchmarkCategory("Divide"), Benchmark]
    public Money Divide() => _euro10 / 2.2m;

    [BenchmarkCategory("Divide"), Benchmark]
    public FastMoney fDivideDec() => _euro10fast / 2.2m;

    [BenchmarkCategory("Divide"), Benchmark]
    public FastMoney fDivideDecWholeNumber() => _euro10fast / 2m;

    [BenchmarkCategory("Divide"), Benchmark]
    public FastMoney fDivideLong() => _euro10fast / 2L;
}
