```

BenchmarkDotNet v0.15.2, Windows 11 (10.0.26100.4946/24H2/2024Update/HudsonValley)
AMD Ryzen 7 5800H with Radeon Graphics 3.20GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 10.0.100-preview.7.25380.108
  [Host]     : .NET 9.0.8 (9.0.825.36511), X64 RyuJIT AVX2
  DefaultJob : .NET 9.0.8 (9.0.825.36511), X64 RyuJIT AVX2


```
| Method             | Mean     | Error   | StdDev  | P95      | Op/s        | Gen0   | Allocated |
|------------------- |---------:|--------:|--------:|---------:|------------:|-------:|----------:|
| Implicit           | 115.5 ns | 2.36 ns | 4.88 ns | 125.5 ns | 8,657,442.3 | 0.0459 |     384 B |
| ImplicitWithFormat | 144.3 ns | 2.81 ns | 3.66 ns | 149.9 ns | 6,931,054.6 | 0.0496 |     416 B |
| Explicit           | 119.0 ns | 2.42 ns | 4.89 ns | 128.3 ns | 8,405,527.7 | 0.0459 |     384 B |
| ExplicitWithFormat | 149.4 ns | 3.01 ns | 2.95 ns | 153.1 ns | 6,695,496.3 | 0.0496 |     416 B |
