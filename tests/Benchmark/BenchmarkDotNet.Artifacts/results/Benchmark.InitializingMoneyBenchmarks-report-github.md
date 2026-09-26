```

BenchmarkDotNet v0.15.8, Linux Omarchy
AMD Ryzen 7 8845HS w/ Radeon 780M Graphics 3.19GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 10.0.401
  [Host]     : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v4
  DefaultJob : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v4


```
| Method                        | Mean       | Error     | Op/s            | Ratio | Gen0   | Allocated | Alloc Ratio |
|------------------------------ |-----------:|----------:|----------------:|------:|-------:|----------:|------------:|
| CurrencyCode                  | 21.4583 ns | 0.0750 ns |    46,602,078.4 |  1.00 |      - |         - |          NA |
| fCurrencyCode                 | 22.8582 ns | 0.1077 ns |    43,747,945.3 |  1.07 |      - |         - |          NA |
| CurrencyCodeAndRoundingMode   | 24.0014 ns | 0.1075 ns |    41,664,206.5 |  1.12 |      - |         - |          NA |
| CurrencyCodeAndContext        | 23.1415 ns | 0.0588 ns |    43,212,356.5 |  1.08 |      - |         - |          NA |
| CurrencyFromCode              | 30.3961 ns | 0.4545 ns |    32,898,979.2 |  1.42 |      - |         - |          NA |
| CurrencyInfoFromCode          | 22.9432 ns | 0.0611 ns |    43,585,982.6 |  1.07 |      - |         - |          NA |
| ExtensionMethodEuro           | 21.8405 ns | 0.4381 ns |    45,786,422.3 |  1.02 |      - |         - |          NA |
| ImplicitCurrencyByConstructor | 58.0552 ns | 1.1317 ns |    17,224,972.9 |  2.71 | 0.0038 |      32 B |          NA |
| ImplicitCurrencyByCasting     | 58.3372 ns | 1.0287 ns |    17,141,720.4 |  2.72 | 0.0038 |      32 B |          NA |
| Deconstruct                   |  0.8444 ns | 0.0161 ns | 1,184,282,504.8 |  0.04 |      - |         - |          NA |
