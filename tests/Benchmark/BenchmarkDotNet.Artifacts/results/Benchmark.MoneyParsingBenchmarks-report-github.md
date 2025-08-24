```

BenchmarkDotNet v0.15.2, Windows 11 (10.0.26100.4946/24H2/2024Update/HudsonValley)
AMD Ryzen 7 5800H with Radeon Graphics 3.20GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 10.0.100-preview.7.25380.108
  [Host]     : .NET 9.0.8 (9.0.825.36511), X64 RyuJIT AVX2
  DefaultJob : .NET 9.0.8 (9.0.825.36511), X64 RyuJIT AVX2


```
| Method            | Mean     | Error   | StdDev  | Op/s        | Gen0   | Allocated |
|------------------ |---------:|--------:|--------:|------------:|-------:|----------:|
| Implicit          | 194.2 ns | 3.86 ns | 8.23 ns | 5,148,875.3 | 0.0191 |     160 B |
| ImplicitTry       | 179.1 ns | 2.49 ns | 2.08 ns | 5,583,433.4 | 0.0191 |     160 B |
| Explicit          | 191.2 ns | 3.21 ns | 2.84 ns | 5,229,086.4 | 0.0191 |     160 B |
| ExplicitAsSpan    | 195.2 ns | 3.78 ns | 3.53 ns | 5,121,650.9 | 0.0191 |     160 B |
| ExplicitTry       | 201.1 ns | 3.80 ns | 4.07 ns | 4,972,065.3 | 0.0191 |     160 B |
| ExplicitTryAsSpan | 187.5 ns | 2.05 ns | 1.82 ns | 5,333,879.3 | 0.0191 |     160 B |
