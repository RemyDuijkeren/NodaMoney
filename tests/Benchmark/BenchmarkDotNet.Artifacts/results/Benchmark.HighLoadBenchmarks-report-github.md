```

BenchmarkDotNet v0.15.6, Windows 11 (10.0.26200.7171)
AMD Ryzen 7 5800H with Radeon Graphics 3.20GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 10.0.100
  [Host]     : .NET 10.0.0 (10.0.0, 10.0.25.52411), X64 RyuJIT x86-64-v3
  DefaultJob : .NET 10.0.0 (10.0.0, 10.0.25.52411), X64 RyuJIT x86-64-v3


```
| Method            | Mean      | Error     | Op/s   | Ratio | Gen0     | Gen1     | Gen2     | Allocated | Alloc Ratio |
|------------------ |----------:|----------:|-------:|------:|---------:|---------:|---------:|----------:|------------:|
| Create1MCurrency  | 12.686 ms | 0.2494 ms |  78.83 |  0.43 | 484.3750 | 484.3750 | 484.3750 |   1.91 MB |        0.13 |
| Create1MMoney     | 29.750 ms | 0.2796 ms |  33.61 |  1.00 | 500.0000 | 500.0000 | 500.0000 |  15.26 MB |        1.00 |
| Create1MFastMoney | 21.997 ms | 0.4164 ms |  45.46 |  0.74 | 500.0000 | 500.0000 | 500.0000 |  11.44 MB |        0.75 |
| Create1MSqlMoney  | 17.601 ms | 0.3084 ms |  56.81 |  0.59 | 500.0000 | 500.0000 | 500.0000 |  15.26 MB |        1.00 |
| Create1MDecimal   |  3.438 ms | 0.1078 ms | 290.87 |  0.12 | 500.0000 | 500.0000 | 500.0000 |  15.26 MB |        1.00 |
