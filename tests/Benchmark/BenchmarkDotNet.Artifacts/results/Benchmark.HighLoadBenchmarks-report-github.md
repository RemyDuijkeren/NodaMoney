```

BenchmarkDotNet v0.15.2, Windows 11 (10.0.26100.4946/24H2/2024Update/HudsonValley)
AMD Ryzen 7 5800H with Radeon Graphics 3.20GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 10.0.100-preview.7.25380.108
  [Host]     : .NET 9.0.8 (9.0.825.36511), X64 RyuJIT AVX2
  DefaultJob : .NET 9.0.8 (9.0.825.36511), X64 RyuJIT AVX2


```
| Method            | Mean      | Error     | StdDev    | P95       | Op/s   | Ratio | RatioSD | Gen0     | Gen1     | Gen2     | Allocated | Alloc Ratio |
|------------------ |----------:|----------:|----------:|----------:|-------:|------:|--------:|---------:|---------:|---------:|----------:|------------:|
| Create1MCurrency  | 13.953 ms | 0.2772 ms | 0.2593 ms | 14.314 ms |  71.67 |  0.38 |    0.01 | 484.3750 | 484.3750 | 484.3750 |   1.91 MB |        0.13 |
| Create1MMoney     | 36.576 ms | 0.6902 ms | 0.6456 ms | 37.541 ms |  27.34 |  1.00 |    0.02 | 571.4286 | 571.4286 | 571.4286 |  15.26 MB |        1.00 |
| Create1MFastMoney | 30.137 ms | 0.3957 ms | 0.3701 ms | 30.816 ms |  33.18 |  0.82 |    0.02 | 500.0000 | 500.0000 | 500.0000 |  11.44 MB |        0.75 |
| Create1MSqlMoney  | 20.931 ms | 0.4076 ms | 0.4185 ms | 21.787 ms |  47.78 |  0.57 |    0.01 | 593.7500 | 593.7500 | 593.7500 |  15.26 MB |        1.00 |
| Create1MDecimal   |  4.050 ms | 0.0805 ms | 0.2122 ms |  4.323 ms | 246.91 |  0.11 |    0.01 | 597.6563 | 597.6563 | 597.6563 |  15.26 MB |        1.00 |
