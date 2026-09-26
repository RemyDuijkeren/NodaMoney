```

BenchmarkDotNet v0.15.8, Linux Omarchy
AMD Ryzen 7 8845HS w/ Radeon 780M Graphics 3.19GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 10.0.401
  [Host]     : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v4
  DefaultJob : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v4


```
| Method                  | Mean      | Error     | Op/s          | Ratio | Allocated | Alloc Ratio |
|------------------------ |----------:|----------:|--------------:|------:|----------:|------------:|
| Add                     | 20.689 ns | 0.3419 ns |  48,333,857.1 | 15.81 |         - |          NA |
| Subtract                | 20.123 ns | 0.0814 ns |  49,693,933.0 | 15.38 |         - |          NA |
| Multiple                | 18.819 ns | 0.3971 ns |  53,138,659.5 | 14.38 |         - |          NA |
| Divide                  | 61.429 ns | 0.8173 ns |  16,278,848.1 | 46.94 |         - |          NA |
| Increment               |  9.589 ns | 0.1794 ns | 104,283,243.7 |  7.33 |         - |          NA |
| Decrement               |  9.581 ns | 0.0612 ns | 104,378,673.5 |  7.32 |         - |          NA |
| Remainder               | 18.224 ns | 0.0914 ns |  54,871,257.8 | 13.92 |         - |          NA |
| fAdd                    |  1.309 ns | 0.0130 ns | 763,999,716.2 |  1.00 |         - |          NA |
| fSubtract               |  1.198 ns | 0.0079 ns | 834,544,475.5 |  0.92 |         - |          NA |
| fMultipleDec            | 15.800 ns | 0.1004 ns |  63,289,385.3 | 12.07 |         - |          NA |
| fMultipleDecWholeNumber |  7.692 ns | 0.0166 ns | 130,010,288.7 |  5.88 |         - |          NA |
| fMultipleLong           |  2.254 ns | 0.0124 ns | 443,601,922.4 |  1.72 |         - |          NA |
| fDivideDec              | 50.625 ns | 0.3219 ns |  19,752,911.9 | 38.68 |         - |          NA |
| fDivideDecWholeNumber   |  8.713 ns | 0.1105 ns | 114,777,109.5 |  6.66 |         - |          NA |
| fDivideLong             |  2.294 ns | 0.0387 ns | 435,944,300.4 |  1.75 |         - |          NA |
| fIncrement              |  5.160 ns | 0.0120 ns | 193,802,443.1 |  3.94 |         - |          NA |
| fDecrement              |  5.196 ns | 0.0139 ns | 192,461,021.5 |  3.97 |         - |          NA |
| fRemainder              |  1.693 ns | 0.0101 ns | 590,631,414.4 |  1.29 |         - |          NA |
