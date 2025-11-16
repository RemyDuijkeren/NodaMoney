```

BenchmarkDotNet v0.15.6, Windows 11 (10.0.26200.7171)
AMD Ryzen 7 5800H with Radeon Graphics 3.20GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 10.0.100
  [Host]     : .NET 10.0.0 (10.0.0, 10.0.25.52411), X64 RyuJIT x86-64-v3
  DefaultJob : .NET 10.0.0 (10.0.0, 10.0.25.52411), X64 RyuJIT x86-64-v3


```
| Method            | Mean     | Error   | Op/s        | Gen0   | Allocated |
|------------------ |---------:|--------:|------------:|-------:|----------:|
| Implicit          | 155.8 ns | 1.68 ns | 6,419,477.6 | 0.0191 |     160 B |
| ImplicitTry       | 184.7 ns | 3.47 ns | 5,412,967.7 | 0.0191 |     160 B |
| Explicit          | 159.3 ns | 1.22 ns | 6,279,071.2 | 0.0191 |     160 B |
| ExplicitAsSpan    | 181.6 ns | 0.66 ns | 5,507,450.0 | 0.0191 |     160 B |
| ExplicitTry       | 167.6 ns | 2.89 ns | 5,967,989.4 | 0.0191 |     160 B |
| ExplicitTryAsSpan | 165.3 ns | 1.93 ns | 6,051,351.3 | 0.0191 |     160 B |
