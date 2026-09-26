```

BenchmarkDotNet v0.15.8, Linux Omarchy
AMD Ryzen 7 8845HS w/ Radeon 780M Graphics 1.09GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 10.0.401
  [Host]     : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v4
  DefaultJob : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v4


```
| Method                  | Mean     | Error     | Op/s          | Allocated |
|------------------------ |---------:|----------:|--------------:|----------:|
| CurrencyFromCode        | 4.792 ns | 0.0113 ns | 208,682,339.3 |         - |
| CurrencyInfoFromCode    | 4.440 ns | 0.0252 ns | 225,209,268.8 |         - |
| CurrencyInfoTryFromCode | 4.155 ns | 0.0221 ns | 240,646,461.3 |         - |
