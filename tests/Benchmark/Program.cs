using BenchmarkDotNet.Running;
using Benchmark;

var initializingCurrencyReport = BenchmarkRunner.Run<InitializingCurrencyBenchmarks>();
var initializingMoneyReport = BenchmarkRunner.Run<InitializingMoneyBenchmarks>();
var moneyOperationsReport = BenchmarkRunner.Run<MoneyOperationsBenchmarks>();
var moneyFormattingReport = BenchmarkRunner.Run<MoneyFormattingBenchmarks>();
var moneyParsingReport = BenchmarkRunner.Run<MoneyParsingBenchmarks>();
var addingCustomCurrencyReport = BenchmarkRunner.Run<CreateCustomCurrencyBenchmarks>();
var highLoadBenchReport = BenchmarkRunner.Run<HighLoadBenchmarks>();
