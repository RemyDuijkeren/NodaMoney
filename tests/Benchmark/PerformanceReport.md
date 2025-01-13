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
| HelperMethod                        | 490.89 ns |  7.103 ns |  5.931 ns | 492.29 ns |  1.01 |    0.02 | 0.0753 |     632 B |
| ImplicitCurrencyByConstructor       | 114.69 ns |  2.276 ns |  2.017 ns | 115.09 ns |  0.24 |    0.00 | 0.0057 |      48 B |
| ImplicitCurrencyByCasting           | 113.58 ns |  1.579 ns |  1.477 ns | 113.67 ns |  0.23 |    0.00 | 0.0057 |      48 B |
| Deconstruct                         |  34.86 ns |  0.348 ns |  0.309 ns |  34.80 ns |  0.07 |    0.00 |      - |         - |
#### after
| Method                              |        Mean |     Error |    StdDev | Ratio |   Gen0 | Allocated | Alloc Ratio |
|-------------------------------------|------------:|----------:|----------:|------:|-------:|----------:|------------:|
| ExplicitCurrencyAsString            | 145.3739 ns | 0.7158 ns | 0.5977 ns | 1.000 |      - |         - |          NA |
| ExplicitCurrencyAsStringAndRounding | 146.6304 ns | 1.1384 ns | 1.0649 ns | 1.009 |      - |         - |          NA |
| ExplicitCurrencyFromCode            | 146.4261 ns | 0.8110 ns | 0.6772 ns | 1.007 |      - |         - |          NA |
| HelperMethod                        | 144.3526 ns | 0.4919 ns | 0.4361 ns | 0.993 |      - |         - |          NA |
| ImplicitCurrencyByConstructor       | 191.6725 ns | 0.4605 ns | 0.3846 ns | 1.319 | 0.0076 |      64 B |          NA |
| ExplicitCurrencyByCasting           | 188.4323 ns | 0.5943 ns | 0.5559 ns | 1.296 | 0.0076 |      64 B |          NA |
| Deconstruct                         |   0.2899 ns | 0.0097 ns | 0.0091 ns | 0.002 |      - |         - |          NA |


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
| Addition                 | 135.813 ns | 1.0134 ns | 0.8984 ns |      - |         - |
| Subtraction              | 133.975 ns | 0.6278 ns | 0.5873 ns |      - |         - |
| CompareSameCurrency      |   3.631 ns | 0.0475 ns | 0.0444 ns |      - |         - |
| CompareDifferentCurrency |   3.928 ns | 0.0377 ns | 0.0353 ns |      - |         - |
| CompareAmount            |   4.227 ns | 0.0194 ns | 0.0172 ns |      - |         - |
| Increment                | 332.818 ns | 0.8453 ns | 0.7059 ns | 0.0038 |      32 B |
| Decrement                | 334.270 ns | 1.5156 ns | 1.3436 ns | 0.0038 |      32 B |

## MoneyFormatting
#### before (v1.x)
| Method             |     Mean |   Error |   StdDev |  Gen 0 | Allocated |
|--------------------|---------:|--------:|---------:|-------:|----------:|
| Implicit           | 194.0 ns | 3.93 ns |  9.10 ns | 0.0286 |     240 B |
| ImplicitWithFormat | 197.6 ns | 3.98 ns |  8.39 ns | 0.0286 |     240 B |
| Explicit           | 271.5 ns | 5.50 ns |  9.33 ns | 0.0525 |     440 B |
| ExplicitWithFormat | 270.3 ns | 5.42 ns | 12.56 ns | 0.0525 |     440 B |
#### after
| Method             |      Mean |    Error |   StdDev |   Gen0 | Allocated |
|--------------------|----------:|---------:|---------:|-------:|----------:|
| Implicit           | 105.91 ns | 0.679 ns | 0.567 ns | 0.0421 |     352 B |
| ImplicitWithFormat |  99.52 ns | 1.288 ns | 1.205 ns | 0.0421 |     352 B |
| Explicit           | 141.43 ns | 2.862 ns | 2.811 ns | 0.0792 |     664 B |
| ExplicitWithFormat | 148.48 ns | 2.805 ns | 2.624 ns | 0.0792 |     664 B |

