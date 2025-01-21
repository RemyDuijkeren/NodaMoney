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
| CurrencyFromCode     | 14.516 ns | 0.0580 ns | 0.0453 ns |         - |
| CurrencyInfoFromCode |  8.098 ns | 0.0537 ns | 0.0503 ns |         - |

## InitializingMoney
#### before (v1.x)
| Method                              |      Mean |     Error |    StdDev |    Median | Ratio | RatioSD |  Gen 0 | Allocated |
|-------------------------------------|----------:|----------:|----------:|----------:|------:|--------:|-------:|----------:|
| ExplicitCurrencyAsString            | 483.87 ns |  6.370 ns |  5.319 ns | 483.75 ns |  1.00 |    0.00 | 0.0753 |     632 B |
| ExplicitCurrencyAsStringAndRounding | 496.87 ns |  9.720 ns | 12.976 ns | 493.09 ns |  1.02 |    0.03 | 0.0753 |     632 B |
| ExplicitCurrencyFromCode            | 521.25 ns | 16.493 ns | 48.630 ns | 494.71 ns |  1.16 |    0.03 | 0.0753 |     632 B |
| ExtensionMethod                     | 490.89 ns |  7.103 ns |  5.931 ns | 492.29 ns |  1.01 |    0.02 | 0.0753 |     632 B |
| ImplicitCurrencyByConstructor       | 114.69 ns |  2.276 ns |  2.017 ns | 115.09 ns |  0.24 |    0.00 | 0.0057 |      48 B |
| ImplicitCurrencyByCasting           | 113.58 ns |  1.579 ns |  1.477 ns | 113.67 ns |  0.23 |    0.00 | 0.0057 |      48 B |
| Deconstruct                         |  34.86 ns |  0.348 ns |  0.309 ns |  34.80 ns |  0.07 |    0.00 |      - |         - |
#### after
| Method                              |       Mean |     Error |    StdDev | Ratio | RatioSD |   Gen0 | Allocated | Alloc Ratio |
|-------------------------------------|-----------:|----------:|----------:|------:|--------:|-------:|----------:|------------:|
| ExplicitCurrencyAsString            | 20.6864 ns | 0.4303 ns | 0.5122 ns |  1.00 |    0.03 |      - |         - |          NA |
| ExplicitCurrencyAsStringAndRounding | 22.6839 ns | 0.4136 ns | 0.3666 ns |  1.10 |    0.03 |      - |         - |          NA |
| ExplicitCurrencyFromCode            | 25.9768 ns | 0.2673 ns | 0.2501 ns |  1.26 |    0.03 |      - |         - |          NA |
| ExplicitCurrencyInfoFromCode        | 20.5756 ns | 0.2304 ns | 0.2155 ns |  1.00 |    0.03 |      - |         - |          NA |
| ExtensionMethod                     | 20.9704 ns | 0.4374 ns | 0.5987 ns |  1.01 |    0.04 |      - |         - |          NA |
| ImplicitCurrencyByConstructor       | 69.7052 ns | 1.2334 ns | 1.1537 ns |  3.37 |    0.10 | 0.0076 |      64 B |          NA |
| ImplicitCurrencyByCasting           | 68.7810 ns | 1.2390 ns | 1.3257 ns |  3.33 |    0.10 | 0.0076 |      64 B |          NA |
| Deconstruct                         |  0.4898 ns | 0.0126 ns | 0.0105 ns |  0.02 |    0.00 |      - |         - |          NA |

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
| Method                   |       Mean |     Error |    StdDev |   Gen0 | Allocated |
|--------------------------|-----------:|----------:|----------:|-------:|----------:|
| Addition                 |  12.311 ns | 0.2696 ns | 0.3599 ns |      - |         - |
| Subtraction              |  17.913 ns | 0.3833 ns | 0.5497 ns |      - |         - |
| CompareSameCurrency      |   3.570 ns | 0.0905 ns | 0.0846 ns |      - |         - |
| CompareDifferentCurrency |   3.833 ns | 0.1055 ns | 0.1579 ns |      - |         - |
| CompareAmount            |   4.470 ns | 0.0475 ns | 0.0421 ns |      - |         - |
| Increment                | 105.702 ns | 1.3173 ns | 1.1000 ns | 0.0038 |      32 B |
| Decrement                | 110.203 ns | 0.8956 ns | 0.7939 ns | 0.0038 |      32 B |

## MoneyFormatting
#### before (v1.x)
| Method             |     Mean |   Error |   StdDev |  Gen 0 | Allocated |
|--------------------|---------:|--------:|---------:|-------:|----------:|
| Implicit           | 194.0 ns | 3.93 ns |  9.10 ns | 0.0286 |     240 B |
| ImplicitWithFormat | 197.6 ns | 3.98 ns |  8.39 ns | 0.0286 |     240 B |
| Explicit           | 271.5 ns | 5.50 ns |  9.33 ns | 0.0525 |     440 B |
| ExplicitWithFormat | 270.3 ns | 5.42 ns | 12.56 ns | 0.0525 |     440 B |
#### after
| Method             |     Mean |   Error |   StdDev |   Median |   Gen0 | Allocated |
|--------------------|---------:|--------:|---------:|---------:|-------:|----------:|
| Implicit           | 119.2 ns | 2.38 ns |  3.09 ns | 119.9 ns | 0.0420 |     352 B |
| ImplicitWithFormat | 111.3 ns | 3.47 ns | 10.24 ns | 110.1 ns | 0.0421 |     352 B |
| Explicit           | 141.5 ns | 2.89 ns |  6.97 ns | 138.2 ns | 0.0792 |     664 B |
| ExplicitWithFormat | 159.3 ns | 3.78 ns | 11.10 ns | 157.6 ns | 0.0792 |     664 B |

