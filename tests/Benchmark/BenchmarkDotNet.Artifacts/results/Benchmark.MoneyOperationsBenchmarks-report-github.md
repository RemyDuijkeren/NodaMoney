```

BenchmarkDotNet v0.15.2, Windows 11 (10.0.26100.6584/24H2/2024Update/HudsonValley)
AMD Ryzen 7 5800H with Radeon Graphics 3.20GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 10.0.100-rc.1.25451.107
  [Host]     : .NET 9.0.9 (9.0.925.41916), X64 RyuJIT AVX2
  DefaultJob : .NET 9.0.9 (9.0.925.41916), X64 RyuJIT AVX2


```
| Method                  | Mean      | Error     | Op/s          | Ratio | Allocated | Alloc Ratio |
|------------------------ |----------:|----------:|--------------:|------:|----------:|------------:|
| Add                     | 26.968 ns | 0.2871 ns |  37,080,694.8 | 18.28 |         - |          NA |
| Subtract                | 25.921 ns | 0.1544 ns |  38,579,474.4 | 17.57 |         - |          NA |
| Multiple                | 30.849 ns | 0.4080 ns |  32,416,301.6 | 20.92 |         - |          NA |
| Divide                  | 83.995 ns | 0.4120 ns |  11,905,409.6 | 56.95 |         - |          NA |
| Increment               | 16.407 ns | 0.0855 ns |  60,950,309.8 | 11.12 |         - |          NA |
| Decrement               | 19.793 ns | 0.3293 ns |  50,521,853.8 | 13.42 |         - |          NA |
| Remainder               | 21.764 ns | 0.1380 ns |  45,948,316.5 | 14.76 |         - |          NA |
| fAdd                    |  1.475 ns | 0.0188 ns | 677,926,754.0 |  1.00 |         - |          NA |
| fSubtract               |  1.927 ns | 0.0433 ns | 518,833,833.6 |  1.31 |         - |          NA |
| fMultipleDec            | 28.538 ns | 0.5040 ns |  35,041,170.3 | 19.35 |         - |          NA |
| fMultipleDecWholeNumber | 14.430 ns | 0.1620 ns |  69,301,660.6 |  9.78 |         - |          NA |
| fMultipleLong           |  2.204 ns | 0.0308 ns | 453,750,585.1 |  1.49 |         - |          NA |
| fDivideDec              | 79.456 ns | 0.7799 ns |  12,585,508.5 | 53.87 |         - |          NA |
| fDivideDecWholeNumber   | 12.855 ns | 0.0787 ns |  77,791,781.7 |  8.72 |         - |          NA |
| fDivideLong             |  1.789 ns | 0.0158 ns | 558,992,014.8 |  1.21 |         - |          NA |
| fIncrement              |  6.150 ns | 0.1163 ns | 162,607,271.5 |  4.17 |         - |          NA |
| fDecrement              |  6.125 ns | 0.0587 ns | 163,260,592.0 |  4.15 |         - |          NA |
| fRemainder              |  1.963 ns | 0.0291 ns | 509,443,667.2 |  1.33 |         - |          NA |
