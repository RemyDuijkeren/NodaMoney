```

BenchmarkDotNet v0.15.5, Windows 11 (10.0.26200.7019)
AMD Ryzen 7 5800H with Radeon Graphics 3.20GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 9.0.306
  [Host]     : .NET 9.0.10 (9.0.10, 9.0.1025.47515), X64 RyuJIT x86-64-v3
  DefaultJob : .NET 9.0.10 (9.0.10, 9.0.1025.47515), X64 RyuJIT x86-64-v3


```
| Method                         | Mean     | Error   | Op/s        | Gen0   | Allocated |
|------------------------------- |---------:|--------:|------------:|-------:|----------:|
| DefaultFormat                  | 121.1 ns | 2.41 ns | 8,259,783.1 | 0.0458 |     384 B |
| FormatWithPrecision            | 161.3 ns | 3.12 ns | 6,199,592.9 | 0.0496 |     416 B |
| FormatProvider                 | 113.2 ns | 2.31 ns | 8,834,277.4 | 0.0459 |     384 B |
| FormatWithPrecisionAndProvider | 147.0 ns | 2.90 ns | 6,803,103.3 | 0.0496 |     416 B |
| CompactFormat                  | 230.4 ns | 4.06 ns | 4,340,834.0 | 0.0572 |     480 B |
| GeneralFormat                  | 170.2 ns | 3.46 ns | 5,876,783.1 | 0.0842 |     704 B |
| RondTripFormat                 | 118.1 ns | 2.42 ns | 8,465,497.5 | 0.0515 |     432 B |
