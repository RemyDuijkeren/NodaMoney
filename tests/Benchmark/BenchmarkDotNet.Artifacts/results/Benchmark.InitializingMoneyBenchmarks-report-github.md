```

BenchmarkDotNet v0.15.2, Windows 11 (10.0.26100.4946/24H2/2024Update/HudsonValley)
AMD Ryzen 7 5800H with Radeon Graphics 3.20GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 10.0.100-preview.7.25380.108
  [Host]     : .NET 9.0.8 (9.0.825.36511), X64 RyuJIT AVX2
  DefaultJob : .NET 9.0.8 (9.0.825.36511), X64 RyuJIT AVX2


```
| Method                        | Mean      | Error     | StdDev    | P95       | Op/s          | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
|------------------------------ |----------:|----------:|----------:|----------:|--------------:|------:|--------:|-------:|----------:|------------:|
| CurrencyCode                  | 37.646 ns | 0.3923 ns | 0.3276 ns | 38.119 ns |  26,562,901.4 |  1.00 |    0.01 |      - |         - |          NA |
| CurrencyCodeAndRoundingMode   | 44.154 ns | 0.4830 ns | 0.4282 ns | 44.831 ns |  22,648,133.6 |  1.17 |    0.01 |      - |         - |          NA |
| CurrencyCodeAndContext        | 41.593 ns | 0.5863 ns | 0.5485 ns | 42.543 ns |  24,042,661.1 |  1.10 |    0.02 |      - |         - |          NA |
| CurrencyFromCode              | 37.623 ns | 0.1818 ns | 0.1611 ns | 37.857 ns |  26,579,744.2 |  1.00 |    0.01 |      - |         - |          NA |
| CurrencyInfoFromCode          | 38.475 ns | 0.1707 ns | 0.1513 ns | 38.728 ns |  25,990,782.1 |  1.02 |    0.01 |      - |         - |          NA |
| ExtensionMethodEuro           | 37.693 ns | 0.4837 ns | 0.4288 ns | 38.492 ns |  26,529,951.0 |  1.00 |    0.01 |      - |         - |          NA |
| ImplicitCurrencyByConstructor | 84.934 ns | 1.0002 ns | 0.8352 ns | 86.135 ns |  11,773,846.6 |  2.26 |    0.03 | 0.0038 |      32 B |          NA |
| ImplicitCurrencyByCasting     | 81.604 ns | 0.4188 ns | 0.3917 ns | 82.196 ns |  12,254,245.1 |  2.17 |    0.02 | 0.0038 |      32 B |          NA |
| Deconstruct                   |  1.060 ns | 0.0259 ns | 0.0242 ns |  1.093 ns | 943,520,894.5 |  0.03 |    0.00 |      - |         - |          NA |
