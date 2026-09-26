```

BenchmarkDotNet v0.15.8, Linux Omarchy
AMD Ryzen 7 8845HS w/ Radeon 780M Graphics 3.19GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 10.0.401
  [Host]     : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v4
  DefaultJob : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v4


```
| Method                         | Mean      | Error    | Op/s         | Gen0   | Allocated |
|------------------------------- |----------:|---------:|-------------:|-------:|----------:|
| DefaultFormat                  |  87.87 ns | 1.146 ns | 11,380,769.4 | 0.0459 |     384 B |
| FormatWithPrecision            | 105.24 ns | 0.607 ns |  9,501,677.7 | 0.0497 |     416 B |
| FormatProvider                 |  98.27 ns | 1.214 ns | 10,176,139.4 | 0.0459 |     384 B |
| FormatWithPrecisionAndProvider | 104.95 ns | 0.527 ns |  9,528,659.7 | 0.0497 |     416 B |
| CompactFormat                  | 105.48 ns | 1.087 ns |  9,480,609.3 | 0.0488 |     408 B |
| GeneralFormat                  | 133.23 ns | 0.967 ns |  7,505,636.1 | 0.0842 |     704 B |
| RondTripFormat                 |  88.45 ns | 0.992 ns | 11,305,598.0 | 0.0516 |     432 B |
