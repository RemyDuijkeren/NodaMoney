using System.Data.SqlTypes;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using NodaMoney;

namespace Benchmark;

/// <summary>Conversions from <see cref="Money"/> and <see cref="FastMoney"/> to the primitive types, <see cref="SqlMoney"/>
/// and OLE Automation Currency, and between the two money types. Grouped per target with the <c>decimal</c> variant as
/// baseline where one exists, so the Ratio column reads as "times a raw decimal".</summary>
[MemoryDiagnoser]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
[CategoriesColumn]
public class MoneyConvertingBenchmarks
{
    readonly decimal _dec = 765.43m;
    readonly Money _euro = new(765.43m, "EUR");
    readonly FastMoney _euroFast = new (765.43m, "EUR");
    readonly SqlMoney _sqlMoney = new(765.43m);
    readonly Currency _eur = CurrencyInfo.FromCode("EUR");

    // A decimal is already a decimal, so Money is the baseline here.
    [BenchmarkCategory("ToDecimal"), Benchmark(Baseline = true)]
    public decimal ToDecimal() => _euro.ToDecimal();

    [BenchmarkCategory("ToDecimal"), Benchmark]
    public decimal fToDecimal() => _euroFast.ToDecimal();

    [BenchmarkCategory("ToDouble"), Benchmark(Baseline = true)]
    public double dToDouble() => Convert.ToDouble(_dec);

    [BenchmarkCategory("ToDouble"), Benchmark]
    public double ToDouble() => _euro.ToDouble();

    [BenchmarkCategory("ToDouble"), Benchmark]
    public double fToDouble() => _euroFast.ToDouble();

    // Convert.ToInt32/ToInt64 round to even, like Money.ToInt32/ToInt64 under the default context.
    [BenchmarkCategory("ToInt32"), Benchmark(Baseline = true)]
    public int dToInt32() => Convert.ToInt32(_dec);

    [BenchmarkCategory("ToInt32"), Benchmark]
    public int ToIn32() => _euro.ToInt32();

    [BenchmarkCategory("ToInt32"), Benchmark]
    public int fToIn32() => _euroFast.ToInt32();

    [BenchmarkCategory("ToInt64"), Benchmark(Baseline = true)]
    public long dToInt64() => Convert.ToInt64(_dec);

    [BenchmarkCategory("ToInt64"), Benchmark]
    public long ToInt64() => _euro.ToInt64();

    [BenchmarkCategory("ToInt64"), Benchmark]
    public long fToInt64() => _euroFast.ToInt64();

    // No decimal analogue for converting between the two money types; Money to FastMoney is the baseline.
    [BenchmarkCategory("MoneyFastMoney"), Benchmark(Baseline = true)]
    public FastMoney ToFastMoney() => new FastMoney(_euro);

    [BenchmarkCategory("MoneyFastMoney"), Benchmark]
    public Money fToMoney() => _euroFast.ToMoney();

    [BenchmarkCategory("ToSqlMoney"), Benchmark(Baseline = true)]
    public SqlMoney dToSqlMoney() => new SqlMoney(_dec);

    [BenchmarkCategory("ToSqlMoney"), Benchmark]
    public SqlMoney fToSqlMoney() => _euroFast.ToSqlMoney();

    [BenchmarkCategory("FromSqlMoney"), Benchmark(Baseline = true)]
    public decimal dFromSqlMoney() => _sqlMoney.Value;

    [BenchmarkCategory("FromSqlMoney"), Benchmark]
    public FastMoney? fFromSqlMoney() => FastMoney.FromSqlMoney(_sqlMoney, _eur);

    [BenchmarkCategory("ToOACurrency"), Benchmark(Baseline = true)]
    public long dToOACurrency() => decimal.ToOACurrency(_dec);

    [BenchmarkCategory("ToOACurrency"), Benchmark]
    public long fToAOCurrency() => _euroFast.ToOACurrency();
}