## MoneyParsing
#### before (v1.x)
| Method      |        Mean |     Error |    StdDev |      Median |  Gen 0 |  Gen 1 | Allocated |
|-------------|------------:|----------:|----------:|------------:|-------:|-------:|----------:|
| Implicit    | 32,037.0 ns | 597.82 ns | 587.14 ns | 31,817.6 ns | 3.0823 | 0.2136 |     25 KB |
| ImplicitTry | 32,111.5 ns | 631.83 ns | 945.70 ns | 32,079.3 ns | 3.0823 | 0.2136 |     25 KB |
| Explicit    |    877.5 ns |  21.39 ns |  62.73 ns |    860.9 ns | 0.1469 |      - |      1 KB |
| ExplicitTry |    870.2 ns |  16.94 ns |  21.42 ns |    871.8 ns | 0.1469 |      - |      1 KB |
#### after
| Method      |     Mean |    Error |  StdDev |   Gen0 | Allocated |
|-------------|---------:|---------:|--------:|-------:|----------:|
| Implicit    | 552.5 ns | 10.13 ns | 9.95 ns | 0.1469 |    1232 B |
| ImplicitTry | 532.6 ns |  5.16 ns | 4.57 ns | 0.1469 |    1232 B |
| Explicit    | 355.8 ns |  4.89 ns | 4.57 ns | 0.1116 |     936 B |
| ExplicitTry | 351.0 ns |  3.38 ns | 2.82 ns | 0.1116 |     936 B |

## AddingCustomCurrency
#### before (v1.x)
| Method        |     Mean |    Error |   StdDev |  Gen 0 | Allocated |
|---------------|---------:|---------:|---------:|-------:|----------:|
| CreateBuilder | 25.65 ns | 0.546 ns | 0.560 ns | 0.0115 |      96 B |
| Build         | 32.48 ns | 0.487 ns | 0.380 ns |      - |         - |
#### after
| Method        |     Mean |    Error |   StdDev |   Gen0 | Allocated |
|---------------|---------:|---------:|---------:|-------:|----------:|
| CreateBuilder | 20.68 ns | 0.452 ns | 0.538 ns | 0.0095 |      80 B |
| Build         | 18.29 ns | 0.226 ns | 0.176 ns | 0.0095 |      80 B |

## HighLoad
#### before (v1.x)
| Method                     |     Mean |    Error |   StdDev |      Gen 0 | Allocated |
|----------------------------|---------:|---------:|---------:|-----------:|----------:|
| CreatingOneMillionCurrency | 482.6 ms |  9.57 ms | 26.68 ms | 75000.0000 |    679 MB |
| CreatingOneMillionMoney    | 516.8 ms | 10.33 ms | 28.46 ms | 75000.0000 |    694 MB |
#### after
| Method                      |      Mean |    Error |   StdDev |     Gen0 |     Gen1 |     Gen2 | Allocated |
|-----------------------------|----------:|---------:|---------:|---------:|---------:|---------:|----------:|
| CreatingOneMillionCurrency  |  15.47 ms | 0.076 ms | 0.068 ms | 484.3750 | 484.3750 | 484.3750 |   1.91 MB |
| CreatingOneMillionMoney     | 114.52 ms | 0.659 ms | 0.584 ms | 600.0000 | 600.0000 | 600.0000 |  22.89 MB |
| CreatingOneMillionMoneyUnit |  50.27 ms | 0.294 ms | 0.275 ms | 545.4545 | 545.4545 | 545.4545 |  15.26 MB |

## Create CurrencyUNit
| Method                             |      Mean |     Error |    StdDev | Ratio |   Gen0 | Allocated | Alloc Ratio |
|------------------------------------|----------:|----------:|----------:|------:|-------:|----------:|------------:|
| CreateCurrencyUnit                 | 22.006 ns | 0.2475 ns | 0.2067 ns |  1.00 | 0.0134 |     112 B |        1.00 |
| CreateCurrencyUnitNoLinq           |  8.281 ns | 0.1674 ns | 0.1719 ns |  0.38 | 0.0076 |      64 B |        0.57 |
| CreateCurrencyUnitNoLinqAndPattern |  8.558 ns | 0.1967 ns | 0.2416 ns |  0.39 | 0.0076 |      64 B |        0.57 |
