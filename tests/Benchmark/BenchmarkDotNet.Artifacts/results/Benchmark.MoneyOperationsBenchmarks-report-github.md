```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.7462/25H2/2025Update/HudsonValley2)
AMD Ryzen 7 5800H with Radeon Graphics 3.20GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 10.0.101
  [Host]     : .NET 10.0.1 (10.0.1, 10.0.125.57005), X64 RyuJIT x86-64-v3
  DefaultJob : .NET 10.0.1 (10.0.1, 10.0.125.57005), X64 RyuJIT x86-64-v3


```
| Method                  |      Mean |     Error |          Op/s | Ratio | Allocated | Alloc Ratio |
|-------------------------|----------:|----------:|--------------:|------:|----------:|------------:|
| Add                     | 22.086 ns | 0.1029 ns |  45,276,806.7 | 11.45 |         - |          NA |
| Subtract                | 22.706 ns | 0.1814 ns |  44,041,379.2 | 11.77 |         - |          NA |
| Multiple                | 22.399 ns | 0.0854 ns |  44,644,223.6 | 11.61 |         - |          NA |
| Divide                  | 61.619 ns | 0.1808 ns |  16,228,811.3 | 31.95 |         - |          NA |
| Increment               |  9.060 ns | 0.1538 ns | 110,370,651.0 |  4.66 |         - |          NA |
| Decrement               |  9.150 ns | 0.0850 ns | 109,292,714.3 |  4.71 |         - |          NA |
| Remainder               | 21.660 ns | 0.2293 ns |  46,167,046.5 | 11.23 |         - |          NA |
| fAdd                    |  1.929 ns | 0.0161 ns | 518,416,084.4 |  1.00 |         - |          NA |
| fSubtract               |  1.703 ns | 0.0154 ns | 587,254,196.0 |  0.88 |         - |          NA |
| fMultipleDec            | 22.378 ns | 0.0876 ns |  44,686,651.7 | 11.60 |         - |          NA |
| fMultipleDecWholeNumber |  8.962 ns | 0.0270 ns | 111,582,197.6 |  4.65 |         - |          NA |
| fMultipleLong           |  2.098 ns | 0.0113 ns | 476,662,397.6 |  1.09 |         - |          NA |
| fDivideDec              | 67.078 ns | 0.2020 ns |  14,907,930.4 | 34.78 |         - |          NA |
| fDivideDecWholeNumber   | 10.307 ns | 0.0301 ns |  97,021,995.4 |  5.34 |         - |          NA |
| fDivideLong             |  1.922 ns | 0.0092 ns | 520,322,438.4 |  1.00 |         - |          NA |
| fIncrement              |  2.121 ns | 0.0395 ns | 471,373,585.9 |  1.10 |         - |          NA |
| fDecrement              |  2.258 ns | 0.0263 ns | 442,806,522.0 |  1.17 |         - |          NA |
| fRemainder              |  1.957 ns | 0.0510 ns | 511,023,677.5 |  1.01 |         - |          NA |
