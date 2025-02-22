``` ini
BenchmarkDotNet v0.14.0, Windows 11 (10.0.22631.4602/23H2/2023Update/SunValley3)
AMD Ryzen 7 5800H with Radeon Graphics, 1 CPU, 16 logical and 8 physical cores
.NET SDK 9.0.101
  [Host]     : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2
  DefaultJob : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2
```
## InitializingCurrency
#### before (v1.x)
| Method               |     Mean |   Error |   StdDev |  Gen 0 | Allocated |
|----------------------|---------:|--------:|---------:|-------:|----------:|
| FromCode             | 443.7 ns | 8.90 ns | 14.87 ns | 0.0753 |     632 B |
#### after
| Method               |      Mean |     Error |    StdDev | Allocated |
|----------------------|----------:|----------:|----------:|----------:|
| CurrencyFromCode     | 14.147 ns | 0.0707 ns | 0.0661 ns |         - |
| CurrencyInfoFromCode |  7.181 ns | 0.0960 ns | 0.0851 ns |         - |

## InitializingMoney
#### before (v1.x)
| Method                          |      Mean |     Error |    StdDev |    Median | Ratio | RatioSD |  Gen 0 | Allocated |
|---------------------------------|----------:|----------:|----------:|----------:|------:|--------:|-------:|----------:|
| ExplicitCurrencyCode            | 483.87 ns |  6.370 ns |  5.319 ns | 483.75 ns |  1.00 |    0.00 | 0.0753 |     632 B |
| ExplicitCurrencyCodeAndRounding | 496.87 ns |  9.720 ns | 12.976 ns | 493.09 ns |  1.02 |    0.03 | 0.0753 |     632 B |
| ExplicitCurrencyFromCode        | 521.25 ns | 16.493 ns | 48.630 ns | 494.71 ns |  1.16 |    0.03 | 0.0753 |     632 B |
| ExtensionMethod                 | 490.89 ns |  7.103 ns |  5.931 ns | 492.29 ns |  1.01 |    0.02 | 0.0753 |     632 B |
| ImplicitCurrencyByConstructor   | 114.69 ns |  2.276 ns |  2.017 ns | 115.09 ns |  0.24 |    0.00 | 0.0057 |      48 B |
| ImplicitCurrencyByCasting       | 113.58 ns |  1.579 ns |  1.477 ns | 113.67 ns |  0.23 |    0.00 | 0.0057 |      48 B |
| Deconstruct                     |  34.86 ns |  0.348 ns |  0.309 ns |  34.80 ns |  0.07 |    0.00 |      - |         - |
#### after
| Method                          |       Mean |     Error |    StdDev | Ratio | RatioSD |   Gen0 | Allocated | Alloc Ratio |
|---------------------------------|-----------:|----------:|----------:|------:|--------:|-------:|----------:|------------:|
| ExplicitCurrencyCode            | 24.2961 ns | 0.1421 ns | 0.1259 ns | 1.000 |    0.01 |      - |         - |          NA |
| ExplicitCurrencyCodeAndRounding | 26.3506 ns | 0.1195 ns | 0.1060 ns | 1.085 |    0.01 |      - |         - |          NA |
| ExplicitCurrencyFromCode        | 30.0748 ns | 0.2464 ns | 0.2305 ns | 1.238 |    0.01 |      - |         - |          NA |
| ExplicitCurrencyInfoFromCode    | 25.5642 ns | 0.2474 ns | 0.2315 ns | 1.052 |    0.01 |      - |         - |          NA |
| ExtensionMethod                 | 25.6926 ns | 0.3348 ns | 0.3132 ns | 1.058 |    0.01 |      - |         - |          NA |
| ImplicitCurrencyByConstructor   | 70.0186 ns | 0.2386 ns | 0.2115 ns | 2.882 |    0.02 | 0.0038 |      32 B |          NA |
| ImplicitCurrencyByCasting       | 71.3199 ns | 0.3612 ns | 0.3202 ns | 2.936 |    0.02 | 0.0038 |      32 B |          NA |
| Deconstruct                     |  0.0913 ns | 0.0073 ns | 0.0068 ns | 0.004 |    0.00 |      - |         - |          NA |

## MoneyOperations
#### before (v1.x)
| Method                   |      Mean |    Error |    StdDev | Allocated |
|--------------------------|----------:|---------:|----------:|----------:|
| Addition                 | 231.00 ns | 4.470 ns |  4.181 ns |         - |
| Subtraction              | 233.42 ns | 4.665 ns |  4.581 ns |         - |
| CompareSameCurrency      |  30.09 ns | 0.518 ns |  0.485 ns |         - |
| CompareDifferentCurrency |  68.83 ns | 0.679 ns |  0.530 ns |         - |
| CompareAmount            | 212.60 ns | 4.271 ns | 10.558 ns |         - |
| Increment                | 374.80 ns | 7.507 ns | 11.907 ns |         - |
| Decrement                | 369.70 ns | 7.295 ns | 14.229 ns |         - |
#### after
| Method                   |      Mean |     Error |    StdDev |   Gen0 | Allocated |
|--------------------------|----------:|----------:|----------:|-------:|----------:|
| Addition                 | 16.135 ns | 0.1946 ns | 0.1820 ns |      - |         - |
| Subtraction              | 15.207 ns | 0.0465 ns | 0.0413 ns |      - |         - |
| CompareSameCurrency      |  3.629 ns | 0.0297 ns | 0.0278 ns |      - |         - |
| CompareDifferentCurrency |  3.553 ns | 0.0247 ns | 0.0219 ns |      - |         - |
| CompareAmount            |  3.888 ns | 0.0282 ns | 0.0264 ns |      - |         - |
| Increment                | 86.121 ns | 0.2972 ns | 0.2635 ns | 0.0038 |      32 B |
| Decrement                | 86.974 ns | 0.3903 ns | 0.3650 ns | 0.0038 |      32 B |

