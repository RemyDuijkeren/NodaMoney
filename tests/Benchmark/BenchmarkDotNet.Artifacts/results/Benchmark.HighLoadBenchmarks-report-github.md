```

BenchmarkDotNet v0.15.8, Linux Omarchy
AMD Ryzen 7 8845HS w/ Radeon 780M Graphics 2.38GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 10.0.401
  [Host]     : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v4
  DefaultJob : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v4


```
| Method            | Mean      | Error     | Op/s   | Ratio | Gen0     | Gen1     | Gen2     | Allocated | Alloc Ratio |
|------------------ |----------:|----------:|-------:|------:|---------:|---------:|---------:|----------:|------------:|
| Create1MCurrency  |  6.672 ms | 0.1021 ms | 149.88 |  3.27 | 492.1875 | 492.1875 | 492.1875 |   1.91 MB |        0.13 |
| Create1MMoney     | 27.572 ms | 0.2489 ms |  36.27 | 13.50 | 500.0000 | 500.0000 | 500.0000 |  15.26 MB |        1.00 |
| Create1MFastMoney | 13.928 ms | 0.1339 ms |  71.80 |  6.82 | 500.0000 | 500.0000 | 500.0000 |  11.44 MB |        0.75 |
| Create1MSqlMoney  | 13.176 ms | 0.2008 ms |  75.90 |  6.45 | 500.0000 | 500.0000 | 500.0000 |  15.26 MB |        1.00 |
| Create1MDecimal   |  2.044 ms | 0.0398 ms | 489.26 |  1.00 | 500.0000 | 500.0000 | 500.0000 |  15.26 MB |        1.00 |
