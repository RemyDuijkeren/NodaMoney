```

BenchmarkDotNet v0.15.8, Linux Omarchy
AMD Ryzen 7 8845HS w/ Radeon 780M Graphics 1.09GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 10.0.401
  [Host]     : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v4
  DefaultJob : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v4


```
| Method                         | Mean     | Error    | Op/s         | Gen0   | Allocated |
|------------------------------- |---------:|---------:|-------------:|-------:|----------:|
| DefaultFormat                  | 62.91 ns | 0.376 ns | 15,895,971.7 | 0.0086 |      72 B |
| FormatWithPrecision            | 60.98 ns | 0.340 ns | 16,398,266.6 | 0.0086 |      72 B |
| FormatProvider                 | 89.79 ns | 0.533 ns | 11,136,906.8 | 0.0459 |     384 B |
| FormatWithPrecisionAndProvider | 86.03 ns | 0.379 ns | 11,623,271.1 | 0.0459 |     384 B |
| CompactFormat                  | 77.61 ns | 0.149 ns | 12,884,186.9 | 0.0114 |      96 B |
| GeneralFormat                  | 78.06 ns | 1.575 ns | 12,810,420.0 | 0.0095 |      80 B |
| RondTripFormat                 | 59.46 ns | 0.286 ns | 16,818,076.8 | 0.0143 |     120 B |
