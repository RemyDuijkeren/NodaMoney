```

BenchmarkDotNet v0.15.8, Linux Omarchy
AMD Ryzen 7 8845HS w/ Radeon 780M Graphics 1.09GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 10.0.401
  [Host]     : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v4
  DefaultJob : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v4


```
| Method            | Mean      | Error     | Op/s   | Ratio | Gen0     | Gen1     | Gen2     | Allocated | Alloc Ratio |
|------------------ |----------:|----------:|-------:|------:|---------:|---------:|---------:|----------:|------------:|
| Create1MCurrency  |  6.545 ms | 0.0355 ms | 152.79 |  0.24 | 492.1875 | 492.1875 | 492.1875 |   1.91 MB |        0.13 |
| Create1MMoney     | 26.783 ms | 0.0379 ms |  37.34 |  1.00 | 500.0000 | 500.0000 | 500.0000 |  15.26 MB |        1.00 |
| Create1MFastMoney | 16.226 ms | 0.1022 ms |  61.63 |  0.61 | 500.0000 | 500.0000 | 500.0000 |  11.44 MB |        0.75 |
| Create1MSqlMoney  | 12.640 ms | 0.0362 ms |  79.11 |  0.47 | 500.0000 | 500.0000 | 500.0000 |  15.26 MB |        1.00 |
| Create1MDecimal   |  1.926 ms | 0.0182 ms | 519.26 |  0.07 | 500.0000 | 500.0000 | 500.0000 |  15.26 MB |        1.00 |
