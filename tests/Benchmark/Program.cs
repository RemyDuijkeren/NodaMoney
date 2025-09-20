using Benchmark;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Running;

// Configure BDN to export only GitHub-flavored Markdown
var config = ManualConfig.CreateEmpty()
                         .AddJob(Job.Default)
                         .AddLogger(ConsoleLogger.Default)
                         .AddExporter(MarkdownExporter.GitHub)
                         .AddColumnProvider(DefaultColumnProviders.Instance)   // add columns like Mean, Error, StdDev, etc.
                         .HideColumns("StdDev", "Median", "RatioSD")
                         .AddColumn(StatisticColumn.OperationsPerSecond)
                         .AddDiagnoser(MemoryDiagnoser.Default);
                         //.WithArtifactsPath(Path.GetFullPath("artifacts"));

// Run all benchmarks in the assembly; honors command-line args (e.g., --runtimes)
BenchmarkSwitcher.FromAssembly(typeof(HighLoadBenchmarks).Assembly).Run(args, config);
