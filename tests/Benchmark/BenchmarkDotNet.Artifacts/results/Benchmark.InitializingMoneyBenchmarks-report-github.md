```

BenchmarkDotNet v0.15.8, Linux Omarchy
AMD Ryzen 7 8845HS w/ Radeon 780M Graphics 1.09GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 10.0.401
  [Host]     : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v4
  DefaultJob : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v4


```
| Method                        | Mean       | Error     | Op/s            | Ratio | Allocated | Alloc Ratio |
|------------------------------ |-----------:|----------:|----------------:|------:|----------:|------------:|
| CurrencyCode                  | 19.4154 ns | 0.0829 ns |    51,505,380.7 |  1.00 |         - |          NA |
| CurrencyCodeNeedsRounding     | 21.5546 ns | 0.0286 ns |    46,393,714.1 |  1.11 |         - |          NA |
| fCurrencyCode                 | 12.6826 ns | 0.0257 ns |    78,848,217.2 |  0.65 |         - |          NA |
| CurrencyCodeAndRoundingMode   | 21.3125 ns | 0.0192 ns |    46,920,755.1 |  1.10 |         - |          NA |
| CurrencyCodeAndContext        | 28.7442 ns | 0.0741 ns |    34,789,631.8 |  1.48 |         - |          NA |
| CurrencyFromCode              | 19.1410 ns | 0.0308 ns |    52,243,941.5 |  0.99 |         - |          NA |
| CurrencyInfoFromCode          | 19.5165 ns | 0.0164 ns |    51,238,614.2 |  1.01 |         - |          NA |
| ExtensionMethodEuro           | 19.2789 ns | 0.0610 ns |    51,870,225.0 |  0.99 |         - |          NA |
| ImplicitCurrencyByConstructor | 15.6066 ns | 0.0272 ns |    64,075,288.7 |  0.80 |         - |          NA |
| ImplicitCurrencyByCasting     | 15.5554 ns | 0.0268 ns |    64,286,154.4 |  0.80 |         - |          NA |
| Deconstruct                   |  0.8231 ns | 0.0060 ns | 1,214,937,912.5 |  0.04 |         - |          NA |
