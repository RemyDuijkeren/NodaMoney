```

BenchmarkDotNet v0.15.8, Linux Omarchy
AMD Ryzen 7 8845HS w/ Radeon 780M Graphics 3.19GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 10.0.401
  [Host]     : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v4
  DefaultJob : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v4


```
| Method                  | Mean     | Error     | Op/s          | Allocated |
|------------------------ |---------:|----------:|--------------:|----------:|
| CurrencyFromCode        | 9.613 ns | 0.2069 ns | 104,025,563.4 |         - |
| CurrencyInfoFromCode    | 4.317 ns | 0.0278 ns | 231,641,612.1 |         - |
| CurrencyInfoTryFromCode | 5.160 ns | 0.1116 ns | 193,811,049.0 |         - |
