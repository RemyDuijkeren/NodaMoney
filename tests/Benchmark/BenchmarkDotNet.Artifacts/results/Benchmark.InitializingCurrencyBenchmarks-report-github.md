```

BenchmarkDotNet v0.15.6, Windows 11 (10.0.26200.7171)
AMD Ryzen 7 5800H with Radeon Graphics 3.20GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 10.0.100
  [Host]     : .NET 10.0.0 (10.0.0, 10.0.25.52411), X64 RyuJIT x86-64-v3
  DefaultJob : .NET 10.0.0 (10.0.0, 10.0.25.52411), X64 RyuJIT x86-64-v3


```
| Method                  | Mean      | Error     | Op/s          | Allocated |
|------------------------ |----------:|----------:|--------------:|----------:|
| CurrencyFromCode        | 12.229 ns | 0.1748 ns |  81,774,812.1 |         - |
| CurrencyInfoFromCode    |  6.611 ns | 0.1740 ns | 151,264,622.9 |         - |
| CurrencyInfoTryFromCode |  6.052 ns | 0.1095 ns | 165,230,528.0 |         - |
