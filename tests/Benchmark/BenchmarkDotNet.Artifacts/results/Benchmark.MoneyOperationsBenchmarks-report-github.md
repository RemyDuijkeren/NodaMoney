```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.7462/25H2/2025Update/HudsonValley2)
AMD Ryzen 7 5800H with Radeon Graphics 3.20GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 10.0.101
  [Host]     : .NET 10.0.1 (10.0.1, 10.0.125.57005), X64 RyuJIT x86-64-v3
  DefaultJob : .NET 10.0.1 (10.0.1, 10.0.125.57005), X64 RyuJIT x86-64-v3


```
| Method                  |      Mean |     Error |          Op/s | Ratio | Allocated | Alloc Ratio |
|-------------------------|----------:|----------:|--------------:|------:|----------:|------------:|
| Add                     | 22.658 ns | 0.2233 ns |  44,134,477.1 | 11.66 |         - |          NA |
| Subtract                | 22.469 ns | 0.3121 ns |  44,505,735.9 | 11.56 |         - |          NA |
| Multiple                | 22.969 ns | 0.3151 ns |  43,536,290.5 | 11.82 |         - |          NA |
| Divide                  | 62.319 ns | 0.4148 ns |  16,046,410.7 | 32.07 |         - |          NA |
| Increment               |  9.060 ns | 0.1538 ns | 110,370,651.0 |  4.66 |         - |          NA |
| Decrement               |  9.150 ns | 0.0850 ns | 109,292,714.3 |  4.71 |         - |          NA |
| Remainder               | 19.964 ns | 0.0962 ns |  50,089,749.6 | 10.27 |         - |          NA |
| fAdd                    |  1.943 ns | 0.0274 ns | 514,543,168.8 |  1.00 |         - |          NA |
| fSubtract               |  1.704 ns | 0.0363 ns | 586,992,554.2 |  0.88 |         - |          NA |
| fMultipleDec            | 22.439 ns | 0.0806 ns |  44,565,077.2 | 11.55 |         - |          NA |
| fMultipleDecWholeNumber |  9.019 ns | 0.0779 ns | 110,879,706.1 |  4.64 |         - |          NA |
| fMultipleLong           |  2.071 ns | 0.0076 ns | 482,747,298.6 |  1.07 |         - |          NA |
| fDivideDec              | 67.947 ns | 1.1503 ns |  14,717,453.6 | 34.97 |         - |          NA |
| fDivideDecWholeNumber   | 10.327 ns | 0.0316 ns |  96,836,532.2 |  5.31 |         - |          NA |
| fDivideLong             |  1.905 ns | 0.0097 ns | 524,836,563.0 |  0.98 |         - |          NA |
| fIncrement              |  5.068 ns | 0.0969 ns | 197,327,456.3 |  2.61 |         - |          NA |
| fDecrement              |  4.994 ns | 0.0314 ns | 200,234,138.1 |  2.57 |         - |          NA |
| fRemainder              |  1.879 ns | 0.0300 ns | 532,138,838.7 |  0.97 |         - |          NA |
