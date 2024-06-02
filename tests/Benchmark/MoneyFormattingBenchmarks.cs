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
    public string Implicit()
    {
        return _euro.ToString();
    }

    [Benchmark]
    public string ImplicitWithFormat()
    {
        return _euro.ToString("C2");
    }

    [Benchmark]
    public string Explicit()
    {
        return _euro.ToString(ci);
    }

    [Benchmark]
    public string ExplicitWithFormat()
    {
        return _euro.ToString("C2", ci);
    }
}