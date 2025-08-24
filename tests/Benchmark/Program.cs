using Benchmark;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Running;

// Configure BDN to export only GitHub-flavored Markdown
var config = ManualConfig.CreateEmpty()
                         .AddJob(Job.Default)
                         .AddLogger(ConsoleLogger.Default)
                         .AddExporter(MarkdownExporter.GitHub);
                         //.WithArtifactsPath(Path.GetFullPath("artifacts"));

// Run all benchmarks in the assembly; honors command-line args (e.g., --runtimes)
BenchmarkSwitcher.FromAssembly(typeof(HighLoadBenchmarks).Assembly).Run(args, config);
