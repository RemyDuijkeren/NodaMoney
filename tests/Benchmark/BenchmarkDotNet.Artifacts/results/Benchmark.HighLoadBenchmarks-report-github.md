```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.7462/25H2/2025Update/HudsonValley2)
AMD Ryzen 7 5800H with Radeon Graphics 3.20GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 10.0.101
  [Host]     : .NET 10.0.1 (10.0.1, 10.0.125.57005), X64 RyuJIT x86-64-v3
  DefaultJob : .NET 10.0.1 (10.0.1, 10.0.125.57005), X64 RyuJIT x86-64-v3


```
| Method            |      Mean |     Error |   Op/s | Ratio |     Gen0 |     Gen1 |     Gen2 | Allocated | Alloc Ratio |
|-------------------|----------:|----------:|-------:|------:|---------:|---------:|---------:|----------:|------------:|
| Create1MCurrency  | 13.433 ms | 0.1456 ms |  74.44 |  0.39 | 484.3750 | 484.3750 | 484.3750 |   1.91 MB |        0.13 |
| Create1MMoney     | 34.384 ms | 0.2132 ms |  29.08 |  1.00 | 466.6667 | 466.6667 | 466.6667 |  15.26 MB |        1.00 |
| Create1MFastMoney | 22.517 ms | 0.3951 ms |  44.41 |  0.65 | 500.0000 | 500.0000 | 500.0000 |  11.44 MB |        0.75 |
| Create1MSqlMoney  | 17.019 ms | 0.1556 ms |  58.76 |  0.49 | 500.0000 | 500.0000 | 500.0000 |  15.26 MB |        1.00 |
| Create1MDecimal   |  3.677 ms | 0.0964 ms | 271.96 |  0.11 | 500.0000 | 500.0000 | 500.0000 |  15.26 MB |        1.00 |
