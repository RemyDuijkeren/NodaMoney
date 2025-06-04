using Benchmark;
using BenchmarkDotNet.Running;

BenchmarkRunner.Run<HighLoadBenchmarks>();
BenchmarkRunner.Run<InitializingCurrencyBenchmarks>();
BenchmarkRunner.Run<InitializingMoneyBenchmarks>();
BenchmarkRunner.Run<MoneyOperationsBenchmarks>();
BenchmarkRunner.Run<MoneyFormattingBenchmarks>();
BenchmarkRunner.Run<MoneyParsingBenchmarks>();