## MoneyFormatting
#### before (v1.x)
| Method             |     Mean |   Error |   StdDev |  Gen 0 | Allocated |
|--------------------|---------:|--------:|---------:|-------:|----------:|
| Implicit           | 194.0 ns | 3.93 ns |  9.10 ns | 0.0286 |     240 B |
| ImplicitWithFormat | 197.6 ns | 3.98 ns |  8.39 ns | 0.0286 |     240 B |
| Explicit           | 271.5 ns | 5.50 ns |  9.33 ns | 0.0525 |     440 B |
| ExplicitWithFormat | 270.3 ns | 5.42 ns | 12.56 ns | 0.0525 |     440 B |
#### after
| Method             |     Mean |   Error |  StdDev |   Gen0 | Allocated |
|--------------------|---------:|--------:|--------:|-------:|----------:|
| Implicit           | 102.5 ns | 2.06 ns | 2.83 ns | 0.0468 |     392 B |
| ImplicitWithFormat | 132.1 ns | 2.64 ns | 3.87 ns | 0.0505 |     424 B |
| Explicit           | 102.1 ns | 2.08 ns | 3.42 ns | 0.0468 |     392 B |
| ExplicitWithFormat | 128.4 ns | 2.60 ns | 4.62 ns | 0.0505 |     424 B |

## MoneyParsing
#### before (v1.x)
| Method      |        Mean |     Error |    StdDev |      Median |  Gen 0 |  Gen 1 | Allocated |
|-------------|------------:|----------:|----------:|------------:|-------:|-------:|----------:|
| Implicit    | 32,037.0 ns | 597.82 ns | 587.14 ns | 31,817.6 ns | 3.0823 | 0.2136 |     25 KB |
| ImplicitTry | 32,111.5 ns | 631.83 ns | 945.70 ns | 32,079.3 ns | 3.0823 | 0.2136 |     25 KB |
| Explicit    |    877.5 ns |  21.39 ns |  62.73 ns |    860.9 ns | 0.1469 |      - |      1 KB |
| ExplicitTry |    870.2 ns |  16.94 ns |  21.42 ns |    871.8 ns | 0.1469 |      - |      1 KB |
#### after
| Method            |     Mean |   Error |  StdDev |   Gen0 | Allocated |
|-------------------|---------:|--------:|--------:|-------:|----------:|
| Implicit          | 411.7 ns | 4.31 ns | 3.60 ns | 0.1173 |     984 B |
| ImplicitTry       | 416.4 ns | 6.19 ns | 5.79 ns | 0.1173 |     984 B |
| Explicit          | 450.9 ns | 6.67 ns | 6.24 ns | 0.1173 |     984 B |
| ExplicitAsSpan    | 450.3 ns | 7.09 ns | 6.63 ns | 0.1173 |     984 B |
| ExplicitTry       | 415.3 ns | 8.25 ns | 8.11 ns | 0.1173 |     984 B |
| ExplicitTryAsSpan | 429.9 ns | 8.52 ns | 7.97 ns | 0.1173 |     984 B |

## AddingCustomCurrency
#### before (v1.x)
| Method        |     Mean |    Error |   StdDev |  Gen 0 | Allocated |
|---------------|---------:|---------:|---------:|-------:|----------:|
| CreateBuilder | 25.65 ns | 0.546 ns | 0.560 ns | 0.0115 |      96 B |
| Build         | 32.48 ns | 0.487 ns | 0.380 ns |      - |         - |
#### after
| Method        |     Mean |    Error |   StdDev |   Gen0 | Allocated |
|---------------|---------:|---------:|---------:|-------:|----------:|
| CreateBuilder | 20.33 ns | 0.460 ns | 1.019 ns | 0.0095 |      80 B |
| Build         | 22.05 ns | 0.465 ns | 0.621 ns | 0.0124 |     104 B |

| Method     |          Mean |        Error |       StdDev |    Gen0 |    Gen1 | Allocated |
|------------|--------------:|-------------:|-------------:|--------:|--------:|----------:|
| Build      |      21.16 ns |     0.467 ns |     1.045 ns |  0.0229 |       - |     192 B |
| Replace    | 372,900.68 ns | 4,157.027 ns | 3,685.096 ns | 85.9375 | 40.0391 |  721946 B |

## HighLoad
#### before (v1.x)
| Method                     |     Mean |    Error |   StdDev |      Gen 0 | Allocated |
|----------------------------|---------:|---------:|---------:|-----------:|----------:|
| CreatingOneMillionCurrency | 482.6 ms |  9.57 ms | 26.68 ms | 75000.0000 |    679 MB |
| CreatingOneMillionMoney    | 516.8 ms | 10.33 ms | 28.46 ms | 75000.0000 |    694 MB |
#### after
| Method                     |     Mean |    Error |   StdDev |     Gen0 |     Gen1 |     Gen2 | Allocated |
|----------------------------|---------:|---------:|---------:|---------:|---------:|---------:|----------:|
| CreatingOneMillionCurrency | 13.07 ms | 0.088 ms | 0.082 ms | 484.3750 | 484.3750 | 484.3750 |   1.91 MB |
| CreatingOneMillionMoney    | 17.42 ms | 0.082 ms | 0.077 ms | 656.2500 | 656.2500 | 656.2500 |  22.89 MB |
