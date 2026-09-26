```

BenchmarkDotNet v0.15.8, Linux Omarchy
AMD Ryzen 7 8845HS w/ Radeon 780M Graphics 1.09GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 10.0.401
  [Host]     : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v4
  DefaultJob : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v4


```
| Method            | Mean     | Error    | Op/s         | Gen0   | Allocated |
|------------------ |---------:|---------:|-------------:|-------:|----------:|
| Implicit          | 72.55 ns | 0.558 ns | 13,783,684.7 | 0.0029 |      24 B |
| ImplicitTry       | 77.14 ns | 0.526 ns | 12,964,156.7 | 0.0029 |      24 B |
| Explicit          | 77.62 ns | 0.360 ns | 12,882,958.1 | 0.0029 |      24 B |
| ExplicitAsSpan    | 74.00 ns | 0.318 ns | 13,514,164.5 | 0.0029 |      24 B |
| ExplicitTry       | 89.35 ns | 0.466 ns | 11,192,107.9 | 0.0029 |      24 B |
| ExplicitTryAsSpan | 90.91 ns | 0.588 ns | 10,999,539.6 | 0.0029 |      24 B |
