using System.Globalization;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using NodaMoney;

namespace Benchmark;

/// <summary>Formatting a <see cref="Money"/> next to formatting the same amount as a <c>decimal</c> with the matching
/// format string. Grouped per format with the <c>decimal</c> variant as baseline, so the Ratio column reads as
/// "times a raw decimal".</summary>
[MemoryDiagnoser]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
[CategoriesColumn]
public class MoneyFormattingBenchmarks
{
    readonly decimal _dec = 765.43m;
    readonly Money _euro = new Money(765.43m, "EUR");
    readonly CultureInfo ci = new CultureInfo("nl-NL");

    [BenchmarkCategory("Currency"), Benchmark(Baseline = true)]
    public string dCurrencyFormat() => _dec.ToString("C");

    [BenchmarkCategory("Currency"), Benchmark]
    public string DefaultFormat() => _euro.ToString();

    [BenchmarkCategory("Currency"), Benchmark]
    public string FormatWithPrecision() => _euro.ToString("C2");

    [BenchmarkCategory("Currency"), Benchmark]
    public string CompactFormat() => _euro.ToString("c");

    [BenchmarkCategory("CurrencyWithProvider"), Benchmark(Baseline = true)]
    public string dCurrencyFormatProvider() => _dec.ToString("C", ci);

    [BenchmarkCategory("CurrencyWithProvider"), Benchmark]
    public string FormatProvider() => _euro.ToString(ci);

    [BenchmarkCategory("CurrencyWithProvider"), Benchmark]
    public string FormatWithPrecisionAndProvider() => _euro.ToString("C2", ci);

    [BenchmarkCategory("General"), Benchmark(Baseline = true)]
    public string dGeneralFormat() => _dec.ToString("G");

    [BenchmarkCategory("General"), Benchmark]
    public string GeneralFormat() => _euro.ToString("G");

    [BenchmarkCategory("RoundTrip"), Benchmark(Baseline = true)]
    public string dRoundTripFormat() => _dec.ToString("R");

    [BenchmarkCategory("RoundTrip"), Benchmark]
    public string RondTripFormat() => _euro.ToString("R");
}
