```

BenchmarkDotNet v0.15.8, Linux Omarchy
AMD Ryzen 7 8845HS w/ Radeon 780M Graphics 1.09GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 10.0.401
  [Host]     : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v4
  DefaultJob : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v4


```
| Method                  | Mean      | Error     | Op/s          | Ratio | Allocated | Alloc Ratio |
|------------------------ |----------:|----------:|--------------:|------:|----------:|------------:|
| Add                     | 16.404 ns | 0.0936 ns |  60,960,929.6 | 13.69 |         - |          NA |
| AddMixedScale           | 17.611 ns | 0.1031 ns |  56,783,495.9 | 14.70 |         - |          NA |
| Subtract                | 17.184 ns | 0.1270 ns |  58,192,692.8 | 14.34 |         - |          NA |
| Multiple                | 19.092 ns | 0.1085 ns |  52,377,791.4 | 15.94 |         - |          NA |
| Divide                  | 63.358 ns | 0.6121 ns |  15,783,370.4 | 52.89 |         - |          NA |
| Increment               |  7.643 ns | 0.1098 ns | 130,830,863.8 |  6.38 |         - |          NA |
| Decrement               |  9.034 ns | 0.0611 ns | 110,691,143.5 |  7.54 |         - |          NA |
| Remainder               | 17.780 ns | 0.0718 ns |  56,242,694.7 | 14.84 |         - |          NA |
| fAdd                    |  1.198 ns | 0.0119 ns | 834,658,443.4 |  1.00 |         - |          NA |
| fSubtract               |  1.192 ns | 0.0108 ns | 838,972,962.1 |  0.99 |         - |          NA |
| fMultipleDec            | 16.397 ns | 0.0625 ns |  60,985,315.4 | 13.69 |         - |          NA |
| fMultipleDecWholeNumber |  8.024 ns | 0.0831 ns | 124,625,460.8 |  6.70 |         - |          NA |
| fMultipleLong           |  2.322 ns | 0.0131 ns | 430,630,750.5 |  1.94 |         - |          NA |
| fDivideDec              | 16.216 ns | 0.0796 ns |  61,667,290.3 | 13.54 |         - |          NA |
| fDivideDecWholeNumber   | 10.012 ns | 0.0807 ns |  99,877,835.2 |  8.36 |         - |          NA |
| fDivideLong             |  2.300 ns | 0.0268 ns | 434,725,822.4 |  1.92 |         - |          NA |
| fIncrement              |  2.751 ns | 0.0272 ns | 363,456,105.1 |  2.30 |         - |          NA |
| fDecrement              |  2.779 ns | 0.0178 ns | 359,827,914.2 |  2.32 |         - |          NA |
| fRemainder              |  1.785 ns | 0.0106 ns | 560,288,361.7 |  1.49 |         - |          NA |
