```

BenchmarkDotNet v0.15.2, Windows 11 (10.0.26100.4946/24H2/2024Update/HudsonValley)
AMD Ryzen 7 5800H with Radeon Graphics 3.20GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 10.0.100-preview.7.25380.108
  [Host]     : .NET 9.0.8 (9.0.825.36511), X64 RyuJIT AVX2
  DefaultJob : .NET 9.0.8 (9.0.825.36511), X64 RyuJIT AVX2


```
| Method                  | Mean      | Error     | StdDev    | P95       | Op/s          | Allocated |
|------------------------ |----------:|----------:|----------:|----------:|--------------:|----------:|
| CurrencyFromCode        | 16.178 ns | 0.2721 ns | 0.2412 ns | 16.525 ns |  61,810,657.5 |         - |
| CurrencyInfoFromCode    |  9.293 ns | 0.0985 ns | 0.0873 ns |  9.427 ns | 107,608,674.6 |         - |
| CurrencyInfoTryFromCode |  6.471 ns | 0.0487 ns | 0.0456 ns |  6.535 ns | 154,534,339.0 |         - |
