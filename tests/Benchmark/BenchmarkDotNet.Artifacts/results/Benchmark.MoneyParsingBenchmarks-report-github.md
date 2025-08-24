```

BenchmarkDotNet v0.15.2, Windows 11 (10.0.26100.4946/24H2/2024Update/HudsonValley)
AMD Ryzen 7 5800H with Radeon Graphics 3.20GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 10.0.100-preview.7.25380.108
  [Host]     : .NET 9.0.8 (9.0.825.36511), X64 RyuJIT AVX2
  DefaultJob : .NET 9.0.8 (9.0.825.36511), X64 RyuJIT AVX2


```
| Method            | Mean     | Error   | StdDev   | Median   | Op/s        | Gen0   | Allocated |
|------------------ |---------:|--------:|---------:|---------:|------------:|-------:|----------:|
| Implicit          | 185.6 ns | 3.82 ns | 10.96 ns | 181.3 ns | 5,386,586.2 | 0.0191 |     160 B |
| ImplicitTry       | 189.7 ns | 3.79 ns |  5.79 ns | 188.6 ns | 5,270,498.0 | 0.0191 |     160 B |
| Explicit          | 193.2 ns | 3.84 ns |  4.57 ns | 192.3 ns | 5,176,120.3 | 0.0191 |     160 B |
| ExplicitAsSpan    | 191.4 ns | 2.19 ns |  1.94 ns | 191.3 ns | 5,223,378.5 | 0.0191 |     160 B |
| ExplicitTry       | 202.4 ns | 2.89 ns |  2.41 ns | 202.7 ns | 4,939,987.4 | 0.0191 |     160 B |
| ExplicitTryAsSpan | 193.3 ns | 1.90 ns |  1.77 ns | 193.7 ns | 5,173,897.1 | 0.0191 |     160 B |
