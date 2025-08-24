```

BenchmarkDotNet v0.15.2, Windows 11 (10.0.26100.4946/24H2/2024Update/HudsonValley)
AMD Ryzen 7 5800H with Radeon Graphics 3.20GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 10.0.100-preview.7.25380.108
  [Host]     : .NET 9.0.8 (9.0.825.36511), X64 RyuJIT AVX2
  DefaultJob : .NET 9.0.8 (9.0.825.36511), X64 RyuJIT AVX2


```
| Method            | Mean     | Error   | StdDev  | P95      | Op/s        | Gen0   | Allocated |
|------------------ |---------:|--------:|--------:|---------:|------------:|-------:|----------:|
| Implicit          | 383.8 ns | 6.06 ns | 5.37 ns | 391.3 ns | 2,605,394.7 | 0.0801 |     672 B |
| ImplicitTry       | 370.7 ns | 6.75 ns | 6.31 ns | 380.2 ns | 2,697,790.3 | 0.0801 |     672 B |
| Explicit          | 395.9 ns | 5.53 ns | 4.62 ns | 402.5 ns | 2,525,750.5 | 0.0801 |     672 B |
| ExplicitAsSpan    | 412.3 ns | 8.10 ns | 7.96 ns | 427.2 ns | 2,425,162.2 | 0.0801 |     672 B |
| ExplicitTry       | 421.2 ns | 8.38 ns | 8.61 ns | 435.6 ns | 2,374,036.0 | 0.0801 |     672 B |
| ExplicitTryAsSpan | 391.4 ns | 7.21 ns | 7.08 ns | 403.2 ns | 2,554,645.2 | 0.0801 |     672 B |
