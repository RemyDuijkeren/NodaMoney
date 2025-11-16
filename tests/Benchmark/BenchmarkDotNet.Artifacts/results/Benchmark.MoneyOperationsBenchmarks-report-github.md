```

BenchmarkDotNet v0.15.6, Windows 11 (10.0.26200.7171)
AMD Ryzen 7 5800H with Radeon Graphics 3.20GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 10.0.100
  [Host]     : .NET 10.0.0 (10.0.0, 10.0.25.52411), X64 RyuJIT x86-64-v3
  DefaultJob : .NET 10.0.0 (10.0.0, 10.0.25.52411), X64 RyuJIT x86-64-v3


```
| Method                  | Mean      | Error     | Op/s          | Ratio | Allocated | Alloc Ratio |
|------------------------ |----------:|----------:|--------------:|------:|----------:|------------:|
| Add                     | 28.195 ns | 0.5836 ns |  35,466,906.4 | 14.90 |         - |          NA |
| Subtract                | 30.374 ns | 0.3741 ns |  32,922,933.9 | 16.05 |         - |          NA |
| Multiple                | 29.046 ns | 0.3349 ns |  34,427,988.1 | 15.35 |         - |          NA |
| Divide                  | 70.748 ns | 0.9017 ns |  14,134,735.2 | 37.39 |         - |          NA |
| Increment               |  9.157 ns | 0.1865 ns | 109,206,346.3 |  4.84 |         - |          NA |
| Decrement               |  9.338 ns | 0.2062 ns | 107,087,451.5 |  4.94 |         - |          NA |
| Remainder               | 20.345 ns | 0.1780 ns |  49,152,774.0 | 10.75 |         - |          NA |
| fAdd                    |  1.893 ns | 0.0566 ns | 528,159,297.6 |  1.00 |         - |          NA |
| fSubtract               |  1.737 ns | 0.0504 ns | 575,731,435.0 |  0.92 |         - |          NA |
| fMultipleDec            | 29.450 ns | 1.6689 ns |  33,955,903.5 | 15.57 |         - |          NA |
| fMultipleDecWholeNumber |  9.521 ns | 0.1498 ns | 105,031,040.1 |  5.03 |         - |          NA |
| fMultipleLong           |  2.131 ns | 0.0690 ns | 469,277,233.0 |  1.13 |         - |          NA |
| fDivideDec              | 70.444 ns | 1.1502 ns |  14,195,762.2 | 37.23 |         - |          NA |
| fDivideDecWholeNumber   | 10.886 ns | 0.2397 ns |  91,858,580.4 |  5.75 |         - |          NA |
| fDivideLong             |  2.024 ns | 0.0540 ns | 493,960,294.3 |  1.07 |         - |          NA |
| fIncrement              |  5.215 ns | 0.1268 ns | 191,744,815.9 |  2.76 |         - |          NA |
| fDecrement              |  4.627 ns | 0.1185 ns | 216,144,432.2 |  2.45 |         - |          NA |
| fRemainder              |  1.935 ns | 0.0278 ns | 516,849,107.9 |  1.02 |         - |          NA |
