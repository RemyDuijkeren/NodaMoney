```

BenchmarkDotNet v0.15.6, Windows 11 (10.0.26200.7171)
AMD Ryzen 7 5800H with Radeon Graphics 3.20GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 10.0.100
  [Host]     : .NET 10.0.0 (10.0.0, 10.0.25.52411), X64 RyuJIT x86-64-v3
  DefaultJob : .NET 10.0.0 (10.0.0, 10.0.25.52411), X64 RyuJIT x86-64-v3


```
| Method                         | Mean     | Error   | Op/s        | Gen0   | Allocated |
|------------------------------- |---------:|--------:|------------:|-------:|----------:|
| DefaultFormat                  | 115.0 ns | 2.30 ns | 8,698,600.7 | 0.0458 |     384 B |
| FormatWithPrecision            | 188.9 ns | 1.70 ns | 5,294,420.3 | 0.0572 |     480 B |
| FormatProvider                 | 106.0 ns | 2.13 ns | 9,432,282.7 | 0.0459 |     384 B |
| FormatWithPrecisionAndProvider | 193.5 ns | 1.88 ns | 5,168,147.6 | 0.0572 |     480 B |
| CompactFormat                  |       NA |      NA |          NA |     NA |        NA |
| GeneralFormat                  | 166.4 ns | 3.39 ns | 6,009,638.1 | 0.0842 |     704 B |
| RondTripFormat                 | 117.0 ns | 2.36 ns | 8,543,902.3 | 0.0516 |     432 B |

Benchmarks with issues:
  MoneyFormattingBenchmarks.CompactFormat: DefaultJob
