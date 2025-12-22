```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.7462/25H2/2025Update/HudsonValley2)
AMD Ryzen 7 5800H with Radeon Graphics 3.20GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 10.0.101
  [Host]     : .NET 10.0.1 (10.0.1, 10.0.125.57005), X64 RyuJIT x86-64-v3
  DefaultJob : .NET 10.0.1 (10.0.1, 10.0.125.57005), X64 RyuJIT x86-64-v3


```
| Method            |     Mean |   Error |        Op/s |   Gen0 | Allocated |
|-------------------|---------:|--------:|------------:|-------:|----------:|
| Implicit          | 140.9 ns | 2.75 ns | 7,094,816.6 | 0.0191 |     160 B |
| ImplicitTry       | 171.9 ns | 1.06 ns | 5,818,675.1 | 0.0191 |     160 B |
| Explicit          | 160.4 ns | 0.82 ns | 6,233,117.8 | 0.0191 |     160 B |
| ExplicitAsSpan    | 188.0 ns | 2.27 ns | 5,319,105.3 | 0.0191 |     160 B |
| ExplicitTry       | 171.5 ns | 0.76 ns | 5,831,963.5 | 0.0191 |     160 B |
| ExplicitTryAsSpan | 191.9 ns | 1.00 ns | 5,210,301.6 | 0.0191 |     160 B |
