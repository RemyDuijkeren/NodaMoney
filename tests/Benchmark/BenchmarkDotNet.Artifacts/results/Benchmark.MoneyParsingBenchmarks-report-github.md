```

BenchmarkDotNet v0.15.8, Linux Omarchy
AMD Ryzen 7 8845HS w/ Radeon 780M Graphics 3.19GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 10.0.401
  [Host]     : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v4
  DefaultJob : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v4


```
| Method            | Mean     | Error   | Op/s        | Gen0   | Allocated |
|------------------ |---------:|--------:|------------:|-------:|----------:|
| Implicit          | 101.7 ns | 0.39 ns | 9,832,950.5 | 0.0191 |     160 B |
| ImplicitTry       | 113.3 ns | 0.27 ns | 8,827,052.4 | 0.0191 |     160 B |
| Explicit          | 113.0 ns | 0.80 ns | 8,846,720.3 | 0.0191 |     160 B |
| ExplicitAsSpan    | 102.3 ns | 0.79 ns | 9,773,753.7 | 0.0191 |     160 B |
| ExplicitTry       | 136.5 ns | 1.69 ns | 7,327,137.6 | 0.0191 |     160 B |
| ExplicitTryAsSpan | 129.2 ns | 1.35 ns | 7,739,427.9 | 0.0191 |     160 B |
