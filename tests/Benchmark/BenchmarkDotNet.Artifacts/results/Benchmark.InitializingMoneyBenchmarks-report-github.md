```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.7462/25H2/2025Update/HudsonValley2)
AMD Ryzen 7 5800H with Radeon Graphics 3.20GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 10.0.101
  [Host]     : .NET 10.0.1 (10.0.1, 10.0.125.57005), X64 RyuJIT x86-64-v3
  DefaultJob : .NET 10.0.1 (10.0.1, 10.0.125.57005), X64 RyuJIT x86-64-v3


```
| Method                        |      Mean |     Error |          Op/s | Ratio |   Gen0 | Allocated | Alloc Ratio |
|-------------------------------|----------:|----------:|--------------:|------:|-------:|----------:|------------:|
| CurrencyCode                  | 22.449 ns | 0.4277 ns |  44,544,486.1 |  1.00 |      - |         - |          NA |
| fCurrencyCode                 | 21.731 ns | 0.4274 ns |  46,017,879.8 |  0.97 |      - |         - |          NA |
| CurrencyCodeAndRoundingMode   | 23.087 ns | 0.3984 ns |  43,314,105.8 |  1.03 |      - |         - |          NA |
| CurrencyCodeAndContext        | 22.404 ns | 0.4712 ns |  44,634,051.4 |  1.00 |      - |         - |          NA |
| CurrencyFromCode              | 22.850 ns | 0.4821 ns |  43,762,734.3 |  1.02 |      - |         - |          NA |
| CurrencyInfoFromCode          | 23.299 ns | 0.4751 ns |  42,920,467.4 |  1.04 |      - |         - |          NA |
| ExtensionMethodEuro           | 23.013 ns | 0.3758 ns |  43,453,755.3 |  1.03 |      - |         - |          NA |
| ImplicitCurrencyByConstructor | 66.579 ns | 0.9321 ns |  15,019,827.7 |  2.97 | 0.0038 |      32 B |          NA |
| ImplicitCurrencyByCasting     | 66.723 ns | 0.4570 ns |  14,987,234.1 |  2.97 | 0.0038 |      32 B |          NA |
| Deconstruct                   |  1.951 ns | 0.0327 ns | 512,596,562.7 |  0.09 |      - |         - |          NA |
