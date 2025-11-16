```

BenchmarkDotNet v0.15.6, Windows 11 (10.0.26200.7171)
AMD Ryzen 7 5800H with Radeon Graphics 3.20GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 10.0.100
  [Host]     : .NET 10.0.0 (10.0.0, 10.0.25.52411), X64 RyuJIT x86-64-v3
  DefaultJob : .NET 10.0.0 (10.0.0, 10.0.25.52411), X64 RyuJIT x86-64-v3


```
| Method                        | Mean      | Error     | Op/s          | Ratio | Gen0   | Allocated | Alloc Ratio |
|------------------------------ |----------:|----------:|--------------:|------:|-------:|----------:|------------:|
| CurrencyCode                  | 28.564 ns | 0.5243 ns |  35,008,927.4 |  1.00 |      - |         - |          NA |
| fCurrencyCode                 | 22.776 ns | 0.2386 ns |  43,905,942.1 |  0.80 |      - |         - |          NA |
| CurrencyCodeAndRoundingMode   | 30.179 ns | 0.6230 ns |  33,135,089.8 |  1.06 |      - |         - |          NA |
| CurrencyCodeAndContext        | 29.527 ns | 0.5400 ns |  33,867,685.3 |  1.03 |      - |         - |          NA |
| CurrencyFromCode              | 28.356 ns | 0.5939 ns |  35,265,913.1 |  0.99 |      - |         - |          NA |
| CurrencyInfoFromCode          | 28.415 ns | 0.5931 ns |  35,193,217.2 |  1.00 |      - |         - |          NA |
| ExtensionMethodEuro           | 28.590 ns | 0.3620 ns |  34,976,875.1 |  1.00 |      - |         - |          NA |
| ImplicitCurrencyByConstructor | 73.518 ns | 1.5096 ns |  13,602,182.8 |  2.57 | 0.0038 |      32 B |          NA |
| ImplicitCurrencyByCasting     | 74.238 ns | 1.4504 ns |  13,470,267.0 |  2.60 | 0.0038 |      32 B |          NA |
| Deconstruct                   |  1.108 ns | 0.0485 ns | 902,338,512.8 |  0.04 |      - |         - |          NA |
