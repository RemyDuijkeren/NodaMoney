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
| CurrencyFromCode     | 14.701 ns | 0.1100 ns | 0.1029 ns |         - |
| CurrencyInfoFromCode |  7.243 ns | 0.0779 ns | 0.0728 ns |         - |

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
| ExplicitCurrencyAsString            | 19.8637 ns | 0.2010 ns | 0.1881 ns |  1.00 |    0.01 |      - |         - |          NA |
| ExplicitCurrencyAsStringAndRounding | 22.1141 ns | 0.0951 ns | 0.0794 ns |  1.11 |    0.01 |      - |         - |          NA |
| ExplicitCurrencyFromCode            | 26.5903 ns | 0.3922 ns | 0.3668 ns |  1.34 |    0.02 |      - |         - |          NA |
| ExplicitCurrencyInfoFromCode        | 20.1313 ns | 0.2839 ns | 0.2655 ns |  1.01 |    0.02 |      - |         - |          NA |
| ExtensionMethod                     | 20.1133 ns | 0.1146 ns | 0.1016 ns |  1.01 |    0.01 |      - |         - |          NA |
| ImplicitCurrencyByConstructor       | 73.1049 ns | 0.5019 ns | 0.3918 ns |  3.68 |    0.04 | 0.0076 |      64 B |          NA |
| ImplicitCurrencyByCasting           | 74.4425 ns | 0.3662 ns | 0.3426 ns |  3.75 |    0.04 | 0.0076 |      64 B |          NA |
| Deconstruct                         |  0.2834 ns | 0.0104 ns | 0.0081 ns |  0.01 |    0.00 |      - |         - |          NA |

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
| Addition                 | 15.957 ns | 0.3332 ns | 0.3422 ns |      - |         - |
| Subtraction              | 15.655 ns | 0.2316 ns | 0.1934 ns |      - |         - |
| CompareSameCurrency      |  3.728 ns | 0.0934 ns | 0.0780 ns |      - |         - |
| CompareDifferentCurrency |  3.874 ns | 0.0720 ns | 0.0674 ns |      - |         - |
| CompareAmount            |  3.873 ns | 0.0441 ns | 0.0412 ns |      - |         - |
| Increment                | 87.604 ns | 0.8389 ns | 0.6550 ns | 0.0038 |      32 B |
| Decrement                | 88.755 ns | 0.4861 ns | 0.4059 ns | 0.0038 |      32 B |

## MoneyFormatting
#### before (v1.x)
| Method             |     Mean |   Error |   StdDev |  Gen 0 | Allocated |
|--------------------|---------:|--------:|---------:|-------:|----------:|
| Implicit           | 194.0 ns | 3.93 ns |  9.10 ns | 0.0286 |     240 B |
| ImplicitWithFormat | 197.6 ns | 3.98 ns |  8.39 ns | 0.0286 |     240 B |
| Explicit           | 271.5 ns | 5.50 ns |  9.33 ns | 0.0525 |     440 B |
| ExplicitWithFormat | 270.3 ns | 5.42 ns | 12.56 ns | 0.0525 |     440 B |
#### after
| Method             |     Mean |   Error |   StdDev |   Gen0 | Allocated |
|--------------------|---------:|--------:|---------:|-------:|----------:|
| Implicit           | 120.3 ns | 3.82 ns | 11.08 ns | 0.0467 |     392 B |
| ImplicitWithFormat | 136.9 ns | 2.59 ns |  2.42 ns | 0.0505 |     424 B |
| Explicit           | 106.9 ns | 2.17 ns |  3.37 ns | 0.0468 |     392 B |
| ExplicitWithFormat | 138.5 ns | 1.32 ns |  1.24 ns | 0.0505 |     424 B |

## MoneyParsing
#### before (v1.x)
| Method      |        Mean |     Error |    StdDev |      Median |  Gen 0 |  Gen 1 | Allocated |
|-------------|------------:|----------:|----------:|------------:|-------:|-------:|----------:|
| Implicit    | 32,037.0 ns | 597.82 ns | 587.14 ns | 31,817.6 ns | 3.0823 | 0.2136 |     25 KB |
| ImplicitTry | 32,111.5 ns | 631.83 ns | 945.70 ns | 32,079.3 ns | 3.0823 | 0.2136 |     25 KB |
| Explicit    |    877.5 ns |  21.39 ns |  62.73 ns |    860.9 ns | 0.1469 |      - |      1 KB |
| ExplicitTry |    870.2 ns |  16.94 ns |  21.42 ns |    871.8 ns | 0.1469 |      - |      1 KB |
#### after
| Method            |     Mean |   Error |   StdDev |   Gen0 | Allocated |
|-------------------|---------:|--------:|---------:|-------:|----------:|
| Implicit          | 427.1 ns | 8.50 ns | 15.33 ns | 0.1173 |     984 B |
| ImplicitTry       | 417.1 ns | 3.65 ns |  3.05 ns | 0.1173 |     984 B |
| Explicit          | 438.0 ns | 7.73 ns |  6.86 ns | 0.1173 |     984 B |
| ExplicitAsSpan    | 450.1 ns | 5.05 ns |  4.72 ns | 0.1173 |     984 B |
| ExplicitTry       | 440.1 ns | 5.94 ns |  5.56 ns | 0.1173 |     984 B |
| ExplicitTryAsSpan | 440.5 ns | 8.18 ns | 13.21 ns | 0.1173 |     984 B |

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

## HighLoad
#### before (v1.x)
| Method                     |     Mean |    Error |   StdDev |      Gen 0 | Allocated |
|----------------------------|---------:|---------:|---------:|-----------:|----------:|
| CreatingOneMillionCurrency | 482.6 ms |  9.57 ms | 26.68 ms | 75000.0000 |    679 MB |
| CreatingOneMillionMoney    | 516.8 ms | 10.33 ms | 28.46 ms | 75000.0000 |    694 MB |
#### after
| Method                     |     Mean |    Error |   StdDev |     Gen0 |     Gen1 |     Gen2 | Allocated |
|----------------------------|---------:|---------:|---------:|---------:|---------:|---------:|----------:|
| CreatingOneMillionCurrency | 13.48 ms | 0.228 ms | 0.213 ms | 484.3750 | 484.3750 | 484.3750 |   1.91 MB |
| CreatingOneMillionMoney    | 18.61 ms | 0.360 ms | 0.370 ms | 656.2500 | 656.2500 | 656.2500 |  22.89 MB |

## Create CurrencyUNit
| Method                             |      Mean |     Error |    StdDev | Ratio |   Gen0 | Allocated | Alloc Ratio |
|------------------------------------|----------:|----------:|----------:|------:|-------:|----------:|------------:|
| CreateCurrencyUnit                 | 22.006 ns | 0.2475 ns | 0.2067 ns |  1.00 | 0.0134 |     112 B |        1.00 |
| CreateCurrencyUnitNoLinq           |  8.281 ns | 0.1674 ns | 0.1719 ns |  0.38 | 0.0076 |      64 B |        0.57 |
| CreateCurrencyUnitNoLinqAndPattern |  8.558 ns | 0.1967 ns | 0.2416 ns |  0.39 | 0.0076 |      64 B |        0.57 |
