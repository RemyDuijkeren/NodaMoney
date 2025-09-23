```

BenchmarkDotNet v0.15.2, Windows 11 (10.0.26100.4946/24H2/2024Update/HudsonValley)
AMD Ryzen 7 5800H with Radeon Graphics 3.20GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 10.0.100-preview.7.25380.108
  [Host]     : .NET 9.0.8 (9.0.825.36511), X64 RyuJIT AVX2
  DefaultJob : .NET 9.0.8 (9.0.825.36511), X64 RyuJIT AVX2


```
| Method            | Mean      | Error     | StdDev    | Op/s   | Ratio | Gen0     | Gen1     | Gen2     | Allocated | Alloc Ratio |
|------------------ |----------:|----------:|----------:|-------:|------:|---------:|---------:|---------:|----------:|------------:|
| Create1MCurrency  | 13.756 ms | 0.2202 ms | 0.2059 ms |  72.70 |  0.38 | 484.3750 | 484.3750 | 484.3750 |   1.91 MB |        0.13 |
| Create1MMoney     | 35.960 ms | 0.1869 ms | 0.1748 ms |  27.81 |  1.00 | 571.4286 | 571.4286 | 571.4286 |  15.26 MB |        1.00 |
| Create1MFastMoney | 31.320 ms | 0.1647 ms | 0.1460 ms |  31.93 |  0.87 | 500.0000 | 500.0000 | 500.0000 |  11.44 MB |        0.75 |
| Create1MSqlMoney  | 20.676 ms | 0.1968 ms | 0.1744 ms |  48.37 |  0.57 | 593.7500 | 593.7500 | 593.7500 |  15.26 MB |        1.00 |
| Create1MDecimal   |  2.999 ms | 0.0583 ms | 0.0624 ms | 333.42 |  0.08 | 597.6563 | 597.6563 | 597.6563 |  15.26 MB |        1.00 |
