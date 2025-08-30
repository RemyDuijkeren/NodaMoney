```

BenchmarkDotNet v0.15.2, Windows 11 (10.0.26100.4946/24H2/2024Update/HudsonValley)
AMD Ryzen 7 5800H with Radeon Graphics 3.20GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 10.0.100-preview.7.25380.108
  [Host]     : .NET 9.0.8 (9.0.825.36511), X64 RyuJIT AVX2
  DefaultJob : .NET 9.0.8 (9.0.825.36511), X64 RyuJIT AVX2


```
| Method                        | Mean      | Error     | StdDev    | Op/s          | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
|------------------------------ |----------:|----------:|----------:|--------------:|------:|--------:|-------:|----------:|------------:|
| CurrencyCode                  | 38.254 ns | 0.5405 ns | 0.5056 ns |  26,140,954.9 |  1.00 |    0.02 |      - |         - |          NA |
| fCurrencyCode                 | 33.994 ns | 0.5416 ns | 0.4522 ns |  29,416,951.4 |  0.89 |    0.02 |      - |         - |          NA |
| CurrencyCodeAndRoundingMode   | 44.330 ns | 0.3970 ns | 0.3520 ns |  22,557,911.1 |  1.16 |    0.02 |      - |         - |          NA |
| CurrencyCodeAndContext        | 42.655 ns | 0.2374 ns | 0.2221 ns |  23,443,999.0 |  1.12 |    0.02 |      - |         - |          NA |
| CurrencyFromCode              | 37.422 ns | 0.2613 ns | 0.2316 ns |  26,722,192.3 |  0.98 |    0.01 |      - |         - |          NA |
| CurrencyInfoFromCode          | 37.151 ns | 0.2870 ns | 0.2544 ns |  26,917,085.2 |  0.97 |    0.01 |      - |         - |          NA |
| ExtensionMethodEuro           | 37.006 ns | 0.4006 ns | 0.3128 ns |  27,022,575.9 |  0.97 |    0.01 |      - |         - |          NA |
| ImplicitCurrencyByConstructor | 84.376 ns | 1.5994 ns | 1.5708 ns |  11,851,753.2 |  2.21 |    0.05 | 0.0038 |      32 B |          NA |
| ImplicitCurrencyByCasting     | 82.875 ns | 1.6222 ns | 1.4381 ns |  12,066,352.9 |  2.17 |    0.05 | 0.0038 |      32 B |          NA |
| Deconstruct                   |  1.040 ns | 0.0248 ns | 0.0232 ns | 961,394,287.4 |  0.03 |    0.00 |      - |         - |          NA |
