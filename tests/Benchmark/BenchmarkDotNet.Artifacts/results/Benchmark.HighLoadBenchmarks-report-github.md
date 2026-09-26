```

BenchmarkDotNet v0.15.8, Linux Omarchy
AMD Ryzen 7 8845HS w/ Radeon 780M Graphics 3.19GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 10.0.401
  [Host]     : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v4
  DefaultJob : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v4


```
| Method            | Mean      | Error     | Op/s   | Ratio | Gen0     | Gen1     | Gen2     | Allocated | Alloc Ratio |
|------------------ |----------:|----------:|-------:|------:|---------:|---------:|---------:|----------:|------------:|
| Create1MCurrency  | 11.011 ms | 0.2096 ms |  90.82 |  0.39 | 484.3750 | 484.3750 | 484.3750 |   1.91 MB |        0.13 |
| Create1MMoney     | 28.304 ms | 0.2866 ms |  35.33 |  1.00 | 500.0000 | 500.0000 | 500.0000 |  15.26 MB |        1.00 |
| Create1MFastMoney | 24.538 ms | 0.3367 ms |  40.75 |  0.87 | 500.0000 | 500.0000 | 500.0000 |  11.44 MB |        0.75 |
| Create1MSqlMoney  | 13.097 ms | 0.2596 ms |  76.35 |  0.46 | 500.0000 | 500.0000 | 500.0000 |  15.26 MB |        1.00 |
| Create1MDecimal   |  2.201 ms | 0.0731 ms | 454.30 |  0.08 | 500.0000 | 500.0000 | 500.0000 |  15.26 MB |        1.00 |