## MoneyParsing
#### before (v1.x)
| Method      |        Mean |     Error |    StdDev |      Median |  Gen 0 |  Gen 1 | Allocated |
|-------------|------------:|----------:|----------:|------------:|-------:|-------:|----------:|
| Implicit    | 32,037.0 ns | 597.82 ns | 587.14 ns | 31,817.6 ns | 3.0823 | 0.2136 |     25 KB |
| ImplicitTry | 32,111.5 ns | 631.83 ns | 945.70 ns | 32,079.3 ns | 3.0823 | 0.2136 |     25 KB |
| Explicit    |    877.5 ns |  21.39 ns |  62.73 ns |    860.9 ns | 0.1469 |      - |      1 KB |
| ExplicitTry |    870.2 ns |  16.94 ns |  21.42 ns |    871.8 ns | 0.1469 |      - |      1 KB |
#### after
| Method            |     Mean |   Error |   StdDev |   Median |   Gen0 | Allocated |
|-------------------|---------:|--------:|---------:|---------:|-------:|----------:|
| Implicit          | 289.1 ns | 1.87 ns |  1.65 ns | 288.9 ns | 0.0525 |     440 B |
| ImplicitTry       | 334.3 ns | 5.41 ns |  4.80 ns | 333.1 ns | 0.0525 |     440 B |
| Explicit          | 259.2 ns | 5.52 ns | 16.02 ns | 253.3 ns | 0.0448 |     376 B |
| ExplicitAsSpan    | 272.3 ns | 5.12 ns |  4.79 ns | 271.1 ns | 0.0448 |     376 B |
| ExplicitTry       | 253.4 ns | 5.10 ns |  6.64 ns | 252.6 ns | 0.0448 |     376 B |
| ExplicitTryAsSpan | 263.9 ns | 3.65 ns |  3.41 ns | 262.5 ns | 0.0448 |     376 B |

## AddingCustomCurrency
#### before (v1.x)
| Method        |     Mean |    Error |   StdDev |  Gen 0 | Allocated |
|---------------|---------:|---------:|---------:|-------:|----------:|
| CreateBuilder | 25.65 ns | 0.546 ns | 0.560 ns | 0.0115 |      96 B |
| Build         | 32.48 ns | 0.487 ns | 0.380 ns |      - |         - |
#### after
| Method        |     Mean |    Error |   StdDev |   Gen0 | Allocated |
|---------------|---------:|---------:|---------:|-------:|----------:|
| CreateBuilder | 19.50 ns | 0.341 ns | 0.302 ns | 0.0095 |      80 B |
| Build         | 18.67 ns | 0.347 ns | 0.325 ns | 0.0095 |      80 B |

## HighLoad
#### before (v1.x)
| Method                     |     Mean |    Error |   StdDev |      Gen 0 | Allocated |
|----------------------------|---------:|---------:|---------:|-----------:|----------:|
| CreatingOneMillionCurrency | 482.6 ms |  9.57 ms | 26.68 ms | 75000.0000 |    679 MB |
| CreatingOneMillionMoney    | 516.8 ms | 10.33 ms | 28.46 ms | 75000.0000 |    694 MB |
#### after
| Method                      |     Mean |    Error |   StdDev |     Gen0 |     Gen1 |     Gen2 | Allocated |
|-----------------------------|---------:|---------:|---------:|---------:|---------:|---------:|----------:|
| CreatingOneMillionCurrency  | 15.38 ms | 0.222 ms | 0.208 ms | 468.7500 | 468.7500 | 468.7500 |   1.91 MB |
| CreatingOneMillionMoney     | 28.84 ms | 0.247 ms | 0.231 ms | 656.2500 | 656.2500 | 656.2500 |  22.89 MB |

## Create CurrencyUNit
| Method                             |      Mean |     Error |    StdDev | Ratio |   Gen0 | Allocated | Alloc Ratio |
|------------------------------------|----------:|----------:|----------:|------:|-------:|----------:|------------:|
| CreateCurrencyUnit                 | 22.006 ns | 0.2475 ns | 0.2067 ns |  1.00 | 0.0134 |     112 B |        1.00 |
| CreateCurrencyUnitNoLinq           |  8.281 ns | 0.1674 ns | 0.1719 ns |  0.38 | 0.0076 |      64 B |        0.57 |
| CreateCurrencyUnitNoLinqAndPattern |  8.558 ns | 0.1967 ns | 0.2416 ns |  0.39 | 0.0076 |      64 B |        0.57 |
