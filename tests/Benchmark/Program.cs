using BenchmarkDotNet.Running;

namespace Benchmark;

static class Program
{
    static void Main(string[] args)
    {
        BenchmarkRunner.Run<HighLoadBenchmarks>();
        BenchmarkRunner.Run<InitializingCurrencyBenchmarks>();
        BenchmarkRunner.Run<InitializingMoneyBenchmarks>();
        BenchmarkRunner.Run<MoneyOperationsBenchmarks>();
        BenchmarkRunner.Run<MoneyFormattingBenchmarks>();
        BenchmarkRunner.Run<MoneyParsingBenchmarks>();
    }
}
