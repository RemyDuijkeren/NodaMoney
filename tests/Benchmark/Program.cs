using Benchmark;
using BenchmarkDotNet.Running;

// Replace multiple explicit runs with a single switcher that honors command-line args
BenchmarkSwitcher.FromAssembly(typeof(HighLoadBenchmarks).Assembly).Run(args);

// BenchmarkRunner.Run<HighLoadBenchmarks>();
// BenchmarkRunner.Run<InitializingCurrencyBenchmarks>();
// BenchmarkRunner.Run<InitializingMoneyBenchmarks>();
// BenchmarkRunner.Run<MoneyOperationsBenchmarks>();
// BenchmarkRunner.Run<MoneyFormattingBenchmarks>();
// BenchmarkRunner.Run<MoneyParsingBenchmarks>();
