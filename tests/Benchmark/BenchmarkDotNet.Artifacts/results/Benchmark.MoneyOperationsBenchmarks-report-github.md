```

BenchmarkDotNet v0.15.2, Windows 11 (10.0.26100.4946/24H2/2024Update/HudsonValley)
AMD Ryzen 7 5800H with Radeon Graphics 3.20GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 10.0.100-preview.7.25380.108
  [Host]     : .NET 9.0.8 (9.0.825.36511), X64 RyuJIT AVX2
  DefaultJob : .NET 9.0.8 (9.0.825.36511), X64 RyuJIT AVX2


```
| Method                      | Mean      | Error     | StdDev    | P95       | Op/s          | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
|---------------------------- |----------:|----------:|----------:|----------:|--------------:|------:|--------:|-------:|----------:|------------:|
| Add                         | 26.730 ns | 0.4042 ns | 0.3583 ns | 27.337 ns |  37,410,832.2 |  1.00 |    0.02 |      - |         - |          NA |
| AddFastMoney                | 11.440 ns | 0.2238 ns | 0.1869 ns | 11.690 ns |  87,414,421.9 |  0.43 |    0.01 |      - |         - |          NA |
| Subtract                    | 26.428 ns | 0.1206 ns | 0.1007 ns | 26.570 ns |  37,839,081.6 |  0.99 |    0.01 |      - |         - |          NA |
| SubtractFastMoney           |  5.868 ns | 0.0755 ns | 0.0669 ns |  5.954 ns | 170,430,275.8 |  0.22 |    0.00 |      - |         - |          NA |
| Multiple                    | 32.960 ns | 0.2225 ns | 0.1972 ns | 33.303 ns |  30,340,060.4 |  1.23 |    0.02 |      - |         - |          NA |
| MultipleFastMoneyDecimal    | 28.330 ns | 0.2286 ns | 0.2026 ns | 28.710 ns |  35,297,951.6 |  1.06 |    0.02 |      - |         - |          NA |
| MultipleFastWholeDecimal    | 14.785 ns | 0.1376 ns | 0.1287 ns | 15.015 ns |  67,637,647.0 |  0.55 |    0.01 |      - |         - |          NA |
| MultipleFastMoneyLong       |  2.083 ns | 0.0350 ns | 0.0310 ns |  2.131 ns | 480,070,346.5 |  0.08 |    0.00 |      - |         - |          NA |
| Divide                      | 83.825 ns | 0.3771 ns | 0.3149 ns | 84.265 ns |  11,929,609.5 |  3.14 |    0.04 |      - |         - |          NA |
| DivideFastMoneyDecimal      | 78.831 ns | 0.4703 ns | 0.4169 ns | 79.427 ns |  12,685,296.2 |  2.95 |    0.04 |      - |         - |          NA |
| DivideFastMoneyWholeDecimal | 13.265 ns | 0.2866 ns | 0.3727 ns | 13.960 ns |  75,385,506.6 |  0.50 |    0.02 |      - |         - |          NA |
| DivideFastMoneyLong         |  1.984 ns | 0.0629 ns | 0.1432 ns |  2.299 ns | 504,005,460.5 |  0.07 |    0.01 |      - |         - |          NA |
| Equal                       |  3.619 ns | 0.0531 ns | 0.0444 ns |  3.682 ns | 276,314,723.4 |  0.14 |    0.00 |      - |         - |          NA |
| NotEqualCurrency            |  1.657 ns | 0.0266 ns | 0.0249 ns |  1.706 ns | 603,660,650.8 |  0.06 |    0.00 |      - |         - |          NA |
| EqualOrBigger               |  5.650 ns | 0.1022 ns | 0.0956 ns |  5.789 ns | 176,989,154.9 |  0.21 |    0.00 |      - |         - |          NA |
| Bigger                      |  5.582 ns | 0.0589 ns | 0.0522 ns |  5.676 ns | 179,156,947.4 |  0.21 |    0.00 |      - |         - |          NA |
| Increment                   | 87.358 ns | 1.0496 ns | 0.9304 ns | 89.013 ns |  11,447,121.2 |  3.27 |    0.05 | 0.0038 |      32 B |          NA |
| Decrement                   | 86.520 ns | 0.8442 ns | 0.7049 ns | 87.642 ns |  11,557,991.8 |  3.24 |    0.05 | 0.0038 |      32 B |          NA |
