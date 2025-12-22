```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.7462/25H2/2025Update/HudsonValley2)
AMD Ryzen 7 5800H with Radeon Graphics 3.20GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 10.0.101
  [Host]     : .NET 10.0.1 (10.0.1, 10.0.125.57005), X64 RyuJIT x86-64-v3
  DefaultJob : .NET 10.0.1 (10.0.1, 10.0.125.57005), X64 RyuJIT x86-64-v3


```
| Method                  |      Mean |     Error |          Op/s | Allocated |
|-------------------------|----------:|----------:|--------------:|----------:|
| CurrencyFromCode        | 11.641 ns | 0.2240 ns |  85,904,593.4 |         - |
| CurrencyInfoFromCode    |  6.260 ns | 0.0540 ns | 159,742,874.9 |         - |
| CurrencyInfoTryFromCode |  5.785 ns | 0.0682 ns | 172,864,355.0 |         - |
