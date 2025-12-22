```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.7462/25H2/2025Update/HudsonValley2)
AMD Ryzen 7 5800H with Radeon Graphics 3.20GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 10.0.101
  [Host]     : .NET 10.0.1 (10.0.1, 10.0.125.57005), X64 RyuJIT x86-64-v3
  DefaultJob : .NET 10.0.1 (10.0.1, 10.0.125.57005), X64 RyuJIT x86-64-v3


```
| Method                         |     Mean |   Error |        Op/s |   Gen0 | Allocated |
|--------------------------------|---------:|--------:|------------:|-------:|----------:|
| DefaultFormat                  | 104.2 ns | 1.88 ns | 9,595,323.6 | 0.0458 |     384 B |
| FormatWithPrecision            | 138.8 ns | 2.77 ns | 7,203,541.8 | 0.0496 |     416 B |
| FormatProvider                 | 109.1 ns | 2.38 ns | 9,163,086.7 | 0.0459 |     384 B |
| FormatWithPrecisionAndProvider | 140.6 ns | 2.86 ns | 7,112,809.4 | 0.0496 |     416 B |
| CompactFormat                  | 226.5 ns | 2.41 ns | 4,415,271.6 | 0.0572 |     480 B |
| GeneralFormat                  | 166.3 ns | 3.38 ns | 6,011,911.3 | 0.0842 |     704 B |
| RondTripFormat                 | 120.5 ns | 2.32 ns | 8,301,989.0 | 0.0515 |     432 B |
