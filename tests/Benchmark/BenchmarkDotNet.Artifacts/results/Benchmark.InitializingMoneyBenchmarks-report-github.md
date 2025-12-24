```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.7462/25H2/2025Update/HudsonValley2)
AMD Ryzen 7 5800H with Radeon Graphics 3.20GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 10.0.101
  [Host]     : .NET 10.0.1 (10.0.1, 10.0.125.57005), X64 RyuJIT x86-64-v3
  DefaultJob : .NET 10.0.1 (10.0.1, 10.0.125.57005), X64 RyuJIT x86-64-v3


```
| Method                        |      Mean |     Error |          Op/s | Ratio |   Gen0 | Allocated | Alloc Ratio |
|-------------------------------|----------:|----------:|--------------:|------:|-------:|----------:|------------:|
| CurrencyCode                  | 22.885 ns | 0.1001 ns |  43,697,052.7 |  1.00 |      - |         - |          NA |
| fCurrencyCode                 | 24.022 ns | 0.1156 ns |  41,629,244.8 |  1.05 |      - |         - |          NA |
| CurrencyCodeAndRoundingMode   | 22.795 ns | 0.0662 ns |  43,868,504.6 |  1.00 |      - |         - |          NA |
| CurrencyCodeAndContext        | 22.494 ns | 0.4495 ns |  44,455,825.4 |  0.98 |      - |         - |          NA |
| CurrencyFromCode              | 22.284 ns | 0.0690 ns |  44,875,048.7 |  0.97 |      - |         - |          NA |
| CurrencyInfoFromCode          | 22.613 ns | 0.2680 ns |  44,221,994.4 |  0.99 |      - |         - |          NA |
| ExtensionMethodEuro           | 22.939 ns | 0.0815 ns |  43,594,333.8 |  1.00 |      - |         - |          NA |
| ImplicitCurrencyByConstructor | 66.292 ns | 0.1979 ns |  15,084,665.3 |  2.90 | 0.0038 |      32 B |          NA |
| ImplicitCurrencyByCasting     | 66.602 ns | 0.4217 ns |  15,014,561.8 |  2.91 | 0.0038 |      32 B |          NA |
| Deconstruct                   |  1.089 ns | 0.0481 ns | 918,181,035.2 |  0.05 |      - |         - |          NA |
