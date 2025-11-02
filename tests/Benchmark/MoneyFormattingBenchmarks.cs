using System.Globalization;
using BenchmarkDotNet.Attributes;
using NodaMoney;

namespace Benchmark;

[MemoryDiagnoser]
public class MoneyFormattingBenchmarks
{
    readonly Money _euro = new Money(765.43m, "EUR");
    readonly CultureInfo ci = new CultureInfo("nl-NL");

    [Benchmark]
    public string DefaultFormat()
    {
        return _euro.ToString();
    }

    [Benchmark]
    public string FormatWithPrecision()
    {
        return _euro.ToString("c2");
    }

    [Benchmark]
    public string FormatProvider()
    {
        return _euro.ToString(ci);
    }

    [Benchmark]
    public string FormatWithPrecisionAndProvider()
    {
        return _euro.ToString("c2", ci);
    }

    [Benchmark]
    public string CompactFormat()
    {
        return _euro.ToString("K");
    }

    [Benchmark]
    public string GeneralFormat()
    {
        return _euro.ToString("G");
    }

    [Benchmark]
    public string RondTripFormat()
    {
        return _euro.ToString("R");
    }
}
