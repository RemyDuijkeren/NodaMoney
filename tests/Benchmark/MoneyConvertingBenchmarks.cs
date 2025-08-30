using System.Data.SqlTypes;
using BenchmarkDotNet.Attributes;
using NodaMoney;

namespace Benchmark;

[MemoryDiagnoser]
public class MoneyConvertingBenchmarks
{
    readonly Money _euro = new(765.43m, "EUR");
    readonly FastMoney _euroFast = new (765.43m, "EUR");

    [Benchmark(Baseline = true)]
    public decimal ToDecimal()
    {
        return _euro.ToDecimal();
    }

    [Benchmark]
    public double ToDouble()
    {
        return _euro.ToDouble();
    }

    [Benchmark]
    public int ToIn32()
    {
        return _euro.ToInt32();
    }

    [Benchmark]
    public long ToInt64()
    {
        return _euro.ToInt64();
    }

    [Benchmark]
    public FastMoney ToFastMoney()
    {
        return FastMoney.FromMoney(_euro);
    }

    [Benchmark]
    public decimal fToDecimal()
    {
        return _euroFast.ToDecimal();
    }

    [Benchmark]
    public double fToDouble()
    {
        return _euroFast.ToDouble();
    }

    [Benchmark]
    public int fToIn32()
    {
        return _euroFast.ToInt32();
    }

    [Benchmark]
    public long fToInt64()
    {
        return _euroFast.ToInt64();
    }

    [Benchmark]
    public Money fToMoney()
    {
        return _euroFast.ToMoney();
    }

    [Benchmark]
    public SqlMoney fToSqlMoney()
    {
        return _euroFast.ToSqlMoney();
    }

    [Benchmark]
    public long fToAOCurrency()
    {
        return _euroFast.ToOACurrency();
    }
}
